using ModengTerm.Base.DataModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal.Watch
{
    public abstract class UnixWatcher : AbstractWatcher
    {
        #region 类变量

        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger("");

        private List<int> CPU_TIME_INDEX = new List<int>() { 1, 2, 3, 6, 7, 8 };

        #endregion

        #region 实例变量

        private Stopwatch stopwatch;
        private int lastCputime;
        private SystemInfo systemInfo;
        protected XTermSession session;
        private UnixDiskCopy diskCopy;

        #endregion

        #region 构造方法

        public UnixWatcher(XTermSession session) :
            base(session)
        {
            this.session = session;
        }

        #endregion

        #region 实例方法

        private bool parse_meminfo_item(string[] items, string key, out int value)
        {
            value = 0;

            string text = items.FirstOrDefault(v => v.StartsWith(key));
            if (string.IsNullOrEmpty(text))
            {
                return false;
            }

            string[] strs = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (strs == null || strs.Length != 3)
            {
                logger.ErrorFormat("mem error = {0}", text);
                return false;
            }

            if (!int.TryParse(strs[1], out value))
            {
                logger.ErrorFormat("mem error2 = {0}", text);
                return false;
            }

            return true;
        }

        private bool parse_cpu(string cpu, out int totaltime)
        {
            totaltime = 0;

            if (string.IsNullOrEmpty(cpu))
            {
                return false;
            }

            string[] strs = cpu.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (strs.Length < 9)
            {
                logger.ErrorFormat("cpu error = {0}", cpu);
                return false;
            }

            foreach (int index in CPU_TIME_INDEX)
            {
                int v;
                if (!int.TryParse(strs[index], out v))
                {
                    logger.ErrorFormat("cpu error2 = {0}", cpu);
                    return false;
                }

                totaltime += v;
            }

            return true;
        }

        private void handle_df(string df) 
        {
            if (string.IsNullOrEmpty(df)) 
            {
                return;
            }

            string[] lines = df.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            if (lines == null || lines.Length == 0) 
            {
                return;
            }

            IEnumerable<string[]> strslist = lines.Select(v => v.Split(' ', StringSplitOptions.RemoveEmptyEntries)).Skip(1);
            this.Copy<string[], DiskInfo>(this.systemInfo.DiskItems, strslist, this.diskCopy);
        }

        #endregion

        #region AbstractWatcher

        public override void Initialize()
        {
            this.systemInfo = new SystemInfo();
            this.stopwatch = new Stopwatch();
            this.diskCopy = new UnixDiskCopy();
        }

        public override void Release()
        {

        }

        public override SystemInfo GetSystemInfo()
        {
            #region 读取内存信息

            string meminfo = this.ReadFile("/proc/meminfo");
            string[] items = meminfo.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            int totalMem;
            if (this.parse_meminfo_item(items, "MemTotal", out totalMem))
            {
                this.systemInfo.TotalMemory = totalMem;
            }

            int availableMem;
            if (this.parse_meminfo_item(items, "MemAvailable", out availableMem))
            {
                this.systemInfo.AvailableMemory = availableMem;
            }

            #endregion

            #region 读取CPU信息

            int cputime;
            string cpuinfo = this.ReadFile("/proc/stat");
            string[] cpuitems = cpuinfo.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            string cpu = cpuitems.FirstOrDefault(v => v.Contains("cpu "));
            if (this.parse_cpu(cpu, out cputime))
            {
                TimeSpan elapsed = this.stopwatch.Elapsed;
                if (elapsed.TotalMilliseconds > 0 && this.lastCputime > 0)
                {
                    int diff = cputime - this.lastCputime;
                    this.systemInfo.CpuPercent = Math.Round(diff / elapsed.TotalMilliseconds * 100, 2);
                }

                this.lastCputime = cputime;
            }
            this.stopwatch.Restart();

            #endregion

            #region 读取磁盘信息

            string df = this.Execute("df");
            this.handle_df(df);

            #endregion

            return this.systemInfo;
        }

        #endregion

        #region 抽象方法

        /// <summary>
        /// 读取指定目录下的文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns>如果读取失败则返回string.Empty</returns>
        protected abstract string ReadFile(string filePath);

        /// <summary>
        /// 执行一个命令并把命令输出返回
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        protected abstract string Execute(string command);

        #endregion
    }
}

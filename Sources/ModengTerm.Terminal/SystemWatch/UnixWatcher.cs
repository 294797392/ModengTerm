using ModengTerm.Base;
using ModengTerm.Base.DataModels;
using ModengTerm.Terminal.Engines;

namespace ModengTerm.Terminal.Watch
{
    public abstract class UnixWatcher : AbstractWatcher
    {
        #region 类变量

        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger("UnixWatcher");

        private static readonly List<int> CPU_USER_TIME_INDEX = new List<int>() { 1, 2 };
        private static readonly List<int> CPU_KERNEL_TIME_INDEX = new List<int>() { 3, 5, 6, 7, 8 };
        private static readonly List<int> CPU_IDLE_TIME_INDEX = new List<int>() { 4 };

        private static readonly string FetchNetworkInterfaces = "interfaces=$(ifconfig | grep -o '^[a-zA-Z0-9]\\+:' | tr -d ':');for interface in $interfaces; do ip_address=$(ifconfig \"$interface\" | grep -o 'inet [0-9.]\\+' | awk '{print $2}');receive_bytes=$(ifconfig \"$interface\" | grep 'RX packets' | awk '{print $5}');transmit_bytes=$(ifconfig \"$interface\" | grep 'TX packets' | awk '{print $5}');echo $interface,$ip_address,$receive_bytes,$transmit_bytes;done;";
        private static readonly string FetchProcess = "for pid in $(ls -d /proc/[0-9]* 2>/dev/null); do pid=${pid##*/};name=$(grep '^Name:' \"/proc/$pid/status\" | awk '{print $2}');rss=$(grep '^VmRSS:' \"/proc/$pid/status\" | awk '{print $2}');stat=($(awk '{print $14, $15}' \"/proc/$pid/stat\"));utime=${stat[0]};ktime=${stat[1]};echo $pid,$name,$rss,$utime,$ktime;done;";

        #endregion

        #region 实例变量

        private int lastCputime;
        private SystemInfo systemInfo;
        protected XTermSession session;
        private UnixDiskCopy diskCopy;
        private UnixNetDeviceCopy netDeviceCopy;
        private UnixProcessCopy processCopy;

        #endregion

        #region 构造方法

        public UnixWatcher(XTermSession session, AbstractEngin driver) :
            base(session, driver)
        {
            this.session = session;
        }

        #endregion

        #region 实例方法

        private bool parse_meminfo_item(string[] items, string key, out ulong value)
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

            if (!ulong.TryParse(strs[1], out value))
            {
                logger.ErrorFormat("mem error2 = {0}", text);
                return false;
            }

            return true;
        }

        private bool calc_cpu_time(string[] strs, List<int> indexs, out ulong cpu_time)
        {
            cpu_time = 0;

            foreach (int index in indexs)
            {
                ulong v;
                if (!ulong.TryParse(strs[index], out v))
                {
                    return false;
                }

                cpu_time += v;
            }

            return true;
        }

        private bool handle_cpu(string cpu)
        {
            //        
            // cpu  1661515 33740 1757851 27139465 233 0 387078 0 0 0

            if (string.IsNullOrEmpty(cpu))
            {
                logger.ErrorFormat("cpu empty");
                return false;
            }

            string[] strs = cpu.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (strs.Length < 9)
            {
                logger.ErrorFormat("cpu error = {0}", cpu);
                return false;
            }

            // 用户时间
            ulong user_time;
            if (!this.calc_cpu_time(strs, CPU_USER_TIME_INDEX, out user_time))
            {
                logger.ErrorFormat("cpu user time error = {0}", cpu);
            }
            else
            {
                this.systemInfo.UserProcessorTime = user_time;
            }

            // 内核时间
            ulong kernel_time;
            if (!this.calc_cpu_time(strs, CPU_KERNEL_TIME_INDEX, out kernel_time))
            {
                logger.ErrorFormat("cpu kernel time error = {0}", cpu);
            }
            else
            {
                this.systemInfo.KernelProcessorTime = kernel_time;
            }

            // 空闲时间
            ulong idle_time;
            if (!this.calc_cpu_time(strs, CPU_IDLE_TIME_INDEX, out idle_time))
            {
                logger.ErrorFormat("cpu idle time error = {0}", cpu);
            }
            else
            {
                this.systemInfo.IdleProcessorTime = idle_time;
            }

            return true;
        }

        private void handle_df(string df)
        {
            if (string.IsNullOrEmpty(df))
            {
                return;
            }

            IEnumerable<string> lines = df.Split('\n', StringSplitOptions.RemoveEmptyEntries).Skip(1);
            if (lines == null)
            {
                return;
            }

            this.Copy<string, VTDrive>(this.systemInfo.DiskItems, lines, this.diskCopy);
        }

        private void handle_netdev(string netdev)
        {
            if (string.IsNullOrEmpty(netdev))
            {
                return;
            }

            IEnumerable<string> lines = netdev.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            if (lines == null)
            {
                return;
            }

            this.Copy<string, VTNetDevice>(this.systemInfo.NetDevices, lines, this.netDeviceCopy);
        }

        private void handle_process(string process) 
        {
            if (string.IsNullOrEmpty(process))
            {
                return;
            }

            IEnumerable<string> lines = process.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            if (lines == null)
            {
                return;
            }

            this.Copy<string, VTProcess>(this.systemInfo.Processes, lines, this.processCopy);
        }

        #endregion

        #region AbstractWatcher

        public override void Initialize()
        {
            this.systemInfo = new SystemInfo();
            this.diskCopy = new UnixDiskCopy();
            this.netDeviceCopy = new UnixNetDeviceCopy();
            this.processCopy = new UnixProcessCopy();
        }

        public override void Release()
        {

        }

        public override SystemInfo GetSystemInfo()
        {
            #region 读取内存信息

            string meminfo = this.ReadFile("/proc/meminfo");
            string[] items = meminfo.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            ulong totalMem;
            if (this.parse_meminfo_item(items, "MemTotal", out totalMem))
            {
                this.systemInfo.TotalMemory.Value = totalMem;
                this.systemInfo.TotalMemory.Unit = UnitType.KB;
            }

            ulong availableMem;
            if (this.parse_meminfo_item(items, "MemAvailable", out availableMem))
            {
                this.systemInfo.AvailableMemory.Value = availableMem;
                this.systemInfo.AvailableMemory.Unit = UnitType.KB;
            }

            #endregion

            #region 读取CPU信息

            string cpuinfo = this.ReadFile("/proc/stat");
            string[] cpuitems = cpuinfo.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            string cpu = cpuitems.FirstOrDefault(v => v.Contains("cpu "));
            this.handle_cpu(cpu);

            #endregion

            #region 读取磁盘信息

            string df = this.Execute("df");
            this.handle_df(df);

            #endregion

            #region 读取网络接口信息

            string netdev = this.Execute(FetchNetworkInterfaces);
            this.handle_netdev(netdev);

            #endregion

            #region 读取进程信息

            string process = this.Execute(FetchProcess);
            this.handle_process(process);

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

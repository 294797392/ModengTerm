using ModengTerm.Base.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal.Watch
{
    public abstract class UnixWatcher : AbstractWatcher
    {
        #region 类变量

        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger("");

        #endregion

        #region 实例变量

        protected XTermSession session;

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
                logger.Error(text);
                return false;
            }

            if (!int.TryParse(strs[1], out value)) 
            {
                logger.Error(text);
                return false;
            }

            return true;
        }

        #endregion

        #region AbstractWatcher

        public override SystemInfo GetSystemInfo()
        {
            string meminfo = this.proc_meminfo();
            string[] items = meminfo.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            int totalMem;
            int freeMem;
            if (!this.parse_meminfo_item(items, "MemTotal", out totalMem) || 
                !this.parse_meminfo_item(items, "MemAvailable", out freeMem)) 
            {
                return null;
            }

            int memUsage = totalMem - freeMem;
            double memUsagePercent = Math.Round((double)memUsage / totalMem, 2);

            return new SystemInfo();
        }

        #endregion

        #region 抽象方法

        public abstract string proc_meminfo();
        public abstract string proc_stat();
        public abstract string df_h();

        #endregion
    }
}

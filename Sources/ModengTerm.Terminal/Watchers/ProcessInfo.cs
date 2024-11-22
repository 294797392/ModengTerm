using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal.Watchers
{
    /// <summary>
    /// 保存进程信息
    /// </summary>
    public class ProcessInfo
    {
        public int PID { get; set; }

        public string Name { get; set; }

        public string FilePath { get; set; }

        public double CpuUsage { get; set; }

        public ProcessInfo()
        {
        }

        /// <summary>
        /// 判断和procInfo是否一致
        /// </summary>
        /// <param name="procInfo"></param>
        /// <returns></returns>
        public bool CompareTo(ProcessInfo procInfo)
        {
            if (procInfo.PID != this.PID)
            {
                return false;
            }

            if (procInfo.Name != this.Name)
            {
                return false;
            }

            if (procInfo.FilePath != this.FilePath)
            {
                return false;
            }

            if (procInfo.CpuUsage != this.CpuUsage)
            {
                return false;
            }

            return true;
        }
    }
}

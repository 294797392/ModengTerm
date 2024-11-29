using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal.Watch
{
    public class SystemInfo
    {
        /// <summary>
        /// 百分比
        /// </summary>
        public double CpuUsage { get; set; }

        /// <summary>
        /// 总内存数
        /// 单位byte
        /// </summary>
        public double TotalMemory { get; set; }

        /// <summary>
        /// 单位byte
        /// </summary>
        public double MemoryUsage { get; set; }
    }
}

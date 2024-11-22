using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal.Watchers
{
    /// <summary>
    /// 保存系统信息
    /// </summary>
    public class SystemInfo
    {
        /// <summary>
        /// CPU占用率
        /// </summary>
        public double CpuUsage { get; set; }

        /// <summary>
        /// 内存使用率
        /// </summary>
        public double MemoryUsage { get; set; }
    }
}

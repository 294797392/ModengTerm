using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal.Watchers
{
    public enum WatchKinds
    {
        /// <summary>
        /// 监控进程列表
        /// </summary>
        ProcessInfo,

        /// <summary>
        /// 监控磁盘信息
        /// </summary>
        DiskInfo,

        /// <summary>
        /// 监控系统信息（CPU，内存等等）
        /// </summary>
        SystemInfo,
    }
}

using DotNEToolkit.DataModels;
using ModengTerm.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal.Watch
{
    public class SystemInfo
    {
        /// <summary>
        /// CPU使用百分比
        /// </summary>
        public double CpuPercent { get; set; }

        /// <summary>
        /// 总内存数
        /// </summary>
        public UnitValue64 TotalMemory { get; private set; }

        /// <summary>
        /// 可用内存数
        /// 单位kb
        /// </summary>
        public UnitValue64 AvailableMemory { get; private set; }

        /// <summary>
        /// 所有磁盘列表
        /// </summary>
        public ChangedItems<DiskInfo> DiskItems { get; private set; }

        /// <summary>
        /// 网路接口列表
        /// </summary>
        public ChangedItems<NetInterfaceInfo> NetworkInterfaces { get; private set; }

        /// <summary>
        /// 所有进程列表
        /// </summary>
        public ChangedItems<ProcessInfo> Processes { get; private set; }

        public SystemInfo()
        {
            this.TotalMemory = new UnitValue64();
            this.AvailableMemory = new UnitValue64();
            this.DiskItems = new ChangedItems<DiskInfo>();
            this.NetworkInterfaces = new ChangedItems<NetInterfaceInfo>();
            this.Processes = new ChangedItems<ProcessInfo>();
        }
    }
}

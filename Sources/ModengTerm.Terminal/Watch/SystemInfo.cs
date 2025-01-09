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
        private ulong kerneProcessorTime;
        private ulong userProcessorTime;
        private ulong idleProcessorTime;

        /// <summary>
        /// 内核使用的cpu总时间
        /// 所有核心加起来的时间
        /// </summary>
        public ulong KernelProcessorTime 
        {
            get { return this.kerneProcessorTime; }
            set
            {
                if (this.kerneProcessorTime != value)
                {
                    this.kerneProcessorTime = value;
                }
            }
        }

        /// <summary>
        /// 用户模式使用的cpu总时间
        /// 所有核心加起来的时间
        /// </summary>
        public ulong UserProcessorTime 
        {
            get { return this.userProcessorTime; }
            set
            {
                if (this.userProcessorTime != value)
                {
                    this.userProcessorTime = value;
                }
            }
        }

        /// <summary>
        /// cpu总空闲时间
        /// 所有核心加起来的时间
        /// </summary>
        public ulong IdleProcessorTime 
        {
            get { return this.idleProcessorTime; }
            set
            {
                if (this.idleProcessorTime != value)
                {
                    this.idleProcessorTime = value;
                }
            }
        }

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
        public ChangedItems<VTDrive> DiskItems { get; private set; }

        /// <summary>
        /// 网路接口列表
        /// </summary>
        public ChangedItems<VTNetDevice> NetDevices { get; private set; }

        /// <summary>
        /// 所有进程列表
        /// </summary>
        public ChangedItems<ProcessInfo> Processes { get; private set; }

        public SystemInfo()
        {
            this.TotalMemory = new UnitValue64();
            this.AvailableMemory = new UnitValue64();
            this.DiskItems = new ChangedItems<VTDrive>();
            this.NetDevices = new ChangedItems<VTNetDevice>();
            this.Processes = new ChangedItems<ProcessInfo>();
        }
    }
}

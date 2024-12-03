using ModengTerm.Enumerations;
using ModengTerm.Terminal.Watch;
using System;
using System.Collections.Generic;
using System.Linq;
using WPFToolkit.MVVM;

namespace ModengTerm.ViewModels.Terminals.PanelContent
{
    public class WatchSystemInfo : WatchObject
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("WatchSystemInfo");

        #endregion

        #region 实例变量

        private double cpuPercent;
        private double memoryPercent;
        private string memoryRatio;
        private DiskVMCopy diskCopy;
        private NetworkInterfaceVMCopy ifaceCopy;
        private ProcessVMCopy processCopy;

        #endregion

        #region 属性

        /// <summary>
        /// CPU占用百分比
        /// </summary>
        public double CpuPercent
        {
            get { return this.cpuPercent; }
            set
            {
                if (this.cpuPercent != value)
                {
                    this.cpuPercent = value;
                    this.NotifyPropertyChanged("CpuPercent");
                }
            }
        }

        /// <summary>
        /// 内存占用百分比
        /// </summary>
        public double MemoryPercent
        {
            get { return this.memoryPercent; }
            set
            {
                if (this.memoryPercent != value)
                {
                    this.memoryPercent = value;
                    this.NotifyPropertyChanged("MemoryPercent");
                }
            }
        }

        /// <summary>
        /// 内存总量
        /// </summary>
        public UnitValue TotalMemory { get; private set; }

        /// <summary>
        /// 已用内存总量
        /// </summary>
        public UnitValue UsedMemory { get; private set; }

        /// <summary>
        /// 可用内存总量
        /// </summary>
        public UnitValue AvailableMemory { get; private set; }

        public string MemoryRatio
        {
            get { return this.memoryRatio; }
            set
            {
                if ((this.memoryRatio != value))
                {
                    this.memoryRatio = value;
                    this.NotifyPropertyChanged("MemoryRatio");
                }
            }
        }

        /// <summary>
        /// 所有磁盘信息
        /// </summary>
        public BindableCollection<DiskVM> Disks { get; private set; }

        /// <summary>
        /// 所有网络接口信息
        /// </summary>
        public BindableCollection<NetworkInterfaceVM> NetworkInterfaces { get; private set; }

        /// <summary>
        /// 所有进程列表
        /// </summary>
        public BindableCollection<ProcessVM> Processes { get; private set; }

        #endregion

        #region WatchObject

        public override void OnInitialize()
        {
            base.OnInitialize();

            this.Disks = new BindableCollection<DiskVM>();
            this.NetworkInterfaces = new BindableCollection<NetworkInterfaceVM>();
            this.TotalMemory = new UnitValue();
            this.UsedMemory = new UnitValue();
            this.AvailableMemory = new UnitValue();
            this.Processes = new BindableCollection<ProcessVM>();

            this.diskCopy = new DiskVMCopy();
            this.ifaceCopy = new NetworkInterfaceVMCopy();
            this.processCopy = new ProcessVMCopy();
        }

        public override void OnRelease()
        {
            base.OnRelease();
        }

        public override void Watch(AbstractWatcher watcher)
        {
            SystemInfo systemInfo = watcher.GetSystemInfo();

            this.CpuPercent = Math.Round(systemInfo.CpuPercent, 2);
            ClientUtils.UpdateUnitValue(this.AvailableMemory, systemInfo.AvailableMemory, SizeUnitsEnum.KB);
            ClientUtils.UpdateUnitValue(this.TotalMemory, systemInfo.TotalMemory, SizeUnitsEnum.KB);
            ClientUtils.UpdateUnitValue(this.UsedMemory, systemInfo.TotalMemory - systemInfo.AvailableMemory, SizeUnitsEnum.KB);
            this.MemoryPercent = Math.Round((double)this.UsedMemory.Bytes / this.TotalMemory.Bytes * 100, 2);
            this.MemoryRatio = string.Format("{0}/{1}", this.UsedMemory, this.TotalMemory);

            // 更新磁盘信息
            this.Copy<DiskVM, DiskInfo>(systemInfo.DiskItems, this.Disks, this.diskCopy);

            // 更新网络接口信息
            this.Copy<NetworkInterfaceVM, NetInterfaceInfo>(systemInfo.NetworkInterfaces, this.NetworkInterfaces, this.ifaceCopy);

            // 更新进程信息
            this.Copy<ProcessVM, ProcessInfo>(systemInfo.Processes, this.Processes, this.processCopy);
            // 按照CPU使用率对进程列表排序
            List<ProcessVM> orderedProcs = this.Processes.OrderByDescending(v => v.CpuUsage).ToList();
            App.Current.Dispatcher.Invoke(() => 
            {
                this.Processes.Clear();
                this.Processes.AddRange(orderedProcs);
            });
        }

        #endregion

        #region 实例方法

        private void Copy<Target, TSource>(ChangedItems<TSource> copyTo, IList<Target> copyFrom, ObjectCopy<Target, TSource> copy)
            where Target : class, new()
            where TSource : class
        {
            IList<TSource> items = copyTo.Items;
            IList<TSource> addItems = copyTo.AddItems;
            IList<TSource> removeItems = copyTo.RemoveItems;

            if (addItems.Count > 0)
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    foreach (TSource add in addItems)
                    {
                        Target viewModel = new Target();
                        copy.CopyTo(viewModel, add);
                        copyFrom.Add(viewModel);
                    }
                });
            }

            if (removeItems.Count > 0)
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    foreach (TSource remove in removeItems)
                    {
                        Target toRemove = null;

                        foreach (Target target in copyFrom)
                        {
                            if (copy.Compare(target, remove))
                            {
                                toRemove = target;
                                break;
                            }
                        }

                        if (toRemove != null)
                        {
                            copyFrom.Remove(toRemove);
                        }
                    }
                });
            }

            foreach (Target viewModel in copyFrom)
            {
                TSource toUpdate = null;

                foreach (TSource source in items)
                {
                    if (copy.Compare(viewModel, source))
                    {
                        toUpdate = source;
                        break;
                    }
                }

                if (toUpdate != null)
                {
                    copy.CopyTo(viewModel, toUpdate);
                }
            }

            // 所有数据都刷新完了，重置计时器
            copy.Stopwatch.Restart();
        }

        #endregion
    }
}

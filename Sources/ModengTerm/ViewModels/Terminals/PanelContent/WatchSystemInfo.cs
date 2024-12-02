using ModengTerm.Enumerations;
using ModengTerm.Terminal.Watch;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using WPFToolkit.MVVM;

namespace ModengTerm.ViewModels.Terminals.PanelContent
{
    public class WatchSystemInfo : WatchVM
    {
        #region 类变量

        private static readonly DiskVMCopy DiskVMSync = new DiskVMCopy();
        private static readonly NetworkInterfaceVMCopy NetworkInterfaceVMCopy = new NetworkInterfaceVMCopy();
        private static log4net.ILog logger = log4net.LogManager.GetLogger("WatchSystemInfo");

        #endregion

        #region 实例变量

        private double cpuPercent;
        private double memoryPercent;
        private string memoryRatio;
        private Stopwatch speedWatch;

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

        #endregion

        #region WatchVM

        public override void OnInitialize()
        {
            base.OnInitialize();

            this.Disks = new BindableCollection<DiskVM>();
            this.NetworkInterfaces = new BindableCollection<NetworkInterfaceVM>();
            this.TotalMemory = new UnitValue();
            this.UsedMemory = new UnitValue();
            this.AvailableMemory = new UnitValue();
            this.speedWatch = new Stopwatch();
        }

        public override void Watch(AbstractWatcher watcher)
        {
            SystemInfo systemInfo = watcher.GetSystemInfo();

            this.CpuPercent = Math.Round(systemInfo.CpuPercent, 2);
            ClientUtils.UpdateSpaceSize(this.AvailableMemory, systemInfo.AvailableMemory, SizeUnitsEnum.KB);
            ClientUtils.UpdateSpaceSize(this.TotalMemory, systemInfo.TotalMemory, SizeUnitsEnum.KB);
            ClientUtils.UpdateSpaceSize(this.UsedMemory, systemInfo.TotalMemory - systemInfo.AvailableMemory, SizeUnitsEnum.KB);
            this.MemoryPercent = Math.Round((double)this.UsedMemory.Bytes / this.TotalMemory.Bytes * 100, 2);
            this.MemoryRatio = string.Format("{0}/{1}", this.UsedMemory, this.TotalMemory);

            // 更新磁盘信息
            this.Copy<DiskVM, DiskInfo>(systemInfo.DiskItems, this.Disks, DiskVMSync);

            // 更新网络接口信息
            this.Copy<NetworkInterfaceVM, NetInterfaceInfo>(systemInfo.NetworkInterfaces, this.NetworkInterfaces, NetworkInterfaceVMCopy);

            // 更新网络接口实时速度
            // 上次到这次的总运行时间
            double seconds = this.speedWatch.Elapsed.Seconds;
            if (seconds > 0)
            {
                foreach (NetworkInterfaceVM interfaze in this.NetworkInterfaces)
                {
                    ulong sent = interfaze.BytesSent - interfaze.PreviousBytesSent;
                    ulong received = interfaze.BytesReceived - interfaze.PreviousBytesReceived;

                    // 单位是字节
                    double upspeedBytes = sent / seconds;
                    double downspeedBytes = received / seconds;

                    SizeUnitsEnum upspeedUnit, downspeedUnit;
                    int upspeed = (int)ClientUtils.ConvertToHumanReadableUnit(upspeedBytes, out upspeedUnit, 1);
                    int downspeed = (int)ClientUtils.ConvertToHumanReadableUnit(downspeedBytes, out downspeedUnit, 1);

                    interfaze.UploadSpeed = string.Format("{0}{1}/s", upspeed, ClientUtils.Unit2Suffix(upspeedUnit));
                    interfaze.DownloadSpeed = string.Format("{0}{1}/s", downspeed, ClientUtils.Unit2Suffix(downspeedUnit));
                }
            }
            this.speedWatch.Restart();
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
        }

        #endregion
    }
}

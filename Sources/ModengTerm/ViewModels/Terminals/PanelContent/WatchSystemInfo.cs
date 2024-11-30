using ModengTerm.Enumerations;
using ModengTerm.Terminal.Watch;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using WPFToolkit.MVVM;

namespace ModengTerm.ViewModels.Terminals.PanelContent
{
    //public class UpdatableVM : ViewModelBase
    //{ 
    //}

    //public class Updater<TViewModel, TDataModel>
    //{ 
    //    public IList<TViewModel> ViewModels { get; set; }

    //    public IList<TDataModel> DataModels { get; set; }

    //    public IList<TDataModel> RemoveItems { get; set; }

    //    public IList<TDataModel> AddItems { get; set; }

    //    public void Update() 
    //    {
    //        if (this.AddItems.Count > 0) 
    //        {
    //            foreach (TDataModel dataModel in this.AddItems)
    //            {
    //            }
    //        }
    //    }
    //}

    public class WatchSystemInfo : WatchVM
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("WatchSystemInfo");

        #endregion

        #region 实例变量

        private double cpuPercent;
        private double memoryPercent;
        private string memoryRatio;

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

            #region 更新磁盘信息

            if (systemInfo.AddDisks.Count > 0)
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    foreach (DiskInfo diskInfo in systemInfo.AddDisks)
                    {
                        DiskVM diskVM = new DiskVM(diskInfo);
                        this.Disks.Add(diskVM);
                    }
                });
            }

            if (systemInfo.RemoveDisks.Count > 0)
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    foreach (DiskInfo removeDisk in systemInfo.RemoveDisks)
                    {
                        DiskVM toRemove = this.Disks.FirstOrDefault(v => v.Name == removeDisk.Name);
                        if (toRemove != null)
                        {
                            this.Disks.Remove(toRemove);
                        }
                    }
                });
            }

            foreach (DiskVM diskVM in this.Disks)
            {
                DiskInfo diskInfo = systemInfo.DiskInfos.FirstOrDefault(v => v.Name == diskVM.Name);
                if (diskInfo != null)
                {
                    diskVM.Update(diskInfo);
                }
            }

            #endregion
        }

        #endregion

        #region 实例方法

        #endregion
    }
}

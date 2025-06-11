using ModengTerm.Base;
using ModengTerm.Terminal.Watch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using WPFToolkit.MVVM;
using ModengTerm.Base.Enumerations;
using ModengTerm.ViewModel;
using ModengTerm.ViewModel.Terminal;

namespace ModengTerm.Addon.Official.SystemMonitor
{
    public class WatchSystemInfo : SessionPanelContentVM
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("WatchSystemInfo");

        #endregion

        #region 实例变量

        private Task watchTask;
        private bool isWatch;
        private ManualResetEvent watchEvent;
        private ShellSessionVM shellSession;

        private double cpuPercent;
        private double memoryPercent;
        private string displayMemoryUsage;
        private DiskVMCopy diskCopy;
        private NetworkInterfaceVMCopy ifaceCopy;
        private ProcessVMCopy processCopy;

        private ulong prevKernelProcessorTime;
        private ulong prevUserProcessorTime;
        private ulong prevIdleProcessorTime;

        #endregion

        #region 属性

        /// <summary>
        /// CPU占用百分比
        /// 计算公式：
        /// （CPU内核时间 + CPU用户时间） / （CPU内核时间 + CPU用户时间 + CPU空闲时间）
        /// 参考：
        /// https://blog.csdn.net/zxf347085420/article/details/137209188
        /// </summary>
        public double CpuPercent
        {
            get { return cpuPercent; }
            set
            {
                if (cpuPercent != value)
                {
                    cpuPercent = value;
                    NotifyPropertyChanged("CpuPercent");
                }
            }
        }

        /// <summary>
        /// 内存占用百分比
        /// </summary>
        public double MemoryPercent
        {
            get { return memoryPercent; }
            set
            {
                if (memoryPercent != value)
                {
                    memoryPercent = value;
                    NotifyPropertyChanged("MemoryPercent");
                }
            }
        }

        /// <summary>
        /// 内存总量
        /// </summary>
        public UnitValueDouble TotalMemory { get; private set; }

        /// <summary>
        /// 已用内存总量
        /// </summary>
        public UnitValueDouble UsedMemory { get; private set; }

        /// <summary>
        /// 可用内存总量
        /// </summary>
        public UnitValueDouble AvailableMemory { get; private set; }

        public string DisplayMemoryUsage
        {
            get { return displayMemoryUsage; }
            set
            {
                if (displayMemoryUsage != value)
                {
                    displayMemoryUsage = value;
                    NotifyPropertyChanged("DisplayMemoryUsage");
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

        #region SessionPanelContentVM

        public override void OnInitialize()
        {
            base.OnInitialize();

            Disks = new BindableCollection<DiskVM>();
            NetworkInterfaces = new BindableCollection<NetworkInterfaceVM>();
            TotalMemory = new UnitValueDouble();
            UsedMemory = new UnitValueDouble();
            AvailableMemory = new UnitValueDouble();
            Processes = new BindableCollection<ProcessVM>();

            diskCopy = new DiskVMCopy();
            ifaceCopy = new NetworkInterfaceVMCopy();
            processCopy = new ProcessVMCopy();

            shellSession = OpenedSessionVM as ShellSessionVM;
            watchEvent = new ManualResetEvent(false);
        }

        public override void OnRelease()
        {
            StopWatch();

            watchEvent.Close();

            base.OnRelease();
        }

        public override void OnLoaded()
        {
            base.OnLoaded();

            watchEvent.Set();
        }

        public override void OnUnload()
        {
            watchEvent.Reset();

            base.OnUnload();
        }

        public override void OnReady()
        {
            StartWatch();
        }

        #endregion

        #region 实例方法

        private void StartWatch()
        {
            if (isWatch)
            {
                return;
            }

            isWatch = true;
            watchTask = Task.Factory.StartNew(WatchTaskProc);
            watchEvent.Set();
        }

        private void StopWatch()
        {
            isWatch = false;
            watchEvent.Set();
        }

        private void Watch(AbstractWatcher watcher)
        {
            SystemInfo systemInfo = watcher.GetSystemInfo();

            #region 计算CPU使用率

            ulong totalProcessorTime = 0;

            // 确保至少读取了一次CPU占用信息
            if (prevKernelProcessorTime > 0)
            {
                ulong idleTime = systemInfo.IdleProcessorTime - prevIdleProcessorTime;
                ulong kernelTime = systemInfo.KernelProcessorTime - prevKernelProcessorTime;
                ulong userTime = systemInfo.UserProcessorTime - prevUserProcessorTime;
                ulong totalTime = idleTime + kernelTime + userTime;
                totalProcessorTime = kernelTime + userTime;
                CpuPercent = Math.Round((double)totalProcessorTime / totalTime * 100, 2);
                if (cpuPercent > 100)
                {
                    logger.FatalFormat("3. {0}, totalProcessorTime = {1}, totalTime = {2}, kernelTime = {3}, userTime = {4}, idleTime = {4}, prevIdleProcessorTime = {5}", CpuPercent, totalProcessorTime, totalTime, kernelTime, userTime, idleTime, prevIdleProcessorTime);
                }
            }

            prevIdleProcessorTime = systemInfo.IdleProcessorTime;
            prevKernelProcessorTime = systemInfo.KernelProcessorTime;
            prevUserProcessorTime = systemInfo.UserProcessorTime;

            #endregion

            VTBaseUtils.UpdateReadable(AvailableMemory, systemInfo.AvailableMemory);
            VTBaseUtils.UpdateReadable(TotalMemory, systemInfo.TotalMemory);
            ulong used = systemInfo.TotalMemory.Value - systemInfo.AvailableMemory.Value;
            VTBaseUtils.UpdateReadable(UsedMemory, TotalMemory.Unit, used, systemInfo.TotalMemory.Unit);
            MemoryPercent = Math.Round((double)used / systemInfo.TotalMemory.Value * 100, 2);
            DisplayMemoryUsage = string.Format("{0}/{1}", UsedMemory, TotalMemory);

            // 更新磁盘信息
            Copy(systemInfo.DiskItems, Disks, diskCopy);

            // 更新网络接口信息
            Copy(systemInfo.NetDevices, NetworkInterfaces, ifaceCopy);

            // 更新进程信息
            processCopy.TotalProcessorTime = totalProcessorTime;
            Copy(systemInfo.Processes, Processes, processCopy);
            // 按照CPU使用率对进程列表排序
            List<ProcessVM> orderedProcs = Processes.OrderByDescending(v => v.CpuUsage).ToList();
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                Processes.Clear();
                Processes.AddRange(orderedProcs);
            });
        }

        private void Copy<Target, TSource>(ChangedItems<TSource> copyTo, IList<Target> copyFrom, ObjectCopy<Target, TSource> copy)
            where Target : class, new()
            where TSource : class
        {
            IList<TSource> items = copyTo.Items;
            IList<TSource> addItems = copyTo.AddItems;
            IList<TSource> removeItems = copyTo.RemoveItems;

            if (addItems.Count > 0)
            {
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
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
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
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

        #region 事件处理器

        private void WatchTaskProc()
        {
            WatchFrequencyEnum frequency = Session.GetOption(OptionKeyEnum.WATCH_FREQUENCY, OptionDefaultValues.WATCH_FREQUENCY);
            int updateInterval = VTBaseUtils.GetWatchInterval(frequency);
            AbstractWatcher watcher = WatcherFactory.Create(shellSession.Transport);
            watcher.Initialize();

            while (isWatch)
            {
                try
                {
                    watchEvent.WaitOne();

                    Watch(watcher);
                }
                catch (Exception ex)
                {
                    logger.Error("WatchThread异常", ex);
                    break;
                }

                Thread.Sleep(updateInterval);
            }

            watcher.Release();
            isWatch = false;
        }

        #endregion
    }
}

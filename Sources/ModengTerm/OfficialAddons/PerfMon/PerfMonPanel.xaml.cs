using DotNEToolkit.Utility;
using ModengTerm.Addon.Controls;
using ModengTerm.Base;
using ModengTerm.Base.Enumerations;
using ModengTerm.Terminal.Watch;
using ModengTerm.ViewModel.Terminal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPFToolkit.MVVM;

namespace ModengTerm.OfficialAddons.PerfMon
{
    /// <summary>
    /// PerfMonPanel.xaml 的交互逻辑
    /// </summary>
    public partial class PerfMonPanel : TabedSidePanel
    {
        /// <summary>
        /// 定义内存大小的单位
        /// </summary>
        private enum UnitType
        {
            bytes,
            KB,
            MB,
            GB,
            TB
        }

        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("PerfMonPanel");

        #endregion

        #region 实例变量

        private TimerHandle timerHandle;
        private DateTime lastUpdateTime = DateTime.MinValue;

        // viewModels
        private BindableCollection<DiskVM> disks;
        private BindableCollection<NetDeviceVM> netDevices;
        private BindableCollection<ProcessVM> processes;
        private CpuMemVM cpuMem;

        // windows使用
        private int memstatSize;
        private PerformanceCounter cpuUsagePerf;

        #endregion

        #region 构造方法

        public PerfMonPanel()
        {
            InitializeComponent();

            this.InitializePanel();
        }

        #endregion

        #region 实例方法

        private void InitializePanel()
        {
            this.disks = new BindableCollection<DiskVM>();
            this.netDevices = new BindableCollection<NetDeviceVM>();
            this.processes = new BindableCollection<ProcessVM>();
            this.cpuMem = new CpuMemVM();
            DataGridDisks.ItemsSource = this.disks;
            ListBoxNetDevices.ItemsSource = this.netDevices;
            DataGridProcesses.ItemsSource = this.processes;
            GridCpuMem.DataContext = this.cpuMem;

            MEMORYSTATUSEX memstat = new MEMORYSTATUSEX();
            this.memstatSize = Marshal.SizeOf(memstat);
        }

        private void UpdateSshPerfMon() { }

        /// <summary>
        /// 
        /// </summary>
        private void UpdateWin32PerfMon()
        {
            #region 磁盘信息

            DriveInfo[] drives = DriveInfo.GetDrives();

            foreach (DriveInfo drive in drives)
            {
                DiskVM disk = this.disks.FirstOrDefault(v => v.Name == drive.Name);
                if (disk == null)
                {
                    disk = new DiskVM()
                    {
                        Name = drive.Name,
                    };

                    Dispatcher.Invoke(() =>
                    {
                        this.disks.Add(disk);
                    });
                }

                double totalSize, availableFreeSpace;
                UnitType totalSizeUnit, availableFreeSpaceUnit;
                this.AutoFitSize(drive.TotalSize, UnitType.bytes, out totalSize, out totalSizeUnit);
                this.AutoFitSize(drive.AvailableFreeSpace, UnitType.bytes, out availableFreeSpace, out availableFreeSpaceUnit);

                disk.FreeRatio = string.Format("{0}{1}/{2}{3}", availableFreeSpace, availableFreeSpaceUnit, totalSize, totalSizeUnit);
            }

            // 删除不存在的磁盘
            if (this.disks.Count > drives.Length)
            {
                this.CleanupDisks(drives.Select(v => v.Name));
            }

            #endregion

            #region 进程信息

            Process[] processes = Process.GetProcesses();

            if (this.processes.Count == 0)
            {
                List<ProcessVM> processVMs = new List<ProcessVM>();

                foreach (Process win32Proc in processes)
                {
                    ProcessVM procVM = new ProcessVM();
                    procVM.PID = win32Proc.Id;
                    procVM.Name = win32Proc.ProcessName;

                    double memory;
                    UnitType memoryUnit;
                    this.AutoFitSize(win32Proc.WorkingSet64, UnitType.bytes, out memory, out memoryUnit);
                    procVM.DisplayMemory = string.Format("{0}{1}", memory, memoryUnit);

                    processVMs.Add(procVM);
                }

                Dispatcher.Invoke(() =>
                {
                    this.processes.AddRange(processVMs);
                });
            }
            else
            {
                foreach (Process win32Proc in processes)
                {
                    ProcessVM procVM = this.processes.FirstOrDefault(v => v.PID == win32Proc.Id);
                    if (procVM == null)
                    {
                        procVM = new ProcessVM();
                        procVM.PID = win32Proc.Id;
                        procVM.Name = win32Proc.ProcessName;

                        Dispatcher.Invoke(() =>
                        {
                            this.processes.Add(procVM);
                        });
                    }

                    double memory;
                    UnitType memoryUnit;
                    this.AutoFitSize(win32Proc.WorkingSet64, UnitType.bytes, out memory, out memoryUnit);
                    procVM.DisplayMemory = string.Format("{0}{1}", memory, memoryUnit);
                }

                if (this.processes.Count > processes.Length)
                {
                    this.CleanupProcesses(processes.Select(v => v.Id));
                }
            }

            #endregion

            #region 网络接口信息

            NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();

            foreach (NetworkInterface interfaze in interfaces)
            {
                NetDeviceVM netDev = this.netDevices.FirstOrDefault(v => v.Name == interfaze.Name);
                if (netDev == null)
                {
                    netDev = new NetDeviceVM();
                    netDev.IfaceId = interfaze.Id;
                    netDev.Name = interfaze.Name;

                    #region 获取网络接口IP地址

                    IPInterfaceProperties properties = interfaze.GetIPProperties();
                    // 一张网卡可以设置多个IP地址，这里取第一个，取IPV4
                    UnicastIPAddressInformation unicastIPAddress = properties.UnicastAddresses.FirstOrDefault(v => v.Address.AddressFamily == AddressFamily.InterNetwork);
                    if (unicastIPAddress != null)
                    {
                        netDev.IPAddress = unicastIPAddress.Address.ToString();
                    }
                    else
                    {
                        logger.WarnFormat("获取网络接口IP地址失败, {0}", interfaze.Name);
                    }

                    #endregion

                    Dispatcher.Invoke(() =>
                    {
                        this.netDevices.Add(netDev);
                    });
                }

                netDev.PrevBytesSent = netDev.BytesSent;
                netDev.PrevBytesReceived = netDev.BytesReceived;

                IPv4InterfaceStatistics statistics = interfaze.GetIPv4Statistics();
                netDev.BytesSent = (ulong)statistics.BytesSent;
                netDev.BytesReceived = (ulong)statistics.BytesReceived;
            }

            if (this.netDevices.Count > interfaces.Length)
            {
                this.CleanupNetDevices(interfaces.Select(v => v.Id));
            }

            #endregion

            #region Cpu占用

            if (this.cpuUsagePerf == null)
            {
                try
                {
                    this.cpuUsagePerf = new PerformanceCounter("Processor", "% Processor Time", "_Total");
                    this.cpuUsagePerf.NextValue(); // 解决第一次获取不到值的问题
                }
                catch (Exception ex)
                {
                    logger.Error("初始化Cpu性能计数器异常", ex);
                }
            }

            if (this.cpuUsagePerf != null)
            {
                this.cpuMem.CpuPercent = Math.Round(this.cpuUsagePerf.NextValue(), 2);
            }

            #endregion

            #region 内存占用

            MEMORYSTATUSEX memstat;
            memstat.dwLength = (uint)this.memstatSize;
            if (GlobalMemoryStatusEx(out memstat))
            {
                this.cpuMem.MemoryPercent = memstat.dwMemoryLoad;
                ulong dwUsagePhys = memstat.dwTotalPhys - memstat.dwAvailPhys;
                double total, usage;
                UnitType totalUnit, usageUnit;
                this.AutoFitSize(memstat.dwTotalPhys, UnitType.bytes, out total, out totalUnit);
                this.AutoFitSize(dwUsagePhys, UnitType.bytes, out usage, out usageUnit);
                this.cpuMem.DisplayMemoryUsage = string.Format("{0}{1}/{2}{3}", usage, usageUnit, total, totalUnit);
            }
            else
            {
                logger.ErrorFormat("GlobalMemoryStatusEx失败, {0}", Marshal.GetLastWin32Error());
            }

            #endregion
        }

        /// <summary>
        /// 根据输入的大小获取一个自适应的大小
        /// </summary>
        private void AutoFitSize(double fromValue, UnitType fromUnit, out double toValue, out UnitType toUnit)
        {
            toValue = fromValue;
            toUnit = fromUnit;

            int decimals = 2;

            // 小于1024，如果再转换那就小于1了，显示不友好
            // 所以直接返回
            if (fromValue < 1024)
            {
                return;
            }

            toValue = fromValue / 1024;

            switch (fromUnit)
            {
                case UnitType.bytes: goto FromBytes;
                case UnitType.KB: goto FromKB;
                case UnitType.MB: goto FromMB;
                case UnitType.GB: goto FromGB;
                default:
                    throw new NotImplementedException();
            }

            FromBytes:
            if (toValue < 1024)
            {
                toValue = Math.Round(toValue, decimals);
                toUnit = UnitType.KB;
                return;
            }

            toValue = toValue / 1024;

            FromKB:
            if (toValue < 1024)
            {
                toValue = Math.Round(toValue, decimals);
                toUnit = UnitType.MB;
                return;
            }

            toValue = toValue / 1024;

            FromMB:
            if (toValue < 1024)
            {
                toValue = Math.Round(toValue, decimals);
                toUnit = UnitType.GB;
                return;
            }

            toValue = toValue / 1024;

            FromGB:
            if (toValue < 1024)
            {
                toValue = Math.Round(toValue, decimals);
                toUnit = UnitType.TB;
            }
            else
            {
                // 大于1024TB
                toValue = Math.Round(toValue, decimals);
                toUnit = UnitType.TB;
            }
        }

        /// <summary>
        /// 删除不存在的磁盘
        /// </summary>
        /// <param name="newestDisks">最新的磁盘名字列表</param>
        private void CleanupDisks(IEnumerable<string> newestDisks)
        {
            for (int i = 0; i < this.disks.Count; i++)
            {
                DiskVM disk = this.disks[i];

                if (newestDisks.FirstOrDefault(v => v == disk.Name) == null)
                {
                    Dispatcher.Invoke(() =>
                    {
                        this.disks.Remove(disk);
                    });
                    i--;
                }
            }
        }

        private void CleanupProcesses(IEnumerable<int> newestPids)
        {
            for (int i = 0; i < this.processes.Count; i++)
            {
                ProcessVM proc = this.processes[i];

                if (newestPids.FirstOrDefault(v => v == proc.PID) == null)
                {
                    Dispatcher.Invoke(() =>
                    {
                        this.processes.Remove(proc);
                    });
                    i--;
                }
            }
        }

        private void CleanupNetDevices(IEnumerable<string> newestNetDeviceIds)
        {
            for (int i = 0; i < this.netDevices.Count; i++)
            {
                NetDeviceVM netDevice = this.netDevices[i];

                if (newestNetDeviceIds.FirstOrDefault(v => v == netDevice.IfaceId) == null)
                {
                    Dispatcher.Invoke(() =>
                    {
                        this.netDevices.Remove(netDevice);
                    });
                    i--;
                }
            }
        }

        #endregion

        #region TabedSidePanel

        public override void Initialize()
        {
        }

        public override void Release()
        {
            if (this.cpuUsagePerf != null)
            {
                this.cpuUsagePerf.Dispose();
                this.cpuUsagePerf = null;
            }
        }

        public override void Load()
        {
            this.timerHandle = TimerUtils.Context.CreateTimer(string.Format("{0} - 性能监控", this.Tab.Name), TimerGranularities.Second, 2, this.PerfMonThreadProc, null);
        }

        public override void Unload()
        {
            TimerUtils.Context.DeleteTimer(this.timerHandle);
        }

        #endregion

        #region 事件处理器

        private void PerfMonThreadProc(TimerHandle timer, object userData)
        {
            switch (this.Tab.Type)
            {
                case SessionTypeEnum.SSH:
                    {
                        this.UpdateSshPerfMon();
                        break;
                    }

                case SessionTypeEnum.Localhost:
                    {
                        this.UpdateWin32PerfMon();
                        break;
                    }

                default:
                    throw new NotImplementedException();
            }

            if (this.lastUpdateTime == DateTime.MinValue)
            {
                this.lastUpdateTime = DateTime.Now;
            }
            else
            {
                TimeSpan ts = DateTime.Now - this.lastUpdateTime;

                #region 计算网络接口上传和下载速度

                foreach (NetDeviceVM netDevice in this.netDevices)
                {
                    ulong bytesReceived = netDevice.BytesReceived - netDevice.PrevBytesReceived;
                    if (bytesReceived > 0)
                    {
                        double value;
                        UnitType valueUnit;
                        this.AutoFitSize(bytesReceived, UnitType.bytes, out value, out valueUnit);
                        netDevice.DownloadSpeed = string.Format("{0}{1}/s", value, valueUnit);
                    }
                    else
                    {
                        netDevice.DownloadSpeed = "0kb/s";
                    }

                    ulong bytesSent = netDevice.BytesSent - netDevice.PrevBytesSent;
                    if (bytesSent > 0)
                    {
                        double value;
                        UnitType valueUnit;
                        this.AutoFitSize(bytesSent, UnitType.bytes, out value, out valueUnit);
                        netDevice.UploadSpeed = string.Format("{0}{1}/s", value, valueUnit);
                    }
                    else
                    {
                        netDevice.UploadSpeed = "0kb/s";
                    }
                }

                #endregion

                this.lastUpdateTime = DateTime.Now;
            }
        }

        #endregion

        #region Win32API Interop

        [StructLayout(LayoutKind.Sequential)]
        public struct FILETIME
        {
            public uint dwLowDateTime;
            public uint dwHighDateTime;
        }
        [DllImport("kernel32")]
        private static extern bool GetSystemTimes(out FILETIME lpIdleTime, out FILETIME lpKernelTime, out FILETIME lpUserTime);

        [StructLayout(LayoutKind.Sequential)]
        public struct MEMORYSTATUSEX //此处全是以字节为单位
        {
            public uint dwLength;//长度
            public uint dwMemoryLoad;//内存使用率
            public UInt64 dwTotalPhys;//总物理内存
            public UInt64 dwAvailPhys;//可用物理内存
            public UInt64 dwTotalPageFile;//交换文件总大小
            public UInt64 dwAvailPageFile;//可用交换文件大小
            public UInt64 dwTotalVirtual;//总虚拟内存
            public UInt64 dwAvailVirtual;//可用虚拟内存大小
            public UInt64 ullAvailExtendedVirtual;
        }
        [DllImport("kernel32")]
        private static extern bool GlobalMemoryStatusEx(out MEMORYSTATUSEX memstat);

        [StructLayout(LayoutKind.Sequential)]
        public struct SYSTEM_INFO
        {
            public ushort wProcessorArchitecture;
            public ushort wReserved;
            public uint dwPageSize;
            public IntPtr lpMinimumApplicationAddress;
            public IntPtr lpMaximumApplicationAddress;
            public IntPtr dwActiveProcessorMask;
            public uint dwNumberOfProcessors;
            public uint dwProcessorType;
            public uint dwAllocationGranularity;
            public ushort wProcessorLevel;
            public ushort wProcessorRevision;
        }
        [DllImport("kernel32")]
        public static extern void GetSystemInfo(ref SYSTEM_INFO sysinfo);

        #endregion
    }
}


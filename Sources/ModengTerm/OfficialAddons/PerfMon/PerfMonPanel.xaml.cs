using DotNEToolkit;
using DotNEToolkit.Utility;
using log4net.Util;
using ModengTerm.Addon.Controls;
using ModengTerm.Addon.Interactive;
using ModengTerm.Base;
using ModengTerm.Base.Enumerations;
using ModengTerm.OfficialAddons.Broadcast;
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
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("PerfMonPanel");

        private static readonly List<int> CPU_USER_TIME_INDEX = new List<int>() { 1, 2 };
        private static readonly List<int> CPU_KERNEL_TIME_INDEX = new List<int>() { 3, 5, 6, 7, 8 };
        private static readonly List<int> CPU_IDLE_TIME_INDEX = new List<int>() { 4 };
        private static readonly string FetchNetworkInterfaces = "interfaces=$(ifconfig | grep -o '^[a-zA-Z0-9]\\+:' | tr -d ':');for interface in $interfaces; do ip_address=$(ifconfig \"$interface\" | grep -o 'inet [0-9.]\\+' | awk '{print $2}');receive_bytes=$(ifconfig \"$interface\" | grep 'RX packets' | awk '{print $5}');transmit_bytes=$(ifconfig \"$interface\" | grep 'TX packets' | awk '{print $5}');echo $interface,$ip_address,$receive_bytes,$transmit_bytes;done;";
        private static readonly string FetchProcess = "for pid in $(ls -d /proc/[0-9]* 2>/dev/null); do pid=${pid##*/};name=$(grep '^Name:' \"/proc/$pid/status\" | awk '{print $2}');rss=$(grep '^VmRSS:' \"/proc/$pid/status\" | awk '{print $2}');stat=($(awk '{print $14, $15}' \"/proc/$pid/stat\"));utime=${stat[0]};ktime=${stat[1]};echo $pid,$name,$rss,$utime,$ktime;done;";

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

        // ssh使用
        private ulong prevKernelProcessorTime;
        private ulong prevUserProcessorTime;
        private ulong prevIdleProcessorTime;

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

        private void UpdateSshPerfMon() 
        {
            IClientShellTab shellTab = this.Tab as IClientShellTab;
            ISshChannel sshEngine = shellTab.GetSshEngine();

            this.UpdateSshMem(sshEngine);
            this.UpdateSshCpu(sshEngine);
            this.UpdateSshDisks(sshEngine);
            this.UpdateSshNetDevices(sshEngine);
            this.UpdateSshProcess(sshEngine);
        }

        private void UpdateSshMem(ISshChannel sshEngine) 
        {
            string meminfo = sshEngine.ExecuteScript("cat /proc/meminfo");
            string[] items = meminfo.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            ulong totalMem;
            if (!this.parse_meminfo_item(items, "MemTotal", out totalMem))
            {
                logger.ErrorFormat("解析MemTotal失败");
                return;
            }

            ulong availableMem;
            if (!this.parse_meminfo_item(items, "MemAvailable", out availableMem))
            {
                logger.ErrorFormat("解析MemAvailable失败");
                return;
            }

            ulong usageMem = totalMem - availableMem;

            double total, usage;
            SizeUnitEnum totalUnit, usageUnit;
            VTBaseUtils.AutoFitSize(totalMem, SizeUnitEnum.KB, out total, out totalUnit);
            VTBaseUtils.AutoFitSize(usageMem, SizeUnitEnum.KB, out usage, out usageUnit);
            this.cpuMem.DisplayMemoryUsage = string.Format("{0}{1}/{2}{3}", usage, usageUnit, total, totalUnit);
            this.cpuMem.MemoryPercent = Math.Round((double)usageMem / totalMem * 100, 2);
        }

        private void UpdateSshCpu(ISshChannel sshEngine) 
        {
            ulong userProcessorTime, kernelProcesstorTime, idleProcesstorTime;

            string cpuinfo = sshEngine.ExecuteScript("cat /proc/stat");
            string[] cpuitems = cpuinfo.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            string cpu = cpuitems.FirstOrDefault(v => v.Contains("cpu "));
            if (!this.handle_cpu(cpu, out userProcessorTime, out kernelProcesstorTime, out idleProcesstorTime)) 
            {
                return;
            }

            ulong totalProcessorTime = 0;

            // 确保至少读取了一次CPU占用信息
            if (this.prevKernelProcessorTime > 0)
            {
                ulong idleTime = idleProcesstorTime - this.prevIdleProcessorTime;
                ulong kernelTime = kernelProcesstorTime - this.prevKernelProcessorTime;
                ulong userTime = userProcessorTime - this.prevUserProcessorTime;
                ulong totalTime = idleTime + kernelTime + userTime;
                totalProcessorTime = kernelTime + userTime;
                this.cpuMem.CpuPercent = Math.Round((double)totalProcessorTime / totalTime * 100, 2);
                if (this.cpuMem.CpuPercent > 100)
                {
                    logger.FatalFormat("3. {0}, totalProcessorTime = {1}, totalTime = {2}, kernelTime = {3}, userTime = {4}, idleTime = {4}, prevIdleProcessorTime = {5}", this.cpuMem.CpuPercent, totalProcessorTime, totalTime, kernelTime, userTime, idleTime, this.prevIdleProcessorTime);
                }
            }

            this.prevIdleProcessorTime = idleProcesstorTime;
            this.prevKernelProcessorTime = kernelProcesstorTime;
            this.prevUserProcessorTime = userProcessorTime;
        }

        private void UpdateSshDisks(ISshChannel sshEngine)
        {
            string df = sshEngine.ExecuteScript("df");

            if (string.IsNullOrEmpty(df))
            {
                return;
            }

            IEnumerable<string> lines = df.Split('\n', StringSplitOptions.RemoveEmptyEntries).Skip(1);
            if (lines == null)
            {
                return;
            }

            foreach (string source in lines)
            {
                string[] strs = source.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (strs.Length < 5)
                {
                    logger.ErrorFormat("unix disk format error, {0}", source);
                    continue;
                }

                string name = strs[strs.Length - 1];

                DiskVM disk = this.disks.FirstOrDefault(v => v.Name == name);
                if (disk == null) 
                {
                    disk = new DiskVM();
                    disk.Name = name;

                    Dispatcher.Invoke(() => 
                    {
                        this.disks.Add(disk);
                    });
                }

                double totalSize = 0, availableFreeSpace = 0;
                SizeUnitEnum totalSizeUnit = SizeUnitEnum.bytes, availableFreeSpaceUnit = SizeUnitEnum.bytes;

                ulong available;
                if (ulong.TryParse(strs[3], out available))
                {
                    VTBaseUtils.AutoFitSize(available, SizeUnitEnum.KB, out availableFreeSpace, out availableFreeSpaceUnit);
                }
                else
                {
                    logger.ErrorFormat("unix disk available error, {0}", source);
                    continue;
                }

                ulong size;
                if (ulong.TryParse(strs[1], out size))
                {
                    VTBaseUtils.AutoFitSize(size, SizeUnitEnum.KB, out totalSize, out totalSizeUnit);
                }
                else
                {
                    logger.ErrorFormat("unix disk size error, {0}", source);
                    continue;
                }

                disk.FreeRatio = string.Format("{0}{1}/{2}{3}", availableFreeSpace, availableFreeSpaceUnit, totalSize, totalSizeUnit);
            }

            if (this.disks.Count > lines.Count())
            {
                IEnumerable<string> names = lines.Select(source => 
                {
                    string[] strs = source.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    if (strs.Length < 5)
                    {
                        return string.Empty;
                    }

                    return strs[strs.Length - 1];
                });

                this.CleanupDisks(names);
            }
        }

        private void UpdateSshNetDevices(ISshChannel sshEngine)
        {
            string netdev = sshEngine.ExecuteScript(FetchNetworkInterfaces);

            if (string.IsNullOrEmpty(netdev))
            {
                return;
            }

            IEnumerable<string> lines = netdev.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            if (lines == null)
            {
                return;
            }

            foreach (string source in lines)
            {
                string[] items = source.Split(',');

                NetDeviceVM netDevice = this.netDevices.FirstOrDefault(v => v.Name == items[0]);
                if (netDevice == null) 
                {
                    netDevice = new NetDeviceVM();
                    netDevice.IfaceId = items[0];
                    netDevice.Name = items[0];

                    Dispatcher.Invoke(() => 
                    {
                        this.netDevices.Add(netDevice);
                    });
                }

                netDevice.IPAddress = items[1];

                ulong bytesReceived, bytesSent;
                if (ulong.TryParse(items[2], out bytesReceived))
                {
                    netDevice.BytesReceived = bytesReceived;
                }

                if (ulong.TryParse(items[3], out bytesSent))
                {
                    netDevice.BytesSent = bytesSent;
                }
            }

            if (this.netDevices.Count > lines.Count())
            {
                this.CleanupNetDevices(lines.Select(v => v.Split(',')[0]));
            }
        }

        private void UpdateSshProcess(ISshChannel sshEngine)
        {
            string process = sshEngine.ExecuteScript(FetchProcess);

            if (string.IsNullOrEmpty(process))
            {
                return;
            }

            IEnumerable<string> lines = process.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            if (lines == null)
            {
                return;
            }

            if (this.processes.Count == 0)
            {
                List<ProcessVM> procVMs = new List<ProcessVM>();

                foreach (string source in lines)
                {
                    string[] items = source.Split(',');

                    int pid;
                    if (!int.TryParse(items[0], out pid))
                    {
                        logger.ErrorFormat("parse unix proc pid failed");
                        continue;
                    }

                    if (string.IsNullOrEmpty(items[1]))
                    {
                        // 进程名字为空
                        continue;
                    }

                    ProcessVM procVM = new ProcessVM();
                    procVM.PID = pid;
                    procVM.Name = items[1];

                    ulong rss;
                    if (ulong.TryParse(items[2], out rss))
                    {
                        double memory;
                        SizeUnitEnum memoryUnit;
                        VTBaseUtils.AutoFitSize(rss, SizeUnitEnum.KB, out memory, out memoryUnit);
                        procVM.DisplayMemory = string.Format("{0}{1}", memory, memoryUnit);
                    }

                    //long utime = 0, ktime = 0;
                    //long.TryParse(items[3], out utime);
                    //long.TryParse(items[4], out ktime);

                    //target.TotalProcessorTime = utime + ktime;

                    procVMs.Add(procVM);
                }

                Dispatcher.Invoke(() =>
                {
                    this.processes.AddRange(procVMs);
                });
            }
            else
            {
                List<int> pids = new List<int>();

                foreach (string source in lines)
                {
                    string[] items = source.Split(',');

                    int pid;
                    if (!int.TryParse(items[0], out pid))
                    {
                        logger.ErrorFormat("parse unix proc pid failed");
                        continue;
                    }

                    if (string.IsNullOrEmpty(items[1])) 
                    {
                        // 进程名字为空
                        continue;
                    }

                    pids.Add(pid);

                    ProcessVM procVM = this.processes.FirstOrDefault(v => v.PID == pid);
                    if (procVM == null)
                    {
                        procVM = new ProcessVM();
                        procVM.PID = pid;

                        Dispatcher.Invoke(() =>
                        {
                            this.processes.Add(procVM);
                        });
                    }

                    procVM.Name = items[1];

                    ulong rss;
                    if (ulong.TryParse(items[2], out rss))
                    {
                        double memory;
                        SizeUnitEnum memoryUnit;
                        VTBaseUtils.AutoFitSize(rss, SizeUnitEnum.KB, out memory, out memoryUnit);
                        procVM.DisplayMemory = string.Format("{0}{1}", memory, memoryUnit);
                    }

                    //long utime = 0, ktime = 0;
                    //long.TryParse(items[3], out utime);
                    //long.TryParse(items[4], out ktime);

                    //target.TotalProcessorTime = utime + ktime;
                }

                if (this.processes.Count > pids.Count)
                {
                    this.CleanupProcesses(pids);
                }
            }
        }




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
                    disk = new DiskVM();
                    disk.Name = drive.Name;

                    Dispatcher.Invoke(() =>
                    {
                        this.disks.Add(disk);
                    });
                }

                double totalSize, availableFreeSpace;
                SizeUnitEnum totalSizeUnit, availableFreeSpaceUnit;
                VTBaseUtils.AutoFitSize(drive.TotalSize, SizeUnitEnum.bytes, out totalSize, out totalSizeUnit);
                VTBaseUtils.AutoFitSize(drive.AvailableFreeSpace, SizeUnitEnum.bytes, out availableFreeSpace, out availableFreeSpaceUnit);

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
                List<ProcessVM> procVMs = new List<ProcessVM>();

                foreach (Process win32Proc in processes)
                {
                    ProcessVM procVM = new ProcessVM();
                    procVM.PID = win32Proc.Id;
                    procVM.Name = win32Proc.ProcessName;

                    double memory;
                    SizeUnitEnum memoryUnit;
                    VTBaseUtils.AutoFitSize(win32Proc.WorkingSet64, SizeUnitEnum.bytes, out memory, out memoryUnit);
                    procVM.DisplayMemory = string.Format("{0}{1}", memory, memoryUnit);

                    procVMs.Add(procVM);
                }

                Dispatcher.Invoke(() =>
                {
                    this.processes.AddRange(procVMs);
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

                        Dispatcher.Invoke(() =>
                        {
                            this.processes.Add(procVM);
                        });
                    }

                    procVM.Name = win32Proc.ProcessName;

                    double memory;
                    SizeUnitEnum memoryUnit;
                    VTBaseUtils.AutoFitSize(win32Proc.WorkingSet64, SizeUnitEnum.bytes, out memory, out memoryUnit);
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
                SizeUnitEnum totalUnit, usageUnit;
                VTBaseUtils.AutoFitSize(memstat.dwTotalPhys, SizeUnitEnum.bytes, out total, out totalUnit);
                VTBaseUtils.AutoFitSize(dwUsagePhys, SizeUnitEnum.bytes, out usage, out usageUnit);
                this.cpuMem.DisplayMemoryUsage = string.Format("{0}{1}/{2}{3}", usage, usageUnit, total, totalUnit);
            }
            else
            {
                logger.ErrorFormat("GlobalMemoryStatusEx失败, {0}", Marshal.GetLastWin32Error());
            }

            #endregion
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newestPids">进程Id列表</param>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newestNetDeviceIds">最新的网络接口Id</param>
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

            if (this.timerHandle != null)
            {
                TimerUtils.Context.DeleteTimer(this.timerHandle);
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
            if (this.Tab.Status != SessionStatusEnum.Connected)
            {
                return;
            }

            switch (this.Tab.Type)
            {
                case SessionTypeEnum.Ssh:
                    {
                        this.UpdateSshPerfMon();
                        break;
                    }

                case SessionTypeEnum.LocalConsole:
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
                        SizeUnitEnum valueUnit;
                        VTBaseUtils.AutoFitSize(bytesReceived, SizeUnitEnum.bytes, out value, out valueUnit);
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
                        SizeUnitEnum valueUnit;
                        VTBaseUtils.AutoFitSize(bytesSent, SizeUnitEnum.bytes, out value, out valueUnit);
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

        #region 解析Ssh返回数据

        private bool parse_meminfo_item(string[] items, string key, out ulong value)
        {
            value = 0;

            string text = items.FirstOrDefault(v => v.StartsWith(key));
            if (string.IsNullOrEmpty(text))
            {
                return false;
            }

            string[] strs = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (strs == null || strs.Length != 3)
            {
                logger.ErrorFormat("mem error = {0}", text);
                return false;
            }

            if (!ulong.TryParse(strs[1], out value))
            {
                logger.ErrorFormat("mem error2 = {0}", text);
                return false;
            }

            return true;
        }

        private bool calc_cpu_time(string[] strs, List<int> indexs, out ulong cpu_time)
        {
            cpu_time = 0;

            foreach (int index in indexs)
            {
                ulong v;
                if (!ulong.TryParse(strs[index], out v))
                {
                    return false;
                }

                cpu_time += v;
            }

            return true;
        }

        private bool handle_cpu(string cpu, out ulong userProcessorTime, out ulong kernelProcessorTime, out ulong idleProcessorTime)
        {
            userProcessorTime = 0;
            kernelProcessorTime = 0;
            idleProcessorTime = 0;

            //        
            // cpu  1661515 33740 1757851 27139465 233 0 387078 0 0 0

            if (string.IsNullOrEmpty(cpu))
            {
                logger.ErrorFormat("cpu empty");
                return false;
            }

            string[] strs = cpu.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (strs.Length < 9)
            {
                logger.ErrorFormat("cpu error = {0}", cpu);
                return false;
            }

            // 用户时间
            ulong user_time;
            if (!this.calc_cpu_time(strs, CPU_USER_TIME_INDEX, out user_time))
            {
                logger.ErrorFormat("cpu user time error = {0}", cpu);
            }
            else
            {
                userProcessorTime = user_time;
            }

            // 内核时间
            ulong kernel_time;
            if (!this.calc_cpu_time(strs, CPU_KERNEL_TIME_INDEX, out kernel_time))
            {
                logger.ErrorFormat("cpu kernel time error = {0}", cpu);
            }
            else
            {
                kernelProcessorTime = kernel_time;
            }

            // 空闲时间
            ulong idle_time;
            if (!this.calc_cpu_time(strs, CPU_IDLE_TIME_INDEX, out idle_time))
            {
                logger.ErrorFormat("cpu idle time error = {0}", cpu);
            }
            else
            {
                idleProcessorTime = idle_time;
            }

            return true;
        }

        private void handle_process(string process)
        {
            if (string.IsNullOrEmpty(process))
            {
                return;
            }

            IEnumerable<string> lines = process.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            if (lines == null)
            {
                return;
            }

            //this.Copy<string, VTProcess>(this.systemInfo.Processes, lines, this.processCopy);
        }

        #endregion
    }
}


using ModengTerm.Base;
using ModengTerm.Base.DataModels;
using ModengTerm.Terminal.Session;
using System.Diagnostics;
using System.IO;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;

namespace ModengTerm.Terminal.Watch
{
    public class Win32Watcher : AbstractWatcher
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("Win32Watcher");

        #endregion

        #region 实例变量

        public Win32DiskCopy Win32DiskSync = new Win32DiskCopy();
        public Win32NetworkInterfaceCopy Win32NetworkInterfaceCopy = new Win32NetworkInterfaceCopy();
        public Win32ProcessCopy Win32ProcessCopy = new Win32ProcessCopy();
        private SystemInfo systemInfo;
        private int memstatSize;

        #endregion

        #region 构造方法

        public Win32Watcher(XTermSession session, SessionDriver driver) :
            base(session, driver)
        {
        }

        #endregion

        #region AbstractWatcher

        public override void Initialize()
        {
            this.systemInfo = new SystemInfo();
            MEMORYSTATUSEX memstat = new MEMORYSTATUSEX();
            this.memstatSize = Marshal.SizeOf(memstat);
        }

        public override void Release()
        {
        }

        public override SystemInfo GetSystemInfo()
        {
            MEMORYSTATUSEX memstat;
            memstat.dwLength = (uint)this.memstatSize;
            if (!GlobalMemoryStatusEx(out memstat))
            {
                logger.ErrorFormat("GlobalMemoryStatusEx失败, {0}", Marshal.GetLastWin32Error());
                return null;
            }

            this.systemInfo.TotalMemory.Value = memstat.dwTotalPhys;
            this.systemInfo.TotalMemory.Unit = UnitType.Byte;
            this.systemInfo.AvailableMemory.Value = memstat.dwAvailPhys;
            this.systemInfo.AvailableMemory.Unit = UnitType.Byte;
            FILETIME idleTime, kernelTime, userTime;
            if (GetSystemTimes(out idleTime, out kernelTime, out userTime))
            {
                ulong a = ((ulong)idleTime.dwHighDateTime) << 32 | idleTime.dwLowDateTime;
                ulong b = ((ulong)kernelTime.dwHighDateTime) << 32 | kernelTime.dwLowDateTime;
                ulong c = ((ulong)userTime.dwHighDateTime) << 32 | userTime.dwLowDateTime;

                this.systemInfo.IdleProcessorTime = ((ulong)idleTime.dwHighDateTime) << 32 | idleTime.dwLowDateTime;
                this.systemInfo.KernelProcessorTime = (((ulong)kernelTime.dwHighDateTime) << 32 | kernelTime.dwLowDateTime) - this.systemInfo.IdleProcessorTime;
                this.systemInfo.UserProcessorTime = ((ulong)userTime.dwHighDateTime) << 32 | userTime.dwLowDateTime;
            }

            // 更新磁盘信息
            DriveInfo[] newDisks = DriveInfo.GetDrives();
            this.Copy<DriveInfo, DiskInfo>(this.systemInfo.DiskItems, newDisks, Win32DiskSync);

            // 更新网络接口信息
            NetworkInterface[] newIfaces = NetworkInterface.GetAllNetworkInterfaces();
            this.Copy<NetworkInterface, NetInterfaceInfo>(this.systemInfo.NetworkInterfaces, newIfaces, Win32NetworkInterfaceCopy);

            // 更新进程信息
            Process[] newProcs = Process.GetProcesses();
            this.Copy<Process, ProcessInfo>(this.systemInfo.Processes, newProcs, Win32ProcessCopy);

            return this.systemInfo;
        }

        #endregion

        #region 实例方法

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


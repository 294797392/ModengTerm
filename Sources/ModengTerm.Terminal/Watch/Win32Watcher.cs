﻿using ModengTerm.Base.DataModels;
using System.Diagnostics;
using System.IO;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Windows.Shell;

namespace ModengTerm.Terminal.Watch
{
    public class Win32Watcher : AbstractWatcher
    {
        #region 类变量

        public static readonly Win32DiskCopy Win32DiskSync = new Win32DiskCopy();
        public static readonly Win32NetworkInterfaceCopy Win32NetworkInterfaceCopy = new Win32NetworkInterfaceCopy();
        private static log4net.ILog logger = log4net.LogManager.GetLogger("Win32Watcher");

        #endregion

        #region 实例变量

        private SystemInfo systemInfo;
        private int memstatSize;
        private PerformanceCounter cpuPerf;

        #endregion

        #region 构造方法

        public Win32Watcher(XTermSession session) :
            base(session)
        {
        }

        #endregion

        #region AbstractWatcher

        public override void Initialize()
        {
            this.systemInfo = new SystemInfo();
            this.cpuPerf = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            this.cpuPerf.NextValue();
            MEMORYSTATUSEX memstat = new MEMORYSTATUSEX();
            this.memstatSize = Marshal.SizeOf(memstat);
        }

        public override void Release()
        {
            this.cpuPerf.Close();
            this.cpuPerf.Dispose();
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

            this.systemInfo.TotalMemory = memstat.dwTotalPhys / 1024;
            this.systemInfo.AvailableMemory = memstat.dwAvailPhys / 1024;
            this.systemInfo.CpuPercent = this.cpuPerf.NextValue();

            // 更新磁盘信息
            DriveInfo[] newDisks = DriveInfo.GetDrives();
            this.Copy<DriveInfo, DiskInfo>(this.systemInfo.DiskItems, newDisks, Win32DiskSync);

            // 更新网络接口信息
            NetworkInterface[] newIfaces = NetworkInterface.GetAllNetworkInterfaces();
            this.Copy<NetworkInterface, NetInterfaceInfo>(this.systemInfo.NetworkInterfaces, newIfaces, Win32NetworkInterfaceCopy);

            return this.systemInfo;
        }

        #endregion

        #region 实例方法

        #endregion

        #region Win32API Interop

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


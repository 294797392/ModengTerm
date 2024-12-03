using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal.Watch
{
    public class DiskInfo
    {
        private string name;
        private double totalSpace;
        private double freeSpace;
        private string format;

        /// <summary>
        /// 磁盘名称
        /// </summary>
        public string Name
        {
            get { return this.name; }
            set
            {
                if (this.name != value)
                {
                    this.name = value;
                }
            }
        }

        /// <summary>
        /// 磁盘总空间，单位字节
        /// </summary>
        public double TotalSpace
        {
            get { return this.totalSpace; }
            set
            {
                if (this.totalSpace != value)
                {
                    this.totalSpace = value;
                }
            }
        }

        /// <summary>
        /// 磁盘空闲空间，单位字节
        /// </summary>
        public double FreeSpace
        {
            get { return this.freeSpace; }
            set
            {
                if (this.freeSpace != value)
                {
                    this.freeSpace = value;
                }
            }
        }

        /// <summary>
        /// 磁盘格式
        /// </summary>
        public string Format
        {
            get { return this.format; }
            set
            {
                if (this.format != value)
                {
                    this.format = value;
                }
            }
        }
    }

    public class Win32DiskCopy : ObjectCopy<DiskInfo, DriveInfo>
    {
        public override void CopyTo(DiskInfo diskInfo, DriveInfo platformDisk)
        {
            DriveInfo driveInfo = platformDisk as DriveInfo;

            diskInfo.Name = driveInfo.Name;
            diskInfo.TotalSpace = driveInfo.TotalSize;
            diskInfo.FreeSpace = driveInfo.TotalFreeSpace;
            diskInfo.Format = driveInfo.DriveFormat;
        }

        public override bool Compare(DiskInfo diskInfo, DriveInfo osDisk)
        {
            return diskInfo.Name == osDisk.Name;
        }
    }

    public class UnixDiskCopy : ObjectCopy<DiskInfo, string[]>
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("");

        public override bool Compare(DiskInfo target, string[] source)
        {
            return target.Name == source[0];
        }

        public override void CopyTo(DiskInfo target, string[] source)
        {
            if (source.Length < 5)
            {
                logger.ErrorFormat("UnixDiskCopy, {0}", source.Length);
                return;
            }

            target.Name = source[0];

            int size;
            if (int.TryParse(source[1], out size))
            {
                target.TotalSpace = size;
            }

            int available;
            if (int.TryParse(source[3], out available))
            {
                target.FreeSpace = available;
            }
        }
    }
}

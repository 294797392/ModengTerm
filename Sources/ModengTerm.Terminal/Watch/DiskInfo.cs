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
        /// 磁盘总空间
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
        /// 磁盘空闲空间
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
}

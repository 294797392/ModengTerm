using ModengTerm.Base;
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
        private UnitValue64 totalSpace;
        private UnitValue64 freeSpace;
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
        public UnitValue64 TotalSpace
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
        public UnitValue64 FreeSpace
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

        public DiskInfo()
        {
            this.TotalSpace = new UnitValue64();
            this.FreeSpace = new UnitValue64();
        }
    }

    public class Win32DiskCopy : ObjectCopy<DiskInfo, DriveInfo>
    {
        public override void CopyTo(DiskInfo diskInfo, DriveInfo driveInfo)
        {
            diskInfo.Name = driveInfo.Name;
            diskInfo.TotalSpace.Value = (ulong)driveInfo.TotalSize;
            diskInfo.TotalSpace.Unit = UnitType.Byte;
            diskInfo.FreeSpace.Value = (ulong)driveInfo.TotalFreeSpace;
            diskInfo.FreeSpace.Unit = UnitType.Byte;
            diskInfo.Format = driveInfo.DriveFormat;
        }

        public override bool Compare(DiskInfo diskInfo, DriveInfo osDisk)
        {
            return diskInfo.Name == osDisk.Name;
        }
    }

    public class UnixDiskCopy : ObjectCopy<DiskInfo, string>
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("UnixDiskCopy");

        public override bool Compare(DiskInfo target, string source)
        {
            int slen = source.Length;
            int sidx = slen - 1;
            int len = target.Name.Length;

            // 先找到最后一个不是空的字符
            for (int i = sidx; i > 0; i--)
            {
                if (source[i] != ' ')
                {
                    break;
                }
            }

            if (sidx == 0)
            {
                return false;
            }

            // 从最后一个字符向前比较
            for (int i = len - 1; i > 0; i--)
            {
                if (source[sidx--] != target.Name[i])
                {
                    return false;
                }
            }

            return true;
        }

        public override void CopyTo(DiskInfo target, string source)
        {
            string[] strs = source.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (strs.Length < 5)
            {
                logger.ErrorFormat("unix disk format error, {0}", source);
                return;
            }

            target.Name = strs[strs.Length - 1];

            ulong available;
            if (ulong.TryParse(strs[3], out available))
            {
                target.FreeSpace.Value = available;
                target.FreeSpace.Unit = UnitType.KB;
            }
            else
            {
                logger.ErrorFormat("unix disk available error, {0}", source);
            }

            ulong size;
            if (ulong.TryParse(strs[1], out size))
            {
                target.TotalSpace.Value = size;
                target.TotalSpace.Unit = UnitType.KB;
            }
            else
            {
                logger.ErrorFormat("unix disk size error, {0}", source);
            }
        }
    }
}

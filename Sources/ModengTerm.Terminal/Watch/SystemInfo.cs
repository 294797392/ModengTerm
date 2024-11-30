using System;
using System.Collections.Generic;
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

    public class NetworkIface
    {
        public string Name { get; set; }

        public int DownloadSpeed { get; set; }

        public int UploadSpeed { get; set; }
    }

    public class SystemInfo
    {
        /// <summary>
        /// CPU使用百分比
        /// </summary>
        public double CpuPercent { get; set; }

        /// <summary>
        /// 总内存数
        /// 单位kb
        /// </summary>
        public double TotalMemory { get; set; }

        /// <summary>
        /// 可用内存数
        /// 单位kb
        /// </summary>
        public double AvailableMemory { get; set; }

        /// <summary>
        /// 所有磁盘列表
        /// </summary>
        public List<DiskInfo> DiskInfos { get; private set; }

        /// <summary>
        /// 新增加的磁盘
        /// </summary>
        public List<DiskInfo> AddDisks { get; private set; }

        /// <summary>
        /// 被删除的磁盘
        /// </summary>
        public List<DiskInfo> RemoveDisks { get; private set; }

        public List<NetworkIface> Interfaces { get; private set; }

        public SystemInfo()
        {
            this.DiskInfos = new List<DiskInfo>();
            this.AddDisks = new List<DiskInfo>();
            this.RemoveDisks = new List<DiskInfo>();

            this.Interfaces = new List<NetworkIface>();
        }
    }
}

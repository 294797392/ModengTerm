using DotNEToolkit.DataModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal.Watch
{
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

        public ChangedItems<DiskInfo> DiskItems { get; private set; }

        public SystemInfo()
        {
            this.DiskItems = new ChangedItems<DiskInfo>();
        }
    }
}

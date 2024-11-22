using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal.Watchers
{
    /// <summary>
    /// 保存磁盘信息
    /// </summary>
    public class DiskInfo
    {
        /// <summary>
        /// 磁盘名字
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 总大小
        /// </summary>
        public double TotalSize { get; set; }

        /// <summary>
        /// 已用大小
        /// </summary>
        public double UsedSize { get; set; }
    }
}

using ModengTerm.FileTrans.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.FileTrans.DataModels
{
    public class FsItemInfo
    {
        /// <summary>
        /// 文件名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 文件完整路径
        /// </summary>
        public string FullPath { get; set; }

        /// <summary>
        /// 文件大小
        /// </summary>
        public long Size { get; set; }

        /// <summary>
        /// 文件类型
        /// </summary>
        public FsItemTypeEnum Type { get; set; }

        /// <summary>
        /// 最后一个更新时间
        /// </summary>
        public DateTime LastUpdateTime { get; set; }

        /// <summary>
        /// 是否是隐藏文件
        /// </summary>
        public bool IsHidden { get; set; }
    }
}

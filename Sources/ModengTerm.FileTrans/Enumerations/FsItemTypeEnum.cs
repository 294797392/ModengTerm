using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.FileTrans.Enumerations
{
    /// <summary>
    /// 文件列表节点类型
    /// </summary>
    public enum FsItemTypeEnum
    {
        /// <summary>
        /// 该节点是一个文件
        /// </summary>
        File,

        /// <summary>
        /// 该节点是一个目录
        /// </summary>
        Directory,

        /// <summary>
        /// 返回上册目录
        /// </summary>
        ParentDirectory
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal.Document
{
    /// <summary>
    /// 定义文档所包含的区域
    /// </summary>
    public enum VTDocumentAreas
    {
        /// <summary>
        /// 相对于整个文档区域的坐标
        /// </summary>
        AllDocument,

        /// <summary>
        /// 相对于书签的坐标
        /// </summary>
        BookmarkArea,

        /// <summary>
        /// 相对于文档内容区域的坐标
        /// </summary>
        ContentArea,
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Document.Graphics
{
    public enum GraphicsObjectTypes
    {
        /// <summary>
        /// 光标
        /// </summary>
        Cursor,

        /// <summary>
        /// 文本块
        /// </summary>
        TextBlock,

        /// <summary>
        /// 鼠标选中的区域
        /// </summary>
        Selection,

        /// <summary>
        /// 文本行
        /// </summary>
        TextLine
    }
}

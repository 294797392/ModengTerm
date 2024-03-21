using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Document.Drawing
{
    public enum DrawingObjectTypes
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
        TextLine,

        /// <summary>
        /// 壁纸
        /// </summary>
        Wallpaper,

        /// <summary>
        /// 矩形区域
        /// </summary>
        Rectangle
    }
}

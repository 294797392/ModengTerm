using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XTerminal.Document.Rendering
{
    /// <summary>
    /// 定义一个TextPointer相对于另外一个TextPointer的位置
    /// </summary>
    public enum TextPointerPositions
    {
        /// <summary>
        /// 在原点位置没动
        /// </summary>
        Original,

        /// <summary>
        /// 正上方
        /// </summary>
        Top,

        /// <summary>
        /// 正下方
        /// </summary>
        Bottom,

        /// <summary>
        /// 正左方
        /// </summary>
        Left,

        /// <summary>
        /// 正右方
        /// </summary>
        Right,

        /// <summary>
        /// 左上方
        /// </summary>
        LeftTop,

        /// <summary>
        /// 左下方
        /// </summary>
        LeftBottom,

        /// <summary>
        /// 右上方
        /// </summary>
        RightTop,

        /// <summary>
        /// 右下方
        /// </summary>
        RightBottom
    }
}

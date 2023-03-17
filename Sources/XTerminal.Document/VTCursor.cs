using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTerminal.Parser;

namespace XTerminal.Document
{
    /// <summary>
    /// 定义光标类型
    /// </summary>
    public enum VTCursorStyles
    {
        None,

        /// <summary>
        /// 光标是一条竖线
        /// </summary>
        Line,

        /// <summary>
        /// 光标是一个矩形块
        /// </summary>
        Block,

        /// <summary>
        /// 光标是半个下划线
        /// </summary>
        Underscore
    }

    /// <summary>
    /// 光标的数据模型
    /// </summary>
    public class VTCursor : VTDocumentElement
    {
        /// <summary>
        /// 光标所在列
        /// </summary>
        public int Column { get; internal set; }

        /// <summary>
        /// 光标所在行
        /// </summary>
        public int Row { get; internal set; }

        /// <summary>
        /// 光标类型
        /// </summary>
        public VTCursorStyles Style { get; set; }

        /// <summary>
        /// 是否闪烁
        /// </summary>
        public bool Blinking { get; set; }

        /// <summary>
        /// 光标颜色
        /// </summary>
        public VTColors Color { get; set; }

        /// <summary>
        /// 光标闪烁的间隔时间，单位是毫秒
        /// </summary>
        public int Interval { get; set; }

        /// <summary>
        /// 是否是显示状态
        /// </summary>
        public bool IsVisible { get; set; }
    }
}

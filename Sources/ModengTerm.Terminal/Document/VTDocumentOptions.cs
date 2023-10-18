using ModengTerm.Base.DataModels;
using ModengTerm.Terminal;
using ModengTerm.Terminal.DataModels;
using ModengTerm.Terminal.Rendering;
using ModengTerm.Terminal.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTerminal.Base;
using XTerminal.Base.Definitions;
using XTerminal.Base.Enumerations;

namespace ModengTerm.Terminal.Document
{
    public class VTDocumentOptions
    {
        /// <summary>
        /// 整个文档的宽度，包含滚动条
        /// </summary>
        public double Width { get; set; }

        /// <summary>
        /// 整个文档的高度，包含滚动条
        /// </summary>
        public double Height { get; set; }

        /// <summary>
        /// 文档所属会话
        /// </summary>
        public XTermSession Session { get; set; }

        /// <summary>
        /// 字体信息
        /// </summary>
        public VTypeface Typeface { get; set; }

        /// <summary>
        /// 滚动条样式
        /// </summary>
        public ScrollbarStyle ScrollbarStyle { get; set; }

        /// <summary>
        /// 鼠标滚轮滚动一次，滚动几行
        /// </summary>
        public int ScrollDelta { get; set; }

        /// <summary>
        /// 文档内边距
        /// </summary>
        public double Padding { get; set; }

        /// <summary>
        /// 文档所能显示的最大列数
        /// </summary>
        public int ViewportColumn { get; set; }

        /// <summary>
        /// 文档所能显示的最大行数
        /// </summary>
        public int ViewportRow { get; set; }

        /// <summary>
        /// DECPrivateAutoWrapMode是否启用
        /// </summary>
        public bool DECPrivateAutoWrapMode { get; set; }

        /// <summary>
        /// 光标样式
        /// </summary>
        public VTCursorStyles CursorStyle { get; set; }

        /// <summary>
        /// 光标闪烁速度
        /// </summary>
        public VTCursorSpeeds CursorSpeed { get; set; }

        /// <summary>
        /// 光标颜色
        /// </summary>
        public string CursorColor { get; set; }
        
        /// <summary>
        /// 光标高度
        /// </summary>
        public VTSize CursorSize { get; set; }

        /// <summary>
        /// 字体样式
        /// </summary>
        public string FontFamily { get; set; }

        /// <summary>
        /// 字体大小
        /// </summary>
        public int FontSize { get; set; }

        /// <summary>
        /// 文本前景色
        /// </summary>
        public string ForegroundColor { get; set; }

        /// <summary>
        /// 背景色
        /// </summary>
        public string BackgroundColor { get; set; }

        /// <summary>
        /// 颜色表
        /// </summary>
        public VTColorTable ColorTable { get; set; }

        /// <summary>
        /// 终端的缩放模式
        /// </summary>
        public TerminalSizeModeEnum SizeMode { get; set; }
    }
}

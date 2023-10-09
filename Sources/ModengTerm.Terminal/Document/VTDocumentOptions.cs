using ModengTerm.Terminal;
using ModengTerm.Terminal.DataModels;
using ModengTerm.Terminal.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTerminal.Base;
using XTerminal.Base.Enumerations;

namespace ModengTerm.Terminal.Document
{
    public class VTDocumentOptions
    {
        /// <summary>
        /// 渲染客户端接口
        /// </summary>
        public IDrawingWindow DrawingWindow { get; set; }

        /// <summary>
        /// 文档所能显示的最大列数
        /// </summary>
        public int ColumnSize { get; set; }

        /// <summary>
        /// 文档所能显示的最大行数
        /// </summary>
        public int RowSize { get; set; }

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
    }
}

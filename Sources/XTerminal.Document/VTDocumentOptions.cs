using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTerminal.Base;
using XTerminal.Document.Rendering;

namespace XTerminal.Document
{
    public class VTDocumentOptions
    {
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
        /// 光标闪烁间隔时间
        /// </summary>
        public int Interval { get; set; }

        /// <summary>
        /// 画布创建器
        /// </summary>
        public ITerminalScreen CanvasCreator { get; set; }
    }
}

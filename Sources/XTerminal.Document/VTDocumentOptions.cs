using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XTerminal.Document
{
    public class VTDocumentOptions
    {
        /// <summary>
        /// 文档所能显示的最大列数
        /// </summary>
        public int Columns { get; set; }

        /// <summary>
        /// 文档所能显示的最大行数
        /// </summary>
        public int Rows { get; set; }

        /// <summary>
        /// DECPrivateAutoWrapMode是否启用
        /// </summary>
        public bool DECPrivateAutoWrapMode { get; set; }

        /// <summary>
        /// 光标样式
        /// </summary>
        public VTCursorStyles CursorStyle { get; set; }

        /// <summary>
        /// 闪烁间隔时间
        /// </summary>
        public int Interval { get; set; }
    }
}

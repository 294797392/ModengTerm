using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTerminal.Base;
using XTerminal.Base.Enumerations;
using XTerminal.Document.Rendering;
using XTerminal.Parser;

namespace XTerminal.Document
{
    /// <summary>
    /// 光标的数据模型
    /// </summary>
    public class VTCursor : VTDocumentElement
    {
        public override VTDocumentElements Type => VTDocumentElements.Cursor;

        /// <summary>
        /// 是否需要重绘
        /// </summary>
        public bool IsDirty { get; private set; }

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
        /// 闪烁速度
        /// </summary>
        public VTCursorSpeeds BlinkSpeed { get; set; }

        /// <summary>
        /// 是否是显示状态
        /// </summary>
        public bool IsVisible { get; set; }

        public void SetDirty(bool isDirty)
        {
            if (this.IsDirty != isDirty)
            {
                this.IsDirty = isDirty;
            }
        }
    }
}

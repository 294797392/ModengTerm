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
        private static log4net.ILog logger = log4net.LogManager.GetLogger("VTCursor");

        private bool visibleChanged;
        private bool isVisible;
        private int column;
        private int row;

        public override VTDocumentElements Type => VTDocumentElements.Cursor;

        /// <summary>
        /// 光标所在列
        /// </summary>
        public int Column
        {
            get { return this.column; }
            internal set
            {
                if (this.column!= value)
                {
                    this.column = value;
                    this.SetArrangeDirty(true);
                }
            }
        }

        /// <summary>
        /// 光标所在行
        /// </summary>
        public int Row
        {
            get { return this.row; }
            internal set
            {
                if (this.row != value)
                {
                    this.row = value;
                    this.SetArrangeDirty(true);
                }
            }
        }

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
        public bool IsVisible 
        {
            get { return this.isVisible; }
            set
            {
                if (this.isVisible != value)
                {
                    this.isVisible = value;

                    if (!this.visibleChanged)
                    {
                        this.visibleChanged = true;
                    }
                }
            }
        }

        public override void RequestInvalidate()
        {
            if (this.arrangeDirty)
            {
                this.DrawingContext.Arrange(this.OffsetX, this.OffsetY);

                this.arrangeDirty = false;
            }

            if (this.visibleChanged)
            {
                if (this.isVisible)
                {
                    this.DrawingContext.SetOpacity(1);
                }
                else
                {
                    this.DrawingContext.SetOpacity(0);
                }

                this.visibleChanged = false;
            }
        }
    }
}

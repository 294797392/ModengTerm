using ModengTerm.Terminal;
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

        #region 实例变量

        private bool isVisible;
        private int column;
        private int row;
        private bool dirty;
        private bool blinkState;
        private bool blinkAllowed;
        private VTDocument ownerDocument;

        #endregion

        #region 属性

        public override VTDocumentElements Type => VTDocumentElements.Cursor;

        /// <summary>
        /// 光标所在列
        /// 从0开始，最大值是终端的ColumnSize - 1
        /// </summary>
        public int Column
        {
            get { return this.column; }
            internal set
            {
                if (this.column != value)
                {
                    this.column = value;
                    this.SetArrangeDirty(true);
                }
            }
        }

        /// <summary>
        /// 光标所在行
        /// 从0开始，最大值是终端的RowSize - 1
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
        /// 光标大小
        /// </summary>
        public VTSize Size { get; set; }

        /// <summary>
        /// 光标颜色
        /// </summary>
        public string Color { get; set; }

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
                    this.SetDirty(true);
                }
            }
        }

        /// <summary>
        /// true表示显示
        /// false表示隐藏
        /// </summary>
        public bool BlinkState
        {
            get { return this.blinkState; }
            set
            {
                if (this.blinkState != value)
                {
                    this.blinkState = value;
                    this.SetDirty(true);
                }
            }
        }

        /// <summary>
        /// 是否允许光标闪烁
        /// </summary>
        public bool AllowBlink
        {
            get { return this.blinkAllowed; }
            set
            {
                if (this.blinkAllowed != value)
                {
                    this.blinkAllowed = value;
                    this.SetDirty(true);
                }
            }
        }

        #endregion

        #region 构造方法

        public VTCursor(VTDocument ownerDocument)
        {
            this.ownerDocument = ownerDocument;
        }

        #endregion

        #region 实例方法

        private void SetDirty(bool dirty)
        {
            if (this.dirty != dirty)
            {
                this.dirty = dirty;
            }
        }

        #endregion

        public override void RequestInvalidate()
        {
            VTextLine cursorLine = this.ownerDocument.FindLine(this.ownerDocument.ActivePhysicsRow);
            if (cursorLine == null)
            {
                // 光标所在行不可见
                // 此时说明有滚动，有滚动的情况下直接隐藏光标
                this.DrawingObject.SetVisible(false);
            }
            else
            {
                // 说明光标所在行可见

                // 设置光标是否可以显示
                this.DrawingObject.SetVisible(this.isVisible);

                // 可以显示的话再执行下面的
                if (this.isVisible)
                {
                    // 设置光标位置
                    // 有可能有中文字符，一个中文字符占用2列
                    // 先通过光标所在列找到真正的字符所在列
                    int characterIndex = cursorLine.FindCharacterIndex(this.column - 1);
                    VTRect rect = cursorLine.MeasureTextBlock(characterIndex, 1);
                    double offsetX = rect.Right;
                    double offsetY = cursorLine.OffsetY;
                    this.DrawingObject.Arrange(offsetX, offsetY);

                    // 当前可以显示光标
                    if (this.AllowBlink)
                    {
                        // 允许光标闪烁
                        if (this.blinkState)
                        {
                            this.DrawingObject.SetVisible(true);
                        }
                        else
                        {
                            this.DrawingObject.SetVisible(false);
                        }
                    }
                    else
                    {
                        // 不允许闪烁光标
                    }
                }
            }
        }
    }
}

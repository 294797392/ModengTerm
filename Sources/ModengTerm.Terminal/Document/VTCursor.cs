using ModengTerm.Terminal;
using ModengTerm.Terminal.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTerminal.Base;
using XTerminal.Base.Enumerations;
using XTerminal.Parser;

namespace ModengTerm.Terminal.Document
{
    /// <summary>
    /// 光标的数据模型
    /// </summary>
    public class VTCursor : VTFramedDocumentElement<IDrawingCursor>
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("VTCursor");

        #endregion

        #region 实例变量

        private int column;
        private int row;
        private bool blinkState;
        private bool blinkAllowed;
        private int characterIndex;
        private VTCursorStyles style;
        private string color;

        #endregion

        #region 属性

        /// <summary>
        /// 光标样式
        /// </summary>
        public VTCursorStyles Style
        {
            get { return this.style; }
            set
            {
                if (this.style != value)
                {
                    this.style = value;
                }
            }
        }

        /// <summary>
        /// 光标颜色
        /// </summary>
        public string Color
        {
            get { return this.color; }
            set
            {
                if (this.color != value)
                {
                    this.color = value;
                }
            }
        }

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
                    this.SetDirtyFlags(VTDirtyFlags.PositionDirty, true);
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
                    this.SetDirtyFlags(VTDirtyFlags.PositionDirty, true);
                }
            }
        }

        /// <summary>
        /// 光标所在位置的前一个字符索引（光标永远都是在下一个要打印的字符位置上）
        /// 注意该字符索引不等于Column，因为有可能一个字符占多列
        /// 在VTDocument.SetCurosr时候设置
        /// </summary>
        public int CharacterIndex
        {
            get { return this.characterIndex; }
            set
            {
                if (this.characterIndex != value)
                {
                    this.characterIndex = value;
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
                }
            }
        }

        /// <summary>
        /// 字体样式信息，用来计算光标大小
        /// </summary>
        public VTypeface Typeface { get; set; }

        #endregion

        #region 构造方法

        public VTCursor(VTDocument ownerDocument) :
            base(ownerDocument)
        {
            this.ownerDocument = ownerDocument;
        }

        #endregion

        #region 实例方法

        #endregion

        #region 公开接口

        protected override void OnInitialize()
        {
            switch (this.style)
            {
                case VTCursorStyles.Block:
                    {
                        this.DrawingObject.Size = new VTRect(0, 0, this.Typeface.SpaceWidth, this.Typeface.Height);
                        break;
                    }

                case VTCursorStyles.Line:
                    {
                        this.DrawingObject.Size = new VTRect(0, 0, 2, this.Typeface.Height);
                        break;
                    }

                case VTCursorStyles.Underscore:
                    {
                        this.DrawingObject.Size = new VTRect(0, this.Typeface.Height - 2, this.Typeface.SpaceWidth, 2);
                        break;
                    }

                case VTCursorStyles.None:
                    {
                        this.DrawingObject.Size = new VTRect();
                        break;
                    }

                default:
                    throw new NotImplementedException();
            }

            this.DrawingObject.Color = this.color;
            this.DrawingObject.Style = this.style;
        }

        protected override void OnRelease()
        {
        }

        /// <summary>
        /// 重绘光标位置
        /// 每次收到数据渲染完之后调用
        /// </summary>
        public void RequestInvalidatePosition()
        {
            if (this.GetDirtyFlags(VTDirtyFlags.VisibleDirty))
            {
                this.DrawingObject.SetOpacity(this.IsVisible ? 1 : 0);

                this.ResetDirtyFlags();
            }

            if (!this.IsVisible)
            {
                return;
            }

            VTextLine cursorLine = this.ownerDocument.ActiveLine;
            if (cursorLine == null)
            {
                return;
            }

            // 设置光标位置
            // 有可能有中文字符，一个中文字符占用2列
            // 先通过光标所在列找到真正的字符所在列
            int characterIndex = this.characterIndex;
            // 字符大于0才去测量
            VTRect rect = cursorLine.MeasureTextBlock(characterIndex, 1);
            double offsetX = rect.Right;
            double offsetY = cursorLine.OffsetY;
            this.DrawingObject.Arrange(offsetX, offsetY);
        }

        /// <summary>
        /// 重绘光标闪烁
        /// 在闪烁定时器里调用
        /// </summary>
        public override void RequestInvalidate()
        {
            VTextLine cursorLine = this.ownerDocument.ActiveLine;

            if (cursorLine == null)
            {
                // 光标所在行不可见
                // 此时说明有滚动，有滚动的情况下直接隐藏光标
                // 滚动之后会调用VTDocument.SetCursorPhysicsRow重新设置光标所在物理行号，这个时候有可能ActiveLine就是空的
                this.DrawingObject.SetOpacity(0);
            }
            else
            {
                // 说明光标所在行可见

                // 设置光标是否可以显示
                this.DrawingObject.SetOpacity(this.IsVisible ? 1 : 0);

                // 可以显示的话再执行下面的
                if (this.IsVisible)
                {
                    // 当前可以显示光标
                    if (this.AllowBlink)
                    {
                        // 允许光标闪烁，改变光标并闪烁
                        this.blinkState = !this.blinkState;
                        this.DrawingObject.SetOpacity(this.blinkState ? 1 : 0);
                    }
                    else
                    {
                        // 不允许闪烁光标
                    }
                }
            }
        }

        #endregion
    }
}

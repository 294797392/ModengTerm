using ModengTerm.Document.Enumerations;
using ModengTerm.Document.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Document
{
    /// <summary>
    /// 光标的数据模型
    /// </summary>
    public class VTCursor : VTElement
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
        private int interval;

        #endregion

        #region 属性

        public override DrawingObjectTypes Type => DrawingObjectTypes.Cursor;

        /// <summary>
        /// 光标样式
        /// </summary>
        public VTCursorStyles Style
        {
            get { return style; }
            set
            {
                if (style != value)
                {
                    style = value;
                }
            }
        }

        /// <summary>
        /// 光标颜色
        /// </summary>
        public string Color
        {
            get { return color; }
            set
            {
                if (color != value)
                {
                    color = value;
                }
            }
        }

        /// <summary>
        /// 光标所在列
        /// 从0开始，最大值是终端的ColumnSize - 1
        /// </summary>
        public int Column
        {
            get { return column; }
            internal set
            {
                if (column != value)
                {
                    column = value;
                    SetDirtyFlags(VTDirtyFlags.PositionDirty, true);
                }
            }
        }

        /// <summary>
        /// 光标所在行
        /// 从0开始，最大值是终端的RowSize - 1
        /// </summary>
        public int Row
        {
            get { return row; }
            internal set
            {
                if (row != value)
                {
                    row = value;
                    SetDirtyFlags(VTDirtyFlags.PositionDirty, true);
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
            get { return characterIndex; }
            set
            {
                if (characterIndex != value)
                {
                    characterIndex = value;
                }
            }
        }

        /// <summary>
        /// 是否允许光标闪烁
        /// </summary>
        public bool AllowBlink
        {
            get { return blinkAllowed; }
            set
            {
                if (blinkAllowed != value)
                {
                    blinkAllowed = value;
                }
            }
        }

        /// <summary>
        /// 字体样式信息，用来计算光标大小
        /// </summary>
        public VTypeface Typeface { get; set; }

        /// <summary>
        /// 光标闪烁的间隔时间
        /// </summary>
        public int Interval 
        {
            get { return this.interval; }
            set
            {
                if (this.interval != value)
                {
                    this.interval = value;
                }
            }
        }

        #endregion

        #region 构造方法

        public VTCursor(VTDocument ownerDocument) :
            base(ownerDocument)
        {
        }

        #endregion

        #region 实例方法

        #endregion

        #region VTElement

        protected override void OnInitialize(IDrawingObject drawingObject)
        {
            IDrawingCursor drawingCursor = drawingObject as IDrawingCursor;

            switch (style)
            {
                case VTCursorStyles.Block:
                    {
                        drawingCursor.Size = new VTRect(0, 0, Typeface.SpaceWidth, Typeface.Height);
                        break;
                    }

                case VTCursorStyles.Line:
                    {
                        drawingCursor.Size = new VTRect(0, 0, 2, Typeface.Height);
                        break;
                    }

                case VTCursorStyles.Underscore:
                    {
                        drawingCursor.Size = new VTRect(0, Typeface.Height - 2, Typeface.SpaceWidth, 2);
                        break;
                    }

                case VTCursorStyles.None:
                    {
                        drawingCursor.Size = new VTRect();
                        break;
                    }

                default:
                    throw new NotImplementedException();
            }

            drawingCursor.Color = color;
            drawingCursor.Style = style;
        }

        protected override void OnRelease()
        {
        }

        /// <summary>
        /// 重绘光标位置
        /// 每次收到数据渲染完之后调用
        /// </summary>
        protected override void OnRender()
        {
            VTextLine cursorLine = this.OwnerDocument.ActiveLine;
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
            DrawingObject.Arrange(offsetX, offsetY);
        }

        #endregion

        #region 公开接口

        /// <summary>
        /// 重绘光标闪烁
        /// 在闪烁定时器里调用
        /// </summary>
        public void Flash()
        {
            VTextLine activeLine = this.OwnerDocument.ActiveLine;

            if (activeLine == null)
            {
                // 光标所在行不可见
                // 此时说明有滚动，有滚动的情况下直接隐藏光标
                // 滚动之后会调用VTDocument.SetCursorPhysicsRow重新设置光标所在物理行号，这个时候有可能ActiveLine就是空的
                DrawingObject.SetOpacity(0);
            }
            else
            {
                // 说明光标所在行可见

                // 设置光标是否可以显示
                DrawingObject.SetOpacity(IsVisible ? 1 : 0);

                // 可以显示的话再执行下面的
                if (IsVisible)
                {
                    // 当前可以显示光标
                    if (AllowBlink)
                    {
                        // 允许光标闪烁，改变光标并闪烁
                        blinkState = !blinkState;
                        DrawingObject.SetOpacity(blinkState ? 1 : 0);
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

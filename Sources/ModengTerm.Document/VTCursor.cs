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
    /// 定义光标类型
    /// </summary>
    public enum VTCursorStyles
    {
        /// <summary>
        /// 不显示光标
        /// </summary>
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
        /// 表示下一个要打印的字符的位置
        /// </summary>
        public int Column
        {
            get { return column; }
            internal set
            {
                if (column != value)
                {
                    column = value;
                    SetDirtyFlags(VTDirtyFlags.RenderDirty, true);
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
                    SetDirtyFlags(VTDirtyFlags.RenderDirty, true);
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
            VTextLine activeLine = this.OwnerDocument.ActiveLine;
            if (activeLine == null)
            {
                // 光标不存在
                // 不要使用SetOpticty隐藏光标，只有闪烁线程才可以调用SetOpacty显隐光标，其他地方如果调用的话会导致光标闪烁不流畅
                DrawingObject.Arrange(-9999, -9999);
                return;
            }

            // 设置光标位置

            // 光标在这个索引位置的字符后面
            int characterIndex = 0;

            if (this.column == activeLine.Columns)
            {
                // 此时说明光标在最后一个字符的后面
                characterIndex = activeLine.Characters.Count - 1;
            }
            else if (this.column < activeLine.Columns)
            {
                // 此时说明光标在某个字符后面
                characterIndex = activeLine.FindCharacterIndex(this.column);
            }
            else
            {
                // 此时说明光标位置已经超出了最后一个字符的位置
                // 不做处理
                logger.DebugFormat("光标超出了最后一个字符的位置, 渲染光标失败");
                return;
            }

            VTextRange textRange = activeLine.MeasureCharacter(characterIndex);
            double offsetX = textRange.OffsetX + textRange.Width;
            double offsetY = activeLine.OffsetY;
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

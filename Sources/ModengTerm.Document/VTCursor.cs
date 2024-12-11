using ModengTerm.Document.Enumerations;
using ModengTerm.Document.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModengTerm.Document.Utility;
using System.Windows.Media.Animation;
using System.Windows;
using System.Windows.Media.Converters;
using System.Windows.Controls.Primitives;
using System.Reflection.Metadata;

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
        private int physicsRow;
        private bool blinkState;
        private bool blinkAllowed;
        private VTCursorStyles style;
        private string color;
        private int interval;
        private bool positionChanged;

        #endregion

        #region 属性

        public override GraphicsObjectTypes Type => GraphicsObjectTypes.Cursor;

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
        /// 从0开始，最大值是终端的ViewportColumn - 1
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

                    this.positionChanged = true;
                }
            }
        }

        /// <summary>
        /// 光标所在行
        /// 从0开始，最大值是终端的ViewportRow - 1
        /// </summary>
        public int Row
        {
            get { return row; }
            internal set
            {
                if (row != value)
                {
                    row = value;

                    this.positionChanged = true;
                }
            }
        }

        /// <summary>
        /// 获取光标所在的物理行号
        /// 使用此值可以判断光标当前是否在可视区域内显示
        /// 在什么时候需要更新：
        /// 1. 增加，删除行（暂时没有方法）
        /// 3. 设置光标逻辑位置的时候（SetCursor）
        /// </summary>
        public int PhysicsRow 
        {
            get { return this.physicsRow; }
            internal set
            {
                if (this.physicsRow != value) 
                {
                    this.physicsRow = value;

                    this.positionChanged = true;
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
        /// 单位是毫秒
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

        /// <summary>
        /// 获取光标的宽度
        /// </summary>
        public double Width { get; private set; }

        /// <summary>
        /// 获取光标高度
        /// </summary>
        public double Height { get; private set; }

        /// <summary>
        /// 获取距离DrawingArea的底部的距离
        /// </summary>
        public double Bottom
        {
            get
            {
                return this.OffsetY + this.Height;
            }
        }

        /// <summary>
        /// 获取距离DrawingArea的顶部的距离
        /// </summary>
        public double Top
        {
            get
            {
                return this.OffsetY;
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

        protected override void OnInitialize()
        {
            VTColor backColor = VTColor.CreateFromRgbKey(this.color);

            switch (style)
            {
                case VTCursorStyles.Block:
                    {
                        VTRect cursorRect = new VTRect(0, 0, Typeface.Width, Typeface.Height);
                        this.DrawingObject.DrawRectangle(cursorRect, null, backColor);
                        break;
                    }

                case VTCursorStyles.Line:
                    {
                        VTRect cursorRect = new VTRect(0, 0, 2, Typeface.Height);
                        this.DrawingObject.DrawRectangle(cursorRect, null, backColor);
                        break;
                    }

                case VTCursorStyles.Underscore:
                    {
                        VTRect cursorRect = new VTRect(0, Typeface.Height - 2, Typeface.Width, 2);
                        this.DrawingObject.DrawRectangle(cursorRect, null, backColor);
                        break;
                    }

                case VTCursorStyles.None:
                    {
                        VTRect cursorRect = new VTRect();
                        this.DrawingObject.DrawRectangle(cursorRect, null, backColor);
                        break;
                    }

                default:
                    throw new NotImplementedException();
            }

            this.Width = this.Typeface.Width;
            this.Height = this.Typeface.Height;
        }

        protected override void OnRelease()
        {
        }

        protected override void OnRender()
        {

        }

        #endregion

        #region 公开接口

        /// <summary>
        /// 闪烁光标
        /// 在闪烁定时器里调用
        /// </summary>
        public void Flash()
        {
            VTDocument document = this.OwnerDocument;

            if (document.OutsideViewport(this.PhysicsRow))
            {
                // 光标在可视区域外
                // 此时说明有滚动，有滚动的情况下直接隐藏光标
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

        /// <summary>
        /// 根据当前的CursorRow和CursorCol对光标进行重新定位
        /// 每次收到数据渲染完之后调用
        /// </summary>
        public void Reposition()
        {
            // 如果光标位置没改变，那么不用动
            if (!this.positionChanged)
            {
                return;
            }

            this.positionChanged = false;

            double newOffsetX = 99999, newOffsetY = 99999;
            VTDocument document = this.OwnerDocument;

            if (document.OutsideViewport(this.PhysicsRow))
            {
                // 光标在可视区域外
                // 不要使用SetOpticty隐藏光标，只有闪烁线程才可以调用SetOpacty显隐光标，其他地方如果调用的话会导致光标闪烁不流畅
                // 不能使用负数，负数会导致鼠标事件出问题，直接把光标移动到99999的位置隐藏光标
            }
            else
            {
                // 光标在可视区域内，直接计算光标的像素位置
                newOffsetX = this.column * this.Typeface.Width;
                newOffsetY = this.row * this.Typeface.Height;
            }

            // 光标位置没变化
            if (newOffsetX == this.OffsetX && newOffsetY == this.OffsetY)
            {
                return;
            }

            this.OffsetX = newOffsetX;
            this.OffsetY = newOffsetY;

            DrawingObject.Arrange(newOffsetX, newOffsetY);
        }

        #endregion
    }
}

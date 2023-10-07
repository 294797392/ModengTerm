using ModengTerm.Terminal.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal.Document
{
    /// <summary>
    /// 指定文档上的元素类型
    /// </summary>
    public enum VTDocumentElements
    {
        /// <summary>
        /// 文本行
        /// </summary>
        TextLine,

        /// <summary>
        /// 光标
        /// </summary>
        Cursor,

        /// <summary>
        /// 选中区域
        /// </summary>
        SelectionRange,

        /// <summary>
        /// 矩形
        /// </summary>
        Rectangle,

        /// <summary>
        /// 滚动条
        /// </summary>
        Scrollbar,

        /// <summary>
        /// 匹配的行
        /// </summary>
        MatchesLine,

        /// <summary>
        /// 背景
        /// </summary>
        Wallpaper
    }

    /// <summary>
    /// 表示文档上的一个元素
    /// </summary>
    public abstract class VTDocumentElement
    {
        #region 实例变量

        protected bool arrangeDirty;

        private double offsetX;
        private double offsetY;

        /// <summary>
        /// 该对象的类型
        /// </summary>
        public abstract VTDocumentElements Type { get; }

        /// <summary>
        /// 用来保存不同平台的绘图上下文信息
        /// </summary>
        public IDrawingObject DrawingObject { get; set; }

        /// <summary>
        /// 该元素左上角的X坐标
        /// </summary>
        public double OffsetX 
        {
            get { return this.offsetX; }
            set
            {
                if (this.offsetX != value)
                {
                    this.offsetX = value;
                    this.SetArrangeDirty(true);
                }
            }
        }

        /// <summary>
        /// 该元素左上角的Y坐标
        /// </summary>
        public double OffsetY 
        {
            get { return this.offsetY; }
            set
            {
                if (this.offsetY != value)
                {
                    this.offsetY = value;
                    this.SetArrangeDirty(true);
                }
            }
        }

        #endregion

        #region 构造方法

        public VTDocumentElement()
        {
        }

        #endregion

        #region 公开接口

        /// <summary>
        /// 请求重绘
        /// 首先判断该绘图元素是否需要重绘
        /// 需要再重绘
        /// </summary>
        public abstract void RequestInvalidate();

        #endregion

        #region 实例方法

        protected void SetArrangeDirty(bool dirty)
        {
            if (this.arrangeDirty != dirty)
            {
                this.arrangeDirty = dirty;
            }
        }

        #endregion
    }
}

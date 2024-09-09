using ModengTerm.Document.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Document
{
    /// <summary>
    /// 封装VTScrollbar对象
    /// 对VTScrollbar对象的状态进行维护并调用VTScrollbar接口更新UI
    /// </summary>
    public class VTScrollInfo
    {
        #region 类变量

        internal static readonly log4net.ILog logger = log4net.LogManager.GetLogger("VTScrollInfo");

        #endregion

        #region 实例变量

        private bool display;
        private int scrollMax;
        private int scrollValue;
        private int viewportRow;
        private VTScrollbar scrollbar;
        private VTDocument ownerDocument;

        #endregion

        #region 属性

        /// <summary>
        /// 获取当前显示的第一行的物理行号
        /// </summary>
        public int FirstPhysicsRow { get { return this.Value; } }

        /// <summary>
        /// 获取当前显示的最后一行的物理行号
        /// </summary>
        public int LastPhysicsRow { get { return this.Value + this.ownerDocument.ViewportRow - 1; } }

        /// <summary>
        /// 可以滚动到的最大值
        /// 也就是滚动条滚动到底的时候，滚动条的值
        /// 也就是滚动条滚动到底的时候，文档里的第一行的PhysicsRow
        /// </summary>
        public int Maximum
        {
            get { return scrollMax; }
            set
            {
                if (scrollMax != value)
                {
                    scrollMax = value;
                }
            }
        }

        /// <summary>
        /// 可以滚动到的最小值
        /// 该值永远是0
        /// </summary>
        public int Minimum { get; private set; }

        /// <summary>
        /// 滚动条的值
        /// 也就是当前Document上渲染的第一行的PhysicsRow
        /// 默认值是0
        /// </summary>
        public int Value
        {
            get { return scrollValue; }
            set
            {
                if (scrollValue != value)
                {
                    scrollValue = value;
                }
            }
        }

        /// <summary>
        /// 获取滚动条是否包含可滚动的内容
        /// </summary>
        public bool HasScroll
        {
            get
            {
                return Maximum > 0;
            }
        }

        /// <summary>
        /// 获取当前滚动条是否滚动到底了
        /// </summary>
        public bool ScrollAtBottom
        {
            get
            {
                return Value == Maximum;
            }
        }

        /// <summary>
        /// 获取当前滚动条是否滚动到顶了
        /// </summary>
        public bool ScrollAtTop
        {
            get
            {
                return Value == Minimum;
            }
        }

        /// <summary>
        /// 文档可视区域的行数
        /// </summary>
        public int ViewportRow
        {
            get { return this.viewportRow; }
            set
            {
                if (this.viewportRow != value)
                {
                    this.viewportRow = value;
                }
            }
        }

        #endregion

        #region 构造方法

        public VTScrollInfo(VTDocument ownerDocument)
        {
            this.ownerDocument = ownerDocument;
        }

        #endregion

        public void Initialize()
        {
            this.scrollbar = ownerDocument.Renderer.Scrollbar;
            this.scrollbar.Maximum = 0;
            this.scrollbar.Value = 0;
            this.scrollbar.Visible = false; // 默认不显示滚动条
            this.scrollbar.Document = this.ownerDocument;
        }

        public void Release()
        {
        }

        public void RequestInvalidate()
        {
            if (!HasScroll)
            {
                // 此时说明没有滚动，那么隐藏滚动条
                this.scrollbar.Visible = false;
                display = false;
            }
            else
            {
                if (!display)
                {
                    this.scrollbar.Visible = true;
                    display = true;
                }

                this.scrollbar.ViewportRow = this.viewportRow;
                this.scrollbar.Maximum = this.Maximum;
                this.scrollbar.Value = this.Value;
            }
        }
    }
}

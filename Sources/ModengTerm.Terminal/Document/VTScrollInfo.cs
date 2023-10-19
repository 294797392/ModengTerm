using ModengTerm.Terminal.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal.Document
{
    public abstract class VTScrollInfo : VTDocumentElement<IDrawingScrollbar>
    {
        internal static readonly log4net.ILog logger = log4net.LogManager.GetLogger("VTScrollInfo");

        protected bool dirty;
        private int scrollMax;
        private int scrollValue;
        private int viewportRow;
        protected VTHistory history;

        public override VTDocumentElements Type => VTDocumentElements.Scrollbar;

        /// <summary>
        /// 最多可以有多少滚动的行数
        /// </summary>
        public int ScrollbackMax { get; set; }

        /// <summary>
        /// 可以滚动到的最大值
        /// 也就是滚动条滚动到底的时候，滚动条的值
        /// 也就是滚动条滚动到底的时候，文档里的第一行的PhysicsRow
        /// </summary>
        public int ScrollMax
        {
            get { return this.scrollMax; }
            set
            {
                if (this.scrollMax != value)
                {
                    this.scrollMax = value;
                    this.SetDirty(true);

                    if (this.scrollMax > this.ScrollbackMax)
                    {
                        // 算出来当前一共有多少行可以滚动
                        int scrolls = this.scrollMax - this.FirstLine.PhysicsRow;

                        // 滚动的行数大于可以滚动的最大行数
                        if (scrolls > this.ScrollbackMax)
                        {
                            // 多了多少行，要把这些行删除
                            int count = scrolls - this.ScrollbackMax;
                            if (count > 0)
                            {
                                for (int i = 0; i < count; i++)
                                {
                                    this.RemoveFirst();
                                }

                                this.ScrollMin += count;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 可以滚动到的最小值
        /// 也就是滚动条滚动到顶的时候，文档里的第一行的PhysicsRow
        /// 这个值在ScrollMax改变的时候更新（在ScrollMax的属性set访问器里更新）
        /// </summary>
        public int ScrollMin { get; private set; }

        /// <summary>
        /// 滚动条的值
        /// 也就是当前Document上渲染的第一行的PhysicsRow
        /// 默认值是0
        /// </summary>
        public int ScrollValue
        {
            get { return this.scrollValue; }
            set
            {
                if (this.scrollValue != value)
                {
                    this.scrollValue = value;
                    this.SetDirty(true);
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
                return this.ScrollMax > 0;
            }
        }

        /// <summary>
        /// 获取当前滚动条是否滚动到底了
        /// </summary>
        public bool ScrollAtBottom
        {
            get
            {
                return this.ScrollValue == this.ScrollMax;
            }
        }

        /// <summary>
        /// 获取当前滚动条是否滚动到顶了
        /// </summary>
        public bool ScrollAtTop
        {
            get
            {
                return this.ScrollValue == this.ScrollMin;
            }
        }

        /// <summary>
        /// 可视区域的行数
        /// </summary>
        public int ViewportRow 
        {
            get { return this.viewportRow; }
            set
            {
                if (this.viewportRow != value) 
                {
                    this.viewportRow = value;
                    this.SetDirty(true);
                }
            }
        }

        /// <summary>
        /// 可滚动内容的第一行记录
        /// </summary>
        public VTHistoryLine FirstLine { get; protected set; }

        /// <summary>
        /// 可滚动内容的最后一行记录
        /// </summary>
        public VTHistoryLine LastLine { get; protected set; }

        public VTScrollInfo(VTDocument ownerDocument) :
            base(ownerDocument)
        {
        }

        #region 实例方法

        protected void SetDirty(bool dirty)
        {
            if (this.dirty != dirty)
            {
                this.dirty = dirty;
            }
        }

        /// <summary>
        /// 移除第一行并更新第一行的指针
        /// </summary>
        private void RemoveFirst()
        {
            this.history.RemoveHistory(this.FirstLine);

            VTHistoryLine firstLine;
            if (!this.history.TryGetHistory(this.FirstLine.PhysicsRow + 1, out firstLine))
            {
                // 不可能发生
                throw new NotImplementedException();
            }

            this.FirstLine = firstLine;
        }

        #endregion

        #region 公开接口

        /// <summary>
        /// 更新文本行对应的历史记录
        /// 如果不存在历史记录，那么创建历史记录
        /// </summary>
        /// <param name="textLine"></param>
        /// <returns>返回被更新后的历史行数据</returns>
        public VTHistoryLine UpdateHistory(VTextLine textLine)
        {
            VTHistoryLine historyLine;
            if (!this.history.TryGetHistory(textLine.PhysicsRow, out historyLine))
            {
                historyLine = new VTHistoryLine();

                if (textLine.PhysicsRow == 0)
                {
                    this.FirstLine = historyLine;
                    this.LastLine = historyLine;
                }
                else if (textLine.PhysicsRow > this.LastLine.PhysicsRow)
                {
                    this.LastLine = historyLine;
                }

                this.history.UpdateHistory(textLine, historyLine);
                this.history.AddHistory(historyLine);
            }
            else
            {
                this.history.UpdateHistory(textLine, historyLine);
            }

            return historyLine;
        }

        /// <summary>
        /// 从指定行开始获取指定的行数
        /// </summary>
        /// <param name="startRow"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public bool TryGetHistories(int startRow, int count, out List<VTHistoryLine> historyLines)
        {
            historyLines = new List<VTHistoryLine>();

            for (int i = 0; i < count; i++)
            {
                VTHistoryLine historyLine;
                if (!this.history.TryGetHistory(startRow + i, out historyLine))
                {
                    return false;
                }

                historyLines.Add(historyLine);
            }

            return true;
        }

        /// <summary>
        /// 获取指定的历史记录行
        /// </summary>
        /// <param name="physicsRow">要获取的历史记录行</param>
        /// <returns></returns>
        public bool TryGetHistory(int physicsRow, out VTHistoryLine historyLine)
        {
            return this.history.TryGetHistory(physicsRow, out historyLine);
        }

        #endregion

        public static VTScrollInfo Create(bool isAlternate, VTDocument ownerDocument)
        {
            if (isAlternate)
            {
                return new VTAlternateScrollInfo(ownerDocument);
            }
            else
            {
                return new VTMainScrollInfo(ownerDocument);
            }
        }
    }

    public class VTAlternateScrollInfo : VTScrollInfo
    {
        public override VTDocumentElements Type => VTDocumentElements.Scrollbar;

        public VTAlternateScrollInfo(VTDocument ownerDocument) :
            base(ownerDocument)
        {
        }

        public override void Initialize()
        {
            this.history = new VTMemoryHistory();
            this.history.Initialize();

            this.DrawingObject.SetOpacity(0);
        }

        public override void Release()
        {
        }

        public override void RequestInvalidate()
        {
        }
    }

    /// <summary>
    /// 存储滚动条的信息
    /// </summary>
    public class VTMainScrollInfo : VTScrollInfo
    {
        #region 实例变量

        private bool display;

        #endregion

        #region 属性

        #endregion

        #region 构造方法

        public VTMainScrollInfo(VTDocument ownerDocument) :
            base(ownerDocument)
        {
        }

        #endregion

        #region VTScrollInfo

        public override void Initialize()
        {
            this.history = new VTMemoryHistory();
            this.history.Initialize();

            this.DrawingObject.Maximum = 0;
            this.DrawingObject.Value = 0;
            this.DrawingObject.ViewportRow = this.ViewportRow;
            this.display = false;
            this.DrawingObject.SetOpacity(0);
            this.DrawingObject.Initialize();
        }

        public override void Release()
        {
            this.history.Release();

            this.DrawingObject.Release();
        }

        public override void RequestInvalidate()
        {
            if (this.dirty)
            {
                if (!this.HasScroll)
                {
                    // 此时说明没有滚动，那么隐藏滚动条
                    this.DrawingObject.SetOpacity(0);
                    this.display = false;
                }
                else
                {
                    if (!this.display)
                    {
                        this.DrawingObject.SetOpacity(1);
                        this.display = true;
                    }

                    this.DrawingObject.ViewportRow = this.ViewportRow;
                    this.DrawingObject.Maximum = this.ScrollMax;
                    this.DrawingObject.Value = this.ScrollValue;
                }

                this.dirty = false;
            }
        }

        #endregion
    }
}

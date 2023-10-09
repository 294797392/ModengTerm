using ModengTerm.Terminal.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal.Document
{
    /// <summary>
    /// 存储滚动条的信息
    /// </summary>
    public class VTScrollInfo : VTDocumentElement
    {
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger("VTScrollInfo");

        #region 实例变量

        private bool dirty;
        private int scrollMax;
        private int scrollValue;
        private IDrawingWindow vt;
        private VTHistory history;

        #endregion

        #region 属性

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
        /// 可滚动内容的第一行记录
        /// </summary>
        public VTHistoryLine FirstLine { get; private set; }

        /// <summary>
        /// 可滚动内容的最后一行记录
        /// </summary>
        public VTHistoryLine LastLine { get; private set; }

        #endregion

        #region 构造方法

        public VTScrollInfo(IDrawingWindow vt)
        {
            this.vt = vt;
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

        public int Initialize()
        {
            this.history = new VTMemoryHistory();
            return this.history.Initialize();
        }

        public void Release()
        {
            this.history.Release();
        }

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

        #region VTDocumentElement

        public override void RequestInvalidate()
        {
            if (this.dirty)
            {
                this.vt.SetScrollInfo(this);

                this.dirty = false;
            }
        }

        #endregion
    }
}

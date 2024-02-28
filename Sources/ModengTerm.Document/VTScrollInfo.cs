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
        protected VTHistory history;
        private VTScrollbar scrollbar;
        private VTDocument ownerDocument;

        #endregion

        #region 属性

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
            get { return scrollMax; }
            set
            {
                if (scrollMax != value)
                {
                    scrollMax = value;

                    if (scrollMax > ScrollbackMax)
                    {
                        // 算出来当前一共有多少行可以滚动
                        int scrolls = scrollMax - FirstLine.PhysicsRow;

                        // 滚动的行数大于可以滚动的最大行数
                        if (scrolls > ScrollbackMax)
                        {
                            // 多了多少行，要把这些行删除
                            int count = scrolls - ScrollbackMax;
                            if (count > 0)
                            {
                                for (int i = 0; i < count; i++)
                                {
                                    RemoveFirst();
                                }

                                ScrollMin += count;
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
                return ScrollMax > 0;
            }
        }

        /// <summary>
        /// 获取当前滚动条是否滚动到底了
        /// </summary>
        public bool ScrollAtBottom
        {
            get
            {
                return ScrollValue == ScrollMax;
            }
        }

        /// <summary>
        /// 获取当前滚动条是否滚动到顶了
        /// </summary>
        public bool ScrollAtTop
        {
            get
            {
                return ScrollValue == ScrollMin;
            }
        }

        /// <summary>
        /// 可视区域的行数
        /// </summary>
        public int ViewportRow
        {
            get { return viewportRow; }
            set
            {
                if (viewportRow != value)
                {
                    viewportRow = value;
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

        #endregion

        #region 构造方法

        public VTScrollInfo(VTDocument ownerDocument)
        {
            this.ownerDocument = ownerDocument;
        }

        #endregion

        #region 实例方法

        /// <summary>
        /// 移除第一行并更新第一行的指针
        /// </summary>
        private void RemoveFirst()
        {
            history.RemoveHistory(FirstLine);

            VTHistoryLine firstLine;
            if (!history.TryGetHistory(FirstLine.PhysicsRow + 1, out firstLine))
            {
                // 不可能发生
                throw new NotImplementedException();
            }

            FirstLine = firstLine;
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
            if (!history.TryGetHistory(textLine.PhysicsRow, out historyLine))
            {
                historyLine = new VTHistoryLine();

                if (textLine.PhysicsRow == 0)
                {
                    FirstLine = historyLine;
                    LastLine = historyLine;
                }
                else if (textLine.PhysicsRow > LastLine.PhysicsRow)
                {
                    LastLine = historyLine;
                }

                history.UpdateHistory(textLine, historyLine);
                history.AddHistory(historyLine);
            }
            else
            {
                history.UpdateHistory(textLine, historyLine);
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
                if (!history.TryGetHistory(startRow + i, out historyLine))
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
            return history.TryGetHistory(physicsRow, out historyLine);
        }

        #endregion

        public void Initialize()
        {
            history = new VTMemoryHistory();
            history.Initialize();

            this.scrollbar = ownerDocument.DrawingObject.Scrollbar;
            this.scrollbar.Maximum = 0;
            this.scrollbar.Value = 0;
            this.scrollbar.ViewportRow = ViewportRow;
            this.scrollbar.Visible = false; // 默认不显示滚动条
        }

        public void Release()
        {
            history.Release();
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

                this.scrollbar.ViewportRow = this.ViewportRow;
                this.scrollbar.Maximum = this.ScrollMax;
                this.scrollbar.Value = this.ScrollValue;
            }
        }
    }
}

using DotNEToolkit.Utility;
using ModengTerm.Base;
using ModengTerm.Base.DataModels;
using ModengTerm.Terminal;
using ModengTerm.Terminal.Document;
using ModengTerm.Terminal.Enumerations;
using ModengTerm.Terminal.Rendering;
using ModengTerm.Terminal.Session;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTerminal.Base.Definitions;
using XTerminal.Base.Enumerations;
using XTerminal.Parser;
using static ModengTerm.Terminal.Document.VTextLine;

namespace ModengTerm.Terminal.Document
{
    /// <summary>
    /// 终端显示的字符的文档模型
    /// </summary>
    public class VTDocument
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("VTDocument");

        #endregion

        #region 实例变量

        #region SelectionRange

        VTextPointer startPointer;
        VTextPointer endPointer;

        /// <summary>
        /// 鼠标是否按下
        /// </summary>
        private bool isMouseDown;
        private VTPoint mouseDownPos;

        /// <summary>
        /// 当前鼠标是否处于Selection状态
        /// </summary>
        private bool selectionState;

        #endregion

        /// <summary>
        /// 初始化文档的参数
        /// </summary>
        internal VTDocumentOptions options;

        /// <summary>
        /// 可视区域内的行数
        /// </summary>
        private int viewportRow;

        /// <summary>
        /// 可视区域内的列数
        /// </summary>
        private int viewportColumn;

        /// <summary>
        /// 保存光标状态
        /// </summary>
        private VTCursorState cursorState;

        /// <summary>
        /// 光标所在的物理行数
        /// </summary>
        private int cursorPhysicsRow;

        /// <summary>
        /// 鼠标滚轮滚动一次，滚动几行
        /// </summary>
        private int scrollDelta;

        /// <summary>
        /// 终端的大小模式
        /// </summary>
        private TerminalSizeModeEnum sizeMode;

        private VTypeface typeface;

        /// <summary>
        /// 存放该文档的容器
        /// </summary>
        private IDrawingWindow ownerWindow;

        #endregion

        #region 属性

        /// <summary>
        /// 文档的名字，方便调试
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 记录光标信息
        /// </summary>
        public VTCursor Cursor { get; private set; }

        /// <summary>
        /// 记录文档的选中信息
        /// </summary>
        public VTextSelection Selection { get; private set; }

        /// <summary>
        /// 滚动和历史记录信息
        /// </summary>
        public VTScrollInfo Scrollbar { get; private set; }

        public bool DECPrivateAutoWrapMode { get; set; }

        /// <summary>
        /// 当光标在该范围内就得滚动
        /// </summary>
        public int ScrollMarginTop { get; private set; }

        /// <summary>
        /// 当光标在该范围内就得滚动
        /// </summary>
        public int ScrollMarginBottom { get; private set; }

        /// <summary>
        /// 光标所在物理行数
        /// </summary>
        public int CursorPhysicsRow { get { return this.cursorPhysicsRow; } }

        /// <summary>
        /// 当前光标所在行
        /// 通过SetCursor函数设置
        /// </summary>
        public VTextLine ActiveLine { get; private set; }

        /// <summary>
        /// 文档里的第一行
        /// </summary>
        public VTextLine FirstLine { get; internal set; }

        /// <summary>
        /// 文档里的最后一行
        /// </summary>
        public VTextLine LastLine { get; internal set; }

        /// <summary>
        /// 该文档是否是空文档
        /// </summary>
        public bool IsEmpty { get { return this.FirstLine == null && this.LastLine == null; } }

        /// <summary>
        /// 渲染该文档的Canvas
        /// </summary>
        internal IDrawingDocument DrawingObject { get; set; }

        /// <summary>
        /// 是否是备用缓冲区
        /// </summary>
        public bool IsAlternate { get; private set; }

        /// <summary>
        /// 当前应用的文本属性
        /// </summary>
        public VTextAttributeState AttributeState { get; private set; }

        public VTCursorState CursorState { get { return this.cursorState; } }

        /// <summary>
        /// 获取当前滚动条是否滚动到底了
        /// </summary>
        public bool ScrollAtBottom
        {
            get
            {
                return this.Scrollbar.ScrollAtBottom;
            }
        }

        /// <summary>
        /// 获取当前滚动条是否滚动到顶了
        /// </summary>
        public bool ScrollAtTop
        {
            get
            {
                return this.Scrollbar.ScrollAtTop;
            }
        }

        public int ViewportRow { get { return this.viewportRow; } }

        public int ViewportColumn { get { return this.viewportColumn; } }

        /// <summary>
        /// 是否显示书签
        /// </summary>
        public bool BookmarkVisible
        {
            get { return this.DrawingObject.BookmarkVisible; }
            set
            {
                if (this.DrawingObject.BookmarkVisible != value)
                {
                    this.DrawingObject.BookmarkVisible = value;
                }
            }
        }

        /// <summary>
        /// 是否显示滚动条
        /// </summary>
        public bool ScrollbarVisible
        {
            get { return this.DrawingObject.ScrollbarVisible; }
            set
            {
                if (this.DrawingObject.ScrollbarVisible != value)
                {
                    this.DrawingObject.ScrollbarVisible = value;
                }
            }
        }

        /// <summary>
        /// 文档内容的边距
        /// </summary>
        public double ContentMargin
        {
            get { return this.DrawingObject.ContentMargin; }
            set
            {
                if (this.DrawingObject.ContentMargin != value)
                {
                    this.DrawingObject.ContentMargin = value;
                }
            }
        }

        /// <summary>
        /// 获取该文档所使用的字体Typeface
        /// </summary>
        public VTypeface Typeface { get { return this.options.Typeface; } }

        #endregion

        #region 构造方法

        public VTDocument(VTDocumentOptions options, IDrawingDocument drawingObject, bool isAlternate)
        {
            this.options = options;
            this.IsAlternate = isAlternate;
            this.DrawingObject = drawingObject;
            this.cursorState = new VTCursorState();
            this.startPointer = new VTextPointer();
            this.endPointer = new VTextPointer();
            this.scrollDelta = options.ScrollDelta;
            this.sizeMode = options.SizeMode;
            this.typeface = options.Typeface;
            this.AttributeState = new VTextAttributeState();
            this.ContentMargin = options.ContentMargin;
            this.ScrollbarVisible = options.ScrollbarVisible;
            this.BookmarkVisible = options.BookmarkVisible;
        }

        #endregion

        #region 实例方法

        /// <summary>
        /// 创建一个新行，该行不挂在链表上
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private VTextLine CreateTextLine(int physicsRow)
        {
            VTextLine textLine = new VTextLine(this)
            {
                OffsetX = 0,
                OffsetY = 0,
                DECPrivateAutoWrapMode = this.DECPrivateAutoWrapMode,
                PhysicsRow = physicsRow,
                Style = this.options.Typeface
            };

            textLine.Initialize();

            return textLine;
        }

        /// <summary>
        /// 给定第一行的行号，重新设置所有行的行号
        /// </summary>
        /// <param name="newFirstRow">要设置的第一行的行号</param>
        private void ResetPhysicsRow(int newFirstRow)
        {
            this.FirstLine.PhysicsRow = newFirstRow;

            VTextLine currentLine = this.FirstLine.NextLine;
            while (currentLine != null)
            {
                currentLine.PhysicsRow = currentLine.PreviousLine.PhysicsRow + 1;

                currentLine = currentLine.NextLine;
            }
        }

        /// <summary>
        /// 滚动到指定的历史记录
        /// 并更新UI上的滚动条位置
        /// 注意该方法不会重新渲染界面，只修改文档模型
        /// </summary>
        /// <param name="physicsRow1">要显示的第一行历史记录</param>
        /// <returns>如果进行了滚动，那么返回true，如果因为某种原因没进行滚动，那么返回false</returns>
        private bool ScrollToHistory(int physicsRow1)
        {
            // 如果是备用缓冲区则不执行滚动
            if (this.IsAlternate)
            {
                return false;
            }

            int scrollValue = physicsRow1;

            // 要滚动的值和当前值是一样的，也不滚动
            if (this.Scrollbar.ScrollValue == scrollValue)
            {
                return false;
            }

            // 判断要滚动的目标值合法性
            if (scrollValue > this.Scrollbar.ScrollMax ||
                scrollValue < this.Scrollbar.ScrollMin)
            {
                return false;
            }

            // 只移动主缓冲区
            VTDocument scrollDocument = this;

            // 要滚动到的值
            int newScroll = scrollValue;
            // 滚动之前的值
            int oldScroll = this.Scrollbar.ScrollValue;

            // 需要进行滚动的行数
            int scrolledRows = Math.Abs(newScroll - oldScroll);

            // 终端行大小
            int rows = this.viewportRow;

            #region 更新要显示的行

            if (scrolledRows >= rows)
            {
                // 此时说明把所有行都滚动到屏幕外了，需要重新显示所有行

                // 遍历显示
                VTextLine currentTextLine = scrollDocument.FirstLine;
                for (int i = 0; i < rows; i++)
                {
                    int physicsRow = scrollValue + i;
                    VTHistoryLine historyLine;
                    if (this.Scrollbar.TryGetHistory(physicsRow, out historyLine))
                    {
                        currentTextLine.SetHistory(historyLine);
                    }
                    else
                    {
                        // 执行clear指令后，因为会增加行，所以可能会找不到对应的历史记录
                        // 打开终端 -> 输入enter直到翻了一整页 -> 滚动到最上面 -> 输入字符，就会复现
                        currentTextLine.PhysicsRow = physicsRow;
                        currentTextLine.EraseAll();
                    }

                    currentTextLine = currentTextLine.NextLine;
                }
            }
            else
            {
                // 此时说明只需要更新移动出去的行就可以了
                if (newScroll > oldScroll)
                {
                    // 往下滚动，把上面的拿到下面，从第一行开始

                    // 从当前文档的最后一行的下一行开始显示
                    int lastRow = scrollDocument.LastLine.PhysicsRow + 1;

                    for (int i = 0; i < scrolledRows; i++)
                    {
                        // 该值永远是第一行，因为下面被Move到最后一行了
                        VTextLine firstLine = scrollDocument.FirstLine;
                        scrollDocument.MoveLine(firstLine, VTextLine.MoveOptions.MoveToLast);

                        VTHistoryLine historyLine;
                        if (this.Scrollbar.TryGetHistory(lastRow + i, out historyLine))
                        {
                            firstLine.SetHistory(historyLine);
                        }
                        else
                        {
                            // 有可能会找不到该行对应的历史记录，找不到就清空
                            // 打开终端 -> clear -> 滚到最上面 -> 再往下滚，就会复现
                            firstLine.EraseAll();
                        }
                    }
                }
                else
                {
                    // 往上滚动，把下面的拿到上面，从最后一行开始

                    // 从当前文档的第一行的上一行开始显示
                    int firstRow = scrollDocument.FirstLine.PhysicsRow - 1;

                    for (int i = 0; i < scrolledRows; i++)
                    {
                        VTHistoryLine historyLine;
                        if (!this.Scrollbar.TryGetHistory(firstRow - i, out historyLine))
                        {
                            // 百分之百不可能找不到！！！
                            throw new NotImplementedException();
                        }

                        VTextLine lastLine = scrollDocument.LastLine;
                        lastLine.SetHistory(historyLine);
                        scrollDocument.MoveLine(lastLine, VTextLine.MoveOptions.MoveToFirst);
                    }
                }
            }

            #endregion

            // 更新当前滚动条的值
            this.Scrollbar.ScrollValue = scrollValue;

            // 有可能光标所在行被滚动到了文档外，此时要更新ActiveLine，ActiveLine就是空的
            scrollDocument.SetCursorPhysicsRow(scrollDocument.CursorPhysicsRow);

            // 每次滚动都需要重新刷新TextSelection区域
            if (!this.Selection.IsEmpty)
            {
                this.Selection.SetDirty(true);
            }

            return true;
        }

        /// <summary>
        /// 当光标在容器外面移动的时候，进行滚动
        /// </summary>
        /// <param name="mousePosition">当前鼠标的坐标</param>
        /// <param name="documentArea">相对于Document的文档显示区域的位置和大小</param>
        /// <returns>是否执行了滚动动作</returns>
        private void ScrollIfCursorOutsideDocument(VTPoint mousePosition, VTRect documentArea)
        {
            // 要滚动到的目标行
            int scrollTarget = -1;

            if (mousePosition.Y < 0)
            {
                // 光标在容器上面
                if (!this.ScrollAtTop)
                {
                    // 不在最上面，往上滚动一行
                    scrollTarget = this.Scrollbar.ScrollValue - 1;
                }
            }
            else if (mousePosition.Y > documentArea.Height)
            {
                // 光标在容器下面
                if (!this.ScrollAtBottom)
                {
                    // 往下滚动一行
                    scrollTarget = this.Scrollbar.ScrollValue + 1;
                }
            }

            if (scrollTarget != -1)
            {
                this.ScrollToHistory(scrollTarget);
            }
        }

        /// <summary>
        /// 使用像素坐标对VTextLine做命中测试
        /// </summary>
        /// <param name="mousePosition">鼠标坐标</param>
        /// <param name="documentArea">相对于Document的文档显示区域的位置和大小</param>
        /// <param name="pointer">存储命中测试结果的变量</param>
        /// <remarks>如果传递进来的鼠标位置在窗口外，那么会把鼠标限定在距离鼠标最近的Surface边缘处</remarks>
        /// <returns>
        /// 是否获取成功
        /// 当光标不在某一行或者不在某个字符上的时候，就获取失败
        /// </returns>
        private bool GetTextPointer(VTPoint mousePosition, VTRect documentArea, VTextPointer pointer)
        {
            double mouseX = mousePosition.X;
            double mouseY = mousePosition.Y;

            VTDocument document = this;

            #region 先计算鼠标位于哪一行上

            VTextLine cursorLine = null;

            if (mouseY < 0)
            {
                // 光标在画布的上面，那么命中的行数就是第一行
                cursorLine = document.FirstLine;
            }
            else if (mouseY > documentArea.Height)
            {
                // 光标在画布的下面，那么命中的行数是最后一行
                cursorLine = document.LastLine;
            }
            else
            {
                // 光标在画布中，那么做命中测试
                // 找到鼠标所在行
                cursorLine = HitTestHelper.HitTestVTextLine(document.FirstLine, mouseY);
                if (cursorLine == null)
                {
                    // 这里说明鼠标没有在任何一行上
                    logger.DebugFormat("没有找到鼠标位置对应的行, cursorY = {0}", mouseY);
                    return false;
                }
            }

            #endregion

            #region 再计算鼠标悬浮于哪个字符上

            int characterIndex = 0;

            if (mouseX < 0)
            {
                // 鼠标在画布左边，那么悬浮的就是第一个字符
                characterIndex = 0;
            }
            if (mouseX > documentArea.Width)
            {
                // 鼠标在画布右边，那么悬浮的就是最后一个字符
                characterIndex = cursorLine.Characters.Count;
            }
            else
            {
                // 鼠标的水平方向在画布中间，那么做字符命中测试
                VTRect characterBounds;
                if (!HitTestHelper.HitTestVTCharacter(cursorLine, mouseX, out characterIndex, out characterBounds))
                {
                    // 没有命中字符
                    return false;
                }
            }

            #endregion

            // 命中成功再更新TextPointer，保证pointer不为空
            pointer.PhysicsRow = cursorLine.PhysicsRow;
            pointer.CharacterIndex = characterIndex;

            return true;
        }

        /// <summary>
        /// 重置终端大小
        /// 模仿Xshell的做法：
        /// 1. 扩大行的时候，如果有滚动内容，那么显示滚动内容。如果没有滚动内容，则直接在后面扩大。
        /// 2. 减少行的时候，如果有滚动内容，那么减少滚动内容。
        /// 3. ActiveLine保持不变
        /// 调用这个函数的时候保证此时文档已经滚动到底
        /// 是否需要考虑scrollMargin?目前没考虑
        /// </summary>
        /// <param name="rowSize">终端的新的行数</param>
        /// <param name="colSize">终端的新的列数</param>
        private void Resize(int rowSize, int colSize, int scrollMin, int scrollMax)
        {
            if (this.viewportRow != rowSize)
            {
                int rows = Math.Abs(this.viewportRow - rowSize);

                // 扩大或者缩小行数之后，第一行应该显示的物理行数
                int firstRow = 0;
                int activeRow = this.ActiveLine.PhysicsRow;

                if (this.viewportRow < rowSize)
                {
                    // 扩大了行数
                    firstRow = Math.Max(scrollMin, this.FirstLine.PhysicsRow - rows);

                    // 找到滚动区域内的最后一行，在该行后添加新行
                    for (int i = 0; i < rows; i++)
                    {
                        VTextLine textLine = this.CreateTextLine(this.LastLine.PhysicsRow + 1);
                        this.LastLine.Append(textLine);
                    }
                }
                else
                {
                    // 减少了行数

                    // 此时滚动条必定是在最低面
                    if (scrollMax > 0)
                    {
                        firstRow = scrollMax + rows;
                    }
                    else
                    {
                        int count = this.LastLine.PhysicsRow - this.ActiveLine.PhysicsRow;
                        if (rows <= count)
                        {
                            firstRow = 0;
                        }
                        else
                        {
                            firstRow = (rows - count);
                        }
                    }

                    // 从最后一行开始删除
                    for (int i = 0; i < rows; i++)
                    {
                        this.LastLine.Remove();
                        this.LastLine.Release();
                    }
                }

                // 更新每一行的索引并清空每一行的数据
                // 1. 如果缓冲区是主缓冲区，那么显示历史记录
                // 2. 如果缓冲区是备用缓冲区，那么SSH主机会重新打印所有字符
                VTextLine currentLine = this.FirstLine;
                for (int i = 0; i < rowSize; i++)
                {
                    currentLine.PhysicsRow = firstRow + i;

                    // 重新设置光标所在行
                    if (currentLine.PhysicsRow == activeRow)
                    {
                        this.SetCursor(i, this.Cursor.Column);
                    }

                    currentLine = currentLine.NextLine;
                }

                this.viewportRow = rowSize;
            }

            if (this.viewportColumn != colSize)
            {
                this.viewportColumn = colSize;
            }
        }

        #endregion

        #region 公开接口

        public void Initialize()
        {
            #region 初始化终端大小

            // 先获取文档显示区域的像素大小
            VTSize windowSize = this.options.WindowSize;

            // 真正的内容显示区域大小
            // 该区域不包含边距，滚动条以及书签区域
            VTSize contentSize = windowSize.Offset(-this.options.ContentMargin * 2);
            //VTSize contentSize = this.options.ContentSize;

            // 然后根据显示区域的像素大小和每个字符的宽度和高度动态计算终端的行和列
            this.sizeMode = this.options.SizeMode;
            switch (this.sizeMode)
            {
                case TerminalSizeModeEnum.AutoFit:
                    {
                        VTUtils.CalculateAutoFitSize(contentSize, typeface, out this.viewportRow, out this.viewportColumn);
                        break;
                    }

                case TerminalSizeModeEnum.Fixed:
                    {
                        this.viewportRow = this.options.DefaultViewportRow;
                        this.viewportColumn = this.options.DefaultViewportColumn;
                        break;
                    }

                default:
                    throw new NotImplementedException();
            }

            #endregion

            this.Cursor = new VTCursor(this);
            this.Cursor.OffsetX = 0;
            this.Cursor.OffsetY = 0;
            this.Cursor.Row = 0;
            this.Cursor.Column = 0;
            this.Cursor.Delay = VTUtils.GetCursorInterval(this.options.CursorSpeed);
            this.Cursor.AllowBlink = true;
            this.Cursor.IsVisible = true;
            this.Cursor.Color = this.options.CursorColor;
            this.Cursor.Style = this.options.CursorStyle;
            this.Cursor.Typeface = this.options.Typeface;
            this.Cursor.Initialize();

            this.Selection = new VTextSelection(this);
            this.Selection.Initialize();

            this.Scrollbar = VTScrollInfo.Create(this.IsAlternate, this);
            this.Scrollbar.ViewportRow = this.viewportRow; ;
            this.Scrollbar.ScrollbackMax = this.options.ScrollbackMax;
            this.Scrollbar.Initialize();

            #region 初始化第一行，并设置链表首尾指针

            VTextLine firstLine = this.CreateTextLine(0);
            this.FirstLine = firstLine;
            this.LastLine = firstLine;
            this.ActiveLine = firstLine;

            #endregion

            // 默认创建80行，可见区域也是80行
            for (int i = 1; i < this.viewportRow; i++)
            {
                VTextLine textLine = this.CreateTextLine(i);
                this.LastLine.Append(textLine);
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Release()
        {
            this.Cursor.Release();
            this.Selection.Release();
            this.Scrollbar.Release();

            VTextLine textLine = this.FirstLine;
            while (textLine != null)
            {
                VTextLine current = textLine;
                textLine = textLine.NextLine;
                current.Release();
            }

            this.Scrollbar.Release();

            this.DrawingObject.DeleteDrawingObjects();
            this.FirstLine = null;
            this.LastLine = null;
            this.ActiveLine = null;
        }

        /// <summary>
        /// 换行
        /// 换行逻辑是把第一行拿到最后一行，要考虑到scrollMargin
        /// </summary>
        public void LineFeed()
        {
            // 可滚动区域的第一行和最后一行
            VTextLine head = this.FirstLine.FindNext(this.ScrollMarginTop);
            VTextLine last = this.LastLine.FindPrevious(this.ScrollMarginBottom);
            VTextLine oldFirstLine = this.FirstLine;
            VTextLine oldLastLine = this.LastLine;

            // 光标所在行是可滚动区域的最后一行
            // 也表示即将滚动
            if (last == this.ActiveLine)
            {
                // 光标在滚动区域的最后一行，那么把滚动区域的第一行拿到滚动区域最后一行的下面
                logger.DebugFormat("LineFeed，光标在可滚动区域最后一行，向下滚动一行");

                // 把第一行拿到最后一行后面
                VTextLine node1 = head;
                VTextLine node1Prev = node1.PreviousLine;
                VTextLine node1Next = node1.NextLine;

                VTextLine node2 = last;
                VTextLine node2Prev = node2.PreviousLine;
                VTextLine node2Next = node2.NextLine;

                // node1Prev不为空说明有MarginTop
                // node1Prev为空说明没有MarginTop
                if (node1Prev != null)
                {
                    node1Prev.NextLine = node1Next;
                }
                node1Next.PreviousLine = node1Prev;

                node1.PreviousLine = node2;
                node1.NextLine = node2Next;
                node2.NextLine = node1;
                if (node2Next != null)
                {
                    node2Next.PreviousLine = node1;
                }

                if (this.ScrollMarginTop == 0)
                {
                    this.FirstLine = node1Next;
                }

                if (this.ScrollMarginBottom == 0)
                {
                    this.LastLine = node1;
                }

                this.ActiveLine = this.FirstLine.FindNext(this.Cursor.Row);

                // 下移之后，删除整行数据，终端会重新打印该行数据的
                // 如果不删除的话，会和ReverseLineFeed一样有可能会显示重叠的信息
                //this.ActiveLine.EraseAll();
                this.ActiveLine.Recycle();

                // 物理行号和scrollMargin无关
                if (!this.IsAlternate)
                {
                    // this.FirstLine.PhysicsRow > 0表示滚动了一行

                    if (this.ScrollMarginTop != 0 || this.ScrollMarginBottom != 0)
                    {
                        // 有margin需要全部重新设置
                        this.ResetPhysicsRow(oldFirstLine.PhysicsRow + 1);
                    }
                    else
                    {
                        this.LastLine.PhysicsRow = oldLastLine.PhysicsRow + 1;
                    }
                }

                this.cursorPhysicsRow = this.ActiveLine.PhysicsRow;
            }
            else
            {
                // 光标不在可滚动区域的最后一行，说明可以直接移动光标
                logger.DebugFormat("LineFeed，光标在滚动区域内，直接移动光标到下一行");
                this.SetCursor(this.Cursor.Row + 1, this.Cursor.Column);
                this.ActiveLine.SetRenderDirty(true);
            }
        }

        /// <summary>
        /// 反向换行
        /// 反向换行不增加新行，也不减少新行，保持总行数不变
        /// </summary>
        public void ReverseLineFeed()
        {
            // 可滚动区域的第一行和最后一行
            VTextLine head = this.FirstLine.FindNext(this.ScrollMarginTop);
            VTextLine last = this.LastLine.FindPrevious(this.ScrollMarginBottom);
            VTextLine oldFirstLine = this.FirstLine;
            VTextLine oldLastLine = this.LastLine;

            if (head == this.ActiveLine)
            {
                // 此时光标位置在可视区域的第一行
                logger.DebugFormat("RI_ReverseLineFeed，光标在可视区域第一行，向上移动一行并且可视区域往上移动一行");

                // 把最后一行拿到第一行前面
                VTextLine node1 = head;
                VTextLine node1Prev = node1.PreviousLine;
                VTextLine node1Next = node1.NextLine;

                VTextLine node2 = last;
                VTextLine node2Prev = node2.PreviousLine;
                VTextLine node2Next = node2.NextLine;

                // node2Next不为空说明有MarginBottom
                // node2Next不为空说明没有MarginBottom
                if (node2Next != null)
                {
                    node2Next.PreviousLine = node2Prev;
                }
                node2Prev.NextLine = node2Next;

                node1.PreviousLine = node2;
                node2.NextLine = node1;
                node2.PreviousLine = node1Prev;
                if (node1Prev != null)
                {
                    node1Prev.NextLine = node2;
                }

                if (this.ScrollMarginTop == 0)
                {
                    this.FirstLine = node2;
                }

                if (this.ScrollMarginBottom == 0)
                {
                    this.LastLine = node2Prev;
                }

                this.ActiveLine = this.FirstLine.FindNext(this.Cursor.Row);

                // 上移之后，删除整行数据，终端会重新打印该行数据的
                // 如果不删除的话，在man程序下有可能会显示重叠的信息
                // 复现步骤：man cc -> enter10次 -> help -> enter10次 -> q -> 一直按上键
                this.ActiveLine.EraseAll();

                // 物理行号和scrollMargin无关
                if (!this.IsAlternate)
                {
                    this.ResetPhysicsRow(oldFirstLine.PhysicsRow);
                }

                this.cursorPhysicsRow = this.ActiveLine.PhysicsRow;
            }
            else
            {
                // 这里假设光标在可视区域里面
                // 实际上有可能光标在可视区域上的上面或者下面，但是目前还没找到判断方式

                // 光标位置在可视区域里面
                logger.DebugFormat("RI_ReverseLineFeed，光标在可视区域里，直接移动光标到上一行");
                this.SetCursor(this.Cursor.Row - 1, this.Cursor.Column);
                this.ActiveLine.SetRenderDirty(true);
            }
        }

        /// <summary>
        /// 在指定的光标位置打印一个字符
        /// TODO：该函数里的VTCharacter没有复用，需要改进
        /// </summary>
        /// <param name="ch"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        public void PrintCharacter(VTextLine textLine, VTCharacter character, int column)
        {
            textLine.PrintCharacter(character, column);
        }

        /// <summary>
        /// 在当前光标所在行开始删除字符操作
        /// </summary>
        /// <param name="textLine">要执行删除操作的行</param>
        /// <param name="column">光标所在列</param>
        /// <param name="eraseType">删除类型</param>
        public void EraseLine(VTextLine textLine, int column, EraseType eraseType)
        {
            switch (eraseType)
            {
                case EraseType.ToEnd:
                    {
                        // 删除从当前光标处到该行结尾的所有字符
                        textLine.EraseToEnd(column);
                        break;
                    }

                case EraseType.FromBeginning:
                    {
                        // 删除从行首到当前光标处的内容
                        textLine.EraseFromBeginning(column);
                        break;
                    }

                case EraseType.All:
                    {
                        // 删除光标所在整行
                        textLine.EraseAll();
                        break;
                    }

                default:
                    throw new NotImplementedException();
            }

        }

        /// <summary>
        /// 执行EraseDisplay指令
        /// </summary>
        /// <param name="textLine">当前光标所在行</param>
        /// <param name="column">当前的光标位置</param>
        /// <param name="eraseType"></param>
        public void EraseDisplay(VTextLine textLine, int column, EraseType eraseType)
        {
            switch (eraseType)
            {
                case EraseType.ToEnd:
                    {
                        // 从当前光标处直到屏幕最后一行全部都删除（包括当前光标处）
                        if (textLine == null)
                        {
                            logger.ErrorFormat("EraseType.ToEnd失败, 光标所在行不存在");
                            return;
                        }

                        // 先删第一行
                        textLine.EraseToEnd(column);

                        // 再删剩下的行
                        VTextLine next = textLine.NextLine;
                        while (next != null)
                        {
                            next.EraseToEnd(0);

                            next = next.NextLine;
                        }

                        break;
                    }

                case EraseType.FromBeginning:
                    {
                        // 从屏幕的开始处删除到当前光标处
                        if (textLine == null)
                        {
                            logger.ErrorFormat("EraseType.FromBeginning失败, 光标所在行不存在");
                            return;
                        }

                        textLine.EraseToEnd(0);

                        VTextLine next = this.FirstLine;
                        while (next != null && next != textLine)
                        {
                            next.EraseAll();

                            next = next.NextLine;
                        }

                        break;
                    }

                default:
                    throw new NotImplementedException();
            }
        }

        public void EraseAll()
        {
            #region 删除所有文本数据

            VTextLine current = this.FirstLine;
            VTextLine last = this.LastLine;

            while (current != null)
            {
                current.EraseAll();

                current = current.NextLine;
            }

            #endregion
        }

        /// <summary>
        /// 从指定行的指定列处开始删除字符，要删除的字符数由count指定
        /// </summary>
        /// <param name="textLine">要删除的字符所在行</param>
        /// <param name="column">要删除的字符起始列</param>
        /// <param name="count">要删除的字符个数</param>
        public void DeleteCharacter(VTextLine textLine, int column, int count)
        {
            textLine.DeleteCharacter(column, count);
        }

        /// <summary>
        /// 把当前可视区域的所有TextLine标记为需要重新渲染的状态
        /// </summary>
        public void DirtyAll()
        {
            VTextLine current = this.FirstLine;
            VTextLine last = this.LastLine;

            while (current != null)
            {
                current.SetRenderDirty(true);

                current = current.NextLine;
            }
        }

        /// <summary>
        /// 在可视区域的指定的行位置插入多个新行，并把指定的行和指定行后面的所有行往后移动
        /// 要考虑到TopMargin和BottomMargin
        /// </summary>
        /// <param name="activeLine">光标所在行</param>
        /// <param name="lines">要插入的行数</param>
        public void InsertLines(VTextLine activeLine, int lines)
        {
            VTextLine head = this.FirstLine.FindNext(this.ScrollMarginTop);
            VTextLine last = this.LastLine.FindPrevious(this.ScrollMarginBottom);

            VTextLine node1 = activeLine;
            VTextLine node1Prev = node1.PreviousLine;
            VTextLine node1Next = node1.NextLine;

            VTextLine node2 = last;
            VTextLine node2Prev = node2.PreviousLine;
            VTextLine node2Next = node2.NextLine;

            VTextLine newHead = null;
            VTextLine newLast = null;

            for (int i = 0; i < lines; i++)
            {
                // 新行就是node2
                // 每次都要把新行挂到node1上面

                // 更新上半部分
                node1.PreviousLine = node2;
                if (node1Prev != null)
                {
                    node1Prev.NextLine = node2;
                }
                node2.NextLine = node1;
                node2.PreviousLine = node1Prev;
                node1Prev = node1.PreviousLine;
                node1Next = node1.NextLine;
                node2.EraseAll();  // 把新行清空

                // 更新下半部分
                node2Prev.NextLine = node2Next;
                if (node2Next != null)
                {
                    node2Next.PreviousLine = node2Prev;
                }
                node2 = node2Prev;
                node2Prev = node2.PreviousLine;
                node2Next = node2.NextLine;

                // 更新FirstLine
                if (this.ScrollMarginTop == 0)
                {
                    if (newHead == null)
                    {
                        newHead = node1.PreviousLine;
                        this.FirstLine = newHead;
                    }
                }

                // 更新LastLine
                if (this.ScrollMarginBottom == 0)
                {
                    this.LastLine = node2Prev;
                }
            }

            // 更新ActiveLine
            this.ActiveLine = this.FirstLine.FindNext(this.Cursor.Row);
        }

        /// <summary>
        /// 从activeLine开始删除lines行，并把后面的行往前移动
        /// </summary>
        /// <param name="activeLine"></param>
        /// <param name="lines"></param>
        public void DeleteLines(VTextLine activeLine, int lines)
        {
            VTextLine current = activeLine;

            VTextLine head = this.FirstLine.FindNext(this.ScrollMarginTop);
            VTextLine last = this.LastLine.FindPrevious(this.ScrollMarginBottom);

            VTextLine node1 = activeLine;
            VTextLine node1Prev = node1.PreviousLine;
            VTextLine node1Next = node1.NextLine;

            VTextLine node2 = last;
            VTextLine node2Prev = node2.PreviousLine;
            VTextLine node2Next = node2.NextLine;

            for (int i = 0; i < lines; i++)
            {
                // node1就是要删除的节点
                // 把node1插到node2下面

                // 更新上半部分
                node1Next.PreviousLine = node1Prev;
                if (node1Prev != null)
                {
                    node1Prev.NextLine = node1Next;
                }

                // 更新下半部分
                node2.NextLine = node1;
                node1.PreviousLine = node2;
                node1.NextLine = node2Next;
                if (node2Next != null)
                {
                    node2Next.PreviousLine = node1;
                }
                node1.EraseAll();

                // 更新FirstLine
                if (this.ScrollMarginTop == 0)
                {
                    this.FirstLine = node1Next;
                }

                // 更新LastLine
                if (this.ScrollMarginBottom == 0)
                {
                    this.LastLine = node1;
                }

                node2 = node1;
                node1Prev = node2.PreviousLine;
                node2Next = node2.NextLine;

                node1 = node1Next;
                node1Prev = node1.PreviousLine;
                node1Next = node1.NextLine;
            }

            // 更新ActiveLine
            this.ActiveLine = this.FirstLine.FindNext(this.Cursor.Row);
        }

        /// <summary>
        /// 在指定行的指定位置处插入N个空白字符,这会将所有现有文本移到右侧。 向右溢出屏幕的文本会被删除
        /// </summary>
        /// <param name="textLine">要插入空白字符的行</param>
        /// <param name="column">要插入的字符的列</param>
        /// <param name="characters">要插入的空白字符的个数</param>
        public void InsertCharacters(VTextLine textLine, int column, int characters)
        {
            for (int i = 0; i < characters; i++)
            {
                // 先找到当前光标处的字符索引
                int characterIndex = textLine.FindCharacterIndex(column);

                // 创建新的字符
                VTCharacter character = VTCharacter.CreateNull();

                // 插入到光标处
                this.ActiveLine.InsertCharacter(characterIndex, character);

                // 如果溢出了列宽，那么删除溢出的字符
                this.ActiveLine.TrimEnd(this.viewportColumn);
            }
        }

        /// <summary>
        /// 设置可滚动区域的大小
        /// </summary>
        /// <param name="marginTop">可滚动区域的上边距</param>
        /// <param name="marginBottom">可滚动区域的下边距</param>
        public void SetScrollMargin(int marginTop, int marginBottom)
        {
            if (this.ScrollMarginTop != marginTop)
            {
                this.ScrollMarginTop = marginTop;
            }

            if (this.ScrollMarginBottom != marginBottom)
            {
                this.ScrollMarginBottom = marginBottom;
            }
        }

        /// <summary>
        /// 设置光标位置和activeLine
        /// row和column从0开始计数，0表示第一行或者第一列
        /// /// </summary>
        /// <param name="row">要设置的行</param>
        /// <param name="column">要设置的列</param>
        public void SetCursor(int row, int column)
        {
            if (this.Cursor.Row != row)
            {
                this.Cursor.Row = row;

                this.ActiveLine = this.FirstLine.FindNext(row);
                this.cursorPhysicsRow = this.ActiveLine.PhysicsRow;
            }

            if (this.Cursor.Column != column)
            {
                this.Cursor.Column = column;

            }

            // 要判断下光标所在行是否为空
            // 如果光标所在行被滚动到可视区域外了，然后执行CR指令，那么ActiveLine就是空的
            if (this.ActiveLine != null)
            {
                this.ActiveLine.PadColumns(column + 1);
                this.Cursor.CharacterIndex = this.ActiveLine.FindCharacterIndex(column) - 1;
            }
        }

        /// <summary>
        /// 设置光标所在物理行号
        /// </summary>
        /// <param name="physicsRow">光标所在物理行号</param>
        public void SetCursorPhysicsRow(int physicsRow)
        {
            if (this.cursorPhysicsRow != physicsRow)
            {
                this.cursorPhysicsRow = physicsRow;
            }

            if (this.ActiveLine == null ||
                this.ActiveLine.PhysicsRow != physicsRow)
            {
                // 如果光标所在物理行被滚动到了文档外，那么ActiveLine就是空的
                this.ActiveLine = this.FindLine(physicsRow);
            }
        }

        /// <summary>
        /// 根据physicsRow找到对应的行
        /// </summary>
        /// <param name="row">指定要找到的行的索引</param>
        /// <returns></returns>
        public VTextLine FindLine(int physicsRow)
        {
            VTextLine current = this.FirstLine;

            while (current != null)
            {
                if (current.PhysicsRow == physicsRow)
                {
                    return current;
                }

                current = current.NextLine;
            }

            return null;
        }

        /// <summary>
        /// 把一行移动到指定位置
        /// 不考虑ScrollMargin
        /// 注意该方法会更新被移动的行的PhysicsRow
        /// </summary>
        /// <param name="textLine"></param>
        /// <param name="options"></param>
        public void MoveLine(VTextLine textLine, MoveOptions options)
        {
            switch (options)
            {
                case MoveOptions.MoveToFirst:
                    {
                        textLine.Remove();
                        VTextLine oldFirstLine = this.FirstLine;
                        this.FirstLine.PreviousLine = textLine;
                        this.FirstLine = textLine;
                        textLine.NextLine = oldFirstLine;
                        textLine.PhysicsRow = oldFirstLine.PhysicsRow - 1;
                        break;
                    }

                case MoveOptions.MoveToLast:
                    {
                        textLine.Remove();
                        VTextLine oldLastLine = this.LastLine;
                        this.LastLine.NextLine = textLine;
                        this.LastLine = textLine;
                        textLine.PreviousLine = oldLastLine;
                        textLine.PhysicsRow = oldLastLine.PhysicsRow + 1;
                        break;
                    }

                default:
                    throw new NotImplementedException();
            }

            this.ActiveLine = this.FirstLine.FindNext(this.Cursor.Row);
        }

        /// <summary>
        /// 重新设置终端的行和列
        /// </summary>
        /// <param name="newSize">文档渲染区域的大小</param>
        public void Resize(VTSize newSize)
        {
            // 如果是固定大小的终端，那么什么都不做
            if (this.sizeMode == TerminalSizeModeEnum.Fixed)
            {
                return;
            }

            int newRows, newCols;
            VTUtils.CalculateAutoFitSize(newSize, this.typeface, out newRows, out newCols);

            // 如果行和列都没变化，那么就什么都不做
            if (this.viewportRow == newRows && this.viewportColumn == newCols)
            {
                return;
            }

            VTDebug.Context.WriteInteractive("ResizeTerminal", "{0},{1},{2},{3}", this.viewportRow, this.viewportColumn, newRows, newCols);

            // 缩放前先滚动到底，不然会有问题
            this.ScrollToBottom();

            // 对Document执行Resize
            // 目前的实现在ubuntu下没问题，但是在Windows10操作系统上运行Windows命令行里的vim程序会有问题，可能是Windows下的vim程序兼容性导致的，暂时先这样
            // 遇到过一种情况：如果终端名称不正确，比如XTerm，那么当行数增加的时候，光标会移动到该行的最右边，终端名称改成xterm就没问题了
            // 目前的实现思路是：如果是减少行，那么从第一行开始删除；如果是增加行，那么从最后一行开始新建行。不考虑ScrollMargin
            int scrollMin = this.Scrollbar.ScrollMin;
            int scrollMax = this.Scrollbar.ScrollMax;
            this.Resize(newRows, newCols, scrollMin, scrollMax);

            // 更新滚动条
            this.Scrollbar.ViewportRow = newRows;

            if (this.IsAlternate)
            {
                #region 处理备用缓冲区

                // 备用缓冲区，因为SSH主机会重新打印所有字符，所以清空所有文本
                this.EraseAll();

                #endregion
            }
            else
            {
                #region 处理主缓冲区

                // 第一行的值就是滚动条的最大值
                int newScrollMax = this.FirstLine.PhysicsRow;
                if (this.Scrollbar.ScrollMax != newScrollMax)
                {
                    this.Scrollbar.ScrollMax = newScrollMax;
                    this.Scrollbar.ScrollValue = newScrollMax;

                    // 从第一行开始重新渲染显示终端内容
                    VTextLine currentLine = this.FirstLine;
                    while (currentLine != null)
                    {
                        VTHistoryLine historyLine;
                        if (this.Scrollbar.TryGetHistory(currentLine.PhysicsRow, out historyLine))
                        {
                            currentLine.SetHistory(historyLine);
                        }
                        else
                        {
                            // 没有找到要显示的滚动区域外的内容，说明已经全部显示了
                            // 但是还是要继续循环下去，因为相当于是把底部的文本行拿到了最上面，此时底部的文本行需要清空
                            currentLine.EraseAll();
                        }

                        currentLine = currentLine.NextLine;
                    }
                }

                #endregion
            }

            // 如果是修改行大小，那么会自动触发重绘
            // 如果是修改列，那么不会自动触发重绘，要手动重绘
            // 这里偷个懒，不管修改的是列还是行都重绘一次
            this.RequestInvalidate();

            // 更新界面上的行和列
            this.viewportColumn = newCols;
            this.viewportRow = newRows;
        }

        /// <summary>
        /// 设置当前要应用的文本装饰
        /// </summary>
        /// <param name="attribute">要应用的装饰类型</param>
        /// <param name="unset">是否应用该装饰</param>
        /// <param name="parameter">该状态对应的参数</param>
        public void SetAttribute(VTextAttributes attribute, bool enabled, object parameter)
        {
            VTUtils.SetTextAttribute(attribute, enabled, ref this.AttributeState.Value);

            switch (attribute)
            {
                case VTextAttributes.Background:
                    {
                        this.AttributeState.Background = parameter as VTColor;
                        break;
                    }

                case VTextAttributes.Foreground:
                    {
                        this.AttributeState.Foreground = parameter as VTColor;
                        break;
                    }

                default:
                    break;
            }
        }

        /// <summary>
        /// 清空当前应用的文本装饰
        /// </summary>
        public void ClearAttribute()
        {
            this.AttributeState.Reset();
        }

        public void CursorSave()
        {
            this.cursorState.Row = this.Cursor.Row;
            this.cursorState.Column = this.Cursor.Column;
        }

        public void CursorRestore()
        {
            this.SetCursor(this.cursorState.Row, this.cursorState.Column);
        }

        /// <summary>
        /// 滚动到指定的行
        /// </summary>
        /// <param name="physicsRow">要滚动到的物理行数</param>
        /// <param name="options">滚动选项</param>
        public void ScrollTo(int physicsRow, ScrollOptions options = ScrollOptions.ScrollToTop)
        {
            int scrollTo = -1;

            switch (options)
            {
                case ScrollOptions.ScrollToTop:
                    {
                        scrollTo = physicsRow;
                        break;
                    }

                case ScrollOptions.ScrollToMiddle:
                    {
                        scrollTo = physicsRow - this.viewportRow / 2;
                        break;
                    }

                case ScrollOptions.ScrollToBottom:
                    {
                        scrollTo = physicsRow - this.viewportRow + 1;
                        break;
                    }

                default:
                    throw new NotImplementedException();
            }

            // 判断要滚动到的行是否超出了滚动最大值或者最小值
            if (scrollTo < this.Scrollbar.ScrollMin)
            {
                scrollTo = this.Scrollbar.ScrollMin;
            }
            else if (scrollTo > this.Scrollbar.ScrollMax)
            {
                scrollTo = this.Scrollbar.ScrollMax;
            }

            this.ScrollToHistory(scrollTo);
        }

        /// <summary>
        /// 把主缓冲区滚动到底
        /// </summary>
        public void ScrollToBottom()
        {
            if (this.IsAlternate)
            {
                return;
            }

            if (!this.ScrollAtBottom)
            {
                this.ScrollToHistory(this.Scrollbar.ScrollMax);
            }
        }

        /// <summary>
        /// 获取当前使用鼠标选中的段落区域
        /// </summary>
        /// <returns></returns>
        public VTParagraph GetSelectedParagraph()
        {
            VTextSelection selection = this.Selection;
            if (selection.IsEmpty)
            {
                return VTParagraph.Empty;
            }

            return this.CreateParagraph(ParagraphTypeEnum.Selected, LogFileTypeEnum.PlainText);
        }

        /// <summary>
        /// 创建指定的段落内容
        /// </summary>
        /// <param name="paragraphType">段落类型</param>
        /// <param name="fileType">要创建的内容格式</param>
        /// <returns></returns>
        public VTParagraph CreateParagraph(ParagraphTypeEnum paragraphType, LogFileTypeEnum fileType)
        {
            List<List<VTCharacter>> characters = new List<List<VTCharacter>>();
            int startCharacterIndex = 0, endCharacterIndex = 0;
            int firstPhysicsRow = 0, lastPhysicsRow = 0;

            switch (paragraphType)
            {
                case ParagraphTypeEnum.AllDocument:
                    {
                        if (this.IsAlternate)
                        {
                            // 备用缓冲区直接保存VTextLine
                            VTextLine current = this.FirstLine;
                            while (current != null)
                            {
                                characters.Add(current.Characters);
                                current = current.NextLine;
                            }

                            startCharacterIndex = 0;
                            endCharacterIndex = Math.Max(0, this.LastLine.Characters.Count - 1);
                            firstPhysicsRow = this.FirstLine.PhysicsRow;
                            lastPhysicsRow = this.LastLine.PhysicsRow;
                        }
                        else
                        {
                            List<VTHistoryLine> historyLines;
                            if (!this.Scrollbar.TryGetHistories(this.Scrollbar.FirstLine.PhysicsRow, this.Scrollbar.LastLine.PhysicsRow, out historyLines))
                            {
                                logger.ErrorFormat("SaveAll失败, 有的历史记录为空");
                                return VTParagraph.Empty;
                            }

                            characters.AddRange(historyLines.Select(v => v.Characters));
                            startCharacterIndex = 0;
                            endCharacterIndex = this.Scrollbar.LastLine.Characters.Count - 1;
                            firstPhysicsRow = this.Scrollbar.FirstLine.PhysicsRow;
                            lastPhysicsRow = this.Scrollbar.LastLine.PhysicsRow;
                        }
                        break;
                    }

                case ParagraphTypeEnum.Viewport:
                    {
                        VTextLine current = this.FirstLine;
                        while (current != null)
                        {
                            characters.Add(current.Characters);
                            current = current.NextLine;
                        }

                        startCharacterIndex = 0;
                        endCharacterIndex = Math.Max(0, this.LastLine.Characters.Count - 1);
                        firstPhysicsRow = this.FirstLine.PhysicsRow;
                        lastPhysicsRow = this.LastLine.PhysicsRow;
                        break;
                    }

                case ParagraphTypeEnum.Selected:
                    {
                        if (this.Selection.IsEmpty)
                        {
                            return VTParagraph.Empty;
                        }

                        int topRow, bottomRow;
                        this.Selection.Normalize(out topRow, out bottomRow, out startCharacterIndex, out endCharacterIndex);
                        firstPhysicsRow = topRow;
                        lastPhysicsRow = bottomRow;

                        if (this.IsAlternate)
                        {
                            // 备用缓冲区没有滚动内容，只能选中当前显示出来的文档
                            VTextLine firstLine = this.FindLine(topRow);
                            while (firstLine != null)
                            {
                                characters.Add(firstLine.Characters);

                                if (firstLine.PhysicsRow == bottomRow)
                                {
                                    break;
                                }

                                firstLine = firstLine.NextLine;
                            }
                        }
                        else
                        {
                            List<VTHistoryLine> historyLines;
                            if (!this.Scrollbar.TryGetHistories(topRow, bottomRow - topRow + 1, out historyLines))
                            {
                                logger.ErrorFormat("SaveSelected失败, 有的历史记录为空");
                                return VTParagraph.Empty;
                            }
                            characters.AddRange(historyLines.Select(v => v.Characters));
                        }
                        break;
                    }

                default:
                    throw new NotImplementedException();
            }

            CreateContentParameter parameter = new CreateContentParameter()
            {
                SessionName = this.options.Session.Name,
                CharactersList = characters,
                StartCharacterIndex = startCharacterIndex,
                EndCharacterIndex = endCharacterIndex,
                ContentType = fileType,
                Typeface = this.options.Typeface
            };

            string text = VTUtils.CreateContent(parameter);

            return new VTParagraph()
            {
                Content = text,
                CreationTime = DateTime.Now,
                StartCharacterIndex = startCharacterIndex,
                EndCharacterIndex = endCharacterIndex,
                FirstPhysicsRow = firstPhysicsRow,
                LastPhysicsRow = lastPhysicsRow,
                CharacterList = characters,
                IsAlternate = this.IsAlternate
            };
        }

        /// <summary>
        /// 选中全部的文本
        /// </summary>
        public void SelectAll()
        {
            int firstRow = 0, lastRow = 0, lastCharacterIndex = 0;
            if (this.IsAlternate)
            {
                firstRow = this.FirstLine.PhysicsRow;
                lastRow = this.LastLine.PhysicsRow;
                lastCharacterIndex = this.LastLine.Characters.Count - 1;
            }
            else
            {
                VTHistoryLine startHistoryLine = this.Scrollbar.FirstLine;
                VTHistoryLine lastHistoryLine = this.Scrollbar.LastLine;
                firstRow = startHistoryLine.PhysicsRow;
                lastRow = lastHistoryLine.PhysicsRow;
                lastCharacterIndex = lastHistoryLine.Characters.Count - 1;
            }

            this.Selection.FirstRow = firstRow;
            this.Selection.LastRow = lastRow;
            this.Selection.FirstRowCharacterIndex = 0;
            this.Selection.LastRowCharacterIndex = lastCharacterIndex;
            this.Selection.RequestInvalidate();
        }

        /// <summary>
        /// update anything about ui
        /// 如果需要布局则进行布局
        /// 如果不需要布局，那么就看是否需要重绘某些文本行
        /// </summary>
        public void RequestInvalidate()
        {
            #region 渲染文档

            // 当前行的Y方向偏移量
            double offsetY = 0;

            VTextLine next = this.FirstLine;

            while (next != null)
            {
                // 更新Y偏移量信息
                next.OffsetY = offsetY;

                next.RequestInvalidate();

                // 更新下一个文本行的Y偏移量
                offsetY += next.Height;

                next = next.NextLine;
            }

            #endregion

            #region 重绘光标

            // 光标闪烁在单独线程处理，这里只改变光标位置
            this.Cursor.RequestInvalidatePosition();

            #endregion

            #region 移动滚动条

            this.Scrollbar.RequestInvalidate();

            #endregion

            #region 更新选中区域

            this.Selection.RequestInvalidate();

            #endregion
        }

        #endregion

        #region 事件处理器

        public void OnMouseDown(VTPoint mouseLocation, int clickCount)
        {
            if (clickCount == 1)
            {
                this.isMouseDown = true;
                this.mouseDownPos = mouseLocation;

                // 点击的时候先清除选中区域
                this.Selection.Reset();
                this.Selection.RequestInvalidate();
            }
            else
            {
                // 双击就是选中单词
                // 三击就是选中整行内容

                int startIndex = 0, endIndex = 0;

                VTextLine lineHit = HitTestHelper.HitTestVTextLine(this.FirstLine, mouseLocation.Y);
                if (lineHit == null)
                {
                    return;
                }

                switch (clickCount)
                {
                    case 2:
                        {
                            // 选中单词
                            string text = VTUtils.CreatePlainText(lineHit.Characters);
                            int characterIndex;
                            VTRect characterBounds;
                            if (!HitTestHelper.HitTestVTCharacter(lineHit, mouseLocation.X, out characterIndex, out characterBounds))
                            {
                                return;
                            }
                            VTUtils.GetSegement(text, characterIndex, out startIndex, out endIndex);
                            break;
                        }

                    case 3:
                        {
                            // 选中一整行
                            string text = VTUtils.CreatePlainText(lineHit.Characters);
                            startIndex = 0;
                            endIndex = text.Length - 1;
                            break;
                        }

                    default:
                        {
                            return;
                        }
                }

                this.Selection.FirstRow = lineHit.PhysicsRow;
                this.Selection.FirstRowCharacterIndex = startIndex;
                this.Selection.LastRow = lineHit.PhysicsRow;
                this.Selection.LastRowCharacterIndex = endIndex;
                this.Selection.RequestInvalidate();
            }
        }

        public void OnMouseMove(VTPoint mouseLocation)
        {
            if (!this.isMouseDown)
            {
                return;
            }

            if (!this.selectionState)
            {
                // 此时说明开始选中操作
                this.selectionState = true;
                this.Selection.Reset();
                this.startPointer.CharacterIndex = -1;
                this.startPointer.PhysicsRow = -1;
                this.endPointer.CharacterIndex = -1;
                this.endPointer.PhysicsRow = -1;
            }

            // 整体思路是算出来StartTextPointer和EndTextPointer之间的几何图形
            // 然后渲染几何图形，SelectionRange本质上就是一堆矩形

            VTRect vtRect = this.DrawingObject.GetContentRect();

            // 如果还没有测量起始字符，那么测量起始字符
            if (startPointer.CharacterIndex == -1)
            {
                if (!this.GetTextPointer(mouseLocation, vtRect, startPointer))
                {
                    // 没有命中起始字符，那么直接返回啥都不做
                    //logger.DebugFormat("没命中起始字符");
                    return;
                }
            }

            // 首先检测鼠标是否在Surface边界框的外面
            // 如果在Surface的外面并且行数超出了Surface可以显示的最多行数，那么根据鼠标方向进行滚动，每次滚动一行
            this.ScrollIfCursorOutsideDocument(mouseLocation, vtRect);

            // 更新当前鼠标的命中信息，保存在endPointer里
            if (!this.GetTextPointer(mouseLocation, vtRect, endPointer))
            {
                // 命中失败，不更新
                return;
            }

            #region 起始字符和结束字符测量出来的索引位置都是-1，啥都不做

            if (startPointer.CharacterIndex < 0 || endPointer.CharacterIndex < 0)
            {
                logger.WarnFormat("鼠标命中的起始字符和结束字符位置都小于0");
                return;
            }

            #endregion

            #region 起始字符和结束字符是同一个字符，啥都不做

            if (startPointer.CharacterIndex == endPointer.CharacterIndex)
            {
                //logger.WarnFormat("鼠标命中的起始字符和结束字符是相同字符");
                return;
            }

            #endregion

            // 重新渲染
            // PerformDrawing会更新TextSelection的形状

            this.Selection.FirstRow = startPointer.PhysicsRow;
            this.Selection.FirstRowCharacterIndex = startPointer.CharacterIndex;
            this.Selection.LastRow = endPointer.PhysicsRow;
            this.Selection.LastRowCharacterIndex = endPointer.CharacterIndex;

            // 此处要全部刷新，因为有可能会触发ScrollIfCursorOutsideDocument
            // ScrollIfCursorOutsideDocument的情况下，是滚动后的数据，所以需要刷新
            this.RequestInvalidate();
        }

        public void OnMouseUp(VTPoint mouseLocation)
        {
            this.isMouseDown = false;
            this.selectionState = false;
        }

        public void OnMouseWheel(bool upper)
        {
            // 只有主缓冲区才可以用鼠标滚轮进行滚动
            // 备用缓冲区不可以滚动
            if (this.IsAlternate)
            {
                return;
            }

            int scrollValue = this.Scrollbar.ScrollValue;
            int scrollMax = this.Scrollbar.ScrollMax;

            if (upper)
            {
                // 向上滚动

                // 先判断是不是已经滚动到顶了
                if (this.ScrollAtTop)
                {
                    // 滚动到顶直接返回
                    return;
                }

                if (scrollValue < this.scrollDelta)
                {
                    // 一次可以全部滚完并且还有剩余
                    this.ScrollToHistory(this.Scrollbar.ScrollMin);
                }
                else
                {
                    this.ScrollToHistory(scrollValue - this.scrollDelta);
                }
            }
            else
            {
                // 向下滚动

                if (this.ScrollAtBottom)
                {
                    // 滚动到底直接返回
                    return;
                }

                // 剩余可以往下滚动的行数
                int remainScroll = scrollMax - scrollValue;

                if (remainScroll >= this.scrollDelta)
                {
                    this.ScrollToHistory(scrollValue + this.scrollDelta);
                }
                else
                {
                    // 直接滚动到底
                    this.ScrollToHistory(scrollMax);
                }
            }

            // 重新渲染
            this.RequestInvalidate();
        }

        #endregion
    }
}

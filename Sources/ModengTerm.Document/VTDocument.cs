using ModengTerm.Document.Drawing;
using ModengTerm.Document.Enumerations;
using ModengTerm.Document.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using static ModengTerm.Document.VTextLine;

namespace ModengTerm.Document
{
    /// <summary>
    /// 文档数据模型
    /// 负责维护文档的所有状态，用户输入
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
        /// 该文档是否可见
        /// </summary>
        private bool visible = true;

        private VTextLine activeLine;

        #endregion

        #region 属性

        /// <summary>
        /// 文档的名字，方便调试
        /// </summary>
        public string Name { get; private set; }

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
        public int CursorPhysicsRow { get { return cursorPhysicsRow; } }

        /// <summary>
        /// 当前光标所在行
        /// 通过SetCursor函数设置
        /// </summary>
        public VTextLine ActiveLine 
        {
            get { return this.activeLine; }
            private set
            {
                if (this.activeLine != value)
                {
                    this.activeLine = value;
                }
            }
        }

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
        public bool IsEmpty { get { return FirstLine == null && LastLine == null; } }

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

        public VTCursorState CursorState { get { return cursorState; } }

        /// <summary>
        /// 获取当前滚动条是否滚动到底了
        /// </summary>
        public bool ScrollAtBottom
        {
            get
            {
                return Scrollbar.ScrollAtBottom;
            }
        }

        /// <summary>
        /// 获取当前滚动条是否滚动到顶了
        /// </summary>
        public bool ScrollAtTop
        {
            get
            {
                return Scrollbar.ScrollAtTop;
            }
        }

        public int ViewportRow { get { return viewportRow; } }

        public int ViewportColumn { get { return viewportColumn; } }

        /// <summary>
        /// 获取该文档所使用的字体Typeface
        /// </summary>
        public VTypeface Typeface { get { return options.Typeface; } }

        /// <summary>
        /// 获取该文档的事件输入
        /// </summary>
        public VTEventInput EventInput { get; private set; }

        #endregion

        #region 构造方法

        public VTDocument(VTDocumentOptions options)
        {
            this.options = options;
            Name = options.Name;
            IsAlternate = options.IsAlternate;
            DrawingObject = options.DrawingObject;
            this.EventInput = new VTEventInput()
            {
                OnMouseDown = this.OnMouseDown,
                OnMouseMove = this.OnMouseMove,
                OnMouseUp = this.OnMouseUp,
                OnMouseWheel = this.OnMouseWheel,
                OnScrollChanged = this.OnScrollChanged,
                OnSizeChanged = this.OnSizeChanged,
                OnInput = this.OnInput
            };
            cursorState = new VTCursorState();
            startPointer = new VTextPointer();
            endPointer = new VTextPointer();
            scrollDelta = options.ScrollDelta;
            AttributeState = new VTextAttributeState();
            viewportRow = options.ViewportRow;
            viewportColumn = options.ViewportColumn;
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
                DECPrivateAutoWrapMode = DECPrivateAutoWrapMode,
                PhysicsRow = physicsRow,
                Typeface = options.Typeface
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
            FirstLine.PhysicsRow = newFirstRow;

            VTextLine currentLine = FirstLine.NextLine;
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
            if (IsAlternate)
            {
                return false;
            }

            int scrollValue = physicsRow1;

            // 要滚动的值和当前值是一样的，也不滚动
            if (Scrollbar.ScrollValue == scrollValue)
            {
                return false;
            }

            // 判断要滚动的目标值合法性
            if (scrollValue > Scrollbar.ScrollMax ||
                scrollValue < Scrollbar.ScrollMin)
            {
                return false;
            }

            // 只移动主缓冲区
            VTDocument scrollDocument = this;

            // 要滚动到的值
            int newScroll = scrollValue;
            // 滚动之前的值
            int oldScroll = Scrollbar.ScrollValue;

            // 需要进行滚动的行数
            int scrolledRows = Math.Abs(newScroll - oldScroll);

            // 终端行大小
            int rows = viewportRow;

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
                    if (Scrollbar.TryGetHistory(physicsRow, out historyLine))
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
                        scrollDocument.MoveLine(firstLine, MoveOptions.MoveToLast);

                        VTHistoryLine historyLine;
                        if (Scrollbar.TryGetHistory(lastRow + i, out historyLine))
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
                        if (!Scrollbar.TryGetHistory(firstRow - i, out historyLine))
                        {
                            // 百分之百不可能找不到！！！
                            throw new NotImplementedException();
                        }

                        VTextLine lastLine = scrollDocument.LastLine;
                        lastLine.SetHistory(historyLine);
                        scrollDocument.MoveLine(lastLine, MoveOptions.MoveToFirst);
                    }
                }
            }

            #endregion

            // 更新当前滚动条的值
            Scrollbar.ScrollValue = scrollValue;

            // 有可能光标所在行被滚动到了文档外，此时要更新ActiveLine，ActiveLine就是空的
            scrollDocument.SetCursorPhysicsRow(scrollDocument.CursorPhysicsRow);

            // 每次滚动都需要重新刷新TextSelection区域
            if (!Selection.IsEmpty)
            {
                Selection.MakeInvalidate();
            }

            return true;
        }

        /// <summary>
        /// 当光标在容器外面移动的时候，进行滚动
        /// </summary>
        /// <param name="mousePosition">当前鼠标的坐标</param>
        /// <param name="documentSize">文档的大小</param>
        /// <returns>是否执行了滚动动作</returns>
        private void ScrollIfCursorOutsideDocument(VTPoint mousePosition, VTSize documentSize)
        {
            // 要滚动到的目标行
            int scrollTarget = -1;

            if (mousePosition.Y < 0)
            {
                // 光标在容器上面
                if (!ScrollAtTop)
                {
                    // 不在最上面，往上滚动一行
                    scrollTarget = Scrollbar.ScrollValue - 1;
                }
            }
            else if (mousePosition.Y > documentSize.Height)
            {
                // 光标在容器下面
                if (!ScrollAtBottom)
                {
                    // 往下滚动一行
                    scrollTarget = Scrollbar.ScrollValue + 1;
                }
            }

            if (scrollTarget != -1)
            {
                ScrollToHistory(scrollTarget);
            }
        }

        /// <summary>
        /// 使用像素坐标对VTextLine做命中测试
        /// </summary>
        /// <param name="mousePosition">鼠标坐标</param>
        /// <param name="documentSize">文档大小</param>
        /// <param name="pointer">存储命中测试结果的变量</param>
        /// <remarks>如果传递进来的鼠标位置在窗口外，那么会把鼠标限定在距离鼠标最近的Surface边缘处</remarks>
        /// <returns>
        /// 是否获取成功
        /// 当光标不在某一行或者不在某个字符上的时候，就获取失败
        /// </returns>
        private bool GetTextPointer(VTPoint mousePosition, VTSize documentSize, VTextPointer pointer)
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
            else if (mouseY > documentSize.Height)
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

            if (mouseX > documentSize.Width)
            {
                // 鼠标在画布右边，那么悬浮的就是最后一个字符
                characterIndex = cursorLine.Characters.Count;
            }
            else
            {
                // 鼠标的水平方向在画布中间，那么做字符命中测试
                VTextRange characterRange;
                if (!HitTestHelper.HitTestVTCharacter(cursorLine, mouseX, out characterIndex, out characterRange))
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
            if (viewportRow != rowSize)
            {
                int rows = Math.Abs(viewportRow - rowSize);

                // 扩大或者缩小行数之后，第一行应该显示的物理行数
                int firstRow = 0;
                int activeRow = ActiveLine.PhysicsRow;

                if (viewportRow < rowSize)
                {
                    // 扩大了行数
                    firstRow = Math.Max(scrollMin, FirstLine.PhysicsRow - rows);

                    // 找到滚动区域内的最后一行，在该行后添加新行
                    for (int i = 0; i < rows; i++)
                    {
                        VTextLine textLine = CreateTextLine(LastLine.PhysicsRow + 1);
                        LastLine.Append(textLine);
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
                        int count = LastLine.PhysicsRow - ActiveLine.PhysicsRow;
                        if (rows <= count)
                        {
                            firstRow = 0;
                        }
                        else
                        {
                            firstRow = rows - count;
                        }
                    }

                    // 从最后一行开始删除
                    for (int i = 0; i < rows; i++)
                    {
                        LastLine.Remove();
                        LastLine.Release();
                    }
                }

                // 更新每一行的索引并清空每一行的数据
                // 1. 如果缓冲区是主缓冲区，那么显示历史记录
                // 2. 如果缓冲区是备用缓冲区，那么SSH主机会重新打印所有字符
                VTextLine currentLine = FirstLine;
                for (int i = 0; i < rowSize; i++)
                {
                    currentLine.PhysicsRow = firstRow + i;

                    // 重新设置光标所在行
                    if (currentLine.PhysicsRow == activeRow)
                    {
                        SetCursor(i, Cursor.Column);
                    }

                    currentLine = currentLine.NextLine;
                }

                viewportRow = rowSize;
            }

            if (viewportColumn != colSize)
            {
                viewportColumn = colSize;
            }
        }

        private void HandleTextInput(VTInput input)
        {
            foreach (char ch in input.Text)
            {
                VTCharacter character = VTCharacter.Create(ch, 1);
                this.PrintCharacter(character, this.Cursor.Column);
                this.SetCursor(this.Cursor.Row, this.Cursor.Column + 1);
            }
        }

        private void HandleKeyInput(VTInput input)
        {
            VTKeys key = input.Key;

            if (key >= VTKeys.A && key <= VTKeys.Z)
            {
                input.Type = VTInputTypes.TextInput;
                input.Text = key.ToString();
                this.HandleTextInput(input);
                return;
            }

            switch (key)
            {
                case VTKeys.Enter:
                    {
                        this.LineFeed();
                        this.SetCursor(this.Cursor.Row, 0);
                        break;
                    }

                default:
                    break;
            }
        }

        #endregion

        #region 公开接口

        public void Initialize()
        {
            Cursor = new VTCursor(this);
            Cursor.OffsetX = 0;
            Cursor.OffsetY = 0;
            Cursor.Row = 0;
            Cursor.Column = 0;
            Cursor.Interval = (int)options.CursorSpeed;
            Cursor.AllowBlink = true;
            Cursor.IsVisible = true;
            Cursor.Color = options.CursorColor;
            Cursor.Style = options.CursorStyle;
            Cursor.Typeface = options.Typeface;
            Cursor.Initialize();

            Selection = new VTextSelection(this);
            Selection.Color = options.SelectionColor;
            Selection.Initialize();

            Scrollbar = new VTScrollInfo(this);
            Scrollbar.ViewportRow = viewportRow;
            Scrollbar.ScrollbackMax = options.ScrollbackMax;
            Scrollbar.Initialize();

            #region 初始化第一行，并设置链表首尾指针

            VTextLine firstLine = CreateTextLine(0);
            FirstLine = firstLine;
            LastLine = firstLine;
            ActiveLine = firstLine;

            #endregion

            // 默认创建80行，可见区域也是80行
            for (int i = 1; i < viewportRow; i++)
            {
                VTextLine textLine = CreateTextLine(i);
                LastLine.Append(textLine);
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Release()
        {
            Cursor.Release();
            Selection.Release();
            Scrollbar.Release();

            VTextLine textLine = FirstLine;
            while (textLine != null)
            {
                VTextLine current = textLine;
                textLine = textLine.NextLine;
                current.Release();
            }

            Scrollbar.Release();

            DrawingObject.DeleteDrawingObjects();
            FirstLine = null;
            LastLine = null;
            ActiveLine = null;
        }

        /// <summary>
        /// 换行
        /// 换行逻辑是把第一行拿到最后一行，要考虑到scrollMargin
        /// </summary>
        public void LineFeed()
        {
            // 可滚动区域的第一行和最后一行
            VTextLine head = FirstLine.FindNext(ScrollMarginTop);
            VTextLine last = LastLine.FindPrevious(ScrollMarginBottom);
            VTextLine oldFirstLine = FirstLine;
            VTextLine oldLastLine = LastLine;

            // 光标所在行是可滚动区域的最后一行
            // 也表示即将滚动
            if (last == ActiveLine)
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

                if (ScrollMarginTop == 0)
                {
                    FirstLine = node1Next;
                }

                if (ScrollMarginBottom == 0)
                {
                    LastLine = node1;
                }

                // 更新光标所在行
                ActiveLine = FirstLine.FindNext(Cursor.Row);

                // 下移之后，删除整行数据，终端会重新打印该行数据的
                // 如果不删除的话，会和ReverseLineFeed一样有可能会显示重叠的信息
                ActiveLine.DeleteAll();

                // this.FirstLine.PhysicsRow > 0表示滚动了一行
                if (ScrollMarginTop != 0 || ScrollMarginBottom != 0)
                {
                    // 物理行号和scrollMargin无关
                    // 有margin需要全部重新设置
                    ResetPhysicsRow(oldFirstLine.PhysicsRow + 1);
                }
                else
                {
                    // 没有scrollMargin
                    LastLine.PhysicsRow = oldLastLine.PhysicsRow + 1;
                }

                cursorPhysicsRow = ActiveLine.PhysicsRow;
            }
            else
            {
                // 光标不在可滚动区域的最后一行，说明可以直接移动光标
                logger.DebugFormat("LineFeed，光标在滚动区域内，直接移动光标到下一行");
                SetCursor(Cursor.Row + 1, Cursor.Column);
            }
        }

        /// <summary>
        /// 反向换行
        /// 反向换行不增加新行，也不减少新行，保持总行数不变
        /// </summary>
        public void ReverseLineFeed()
        {
            // 可滚动区域的第一行和最后一行
            VTextLine head = FirstLine.FindNext(ScrollMarginTop);
            VTextLine last = LastLine.FindPrevious(ScrollMarginBottom);
            VTextLine oldFirstLine = FirstLine;
            VTextLine oldLastLine = LastLine;

            if (head == ActiveLine)
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

                if (ScrollMarginTop == 0)
                {
                    FirstLine = node2;
                }

                if (ScrollMarginBottom == 0)
                {
                    LastLine = node2Prev;
                }

                ActiveLine = FirstLine.FindNext(Cursor.Row);

                // 上移之后，删除整行数据，终端会重新打印该行数据的
                // 如果不删除的话，在man程序下有可能会显示重叠的信息
                // 复现步骤：man cc -> enter10次 -> help -> enter10次 -> q -> 一直按上键
                ActiveLine.DeleteAll();

                // 物理行号和scrollMargin无关
                if (!IsAlternate)
                {
                    ResetPhysicsRow(oldFirstLine.PhysicsRow);
                }

                cursorPhysicsRow = ActiveLine.PhysicsRow;
            }
            else
            {
                // 这里假设光标在可视区域里面
                // 实际上有可能光标在可视区域上的上面或者下面，但是目前还没找到判断方式

                // 光标位置在可视区域里面
                logger.DebugFormat("RI_ReverseLineFeed，光标在可视区域里，直接移动光标到上一行");
                SetCursor(Cursor.Row - 1, Cursor.Column);
            }
        }

        /// <summary>
        /// 在指定的光标位置打印一个字符
        /// TODO：该函数里的VTCharacter没有复用，需要改进
        /// </summary>
        /// <param name="ch"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        public void PrintCharacter(VTCharacter character, int column)
        {
            this.ActiveLine.PrintCharacter(character, column);
        }

        /// <summary>
        /// 在当前光标所在行使用填充字符的方式操作行
        /// </summary>
        /// <param name="column">光标所在列</param>
        /// <param name="eraseType">删除类型</param>
        public void EraseLine(int column, EraseType eraseType)
        {
            VTextLine textLine = this.ActiveLine;

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
        public void EraseDisplay(int column, EraseType eraseType)
        {
            VTextLine textLine = this.ActiveLine;

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

                        VTextLine next = FirstLine;
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

            VTextLine current = FirstLine;
            VTextLine last = LastLine;

            while (current != null)
            {
                current.EraseAll();

                current = current.NextLine;
            }

            #endregion
        }

        /// <summary>
        /// 删除光标所在行里的字符
        /// </summary>
        /// <param name="column">要删除的字符起始列</param>
        /// <param name="count">要删除的字符个数</param>
        public void DeleteCharacter(int column, int count)
        {
            VTextLine textLine = this.ActiveLine;

            textLine.DeleteRange(column, count);
        }

        /// <summary>
        /// 从指定列开始删除光标所在行的字符
        /// </summary>
        /// <param name="column">要从第几列开始删除</param>
        public void DeleteCharacter(int column) 
        {
            VTextLine textLine = this.ActiveLine;

            textLine.Delete(column);
        }

        /// <summary>
        /// 从光标所在行开始删除行，并把后面的行往前移动
        /// </summary>
        /// <param name="lines">要删除的行数</param>
        public void DeleteLines(int lines)
        {
            VTextLine activeLine = this.ActiveLine;

            VTextLine current = activeLine;

            VTextLine head = FirstLine.FindNext(ScrollMarginTop);
            VTextLine last = LastLine.FindPrevious(ScrollMarginBottom);

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
                if (ScrollMarginTop == 0)
                {
                    FirstLine = node1Next;
                }

                // 更新LastLine
                if (ScrollMarginBottom == 0)
                {
                    LastLine = node1;
                }

                node2 = node1;
                node1Prev = node2.PreviousLine;
                node2Next = node2.NextLine;

                node1 = node1Next;
                node1Prev = node1.PreviousLine;
                node1Next = node1.NextLine;
            }

            // 更新ActiveLine
            ActiveLine = FirstLine.FindNext(Cursor.Row);
        }

        /// <summary>
        /// 在光标所在行的位置插入多个新行，并把光标所在行和后面的所有行往后移动
        /// 要考虑到TopMargin和BottomMargin
        /// </summary>
        /// <param name="lines">要插入的行数</param>
        public void InsertLines(int lines)
        {
            VTextLine activeLine = this.ActiveLine;

            VTextLine head = FirstLine.FindNext(ScrollMarginTop);
            VTextLine last = LastLine.FindPrevious(ScrollMarginBottom);

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
                if (ScrollMarginTop == 0)
                {
                    if (newHead == null)
                    {
                        newHead = node1.PreviousLine;
                        FirstLine = newHead;
                    }
                }

                // 更新LastLine
                if (ScrollMarginBottom == 0)
                {
                    LastLine = node2Prev;
                }
            }

            // 更新ActiveLine
            ActiveLine = FirstLine.FindNext(Cursor.Row);
        }

        /// <summary>
        /// 在指定行的指定位置处插入N个空白字符,这会将所有现有文本移到右侧。 向右溢出屏幕的文本会被删除
        /// </summary>
        /// <param name="column">要插入的字符的列</param>
        /// <param name="characters">要插入的空白字符的个数</param>
        public void InsertCharacters(int column, int characters)
        {
            VTextLine textLine = this.ActiveLine;

            for (int i = 0; i < characters; i++)
            {
                // 先找到当前光标处的字符索引
                int characterIndex = textLine.FindCharacterIndex(column);

                // 创建新的字符
                VTCharacter character = VTCharacter.CreateNull();

                // 插入到光标处
                ActiveLine.InsertCharacter(characterIndex, character);

                // 如果溢出了列宽，那么删除溢出的字符
                ActiveLine.TrimEnd(viewportColumn);
            }
        }

        /// <summary>
        /// 设置可滚动区域的大小
        /// </summary>
        /// <param name="marginTop">可滚动区域的上边距</param>
        /// <param name="marginBottom">可滚动区域的下边距</param>
        public void SetScrollMargin(int marginTop, int marginBottom)
        {
            if (ScrollMarginTop != marginTop)
            {
                ScrollMarginTop = marginTop;
            }

            if (ScrollMarginBottom != marginBottom)
            {
                ScrollMarginBottom = marginBottom;
            }
        }

        /// <summary>
        /// 设置光标位置和activeLine
        /// row和column从0开始计数，0表示第一行或者第一列
        /// </summary>
        /// <param name="row">要设置的行</param>
        /// <param name="column">要设置的列</param>
        public void SetCursor(int row, int column)
        {
            if (Cursor.Row != row)
            {
                Cursor.Row = row;

                ActiveLine = FirstLine.FindNext(row);
                cursorPhysicsRow = ActiveLine.PhysicsRow;
            }

            if (Cursor.Column != column)
            {
                Cursor.Column = column;
            }

            if (ActiveLine == null)
            {
                Console.WriteLine("SetCursor ActiveLine == NULL");
            }
        }

        /// <summary>
        /// 设置光标所在物理行号
        /// </summary>
        /// <param name="physicsRow">光标所在物理行号</param>
        public void SetCursorPhysicsRow(int physicsRow)
        {
            if (cursorPhysicsRow != physicsRow)
            {
                cursorPhysicsRow = physicsRow;
            }

            if (ActiveLine == null ||
                ActiveLine.PhysicsRow != physicsRow)
            {
                // 如果光标所在物理行被滚动到了文档外，那么ActiveLine就是空的
                // 比如通过滚动滚动条把光标所在行移动到了可视区域外
                ActiveLine = FindLine(physicsRow);
            }
        }

        /// <summary>
        /// 根据physicsRow找到对应的行
        /// </summary>
        /// <param name="row">指定要找到的行的索引</param>
        /// <returns></returns>
        public VTextLine FindLine(int physicsRow)
        {
            VTextLine current = FirstLine;

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
                        VTextLine oldFirstLine = FirstLine;
                        FirstLine.PreviousLine = textLine;
                        FirstLine = textLine;
                        textLine.NextLine = oldFirstLine;
                        textLine.PhysicsRow = oldFirstLine.PhysicsRow - 1;
                        break;
                    }

                case MoveOptions.MoveToLast:
                    {
                        textLine.Remove();
                        VTextLine oldLastLine = LastLine;
                        LastLine.NextLine = textLine;
                        LastLine = textLine;
                        textLine.PreviousLine = oldLastLine;
                        textLine.PhysicsRow = oldLastLine.PhysicsRow + 1;
                        break;
                    }

                default:
                    throw new NotImplementedException();
            }

            ActiveLine = FirstLine.FindNext(Cursor.Row);
        }

        /// <summary>
        /// 重新设置文档可视区域的行和列
        /// </summary>
        /// <param name="viewportRow">文档新的可视区域行数</param>
        /// <param name="viewportColumn">文档新的可视区域的列数</param>
        public void Resize(int viewportRow, int viewportColumn)
        {
            // 如果行和列都没变化，那么就什么都不做
            if (this.viewportRow == viewportRow && this.viewportColumn == viewportColumn)
            {
                return;
            }

            // 缩放前先滚动到底，不然会有问题
            ScrollToBottom();

            // 对Document执行Resize
            // 目前的实现在ubuntu下没问题，但是在Windows10操作系统上运行Windows命令行里的vim程序会有问题，可能是Windows下的vim程序兼容性导致的，暂时先这样
            // 遇到过一种情况：如果终端名称不正确，比如XTerm，那么当行数增加的时候，光标会移动到该行的最右边，终端名称改成xterm就没问题了
            // 目前的实现思路是：如果是减少行，那么从第一行开始删除；如果是增加行，那么从最后一行开始新建行。不考虑ScrollMargin
            int scrollMin = Scrollbar.ScrollMin;
            int scrollMax = Scrollbar.ScrollMax;
            Resize(viewportRow, viewportColumn, scrollMin, scrollMax);

            // 更新滚动条
            Scrollbar.ViewportRow = viewportRow;

            if (IsAlternate)
            {
                #region 处理备用缓冲区

                // 备用缓冲区，因为SSH主机会重新打印所有字符，所以清空所有文本
                EraseAll();

                #endregion
            }
            else
            {
                #region 处理主缓冲区

                // 第一行的值就是滚动条的最大值
                int newScrollMax = FirstLine.PhysicsRow;
                if (Scrollbar.ScrollMax != newScrollMax)
                {
                    Scrollbar.ScrollMax = newScrollMax;
                    Scrollbar.ScrollValue = newScrollMax;

                    // 从第一行开始重新渲染显示终端内容
                    VTextLine currentLine = FirstLine;
                    while (currentLine != null)
                    {
                        VTHistoryLine historyLine;
                        if (Scrollbar.TryGetHistory(currentLine.PhysicsRow, out historyLine))
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
            RequestInvalidate();

            // 更新界面上的行和列
            this.viewportColumn = viewportRow;
            this.viewportRow = viewportColumn;
        }

        /// <summary>
        /// 设置当前要应用的文本装饰
        /// </summary>
        /// <param name="attribute">要应用的装饰类型</param>
        /// <param name="unset">是否应用该装饰</param>
        /// <param name="parameter">该状态对应的参数</param>
        public void SetAttribute(VTextAttributes attribute, bool enabled, object parameter)
        {
            VTUtils.SetTextAttribute(attribute, enabled, ref AttributeState.Value);

            switch (attribute)
            {
                case VTextAttributes.Background:
                    {
                        AttributeState.Background = parameter as VTColor;
                        break;
                    }

                case VTextAttributes.Foreground:
                    {
                        AttributeState.Foreground = parameter as VTColor;
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
            AttributeState.Reset();
        }

        public void CursorSave()
        {
            cursorState.Row = Cursor.Row;
            cursorState.Column = Cursor.Column;
        }

        public void CursorRestore()
        {
            SetCursor(cursorState.Row, cursorState.Column);
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
                        scrollTo = physicsRow - viewportRow / 2;
                        break;
                    }

                case ScrollOptions.ScrollToBottom:
                    {
                        scrollTo = physicsRow - viewportRow + 1;
                        break;
                    }

                default:
                    throw new NotImplementedException();
            }

            // 判断要滚动到的行是否超出了滚动最大值或者最小值
            if (scrollTo < Scrollbar.ScrollMin)
            {
                scrollTo = Scrollbar.ScrollMin;
            }
            else if (scrollTo > Scrollbar.ScrollMax)
            {
                scrollTo = Scrollbar.ScrollMax;
            }

            ScrollToHistory(scrollTo);
        }

        /// <summary>
        /// 把主缓冲区滚动到底
        /// </summary>
        public void ScrollToBottom()
        {
            if (!ScrollAtBottom)
            {
                ScrollToHistory(Scrollbar.ScrollMax);
            }
        }

        /// <summary>
        /// 获取当前使用鼠标选中的段落区域
        /// </summary>
        /// <returns></returns>
        public VTParagraph GetSelectedParagraph()
        {
            VTextSelection selection = Selection;
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
                        if (IsAlternate)
                        {
                            // 备用缓冲区直接保存VTextLine
                            VTextLine current = FirstLine;
                            while (current != null)
                            {
                                characters.Add(current.Characters);
                                current = current.NextLine;
                            }

                            startCharacterIndex = 0;
                            endCharacterIndex = Math.Max(0, LastLine.Characters.Count - 1);
                            firstPhysicsRow = FirstLine.PhysicsRow;
                            lastPhysicsRow = LastLine.PhysicsRow;
                        }
                        else
                        {
                            List<VTHistoryLine> historyLines;
                            if (!Scrollbar.TryGetHistories(Scrollbar.FirstLine.PhysicsRow, Scrollbar.LastLine.PhysicsRow, out historyLines))
                            {
                                logger.ErrorFormat("SaveAll失败, 有的历史记录为空");
                                return VTParagraph.Empty;
                            }

                            characters.AddRange(historyLines.Select(v => v.Characters));
                            startCharacterIndex = 0;
                            endCharacterIndex = Scrollbar.LastLine.Characters.Count - 1;
                            firstPhysicsRow = Scrollbar.FirstLine.PhysicsRow;
                            lastPhysicsRow = Scrollbar.LastLine.PhysicsRow;
                        }
                        break;
                    }

                case ParagraphTypeEnum.Viewport:
                    {
                        VTextLine current = FirstLine;
                        while (current != null)
                        {
                            characters.Add(current.Characters);
                            current = current.NextLine;
                        }

                        startCharacterIndex = 0;
                        endCharacterIndex = Math.Max(0, LastLine.Characters.Count - 1);
                        firstPhysicsRow = FirstLine.PhysicsRow;
                        lastPhysicsRow = LastLine.PhysicsRow;
                        break;
                    }

                case ParagraphTypeEnum.Selected:
                    {
                        if (Selection.IsEmpty)
                        {
                            return VTParagraph.Empty;
                        }

                        int topRow, bottomRow;
                        Selection.Normalize(out topRow, out bottomRow, out startCharacterIndex, out endCharacterIndex);
                        firstPhysicsRow = topRow;
                        lastPhysicsRow = bottomRow;

                        if (IsAlternate)
                        {
                            // 备用缓冲区没有滚动内容，只能选中当前显示出来的文档
                            VTextLine firstLine = FindLine(topRow);
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
                            if (!Scrollbar.TryGetHistories(topRow, bottomRow - topRow + 1, out historyLines))
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
                SessionName = String.Empty,
                CharactersList = characters,
                StartCharacterIndex = startCharacterIndex,
                EndCharacterIndex = endCharacterIndex,
                ContentType = fileType,
                Typeface = options.Typeface
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
                IsAlternate = IsAlternate
            };
        }

        /// <summary>
        /// 选中全部的文本
        /// </summary>
        public void SelectAll()
        {
            int firstRow = 0, lastRow = 0, lastCharacterIndex = 0;
            if (IsAlternate)
            {
                firstRow = FirstLine.PhysicsRow;
                lastRow = LastLine.PhysicsRow;
                lastCharacterIndex = LastLine.Characters.Count - 1;
            }
            else
            {
                VTHistoryLine startHistoryLine = Scrollbar.FirstLine;
                VTHistoryLine lastHistoryLine = Scrollbar.LastLine;
                firstRow = startHistoryLine.PhysicsRow;
                lastRow = lastHistoryLine.PhysicsRow;
                lastCharacterIndex = lastHistoryLine.Characters.Count - 1;
            }

            Selection.FirstRow = firstRow;
            Selection.LastRow = lastRow;
            Selection.FirstRowCharacterIndex = 0;
            Selection.LastRowCharacterIndex = lastCharacterIndex;
            Selection.RequestInvalidate();
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

            VTextLine next = FirstLine;

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
            Cursor.MakeInvalidate();
            Cursor.RequestInvalidate();

            #endregion

            #region 移动滚动条

            Scrollbar.RequestInvalidate();

            #endregion

            #region 更新选中区域

            Selection.RequestInvalidate();

            #endregion
        }

        /// <summary>
        /// 设置文档的可见性
        /// </summary>
        /// <param name="visible"></param>
        public void SetVisible(bool visible)
        {
            if (this.visible == visible)
            {
                return;
            }

            this.visible = visible;

            this.DrawingObject.Visible = visible;
        }

        #endregion

        #region 事件处理器

        private void OnMouseDown(VTPoint mouseLocation, int clickCount)
        {
            if (clickCount == 1)
            {
                isMouseDown = true;
                mouseDownPos = mouseLocation;

                if (!Selection.IsEmpty)
                {
                    // 点击的时候先清除选中区域
                    Selection.Clear();
                    Selection.RequestInvalidate();
                }
            }
            else
            {
                // 双击就是选中单词
                // 三击就是选中整行内容

                int startIndex = 0, endIndex = 0;

                VTextLine lineHit = HitTestHelper.HitTestVTextLine(FirstLine, mouseLocation.Y);
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
                            VTextRange characterRange;
                            if (!HitTestHelper.HitTestVTCharacter(lineHit, mouseLocation.X, out characterIndex, out characterRange))
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

                Selection.FirstRow = lineHit.PhysicsRow;
                Selection.FirstRowCharacterIndex = startIndex;
                Selection.LastRow = lineHit.PhysicsRow;
                Selection.LastRowCharacterIndex = endIndex;
                Selection.RequestInvalidate();
            }
        }

        private void OnMouseMove(VTPoint mouseLocation)
        {
            if (!isMouseDown)
            {
                return;
            }

            if (!selectionState)
            {
                // 此时说明开始选中操作
                selectionState = true;
                Selection.Clear();
                startPointer.CharacterIndex = -1;
                startPointer.PhysicsRow = -1;
                endPointer.CharacterIndex = -1;
                endPointer.PhysicsRow = -1;
            }

            // 整体思路是算出来StartTextPointer和EndTextPointer之间的几何图形
            // 然后渲染几何图形，SelectionRange本质上就是一堆矩形

            VTSize size = DrawingObject.Size;

            // 如果还没有测量起始字符，那么测量起始字符
            if (startPointer.CharacterIndex == -1)
            {
                if (!GetTextPointer(mouseLocation, size, startPointer))
                {
                    // 没有命中起始字符，那么直接返回啥都不做
                    //logger.DebugFormat("没命中起始字符");
                    return;
                }
            }

            // 首先检测鼠标是否在Surface边界框的外面
            // 如果在Surface的外面并且行数超出了Surface可以显示的最多行数，那么根据鼠标方向进行滚动，每次滚动一行
            ScrollIfCursorOutsideDocument(mouseLocation, size);

            // 更新当前鼠标的命中信息，保存在endPointer里
            if (!GetTextPointer(mouseLocation, size, endPointer))
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

            // 重新渲染
            // PerformDrawing会更新TextSelection的形状

            Selection.FirstRow = startPointer.PhysicsRow;
            Selection.FirstRowCharacterIndex = startPointer.CharacterIndex;
            Selection.LastRow = endPointer.PhysicsRow;
            Selection.LastRowCharacterIndex = endPointer.CharacterIndex;

            // 此处要全部刷新，因为有可能会触发ScrollIfCursorOutsideDocument
            // ScrollIfCursorOutsideDocument的情况下，要显示滚动后的数据
            RequestInvalidate();
        }

        private void OnMouseUp(VTPoint mouseLocation)
        {
            isMouseDown = false;
            selectionState = false;
        }

        private void OnMouseWheel(bool upper)
        {
            int scrollValue = Scrollbar.ScrollValue;
            int scrollMax = Scrollbar.ScrollMax;

            if (upper)
            {
                // 向上滚动

                // 先判断是不是已经滚动到顶了
                if (ScrollAtTop)
                {
                    // 滚动到顶直接返回
                    return;
                }

                if (scrollValue < scrollDelta)
                {
                    // 一次可以全部滚完并且还有剩余
                    ScrollToHistory(Scrollbar.ScrollMin);
                }
                else
                {
                    ScrollToHistory(scrollValue - scrollDelta);
                }
            }
            else
            {
                // 向下滚动

                if (ScrollAtBottom)
                {
                    // 滚动到底直接返回
                    return;
                }

                // 剩余可以往下滚动的行数
                int remainScroll = scrollMax - scrollValue;

                if (remainScroll >= scrollDelta)
                {
                    ScrollToHistory(scrollValue + scrollDelta);
                }
                else
                {
                    // 直接滚动到底
                    ScrollToHistory(scrollMax);
                }
            }

            // 重新渲染
            RequestInvalidate();
        }

        private void OnSizeChanged(VTSize newSize)
        {

        }

        private void OnScrollChanged(int scrollValue)
        {
            this.ScrollTo(scrollValue);
            this.RequestInvalidate();
        }

        /// <summary>
        /// 当有输入的时候触发
        /// </summary>
        /// <param name="text"></param>
        private void OnInput(VTInput input)
        {
            switch (input.Type)
            {
                case VTInputTypes.TextInput:
                    {
                        this.HandleTextInput(input);
                        break;
                    }

                case VTInputTypes.KeyInput:
                    {
                        this.HandleKeyInput(input);
                        break;
                    }

                default:
                    throw new NotImplementedException();
            }

            this.RequestInvalidate();
        }

        #endregion
    }
}

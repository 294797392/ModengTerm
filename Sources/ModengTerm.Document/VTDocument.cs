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

        #region 事件处理器

        /// <summary>
        /// 当滚动结束并渲染完毕之后触发
        /// 触发的时候，所有渲染对象的测量信息都是最新的
        /// 
        /// 有三个地方会触发该事件：
        /// 1. 使用鼠标中间滚动的时候（OnMouseWheel）
        /// 2. 使用鼠标拖动滚动条的时候（OnScrollChanged）
        /// 3. 选中屏幕之外的数据的时候（ScrollIfCursorOutsideDocument）
        /// 4. 在VideoTerminal接收并处理完数据之后触发（VideoTerminal.ProcessData，考虑到每换一行都会触发该事件，优化换行的时候触发该事件的时机）
        /// </summary>
        public event Action<VTDocument, VTScrollData> ScrollChanged;

        /// <summary>
        /// 当历史记录条数到达了设定的最大值并且被丢弃的时候触发
        /// </summary>
        public event Action<VTDocument> DiscardLine;

        #endregion

        #region 实例变量

        #region SelectionRange

        VTextPointer startPointer;
        VTextPointer endPointer;

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
        /// 鼠标滚轮滚动一次，滚动几行
        /// </summary>
        private int scrollDelta;

        /// <summary>
        /// 该文档是否可见
        /// </summary>
        private bool visible = true;

        private VTextLine activeLine;

        private VTHistory history;

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

        /// <summary>
        /// 当一行字符超出了最大列数之后是否自动换行
        /// </summary>
        public bool AutoWrapMode { get; set; }

        /// <summary>
        /// 当光标在该范围内就得滚动
        /// </summary>
        public int ScrollMarginTop { get; private set; }

        /// <summary>
        /// 当光标在该范围内就得滚动
        /// </summary>
        public int ScrollMarginBottom { get; private set; }

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

        /// <summary>
        /// 可视区域的行数
        /// </summary>
        public int ViewportRow { get { return viewportRow; } }

        /// <summary>
        /// 可视区域的列数
        /// </summary>
        public int ViewportColumn { get { return viewportColumn; } }

        /// <summary>
        /// 获取该文档所使用的字体Typeface
        /// </summary>
        public VTypeface Typeface { get { return options.Typeface; } }

        /// <summary>
        /// 获取该文档的事件输入
        /// </summary>
        public VTEventInput EventInput { get; private set; }

        public VTHistory History { get { return this.history; } }

        /// <summary>
        /// 滚动条的最大值
        /// 该值和可以保存的最多历史记录条数有关
        /// </summary>
        public int ScrollMax
        {
            get
            {
                if (this.options.ScrollbackMax == 0)
                {
                    return 0;
                }
                else
                {
                    return this.options.ScrollbackMax - this.viewportRow;
                }
            }
        }

        /// <summary>
        /// 获取当前显示的第一行的物理行号
        /// </summary>
        private int ViewportFirst { get { return this.Scrollbar.ScrollValue; } }

        /// <summary>
        /// 获取当前显示的最后一行的物理行号
        /// </summary>
        public int ViewportLast { get { return this.Scrollbar.ScrollValue + this.viewportRow - 1; } }

        #endregion

        #region 构造方法

        public VTDocument(VTDocumentOptions options)
        {
            this.options = options;
            Name = options.Name;
            DrawingObject = options.DrawingObject;
            this.EventInput = new VTEventInput()
            {
                OnMouseDown = this.OnMouseDown,
                OnMouseMove = this.OnMouseMove,
                OnMouseUp = this.OnMouseUp,
                OnMouseWheel = this.OnMouseWheel,
                OnScrollChanged = this.OnScrollChanged,
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
        private VTextLine CreateTextLine()
        {
            VTextLine textLine = new VTextLine(this)
            {
                OffsetX = 0,
                OffsetY = 0,
                Typeface = options.Typeface
            };

            textLine.Initialize();

            this.history.AddHistory(textLine.History);

            return textLine;
        }

        /// <summary>
        /// 滚动到指定的历史记录
        /// 并更新UI上的滚动条位置
        /// 注意该方法不会重新渲染界面，只修改文档模型
        /// </summary>
        /// <param name="scrollValue">要显示的第一行历史记录</param>
        /// <returns>如果进行了滚动，那么返回滚动数据，如果因为某种原因没进行滚动，那么返回空</returns>
        private VTScrollData ScrollToHistory(int scrollValue)
        {
            // 要滚动的值和当前值是一样的，也不滚动
            if (Scrollbar.ScrollValue == scrollValue)
            {
                return null;
            }

            // 判断要滚动的目标值合法性
            if (scrollValue > Scrollbar.ScrollMax ||
                scrollValue < Scrollbar.ScrollMin)
            {
                return null;
            }

            // 滚动前光标所在行
            int oldCursorRow = this.Cursor.Row;
            // 滚动后光标所在行
            int newCursorRow = 0;

            // 要滚动到的值
            int newScroll = scrollValue;
            // 滚动之前的值
            int oldScroll = Scrollbar.ScrollValue;

            // 需要进行滚动的行数
            int scrolledRows = Math.Abs(newScroll - oldScroll);

            List<VTHistoryLine> removedLines = new List<VTHistoryLine>();
            List<VTHistoryLine> addedLines = new List<VTHistoryLine>();

            #region 更新光标位置

            if (newScroll > oldScroll)
            {
                // 往下滚动，光标需要往上移动
                newCursorRow = oldCursorRow - scrolledRows;
                // 如果光标在第一行，那么newCursorRow就是负数
            }
            else
            {
                // 往上滚动，光标需要上下移动
                newCursorRow = oldCursorRow + scrolledRows;
                // 如果光标在最后一行，那么newCursorRow就会大于ViewportRow
            }

            #endregion

            #region 更新要显示的行

            if (scrolledRows >= viewportRow)
            {
                // 此时说明把所有行都滚动到屏幕外了，需要重新显示所有行

                // 遍历显示
                VTextLine current = this.FirstLine;
                for (int i = 0; i < viewportRow; i++)
                {
                    removedLines.Add(current.History);

                    int rowIndex = scrollValue + i;
                    VTHistoryLine historyLine;
                    if (this.history.TryGetHistory(rowIndex, out historyLine))
                    {
                        current.SetHistory(historyLine);
                    }
                    else
                    {
                        // 执行clear指令后，因为会增加行，所以可能会找不到对应的历史记录
                        // 打开终端 -> 输入enter直到翻了一整页 -> 滚动到最上面 -> 输入字符，就会复现
                        current.EraseAll();
                    }

                    addedLines.Add(current.History);

                    current = current.NextLine;
                }
            }
            else
            {
                // 此时说明只需要更新移动出去的行就可以了
                if (newScroll > oldScroll)
                {
                    // 往下滚动，把第一行拿到最后一行

                    // 从当前文档的最后一行的下一行开始显示
                    int lastRow = oldScroll + this.viewportRow;

                    for (int i = 0; i < scrolledRows; i++)
                    {
                        // 该值永远是第一行，因为下面被Move到最后一行了
                        VTextLine firstLine = this.FirstLine;

                        removedLines.Add(firstLine.History);

                        this.SwapLine(firstLine, this.LastLine);

                        VTHistoryLine historyLine;
                        if (this.history.TryGetHistory(lastRow + i, out historyLine))
                        {
                            firstLine.SetHistory(historyLine);
                        }
                        else
                        {
                            // 有可能会找不到该行对应的历史记录，找不到就清空
                            // 打开终端 -> clear -> 滚到最上面 -> 再往下滚，就会复现
                            firstLine.EraseAll();
                        }

                        addedLines.Add(firstLine.History);
                    }
                }
                else
                {
                    // 往上滚动，把最后一行拿到第一行

                    // 从当前文档的第一行的上一行开始显示
                    int firstRow = oldScroll - 1;

                    for (int i = 0; i < scrolledRows; i++)
                    {
                        VTHistoryLine historyLine;
                        if (!this.history.TryGetHistory(firstRow - i, out historyLine))
                        {
                            // 百分之百不可能找不到！！！
                            throw new NotImplementedException();
                        }

                        VTextLine lastLine = this.LastLine;

                        removedLines.Add(lastLine.History);

                        lastLine.SetHistory(historyLine);
                        this.SwapLineReverse(lastLine, this.FirstLine);

                        addedLines.Add(lastLine.History);
                    }
                }
            }

            #endregion

            // 更新当前滚动条的值
            Scrollbar.ScrollValue = scrollValue;

            // 有可能光标所在行被滚动到了文档外，此时要更新ActiveLine，ActiveLine就是空的
            this.SetCursor(newCursorRow, this.Cursor.Column);

            // 填充滚动数据
            return new VTScrollData()
            {
                NewScroll = newScroll,
                OldScroll = oldScroll,
                AddedLines = addedLines,
                RemovedLines = removedLines
            };
        }

        /// <summary>
        /// 当光标在容器外面移动的时候，进行滚动
        /// </summary>
        /// <param name="mousePosition">当前鼠标的坐标</param>
        /// <param name="documentSize">文档的大小</param>
        /// <param name="scrollData">保存滚动数据</param>
        /// <returns>如果进行了滚动，那么返回滚动数据，如果因为某种原因没进行滚动，那么返回空</returns>
        private VTScrollData ScrollIfCursorOutsideDocument(VTPoint mousePosition, VTSize documentSize)
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
                return ScrollToHistory(scrollTarget);
            }

            return null;
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

            // 逻辑索引，从0开始
            int logicalRow = 0;

            if (mouseY < 0)
            {
                // 光标在画布的上面，那么命中的行数就是第一行
                cursorLine = document.FirstLine;
            }
            else if (mouseY > documentSize.Height)
            {
                // 光标在画布的下面，那么命中的行数是最后一行
                cursorLine = document.LastLine;
                logicalRow = this.viewportRow - 1;
            }
            else
            {
                // 光标在画布中，那么做命中测试
                // 找到鼠标所在行
                cursorLine = HitTestHelper.HitTestVTextLine(document.FirstLine, mouseY, out logicalRow);
                if (cursorLine == null)
                {
                    // 这里说明鼠标没有在任何一行上
                    //logger.WarnFormat("没有找到鼠标位置对应的行, cursorY = {0}", mouseY);
                    return false;
                }
            }

            #endregion

            #region 再计算鼠标悬浮于哪个字符上

            int characterIndex;
            VTextRange textRange = VTextRange.Empty;

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
                if (!HitTestHelper.HitTestVTCharacter(cursorLine, mouseX, out characterIndex, out textRange))
                {
                    // 没有命中字符
                    //logger.WarnFormat("没有找到鼠标对应的字符, cursorY = {0}", mouseY);
                    return false;
                }
            }

            #endregion

            // 命中成功再更新TextPointer，保证pointer不为空
            pointer.TextLine = cursorLine;
            pointer.PhysicsRow = logicalRow + this.Scrollbar.ScrollValue;
            pointer.CharacterIndex = characterIndex;

            return true;
        }

        /// <summary>
        /// 从链表中移除指定的行
        /// </summary>
        /// <param name="textLine"></param>
        private void RemoveLine(VTextLine textLine)
        {
            if (textLine == this.FirstLine)
            {
                VTextLine next = textLine.NextLine;
                next.PreviousLine = null;
                this.FirstLine = next;
            }
            else if (textLine == this.LastLine)
            {
                VTextLine prev = textLine.PreviousLine;
                prev.NextLine = null;
                this.LastLine = prev;
            }
            else
            {
                VTextLine next = textLine.NextLine;
                next.PreviousLine = textLine.PreviousLine;
                textLine.PreviousLine.NextLine = next;
            }
        }

        #endregion

        #region 公开接口

        public void Initialize()
        {
            this.history = new VTMemoryHistory();
            this.history.MaxHistory = this.options.ScrollbackMax;
            this.history.Initialize();

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
            Scrollbar.Initialize();

            #region 初始化第一行，并设置链表首尾指针

            VTextLine firstLine = CreateTextLine();
            FirstLine = firstLine;
            LastLine = firstLine;
            ActiveLine = firstLine;

            #endregion

            // 默认创建80行，可见区域也是80行
            for (int i = 1; i < viewportRow; i++)
            {
                VTextLine textLine = CreateTextLine();
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

            this.history.Release();

            DrawingObject.DeleteDrawingObjects();
            FirstLine = null;
            LastLine = null;
            ActiveLine = null;
        }

        /// <summary>
        /// 把src挂载到dest后面
        /// 保持CursorY不变，更新光标所在行
        /// </summary>
        /// <param name="src"></param>
        /// <param name="dest"></param>
        public void SwapLine(VTextLine src, VTextLine dest)
        {
            if (src == dest)
            {
                throw new NotImplementedException("SwapLine警告, src == dest");
            }

            // 先把src拿出来
            this.RemoveLine(src);

            // 再把src挂到dest后面
            if (dest == this.LastLine)
            {
                /****************************
                 1.-----         2.-----
                 2.-----         3.----- 
                 3.-----      -> 4.----- 
                 4.-----         5.----- dest
                 5.----- dest    6.----- src
                 ****************************/

                dest.NextLine = src;
                src.PreviousLine = dest;
                src.NextLine = null;
                this.LastLine = src;
            }
            else if (dest == this.FirstLine)
            {
                /****************************
                 1.----- dest     1.----- dest
                 2.----- node1    2.----- src
                 3.-----       -> 3.----- node1
                 4.-----          4.----- 
                 5.-----          5.-----
                 ***************************/

                VTextLine node1 = dest.NextLine;

                dest.NextLine = src;
                src.PreviousLine = dest;
                src.NextLine = node1;
                node1.PreviousLine = src;
            }
            else
            {
                /***************************
                 1.-----          1.-----
                 2.----- dest     2.----- dest
                 3.----- node1 -> 3.----- src
                 4.-----          4.----- node1
                 5.-----          5.-----
                 ***************************/

                VTextLine node1 = dest.NextLine;

                dest.NextLine = src;
                src.PreviousLine = dest;
                src.NextLine = node1;
                node1.PreviousLine = src;
            }

            // 更新光标所在行
            this.ActiveLine = this.FirstLine.FindNext(this.Cursor.Row);
        }

        /// <summary>
        /// 把src挂载到dest前面
        /// 保持CursorY不变，并更新光标所在行
        /// </summary>
        /// <param name="src"></param>
        /// <param name="dest"></param>
        public void SwapLineReverse(VTextLine src, VTextLine dest)
        {
            if (src == dest)
            {
                throw new NotImplementedException("SwapLineReverse警告, src == dest");
            }

            // 先把src拿出来
            this.RemoveLine(src);

            // 再把src挂到dest前面
            if (dest == this.LastLine)
            {
                /****************************
                 1.-----          1.-----
                 2.-----          2.----- 
                 3.-----       -> 3.----- node1
                 4.----- node1    4.----- src
                 5.----- dest     5.----- dest
                 ****************************/

                VTextLine node1 = dest.PreviousLine;

                node1.NextLine = src;
                src.PreviousLine = node1;
                src.NextLine = dest;
                dest.PreviousLine = src;
            }
            else if (dest == this.FirstLine)
            {
                /****************************
                 1.----- dest     1.----- src
                 2.-----          2.----- dest
                 3.-----       -> 3.----- 
                 4.-----          4.----- 
                 5.-----          5.-----
                 ***************************/

                src.PreviousLine = null;
                src.NextLine = dest;
                dest.PreviousLine = src;
                this.FirstLine = src;
            }
            else
            {
                /***************************
                 1.----- node1    1.----- node1
                 2.----- dest     2.----- src
                 3.-----       -> 3.----- dest
                 4.-----          4.----- 
                 5.-----          5.-----
                 ***************************/

                VTextLine node1 = dest.NextLine;

                node1.NextLine = src;
                src.PreviousLine = node1;
                src.NextLine = dest;
                dest.PreviousLine = src;
            }

            // 更新光标所在行
            this.ActiveLine = this.FirstLine.FindNext(this.Cursor.Row);
        }

        /// <summary>
        /// 在当前光标位置打印一个字符
        /// TODO：该函数里的VTCharacter没有复用，需要改进
        /// </summary>
        /// <param name="ch"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        public void PrintCharacter(VTCharacter character)
        {
            this.ActiveLine.PrintCharacter(character, this.Cursor.Column);
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
            if (this.activeLine == null)
            {
                logger.WarnFormat("DeleteCharacter失败, ActiveLine为空");
                return;
            }

            VTextLine textLine = this.activeLine;

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
            }

            if (Cursor.Column != column)
            {
                Cursor.Column = column;
            }
        }

        /// <summary>
        /// 根据physicsRow找到对应的行
        /// </summary>
        /// <param name="row">指定要找到的行的索引</param>
        /// <returns></returns>
        public VTextLine FindLine(int physicsRow)
        {
            int scrollValue = this.Scrollbar.ScrollValue;
            int rowIndex = 0;

            VTextLine current = FirstLine;

            while (current != null)
            {
                if (rowIndex + scrollValue == physicsRow)
                {
                    return current;
                }

                rowIndex++;
                current = current.NextLine;
            }

            return null;
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

        public VTScrollData ScrollTo(int physicsRow)
        {
            return this.ScrollTo(physicsRow, ScrollOptions.ScrollToTop);
        }

        /// <summary>
        /// 滚动到指定的行
        /// </summary>
        /// <param name="physicsRow">要滚动到的物理行数</param>
        /// <param name="options">滚动选项</param>
        /// <param name="scrollData">用来保存本次滚动相关的数据</param>
        /// <returns>如果进行了滚动，那么返回true，否则返回false</returns>
        public VTScrollData ScrollTo(int physicsRow, ScrollOptions options = ScrollOptions.ScrollToTop)
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

            return ScrollToHistory(scrollTo);
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
        /// 选中全部的文本（包含滚动的文本）
        /// </summary>
        public void SelectAll()
        {
            if (this.history.FirstLine == null || this.history.LastLine == null)
            {
                logger.WarnFormat("SelectAll失败, FirstLine或者LastLine不存在");
                return;
            }

            VTHistoryLine startHistoryLine = this.history.FirstLine;
            VTHistoryLine lastHistoryLine = this.history.LastLine;
            int firstRow = 0;
            int lastRow = this.history.Lines - 1;
            int lastCharacterIndex = Math.Max(0, lastHistoryLine.Characters.Count - 1);

            Selection.StartRow = firstRow;
            Selection.EndRow = lastRow;
            Selection.StartColumn = 0;
            Selection.EndColumn = lastCharacterIndex;

            // 立即显示选中区域
            Selection.UpdateGeometry();
            Selection.RequestInvalidate();
        }

        /// <summary>
        /// 选中当前显示区域的所有文本
        /// </summary>
        public void SelectViewport()
        {
            Selection.StartRow = this.ViewportFirst;
            Selection.EndRow = this.ViewportLast;
            Selection.StartColumn = 0;
            Selection.EndColumn = Math.Max(0, this.LastLine.Characters.Count - 1);

            // 立即显示选中区域
            Selection.UpdateGeometry();
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

            if (!Selection.IsEmpty)
            {
                // 根据当前选中数据和文档信息，重新计算选中区域要显示的位置
                // 在渲染的时候再计算选中区域，因为此时所有元素的边界框都是最新的
                Selection.UpdateGeometry();
            }

            // 如果在之前调用了Clear，此时IsEmpty是false，但是需要重绘
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

        public void Resize(int oldRow, int newRow, int oldCol, int newCol)
        {
            if (newRow != oldRow)
            {
                int rows = Math.Abs(newRow - oldRow);

                if (newRow > oldRow)
                {
                    // 增加了行数
                    for (int i = 0; i < rows; i++)
                    {
                        VTextLine textLine = this.CreateTextLine();

                        this.LastLine.NextLine = textLine;
                        textLine.PreviousLine = this.LastLine;
                        this.LastLine = textLine;
                    }
                }
                else
                {
                    // 删除了行数
                    for (int i = 0; i < rows; i++)
                    {
                        this.RemoveLine(this.LastLine);
                        this.LastLine.Release();
                    }
                }

                // 更新滚动条信息
                if (this.options.ScrollbackMax > 0)
                {
                    int scrollMax = this.History.Lines - newRow;
                    if (scrollMax > 0)
                    {
                        this.Scrollbar.ScrollMax = Math.Min(scrollMax, this.ScrollMax);
                        this.Scrollbar.ScrollValue = this.Scrollbar.ScrollMax;
                    }
                }

                #region 更新光标所在行

                // 思路是保持光标所在行不变



                #endregion
            }

            if (newCol != oldCol)
            {
                // 对于行来说，暂时没啥需要处理的
            }

            this.viewportRow = newRow;
            this.viewportColumn = newCol;
        }

        /// <summary>
        /// 手动触发滚动事件
        /// </summary>
        /// <param name="scrollData"></param>
        public void InvokeScrollChanged(VTScrollData scrollData)
        {
            if (this.ScrollChanged != null)
            {
                this.ScrollChanged(this, scrollData);
            }
        }

        #endregion

        #region 事件处理器

        private void OnMouseDown(MouseData mouseData)
        {
            if (mouseData.ClickCount == 1)
            {
                if (!Selection.IsEmpty)
                {
                    // 点击的时候先清除选中区域
                    Selection.Clear();
                    Selection.RequestInvalidate();
                    selectionState = false;
                }

                mouseData.CaptureAction = MouseData.CaptureActions.Capture;
            }
            else
            {
                // 双击就是选中单词
                // 三击就是选中整行内容

                int startIndex = 0, endIndex = 0, logicalRow = 0;

                VTextLine lineHit = HitTestHelper.HitTestVTextLine(FirstLine, mouseData.Y, out logicalRow);
                if (lineHit == null)
                {
                    return;
                }

                switch (mouseData.ClickCount)
                {
                    case 2:
                        {
                            // 选中单词
                            string text = VTUtils.CreatePlainText(lineHit.Characters);
                            int characterIndex;
                            VTextRange characterRange;
                            if (!HitTestHelper.HitTestVTCharacter(lineHit, mouseData.X, out characterIndex, out characterRange))
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

                Selection.StartRow = this.Scrollbar.ScrollValue + logicalRow;
                Selection.StartColumn = startIndex;
                Selection.EndRow = this.Scrollbar.ScrollValue + logicalRow;
                Selection.EndColumn = endIndex;
                Selection.RequestInvalidate();
            }
        }

        private void OnMouseMove(MouseData mouseData)
        {
            if (!mouseData.IsMouseCaptured)
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

            VTPoint mouseLocation = new VTPoint(mouseData.X, mouseData.Y);

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
            VTScrollData scrollData = ScrollIfCursorOutsideDocument(mouseLocation, size);

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

            Selection.StartRow = startPointer.PhysicsRow;
            Selection.StartColumn = startPointer.CharacterIndex;
            Selection.EndRow = endPointer.PhysicsRow;
            Selection.EndColumn = endPointer.CharacterIndex;

            // 此处要全部刷新，因为有可能会触发ScrollIfCursorOutsideDocument
            // ScrollIfCursorOutsideDocument的情况下，要显示滚动后的数据
            RequestInvalidate();

            if (scrollData != null)
            {
                this.InvokeScrollChanged(scrollData);
            }
        }

        private void OnMouseUp(MouseData mouseData)
        {
            selectionState = false;
            mouseData.CaptureAction = MouseData.CaptureActions.ReleaseCapture;
        }

        private void OnMouseWheel(bool upper)
        {
            int oldScroll = Scrollbar.ScrollValue;
            int scrollMax = Scrollbar.ScrollMax;
            int newScroll = 0; // 最终要滚动到的值

            if (upper)
            {
                // 向上滚动

                // 先判断是不是已经滚动到顶了
                if (ScrollAtTop)
                {
                    // 滚动到顶直接返回
                    return;
                }

                if (oldScroll < scrollDelta)
                {
                    // 一次可以全部滚完并且还有剩余
                    newScroll = Scrollbar.ScrollMin;
                }
                else
                {
                    newScroll = oldScroll - scrollDelta;
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
                int remainScroll = scrollMax - oldScroll;

                if (remainScroll >= scrollDelta)
                {
                    newScroll = oldScroll + scrollDelta;
                }
                else
                {
                    // 直接滚动到底
                    newScroll = scrollMax;
                }
            }

            VTScrollData scrollData = this.ScrollToHistory(newScroll);
            if (scrollData == null)
            {
                return;
            }

            // 重新渲染
            RequestInvalidate();

            this.InvokeScrollChanged(scrollData);
        }

        private void OnScrollChanged(ScrollChangedData changed)
        {
            VTScrollData scrollData = this.ScrollTo(changed.NewScroll);
            if (scrollData == null) 
            {
                return;
            }

            this.RequestInvalidate();

            this.InvokeScrollChanged(scrollData);
        }

        #endregion
    }
}

﻿using ModengTerm.Document.Drawing;
using ModengTerm.Document.Enumerations;
using ModengTerm.Document.EventData;
using ModengTerm.Document.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModengTerm.Document
{
    /// <summary>
    /// 文档数据模型
    /// 负责维护文档的所有状态，用户输入
    /// 为了提高性能，所有函数只修改文档的数据结构，不渲染。由调用者统一在外部调用RequestInvalidate进行渲染
    /// </summary>
    public class VTDocument
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("VTDocument");

        #endregion

        #region 公开事件

        /// <summary>
        /// 当滚动结束并渲染完毕之后触发
        /// 触发的时候，所有渲染对象的测量信息都是最新的
        /// 
        /// 有三个地方会触发该事件：
        /// 1. 使用鼠标滚轮滚动的时候（OnMouseWheel）
        /// 2. 使用鼠标拖动滚动条的时候（OnScrollChanged）
        /// 3. 选中屏幕之外的数据的时候（ScrollIfCursorOutsideDocument）
        /// 4. 在VideoTerminal.Render的时候触发
        /// </summary>
        public event Action<VTDocument, VTScrollData> ScrollChanged;

        /// <summary>
        /// 当历史记录条数到达了设定的最大值并且被丢弃的时候触发
        /// </summary>
        public event Action<VTDocument> DiscardLine;

        /// <summary>
        /// 当渲染结束之后触发
        /// </summary>
        public event Action<VTDocument, VTRenderData> Rendering;

        /// <summary>
        /// 当按下鼠标的时候触发
        /// </summary>
        public event Action<VTDocument, MouseData> MouseDown;
        public event Action<VTDocument, MouseData> MouseUp;

        #endregion

        #region 实例变量

        #region SelectionRange

        private VTextPointer startPointer;
        private VTextPointer endPointer;

        /// <summary>
        /// 当前鼠标是否处于Selection状态
        /// </summary>
        private bool selectionState;
        private bool isMouseDown;

        private VTCursor cursor;

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

        private GraphicsInterface graphicsInterface;

        #endregion

        #region 属性

        /// <summary>
        /// 文档的名字，方便调试
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 记录光标信息
        /// </summary>
        public VTCursor Cursor { get { return this.cursor; } }

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
        /// 该文档的图形接口
        /// </summary>
        public GraphicsInterface GraphicsInterface { get { return this.graphicsInterface; } }

        /// <summary>
        /// 当前应用的文本属性
        /// </summary>
        public VTextAttributeState AttributeState { get; private set; }

        /// <summary>
        /// 获取当前保存的光标状态
        /// </summary>
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
        /// 获取该文档的历史记录信息
        /// </summary>
        public VTHistory History { get { return this.history; } }

        /// <summary>
        /// 对历史记录行数做限制
        /// 获取滚动条可以滚动到的最大值
        /// 滚动条的最大值不可以超出该值
        /// </summary>
        public int RollbackMax
        {
            get
            {
                return this.options.RollbackMax;
            }
        }

        /// <summary>
        /// 可视区域的第一行物理行号
        /// </summary>
        public int FirstPhysicsRow
        {
            get { return this.Scrollbar.Value; }
        }

        /// <summary>
        /// 可视区域最后一行的物理行号
        /// </summary>
        public int LastPhysicsRow
        {
            get
            {
                return this.Scrollbar.Value + this.viewportRow - 1;
            }
        }

        #endregion

        #region 构造方法

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="options">文档参数</param>
        public VTDocument(VTDocumentOptions options)
        {
            this.options = options;
            this.Name = options.Name;

            this.graphicsInterface = options.GraphicsInterface;
            this.graphicsInterface.GIMouseDown += GraphicsInterface_GIMouseDown;
            this.graphicsInterface.GIMouseUp += GraphicsInterface_GIMouseUp;
            this.graphicsInterface.GIMouseMove += GraphicsInterface_GIMouseMove;
            this.graphicsInterface.GIMouseWheel += GraphicsInterface_GIMouseWheel;
            this.graphicsInterface.GIScrollChanged += GraphicsInterface_GIScrollChanged;
            this.graphicsInterface.GILoaded += GraphicsInterface_GILoaded;

            this.cursorState = new VTCursorState();
            this.startPointer = new VTextPointer();
            this.startPointer.ColumnIndex = -1;
            this.endPointer = new VTextPointer();
            this.endPointer.ColumnIndex = -1;
            this.scrollDelta = options.ScrollDelta;
            this.AttributeState = new VTextAttributeState();
            this.viewportRow = options.ViewportRow;
            this.viewportColumn = options.ViewportColumn;
        }

        #endregion

        #region 实例方法

        /// <summary>
        /// 创建一个新行，该行不挂在链表上
        /// </summary>
        /// <param name="addToHistory">是否在历史行里新加一行</param>
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

            return textLine;
        }

        /// <summary>
        /// 滚动到指定的历史记录
        /// 并更新UI上的滚动条位置
        /// 注意该方法不会重新渲染界面，只修改文档模型
        /// </summary>
        /// <param name="physicsRow">要显示的第一行历史记录</param>
        /// <returns>如果进行了滚动，那么返回滚动数据，如果因为某种原因没进行滚动，那么返回空</returns>
        private VTScrollData ScrollToHistory(int physicsRow)
        {
            // 要滚动的值和当前值是一样的，也不滚动
            if (Scrollbar.Value == physicsRow)
            {
                return null;
            }

            // 判断要滚动的目标值合法性
            if (physicsRow > Scrollbar.Maximum ||
                physicsRow < Scrollbar.Minimum)
            {
                return null;
            }

            // 要滚动到的值
            int newScroll = physicsRow;
            // 滚动之前的值
            int oldScroll = Scrollbar.Value;

            // 需要进行滚动的行数
            int scrolledRows = Math.Abs(newScroll - oldScroll);

            List<VTHistoryLine> removedLines = new List<VTHistoryLine>();
            List<VTHistoryLine> addedLines = new List<VTHistoryLine>();

            #region 更新要显示的行

            if (scrolledRows >= viewportRow)
            {
                // 此时说明把所有行都滚动到屏幕外了，需要重新显示所有行

                // 遍历显示
                VTextLine current = this.FirstLine;
                for (int i = 0; i < viewportRow; i++)
                {
                    removedLines.Add(current.History);

                    int rowIndex = physicsRow + i;
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
                            // 有可能会找不到该行对应的历史记录，此时说明该行还没有被打印过，新建一个空的历史行
                            // 打开终端 -> clear -> 滚到最上面 -> 再往下滚，就会复现
                            historyLine = new VTHistoryLine();
                            firstLine.SetHistory(historyLine);
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
            Scrollbar.Value = physicsRow;

            #region 更新ActiveLine

            // 光标所在物理行号不变，ActiveLine有可能会为空（ActiveLine被滚动到可视区域外之后就是空）

            // 更新ActiveLine
            this.SetCursorPhysical(this.Cursor.PhysicsRow);

            #endregion

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
        private VTScrollData ScrollIfCursorOutsideDocument(IMousePosition mousePosition)
        {
            double mouseX = mousePosition.X;
            double mouseY = mousePosition.Y;

            VTSize displaySize = this.graphicsInterface.DrawAreaSize;

            // 要滚动到的目标行
            int scrollTarget = -1;

            if (mouseY < 0)
            {
                // 光标在容器上面
                if (!ScrollAtTop)
                {
                    // 不在最上面，往上滚动一行
                    scrollTarget = Scrollbar.Value - 1;
                }
            }
            else if (mouseY > displaySize.Height)
            {
                // 光标在容器下面
                if (!ScrollAtBottom)
                {
                    // 往下滚动一行
                    scrollTarget = Scrollbar.Value + 1;
                }
            }

            if (scrollTarget != -1)
            {
                return ScrollToHistory(scrollTarget);
            }

            return null;
        }

        #endregion

        #region 公开接口

        public void Initialize()
        {
            this.history = new VTHistory();
            // 最多历史行数 = 回滚行数 + 可视区域行数
            this.history.MaxHistory = this.options.RollbackMax + this.options.ViewportRow;
            this.history.Initialize();

            this.cursor = new VTCursor(this);
            this.cursor.OffsetX = 0;
            this.cursor.OffsetY = 0;
            this.cursor.Row = 0;
            this.cursor.Column = 0;
            this.cursor.Interval = (int)options.CursorSpeed;
            this.cursor.AllowBlink = true;
            this.cursor.IsVisible = true;
            this.cursor.Color = options.CursorColor;
            this.cursor.Style = options.CursorStyle;
            this.cursor.Typeface = options.Typeface;
            this.cursor.Initialize();

            Selection = new VTextSelection(this);
            Selection.Color = options.SelectionColor;
            Selection.StartPointer = this.startPointer;
            Selection.EndPointer = this.endPointer;
            Selection.Initialize();

            Scrollbar = new VTScrollInfo(this);
            Scrollbar.ViewportRow = this.options.ViewportRow;
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
            this.cursor.Release();
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

            graphicsInterface.DeleteDrawingObjects();
            FirstLine = null;
            LastLine = null;
            ActiveLine = null;
        }

        /// <summary>
        /// 把src挂载到dest后面
        /// 不更新光标所在行(ActiveLine)
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
        }

        /// <summary>
        /// 把src挂载到dest前面
        /// 不更新光标所在行
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

                VTextLine node1 = dest.PreviousLine;

                node1.NextLine = src;
                src.PreviousLine = node1;
                src.NextLine = dest;
                dest.PreviousLine = src;
            }
        }

        /// <summary>
        /// 在当前光标位置打印一个字符，不移动光标位置
        /// TODO：该函数里的VTCharacter没有复用，需要改进
        /// </summary>
        /// <param name="ch"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        public void PrintCharacter(VTCharacter character)
        {
            this.PrintCharacter(character, this.cursor.Column);
        }

        /// <summary>
        /// 在光标所在行的指定列打印一个字符
        /// </summary>
        /// <param name="character"></param>
        /// <param name="column"></param>
        public void PrintCharacter(VTCharacter character, int column)
        {
            this.ActiveLine.PrintCharacter(character, column);
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

        /// <summary>
        /// 用空白符填充所有文本
        /// </summary>
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
        /// 删除当前视图中的所有字符
        /// </summary>
        public void DeleteViewoprt()
        {
            VTextLine current = this.FirstLine;

            while (current != null)
            {
                current.DeleteAll();

                current = current.NextLine;
            }
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
                VTCharacter character = VTCharacter.CreateEmpty();

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
        /// 设置光标在当前视图中逻辑位置和activeLine
        /// row和column从0开始计数，0表示第一行或者第一列
        /// </summary>
        /// <param name="row">要设置的行</param>
        /// <param name="column">要设置的列</param>
        public void SetCursorLogical(int row, int column)
        {
            // 有可能ActiveLine变了，更新ActiveLine和PhysicsRow
            this.cursor.Row = row;
            VTextLine newActiveLine = this.FirstLine.FindNext(row);
            if (this.activeLine != newActiveLine)
            {
                this.activeLine = newActiveLine;
            }
            this.cursor.PhysicsRow = this.Scrollbar.Value + row;
            this.cursor.Column = column;
        }

        /// <summary>
        /// 设置光标的物理行号
        /// 如果物理行号没变化，那么尝试更新ActiveLine
        /// </summary>
        /// <param name="row"></param>
        public void SetCursorPhysical(int row)
        {
            // 物理行号没变化，逻辑行号或者ActiveLine会变化（当光标在可视区域内，滚动滚动条）
            if (this.cursor.PhysicsRow == row)
            {
                int logicalRow = row - this.Scrollbar.Value;
                this.cursor.Row = logicalRow;
                this.ActiveLine = this.FirstLine.FindNext(logicalRow);
                return;
            }

            if (this.OutsideViewport(row))
            {
                // 光标在可视区域外
                this.ActiveLine = null;
            }
            else
            {
                int logicalRow = row - this.Scrollbar.Value;
                this.cursor.Row = logicalRow;
                this.ActiveLine = this.FirstLine.FindNext(logicalRow);
            }

            this.cursor.PhysicsRow = row;
        }

        /// <summary>
        /// 判断物理行是否在可视区域外
        /// </summary>
        /// <param name="physicsRow">要判断的物理行号</param>
        /// <returns></returns>
        public bool OutsideViewport(int physicsRow)
        {
            if (physicsRow >= this.FirstPhysicsRow &&
                physicsRow <= this.LastPhysicsRow)
            {
                // 此时说明光标在可视区域内
                return false;
            }

            return true;
        }

        /// <summary>
        /// 根据physicsRow找到对应的行
        /// </summary>
        /// <param name="row">指定要找到的行的索引</param>
        /// <returns></returns>
        public VTextLine FindLine(int physicsRow)
        {
            if (this.OutsideViewport(physicsRow))
            {
                return null;
            }

            // 要找的行在可视区域中

            // 往前找几行
            int nrows = physicsRow - this.FirstPhysicsRow;

            // TODO：使用索引查找提高速度
            return this.FirstLine.FindNext(nrows);
        }

        /// <summary>
        /// 设置当前要应用的文本装饰
        /// </summary>
        /// <param name="attribute">要应用的装饰类型</param>
        /// <param name="unset">是否应用该装饰</param>
        /// <param name="parameter">该状态对应的参数</param>
        public void SetAttribute(VTextAttributes attribute, bool enabled, object color)
        {
            VTUtils.SetTextAttribute(attribute, enabled, ref AttributeState.Value);

            switch (attribute)
            {
                case VTextAttributes.Background:
                    {
                        AttributeState.Background = color as VTColor;
                        break;
                    }

                case VTextAttributes.Foreground:
                    {
                        AttributeState.Foreground = color as VTColor;
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
            SetCursorLogical(cursorState.Row, cursorState.Column);
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
            if (scrollTo < Scrollbar.Minimum)
            {
                scrollTo = Scrollbar.Minimum;
            }
            else if (scrollTo > Scrollbar.Maximum)
            {
                scrollTo = Scrollbar.Maximum;
            }

            return ScrollToHistory(scrollTo);
        }

        /// <summary>
        /// 往下滚动一行
        /// </summary>
        public void ScrollDown()
        {
            this.ScrollTo(this.Scrollbar.Value + 1);
        }

        /// <summary>
        /// 往上滚动一行
        /// </summary>
        public void ScrollUp()
        {
            this.ScrollTo(this.Scrollbar.Value - 1);
        }

        /// <summary>
        /// 把主缓冲区滚动到底
        /// </summary>
        public void ScrollToBottom()
        {
            if (!ScrollAtBottom)
            {
                ScrollToHistory(Scrollbar.Maximum);
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

            // 立即显示选中区域
            this.Selection.SelectAll();
        }

        /// <summary>
        /// 清空选中的区域
        /// </summary>
        public void UnSelectAll()
        {
            this.Selection.Clear();
        }

        /// <summary>
        /// 选中当前显示区域的所有文本
        /// </summary>
        public void SelectViewport()
        {
            this.Selection.SelectViewport();
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

            // 光标闪烁在单独线程处理，这里只对光标进行重新定位
            Cursor.Reposition();

            #endregion

            #region 移动滚动条

            Scrollbar.Render();

            #endregion

            #region 更新选中区域

            if (!Selection.IsEmpty)
            {
                // 根据当前选中数据和文档信息，重新计算选中区域要显示的位置
                // 在渲染的时候再计算选中区域，因为此时所有元素的边界框都是最新的

                // 滚动了之后需要更新
                Selection.UpdateGeometry();
            }

            Selection.RequestInvalidate();

            #endregion

            this.Rendering?.Invoke(this, VTRenderData.Instance);
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

            this.graphicsInterface.Visible = visible;
        }

        /// <summary>
        /// 重置文档的行和列大小
        /// </summary>
        /// <param name="newRow">新的行数</param>
        /// <param name="newCol">新的列数</param>
        public void Resize(int newRow, int newCol)
        {
            int oldRow = this.viewportRow;
            int oldCol = this.viewportColumn;

            VTScrollInfo scrollInfo = this.Scrollbar;
            int oldScrollValue = scrollInfo.Value;
            int newScrollValue = 0;
            int oldScrollMax = scrollInfo.Maximum;
            int newScrollMax = 0;
            int allRows = this.history.Lines;

            if (newRow != oldRow)
            {
                // 计算减了或者增加了多少行
                int rows = Math.Abs(oldRow - newRow);

                if (newRow > oldRow)
                {
                    // 增加了行数

                    for (int i = 0; i < rows; i++)
                    {
                        VTextLine textLine = this.CreateTextLine();
                        this.LastLine.Append(textLine);
                    }
                }
                else
                {
                    // 减少了行数

                    for (int i = 0; i < rows; i++)
                    {
                        this.LastLine.Release();

                        this.RemoveLine(this.LastLine);
                    }
                }

                // 更新滚动条信息
                scrollInfo.ViewportRow = newRow;
            }

            if (newCol != oldCol)
            {
                // 对于列来说，暂时没啥需要处理的
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

        /// <summary>
        /// 获取当前视野中显示的行数
        /// </summary>
        /// <param name="scrollValue">滚动条的值</param>
        /// <returns></returns>
        public int GetDisplayRows(int scrollValue)
        {
            return Math.Max(0, history.Lines - scrollValue);
        }

        public VTParagraph GetParagraph(VTParagraphOptions options)
        {
            ParagraphFormatEnum formatType = options.FormatType;
            int firstPhysicsRow = options.FirstPhysicsRow;
            int lastPhysicsRow = options.LastPhysicsRow;
            int startColumn = options.StartColumn;
            int endColumn = options.EndColumn;

            List<List<VTCharacter>> charactersList = new List<List<VTCharacter>>();

            // 为了兼容AlternateDocument, 如果要创建的内容都在可视区域里，那么用可视区域里的数据
            // 因为AlternateDocument不会保存历史记录
            int topPhysicsRow = this.Scrollbar.Value;
            int bottomPhysicsRow = this.Scrollbar.Value + this.viewportRow - 1;

            if (firstPhysicsRow >= topPhysicsRow && lastPhysicsRow <= bottomPhysicsRow)
            {
                // 选中内容在可视区域内
                int n = lastPhysicsRow - firstPhysicsRow + 1;
                VTextLine current = this.FindLine(firstPhysicsRow);
                for (int i = 0; i < n; i++)
                {
                    charactersList.Add(current.Characters);

                    current = current.NextLine;
                }
            }
            else
            {
                IEnumerable<VTHistoryLine> historyLines;
                if (!this.history.TryGetHistories(firstPhysicsRow, lastPhysicsRow, out historyLines))
                {
                    return VTParagraph.Empty;
                }

                charactersList.AddRange(historyLines.Select(v => v.Characters));
            }

            CreateLineDelegate createLine = VTUtils.GetCreateLineDelegate(formatType);
            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < charactersList.Count; i++)
            {
                List<VTCharacter> characters = charactersList[i];

                // 第一个字符的索引
                int startIndex = 0;
                // 字符数量
                int count = 0;

                if (i == 0 && charactersList.Count == 1)
                {
                    // 优先处理只选中了一行的情况
                    startIndex = VTUtils.GetCharacterIndex(characters, startColumn);
                    int endIndex = VTUtils.GetCharacterIndex(characters, endColumn);
                    count = endIndex - startIndex + 1;
                }
                else if (i == 0)
                {
                    // 第一行
                    startIndex = VTUtils.GetCharacterIndex(characters, startColumn);
                    count = characters.Count - startIndex;
                }
                else if (i == charactersList.Count - 1)
                {
                    // 最后一行
                    startIndex = 0;
                    count = VTUtils.GetCharacterIndex(characters, endColumn) + 1;
                }
                else
                {
                    startIndex = 0;
                    count = characters.Count;
                }

                // VTUtils.GetCharacterIndex有可能返回-1，在回放的时候复制会有这个问题，暂时没找到原因
                if (startIndex == -1)
                {
                    continue;
                }

                createLine(characters, builder, startIndex, count, i == lastPhysicsRow);
            }

            //if (fileType == ParagraphFormatEnum.HTML)
            //{
            //    string htmlBackground = VTColor.CreateFromRgbKey(parameter.Typeface.BackgroundColor).Html;
            //    string htmlForeground = VTColor.CreateFromRgbKey(parameter.Typeface.ForegroundColor).Html;
            //    return string.Format(HtmlTemplate, parameter.SessionName, builder.ToString(), htmlBackground,
            //    parameter.Typeface.FontSize, parameter.Typeface.FontFamily, htmlForeground);
            //}

            return new VTParagraph()
            {
                Content = builder.ToString()
            };
        }

        /// <summary>
        /// 从文档中移除指定的行
        /// 只移除，不释放被移除的行的资源
        /// </summary>
        /// <param name="textLine">要移除的行</param>
        public void RemoveLine(VTextLine textLine)
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

        /// <summary>
        /// 把Viewport里的指定区域里的数据更新到History里
        /// 只更指定区域里的数据，包含marginTop和marginBottom两行
        /// </summary>
        /// <param name="marginTop">要更新的区域的MarginTop</param>
        /// <param name="marginBottom">要更新的区域的MarginBottom</param>
        public void UpdateViewportHistory(int marginTop, int marginBottom)
        {
            VTScrollInfo scrollInfo = this.Scrollbar;

            // 更新区域内的第一行的物理行号
            int regionTopLinePhysicsRow = scrollInfo.Value + marginTop;
            VTextLine regionTopLine = this.FirstLine.FindNext(marginTop);
            VTextLine regionBottomLine = this.LastLine.FindPrevious(marginBottom);

            VTextLine nextLine = regionTopLine;
            int nextPhysicsRow = regionTopLinePhysicsRow;
            while (true)
            {
                history.Set(nextPhysicsRow, nextLine.History);
                nextPhysicsRow++;

                if (nextLine == regionBottomLine)
                {
                    break;
                }

                nextLine = nextLine.NextLine;
            }
        }

        /// <summary>
        /// 使用像素坐标对VTextLine做命中测试
        /// </summary>
        /// <param name="mousePosition">鼠标坐标</param>
        /// <param name="pointer">存储命中测试结果的变量</param>
        /// <remarks>如果传递进来的鼠标位置在窗口外，那么会把鼠标限定在距离鼠标最近的Surface边缘处</remarks>
        /// <returns>
        /// 是否获取成功
        /// 当光标不在任何一行上的时候，就获取失败
        /// </returns>
        public bool HitTest(IMousePosition mousePosition, VTextPointer pointer)
        {
            double mouseX = mousePosition.X;
            double mouseY = mousePosition.Y;

            VTDocument document = this;
            VTSize displaySize = this.graphicsInterface.DrawAreaSize;

            #region 先计算鼠标位于哪一行上

            VTextLine textLine = null;

            // 逻辑索引，从0开始
            int logicalRow = 0;

            if (mouseY < 0)
            {
                // 光标在画布的上面，那么命中的行数就是第一行
                textLine = document.FirstLine;
            }
            else if (mouseY > displaySize.Height)
            {
                // 光标在画布的下面，那么命中的行数是最后一行
                textLine = document.LastLine;
                logicalRow = this.viewportRow - 1;
            }
            else
            {
                // 光标在画布中，那么做命中测试
                // 找到鼠标所在行
                textLine = HitTestHelper.HitTestVTextLine(document, mouseY, out logicalRow);
                if (textLine == null)
                {
                    // 这里说明鼠标没有在任何一行上
                    //logger.WarnFormat("没有找到鼠标位置对应的行, cursorY = {0}", mouseY);
                    return false;
                }
            }

            #endregion

            #region 再计算鼠标悬浮于哪个字符上

            int charIndexHit;
            int columnIndexHit;
            VTextRange textRange = VTextRange.Empty;

            if (mouseX < 0)
            {
                // 鼠标在画布左边，那么悬浮的就是第一个字符
                charIndexHit = 0;
                columnIndexHit = 0;
            }
            else if (mouseX > displaySize.Width)
            {
                // 鼠标在画布右边，那么悬浮的就是最后一个字符
                charIndexHit = textLine.Characters.Count - 1;
                columnIndexHit = this.viewportColumn - 1;
            }
            else
            {
                // 鼠标的水平方向在画布中间，那么做字符命中测试
                HitTestHelper.HitTestVTCharacter(textLine, mouseX, out charIndexHit, out textRange, out columnIndexHit);
            }

            #endregion

            // 命中成功再更新TextPointer，保证pointer不为空
            pointer.PhysicsRow = logicalRow + this.Scrollbar.Value;
            pointer.LogicalRow = logicalRow;
            pointer.CharacterIndex = charIndexHit;
            pointer.ColumnIndex = columnIndexHit;

            return true;
        }

        #endregion

        #region 事件处理器

        private void GraphicsInterface_GIMouseMove(GraphicsInterface graphicsInterface, MouseData mouseData)
        {
            if (this.isMouseDown)
            {
                if (!this.graphicsInterface.GIMouseCaptured)
                {
                    this.graphicsInterface.GICaptureMouse();
                }

                // 整体思路是算出来StartTextPointer和EndTextPointer之间的几何图形
                // 然后渲染几何图形，SelectionRange本质上就是一堆矩形

                if (!selectionState)
                {
                    // 此时说明开始选中操作
                    selectionState = true;
                    Selection.Clear();
                }

                // 如果还没有测量起始字符，那么测量起始字符
                if (startPointer.ColumnIndex == -1)
                {
                    if (!HitTest(mouseData, startPointer))
                    {
                        // 没有命中起始字符，那么直接返回啥都不做
                        //logger.DebugFormat("没命中起始字符");
                        return;
                    }
                }

                // 首先检测鼠标是否在Surface边界框的外面
                // 如果在Surface的外面并且行数超出了Surface可以显示的最多行数，那么根据鼠标方向进行滚动，每次滚动一行
                VTScrollData scrollData = ScrollIfCursorOutsideDocument(mouseData);

                // 更新当前鼠标的命中信息，保存在endPointer里
                if (!HitTest(mouseData, endPointer))
                {
                    // 命中失败，不更新
                    return;
                }

                // 此时说明文档被滚动了，需要全部渲染。显示滚动后的数据
                if (scrollData != null)
                {
                    this.RequestInvalidate();
                    this.InvokeScrollChanged(scrollData);
                }
                else
                {
                    // 此时只需要渲染选中区域
                    this.Selection.UpdateGeometry();
                    this.Selection.RequestInvalidate();
                }
            }
        }

        private void GraphicsInterface_GIMouseUp(GraphicsInterface graphicsInterface, MouseData mouseData)
        {
            this.MouseUp?.Invoke(this, mouseData);

            if (mouseData.Button != VTMouseButton.LeftButton)
            {
                return;
            }

            this.isMouseDown = false;
            this.selectionState = false;
            this.graphicsInterface.GIRleaseMouseCapture();
        }

        private void GraphicsInterface_GIMouseDown(GraphicsInterface graphicsInterface, MouseData mouseData)
        {
            this.MouseDown?.Invoke(this, mouseData);

            if (mouseData.Button != VTMouseButton.LeftButton)
            {
                return;
            }

            if (mouseData.ClickCount == 1)
            {
                if (!Selection.IsEmpty)
                {
                    // 点击的时候先清除选中区域
                    Selection.Clear();
                    Selection.RequestInvalidate();
                    selectionState = false;
                }

                this.isMouseDown = true;
            }
            else
            {
                // 双击就是选中单词
                // 三击就是选中整行内容

                int startIndex = 0, count = 0, logicalRow = 0;

                VTextLine textLine = HitTestHelper.HitTestVTextLine(this, mouseData.Y, out logicalRow);
                if (textLine == null)
                {
                    return;
                }

                switch (mouseData.ClickCount)
                {
                    case 2:
                        {
                            // 选中单词
                            int characterIndex;
                            int columnIndex;
                            VTextRange characterRange;
                            HitTestHelper.HitTestVTCharacter(textLine, mouseData.X, out characterIndex, out characterRange, out columnIndex);
                            if (characterIndex == -1)
                            {
                                return;
                            }
                            VTUtils.GetSegement(textLine.Characters, characterIndex, out startIndex, out count);
                            this.Selection.SelectRange(textLine, logicalRow, startIndex, count);
                            break;
                        }

                    case 3:
                        {
                            // 选中一整行
                            this.Selection.SelectRow(textLine, logicalRow);
                            break;
                        }

                    default:
                        {
                            return;
                        }
                }
            }
        }

        private void GraphicsInterface_GIMouseWheel(GraphicsInterface graphicsInterface, MouseWheelData wheelData)
        {
            int oldScroll = Scrollbar.Value;
            int scrollMax = Scrollbar.Maximum;
            int newScroll = 0; // 最终要滚动到的值

            if (wheelData.Delta > 0)
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
                    newScroll = Scrollbar.Minimum;
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
            this.RequestInvalidate();

            this.InvokeScrollChanged(scrollData);
        }

        private void GraphicsInterface_GIScrollChanged(GraphicsInterface arg1, ScrollChangedData scrollChangedData)
        {
            VTScrollData scrollData = this.ScrollTo(scrollChangedData.NewScroll);
            if (scrollData == null)
            {
                return;
            }

            this.RequestInvalidate();

            this.InvokeScrollChanged(scrollData);
        }

        private void GraphicsInterface_GILoaded(GraphicsInterface obj)
        {
            // 把光标加入到定时器里进行闪烁
            VTCursorTimer.Context.SetCursor(this.cursor);
        }

        #endregion
    }
}

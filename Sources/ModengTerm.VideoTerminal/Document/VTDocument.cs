using DotNEToolkit.Utility;
using ModengTerm.Terminal;
using ModengTerm.Terminal.Document;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTerminal.Document.Rendering;
using XTerminal.Parser;
using static XTerminal.Document.VTextLine;

namespace XTerminal.Document
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

        /// <summary>
        /// 初始化文档的参数
        /// </summary>
        internal VTDocumentOptions options;

        /// <summary>
        /// 当前终端屏幕的总行数
        /// </summary>
        private int rowSize;

        /// <summary>
        /// 当前终端屏幕的总列数
        /// </summary>
        private int colSize;

        /// <summary>
        /// 保存光标状态
        /// </summary>
        private VTCursorState cursorState;

        /// <summary>
        /// 光标所在的物理行数
        /// </summary>
        private int activePhysicsRow;

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
        public int ActivePhysicsRow { get { return this.activePhysicsRow; } }

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
        /// 渲染该文档的Surface
        /// </summary>
        public IDrawingDocument Drawing { get; private set; }

        /// <summary>
        /// 是否需要重新布局
        /// </summary>
        public bool IsArrangeDirty { get; private set; }

        /// <summary>
        /// 是否是备用缓冲区
        /// </summary>
        public bool IsAlternate { get; private set; }

        /// <summary>
        /// 当前应用的文本属性
        /// </summary>
        public List<VTextAttributeState> AttributeStates { get; private set; }

        public VTCursorState CursorState { get { return this.cursorState; } }

        /// <summary>
        /// 记录该文档在界面上的大小
        /// </summary>
        public VTRect Rect { get; set; }

        #endregion

        #region 构造方法

        public VTDocument(VTDocumentOptions options, IDrawingDocument canvas, bool isAlternate)
        {
            this.options = options;
            this.Drawing = canvas;
            this.IsAlternate = isAlternate;
            this.cursorState = new VTCursorState();

            this.rowSize = this.options.RowSize;
            this.colSize = this.options.ColumnSize;

            this.AttributeStates = VTUtils.CreateTextAttributeStates();

            this.Cursor = new VTCursor()
            {
                Blinking = true,
                Color = options.CursorColor,
                OffsetX = 0,
                OffsetY = 0,
                Row = 0,
                Column = 0,
                Style = options.CursorStyle,
                BlinkSpeed = options.CursorSpeed,
                BlinkRemain = (int)options.CursorSpeed,
                Size = options.CursorSize
            };
            this.Cursor.DrawingObject = this.Drawing.CreateDrawingObject(this.Cursor);

            #region 初始化第一行，并设置链表首尾指针

            VTextLine firstLine = this.CreateTextLine(0);
            this.FirstLine = firstLine;
            this.LastLine = firstLine;
            this.ActiveLine = firstLine;

            #endregion

            // 默认创建80行，可见区域也是80行
            for (int i = 1; i < options.RowSize; i++)
            {
                VTextLine textLine = this.CreateTextLine(i);
                this.LastLine.Append(textLine);
            }

            // 更新可视区域
            this.SetArrangeDirty(true);
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
                Style = new VTextStyle()
                {
                    FontSize = this.options.FontSize,
                    FontFamily = this.options.FontFamily,
                    ColorTable = this.options.ColorTable,
                    Foreground = this.options.ForegroundColor,
                    Background = this.options.BackgroundColor,
                },
            };

            textLine.DrawingObject = this.Drawing.CreateDrawingObject(textLine);

            return textLine;
        }

        /// <summary>
        /// 设置是否需要重新布局
        /// </summary>
        /// <param name="isDirty"></param>
        public void SetArrangeDirty(bool isDirty)
        {
            if (this.IsArrangeDirty != isDirty)
            {
                this.IsArrangeDirty = isDirty;
            }
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

        #endregion

        #region 公开接口

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
                this.ActiveLine.EraseAll();

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

                this.activePhysicsRow = this.ActiveLine.PhysicsRow;

                this.SetArrangeDirty(true);
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

                this.activePhysicsRow = this.ActiveLine.PhysicsRow;

                this.SetArrangeDirty(true);
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
        /// <param name="column">要删除的起始列，该列也需要删除</param>
        /// <param name="eraseType">删除类型</param>
        public void EraseLine(VTextLine textLine, int column, EraseType eraseType)
        {
            switch (eraseType)
            {
                case EraseType.ToEnd:
                    {
                        // 删除从当前光标处到该行结尾的所有字符
                        textLine.EraseAll(column);
                        break;
                    }

                case EraseType.FromBeginning:
                    {
                        // 删除从行首到当前光标处的内容
                        textLine.EraseRange(0, column);
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
                        textLine.EraseAll(column);

                        // 再删剩下的行
                        VTextLine next = textLine.NextLine;
                        while (next != null)
                        {
                            next.EraseAll(0);

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

                        textLine.EraseAll(0);

                        VTextLine next = this.FirstLine;
                        while (next != null && next != textLine)
                        {
                            next.EraseAll();

                            next = next.NextLine;
                        }

                        break;
                    }

                case EraseType.All:
                    {
                        // 相关命令：clear
                        // 删除显示的全部字符，所有行都被删除，changed to single-width，光标不移动
                        VTextLine next = this.FirstLine;
                        while (next != null)
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
            textLine.EraseRange(column, count);
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

            this.IsArrangeDirty = true;
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

            this.SetArrangeDirty(true);
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

            this.SetArrangeDirty(true);
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
                int characterIndex = this.ActiveLine.FindCharacterIndex(column);

                // 创建新的字符
                VTCharacter character = VTCharacter.CreateNull();

                // 插入到光标处
                this.ActiveLine.InsertCharacter(characterIndex, character);

                // 如果溢出了列宽，那么删除溢出的字符
                this.ActiveLine.TrimEnd(this.colSize);
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
            if (this.Cursor.Column != column)
            {
                this.Cursor.Column = column;
            }

            if (this.Cursor.Row != row)
            {
                this.Cursor.Row = row;

                this.ActiveLine = this.FirstLine.FindNext(row);

                this.activePhysicsRow = this.ActiveLine.PhysicsRow;
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            this.FirstLine = null;
            this.LastLine = null;
            this.ActiveLine = null;
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
        public void Resize(int rowSize, int colSize, int scrollMin, int scrollMax)
        {
            if (this.rowSize != rowSize)
            {
                int rows = Math.Abs(this.rowSize - rowSize);

                // 扩大或者缩小行数之后，第一行应该显示的物理行数
                int firstRow = 0;
                int activeRow = this.ActiveLine.PhysicsRow;

                if (this.rowSize < rowSize)
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
                        this.Drawing.DeleteDrawingObject(this.LastLine.DrawingObject);
                        this.LastLine.Remove();
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

                this.SetArrangeDirty(true);

                this.rowSize = rowSize;
            }

            if (this.colSize != colSize)
            {
                this.colSize = colSize;
            }
        }

        /// <summary>
        /// 设置当前要应用的文本装饰
        /// </summary>
        /// <param name="attribute">要应用的装饰类型</param>
        /// <param name="unset">是否应用该装饰</param>
        /// <param name="parameter">该状态对应的参数</param>
        public void SetAttribute(VTextAttributes attribute, bool enabled, object parameter)
        {
            VTextAttributeState attributeState = this.AttributeStates[(int)attribute];
            attributeState.Enabled = enabled;
            attributeState.Parameter = parameter;
        }

        /// <summary>
        /// 清空当前应用的文本装饰
        /// </summary>
        public void ClearAttribute()
        {
            foreach (VTextAttributeState attributeState in this.AttributeStates)
            {
                attributeState.Parameter = null;
                attributeState.Enabled = false;
            }
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

        #endregion
    }
}

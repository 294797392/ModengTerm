using DotNEToolkit.Utility;
using ModengTerm.Terminal;
using ModengTerm.Terminal.Document;
using System;
using System.Collections.Generic;
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

        private int lineId;

        private List<VTextDecorationState> decorationStates;

        private VTextLine activeLine;

        #endregion

        #region 属性

        /// <summary>
        /// 文档的名字，方便调试
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 记录文档中光标的位置
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

                    // 此时必须保证Column是最新的，也就是说在设置光标的时候，需要先设置Column，再设置Row
                    // 因为这个时候要检测是否有文本特效，给行增加文本特效

                    foreach (VTextDecorationState vds in this.decorationStates)
                    {
                        if (!vds.AlreadySet)
                        {
                            continue;
                        }

                        this.SetTextDecoration(vds.Decoration, vds.Parameter, vds.Unset);
                    }
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
        public bool IsEmpty { get { return this.FirstLine == null && this.LastLine == null; } }

        /// <summary>
        /// 渲染该文档的Surface
        /// </summary>
        public IDrawingCanvas Canvas { get; private set; }

        /// <summary>
        /// 是否需要重新布局
        /// </summary>
        public bool IsArrangeDirty { get; private set; }

        /// <summary>
        /// 是否是备用缓冲区
        /// </summary>
        public bool IsAlternate { get; private set; }

        #endregion

        #region 构造方法

        public VTDocument(VTDocumentOptions options, IDrawingCanvas canvas, bool isAlternate)
        {
            this.options = options;
            this.Canvas = canvas;
            this.IsAlternate = isAlternate;

            this.rowSize = this.options.RowSize;
            this.colSize = this.options.ColumnSize;

            this.decorationStates = new List<VTextDecorationState>();
            IEnumerable<VTextDecorations> textDecorations = Enum.GetValues(typeof(VTextDecorations)).Cast<VTextDecorations>();
            foreach (VTextDecorations decorations in textDecorations)
            {
                VTextDecorationState decorationState = new VTextDecorationState(decorations);
                this.decorationStates.Add(decorationState);
            }

            this.Cursor = new VTCursor()
            {
                Blinking = true,
                Color = VTColor.DarkBlack,
                OffsetX = 0,
                OffsetY = 0,
                Row = 0,
                Column = 0,
                Style = options.CursorStyle,
                BlinkSpeed = options.BlinkSpeed,
                BlinkRemain = (int)options.BlinkSpeed
            };
            this.Cursor.DrawingContext = this.Canvas.CreateDrawingObject(this.Cursor);

            #region 初始化第一行，并设置链表首尾指针

            VTextLine firstLine = this.CreateLine(0);
            this.FirstLine = firstLine;
            this.LastLine = firstLine;
            this.ActiveLine = firstLine;

            #endregion

            // 默认创建80行，可见区域也是80行
            for (int i = 1; i < options.RowSize; i++)
            {
                VTextLine textLine = this.CreateLine(i);
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
        private VTextLine CreateLine(int physicsRow)
        {
            VTextLine textLine = new VTextLine(this)
            {
                ID = this.lineId++,
                OffsetX = 0,
                OffsetY = 0,
                DECPrivateAutoWrapMode = this.DECPrivateAutoWrapMode,
                PhysicsRow = physicsRow,
                Style = new VTextStyle()
                {
                    FontSize = this.options.FontSize,
                    FontFamily = this.options.FontFamily,
                    Foreground = this.options.Foreground
                },
            };

            textLine.DrawingContext = this.Canvas.CreateDrawingObject(textLine);

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
        /// 把当前Document的可显示区域打印到日志里，方便调试
        /// </summary>
        public void Print()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine();

            VTextLine next = this.FirstLine;
            while (next != null)
            {
                builder.AppendLine(next.Text);

                if (next == this.LastLine)
                {
                    break;
                }

                next = next.NextLine;
            }

            logger.FatalFormat(builder.ToString());
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

            // 光标所在行是可滚动区域的最后一行
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
                this.ActiveLine.DeleteAll();

                this.SetArrangeDirty(true);
            }
            else
            {
                // 光标不在可滚动区域的最后一行，说明可以直接移动光标
                logger.DebugFormat("LineFeed，光标在滚动区域内，直接移动光标到下一行");
                this.SetCursor(this.Cursor.Row + 1, this.Cursor.Column);
                this.ActiveLine.SetRenderDirty(true);
            }

            // 清空新行的文本特效，不然可能会遗留之前的文本特效
            VTextDecoration.Recycle(this.ActiveLine.Decorations);
            this.ActiveLine.Decorations.Clear();
        }

        /// <summary>
        /// 反向换行
        /// </summary>
        public void ReverseLineFeed()
        {
            // 可滚动区域的第一行和最后一行
            VTextLine head = this.FirstLine.FindNext(this.ScrollMarginTop);
            VTextLine last = this.LastLine.FindPrevious(this.ScrollMarginBottom);

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
                this.ActiveLine.DeleteAll();

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

            // 清空新行的文本特效，不然可能会遗留之前的文本特效
            VTextDecoration.Recycle(this.ActiveLine.Decorations);
            this.ActiveLine.Decorations.Clear();
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
                        textLine.DeleteText(column);
                        break;
                    }

                case EraseType.FromBeginning:
                    {
                        // 删除从行首到当前光标处的内容
                        textLine.DeleteText(0, column);
                        break;
                    }

                case EraseType.All:
                    {
                        // 删除光标所在整行
                        textLine.DeleteAll();
                        break;
                    }

                default:
                    throw new NotImplementedException();
            }

        }

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
                        textLine.Erase(column);

                        // 再删剩下的行
                        VTextLine next = textLine.NextLine;
                        while (next != null)
                        {
                            next.Erase(0);

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

                        textLine.Erase(0);

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

        /// <summary>
        /// 从指定行的指定列处开始删除字符，要删除的字符数由count指定
        /// </summary>
        /// <param name="textLine">要删除的字符所在行</param>
        /// <param name="column">要删除的字符起始列</param>
        /// <param name="count">要删除的字符个数</param>
        public void DeleteCharacter(VTextLine textLine, int column, int count)
        {
            textLine.DeleteText(column, count);
        }

        /// <summary>
        /// 清除数据
        /// 1. 清除所有文本数据
        /// 2. 清除所有的文本装饰信息
        /// </summary>
        public void DeleteAll()
        {
            #region 删除所有文本数据

            VTextLine current = this.FirstLine;
            VTextLine last = this.LastLine;

            while (current != null)
            {
                current.DeleteAll();

                current = current.NextLine;
            }

            #endregion

            #region 重置文本装饰状态

            foreach (VTextDecorationState decorationState in this.decorationStates)
            {
                decorationState.AlreadySet = false;
                decorationState.Parameter = null;
            }

            #endregion

            this.IsArrangeDirty = true;
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
                node2.DeleteAll();  // 把新行清空

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
                node1.DeleteAll();

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
        /// 注意必须先设置Column，再设置Row。因为在ActiveLine改变的时候，会设置文本装饰，必须保证列是最新的
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
                        VTextLine firstLine = this.FirstLine;
                        this.FirstLine.PreviousLine = textLine;
                        this.FirstLine = textLine;
                        textLine.NextLine = firstLine;
                        break;
                    }

                case MoveOptions.MoveToLast:
                    {
                        textLine.Remove();
                        VTextLine lastLine = this.LastLine;
                        this.LastLine.NextLine = textLine;
                        this.LastLine = textLine;
                        textLine.PreviousLine = lastLine;
                        break;
                    }

                default:
                    throw new NotImplementedException();
            }

            this.ActiveLine = this.FirstLine.FindNext(this.Cursor.Row);
        }

        /// <summary>
        /// 重置终端大小
        /// 需要考虑scrollMargin
        /// </summary>
        /// <param name="rowSize">终端的新的行数</param>
        /// <param name="colSize">终端的新的列数</param>
        public void Resize(int rowSize, int colSize)
        {
            if (this.rowSize != rowSize)
            {
                int rows = Math.Abs(this.rowSize - rowSize);

                if (this.rowSize < rowSize)
                {
                    // 扩大了行数

                    // 找到滚动区域内的最后一行，在该行后添加新行
                    // 此时ActiveLine不变
                    for (int i = 0; i < rows; i++)
                    {
                        VTextLine textLine = this.CreateLine(this.LastLine.PhysicsRow + 1);
                        this.LastLine.Append(textLine);
                    }
                }
                else
                {
                    // 减少了行数

                    if (this.ActiveLine == this.LastLine)
                    {
                        // 此时说明光标在最后一行，那么就从第一行开始删除
                        // 此时ActiveLine不变
                        for (int i = 0; i < rows; i++)
                        {
                            this.Canvas.DeleteDrawingObject(this.FirstLine.DrawingContext);
                            this.FirstLine.Remove();
                        }

                        // 注意当光标在最后一行的时候，Cursor.Row是会减少的，这里更新一下Cursor.Row
                        // 虽然Row变了，但是ActiveLine没变
                        this.SetCursor(this.Cursor.Row - rows, this.Cursor.Column);
                    }
                    else
                    {
                        // 光标不在最后一行，那么从最后一行开始删除
                        // ActiveLine貌似也不变
                        for (int i = 0; i < rows; i++)
                        {
                            this.Canvas.DeleteDrawingObject(this.LastLine.DrawingContext);
                            this.LastLine.Remove();
                        }
                    }
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
        /// 设置当前行的TextDecoration
        /// </summary>
        /// <param name="decorations"></param>
        /// <param name="parameter"></param>
        /// <param name="unset"></param>
        public void SetTextDecoration(VTextDecorations decorations, object parameter, bool unset)
        {
            #region 先为当前行设置TextDecoration

            if (unset)
            {
                // 如果是unset，那么就找到当前行第一个没有关闭的Decoration并关闭
                // 有可能会连续多次设置Foreground或Background，然后再设置一次unset，所以这里有可能会有多个未关闭的TextDecoration
                List<VTextDecoration> decorationList = this.ActiveLine.Decorations.Where(v => v.Decoration == decorations).ToList();

                foreach (VTextDecoration decoration in decorationList)
                {
                    if (decoration.StartColumn == this.Cursor.Column)
                    {
                        // 在同一列Unset，那么说明这个单元格不需要这个Decoration了，回收这个Decoration

                        VTextDecoration.Recycle(decoration);

                        this.ActiveLine.Decorations.Remove(decoration);
                    }
                    else
                    {
                        if (!decoration.Closed)
                        {
                            // 不是在同一列unset，那么清除文本装饰
                            decoration.Closed = true;
                            decoration.EndColumn = this.Cursor.Column == 0 ? 0 : this.Cursor.Column - 1;
                        }
                    }
                }
            }
            else
            {
                // 最坏情况下，每一个单元格都包含一组TextDecoration
                // 找到当前光标所在单元格所拥有的文本装饰

                VTextDecoration decoration = this.ActiveLine.Decorations.FirstOrDefault(v => v.StartColumn == this.Cursor.Column && v.Decoration == decorations);

                if (decoration == null)
                {
                    decoration = VTextDecoration.Create();
                    decoration.Decoration = decorations;
                    decoration.StartColumn = this.Cursor.Column;
                    this.ActiveLine.Decorations.Add(decoration);
                }

                decoration.Parameter = parameter;
            }

            #endregion

            #region 再把当前的TextDecoration缓存下来

            // 缓存下来，后面再ActiveLine Changed的时候使用
            VTextDecorationState decorationState = this.decorationStates[(int)decorations];
            if (!decorationState.AlreadySet)
            {
                decorationState.AlreadySet = true;
            }
            decorationState.Parameter = parameter;
            decorationState.Unset = unset;

            #endregion
        }

        #endregion
    }
}

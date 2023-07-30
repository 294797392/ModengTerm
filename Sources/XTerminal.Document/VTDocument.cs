using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTerminal.Document.Rendering;
using XTerminal.Parser;

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
        /// 可视区域的最大列数
        /// </summary>
        public int ColumnSize { get { return this.options.ColumnSize; } }

        /// <summary>
        /// 可视区域的最大行数
        /// </summary>
        public int RowSize { get { return this.options.RowSize; } }

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
        public ITerminalSurface Surface { get; private set; }

        /// <summary>
        /// 是否需要重新布局
        /// </summary>
        public bool IsArrangeDirty { get; private set; }

        #endregion

        #region 构造方法

        public VTDocument(VTDocumentOptions options)
        {
            this.options = options;
            this.Surface = options.CanvasCreator.CreateSurface();

            this.Cursor = new VTCursor()
            {
                Blinking = true,
                Color = VTColors.DarkBlack,
                OffsetX = 0,
                OffsetY = 0,
                Row = 0,
                Column = 0,
                Style = options.CursorStyle,
                Interval = options.Interval
            };

            #region 初始化第一行，并设置链表首尾指针

            VTextLine firstLine = new VTextLine(this)
            {
                LogicalRow = 0,
                OffsetX = 0,
                OffsetY = 0,
                DECPrivateAutoWrapMode = options.DECPrivateAutoWrapMode,
            };
            this.FirstLine = firstLine;
            this.LastLine = firstLine;
            this.ActiveLine = firstLine;

            #endregion

            // 默认创建80行，可见区域也是80行
            for (int i = 1; i < options.RowSize; i++)
            {
                this.CreateNextLine(i);
            }

            // 更新可视区域
            this.SetArrangeDirty(true);
        }

        #endregion

        #region 实例方法

        /// <summary>
        /// 创建一个新行并将新行挂到链表的最后一个节点后面
        /// </summary>
        /// <returns></returns>
        private void CreateNextLine(int row)
        {
            VTextLine textLine = new VTextLine(this)
            {
                LogicalRow = row,
                OffsetX = 0,
                OffsetY = 0,
                DECPrivateAutoWrapMode = this.DECPrivateAutoWrapMode,
            };

            this.LastLine.NextLine = textLine;
            textLine.PreviousLine = this.LastLine;
            this.LastLine = textLine;
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
        }

        /// <summary>
        /// 在指定的光标位置打印一个字符
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
        /// 从当前光标处开始删除字符，要删除的字符数由count指定
        /// </summary>
        /// <param name="count"></param>
        public void DeleteCharacter(VTextLine textLine, int column, int count)
        {
            textLine.DeleteText(column, count);
        }

        /// <summary>
        /// 删除所有行
        /// </summary>
        public void DeleteAll()
        {
            VTextLine current = this.FirstLine;
            VTextLine last = this.LastLine;

            while (current != null)
            {
                // 取消关联关系
                current.DeleteAll();

                current = current.NextLine;
            }

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
        /// </summary>
        /// <param name="row">要设置的行</param>
        /// <param name="column">要设置的列</param>
        public void SetCursor(int row, int column)
        {
            if (this.Cursor.Row != row)
            {
                this.Cursor.Row = row;

                this.ActiveLine = this.FirstLine.FindNext(row);
            }

            if (this.Cursor.Column != column)
            {
                this.Cursor.Column = column;

                // 在VIM模式下，如果在一行的最后输入空格，那么收到CUP指令，CUP的位置是把光标位置向后移动一列
                // 此时如果后面没有字符，那么在渲染的时候，光标的位置并不会变，所以这里先判断要移动到的光标处是否有字符
                // 如果有则直接移动，如果没有则添加一个空的字符
                this.ActiveLine.PadColumns(column + 1);
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

        #endregion
    }
}

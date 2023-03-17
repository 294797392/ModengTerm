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
    public class VTDocument : VTDocumentBase
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("VTDocument");

        #endregion

        #region 实例变量

        //internal Dictionary<int, VTextLine> lineMap;

        private VTDocumentOptions options;

        ///// <summary>
        ///// 光标所在行
        ///// </summary>
        //private VTextLine activeLine;

        private int row;

        #endregion

        #region 属性

        /// <summary>
        /// 上边距的文档
        /// </summary>
        public VTMarginedDocument TopMarginArea { get; private set; }

        /// <summary>
        /// 下边距的文档
        /// </summary>
        public VTMarginedDocument BottomMarginArea { get; private set; }

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
        public int Columns { get { return this.options.Columns; } }

        /// <summary>
        /// 可视区域的最大行数
        /// </summary>
        public int Rows { get { return this.options.Rows; } }

        /// <summary>
        /// 总行数
        /// </summary>
        public int TotalRows { get; private set; }

        /// <summary>
        /// 该文档要渲染的区域
        /// </summary>
        public ViewableDocument ViewableArea { get; private set; }

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

        #endregion

        #region 构造方法

        public VTDocument(VTDocumentOptions options)
        {
            this.options = options;

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

            this.ViewableArea = new ViewableDocument(this);
            this.TopMarginArea = new VTMarginedDocument(this);
            this.BottomMarginArea = new VTMarginedDocument(this);

            VTextLine firstLine = new VTextLine(this)
            {
                OffsetX = 0,
                OffsetY = 0,
                CursorAtRightMargin = false,
                DECPrivateAutoWrapMode = options.DECPrivateAutoWrapMode,
            };
            this.FirstLine = firstLine;
            this.LastLine = firstLine;
            this.ActiveLine = firstLine;

            // 默认创建80行，可见区域也是80行
            for (int i = 1; i < options.Rows; i++)
            {
                this.CreateNextLine();
            }

            this.TotalRows = options.Rows;

            // 更新可视区域
            this.ViewableArea.FirstLine = firstLine;
            this.ViewableArea.LastLine = this.LastLine;
            this.SetArrangeDirty();
        }

        #endregion

        #region 实例方法

        private void SetArrangeDirty()
        {
            if (!this.IsArrangeDirty)
            {
                this.IsArrangeDirty = true;
            }
        }

        /// <summary>
        /// 把当前Document的可显示区域打印到日志里，方便调试
        /// </summary>
        public void Print()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine();

            VTextLine next = this.ViewableArea.FirstLine;
            while (next != null)
            {
                builder.AppendLine(next.GetText());

                if (next == this.ViewableArea.LastLine)
                {
                    break;
                }

                next = next.NextLine;
            }

            logger.FatalFormat(builder.ToString());
        }

        private void ProcessMargin(VTMarginedDocument marginDocument, int newMargin, int oldMargin)
        {
            if (marginDocument.IsEmpty)
            {
                // 空文档，那么直接创建
                marginDocument.InitializeLines(newMargin);

                // 更新可视区域
                this.ViewableArea.Shrink(newMargin);
            }
            else
            {
                int delta = Math.Abs(newMargin - oldMargin);

                if (newMargin < oldMargin)
                {
                    // 更新上边距区域
                    // 新的Margin比之前的Margin少，说明要移除行
                    marginDocument.Remove(delta);

                    // 更新可视区域
                    this.ViewableArea.Expand(delta);
                }
                else
                {
                    // 更新上边距区域
                    // 新的Margin比之前的Margin多，说明要增加行
                    marginDocument.Add(delta);

                    // 更新可视区域
                    this.ViewableArea.Shrink(delta);
                }
            }

        }

        /// <summary>
        /// 解除文档里所有附加的Drawable
        /// </summary>
        /// <param name="document">要解除的文档</param>
        private void DetachAllDrawable(VTDocumentBase document)
        {
            VTextLine current = document.FirstLine;
            VTextLine last = document.LastLine;

            while (current != null)
            {
                // 取消关联关系
                current.DetachDrawable();

                if (current == last)
                {
                    break;
                }

                current = current.NextLine;
            }
        }

        /// <summary>
        /// 把drawables附加到某个文档里
        /// </summary>
        /// <param name="document">要附加的文档</param>
        /// <param name="drawables">要附加到文档的drawable集合</param>
        /// <param name="startIndex">drawables的偏移量</param>
        /// <returns>附加之后的索引</returns>
        private int AttachAllDrawable(VTDocumentBase document, List<IDocumentDrawable> drawables, int startIndex)
        {
            int index = startIndex;

            VTextLine current = document.FirstLine;
            VTextLine last = document.LastLine;

            while (current != null)
            {
                IDocumentDrawable drawable = drawables[index++];

                // 取消关联关系
                current.AttachDrawable(drawable);

                if (current == last)
                {
                    break;
                }

                current = current.NextLine;
            }

            return index;
        }

        #endregion

        #region 公开接口

        /// <summary>
        /// 换行
        /// </summary>
        public void LineFeed()
        {
            ViewableDocument document = this.ViewableArea;
            VTextLine oldFirstRow = document.FirstLine;
            VTextLine oldLastRow = document.LastLine;

            if (!this.HasNextLine(this.ActiveLine))
            {
                this.CreateNextLine();
            }

            if (oldLastRow == this.ActiveLine)
            {
                // 光标在可视区域的最后一行，那么要把可视区域向下移动
                logger.DebugFormat("LineFeed，光标在可视区域最后一行，向下移动一行并且可视区域往下移动一行");
                document.ScrollDocument(ScrollOrientation.Down, 1);

                // 更新文档模型和渲染模型的关联信息
                // 把oldFirstRow的渲染模型拿给newLastRow使用
                VTextLine newLastRow = document.LastLine;
                newLastRow.AttachDrawable(oldFirstRow.Drawable);
                this.ActiveLine = this.ActiveLine.NextLine;
            }
            else
            {
                // 这里假设光标在可视区域里
                // 实际上光标有可能在可视区域的上面或者下面，但是暂时还没找到方法去判定

                // 光标在可视区域里
                logger.DebugFormat("LineFeed，光标在可视区域里，直接移动光标到下一行");
                this.SetCursor(this.Cursor.Row + 1, this.Cursor.Column);
            }
        }

        /// <summary>
        /// 反向换行
        /// </summary>
        public void ReverseLineFeed()
        {
            ViewableDocument document = this.ViewableArea;
            VTextLine oldFirstRow = document.FirstLine;
            VTextLine oldLastRow = document.LastLine;

            if (oldFirstRow == this.ActiveLine)
            {
                // 此时光标位置在可视区域的第一行
                logger.DebugFormat("RI_ReverseLineFeed，光标在可视区域第一行，向上移动一行并且可视区域往上移动一行");
                VTextLine newFirstRow = document.ScrollDocument(ScrollOrientation.Up, 1);

                // 把oldLastRow的渲染模型拿给newFirstRow使用
                newFirstRow.AttachDrawable(oldLastRow.Drawable);
                this.ActiveLine = this.ActiveLine.PreviousLine;
            }
            else
            {
                // 这里假设光标在可视区域里面
                // 实际上有可能光标在可视区域上的上面或者下面，但是目前还没找到判断方式

                // 光标位置在可视区域里面
                logger.DebugFormat("RI_ReverseLineFeed，光标在可视区域里，直接移动光标到上一行");
                this.SetCursor(this.Cursor.Row - 1, this.Cursor.Column);
            }
        }

        /// <summary>
        /// 创建一个新行并将新行挂到链表的最后一个节点后面
        /// </summary>
        /// <returns></returns>
        public void CreateNextLine()
        {
            VTextLine textLine = new VTextLine(this)
            {
                ID = row++,
                OffsetX = 0,
                OffsetY = 0,
                CursorAtRightMargin = false,
                DECPrivateAutoWrapMode = this.DECPrivateAutoWrapMode,
            };

            this.LastLine.NextLine = textLine;
            textLine.PreviousLine = this.LastLine;
            this.LastLine = textLine;

            this.TotalRows++;
        }

        public bool HasNextLine(VTextLine textLine)
        {
            return textLine.NextLine != null;
        }

        /// <summary>
        /// 在指定的光标位置打印一个字符
        /// </summary>
        /// <param name="ch"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        public void PrintCharacter(VTextLine textLine, char ch, int col)
        {
            textLine.PrintCharacter(ch, col);
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
                        textLine.Replace(column, ' ');

                        // 这里有可能会出现性能问题，因为删除的不是可视区域的行，而是整个VTDocument的行！

                        VTextLine next = textLine.NextLine;
                        while (next != null)
                        {
                            next.ReplaceAll(' ');

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

                        textLine.Replace(0, column, ' ');

                        VTextLine next = this.FirstLine;
                        while (next != null && next != textLine)
                        {
                            next.Replace(0, ' ');

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
                            next.ReplaceAll(' ');

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
        /// 使用当前Document里的Row重置终端的行数
        /// </summary>
        /// <param name="rows"></param>
        public void ResetRows()
        {
            // 重置Viewable的状态
            this.FirstLine = this.ViewableArea.FirstLine;
            this.LastLine = this.ViewableArea.LastLine;

            this.FirstLine.PreviousLine = null;
            this.LastLine.NextLine = null;

            this.SetArrangeDirty();
        }

        /// <summary>
        /// 使用新的行数和列数来调整终端的大小
        /// 主要是需要调整ViewableDocument的大小
        /// </summary>
        /// <param name="rows">终端的新行数</param>
        /// <param name="columns">终端的新列数</param>
        public void Resize(int rows, int columns)
        {
            int oldColumns = this.options.Columns;
            int newColumns = columns;
            int oldRows = this.options.Rows;
            int newRows = rows;

            if (newColumns != oldColumns)
            {
                this.options.Columns = newColumns;

                // 先不管列
            }

            if (newRows != oldRows)
            {
                this.options.Rows = newRows;

                VTextLine firstLine = this.FirstLine;
                VTextLine lastLine = this.LastLine;
                VTextLine visibleFirstLine = this.ViewableArea.FirstLine;
                VTextLine visibleLastLine = this.ViewableArea.LastLine;

                if (newRows > oldRows)
                {
                    // 新的行数比旧的行数大
                }
                else
                {
                    // 新的行数比旧的行数小

                    // 可显示区域的最后一行的索引不变
                    // 改变可视区域的第一行的索引
                    //visibleFirstLine.Row + (oldRows - newRows);
                }

                #region Resize可视区域



                #endregion
            }
        }

        /// <summary>
        /// 在可视区域的指定的行位置插入多个新行，并把指定的行和指定行后面的所有行往后移动
        /// </summary>
        /// <param name="activeLine">光标所在行</param>
        /// <param name="lines">要插入的行数</param>
        public void InsertLines(VTextLine activeLine, int lines)
        {
            // 可视区域的最后一行
            VTextLine lastVisibleLine = this.ViewableArea.LastLine;

            // 从当前行的上一行开始插入
            VTextLine startLine = activeLine.PreviousLine;

            // 当前行作为插入的最后一行
            VTextLine endLine = activeLine;

            for (int i = 0; i < lines; i++)
            {
                VTextLine newLine = new VTextLine(this)
                {
                    CursorAtRightMargin = false,
                    DECPrivateAutoWrapMode = this.DECPrivateAutoWrapMode
                };

                // 新行关联渲染模型
                newLine.AttachDrawable(lastVisibleLine.Drawable);
                lastVisibleLine.DetachDrawable();

                // 更新链表指针
                startLine.NextLine = newLine;
                newLine.PreviousLine = startLine;
                newLine.NextLine = endLine;
                endLine.PreviousLine = newLine;

                lastVisibleLine = lastVisibleLine.PreviousLine;

                // 更新可视区域最后一行的指针
                this.ViewableArea.LastLine = lastVisibleLine;
            }

            // 如果插入的行是可视区域的第一行的话，那么要记得更新可视区域第一行的指针
            if (activeLine == this.ViewableArea.FirstLine)
            {
                this.ViewableArea.FirstLine = startLine.NextLine;
            }

            // 更新ActiveLine
            this.ActiveLine = this.ViewableArea.FirstLine.FindNext(this.Cursor.Row);

            this.SetArrangeDirty();
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
                this.ProcessMargin(this.TopMarginArea, marginTop, this.ScrollMarginTop);
                this.ScrollMarginTop = marginTop;
            }

            if (this.ScrollMarginBottom != marginBottom)
            {
                this.ProcessMargin(this.BottomMarginArea, marginBottom, this.ScrollMarginBottom);
                this.ScrollMarginBottom = marginBottom;
            }
        }

        /// <summary>
        /// 把所有的TextLine取消关联渲染模型
        /// </summary>
        public void DetachAll()
        {
            // Detach TopMarginArea
            if (!this.TopMarginArea.IsEmpty)
            {
                this.DetachAllDrawable(this.TopMarginArea);
            }

            // Detach ViewableArea
            this.DetachAllDrawable(this.ViewableArea);

            // Detach BottomMarginArea
            if (!this.BottomMarginArea.IsEmpty)
            {
                this.DetachAllDrawable(this.BottomMarginArea);
            }
        }

        /// <summary>
        /// 按顺序为每个VTextLine附加渲染对象
        /// </summary>
        /// <param name="drawables"></param>
        public void AttachAll(List<IDocumentDrawable> drawables)
        {
            int startIndex = 0;

            // Attach TopMarginArea
            if (!this.TopMarginArea.IsEmpty)
            {
                startIndex = this.AttachAllDrawable(this.TopMarginArea, drawables, startIndex);
            }

            // Attach ViewableArea
            startIndex = this.AttachAllDrawable(this.ViewableArea, drawables, startIndex);

            // Attach BottomMarginArea
            if (!this.BottomMarginArea.IsEmpty)
            {
                startIndex = this.AttachAllDrawable(this.BottomMarginArea, drawables, startIndex);
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

                /*****************
                 *       1  0
                 *       2  1       -> TopMarginArea
                 *       3  2
                 *       ----
                 *       4  3
                 *       5  4       -> ViewableArea
                 *       6  5
                 *       7  6
                 *       ----
                 *       8  7
                 *       9  8       -> BottomMarginArea
                 *      10  9
                 *      11 10
                 ******************/

                int topMarginRows = this.ScrollMarginTop;
                int topMarginStart = 0;
                int topMarginEnd = topMarginStart + this.ScrollMarginTop - 1;

                int bottomMarginRows = this.ScrollMarginBottom;
                int bottomMarginStart = this.Rows - bottomMarginRows;
                int bottomMarginEnd = this.Rows - 1;

                int viewableStart = topMarginEnd + 1;
                int viewableEnd = bottomMarginStart - 1;

                if (topMarginRows > 0 && row >= topMarginStart && row <= topMarginEnd)
                {
                    // 光标在上边距文档内
                    this.ActiveLine = this.TopMarginArea.FirstLine.FindNext(row);
                }
                else if (bottomMarginRows > 0 && row >= bottomMarginStart && row <= bottomMarginEnd)
                {
                    // 光标在下边距文档内
                    this.ActiveLine = this.BottomMarginArea.LastLine.FindPrevious(bottomMarginEnd - row);
                }
                else
                {
                    // 光标在可视区域内
                    this.ActiveLine = this.ViewableArea.FirstLine.FindNext((row - viewableStart));
                }
            }

            if (this.Cursor.Column != column)
            {
                this.Cursor.Column = column;
            }
        }

        #endregion
    }
}

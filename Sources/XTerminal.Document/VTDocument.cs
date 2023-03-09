using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        private Dictionary<int, VTextLine> lineMap;

        private VTDocumentOptions options;

        ///// <summary>
        ///// 光标所在行
        ///// </summary>
        //private VTextLine activeLine;

        #endregion

        #region 属性

        /// <summary>
        /// 文档的名字，方便调试
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 文档中的第一行
        /// </summary>
        public VTextLine FirstLine { get; private set; }

        /// <summary>
        /// 文档中的最后一行
        /// </summary>
        public VTextLine LastLine { get; private set; }

        /// <summary>
        /// 记录文档中光标的位置
        /// </summary>
        public VCursor Cursor { get; private set; }

        public bool DECPrivateAutoWrapMode { get; set; }

        /// <summary>
        /// 可视区域的最大列数
        /// </summary>
        public int Columns { get { return this.options.Columns; } }

        /// <summary>
        /// 可视区域的最大行数
        /// </summary>
        public int Rows { get { return this.options.Rows; } }

        ///// <summary>
        ///// 光标所在行
        ///// </summary>
        //public VTextLine ActiveLine { get { return this.activeLine; } }

        /// <summary>
        /// 该文档要渲染的区域
        /// </summary>
        public ViewableDocument ViewableArea { get; private set; }

        /// <summary>
        /// 该文档是否需要重新布局
        /// </summary>
        public bool IsArrangeDirty { get { return this.ViewableArea.IsArrangeDirty; } set { this.ViewableArea.IsArrangeDirty = value; } }

        #endregion

        #region 构造方法

        public VTDocument(VTDocumentOptions options)
        {
            this.options = options;

            this.lineMap = new Dictionary<int, VTextLine>();
            this.Cursor = new VCursor();
            this.ViewableArea = new ViewableDocument();

            VTextLine firstLine = new VTextLine(options.Columns)
            {
                Row = 0,
                OffsetX = 0,
                OffsetY = 0,
                CursorAtRightMargin = false,
                DECPrivateAutoWrapMode = options.DECPrivateAutoWrapMode,
                OwnerDocument = this,
                DrawingElement = null
            };
            this.lineMap[0] = firstLine;
            this.FirstLine = firstLine;
            this.LastLine = firstLine;

            // 更新可视区域
            this.ViewableArea.FirstLine = firstLine;
            this.ViewableArea.LastLine = firstLine;
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
                builder.AppendLine(next.BuildText());

                if (next == this.ViewableArea.LastLine)
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
        /// 创建下一行，并把ActiveLine设置为新创建的行
        /// </summary>
        /// <returns></returns>
        public VTextLine CreateNextLine()
        {
            int row = this.LastLine.Row + 1;

            VTextLine textLine = new VTextLine(this.Columns)
            {
                Row = row,
                OffsetX = 0,
                OffsetY = this.LastLine.Bounds.LeftBottom.Y,
                CursorAtRightMargin = false,
                DECPrivateAutoWrapMode = this.DECPrivateAutoWrapMode,
                OwnerDocument = this
            };
            this.lineMap[row] = textLine;

            this.LastLine.NextLine = textLine;
            textLine.PreviousLine = this.LastLine;
            this.LastLine = textLine;

            return textLine;
        }

        public bool ContainsLine(int row)
        {
            return this.lineMap.ContainsKey(row);
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

        ///// <summary>
        ///// 设置文档中的光标位置
        ///// 并更新ActiveLine
        ///// </summary>
        ///// <param name="row"></param>
        ///// <param name="column"></param>
        //public void SetCursor(int row, int column)
        //{
        //    if (this.Cursor.Row != row)
        //    {
        //        this.Cursor.Row = row;

        //        // 光标位置改变的时候，就改变activeLine
        //        if (!this.lineMap.TryGetValue(row, out this.activeLine))
        //        {
        //            this.activeLine = null;
        //            logger.ErrorFormat("切换activeLine失败, 没找到对应的行, row = {0}", row);
        //            return;
        //        }
        //    }

        //    if (this.Cursor.Column != column)
        //    {
        //        this.Cursor.Column = column;
        //    }
        //}

        /// <summary>
        /// 在当前光标所在行开始删除字符操作
        /// </summary>
        public void EraseLine(VTextLine textLine, EraseType eraseType)
        {
            switch (eraseType)
            {
                case EraseType.ToEnd:
                    {
                        // 删除从当前光标处到该行结尾的所有字符
                        textLine.DeleteText(this.Cursor.Column);
                        break;
                    }

                case EraseType.FromBeginning:
                    {
                        // 删除从行首到当前光标处的内容
                        textLine.DeleteText(0, this.Cursor.Column);
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

        public void EraseDisplay(VTextLine textLine, EraseType eraseType)
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
                        textLine.DeleteText(this.Cursor.Column);

                        VTextLine next = textLine.NextLine;
                        while (next != null)
                        {
                            next.DeleteAll();

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

                        textLine.DeleteText(0, this.Cursor.Column);

                        VTextLine next = this.FirstLine;
                        while (next != null && next != textLine)
                        {
                            next.DeleteAll();

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
                            next.DeleteAll();

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
        public void DeleteCharacter(VTextLine textLine, int count)
        {
            textLine.DeleteText(this.Cursor.Column, count);
        }

        /// <summary>
        /// 清除所有字符
        /// 只留下第一行的文本行
        /// </summary>
        public void Reset()
        {
            // 先解除和DrawingElement的关联关系
            VTextLine current = this.ViewableArea.FirstLine;
            VTextLine last = this.ViewableArea.LastLine;

            while (current != null)
            {
                if (current.DrawingElement != null)
                {
                    current.DrawingElement.Data = null;
                    current.DrawingElement = null;
                }

                if (current == last)
                {
                    break;
                }

                current = current.NextLine;
            }

            this.FirstLine.DeleteAll();
            this.FirstLine.NextLine = null;
            this.LastLine = this.FirstLine;
            this.lineMap.Clear();
            this.lineMap[0] = this.FirstLine;
            this.ViewableArea.FirstLine = this.FirstLine;
            this.ViewableArea.LastLine = this.FirstLine;
            this.IsArrangeDirty = true;
        }

        /// <summary>
        /// 滚动可视区域
        /// </summary>
        /// <param name="orientation">滚动方向</param>
        /// <param name="scrollRows">要滚动的行数</param>
        public void ScrollViewableDocument(ScrollOrientation orientation, int scrollRows)
        {
            if (this.LastLine.Row + 1 <= this.Rows)
            {
                /*
                 * ViewableArea:
                 * |------------|
                 * |------------|
                 * |------------|
                 * |------------|
                 * |            |
                 * |            |
                 * |____________|
                 */

                // 可视区域的总行数小于最大行数
                // 只需要更新最后一行即可
                if (this.ViewableArea.LastLine != this.LastLine)
                {
                    this.ViewableArea.LastLine = this.LastLine;
                    this.SetArrangeDirty();
                }
            }
            else
            {
                VTextLine oldFirstLine = this.ViewableArea.FirstLine;
                VTextLine oldLastLine = this.ViewableArea.LastLine;
                VTextLine newFirstLine = null;
                VTextLine newLastLine = null;

                #region 更新新的可视区域的第一行和最后一行的指针

                switch (orientation)
                {
                    case ScrollOrientation.Down:
                        {
                            newFirstLine = this.lineMap[oldFirstLine.Row + scrollRows];
                            newLastLine = this.lineMap[oldLastLine.Row + scrollRows];
                            break;
                        }

                    case ScrollOrientation.Up:
                        {
                            newFirstLine = this.lineMap[oldFirstLine.Row - scrollRows];
                            newLastLine = this.lineMap[oldLastLine.Row - scrollRows];
                            break;
                        }

                    default:
                        throw new NotImplementedException();
                }

                this.ViewableArea.FirstLine = newFirstLine;
                this.ViewableArea.LastLine = newLastLine;

                #endregion

                #region 计算从可视区域移出的行和移入的行

                // 从可视区域内被删除的行
                VTextLine removedFirst = null;
                // 新增加到可视区域内的行
                VTextLine addedFirst = null;

                if (scrollRows >= this.Rows)
                {
                    // 此时说明已经移动了一整个屏幕了, 被复用的起始行和结束行就等于移动之前的起始行和结束行
                    removedFirst = oldFirstLine;
                    addedFirst = newFirstLine;
                }
                else
                {
                    if (orientation == ScrollOrientation.Up)
                    {
                        // 把可视区域往上移动
                        removedFirst = newLastLine.NextLine;
                        addedFirst = newFirstLine;
                    }
                    else if (orientation == ScrollOrientation.Down)
                    {
                        // 把可视区域往下移动
                        removedFirst = oldFirstLine;
                        addedFirst = oldLastLine.NextLine;
                    }
                }

                #endregion

                #region 复用移出的行的DrawingElement

                VTextLine removedCurrent = removedFirst;
                VTextLine addedCurrent = addedFirst;
                for (int i = 0; i < scrollRows; i++)
                {
                    if (removedCurrent.DrawingElement != null)
                    {
                        addedCurrent.DrawingElement = removedCurrent.DrawingElement;
                        addedCurrent.DrawingElement.Data = addedCurrent;
                    }
                    addedCurrent.IsCharacterDirty = true;

                    removedCurrent = removedCurrent.NextLine;
                    addedCurrent = addedCurrent.NextLine;
                }

                #endregion

                // 下次渲染的时候排版
                this.SetArrangeDirty();
            }
        }

        public bool TryGetLine(int row, out VTextLine textLine)
        {
            return this.lineMap.TryGetValue(row, out textLine);
        }

        #endregion
    }
}

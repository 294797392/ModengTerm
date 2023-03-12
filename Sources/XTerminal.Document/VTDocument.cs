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

        internal Dictionary<int, VTextLine> lineMap;

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
            this.Cursor = new VTCursor();
            this.ViewableArea = new ViewableDocument(options)
            {
                OwnerDocument = this
            };

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

            // 默认创建80行，可见区域也是80行
            for (int i = 1; i < options.Rows; i++)
            {
                this.CreateNextLine();
            }

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
        /// 创建一个新行并更新Last指针
        /// </summary>
        /// <returns></returns>
        public void CreateNextLine()
        {
            int row = this.LastLine.Row + 1;

            VTextLine textLine = new VTextLine(this.Columns)
            {
                Row = row,
                OffsetX = 0,
                OffsetY = 0,
                CursorAtRightMargin = false,
                DECPrivateAutoWrapMode = this.DECPrivateAutoWrapMode,
                OwnerDocument = this
            };
            this.lineMap[row] = textLine;

            this.LastLine.NextLine = textLine;
            textLine.PreviousLine = this.LastLine;
            this.LastLine = textLine;
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
                        textLine.DeleteText(column);

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

                        textLine.DeleteText(0, column);

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
        public void DeleteCharacter(VTextLine textLine, int column, int count)
        {
            textLine.DeleteText(column, count);
        }

        /// <summary>
        /// 清除当前文档里的所有内容
        /// </summary>
        public void Clear()
        {
            VTextLine current = this.FirstLine;
            VTextLine last = this.LastLine;

            while (current != null)
            {
                if (current.DrawingElement != null)
                {
                    current.DrawingElement.Data = null;
                    current.DrawingElement = null;
                }

                // 重置文本行
                current.DeleteAll();

                if (current == last)
                {
                    break;
                }

                current = current.NextLine;
            }
        }

        public bool TryGetLine(int row, out VTextLine textLine)
        {
            return this.lineMap.TryGetValue(row, out textLine);
        }

        /// <summary>
        /// 使用当前Document里的Row重置终端的行数
        /// 多余的行会删除
        /// 不足的行会补齐
        /// </summary>
        /// <param name="rows"></param>
        public void ResetRows()
        {
            VTextLine lastLine;
            if (!this.lineMap.TryGetValue(this.Rows - 1, out lastLine))
            {
                // 此时说明动态修改了行数，需要补齐
                int count = (this.Rows - 1) - this.LastLine.Row;
                for (int i = 0; i < count; i++)
                {
                    this.CreateNextLine();
                }
            }
            else
            {
                // 有可能行数比最大行数多，需要删除
                for (int i = this.LastLine.Row; i > lastLine.Row; i--)
                {
                    this.lineMap.Remove(i);
                }

                this.LastLine = lastLine;
            }

            // 重置Viewable的状态
            this.ViewableArea.FirstLine = this.FirstLine;
            this.ViewableArea.LastLine = this.LastLine;

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
        /// 在指定的行位置插入多行，并把指定位置和指定位置后面的所有行往后移动
        /// </summary>
        /// <param name="row">要插入的行的位置</param>
        /// <param name="lines"></param>
        public void InsertLines(int row, int lines)
        {

        }

        #endregion
    }
}

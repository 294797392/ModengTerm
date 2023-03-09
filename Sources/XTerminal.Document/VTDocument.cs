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

        /// <summary>
        /// 光标所在行
        /// </summary>
        private VTextLine activeLine;

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

        /// <summary>
        /// 光标所在行
        /// </summary>
        public VTextLine ActiveLine { get { return this.activeLine; } }

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
            this.activeLine = firstLine;
            this.ViewableArea.FirstLine = firstLine;
            this.ViewableArea.LastLine = firstLine;

            this.IsArrangeDirty = true;
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
        /// 根据当前光标位置更新可显示区域
        /// 当行数量改变的时候调用此方法就可以
        /// </summary>
        /// <param name="oldCursorRow">光标移动之前的行</param>
        /// <param name="newCursorRow">光标要移动到的行</param>
        private void UpdateViewableArea(int oldCursorRow, int newCursorRow)
        {
            if (oldCursorRow == newCursorRow)
            {
                return;
            }

            // 可视区域的第一行
            int firstRow = this.ViewableArea.FirstLine.Row;

            // 可视区域的最后一行
            int lastRow = this.ViewableArea.LastLine.Row;

            // 可视区域的总行数
            int rows = lastRow - firstRow + 1;

            if (rows < this.Rows)
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
                /*
                 * ViewableArea:
                 * |------------|
                 * |------------|
                 * |------------|
                 * |------------|
                 * |------------|
                 * |------------|
                 * |------------|
                 */

                // 可视区域的总行数大于等于最大行数
                // 检查光标的新位置是否小于第一行或者是否大于最后一行

                // 标记布局已失效，下次渲染的时候需要重新布局
                this.SetArrangeDirty();

                VTextLine oldFirstLine = this.ViewableArea.FirstLine;
                VTextLine oldLastLine = this.ViewableArea.LastLine;

                // 重新计算第一行和最后一行的指针
                if (newCursorRow < firstRow)
                {
                    // 光标的新位置小于第一行，说明需要移动FirstLine指针
                    // 此时activeLine就是光标所在的行，可以直接使用activeLine
                    this.ViewableArea.FirstLine = this.activeLine;

                    // 同时更新LastLine指针
                    this.ViewableArea.LastLine = this.ViewableArea.LastLine.PreviousLine;
                }
                else if (newCursorRow > lastRow)
                {
                    // 和上面一样的思路更新首尾指针
                    this.ViewableArea.LastLine = this.activeLine;
                    this.ViewableArea.FirstLine = this.ViewableArea.FirstLine.NextLine;
                }

                VTextLine newFirstLine = this.ViewableArea.FirstLine;
                VTextLine newLastLine = this.ViewableArea.LastLine;

                // 复用DrawingObject
                newFirstLine.DrawingElement = oldFirstLine.DrawingElement;
                newFirstLine.DrawingElement.Element = newFirstLine;
                newLastLine.DrawingElement = oldLastLine.DrawingElement;
                newLastLine.DrawingElement.Element = newLastLine;
                newFirstLine.IsCharacterDirty = true; // 下次要重绘
                oldFirstLine.IsCharacterDirty = true; // 下次要重绘
            }
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
            this.activeLine = textLine;

            // 更新可视区域
            this.UpdateViewableArea(this.LastLine.Row, row);

            return textLine;
        }

        /// <summary>
        /// 在指定的光标位置打印一个字符
        /// </summary>
        /// <param name="ch"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        public void PrintCharacter(char ch, int row, int col)
        {
            VTextLine textLine;
            if (!this.lineMap.TryGetValue(row, out textLine))
            {
                logger.ErrorFormat("PrintCharacter失败, 获取对应的行失败, ch = {0}, row = {1}, col = {2}", ch, row, col);
                return;
            }

            textLine.PrintCharacter(ch, col);
        }

        /// <summary>
        /// 在当前光标处打印字符
        /// </summary>
        /// <param name="ch"></param>
        /// <param name="col"></param>
        public void PrintCharacter(char ch, int col)
        {
            if (this.activeLine == null)
            {
                logger.ErrorFormat("PrintCharacter失败，activeLine不存在, ch = {0}, col = {1}", ch, col);
                return;
            }

            this.activeLine.PrintCharacter(ch, col);
        }

        /// <summary>
        /// 设置文档中的光标位置
        /// 并更新ActiveLine
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        public void SetCursor(int row, int column)
        {
            if (this.Cursor.Row != row)
            {
                int oldCursorRow = this.Cursor.Row;
                this.Cursor.Row = row;

                // 光标位置改变的时候，就改变activeLine
                if (!this.lineMap.TryGetValue(row, out this.activeLine))
                {
                    this.activeLine = null;
                    logger.ErrorFormat("切换activeLine失败, 没找到对应的行, {0}", row);
                    return;
                }

                this.UpdateViewableArea(oldCursorRow, this.Cursor.Row);
            }

            if (this.Cursor.Column != column)
            {
                this.Cursor.Column = column;
            }
        }

        /// <summary>
        /// 在当前光标所在行开始删除字符操作
        /// </summary>
        public void EraseLine(EraseType eraseType)
        {
            if (this.activeLine == null)
            {
                logger.ErrorFormat("EraseLine失败, EraseType = {0}, 光标所在行不存在", eraseType);
                return;
            }

            switch (eraseType)
            {
                case EraseType.ToEnd:
                    {
                        // 删除从当前光标处到该行结尾的所有字符
                        this.activeLine.DeleteText(this.Cursor.Column);
                        break;
                    }

                case EraseType.FromBeginning:
                    {
                        // 删除从行首到当前光标处的内容
                        this.activeLine.DeleteText(0, this.Cursor.Column);
                        break;
                    }

                case EraseType.All:
                    {
                        // 删除光标所在整行
                        this.activeLine.DeleteAll();
                        break;
                    }

                default:
                    throw new NotImplementedException();
            }

        }

        public void EraseDisplay(EraseType eraseType)
        {
            switch (eraseType)
            {
                case EraseType.ToEnd:
                    {
                        // 从当前光标处直到屏幕最后一行全部都删除（包括当前光标处）
                        if (this.activeLine == null)
                        {
                            logger.ErrorFormat("EraseType.ToEnd失败, 光标所在行不存在");
                            return;
                        }

                        // 先删第一行
                        this.activeLine.DeleteText(this.Cursor.Column);

                        VTextLine next = this.activeLine.NextLine;
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
                        if (this.activeLine == null)
                        {
                            logger.ErrorFormat("EraseType.FromBeginning失败, 光标所在行不存在");
                            return;
                        }

                        this.activeLine.DeleteText(0, this.Cursor.Column);

                        VTextLine next = this.FirstLine;
                        while (next != null && next != this.activeLine)
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
        public void DeleteCharacter(int count)
        {
            if (this.activeLine == null)
            {
                logger.ErrorFormat("DeleteCharacter失败, 光标所在行不存在");
                return;
            }

            this.activeLine.DeleteText(this.Cursor.Column, count);
        }

        /// <summary>
        /// 清除所有字符
        /// 只留下第一行的文本行
        /// </summary>
        public void Reset()
        {
            this.FirstLine.DeleteAll();
            this.FirstLine.NextLine = null;
            this.LastLine = this.FirstLine;
            this.activeLine = this.FirstLine;
            this.lineMap.Clear();
            this.lineMap[0] = this.FirstLine;
        }

        #endregion
    }
}

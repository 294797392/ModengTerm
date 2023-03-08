using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTerminalParser;

namespace XTerminal.Document
{
    public class VTDocument
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("VTDocument");

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
        /// 最大列数
        /// </summary>
        public int Columns { get { return this.options.Columns; } }

        /// <summary>
        /// 光标所在行
        /// </summary>
        public VTextLine ActiveLine { get { return this.activeLine; } }

        #endregion

        #region 构造方法

        public VTDocument(VTDocumentOptions options)
        {
            this.lineMap = new Dictionary<int, VTextLine>();
            this.Cursor = new VCursor();

            VTextLine firstLine = new VTextLine()
            {
                Row = 0,
                OffsetX = 0,
                OffsetY = 0,
                CursorAtRightMargin = false,
                TerminalColumns = options.Columns,
                DECPrivateAutoWrapMode = options.DECPrivateAutoWrapMode,
                OwnerDocument = this,
            };
            this.lineMap[0] = firstLine;
            this.FirstLine = firstLine;
            this.LastLine = firstLine;
            this.activeLine = firstLine;

            this.options = options;
        }

        #endregion

        #region 公开接口

        /// <summary>
        /// 创建下一行
        /// </summary>
        /// <returns></returns>
        public VTextLine CreateNextLine()
        {
            int row = this.LastLine.Row + 1;

            VTextLine textLine = new VTextLine()
            {
                Row = row,
                OffsetX = 0,
                OffsetY = this.LastLine.Bounds.LeftBottom.Y,
                CursorAtRightMargin = false,
                TerminalColumns = this.Columns,
                DECPrivateAutoWrapMode = this.DECPrivateAutoWrapMode,
                OwnerDocument = this
            };
            this.lineMap[row] = textLine;

            this.LastLine.NextLine = textLine;
            textLine.PreviousLine = this.LastLine;
            this.LastLine = textLine;
            this.activeLine = textLine;

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
                this.Cursor.Row = row;

                // 光标位置改变的时候，就改变activeLine
                if (!this.lineMap.TryGetValue(row, out this.activeLine))
                {
                    this.activeLine = null;
                    logger.ErrorFormat("切换activeLine失败, 没找到对应的行, {0}", row);
                    return;
                }
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

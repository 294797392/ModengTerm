using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using XTerminal.Document.Rendering;

namespace XTerminal.Document
{
    public enum InsertOptions
    {
        /// <summary>
        /// 插入到前面
        /// </summary>
        PrependInsert,

        /// <summary>
        /// 插入到后面
        /// </summary>
        AppendInsert
    }

    /// <summary>
    /// 1. 对文本行进行排版，分块
    /// 2. 维护行的测量信息
    /// </summary>
    public class VTextLine : VTextElement
    {
        private static readonly string BlankText = " ";

        #region 实例变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("VTextLine");

        #endregion

        #region 实例变量

        #endregion

        #region 属性

        public int ID { get; set; }

        /// <summary>
        /// 终端行的最大列数
        /// 规定终端一行里的字符数不能超过列数
        /// 超过列数要按照手册里定义的标准来执行动作
        /// 在linux里使用stty size获取
        /// </summary>
        public int Capacity { get { return this.TextSource.Capacity; } }

        /// <summary>
        /// 已经显示了的行数
        /// </summary>
        public int Columns { get { return this.TextSource.Columns; } }

        /// <summary>
        /// 该行所有的文本块
        /// </summary>
        private List<VTextBlock> TextBlocks { get; set; }

        /// <summary>
        /// 上一个文本行
        /// </summary>
        public VTextLine PreviousLine { get; set; }

        /// <summary>
        /// 下一个文本行
        /// </summary>
        public VTextLine NextLine { get; set; }

        ///// <summary>
        ///// 第一个文本块
        ///// </summary>
        //public VTextBlock FirstBlock { get; private set; }

        ///// <summary>
        ///// 最后一个文本块
        ///// </summary>
        //public VTextBlock LastBlock { get; private set; }

        /// <summary>
        /// 是否开启了DECAWM模式
        /// </summary>
        public bool DECPrivateAutoWrapMode { get; set; }

        /// <summary>
        /// 当前光标是否在最右边
        /// </summary>
        public bool CursorAtRightMargin { get; set; }

        /// <summary>
        /// 所属的文档
        /// </summary>
        public VTDocument OwnerDocument { get; set; }

        /// <summary>
        /// 文本数据源
        /// </summary>
        public VTextSource TextSource { get; private set; }

        ///// <summary>
        ///// 该行的所有字符
        ///// </summary>
        //private List<VTCharacter> Characters { get; set; }

        #endregion

        #region 构造方法

        public VTextLine(int capacity)
        {
            //this.Characters = new List<VTCharacter>();
            this.TextBlocks = new List<VTextBlock>();
            this.TextSource = VTextSourceFactory.Create(VTextSources.CharactersTextSource, capacity);
        }

        #endregion

        #region 实例方法

        #endregion

        #region 公开接口

        /// <summary>
        /// 设置指定位置处的字符
        /// </summary>
        /// <param name="ch">要插入的字符</param>
        /// <param name="column">索引位置，在此处插入字符串</param>
        public void PrintCharacter(char ch, int column)
        {
            //if (this.CursorAtRightMargin && this.DECPrivateAutoWrapMode)
            //{
            //    // 说明光标已经在最右边了
            //    // 并且开启了自动换行(DECAWM)的功能，那么要自动换行

            //    // 换行完了之后再重置状态
            //    this.CursorAtRightMargin = false;
            //}
            //else
            //{
            // 更新文本
            if (column + 1 > this.Columns)
            {
                this.TextSource.PadRight(column + 1, ' ');
            }

            this.TextSource.SetCharacter(column, ch);

            this.SetDirty(true);

            //}

            ////if (this.Columns == this)
            //{
            //    // 光标在最右边了，下次在收到字符，要自动换行了
            //    // 在收到移动光标指令的时候，要清除这个标志
            //    this.CursorAtRightMargin = true;
            //}
        }

        /// <summary>
        /// 从指定位置开始删除字符串
        /// </summary>
        /// <param name="column">从此处开始删除字符</param>
        public void DeleteText(int column)
        {
            if (column >= this.Columns)
            {
                logger.WarnFormat("DeleteText失败，删除的索引位置在字符之外, {0}", this.Drawable.ID);
                return;
            }

            this.TextSource.Remove(column);

            this.SetDirty(true);
        }

        /// <summary>
        /// 删除指定位置处的字符串
        /// </summary>
        /// <param name="column">从此处开始删除字符</param>
        /// <param name="count">要删除的字符个数</param>
        public void DeleteText(int column, int count)
        {
            if (column >= this.Columns)
            {
                logger.WarnFormat("DeleteText失败，删除的索引位置在字符之外, {0}", this.Drawable.ID);
                return;
            }

            this.TextSource.Remove(column, count);

            this.SetDirty(true);
        }

        /// <summary>
        /// 删除整行字符
        /// </summary>
        public void DeleteAll()
        {
            this.TextSource.DeleteAll();

            this.SetDirty(true);
        }

        /// <summary>
        /// 从指定的位置开始使用指定的字符串替换源TextSource里的字符
        /// </summary>
        /// <param name="column"></param>
        /// <param name="ch"></param>
        public void Replace(int column, char ch)
        {
            if (column + 1 > this.Columns)
            {
                return;
            }

            for (int i = column; i < this.TextSource.Columns; i++)
            {
                this.TextSource.SetCharacter(i, ch);
            }

            this.SetDirty(true);
        }

        public void Replace(int column, int count, char ch)
        {
            for (int i = 0; i < count; i++)
            {
                int replaceColumn = column + i;

                if (replaceColumn > this.Columns)
                {
                    return;
                }

                this.TextSource.SetCharacter(replaceColumn, ch);
            }

            this.SetDirty(true);
        }

        public void ReplaceAll(char ch)
        {
            for (int i = 0; i < this.Columns; i++)
            {
                this.TextSource.SetCharacter(i, ch);
            }

            this.SetDirty(true);
        }

        public void Insert(int column, char ch)
        {
            this.TextSource.Insert(column, ch);

            this.SetDirty(true);
        }

        /// <summary>
        /// 获取该行的文本，如果字符数量是0，那么返回空白字符
        /// </summary>
        /// <returns></returns>
        public string GetText()
        {
            string text = this.TextSource.GetText();
            return text.Length == 0 ? BlankText : text;
        }

        /// <summary>
        /// 往前找到下一个VTextLine
        /// </summary>
        /// <param name="rows">向前几行</param>
        /// <returns></returns>
        public VTextLine FindNext(int rows)
        {
            VTextLine current = this;

            for (int i = 0; i < rows; i++)
            {
                current = current.NextLine;
            }

            return current;
        }

        /// <summary>
        /// 往后找到上一个VTextLine
        /// </summary>
        /// <param name="rows">向后几行</param>
        /// <returns></returns>
        public VTextLine FindPrevious(int rows)
        {
            VTextLine current = this;

            for (int i = 0; i < rows; i++)
            {
                current = current.PreviousLine;
            }

            return current;
        }

        /// <summary>
        /// 插到一个节点
        /// </summary>
        /// <param name="line">要插入的节点</param>
        /// <param name="options">是否是</param>
        public void InsertLine(VTextLine line, InsertOptions options)
        {
            switch (options)
            {
                case InsertOptions.AppendInsert:
                    {
                        // 插入到后面
                        if (this == this.OwnerDocument.LastLine)
                        {
                            // 该行是最后一行了，直接插入
                            this.NextLine = line;
                            line.PreviousLine = this;
                            this.OwnerDocument.LastLine = line;
                        }
                        else
                        {
                            // 该行不是最后一行
                            line.NextLine = this.NextLine;
                            line.PreviousLine = this;
                            this.NextLine = line;
                        }

                        break;
                    }

                case InsertOptions.PrependInsert:
                    {
                        // 插入到前面
                        if (this == this.OwnerDocument.FirstLine)
                        {
                            // 该行是第一行，直接插入
                            this.PreviousLine = line;
                            line.NextLine = this;
                            this.OwnerDocument.FirstLine = line;
                        }
                        else
                        {
                            // 该行不是最后一行
                            line.PreviousLine = this.PreviousLine;
                            line.NextLine = this;
                            this.PreviousLine = line;
                        }

                        break;
                    }

                default:
                    throw new NotImplementedException();
            }
        }

        #endregion
    }
}

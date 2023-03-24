using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using XTerminal.Document.Rendering;

namespace XTerminal.Document
{
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

        public override VTDocumentElements Type => VTDocumentElements.TextLine;

        /// <summary>
        /// 列大小
        /// 规定终端一行里的字符数不能超过列数
        /// 超过列数要按照手册里定义的标准来执行动作
        /// 在linux里使用stty size获取
        /// </summary>
        public int ColumnSize { get; set; }

        /// <summary>
        /// 已经显示了的行数
        /// </summary>
        public int Columns { get { return this.TextSource.Columns; } }

        /// <summary>
        /// 上一个文本行
        /// </summary>
        public VTextLine PreviousLine { get; set; }

        /// <summary>
        /// 下一个文本行
        /// </summary>
        public VTextLine NextLine { get; set; }

        /// <summary>
        /// 是否开启了DECAWM模式
        /// </summary>
        public bool DECPrivateAutoWrapMode { get; set; }

        /// <summary>
        /// 当前光标是否在最右边
        /// </summary>
        public bool CursorAtRightMargin { get; private set; }

        /// <summary>
        /// 文本数据源
        /// </summary>
        public VTextSource TextSource { get; private set; }

        /// <summary>
        /// 文本特性列表
        /// </summary>
        public List<VTextAttribute> Attributes { get; private set; }

        /// <summary>
        /// 获取该行的文本，如果字符数量是0，那么返回一个空白字符，目的是可以测量出来文本的测量信息
        /// </summary>
        public string Text
        {
            get
            {
                string text = this.TextSource.GetText();
                return text.Length == 0 ? BlankText : text;
            }
        }

        #endregion

        #region 构造方法

        /// <summary>
        /// 
        /// </summary>
        /// <param name="owner">该行所属的文档</param>
        public VTextLine(VTDocument owner) : base(owner)
        {
            this.ColumnSize = owner.ColumnSize;
            this.TextSource = VTextSourceFactory.Create(VTextSources.CharactersTextSource);
            this.Attributes = new List<VTextAttribute>();
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

            if (column == this.ColumnSize - 1)
            {
                //logger.ErrorFormat("光标在最右边");
                // 此时说明光标在最右边
                this.CursorAtRightMargin = true;
            }

            this.SetRenderDirty(true);
            //}

            //if (this.Columns == this)
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
                logger.WarnFormat("DeleteText失败，删除的索引位置在字符之外");
                return;
            }

            this.TextSource.Remove(column);

            this.SetRenderDirty(true);
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
                logger.WarnFormat("DeleteText失败，删除的索引位置在字符之外");
                return;
            }

            this.TextSource.Remove(column, count);

            this.SetRenderDirty(true);
        }

        /// <summary>
        /// 删除整行字符
        /// </summary>
        public void DeleteAll()
        {
            this.TextSource.DeleteAll();

            this.SetRenderDirty(true);
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

            this.SetRenderDirty(true);
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

            this.SetRenderDirty(true);
        }

        public void ReplaceAll(char ch)
        {
            for (int i = 0; i < this.Columns; i++)
            {
                this.TextSource.SetCharacter(i, ch);
            }

            this.SetRenderDirty(true);
        }

        public void Insert(int column, char ch)
        {
            this.TextSource.Insert(column, ch);

            this.SetRenderDirty(true);
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
        /// 把一个历史行的数据应用到VTextLine上
        /// </summary>
        /// <param name="historyLine">要应用的历史行数据</param>
        public void SetHistory(VTHistoryLine historyLine)
        {
            this.Attributes.Clear();
            this.Attributes.AddRange(historyLine.Attributes);

            this.TextSource.SetText(historyLine.Text);

            this.SetRenderDirty(true);
        }

        #endregion
    }
}

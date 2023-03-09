using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XTerminal.Document
{
    /// <summary>
    /// 1. 对文本行进行排版，分块
    /// 2. 维护行的测量信息
    /// </summary>
    public class VTextLine : VTextElement
    {
        #region 实例变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("VTextLine");

        #endregion

        #region 属性

        /// <summary>
        /// 终端行的最大列数
        /// 规定终端一行里的字符数不能超过列数
        /// 超过列数要按照手册里定义的标准来执行动作
        /// 在linux里使用stty size获取
        /// </summary>
        public int Columns { get; private set; }

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
        /// 画图对象
        /// </summary>
        public IDrawingObject DrawingObject { get; set; }

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
        /// 该行的所有字符
        /// </summary>
        private List<VTCharacter> Characters { get; set; }

        /// <summary>
        /// 表示该行是否有字符被修改了，需要重新渲染
        /// </summary>
        public bool IsCharacterDirty { get; set; }

        #endregion

        #region 构造方法

        public VTextLine(int columns)
        {
            this.Columns = columns;
            this.Characters = new List<VTCharacter>();
            this.TextBlocks = new List<VTextBlock>();
        }

        #endregion

        #region 实例方法

        /// <summary>
        /// 删除空的文本，然后使文本左对齐
        /// 并更新所有TextBlock的测量信息
        /// </summary>
        private void LeftAlignment()
        {
            //if (this.First == null)
            //{
            //    return;
            //}

            //string blockText = string.Empty;

            //#region 先对第一个元素排版

            //this.First.OffsetX = 0;
            //this.First.Column = 0;
            //blockText = this.Text.Substring(this.First.Column, this.First.Columns);
            //this.First.Metrics = this.OwnerCanvas.MeasureText(blockText, this.First.Style);

            //#endregion

            //// 从第二个元素开始遍历
            //VTextBlock current = this.First.Next;

            //while (current != null)
            //{
            //    if (current.IsEmpty())
            //    {
            //        // 如果这个文本块是空的，那么删除掉，并且更新current指针
            //        this.DeleteTextWithoutAlignment(current);

            //        // 移动current指针指向下一个元素
            //        current = current.Next;

            //        // 这里要continue，因为下一个元素也可能是空的
            //        continue;
            //    }

            //    VTextBlock previous = current.Previous;
            //    if (previous != null)
            //    {
            //        current.OffsetX = previous.OffsetX + previous.Width;
            //        current.Column = previous.Column + previous.Columns + 1;
            //    }

            //    // 更新测量信息
            //    blockText = this.Text.Substring(current.Column, current.Columns);
            //    current.Metrics = this.OwnerCanvas.MeasureText(blockText, current.Style);
            //}
        }

        /// <summary>
        /// 从缓存和链表里删除一个TextBlock
        /// 并更新链表指针
        /// </summary>
        /// <param name="textBlock">要删除的TextBlock</param>
        private void DeleteTextWithoutAlignment(VTextBlock textBlock)
        {
            //if (this.TextBlocks.Count == 1 && this.TextBlocks[0] == textBlock)
            //{
            //    // 此时该行只有一个TextBlock了，不能删光，要留一个备用
            //    return;
            //}

            //VTextBlock previous = textBlock.Previous;
            //if (previous != null)
            //{
            //    previous.Next = textBlock.Next;
            //}

            //VTextBlock next = textBlock.Next;
            //if (next != null)
            //{
            //    next.Previous = previous;
            //}

            //if (textBlock == this.First)
            //{
            //    this.First = textBlock.Next;
            //}

            //if (textBlock == this.Last)
            //{
            //    this.Last = textBlock.Previous;
            //}

            //this.TextBlocks.Remove(textBlock);
        }

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
            if (column > this.Characters.Count - 1)
            {
                // 说明是追加字符串操作
                this.Characters.Add(new VTCharacter(ch));
            }
            else
            {
                this.Characters[column].Character = ch;
            }            //}

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
            this.DeleteText(column, this.Characters.Count - column);
        }

        /// <summary>
        /// 删除指定位置处的字符串
        /// </summary>
        /// <param name="column">从此处开始删除字符</param>
        /// <param name="count">要删除的字符个数</param>
        public void DeleteText(int column, int count)
        {
            this.Characters.RemoveRange(column, count);
        }

        /// <summary>
        /// 删除整行
        /// </summary>
        public void DeleteAll()
        {
            this.Characters.Clear();
        }

        public string BuildText()
        {
            string text = string.Empty;

            foreach (VTCharacter character in this.Characters)
            {
                text += character.Character;
            }

            return text;
        }

        #endregion
    }
}

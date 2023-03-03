using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XTerminalBase
{
    /// <summary>
    /// 存储终端一行的数据
    /// </summary>
    public class VTextLine
    {
        #region 实例变量

        #endregion

        #region 属性

        /// <summary>
        /// 该行的索引
        /// </summary>
        public int Row { get; set; }

        /// <summary>
        /// 该行所有的文本块
        /// </summary>
        public List<VTextBlock> TextBlocks { get; set; }

        /// <summary>
        /// 第一个文本块
        /// </summary>
        public VTextBlock FirstTextBlock { get; private set; }

        /// <summary>
        /// 该行的Y偏移量
        /// </summary>
        public double OffsetY { get; set; }

        #endregion

        #region 构造方法

        public VTextLine()
        {
            this.TextBlocks = new List<VTextBlock>();
        }

        #endregion

        public void AddText(VTextBlock textBlock)
        {
            if (this.FirstTextBlock == null)
            {
                this.FirstTextBlock = textBlock;
            }

            VTextBlock previous = this.TextBlocks.LastOrDefault();
            textBlock.Previous = previous;
            if (previous != null)
            {
                previous.Next = textBlock;
            }
            this.TextBlocks.Add(textBlock);
        }

        public void DeleteText(List<VTextBlock> textBlocks)
        {
            foreach (VTextBlock textBlock in textBlocks)
            {
                this.DeleteText(textBlock);
            }
        }

        public void DeleteText(VTextBlock textBlock)
        {
            VTextBlock previous = textBlock.Previous;
            if (previous != null)
            {
                previous.Next = textBlock.Next;
            }

            VTextBlock next = textBlock.Next;
            if(next != null) 
            {
                next.Previous = previous;
            }

            if (textBlock == this.FirstTextBlock)
            {
                this.FirstTextBlock = textBlock.Next;
            }

            this.TextBlocks.Remove(textBlock);
        }

        /// <summary>
        /// 查询大于等于column列所有的文本块
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public List<VTextBlock> GetTextBlockAfter(int column)
        {
            return this.TextBlocks.Where(v => v.Column >= column || column >= v.Column && column <= v.Column + v.Columns).ToList();
        }

        /// <summary>
        /// 查询小于等于column列所有的文本块
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public List<VTextBlock> GetTextBlockBefore(int column)
        {
            return this.TextBlocks.Where(v => v.Column <= column || column >= v.Column && column <= v.Column + v.Columns).ToList();
        }

        /// <summary>
        /// 返回某个列所属的TextBlock
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public VTextBlock HitTestText(int column)
        {
            foreach (VTextBlock textBlock in this.TextBlocks)
            {
                int startCol = textBlock.Column;
                int endCol = textBlock.Column + textBlock.Columns;

                if(column >= startCol && column <= endCol)
                {
                    return textBlock;
                }
            }

            return null;
        }

        /// <summary>
        /// 获取该行所包含的所有文本块
        /// </summary>
        /// <returns></returns>
        public List<VTextBlock> GetAllText()
        {
            return this.TextBlocks.ToList();
        }

        /// <summary>
        /// 删除空的文本块
        /// </summary>
        public void DeleteEmpty()
        {
            VTextBlock next = this.FirstTextBlock;

            while (next != null)
            {
                if (next.IsEmpty())
                {
                    this.DeleteText(next);
                }

                next = next.Next;
            }
        }

        /// <summary>
        /// 删除空的文本，然后使文本左对齐
        /// </summary>
        public void LeftAlignment()
        {
            VTextBlock current = this.FirstTextBlock;

            while (current != null)
            {
                //current.X = current.Previous.X + current.Width;
            }
        }
    }
}

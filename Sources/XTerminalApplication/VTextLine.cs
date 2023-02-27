using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XTerminalDevice
{
    /// <summary>
    /// 存储终端一行的数据
    /// </summary>
    public class VTextLine
    {
        /// <summary>
        /// 该行的索引
        /// </summary>
        public int Row { get; set; }

        /// <summary>
        /// 该行所有的文本块
        /// </summary>
        private List<VTextBlock> TextBlocks { get; set; }

        /// <summary>
        /// 该行的Y偏移量
        /// </summary>
        public double OffsetY { get; set; }

        public VTextLine()
        {
            this.TextBlocks = new List<VTextBlock>();
        }

        public void AddTextBlock(VTextBlock textBlock)
        {
            this.TextBlocks.Add(textBlock);
        }

        public void DeleteTextBlock(IEnumerable<VTextBlock> textBlocks)
        {
            foreach (VTextBlock textBlock in textBlocks)
            {
                this.TextBlocks.Remove(textBlock);
            }
        }

        public void DeleteAllTextBlocks()
        {
            this.TextBlocks.Clear();
        }

        /// <summary>
        /// 查询大于等于column列所有的文本块
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public IEnumerable<VTextBlock> GetTextBlockAfter(int column)
        {
            return this.TextBlocks.Where(v => v.Column >= column);
        }

        /// <summary>
        /// 查询小于等于column列所有的文本块
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public IEnumerable<VTextBlock> GetTextBlockBefore(int column)
        {
            return this.TextBlocks.Where(v => v.Column <= column);
        }

        /// <summary>
        /// 获取该行所包含的所有文本块
        /// </summary>
        /// <returns></returns>
        public IEnumerable<VTextBlock> GetAllTextBlocks()
        {
            return this.TextBlocks;
        }
    }
}

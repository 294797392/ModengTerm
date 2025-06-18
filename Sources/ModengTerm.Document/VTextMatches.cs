using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Document
{
    /// <summary>
    /// 存储与搜索的关键字匹配的数据内容
    /// </summary>
    public struct VTextMatches
    {
        /// <summary>
        /// 关键字长度
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// 匹配到的关键字在该行中的索引位置
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// 文本相对于文档的位置
        /// </summary>
        public VTextRange TextRange { get; set; }

        public VTextMatches(VTextRange textRange, int length, int index)
        {
            this.TextRange = textRange;
            this.Length = length;
            this.Index = index;
        }
    }
}

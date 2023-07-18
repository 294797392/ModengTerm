using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XTerminal.Document
{
    /// <summary>
    /// 表示文本元素里的一个位置
    /// </summary>
    public class VTextPointer
    {
        /// <summary>
        /// 指针命中的行数
        /// </summary>
        public int Row { get { return this.LineHit.Row; } }

        /// <summary>
        /// 光标所在行
        /// </summary>
        public VTHistoryLine LineHit { get; set; }

        /// <summary>
        /// 光标所命中的字符的边界框信息
        /// </summary>
        public VTRect CharacterBounds { get; set; }

        /// <summary>
        /// 命中的字符的索引
        /// 如果没命中，那么就是-1
        /// </summary>
        public int CharacterIndex { get; set; }
    }
}

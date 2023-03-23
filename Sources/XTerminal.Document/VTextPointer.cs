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
        public int Row { get { return this.Line.Row; } }

        /// <summary>
        /// 命中的字符的测量信息
        /// </summary>
        public VTRect CharacterBounds { get; set; }

        /// <summary>
        /// 命中的字符的索引
        /// </summary>
        public int CharacterIndex { get; set; }

        /// <summary>
        /// 光标所命中的行
        /// </summary>
        public VTHistoryLine Line { get; set; }

        /// <summary>
        /// 是否是空的位置
        /// </summary>
        public bool IsEmpty { get; set; }
    }
}

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
        /// 命中的行的物理行数，从0开始
        /// 如果没命中，那么就是-1
        /// </summary>
        public int PhysicsRow { get; set; }

        ///// <summary>
        ///// 命中的行相对于Surface的Y偏移量
        ///// </summary>
        //public double OffsetY { get; set; }

        /// <summary>
        /// 命中的字符的索引，从0开始
        /// 如果没命中，那么就是-1
        /// </summary>
        public int CharacterIndex { get; set; }

        ///// <summary>
        ///// 命中的字符的边界框信息
        ///// 注意Y值是0，因为是相对于该行的边界框
        ///// </summary>
        //public VTRect CharacterBounds { get; set; }
    }
}

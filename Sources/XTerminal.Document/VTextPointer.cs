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
        /// 相对于行的Y偏移量
        /// </summary>
        public double OffsetYLine { get; set; }

        /// <summary>
        /// 所属的行
        /// </summary>
        public VTextLine OwnerLine { get; set; }
    }
}

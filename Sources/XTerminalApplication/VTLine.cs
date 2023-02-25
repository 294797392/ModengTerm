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
    public class VTLine
    {
        /// <summary>
        /// 该行的索引
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// 该行所有的文本块
        /// </summary>
        public List<VTextBlock> TextBlocks { get; private set; }

        public VTLine() 
        {
            this.TextBlocks = new List<VTextBlock>();
        }
    }
}

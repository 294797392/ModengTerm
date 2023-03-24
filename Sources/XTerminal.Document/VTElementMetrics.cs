using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XTerminal.Document
{
    /// <summary>
    /// 存储文档里某个元素的测量信息
    /// </summary>
    public class VTElementMetrics
    {
        /// <summary>
        /// 渲染后该文本块的宽度
        /// 忽略空白宽度
        /// </summary>
        public double Width { get; set; }

        /// <summary>
        /// 渲染后该文本块的高度
        /// </summary>
        public double Height { get; set; }

        public VTElementMetrics()
        { }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XTerminal.Document
{
    /// <summary>
    /// 文本元素的基类
    /// 定义了文本元素的一些基础属性，测量信息
    /// </summary>
    public class VTextElement : VTDocumentElement
    {
        /// <summary>
        /// 文本的测量信息
        /// </summary>
        public VTElementMetrics Metrics { get; set; }

        /// <summary>
        /// 获取该文本块的宽度
        /// </summary>
        public double Width { get { return this.Metrics.WidthIncludingWhitespace; } }

        /// <summary>
        /// 该行高度，当DECAWM被设置的时候，终端里的一行如果超出了列数，那么会自动换行
        /// 当一行的字符超过终端的列数的时候，DECAWM指令指定了超出的字符要如何处理
        /// DECAWM SET：超出后要在新的一行上从头开始显示字符
        /// DECAWM RESET：超出后在该行的第一个字符处开始显示字符
        /// </summary>
        public double Height { get { return this.Metrics.Height; } }

        /// <summary>
        /// 获取该文本的边界框信息
        /// 在画完之后会更新测量的矩形框信息
        /// </summary>
        public VTRect Bounds { get { return new VTRect(this.OffsetX, this.OffsetY, this.Width, this.Height); } }

        public VTextElement()
        {
            this.Metrics = new VTElementMetrics();
        }
    }
}

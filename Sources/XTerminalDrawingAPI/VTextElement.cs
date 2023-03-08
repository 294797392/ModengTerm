using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XTerminal.Drawing
{
    public abstract class VTextElement<TTextElement>
    {
        /// <summary>
        /// 上一个文本元素
        /// </summary>
        public TTextElement Previous { get; set; }

        /// <summary>
        /// 下一个文本元素
        /// </summary>
        public TTextElement Next { get; set; }

        /// <summary>
        /// 该文本块左上角的X坐标
        /// </summary>
        public double OffsetX { get; set; }

        /// <summary>
        /// 该文本块左上角的Y坐标
        /// </summary>
        public double OffsetY { get; set; }

        /// <summary>
        /// 文本的测量信息
        /// </summary>
        public VTextMetrics Metrics { get; set; }

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
        /// 获取该文本的矩形框
        /// </summary>
        public VTRect Boundary { get { return new VTRect(this.OffsetX, this.OffsetY, this.Width, this.Height); } }

        /// <summary>
        /// 该文本块所在行数，从0开始
        /// </summary>
        public int Row { get; set; }

        public VTextElement()
        {
            this.Metrics = new VTextMetrics();
        }
    }
}

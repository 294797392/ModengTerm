using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTerminalParser;

namespace XTerminalDevice
{
    public class VTextBlock
    {
        /// <summary>
        /// TextBlock的索引号
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// 文本的颜色
        /// </summary>
        public VTForeground Foreground { get; set; }

        /// <summary>
        /// 字号
        /// </summary>
        public double Size { get; set; }

        /// <summary>
        /// 要显示的文本
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// 该文本块左上角的X坐标
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// 该文本块左上角的Y坐标
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// 文本的测量信息
        /// Draw完之后就有了测量信息
        /// </summary>
        public VTextBlockMetrics Metrics { get; private set; }

        /// <summary>
        /// 获取该文本块的宽度
        /// </summary>
        public double Width { get { return this.Metrics.Width; } }

        /// <summary>
        /// 获取该文本块的高度
        /// </summary>
        public double Height { get { return this.Metrics.Height; } }

        /// <summary>
        /// 获取该文本的矩形框
        /// </summary>
        public VTRect Boundary { get { return new VTRect(this.X, this.Y, this.Width, this.Height); } }

        public VTextBlock()
        {
            this.Metrics = new VTextBlockMetrics();
        }

        public void AppendText(char text)
        {
            this.Text += text;
        }

        public void AppendText(string text)
        {
            this.Text += text;
        }
    }
}

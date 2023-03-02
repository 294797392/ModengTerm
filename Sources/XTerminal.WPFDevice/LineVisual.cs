using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace XTerminal.WPFRenderer
{
    public class LineVisual : TerminalVisual
    {
        /// <summary>
        /// 文本块的索引
        /// </summary>
        public string ID { get { return this.TextBlock.ID; } }

        public double PixelsPerDip { get; set; }

        public Typeface Typeface { get; set; }

        /// <summary>
        /// 要渲染的文本
        /// </summary>
        public VTextLine TextBlock { get; private set; }

        /// <summary>
        /// 渲染的文本测量信息
        /// </summary>
        public VTextBlockMetrics Metrics { get { return this.TextBlock.Metrics; } }

        protected override void Draw(DrawingContext dc)
        {
            throw new NotImplementedException();
        }
    }
}

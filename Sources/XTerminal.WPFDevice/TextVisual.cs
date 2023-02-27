using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using XTerminalDevice;

namespace XTerminal.WPFRenderer
{
    /// <summary>
    /// 表示PresentationDevice上的一个文本块
    /// </summary>
    public class TextVisual : TerminalVisual
    {
        #region 属性

        /// <summary>
        /// 文本块的索引
        /// </summary>
        public int Index { get { return this.TextBlock.Index; } }

        public double PixelsPerDip { get; set; }

        public Typeface Typeface { get; set; }

        /// <summary>
        /// 要渲染的文本
        /// </summary>
        public VTextBlock TextBlock { get; private set; }

        /// <summary>
        /// 渲染的文本测量信息
        /// </summary>
        public VTextBlockMetrics Metrics { get { return this.TextBlock.Metrics; } }

        #endregion

        #region 构造方法

        public TextVisual(VTextBlock textBlock)
        {
            this.TextBlock = textBlock;
        }

        #endregion

        #region TerminalVisual

        protected override void Draw(DrawingContext dc)
        {
            this.Offset = new Vector(this.TextBlock.X, this.TextBlock.Y);

            FormattedText formattedText = TerminalUtils.CreateFormattedText(this.TextBlock, this.Typeface, this.PixelsPerDip);
            TerminalUtils.UpdateTextMetrics(this.TextBlock, formattedText);
            dc.DrawText(formattedText, new Point());
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using XTerminalController;

namespace XTerminal.VideoTerminal
{
    public class TextVisual : TerminalVisual
    {
        #region 属性

        public double PixelsPerDip { get; set; }

        public Typeface Typeface { get; set; }

        public double FontSize { get; set; }

        public Brush Foreground { get; set; }

        /// <summary>
        /// 要渲染的文本
        /// </summary>
        public VTextBlock TextBlock { get; private set; }

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

            FormattedText formattedText = new FormattedText(this.TextBlock.Text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, this.Typeface, this.FontSize, this.Foreground, this.PixelsPerDip);
            dc.DrawText(formattedText, new System.Windows.Point(0, 0));

            this.TextBlock.Width = formattedText.Width;
            this.TextBlock.Height = formattedText.Height;
        }

        #endregion
    }
}

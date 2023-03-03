using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using XTerminalBase;

namespace XTerminal.WPFRenderer
{
    /// <summary>
    /// 表示PresentationDevice上的一个文本块
    /// </summary>
    public class DrawingText : DrawingObject
    {
        #region 属性

        /// <summary>
        /// 文本块的索引
        /// </summary>
        public string ID { get { return this.TextBlock.ID; } }

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

        public DrawingText(VTextBlock textBlock)
        {
            this.TextBlock = textBlock;
        }

        #endregion

        #region TerminalVisual

        protected override void Draw(DrawingContext dc)
        {
            this.Offset = new Vector(this.TextBlock.X, this.TextBlock.Y);

            Typeface typeface = TerminalUtils.GetTypeface(this.TextBlock);
            FormattedText formattedText = TerminalUtils.UpdateTextMetrics(this.TextBlock);
            dc.DrawText(formattedText, new Point());
            dc.DrawRectangle(null, new Pen(Brushes.Black, 1), new Rect(0, 0, this.TextBlock.Width, this.TextBlock.Height));
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;

namespace XTerminal.Document
{
    public class DrawingLine : DrawingObject
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("DrawingLine");

        #region 属性

        /// <summary>
        /// 该行在ViewableDocument里的位置
        /// </summary>
        public int Row { get; set; }

        #endregion

        protected override void Draw(DrawingContext dc)
        {
            VTextLine textLine = this.Element as VTextLine;

            string text = textLine.BuildText();

            //text = string.Format("{0} - {1} ******* {2}", this.Row, textLine.Row, text);

            this.Offset = new Vector(0, textLine.OffsetY);

            Typeface typeface = TerminalUtils.GetTypeface(VTextStyle.Default);
            FormattedText formattedText = new FormattedText(text, System.Globalization.CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeface,
                DefaultValues.FontSize, DefaultValues.Foreground, null, TextFormattingMode.Display, TerminalUtils.PixelsPerDip);

            TerminalUtils.UpdateTextMetrics(textLine.Metrics, formattedText);

            //logger.InfoFormat("Render:{0}", text);

            //// 遍历链表，给每个TextBlock设置样式
            //VTextBlock current = this.TextLine.First;

            //while (current != null)
            //{
            //    // TODO：设置样式

            //    current = current.Next;
            //}

            dc.DrawText(formattedText, new Point());
        }
    }
}

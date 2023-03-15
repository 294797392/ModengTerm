using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;
using XTerminal.Document;

namespace XTerminal.Rendering
{
    public class DrawableLine : XDocumentDrawable
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("DrawableLine");

        #region 属性

        /// <summary>
        /// 该行在ViewableDocument里的位置
        /// </summary>
        public int Row { get; private set; }

        #endregion

        public DrawableLine(int row)
        {
            this.Row = row;
            this.ID = "Drawable" + string.Format("{0}", row).PadLeft(2, '0');
        }

        protected override void Draw(DrawingContext dc)
        {
            VTextLine textLine = this.OwnerElement as VTextLine;

            string text = textLine.GetText();

            text = string.Format("{0} - {1}", this.ID, text);

            this.Offset = new Vector(0, textLine.OffsetY);

            Typeface typeface = WPFRenderUtils.GetTypeface(VTextStyle.Default);
            FormattedText formattedText = new FormattedText(text, System.Globalization.CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeface,
                DefaultValues.FontSize, DefaultValues.Foreground, null, TextFormattingMode.Display, App.PixelsPerDip);

            WPFRenderUtils.UpdateTextMetrics(textLine.Metrics, formattedText);

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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;
using XTerminal.Base;
using XTerminal.Document;

namespace XTerminal.Rendering
{
    public class DrawingLine : DrawingObject
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("DrawableLine");

        #region 属性

        /// <summary>
        /// 该行在ViewableDocument里的位置
        /// </summary>
        public int Row { get; private set; }

        #endregion

        public DrawingLine()
        {
            this.ID = "Drawable" + string.Format("{0}", -1).PadLeft(2, '0');
        }

        protected override void Draw(DrawingContext dc)
        {
            VTextLine textLine = this.Drawable as VTextLine;

            string text = textLine.Text;

            //text = string.Format("{0} - {1}", this.ID, text);

            this.Offset = new Vector(0, textLine.OffsetY);

            Typeface typeface = WPFRenderUtils.GetTypeface(VTextStyle.Default);
            FormattedText formattedText = new FormattedText(text, System.Globalization.CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeface,
                XTermDefaultValues.FontSize, XTermDefaultValues.Foreground, null, TextFormattingMode.Display, App.PixelsPerDip);

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

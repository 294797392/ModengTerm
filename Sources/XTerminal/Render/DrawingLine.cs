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
    public class DrawingLine : DrawingObject, IDrawingObject
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("DrawingLine");

        #region 属性

        /// <summary>
        /// 要渲染的textLine对象
        /// </summary>
        public VTextLine TextLine { get; set; }

        #endregion

        protected override void Draw(DrawingContext dc)
        {
            string text = this.TextLine.BuildText();

            text = string.Format("{0} ******* {1}", this.TextLine.Row, text);

            this.Offset = new Vector(0, this.TextLine.OffsetY);

            Typeface typeface = TerminalUtils.GetTypeface(VTextStyle.Default);
            FormattedText formattedText = new FormattedText(text, System.Globalization.CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeface,
                DefaultValues.FontSize, DefaultValues.Foreground, null, TextFormattingMode.Display, TerminalUtils.PixelsPerDip);

            TerminalUtils.UpdateTextMetrics(this.TextLine.Metrics, formattedText);

            logger.InfoFormat("Render:{0}", text);

            //// 遍历链表，给每个TextBlock设置样式
            //VTextBlock current = this.TextLine.First;

            //while (current != null)
            //{
            //    // TODO：设置样式

            //    current = current.Next;
            //}

            dc.DrawText(formattedText, new Point());
        }

        public void Reset()
        {
            this.TextLine = null;
            this.Offset = new Vector();
            DrawingContext dc = this.RenderOpen();
            dc.Close();
        }
    }
}

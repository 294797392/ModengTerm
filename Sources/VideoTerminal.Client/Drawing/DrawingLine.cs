using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;

namespace XTerminal.Drawing
{
    public class DrawingLine : DrawingObject
    {
        #region 属性

        /// <summary>
        /// 要渲染的textLine对象
        /// </summary>
        public VTextLine TextLine { get; set; }

        #endregion

        protected override void Draw(DrawingContext dc)
        {
            this.Offset = new Vector(0, this.TextLine.OffsetY);

            Typeface typeface = TerminalUtils.GetTypeface(VTextStyle.Default);
            FormattedText formattedText = new FormattedText(this.TextLine.Text, System.Globalization.CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeface, DefaultValues.FontSize, DefaultValues.Foreground,
                null, TextFormattingMode.Display, TerminalUtils.PixelsPerDip);

            // 遍历链表，给每个TextBlock设置样式
            VTextBlock current = this.TextLine.First;

            while (current != null)
            {
                // TODO：设置样式

                current = current.Next;
            }

            dc.DrawText(formattedText, new Point());
        }
    }
}

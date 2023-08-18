using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using XTerminal.Document;
using XTerminal.Document.Rendering;

namespace XTerminal.Rendering
{
    public class WPFTextMeter : VTextMeter
    {
        private static readonly Point ZeroPoint = new Point();

        private VTRect CommonMeasureLine(VTextLine textLine, int startIndex, int count)
        {
            int totalChars = textLine.Characters.Count;
            if (startIndex + count > totalChars)
            {
                startIndex = 0;
                count = totalChars;
            }

            if (startIndex == 0 && count == 0)
            {
                return new VTRect();
            }

            FormattedText formattedText = DrawingUtils.CreateFormattedText(textLine);
            Geometry geometry = formattedText.BuildHighlightGeometry(ZeroPoint, startIndex, count);
            return new VTRect(geometry.Bounds.Left, geometry.Bounds.Top, geometry.Bounds.Width, geometry.Bounds.Height);
        }

        /// <summary>
        /// 测量某个渲染模型的大小
        /// TODO：如果测量的是字体，要考虑到对字体应用样式后的测量信息
        /// </summary>
        /// <param name="textLine">要测量的数据模型</param>
        /// <param name="maxCharacters">要测量的最大字符数</param>
        /// <returns></returns>
        public override VTRect MeasureLine(VTextLine textLine, int startIndex, int count)
        {
            return this.CommonMeasureLine(textLine, startIndex, count);
        }

        public override VTRect MeasureCharacter(VTextLine textLine, int characterIndex)
        {
            return this.CommonMeasureLine(textLine, characterIndex, 1);
        }

        public override VTextMetrics MeasureText(string text, double fontSize, string fontFamily)
        {
            Typeface typeface = new Typeface(new FontFamily(fontFamily), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
            FormattedText formattedText = new FormattedText(text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeface,
                fontSize, Brushes.Black, null, TextFormattingMode.Display, App.PixelsPerDip);

            VTextMetrics metrics = new VTextMetrics() 
            {
                Height = formattedText.Height,
                Width = formattedText.WidthIncludingTrailingWhitespace
            };

            return metrics;
        }
    }
}

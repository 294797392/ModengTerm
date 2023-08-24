using ModengTerm.VideoTerminal;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using XTerminal;
using XTerminal.Base;
using XTerminal.Document;
using XTerminal.Parser;

namespace ModengTerm.Rendering
{
    public static class DrawingUtils
    {
        private static readonly BrushConverter BrushConverter = new BrushConverter();

        static DrawingUtils()
        {
        }

        public static Brush VTColor2Brush(VTColors foreground)
        {
            switch (foreground)
            {
                case VTColors.BrightBlack: return Brushes.Black;
                case VTColors.BrightBlue: return Brushes.Blue;
                case VTColors.BrightCyan: return Brushes.Cyan;
                case VTColors.BrightGreen: return Brushes.Green;
                case VTColors.BrightMagenta: return Brushes.Magenta;
                case VTColors.BrightRed: return Brushes.Red;
                case VTColors.BrightWhite: return Brushes.White;
                case VTColors.BrightYellow: return Brushes.Yellow;

                case VTColors.DarkBlack: return Brushes.Black;
                case VTColors.DarkBlue: return Brushes.DarkBlue;
                case VTColors.DarkCyan: return Brushes.DarkCyan;
                case VTColors.DarkGreen: return Brushes.DarkGreen;
                case VTColors.DarkMagenta: return Brushes.DarkMagenta;
                case VTColors.DarkRed: return Brushes.DarkRed;
                case VTColors.DarkWhite: return Brushes.White;
                case VTColors.DarkYellow: return Brushes.Yellow;

                default:
                    throw new NotImplementedException();
            }
        }

        public static void UpdateTextMetrics(VTextLine textLine, FormattedText formattedText)
        {
            textLine.Metrics.Width = formattedText.WidthIncludingTrailingWhitespace;
            textLine.Metrics.Height = formattedText.Height;
        }

        public static VTKeys ConvertToVTKey(Key key)
        {
            return (VTKeys)key;
        }

        public static Brush ConvertBrush(string colorName)
        {
            return (Brush)BrushConverter.ConvertFrom(colorName);
        }

        /// <summary>
        /// 根据VTextLine生成一个FormattedText
        /// </summary>
        /// <param name="textLine"></param>
        /// <returns></returns>
        public static FormattedText CreateFormattedText(VTextLine textLine)
        {
            //string text = string.Format("{0} - {1}", textLine.ID, textLine.Text);
            string text = textLine.Text;

            DrawingLine drawingLine = textLine.DrawingContext as DrawingLine;
            FormattedText formattedText = new FormattedText(text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, drawingLine.typeface,
                textLine.Style.FontSize, drawingLine.foreground, null, TextFormattingMode.Display, App.PixelsPerDip);
            return formattedText;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using XTerminal.Terminal;
using XTerminalParser;

namespace XTerminal.Document
{
    public static class TerminalUtils
    {
        private static Dictionary<string, Typeface> typefaceMap = new Dictionary<string, Typeface>();

        public static double PixelsPerDip = 0;

        static TerminalUtils()
        {
            PixelsPerDip = VisualTreeHelper.GetDpi(Application.Current.MainWindow).PixelsPerDip;
        }

        public static Brush VTForeground2Brush(VTForeground foreground)
        {
            switch (foreground)
            {
                case VTForeground.BrightBlack: return Brushes.Black;
                case VTForeground.BrightBlue: return Brushes.Blue;
                case VTForeground.BrightCyan: return Brushes.Cyan;
                case VTForeground.BrightGreen: return Brushes.Green;
                case VTForeground.BrightMagenta: return Brushes.Magenta;
                case VTForeground.BrightRed: return Brushes.Red;
                case VTForeground.BrightWhite: return Brushes.White;
                case VTForeground.BrightYellow: return Brushes.Yellow;

                case VTForeground.DarkBlack: return Brushes.Black;
                case VTForeground.DarkBlue: return Brushes.DarkBlue;
                case VTForeground.DarkCyan: return Brushes.DarkCyan;
                case VTForeground.DarkGreen: return Brushes.DarkGreen;
                case VTForeground.DarkMagenta: return Brushes.DarkMagenta;
                case VTForeground.DarkRed: return Brushes.DarkRed;
                case VTForeground.DarkWhite: return Brushes.White;
                case VTForeground.DarkYellow: return Brushes.Yellow;

                default:
                    throw new NotImplementedException();
            }
        }

        public static VTextMetrics UpdateTextMetrics(string text, VTextStyle textStyle)
        {
            Typeface typeface = GetTypeface(textStyle);
            FormattedText formattedText = new FormattedText(text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeface, textStyle.FontSize, Brushes.Black, null, TextFormattingMode.Display, TerminalUtils.PixelsPerDip);
            VTextMetrics textMetrics = new VTextMetrics();
            UpdateTextMetrics(textMetrics, formattedText);
            return textMetrics;
        }

        public static void UpdateTextMetrics(VTextMetrics textMerics, FormattedText formattedText)
        {
            textMerics.Width = formattedText.Width;
            textMerics.WidthIncludingWhitespace = formattedText.WidthIncludingTrailingWhitespace;
            textMerics.Height = formattedText.Height;
        }

        public static VTKeys ConvertToVTKey(Key key)
        {
            return (VTKeys)key;
        }

        public static Typeface GetTypeface(VTextStyle style)
        {
            Typeface typeface;
            if (!typefaceMap.TryGetValue(style.HashID, out typeface))
            {
                typeface = new Typeface(new FontFamily(style.FontFamily), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
                typefaceMap[style.HashID] = typeface;
            }
            return typeface;
        }
    }
}

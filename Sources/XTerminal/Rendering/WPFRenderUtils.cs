using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using XTerminal.Document;
using XTerminal.Parser;
using XTerminal.VTDefinitions;

namespace XTerminal.Rendering
{
    public static class WPFRenderUtils
    {
        private static Dictionary<string, Typeface> typefaceMap = new Dictionary<string, Typeface>();

        public static double PixelsPerDip = 0;

        static WPFRenderUtils()
        {
            PixelsPerDip = VisualTreeHelper.GetDpi(Application.Current.MainWindow).PixelsPerDip;
        }

        public static Brush VTForeground2Brush(VTColors foreground)
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

        public static void UpdateTextMetrics(VTElementMetrics textMerics, FormattedText formattedText)
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

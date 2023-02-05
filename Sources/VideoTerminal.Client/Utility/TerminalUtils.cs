using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using XTerminalController;
using XTerminalParser;

namespace VideoTerminal.Utility
{
    public static class TerminalUtils
    {
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

        public static FormattedText CreateFormattedText(VTextBlock textBlock, Typeface typeface, double pixelsPerDip)
        {
            Brush foreground = TerminalUtils.VTForeground2Brush(textBlock.Foreground);
            FormattedText formattedText = new FormattedText(textBlock.Text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeface, textBlock.Size, foreground, pixelsPerDip);
            return formattedText;
        }

        public static void UpdateTextMetrics(VTextBlock textBlock, FormattedText formattedText)
        {
            textBlock.Metrics.Width = formattedText.Width;
            textBlock.Metrics.WidthIncludingWhitespace = formattedText.WidthIncludingTrailingWhitespace;
            textBlock.Metrics.Height = formattedText.Height;
        }

        public static VTKeys ConvertToVTKey(Key key)
        {
            switch (key)
            {
                case Key.A: return VTKeys.A;
                case Key.B: return VTKeys.B;
                case Key.C: return VTKeys.C;
                case Key.D: return VTKeys.D;
                case Key.E: return VTKeys.E;
                case Key.F: return VTKeys.F;
                case Key.G: return VTKeys.G;
                case Key.H: return VTKeys.H;
                case Key.I: return VTKeys.I;
                case Key.J: return VTKeys.J;
                case Key.K: return VTKeys.K;
                case Key.L: return VTKeys.L;
                case Key.M: return VTKeys.M;
                case Key.N: return VTKeys.N;
                case Key.O: return VTKeys.O;
                case Key.P: return VTKeys.P;
                case Key.Q: return VTKeys.Q;
                case Key.R: return VTKeys.R;
                case Key.S: return VTKeys.S;
                case Key.T: return VTKeys.T;
                case Key.U: return VTKeys.U;
                case Key.V: return VTKeys.V;
                case Key.W: return VTKeys.W;
                case Key.X: return VTKeys.X;
                case Key.Y: return VTKeys.Y;
                case Key.Z: return VTKeys.Z;

                case Key.Enter: return VTKeys.Enter;
                case Key.Space: return VTKeys.Spacebar;

                case Key.Up: return VTKeys.UpArrow;
                case Key.Down: return VTKeys.DownArrow;
                case Key.Left: return VTKeys.LeftArrow;
                case Key.Right: return VTKeys.RightArrow;

                default:
                    return VTKeys.None;
            }
        }
    }
}

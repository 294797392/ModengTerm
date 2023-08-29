using ModengTerm.Terminal;
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

        public static readonly Point ZeroPoint = new Point();

        public static readonly Pen TransparentPen = new Pen(Brushes.Transparent, 0);
        public static readonly Pen BlackPen = new Pen(Brushes.Black, 1);

        public static readonly TextDecorationCollection Underline = new TextDecorationCollection()
        {
            new TextDecoration(TextDecorationLocation.Underline, BlackPen, 0, TextDecorationUnit.FontRecommended, TextDecorationUnit.FontRecommended)
        };

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
        /// <param name="dc">画Background的时候需要</param>
        /// <returns></returns>
        public static FormattedText CreateFormattedText(VTextLine textLine, DrawingContext dc = null)
        {
            //string text = string.Format("{0} - {1}", textLine.ID, textLine.Text);
            string text = textLine.Text;

            DrawingLine drawingLine = textLine.DrawingContext as DrawingLine;
            FormattedText formattedText = new FormattedText(text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, drawingLine.typeface,
                textLine.Style.FontSize, drawingLine.foreground, null, TextFormattingMode.Display, App.PixelsPerDip);

            #region 画文本装饰

            foreach (VTextAttribute textAttribute in textLine.Attributes)
            {
                if (!textAttribute.Unset)
                {
                    // 没有设置Unset，那么不渲染
                    continue;
                }

                int startIndex = textLine.FindCharacterIndex(textAttribute.StartColumn);
                int endIndex = textLine.FindCharacterIndex(textAttribute.EndColumn);
                if (startIndex == -1 || endIndex == -1)
                {
                    continue;
                }

                if (startIndex > endIndex)
                {
                    continue;
                }

                switch (textAttribute.Decoration)
                {
                    case VTextDecorations.Foreground:
                        {
                            if (textAttribute.Parameter == null)
                            {
                                continue;
                            }

                            VTColors color = (VTColors)textAttribute.Parameter;
                            Brush brush = DrawingUtils.VTColor2Brush(color);
                            formattedText.SetForegroundBrush(brush, startIndex, endIndex - startIndex + 1);
                            break;
                        }

                    case VTextDecorations.Background:
                        {
                            if(dc == null)
                            {
                                continue;
                            }

                            if (textAttribute.Parameter == null)
                            {
                                continue;
                            }

                            VTColors color = (VTColors)textAttribute.Parameter;
                            Brush brush = DrawingUtils.VTColor2Brush(color);
                            Geometry geometry = formattedText.BuildHighlightGeometry(ZeroPoint, startIndex, endIndex - startIndex + 1);
                            dc.DrawRectangle(brush, DrawingUtils.TransparentPen, geometry.Bounds);
                            break;
                        }

                    case VTextDecorations.Bold:
                        {
                            formattedText.SetFontWeight(FontWeights.Bold, startIndex, endIndex - startIndex + 1);
                            break;
                        }

                    case VTextDecorations.Italics:
                        {
                            formattedText.SetFontStyle(FontStyles.Italic, startIndex, endIndex - startIndex + 1);
                            break;
                        }

                    case VTextDecorations.Underline:
                        {
                            formattedText.SetTextDecorations(TextDecorations.Underline, startIndex, endIndex - startIndex + 1);
                            break;
                        }

                    default:
                        break;
                }
            }

            #endregion

            return formattedText;
        }
    }
}

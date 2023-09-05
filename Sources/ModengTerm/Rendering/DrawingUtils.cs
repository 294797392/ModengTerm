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
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger("DrawingUtils");

        private static readonly BrushConverter BrushConverter = new BrushConverter();

        public static readonly Point ZeroPoint = new Point();

        public static readonly Pen TransparentPen = new Pen(Brushes.Transparent, 0);
        public static readonly Pen BlackPen = new Pen(Brushes.Black, 1);

        private static readonly Dictionary<VTColor, Brush> VTColorBrushMap = new Dictionary<VTColor, Brush>()
        {
            { VTColor.BrightBlack, Brushes.Black }, { VTColor.BrightBlue, Brushes.Blue }, { VTColor.BrightCyan, Brushes.Cyan },
            { VTColor.BrightGreen, Brushes.Green }, { VTColor.BrightMagenta, Brushes.Magenta }, { VTColor.BrightRed, Brushes.Red },
            { VTColor.BrightWhite, Brushes.White }, { VTColor.BrightYellow, Brushes.Yellow },

            { VTColor.DarkBlack, Brushes.Black }, { VTColor.DarkBlue, Brushes.Red }, { VTColor.DarkCyan, Brushes.DarkCyan },
            { VTColor.DarkGreen, Brushes.LightGreen }, { VTColor.DarkMagenta, Brushes.DarkMagenta }, { VTColor.DarkRed, Brushes.DarkRed },
            { VTColor.DarkWhite, Brushes.White }, { VTColor.DarkYellow, Brushes.Yellow }
        };

        static DrawingUtils()
        {
        }

        public static Brush VTColor2Brush(VTColor color)
        {
            if (color == null) 
            {
                return null;
            }

            Brush brush;
            if (!VTColorBrushMap.TryGetValue(color, out brush))
            {
                brush = new SolidColorBrush(Color.FromRgb(color.R, color.G, color.B));
                VTColorBrushMap[color] = brush;
            }
            return brush;
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
            string text = textLine.Text;

            DrawingLine drawingLine = textLine.DrawingContext as DrawingLine;
            FormattedText formattedText = new FormattedText(text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, drawingLine.typeface,
                textLine.Style.FontSize, drawingLine.foreground, null, TextFormattingMode.Display, App.PixelsPerDip);

            #region 画文本装饰

            foreach (VTextAttribute textAttribute in textLine.Attributes)
            {
                int startIndex = textLine.FindCharacterIndex(textAttribute.StartColumn);
                if (startIndex < 0)
                {
                    // 有可能会找不到对应的字符索引
                    continue;
                }

                int count = text.Length - startIndex; // 剩余字符数

                switch (textAttribute.Decoration)
                {
                    case VTextDecorations.Foreground:
                        {
                            VTColor color = textAttribute.Parameter as VTColor;
                            Brush brush = DrawingUtils.VTColor2Brush(color);
                            //logger.ErrorFormat("Foreground startIndex = {0}, count = {1}, total = {2}", startIndex, count, text.Length);
                            formattedText.SetForegroundBrush(brush, startIndex, count);
                            break;
                        }
                    case VTextDecorations.ForegroundRGB:
                        {
                            if (textAttribute.Parameter == null) 
                            {
                                continue;
                            }

                            VTColor color = textAttribute.Parameter as VTColor;
                            Brush brush = DrawingUtils.VTColor2Brush(color);
                            formattedText.SetForegroundBrush(brush, startIndex, count);
                            break;
                        }
                    case VTextDecorations.ForegroundUnset:
                        {
                            //logger.ErrorFormat("ForegroundUnset startIndex = {0}, count = {1}, total = {2}, {3}", startIndex, count, text.Length, textAttribute.StartColumn);
                            formattedText.SetForegroundBrush(drawingLine.foreground, startIndex, count);
                            break;
                        }

                    case VTextDecorations.BackgroundRGB:
                    case VTextDecorations.Background:
                    case VTextDecorations.BackgroundUnset:
                        {
                            // 背景颜色最后画, 因为文本的粗细会影响到背景颜色的大小
                            break;
                        }

                    case VTextDecorations.Bold:
                        {
                            //formattedText.SetFontWeight(FontWeights.Bold, startIndex, count);
                            break;
                        }

                    case VTextDecorations.Italics:
                        {
                            formattedText.SetFontStyle(FontStyles.Italic, startIndex, count);
                            break;
                        }

                    case VTextDecorations.Underline:
                        {
                            formattedText.SetTextDecorations(TextDecorations.Underline, startIndex, count);
                            break;
                        }

                    default:
                        break;
                }
            }

            #endregion

            //if (dc != null)
            //{
            //    // 画背景颜色
            //    // 背景颜色要在最后画，因为文本的粗细会影响到背景颜色的大小
            //    foreach (VTextAttribute textAttribute in textLine.Attributes)
            //    {
            //        if (textAttribute.Decoration != VTextDecorations.Background &&
            //            textAttribute.Decoration != VTextDecorations.BackgroundRGB &&
            //            textAttribute.Decoration != VTextDecorations.BackgroundUnset)
            //        {
            //            continue;
            //        }

            //        int startIndex = textLine.FindCharacterIndex(textAttribute.StartColumn);
            //        if (startIndex < 0)
            //        {
            //            continue;
            //        }

            //        int count = text.Length - startIndex;
            //        Brush brush = null;

            //        if (textAttribute.Decoration == VTextDecorations.Background)
            //        {
            //            VTColor color = textAttribute.Parameter as VTColor;
            //            brush = DrawingUtils.VTColor2Brush(color);
            //        }
            //        else if (textAttribute.Decoration == VTextDecorations.BackgroundRGB)
            //        {
            //            VTColor color = textAttribute.Parameter as VTColor;
            //            brush = DrawingUtils.VTColor2Brush(color);
            //        }
            //        else if(textAttribute.Decoration == VTextDecorations.BackgroundUnset)
            //        {
            //            brush = Brushes.Black;
            //        }

            //        Geometry geometry = formattedText.BuildHighlightGeometry(ZeroPoint, startIndex, count);
            //        dc.DrawRectangle(brush, DrawingUtils.TransparentPen, geometry.Bounds);
            //    }
            //}

            return formattedText;
        }
    }
}

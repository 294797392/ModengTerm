using ModengTerm.Terminal;
using ModengTerm.Terminal.Document;
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

            { VTColor.DarkBlack, Brushes.Black }, { VTColor.DarkBlue, Brushes.DarkBlue }, { VTColor.DarkCyan, Brushes.DarkCyan },
            { VTColor.DarkGreen, Brushes.DarkGreen }, { VTColor.DarkMagenta, Brushes.DarkMagenta }, { VTColor.DarkRed, Brushes.DarkRed },
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
            VTextData textData = textLine.BuildData();

            // 如果文本是空的那么无法测量出来高度
            // 空白字符可以测量出来高度
            if (string.IsNullOrEmpty(textData.Text))
            {
                textData.Text = " ";
            }

            DrawingLine drawingLine = textLine.DrawingContext as DrawingLine;
            FormattedText formattedText = new FormattedText(textData.Text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, drawingLine.typeface,
                textLine.Style.FontSize, drawingLine.foreground, null, TextFormattingMode.Display, App.PixelsPerDip);

            #region 画文本装饰

            foreach (VTextAttribute textAttribute in textData.Attributes)
            {
                switch (textAttribute.Attribute)
                {
                    case VTextAttributes.Foreground:
                        {
                            VTColor color = textAttribute.Parameter as VTColor;
                            Brush brush = DrawingUtils.VTColor2Brush(color);
                            formattedText.SetForegroundBrush(brush, textAttribute.StartIndex, textAttribute.Count);
                            break;
                        }

                    case VTextAttributes.Background:
                        {
                            // 背景颜色最后画, 因为文本的粗细会影响到背景颜色的大小
                            break;
                        }

                    case VTextAttributes.Bold:
                        {
                            //formattedText.SetFontWeight(FontWeights.Bold, startIndex, count);
                            break;
                        }

                    case VTextAttributes.Italics:
                        {
                            formattedText.SetFontStyle(FontStyles.Italic, textAttribute.StartIndex, textAttribute.Count);
                            break;
                        }

                    case VTextAttributes.Underline:
                        {
                            formattedText.SetTextDecorations(TextDecorations.Underline, textAttribute.StartIndex, textAttribute.Count);
                            break;
                        }

                    default:
                        break;
                }
            }

            #endregion

            if (dc != null)
            {
                // 画背景颜色
                // 背景颜色要在最后画，因为文本的粗细会影响到背景颜色的大小
                foreach (VTextAttribute textAttribute in textData.Attributes)
                {
                    if (textAttribute.Attribute != VTextAttributes.Background)
                    {
                        continue;
                    }

                    VTColor color = textAttribute.Parameter as VTColor;
                    Brush brush = DrawingUtils.VTColor2Brush(color);
                    Geometry geometry = formattedText.BuildHighlightGeometry(ZeroPoint, textAttribute.StartIndex, textAttribute.Count);
                    dc.DrawRectangle(brush, DrawingUtils.TransparentPen, geometry.Bounds);
                }
            }

            return formattedText;
        }
    }
}

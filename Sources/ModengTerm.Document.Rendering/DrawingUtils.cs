using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ModengTerm.Document.Rendering
{
    public static class DrawingUtils
    {
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger("DrawingUtils");

        public static readonly Point ZeroPoint = new Point();
        private static readonly string[] Splitter = new string[] { "," };

        public static double PixelPerDpi = 0;

        /// <summary>
        /// RGB字符串 -> Brush
        /// </summary>
        private static readonly Dictionary<string, Brush> BrushMap = new Dictionary<string, Brush>();
        private static readonly Dictionary<string, Typeface> TypefaceMap = new Dictionary<string, Typeface>();

        static DrawingUtils()
        {
            PixelPerDpi = VisualTreeHelper.GetDpi(new System.Windows.Controls.Control()).PixelsPerDip;
        }

        public static void UpdateTextMetrics(VTextLine textLine, FormattedText formattedText)
        {
            textLine.Metrics.Width = formattedText.WidthIncludingTrailingWhitespace;
            textLine.Metrics.Height = formattedText.Height;
        }

        public static Typeface GetTypeface(string fontFamily)
        {
            Typeface typeface;
            if (!TypefaceMap.TryGetValue(fontFamily, out typeface))
            {
                FontFamily family = new FontFamily(fontFamily);
                typeface = new Typeface(family, FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
                TypefaceMap[fontFamily] = typeface;
            }
            return typeface;
        }

        /// <summary>
        /// rgb字符串转Brush
        /// </summary>
        /// <param name="rgb">
        /// 用逗号分隔的rgb字符串
        /// 例如：255,255,255
        /// </param>
        /// <returns></returns>
        public static Brush GetBrush(string rgbKey)
        {
            Brush brush;
            if (!BrushMap.TryGetValue(rgbKey, out brush))
            {
                string[] values = rgbKey.Split(Splitter, StringSplitOptions.RemoveEmptyEntries);

                byte r = byte.Parse(values[0]);
                byte g = byte.Parse(values[1]);
                byte b = byte.Parse(values[2]);

                Color color = Color.FromRgb(r, g, b);
                brush = new SolidColorBrush(color);
                BrushMap[rgbKey] = brush;
            }

            return brush;
        }

        public static Color GetColor(string rgbKey) 
        {
            SolidColorBrush brush = GetBrush(rgbKey) as SolidColorBrush;
            return brush.Color;
        }

        public static string GetRgbKey(Color color) 
        {
            return string.Format("{0},{1},{2}", color.R, color.G, color.B);
        }

        /// <summary>
        /// 根据VTextLine生成一个FormattedText
        /// </summary>
        /// <param name="textLine"></param>
        /// <param name="dc">画Background的时候需要</param>
        /// <returns></returns>
        public static FormattedText CreateFormattedText(VTFormattedText textData, DrawingContext dc = null)
        {
            VTypeface textStyle = textData.Style;
            Brush foreground = DrawingUtils.GetBrush(textStyle.ForegroundColor);
            Typeface typeface = DrawingUtils.GetTypeface(textStyle.FontFamily);

            FormattedText formattedText = new FormattedText(textData.Text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeface,
                textStyle.FontSize, foreground, null, TextFormattingMode.Display, PixelPerDpi);

            #region 画文本装饰

            foreach (VTextAttribute textAttribute in textData.Attributes)
            {
                switch (textAttribute.Attribute)
                {
                    case VTextAttributes.Foreground:
                        {
                            VTColor color = textAttribute.Parameter as VTColor;
                            Brush brush = DrawingUtils.GetBrush(color.Key);
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
                    Brush brush = DrawingUtils.GetBrush(color.Key);
                    Geometry geometry = formattedText.BuildHighlightGeometry(new Point(textData.OffsetX, textData.OffsetY), textAttribute.StartIndex, textAttribute.Count);
                    dc.DrawRectangle(brush, null, geometry.Bounds);
                }
            }

            return formattedText;
        }
    }
}

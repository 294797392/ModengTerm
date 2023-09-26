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

        public static readonly Point ZeroPoint = new Point();
        private static readonly string[] Splitter = new string[] { "," };

        public static readonly Pen TransparentPen = new Pen(Brushes.Transparent, 0);
        public static readonly Pen BlackPen = new Pen(Brushes.Black, 1);

        /// <summary>
        /// RGB字符串 -> Brush
        /// </summary>
        private static readonly Dictionary<string, Brush> BrushMap = new Dictionary<string, Brush>();
        private static readonly Dictionary<string, Typeface> TypefaceMap = new Dictionary<string, Typeface>();

        static DrawingUtils()
        {
        }

        public static void UpdateTextMetrics(VTextLine textLine, FormattedText formattedText)
        {
            textLine.Metrics.Width = formattedText.WidthIncludingTrailingWhitespace;
            textLine.Metrics.Height = formattedText.Height;
        }

        public static VTKeys ConvertToVTKey(Key key)
        {
            switch (key)
            {
                case Key.System:
                    {
                        return VTKeys.F10;
                    }

                default:
                    {
                        return (VTKeys)key;
                    }
            }
        }

        public static Typeface GetTypeface(VTextStyle textStyle)
        {
            Typeface typeface;
            if (!TypefaceMap.TryGetValue(textStyle.FontFamily, out typeface))
            {
                FontFamily fontFamily = new FontFamily(textStyle.FontFamily);
                typeface = new Typeface(fontFamily, FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
                TypefaceMap[textStyle.FontFamily] = typeface;
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

        /// <summary>
        /// 根据VTextLine生成一个FormattedText
        /// </summary>
        /// <param name="textLine"></param>
        /// <param name="dc">画Background的时候需要</param>
        /// <returns></returns>
        public static FormattedText CreateFormattedText(VTextLine textLine, DrawingContext dc = null)
        {
            VTFormattedText textData = VTUtils.CreateFormattedText(textLine.Characters);

            // 如果文本是空的那么无法测量出来高度
            // 空白字符可以测量出来高度
            if (string.IsNullOrEmpty(textData.Text))
            {
                textData.Text = " ";
            }

            //textData.Text = string.Format("{0} - {1}", textLine.PhysicsRow, textData.Text);

            textData.Style = textLine.Style;

            return DrawingUtils.CreateFormattedText(textData, dc);
        }

        /// <summary>
        /// 根据VTextLine生成一个FormattedText
        /// </summary>
        /// <param name="textBlock"></param>
        /// <param name="dc">画Background的时候需要</param>
        /// <returns></returns>
        public static FormattedText CreateFormattedText(VTFormattedText textBlock, DrawingContext dc = null)
        {
            VTextStyle textStyle = textBlock.Style;
            Brush foreground = DrawingUtils.GetBrush(textStyle.Foreground);
            Typeface typeface = DrawingUtils.GetTypeface(textStyle);

            FormattedText formattedText = new FormattedText(textBlock.Text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeface,
                textStyle.FontSize, foreground, null, TextFormattingMode.Display, App.PixelsPerDip);

            #region 画文本装饰

            foreach (VTextAttribute textAttribute in textBlock.Attributes)
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
                foreach (VTextAttribute textAttribute in textBlock.Attributes)
                {
                    if (textAttribute.Attribute != VTextAttributes.Background)
                    {
                        continue;
                    }

                    VTColor color = textAttribute.Parameter as VTColor;
                    Brush brush = DrawingUtils.GetBrush(color.Key);
                    Geometry geometry = formattedText.BuildHighlightGeometry(new Point(textBlock.OffsetX, textBlock.OffsetY), textAttribute.StartIndex, textAttribute.Count);
                    dc.DrawRectangle(brush, null, geometry.Bounds);
                }
            }

            return formattedText;
        }
    }
}

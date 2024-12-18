﻿using ModengTerm.Document;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace ModengTerm.UserControls.TerminalUserControls.Rendering
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
                byte a = byte.Parse(values[3]);

                Color color = Color.FromArgb(a, r, g, b);
                brush = new SolidColorBrush(color);
                brush.Freeze(); // 冻结对象可以提升性能，冻结之后该对象的属性不可以修改
                BrushMap[rgbKey] = brush;
            }

            return brush;
        }

        public static Brush GetBrush(VTColor vtColor)
        {
            string rgbKey = vtColor.Key;
            return DrawingUtils.GetBrush(rgbKey);
        }

        public static Pen GetPen(VTPen vtPen)
        {
            Pen pen = new Pen(DrawingUtils.GetBrush(vtPen.Color), vtPen.Width);
            pen.Freeze();
            return pen;
        }

        public static VTKeys ConvertToVTKey(Key key)
        {
            return (VTKeys)key;
        }

        public static VTModifierKeys ConvertToVTModifierKeys(ModifierKeys modifierKeys)
        {
            if (modifierKeys == ModifierKeys.None) 
            {
                return VTModifierKeys.None;
            }
            else if (modifierKeys.HasFlag(ModifierKeys.Alt))
            {
                return VTModifierKeys.MetaAlt;
            }
            else if (modifierKeys.HasFlag(ModifierKeys.Control))
            {
                return VTModifierKeys.Control;
            }
            else if (modifierKeys.HasFlag(ModifierKeys.Shift))
            {
                return VTModifierKeys.Shift;
            }

            return VTModifierKeys.None;
        }

        public static Color GetColor(string rgbKey) 
        {
            SolidColorBrush brush = GetBrush(rgbKey) as SolidColorBrush;
            return brush.Color;
        }

        public static string GetRgbKey(Color color) 
        {
            return string.Format("{0},{1},{2},{3}", color.R, color.G, color.B, color.A);
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
                    System.Windows.Media.Geometry geometry = formattedText.BuildHighlightGeometry(new Point(textData.OffsetX, textData.OffsetY), textAttribute.StartIndex, textAttribute.Count);
                    dc.DrawRectangle(brush, null, geometry.Bounds);
                }
            }

            return formattedText;
        }


        ///// <summary>
        ///// 当Wallpaper是动态图的时候，获取动态图的元数据，用来实时渲染
        ///// </summary>
        ///// <param name="uri"></param>
        ///// <returns></returns>
        //public static GifMetadata GetWallpaperMetadata(string uri)
        //{
        //    GifMetadata gifMetadata;
        //    if (!GifMetadataMap.TryGetValue(uri, out gifMetadata))
        //    {
        //        Stream stream = GetWallpaperStream(uri);
        //        if (stream == null)
        //        {
        //            return new GifMetadata();
        //        }

        //        gifMetadata = GifParser.GetFrames(uri, stream);
        //        GifMetadataMap[uri] = gifMetadata;
        //    }
        //    return gifMetadata;
        //}

        ///// <summary>
        ///// 获取动态背景或静态背景的预览图
        ///// </summary>
        ///// <param name="paperType">标识是静态图还是动态图</param>
        ///// <param name="uri">背景图的路径</param>
        ///// <returns></returns>
        //public static BitmapSource GetWallpaperThumbnail(WallpaperTypeEnum paperType, string uri)
        //{
        //    switch (paperType)
        //    {
        //        case WallpaperTypeEnum.Image:
        //            {
        //                return GetWallpaperBitmap(uri, 200, 200);
        //            }

        //        case WallpaperTypeEnum.Live:
        //            {
        //                Stream stream = VTUtils.GetWallpaperStream(uri);
        //                if (stream == null)
        //                {
        //                    throw new NotImplementedException();
        //                }

        //                return GifParser.GetThumbnail(stream);
        //            }

        //        default:
        //            throw new NotImplementedException();
        //    }
        //}

        ///// <summary>
        ///// 当Wallpaper是静态图的时候，获取静态图
        ///// </summary>
        ///// <param name="uri"></param>
        ///// <param name="pixelWidth">设置解码后的图像宽度，减少这个值可以减少内存占用</param>
        ///// <param name="pixelHeight">设置解码后的图像高度，减少这个值可以减少内存占用</param>
        ///// <returns></returns>
        //public static BitmapSource GetWallpaperBitmap(string uri, int pixelWidth = 0, int pixelHeight = 0)
        //{
        //    Stream stream = VTUtils.GetWallpaperStream(uri);
        //    if (stream == null)
        //    {
        //        throw new NotImplementedException();
        //    }

        //    BitmapImage bitmapImage = new BitmapImage();
        //    bitmapImage.BeginInit();
        //    bitmapImage.DecodePixelHeight = pixelHeight;
        //    bitmapImage.DecodePixelWidth = pixelWidth;
        //    bitmapImage.StreamSource = stream;
        //    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
        //    bitmapImage.EndInit();

        //    return bitmapImage;
        //}
    }
}

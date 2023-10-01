﻿using ModengTerm.Base;
using ModengTerm.Base.Enumerations;
using ModengTerm.Rendering;
using ModengTerm.Terminal;
using ModengTerm.Terminal.Document;
using ModengTerm.Terminal.Enumerations;
using ModengTerm.Terminal.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using XTerminal.Base.Definitions;
using XTerminal.Base.Enumerations;

namespace ModengTerm
{
    public static class EnumConverterUtils
    {
        public static bool CheckValue(object value)
        {
            if (value == null)
            {
                return false;
            }

            string v = value.ToString();
            if (string.IsNullOrEmpty(v))
            {
                return false;
            }

            return true;
        }
    }

    public class SSHAuthTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!EnumConverterUtils.CheckValue(value))
            {
                return "未知";
            }

            switch ((SSHAuthTypeEnum)value)
            {
                case SSHAuthTypeEnum.None: return "不需要身份验证";
                case SSHAuthTypeEnum.Password: return "用户名和密码";
                case SSHAuthTypeEnum.PrivateKey: return "密钥验证";
                default:
                    throw new NotImplementedException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class CloseSessionButtonColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool selected = (bool)value;
            return selected ? Brushes.White : Brushes.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class SessionTypeStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return "未知类型";
            }

            SessionTypeEnum type = (SessionTypeEnum)value;

            switch (type)
            {
                case SessionTypeEnum.SerialPort: return "串口";
                case SessionTypeEnum.libvtssh:
                case SessionTypeEnum.SSH: return "SSH";
                case SessionTypeEnum.Win32CommandLine: return "命令行";
                case SessionTypeEnum.SFTP: return "SFTP";

                default:
                    throw new NotImplementedException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class VTCursorStyle2StringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!EnumConverterUtils.CheckValue(value))
            {
                return "未知";
            }

            switch ((VTCursorStyles)value)
            {
                case VTCursorStyles.Block: return "矩形块";
                case VTCursorStyles.Line: return "竖线";
                case VTCursorStyles.None: return "无";
                case VTCursorStyles.Underscore: return "下划线";
                default:
                    throw new NotImplementedException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class VTCursorSpeed2StringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!EnumConverterUtils.CheckValue(value))
            {
                return "未知";
            }

            switch ((VTCursorSpeeds)value)
            {
                case VTCursorSpeeds.HighSpeed: return "快";
                case VTCursorSpeeds.LowSpeed: return "慢";
                case VTCursorSpeeds.NormalSpeed: return "普通";
                default:
                    throw new NotImplementedException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class FindScopes2StringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!EnumConverterUtils.CheckValue(value))
            {
                return "未知";
            }

            switch ((FindScopes)value)
            {
                case FindScopes.All: return "查找全部";
                case FindScopes.Document: return "查找当前文档";
                default:
                    throw new NotImplementedException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class FindStartups2StringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!EnumConverterUtils.CheckValue(value))
            {
                return "未知";
            }

            switch ((FindStartups)value)
            {
                case FindStartups.FromBegin: return "从头开始查找";
                case FindStartups.FromEnd: return "从结尾开始查找";
                case FindStartups.CurrentToEnd: return "从当前位置往下查找";
                case FindStartups.CurrentToBegin: return "从当前位置往上查找";
                default:
                    throw new NotImplementedException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class WallpaperType2StringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!EnumConverterUtils.CheckValue(value))
            {
                return "未知";
            }

            switch ((WallpaperTypeEnum)value)
            {
                case WallpaperTypeEnum.Live: return "动态背景";
                case WallpaperTypeEnum.Color: return "纯色";
                case WallpaperTypeEnum.Image: return "图片";
                default:
                    throw new NotImplementedException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class EffectType2StringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!EnumConverterUtils.CheckValue(value))
            {
                return "未知";
            }

            switch ((EffectTypeEnum)value)
            {
                case EffectTypeEnum.None: return "无";
                case EffectTypeEnum.Snow: return "飘雪";
                case EffectTypeEnum.Star: return "星空";
                default:
                    throw new NotImplementedException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ColorDefinition2BrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ColorDefinition colorDefinition = value as ColorDefinition;
            if (colorDefinition == null)
            {
                return Brushes.Transparent;
            }

            return DrawingUtils.GetBrush(colorDefinition.Value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class LivePaperBrushConverter : IValueConverter
    {
        private static readonly Dictionary<string, Brush> BrushCache = new Dictionary<string, Brush>();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ColorDefinition colorDefinition = value as ColorDefinition;
            if (colorDefinition == null)
            {
                return Brushes.Transparent;
            }

            string uri = colorDefinition.Uri;

            Brush brush;
            if (!BrushCache.TryGetValue(uri, out brush))
            {
                WallpaperTypeEnum paperType = (WallpaperTypeEnum)parameter;

                ImageSource imageSource = VTUtils.GetWallpaperThumbnail(paperType, uri);
                ImageBrush imageBrush = new ImageBrush(imageSource);
                BrushCache[uri] = imageBrush;
                brush = imageBrush;
            }
            return brush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
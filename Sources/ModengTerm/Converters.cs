using ModengTerm.Base;
using ModengTerm.Base.Enumerations;
using ModengTerm.Base.Enumerations.Terminal;
using ModengTerm.Document;
using ModengTerm.Document.Enumerations;
using ModengTerm.Document.Rendering;
using ModengTerm.Terminal;
using ModengTerm.Terminal.Enumerations;
using ModengTerm.Terminal.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using XTerminal.Base.Definitions;
using XTerminal.Base.Enumerations;

namespace ModengTerm
{
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
                case SessionTypeEnum.SSH: return "SSH";
                case SessionTypeEnum.CommandLine: return "命令行";
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
                //case WallpaperTypeEnum.Live: return "动态背景";
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

            //switch ((EffectTypeEnum)value)
            //{
            //    case EffectTypeEnum.None: return "无";
            //    case EffectTypeEnum.Snow: return "飘雪";
            //    case EffectTypeEnum.Star: return "星空";
            //    default:
            //        throw new NotImplementedException();
            //}
            return string.Empty;
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
            //ColorDefinition colorDefinition = value as ColorDefinition;
            //if (colorDefinition == null)
            //{
            //    return Brushes.Transparent;
            //}

            //string uri = colorDefinition.Uri;

            //Brush brush;
            //if (!BrushCache.TryGetValue(uri, out brush))
            //{
            //    WallpaperTypeEnum paperType = (WallpaperTypeEnum)parameter;

            //    ImageSource imageSource = .GetWallpaperThumbnail(paperType, uri);
            //    ImageBrush imageBrush = new ImageBrush(imageSource);
            //    BrushCache[uri] = imageBrush;
            //    brush = imageBrush;
            //}
            //return brush;
            return Brushes.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class SessionStatus2VisibleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            SessionStatusEnum visibleStatus = (SessionStatusEnum)parameter;
            SessionStatusEnum currentStatus = (SessionStatusEnum)value;
            return visibleStatus == currentStatus ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class SessionImageConverter : IValueConverter
    {
        public static ImageSource SSHImage = WPFToolkit.Utility.ImageUtility.FromURI("pack://application:,,,/ModengTerm;component/Images/ssh.png");
        public static ImageSource HostCommandLineImage = WPFToolkit.Utility.ImageUtility.FromURI("pack://application:,,,/ModengTerm;component/Images/cmdline.png");
        public static ImageSource SerialPortImage = WPFToolkit.Utility.ImageUtility.FromURI("pack://application:,,,/ModengTerm;component/Images/serialport.png");

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            SessionTypeEnum sessionType = (SessionTypeEnum)value;

            switch (sessionType)
            {
                case SessionTypeEnum.SerialPort:
                    {
                        return SerialPortImage;
                    }

                case SessionTypeEnum.SSH:
                    {
                        return SSHImage;
                    }

                case SessionTypeEnum.CommandLine:
                    {
                        return HostCommandLineImage;
                    }

                default:
                    throw new NotImplementedException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BehaviorRightClicksConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is BehaviorRightClicks))
            {
                return string.Empty;
            }

            BehaviorRightClicks brc = (BehaviorRightClicks)value;

            switch (brc)
            {
                case BehaviorRightClicks.ContextMenu:
                    {
                        return "显示上下文菜单";
                    }

                case BehaviorRightClicks.FastCopyPaste:
                    {
                        return "快速复制粘贴";
                    }

                case BehaviorRightClicks.None:
                    {
                        return "什么都不做";
                    }

                default:
                    throw new NotImplementedException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ShellCommandTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is CommandTypeEnum))
            {
                return string.Empty;
            }

            CommandTypeEnum commandType = (CommandTypeEnum)value;

            switch (commandType)
            {
                case CommandTypeEnum.PureText: return "纯文本";
                case CommandTypeEnum.HexData: return "十六进制数据（如有多个数值，请使用空格分开）";
                default: throw new NotImplementedException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class RenderModeTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is RenderModeEnum))
            {
                return string.Empty;
            }

            RenderModeEnum mode = (RenderModeEnum)value;

            switch (mode)
            {
                case RenderModeEnum.Default: return "默认";
                case RenderModeEnum.Hexdump: return "十六进制";
                default:
                    throw new NotImplementedException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

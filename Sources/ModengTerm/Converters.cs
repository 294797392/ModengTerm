using ModengTerm.Base;
using ModengTerm.Base.Enumerations;
using ModengTerm.Base.Enumerations.Terminal;
using ModengTerm.Document;
using ModengTerm.Document.Enumerations;
using ModengTerm.Terminal.Enumerations;
using ModengTerm.Themes;
using ModengTerm.UserControls.TerminalUserControls.Rendering;
using ModengTerm.ViewModels.Session;
using ModengTerm.ViewModels.Sessions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
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
                case SessionTypeEnum.WindowsConsole: return "命令行";
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

    public class SessionImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            SessionTypeEnum sessionType = (SessionTypeEnum)value;
            return ThemeManager.GetSessionTypeImageSource(sessionType);
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

    public class PortForwardTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return string.Empty;
            }

            if (!(value is PortForwardTypeEnum) && !(value is int))
            {
                return string.Empty;
            }

            PortForwardTypeEnum type = (PortForwardTypeEnum)value;

            switch (type)
            {
                case PortForwardTypeEnum.Local: return "本地转发";
                case PortForwardTypeEnum.Dynamic: return "动态转发";
                case PortForwardTypeEnum.Remote: return "远程转发";
                default:
                    throw new NotImplementedException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class RawTcpTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return string.Empty;
            }

            if (!(value is RawTcpTypeEnum) && !(value is int))
            {
                return string.Empty;
            }

            RawTcpTypeEnum type = (RawTcpTypeEnum)value;

            switch (type)
            {
                case RawTcpTypeEnum.Client: return "客户端";
                case RawTcpTypeEnum.Server: return "服务器";
                default:
                    throw new NotImplementedException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class RecordStatusVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            RecordStatusEnum targetStatus = (RecordStatusEnum)parameter;
            RecordStatusEnum currentStatus = (RecordStatusEnum)value;

            if (targetStatus == currentStatus)
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class RecordStatus2TextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            RecordStatusEnum currentStatus = (RecordStatusEnum)value;

            switch (currentStatus)
            {
                case RecordStatusEnum.Stop: return "未录制";
                case RecordStatusEnum.Recording: return "录制中";
                default:
                    throw new NotImplementedException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class SessionStatus2TextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            SessionStatusEnum sessionStatus = (SessionStatusEnum)value;

            switch (sessionStatus)
            {
                case SessionStatusEnum.Connected: return "已连接";
                case SessionStatusEnum.Connecting: return "连接中";
                case SessionStatusEnum.ConnectError: return "连接失败";
                case SessionStatusEnum.Disconnected: return "连接断开";
                default:
                    throw new NotImplementedException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class SessionStatus2BrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            SessionStatusEnum sessionStatus = (SessionStatusEnum)value;

            switch (sessionStatus)
            {
                case SessionStatusEnum.Connected: return Brushes.Green;
                case SessionStatusEnum.Connecting: return Brushes.Orange;
                case SessionStatusEnum.ConnectError: return Brushes.DarkRed;
                case SessionStatusEnum.Disconnected: return Brushes.Red;
                default:
                    throw new NotImplementedException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class SessionStatus2ImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            SessionStatusEnum sessionStatus = (SessionStatusEnum)value;

            switch (sessionStatus)
            {
                case SessionStatusEnum.Connected: return ThemeManager.GetResource<ImageSource>("5001");
                case SessionStatusEnum.Connecting: return ThemeManager.GetResource<ImageSource>("5003");
                case SessionStatusEnum.ConnectError: return ThemeManager.GetResource<ImageSource>("5004");
                case SessionStatusEnum.Disconnected: return ThemeManager.GetResource<ImageSource>("5002");
                default:
                    throw new NotImplementedException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class SessionTreeNodeImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) 
            {
                return null;
            }

            SessionTreeNodeVM treeNode = value as SessionTreeNodeVM;

            switch (treeNode.NodeType)
            {
                case SessionTreeNodeTypeEnum.Session:
                    {
                        XTermSessionVM sessionVM = treeNode as XTermSessionVM;
                        return ThemeManager.GetSessionTypeImageSource(sessionVM.Type);
                    }

                case SessionTreeNodeTypeEnum.Group: return ThemeManager.GetResource<ImageSource>("5021");
                case SessionTreeNodeTypeEnum.GobackGroup: return ThemeManager.GetResource<ImageSource>("5026");
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

using ModengTerm.Addon.Interactive;
using ModengTerm.Base;
using ModengTerm.Base.Addon;
using ModengTerm.Base.DataModels;
using ModengTerm.Base.Definitions;
using ModengTerm.Base.Enumerations;
using ModengTerm.Base.Enumerations.Ssh;
using ModengTerm.Base.Enumerations.Terminal;
using ModengTerm.Document;
using ModengTerm.Document.Enumerations;
using ModengTerm.FileTrans;
using ModengTerm.FileTrans.Enumerations;
using ModengTerm.Ftp.Enumerations;
using ModengTerm.Terminal.Enumerations;
using ModengTerm.Themes;
using ModengTerm.UserControls.TerminalUserControls;
using ModengTerm.ViewModel.Ftp;
using ModengTerm.ViewModel.Session;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using WPFToolkit.MVVM;

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
                case VTCursorSpeeds.High: return "快";
                case VTCursorSpeeds.Low: return "慢";
                case VTCursorSpeeds.Normal: return "普通";
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
            if (!(value is RightClickActions))
            {
                return string.Empty;
            }

            RightClickActions brc = (RightClickActions)value;

            switch (brc)
            {
                case RightClickActions.ContextMenu:
                    {
                        return "显示上下文菜单";
                    }

                case RightClickActions.FastCopyPaste:
                    {
                        return "快速复制粘贴";
                    }

                case RightClickActions.None:
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
                case SessionStatusEnum.Connected: return ThemeManager.GetResource<ImageSource>("5520");
                case SessionStatusEnum.Connecting: return ThemeManager.GetResource<ImageSource>("5522");
                case SessionStatusEnum.ConnectError: return ThemeManager.GetResource<ImageSource>("5523");
                case SessionStatusEnum.Disconnected: return ThemeManager.GetResource<ImageSource>("5521");
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

                case SessionTreeNodeTypeEnum.Group: return ThemeManager.GetResource<ImageSource>("500");
                case SessionTreeNodeTypeEnum.GobackGroup: return ThemeManager.GetResource<ImageSource>("505");
                default:
                    throw new NotImplementedException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class SessionTreeNodeTypeNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return string.Empty;
            }

            SessionTreeNodeVM treeNode = value as SessionTreeNodeVM;

            switch (treeNode.NodeType)
            {
                case SessionTreeNodeTypeEnum.Session:
                    {
                        XTermSessionVM sessionVM = treeNode as XTermSessionVM;

                        SessionTypeEnum type = (SessionTypeEnum)sessionVM.Type;

                        switch (type)
                        {
                            case SessionTypeEnum.SerialPort: return "串口";
                            case SessionTypeEnum.Ssh: return "SSH";
                            case SessionTypeEnum.Console: return "命令行";
                            case SessionTypeEnum.Sftp: return "SFTP";
                            case SessionTypeEnum.Tcp: return "Tcp";

                            default:
                                throw new NotImplementedException();
                        }

                    }

                case SessionTreeNodeTypeEnum.Group: return "分组";
                case SessionTreeNodeTypeEnum.GobackGroup: return "返回";
                default:
                    throw new NotImplementedException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class WatchFrequencyTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is WatchFrequencyEnum))
            {
                return string.Empty;
            }

            WatchFrequencyEnum commandType = (WatchFrequencyEnum)value;

            switch (commandType)
            {
                case WatchFrequencyEnum.Normal: return "正常";
                case WatchFrequencyEnum.Low: return "低";
                case WatchFrequencyEnum.High: return "高";
                default:
                    throw new NotImplementedException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class LineTerminator2TextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is LineTerminators))
            {
                return string.Empty;
            }

            LineTerminators lineTerminators = (LineTerminators)value;

            switch (lineTerminators)
            {
                case LineTerminators.None: return "None";
                case LineTerminators.CR: return "CR";
                case LineTerminators.LF: return "LF";
                case LineTerminators.CRLF: return "CRLF";
                default:
                    throw new NotImplementedException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class OverlayPanelDock2HorizontalAlignmentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            OverlayPanelDocks dock = (OverlayPanelDocks)value;

            switch (dock)
            {
                case OverlayPanelDocks.FillBottom: return HorizontalAlignment.Stretch;
                case OverlayPanelDocks.FillTop: return HorizontalAlignment.Stretch;
                case OverlayPanelDocks.LeftTop: return HorizontalAlignment.Left;
                case OverlayPanelDocks.LeftBottom: return HorizontalAlignment.Left;
                case OverlayPanelDocks.RightBottom: return HorizontalAlignment.Right;
                case OverlayPanelDocks.RightTop: return HorizontalAlignment.Right;
                default:
                    throw new NotImplementedException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class OverlayPanelDock2VerticalAlignmentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            OverlayPanelDocks dock = (OverlayPanelDocks)value;

            switch (dock)
            {
                case OverlayPanelDocks.FillBottom: return VerticalAlignment.Bottom;
                case OverlayPanelDocks.FillTop: return VerticalAlignment.Top;
                case OverlayPanelDocks.LeftTop: return VerticalAlignment.Top;
                case OverlayPanelDocks.LeftBottom: return VerticalAlignment.Bottom;
                case OverlayPanelDocks.RightBottom: return VerticalAlignment.Bottom;
                case OverlayPanelDocks.RightTop: return VerticalAlignment.Top;
                default:
                    throw new NotImplementedException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class TerminalSizeModeTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            TerminalSizeModeEnum sizeMode = (TerminalSizeModeEnum)value;

            switch (sizeMode)
            {
                case TerminalSizeModeEnum.Fixed: return "固定大小";
                case TerminalSizeModeEnum.AutoFit: return "自适应";
                default:
                    throw new NotImplementedException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class Win32ConsoleEngineTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Win32ConsoleEngineEnum consoleEngine = (Win32ConsoleEngineEnum)value;

            switch (consoleEngine)
            {
                case Win32ConsoleEngineEnum.Auto: return "自动选择";
                default:
                    return consoleEngine.ToString();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class FileItemDisplaySizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            long size = (long)value;

            double toValue;
            SizeUnitEnum toUnit;
            VTBaseUtils.AutoFitSize(size, SizeUnitEnum.bytes, out toValue, out toUnit);

            return string.Format("{0}{1}", toValue, VTBaseUtils.GetSizeUnitName(toUnit));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class FsItemTypeNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            FsItemTypeEnum type = (FsItemTypeEnum)value;

            switch (type)
            {
                case FsItemTypeEnum.File: return "文件";
                case FsItemTypeEnum.Directory: return "文件夹";
                case FsItemTypeEnum.ParentDirectory: return "上级目录";
                default:
                    throw new NotImplementedException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class FsItemType2SizeVisibleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            FsItemTypeEnum type = (FsItemTypeEnum)value;

            switch (type)
            {
                case FsItemTypeEnum.Directory: return Visibility.Collapsed;
                default:
                    return Visibility.Visible;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class FsOperationTypeNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            FsOperationTypeEnum type = (FsOperationTypeEnum)value;

            switch (type)
            {
                case FsOperationTypeEnum.UploadFile: return "上传文件";
                case FsOperationTypeEnum.DeleteFile: return "删除文件";
                case FsOperationTypeEnum.DeleteDirectory: return "删除目录";
                case FsOperationTypeEnum.CreateDirectory: return "新建目录";
                case FsOperationTypeEnum.DownloadFile: return "下载文件";
                default:
                    throw new NotImplementedException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ProcessStatesNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ProcessStates processStates = (ProcessStates)value;

            switch (processStates)
            {
                case ProcessStates.Starting:
                case ProcessStates.ProgressChanged: return "传输中";
                case ProcessStates.Failure: return "传输失败";
                case ProcessStates.Success: return "传输完成";
                case ProcessStates.WaitQueued: return "等待入队";
                case ProcessStates.Queued: return "已入队, 等待传输";
                default:
                    throw new NotImplementedException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class FsItemIsHidden2ForegroundConverter : IValueConverter
    {
        private static readonly Brush HiddenItemForeground = new SolidColorBrush(Color.FromArgb(150, 0, 0, 0));

        static FsItemIsHidden2ForegroundConverter()
        {
            HiddenItemForeground.Freeze();
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isHidden = (bool)value;

            if (isHidden)
            {
                return HiddenItemForeground;
            }
            else
            {
                return Brushes.Black;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}

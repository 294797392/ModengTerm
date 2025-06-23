using ModengTerm.Base;
using ModengTerm.Base.DataModels;
using ModengTerm.Base.DataModels.Terminal;
using ModengTerm.Base.Definitions;
using ModengTerm.Base.Enumerations;
using ModengTerm.Document;
using ModengTerm.Document.Enumerations;
using ModengTerm.Document.Graphics;
using ModengTerm.Document.Utility;
using ModengTerm.Terminal.Parsing;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace ModengTerm.Terminal
{
    public enum AdbPullResult
    {
        /// <summary>
        /// Pull成功
        /// </summary>
        Susccess,

        /// <summary>
        /// 设备内部不存在这个文件
        /// </summary>
        DeviceFileNotExist,

        AdbProcessException,

        /// <summary>
        /// 创建本地文件失败
        /// </summary>
        CreateLocalFileFailed,

        /// <summary>
        /// 未知的指令执行失败
        /// </summary>
        UnkownFailed
    }

    public enum AdbReadResult
    {
        /// <summary>
        /// Pull成功
        /// </summary>
        Susccess,

        /// <summary>
        /// 设备内部不存在这个文件
        /// </summary>
        DeviceFileNotExist,

        AdbProcessException,

        /// <summary>
        /// 创建本地文件失败
        /// </summary>
        CreateLocalFileFailed,

        /// <summary>
        /// 未知的指令执行失败
        /// </summary>
        UnkownFailed,

        /// <summary>
        /// 读取文件失败
        /// </summary>
        ReadFileFailed,
    }

    public static class VTermUtils
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("TermUtils");

        public static VTColorIndex GraphicsOptions2VTColorIndex(GraphicsOptions options)
        {
            switch (options)
            {
                case GraphicsOptions.ForegroundBlack: return VTColorIndex.DarkBlack;
                case GraphicsOptions.ForegroundBlue: return VTColorIndex.DarkBlue;
                case GraphicsOptions.ForegroundGreen: return VTColorIndex.DarkGreen;
                case GraphicsOptions.ForegroundCyan: return VTColorIndex.DarkCyan;
                case GraphicsOptions.ForegroundRed: return VTColorIndex.DarkRed;
                case GraphicsOptions.ForegroundMagenta: return VTColorIndex.DarkMagenta;
                case GraphicsOptions.ForegroundYellow: return VTColorIndex.DarkYellow;
                case GraphicsOptions.ForegroundWhite: return VTColorIndex.DarkWhite;

                case GraphicsOptions.BrightForegroundBlack: return VTColorIndex.BrightBlack;
                case GraphicsOptions.BrightForegroundBlue: return VTColorIndex.BrightBlue;
                case GraphicsOptions.BrightForegroundGreen: return VTColorIndex.BrightGreen;
                case GraphicsOptions.BrightForegroundCyan: return VTColorIndex.BrightCyan;
                case GraphicsOptions.BrightForegroundRed: return VTColorIndex.BrightRed;
                case GraphicsOptions.BrightForegroundMagenta: return VTColorIndex.BrightMagenta;
                case GraphicsOptions.BrightForegroundYellow: return VTColorIndex.BrightYellow;
                case GraphicsOptions.BrightForegroundWhite: return VTColorIndex.BrightWhite;

                case GraphicsOptions.BackgroundBlack: return VTColorIndex.DarkBlack;
                case GraphicsOptions.BackgroundBlue: return VTColorIndex.DarkBlue;
                case GraphicsOptions.BackgroundGreen: return VTColorIndex.DarkGreen;
                case GraphicsOptions.BackgroundCyan: return VTColorIndex.DarkCyan;
                case GraphicsOptions.BackgroundRed: return VTColorIndex.DarkRed;
                case GraphicsOptions.BackgroundMagenta: return VTColorIndex.DarkMagenta;
                case GraphicsOptions.BackgroundYellow: return VTColorIndex.DarkYellow;
                case GraphicsOptions.BackgroundWhite: return VTColorIndex.DarkWhite;

                case GraphicsOptions.BrightBackgroundBlack: return VTColorIndex.BrightBlack;
                case GraphicsOptions.BrightBackgroundBlue: return VTColorIndex.BrightBlue;
                case GraphicsOptions.BrightBackgroundGreen: return VTColorIndex.BrightGreen;
                case GraphicsOptions.BrightBackgroundCyan: return VTColorIndex.BrightCyan;
                case GraphicsOptions.BrightBackgroundRed: return VTColorIndex.BrightRed;
                case GraphicsOptions.BrightBackgroundMagenta: return VTColorIndex.BrightMagenta;
                case GraphicsOptions.BrightBackgroundYellow: return VTColorIndex.BrightYellow;
                case GraphicsOptions.BrightBackgroundWhite: return VTColorIndex.BrightWhite;

                default:
                    throw new NotImplementedException();
            }
        }


        public static VTPoint ToVTPoint(this Point wpfPoint)
        {
            return new VTPoint(wpfPoint.X, wpfPoint.Y);
        }

        public static VTSize ToVTSize(this Size wpfSize)
        {
            return new VTSize(wpfSize.Width, wpfSize.Height);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="vpw">文档可视区域的宽度, 不包含Padding</param>
        /// <param name="vph">文档可视区域的高度, 不包含Padding</param>
        /// <param name="sessionInfo"></param>
        /// <param name="graphicsInterface"></param>
        /// <returns></returns>
        public static VTDocumentOptions CreateDocumentOptions(string name, double vpw, double vph, XTermSession sessionInfo, GraphicsFactory graphicsInterface)
        {
            string fontFamily = sessionInfo.GetOption<string>(OptionKeyEnum.THEME_FONT_FAMILY);
            double fontSize = sessionInfo.GetOption<double>(OptionKeyEnum.THEME_FONT_SIZE);

            VTypeface typeface = graphicsInterface.GetTypeface(fontSize, fontFamily);
            typeface.BackgroundColor = sessionInfo.GetOption<string>(OptionKeyEnum.THEME_BACKGROUND_COLOR);
            typeface.ForegroundColor = sessionInfo.GetOption<string>(OptionKeyEnum.THEME_FONT_COLOR);

            VTSize displaySize = new VTSize(vpw, vph);
            TerminalSizeModeEnum sizeMode = sessionInfo.GetOption<TerminalSizeModeEnum>(OptionKeyEnum.SSH_TERM_SIZE_MODE);

            int viewportRow = sessionInfo.GetOption<int>(OptionKeyEnum.SSH_TERM_ROW);
            int viewportColumn = sessionInfo.GetOption<int>(OptionKeyEnum.SSH_TERM_COL);
            if (sizeMode == TerminalSizeModeEnum.AutoFit)
            {
                /// 如果SizeMode等于Fixed，那么就使用DefaultViewportRow和DefaultViewportColumn
                /// 如果SizeMode等于AutoFit，那么动态计算行和列
                VTDocUtils.CalculateAutoFitSize(displaySize, typeface, out viewportRow, out viewportColumn);
            }

            VTDocumentOptions documentOptions = new VTDocumentOptions()
            {
                Name = name,
                ViewportRow = viewportRow,
                ViewportColumn = viewportColumn,
                AutoWrapMode = false,
                CursorStyle = sessionInfo.GetOption<VTCursorStyles>(OptionKeyEnum.THEME_CURSOR_STYLE),
                CursorColor = sessionInfo.GetOption<string>(OptionKeyEnum.THEME_CURSOR_COLOR),
                CursorSpeed = sessionInfo.GetOption<VTCursorSpeeds>(OptionKeyEnum.THEME_CURSOR_SPEED),
                ScrollDelta = sessionInfo.GetOption<int>(OptionKeyEnum.MOUSE_SCROLL_DELTA),
                RollbackMax = sessionInfo.GetOption<int>(OptionKeyEnum.TERM_MAX_ROLLBACK),
                Typeface = typeface,
                GraphicsInterface = graphicsInterface,
                SelectionColor = sessionInfo.GetOption<string>(OptionKeyEnum.THEME_SELECTION_COLOR)
            };

            return documentOptions;
        }



        #region AdbUtility

        /// <summary>
        /// 确保Adb守护进程已启动
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public static bool StartAdbServer(XTermSession session) 
        {
            string exePath = session.GetOption<string>(OptionKeyEnum.WATCH_ADB_PATH);
            int timeout = session.GetOption<int>(OptionKeyEnum.WATCH_ADB_LOGIN_TIMEOUT, OptionDefaultValues.WATCH_ADB_LOGIN_TIMEOUT);

            ProcessStartInfo startInfo = new ProcessStartInfo()
            {
                FileName = exePath,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
                Arguments = "start-server"
            };

            try
            {
                Process process = Process.Start(startInfo);
                if (!process.WaitForExit(timeout))
                {
                    logger.ErrorFormat("启动adb server失败, 超时了, 可能端口被占用了");
                    // 进程超时了还没退出
                    // 此时表示启动失败
                    process.Kill();
                    process.Dispose();
                    return false;
                }
                else
                {
                    if (process.ExitCode != 0)
                    {
                        // 说明进程因为启动失败退出
                        // 比如5037端口被占用，adb尝试连接5037端口，然后占用5037端口的进程退出，此时exitCode不等于0
                        logger.ErrorFormat("启动adb server失败, exitCode = {0}", process.ExitCode);
                        return false;
                    }

                    logger.InfoFormat("adb server启动成功");

                    return true;
                }
            }
            catch (Exception ex)
            {
                logger.Error("启动adb server进程异常", ex);
                return false;
            }
        }

        /// <summary>
        /// 把设备上的文件拉取到本地
        /// </summary>
        /// <param name="adbExePath">adb可执行文件路径</param>
        /// <param name="remotePath">设备里的文件路径</param>
        /// <param name="localPath">要拷贝到的本地路径</param>
        /// <param name="message">adb输出的内容</param>
        /// <returns>pull指令是否执行成功</returns>
        public static AdbPullResult AdbPullFile(string adbExePath, string remotePath, string localPath, out string message)
        {
            message = string.Empty;

            //logger.InfoFormat("adb pull, remotePath = {0}, localPath = {1}", remotePath, localPath);

            try
            {
                if (!File.Exists(localPath))
                {
                    File.Create(localPath).Close();
                }
            }
            catch (Exception ex)
            {
                logger.Error("adb pull异常, 创建本地文件异常", ex);
                return AdbPullResult.CreateLocalFileFailed;
            }

            string pullCommand = string.Format("pull {0} {1}", remotePath, localPath);

            ProcessStartInfo psi = new ProcessStartInfo()
            {
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                FileName = adbExePath,
                Arguments = pullCommand
            };

            Process process = null;

            try
            {
                process = Process.Start(psi);
            }
            catch (Exception ex)
            {
                logger.Error("adb pull异常", ex);
                message = ex.Message;
                return AdbPullResult.AdbProcessException;
            }

            message = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            process.Dispose();

            if (message.Contains("remote object") && message.Contains("does not exist"))
            {
                return AdbPullResult.DeviceFileNotExist;
            }
            else if (message.Contains("file pulled"))
            {
                return AdbPullResult.Susccess;
            }
            else
            {
                logger.ErrorFormat("pull指令执行失败, {0}, {1}", pullCommand, message);
                return AdbPullResult.UnkownFailed;
            }
        }

        /// <summary>
        /// 读取设备里的文件内容并返回
        /// 为了防止有些系统需要登录才能读取，做法是先把要读取的文件拷贝到本地，然后读取本地文件内容
        /// </summary>
        /// <param name="adbExePath"></param>
        /// <param name="remotePath"></param>
        /// <param name="tempPath">要保存本地文件的名字</param>
        /// <param name="content">保存读取到的文件内容</param>
        /// <param name="adbMessage">adb输出的消息</param>
        /// <returns></returns>
        public static AdbReadResult AdbReadFile(string adbExePath, string remotePath, string tempPath, out string content, out string adbMessage)
        {
            content = string.Empty;

            AdbPullResult pullResult = AdbPullFile(adbExePath, remotePath, tempPath, out adbMessage);
            if (pullResult != AdbPullResult.Susccess)
            {
                return (AdbReadResult)pullResult;
            }

            try
            {
                content = File.ReadAllText(tempPath);

                return AdbReadResult.Susccess;
            }
            catch (Exception ex)
            {
                logger.Error("AdbReadFile异常", ex);
                return AdbReadResult.ReadFileFailed;
            }
        }

        #endregion
    }
}

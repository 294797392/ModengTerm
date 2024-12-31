using DotNEToolkit;
using ModengTerm.Base;
using ModengTerm.Base.DataModels;
using ModengTerm.Base.Enumerations;
using ModengTerm.Document;
using ModengTerm.Document.Enumerations;
using ModengTerm.Terminal.DataModels;
using ModengTerm.Terminal.Parsing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

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

        /// <summary>
        /// 获取回放文件的完整目录
        /// </summary>
        /// <param name="sessionId">回放文件所属的会话Id</param>
        /// <param name="playbackFile">回放文件</param>
        /// <returns></returns>
        public static string GetPlaybackFilePath(Playback playbackFile)
        {
            string directory = GetPlaybackFileDirectory(playbackFile.Session.ID);

            return Path.Combine(directory, playbackFile.Name);
        }

        /// <summary>
        /// 获取某个会话对应的回放文件的存储目录
        /// </summary>
        /// <param name="sessionId">回放文件所属的会话Id</param>
        /// <returns></returns>
        public static string GetPlaybackFileDirectory(string sessionId)
        {
            string dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "playback", sessionId);
            if (!Directory.Exists(dir))
            {
                try
                {
                    Directory.CreateDirectory(dir);
                }
                catch (Exception ex)
                {
                    logger.ErrorFormat("创建Playback存储目录异常, {0}, {1}", ex, dir);
                    return string.Empty;
                }
            }

            return dir;
        }


        public static VTPoint ToVTPoint(this Point wpfPoint)
        {
            return new VTPoint(wpfPoint.X, wpfPoint.Y);
        }

        public static VTSize ToVTSize(this Size wpfSize)
        {
            return new VTSize(wpfSize.Width, wpfSize.Height);
        }

        #region AdbUtility

        /// <summary>
        /// 确保Adb守护进程已启动
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public static bool StartAdbServer(XTermSession session) 
        {
            string exePath = session.GetOption<string>(OptionKeyEnum.ADBSH_ADB_PATH);
            int timeout = session.GetOption<int>(OptionKeyEnum.ADBSH_START_SVR_TIMEOUT, MTermConsts.DefaultAdbStartServerTimeout);

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

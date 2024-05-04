using DotNEToolkit;
using ModengTerm.Document;
using ModengTerm.Document.Enumerations;
using ModengTerm.Document.Rendering;
using ModengTerm.ServiceAgents.DataModels;
using ModengTerm.Terminal.DataModels;
using ModengTerm.Terminal.Parsing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace ModengTerm.Terminal
{
    public static class TermUtils
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
                case GraphicsOptions.ForegroundWhite: return VTColorIndex.DarkBlack;

                case GraphicsOptions.BackgroundBlack: return VTColorIndex.DarkBlack;
                case GraphicsOptions.BackgroundBlue: return VTColorIndex.DarkBlue;
                case GraphicsOptions.BackgroundGreen: return VTColorIndex.DarkGreen;
                case GraphicsOptions.BackgroundCyan: return VTColorIndex.DarkCyan;
                case GraphicsOptions.BackgroundRed: return VTColorIndex.DarkRed;
                case GraphicsOptions.BackgroundMagenta: return VTColorIndex.DarkMagenta;
                case GraphicsOptions.BackgroundYellow: return VTColorIndex.DarkYellow;
                case GraphicsOptions.BackgroundWhite: return VTColorIndex.DarkWhite;

                case GraphicsOptions.BrightForegroundBlack: return VTColorIndex.BrightBlack;
                case GraphicsOptions.BrightForegroundBlue: return VTColorIndex.BrightBlue;
                case GraphicsOptions.BrightForegroundGreen: return VTColorIndex.BrightGreen;
                case GraphicsOptions.BrightForegroundCyan: return VTColorIndex.BrightCyan;
                case GraphicsOptions.BrightForegroundRed: return VTColorIndex.BrightRed;
                case GraphicsOptions.BrightForegroundMagenta: return VTColorIndex.BrightMagenta;
                case GraphicsOptions.BrightForegroundYellow: return VTColorIndex.BrightYellow;
                case GraphicsOptions.BrightForegroundWhite: return VTColorIndex.BrightWhite;

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
        public static string GetPlaybackFilePath(PlaybackFile playbackFile)
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

        /// <summary>
        /// 获取终端的清单文件
        /// </summary>
        /// <returns></returns>
        public static TerminalManifest GetManifest()
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "manifest.json");

            if (!File.Exists(filePath))
            {
                return new TerminalManifest();
            }

            try
            {
                return JSONHelper.ParseFile<TerminalManifest>(filePath);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("加载终端清单文件异常, {0}, {1}", ex, filePath);
                return new TerminalManifest();
            }
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

        public static VTPoint ToVTPoint(this Point wpfPoint)
        {
            return new VTPoint(wpfPoint.X, wpfPoint.Y);
        }

        public static VTSize ToVTSize(this Size wpfSize)
        {
            return new VTSize(wpfSize.Width, wpfSize.Height);
        }
    }
}

using DotNEToolkit;
using ModengTerm.Document;
using ModengTerm.Document.Enumerations;
using ModengTerm.Document.Rendering;
using ModengTerm.ServiceAgents.DataModels;
using ModengTerm.Terminal.DataModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using XTerminal.Parser;

namespace ModengTerm.Terminal
{
    public static class TermUtils
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("TermUtils");

        public static VTextAttributes VTAction2TextAttribute(VTActions actions, out bool enabled)
        {
            enabled = true;

            switch (actions)
            {
                case VTActions.Bold: return VTextAttributes.Bold;
                case VTActions.BoldUnset: enabled = false; return VTextAttributes.Bold;
                case VTActions.Underline: return VTextAttributes.Underline;
                case VTActions.UnderlineUnset: enabled = false; return VTextAttributes.Underline;
                case VTActions.Italics: return VTextAttributes.Italics;
                case VTActions.ItalicsUnset: enabled = false; return VTextAttributes.Italics;
                case VTActions.DoublyUnderlined: return VTextAttributes.DoublyUnderlined;
                case VTActions.DoublyUnderlinedUnset: enabled = false; return VTextAttributes.DoublyUnderlined;
                case VTActions.Background: return VTextAttributes.Background;
                case VTActions.BackgroundUnset: enabled = false; return VTextAttributes.Background;
                case VTActions.Foreground: return VTextAttributes.Foreground;
                case VTActions.ForegroundUnset: enabled = false; return VTextAttributes.Foreground;

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
    }
}

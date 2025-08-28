using ModengTerm.Base.DataModels;
using ModengTerm.Base.DataModels.Terminal;
using ModengTerm.Base.Definitions;
using ModengTerm.Base.Enumerations;
using ModengTerm.Base.Metadatas;
using ModengTerm.ViewModel;
using ModengTerm.ViewModel.Session;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Input;

namespace ModengTerm
{
    public static class ClientUtils
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("ClientUtils");

        /// <summary>
        /// 默认要打开的会话
        /// </summary>
        public static readonly XTermSession DefaultSession = new XTermSession() 
        {
            ID = "0",
            Name = "控制台会话",
            Type = (int)SessionTypeEnum.LocalConsole,
            Options = new Dictionary<string, Dictionary<string, object>>()
            {
                {
                    "3738D66F-9B8A-BB45-C823-22DAF39AAAF6", 
                    new Dictionary<string, object>() 
                    {
                        { PredefinedOptions.CMD_STARTUP_DIR, AppDomain.CurrentDomain.BaseDirectory },
                        { PredefinedOptions.CMD_STARTUP_ARGUMENT, string.Empty },
                        { PredefinedOptions.CMD_CONSOLE_ENGINE, "auto" },
                        { PredefinedOptions.CMD_STARTUP_PATH, Path.Combine(Environment.SystemDirectory, "cmd.exe") },
                        { PredefinedOptions.TERM_ADVANCE_RENDER_WRITE, false },
                        { PredefinedOptions.TERM_ADVANCE_AUTO_WRAP_MODE, false },
                        { PredefinedOptions.THEME_BACKGROUND_IMAGE_DATA, string.Empty },
                        { PredefinedOptions.TERM_ADVANCE_CLICK_TO_CURSOR, false },
                        { PredefinedOptions.THEME_ID, "7A2A6563-8C16-4E6A-9C9F-AA610E4C6827" },
                        { PredefinedOptions.THEME_FONT_FAMILY, "新宋体" },
                        { PredefinedOptions.THEME_FONT_SIZE, 16 },
                        { PredefinedOptions.THEME_BACK_COLOR, "36,36,36,255" },
                        { PredefinedOptions.THEME_FONT_COLOR, "242,242,242,255" },
                        { PredefinedOptions.THEME_CURSOR_STYLE, "line" },
                        { PredefinedOptions.THEME_CURSOR_SPEED, "normal" },
                        { PredefinedOptions.THEME_CURSOR_COLOR, "255,255,255,255" },
                        { PredefinedOptions.TEHEM_COLOR_TABLE, "{ \"rgbKeys\":[\"54,52,46,255\",\"165,100,52,255\",\"0,128,0,255\",\"153,150,6,255\",\"70,70,255,255\",\"123,81,117,255\",\"0,162,196,255\",\"207,216,211,255\",\"83,87,85,255\",\"207,158,114,255\",\"28,196,112,255\",\"226,226,52,255\",\"111,111,244,255\",\"169,126,173,255\",\"80,235,252,255\",\"236,238,238,255\"] }" },
                        { PredefinedOptions.THEME_FIND_HIGHLIGHT_BACKCOLOR, "236,238,238,100" },
                        { PredefinedOptions.THEME_FIND_HIGHLIGHT_FORECOLOR, "54,52,46,255" },
                        { PredefinedOptions.THEME_SELECTION_COLOR, "255,255,255,100" },
                        { PredefinedOptions.SSH_TERM_ROW, 24 },
                        { PredefinedOptions.SSH_TERM_COL, 80 },
                        { PredefinedOptions.SSH_TERM_TYPE, "xterm-256color" },
                        { PredefinedOptions.SSH_TERM_SIZE_MODE, "autoFit" },
                        { PredefinedOptions.TERM_READ_ENCODING, "UTF-8" },
                        { PredefinedOptions.TERM_WRITE_ENCODING, "UTF-8" },
                        { PredefinedOptions.SSH_READ_BUFFER_SIZE, 8192 },
                        { PredefinedOptions.TERM_MAX_ROLLBACK, 99999 },
                        { PredefinedOptions.TERM_MAX_CLIPBOARD_HISTORY, 50 },
                        { PredefinedOptions.SSH_THEME_DOCUMENT_PADDING, 5 },
                        { PredefinedOptions.MOUSE_SCROLL_DELTA, 1 },
                        { PredefinedOptions.TERM_DISABLE_BELL, false },
                        { PredefinedOptions.BEHAVIOR_RIGHT_CLICK, "contextMenu" },
                        { PredefinedOptions.TERM_ADVANCE_RENDER_MODE, "default" },
                        { PredefinedOptions.TERM_ADVANCE_AUTO_COMPLETION_ENABLED, false },
                    }
                }
            }
        };

        public static string GetHotkeyName(ModifierKeys modKeys, List<Key> keys, bool doubleModKeys)
        {
            // 如果按了多个快捷键，判断快捷键顺序
            // 第一个按下的键必须是Ctrl键
            string hotKey = string.Empty;

            if (doubleModKeys)
            {
                if (!modKeys.HasFlag(ModifierKeys.Control))
                {
                    // 如果快捷键里没有Control，那么就是不合法的快捷键
                    return string.Empty;
                }

                hotKey += "Ctrl+";

                if (modKeys.HasFlag(ModifierKeys.Alt))
                {
                    hotKey += "Alt+";
                }
                else if (modKeys.HasFlag(ModifierKeys.Shift))
                {
                    hotKey += "Shift+";
                }
                else
                {
                    return string.Empty;
                }
            }
            else
            {
                if (modKeys == ModifierKeys.Control)
                {
                    hotKey += string.Format("Ctrl+");
                }
                else
                {
                    hotKey += string.Format("{0}+", modKeys);
                }
            }

            for (int i = 0; i < keys.Count; i++)
            {
                if (i == keys.Count - 1)
                {
                    hotKey += keys[i].ToString();
                }
                else
                {
                    hotKey += string.Format("{0},", keys[i].ToString());
                }
            }

            return hotKey;
        }

        /// <summary>
        /// 判断按键是否是数字键
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        public static bool IsNumeric(Key k)
        {
            return k >= Key.D0 && k <= Key.D9;
        }

        /// <summary>
        /// 判断按键是否是阿拉伯字母键
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        public static bool IsLetter(Key k)
        {
            return k >= Key.A && k <= Key.Z;
        }

        public static Key GetPressedKey(KeyEventArgs e)
        {
            if (e.Key == Key.System)
            {
                return e.SystemKey;
            }

            return e.Key;
        }

        /// <summary>
        /// 根据指定的SessionType返回需要加载的侧边栏面板列表
        /// </summary>
        /// <param name="metadatas"></param>
        /// <param name="scope"></param>
        /// <returns></returns>
        public static List<TPanelMetadata> GetScopedPanelMetadatas<TPanelMetadata>(List<TPanelMetadata> metadatas, SessionTypeEnum scope) where TPanelMetadata : PanelMetadata
        {
            List<TPanelMetadata> result = new List<TPanelMetadata>();

            foreach (TPanelMetadata metadata in metadatas)
            {
                // 是否需要创建SidePanel
                bool create = metadata.Scopes.Contains(scope);

                if (create) 
                {
                    result.Add(metadata);
                }
            }

            return result;
        }
    }
}

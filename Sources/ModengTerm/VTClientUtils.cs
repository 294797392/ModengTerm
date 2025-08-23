using ModengTerm.Base.Definitions;
using ModengTerm.Base.Enumerations;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Input;

namespace ModengTerm
{
    public static class VTClientUtils
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("VTClientUtils");

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
        /// <param name="sessionType"></param>
        /// <returns></returns>
        public static List<TPanelMetadata> GetTabedPanelMetadatas<TPanelMetadata>(List<TPanelMetadata> metadatas, SessionTypeEnum sessionType) where TPanelMetadata : PanelMetadata
        {
            List<TPanelMetadata> result = new List<TPanelMetadata>();

            foreach (TPanelMetadata metadata in metadatas)
            {
                // 是否需要创建SidePanel
                bool create = false;

                #region 判断是否需要创建SidePanel

                switch (sessionType)
                {
                    case SessionTypeEnum.SSH:
                        {
                            create = metadata.Scopes.Contains(PanelScope.SshTab);
                            break;
                        }

                    case SessionTypeEnum.SFTP:
                        {
                            create = metadata.Scopes.Contains(PanelScope.SftpTab);
                            break;
                        }

                    case SessionTypeEnum.Tcp:
                        {
                            create = metadata.Scopes.Contains(PanelScope.TcpTab);
                            break;
                        }

                    case SessionTypeEnum.SerialPort:
                        {
                            create = metadata.Scopes.Contains(PanelScope.SerialPortTab);
                            break;
                        }

                    case SessionTypeEnum.Localhost:
                        {
                            create = metadata.Scopes.Contains(PanelScope.ConsoleTab);
                            break;
                        }

                    default:
                        throw new NotImplementedException();
                }

                #endregion

                if (create) 
                {
                    result.Add(metadata);
                }
            }

            return result;
        }
    }
}

using ModengTerm.Addon;
using ModengTerm.Addon.ClientBridges;
using ModengTerm.Addon.Interactive;
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
using System.Data.Entity.Infrastructure.Interception;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace ModengTerm
{
    public static class ClientUtils
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("ClientUtils");

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

        /// <summary>
        /// 执行一个命令
        /// </summary>
        /// <param name="menuItemVm">要触发命令的菜单</param>
        public static void DispatchCommand(MenuItemVM menuItemVm) 
        {
            if (string.IsNullOrEmpty(menuItemVm.CommandKey))
            {
                // 此时说明该菜单没有注册对应的事件
                return;
            }

            IClientEventRegistry eventRegistry = Client.GetEventRegistry();
            CommandArgs.Instance.CommandKey = menuItemVm.CommandKey;
            CommandArgs.Instance.ActiveTab = Client.GetActiveTab<IClientTab>();
            eventRegistry.PublishCommand(CommandArgs.Instance);
        }
    }
}

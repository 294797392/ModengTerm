using ModengTerm.Addons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Addon.Interactive
{
    public delegate void ClientEventDelegate(ClientEvent evType, ClientEventArgs evArgs);

    public delegate void TabEventDelegate(TabEventArgs e);

    public delegate void ClientHotkeyDelegate();

    public interface IClientEventRegistry
    {
        #region ClientEvent

        /// <summary>
        /// 订阅客户端的事件
        /// </summary>
        /// <param name="evType"></param>
        /// <param name="delegate"></param>
        void SubscribeEvent(ClientEvent evType, ClientEventDelegate @delegate);

        /// <summary>
        /// 取消订阅客户端事件
        /// </summary>
        /// <param name="evType"></param>
        /// <param name="delegate"></param>
        void UnsubscribeEvent(ClientEvent evType, ClientEventDelegate @delegate);

        /// <summary>
        /// 发布一个事件
        /// </summary>
        /// <param name="evArgs"></param>
        void PublishEvent(ClientEventArgs evArgs);

        #endregion

        #region TabEvent

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tab">要订阅的标签页</param>
        /// <param name="evType">要订阅的事件</param>
        /// <param name="evArgs"></param>
        void SubscribeTabEvent(IClientTab tab, TabEvent evType, TabEventDelegate @delegate);

        void UnsubscribeTabEvent(IClientTab tab, TabEvent evType, TabEventDelegate @delegate);

        /// <summary>
        /// 取消指定tab的所有订阅的事件
        /// </summary>
        /// <param name="tab"></param>
        void UnsubscribeTabEvent(IClientTab tab);

        void PublishTabEvent(IClientTab tab, TabEventArgs evArgs);

        #endregion

        #region HotkeyEvent

        /// <summary>
        /// 注册快捷键事件
        /// </summary>
        /// <param name="addon">要注册快捷键的插件</param>
        /// <param name="hotkey">
        /// 要注册的快捷键
        /// 格式：
        /// 1. Esc
        /// 2. Ctrl+A
        /// 3. Ctrl+A,B（按下两个快捷键执行）
        /// 4. Alt+A
        /// 5. Alt+A,B
        /// 6. Shift+A
        /// 7. Shift+A,B
        /// 8. Ctrl+Shift+A
        /// 9. Ctrl+Alt+A
        /// </param>
        /// <param name="delegate">快捷键回调</param>
        void RegisterHotkey(AddonModule addon, string hotkey, HotkeyScopes scope, ClientHotkeyDelegate @delegate);

        /// <summary>
        /// 取消注册快捷键
        /// </summary>
        /// <param name="addon">要取消注册快捷键的插件</param>
        /// <param name="hotkey">要取消注册的快捷键</param>
        void UnregisterHotkey(AddonModule addon, string hotkey);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hotkey"></param>
        /// <returns>是否执行了快捷键事件</returns>
        bool PublishHotkeyEvent(string hotkey);

        #endregion
    }
}
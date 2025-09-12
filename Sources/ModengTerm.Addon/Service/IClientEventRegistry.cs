using ModengTerm.Addon.Interactive;
using ModengTerm.Base;

namespace ModengTerm.Addon.Service
{
    public delegate void ClientEventDelegate(ClientEventArgs e, object userData);

    public delegate void TabEventDelegate(TabEventArgs e, object userData);

    public delegate void ClientHotkeyDelegate(object userData);

    /// <summary>
    /// 点击顶部菜单栏或者右键菜单所执行的事件处理器
    /// </summary>
    /// <param name="e"></param>
    public delegate void CommandDelegate(CommandArgs e);

    public interface IClientEventRegistry
    {
        #region ClientEvent

        void SubscribeEvent(ClientEvent ev, ClientEventDelegate @delegate, object userData = null);

        /// <summary>
        /// 订阅客户端的事件
        /// </summary>
        /// <param name="evid"></param>
        /// <param name="delegate"></param>
        void SubscribeEvent(ClientEvent evid, string evcond, ClientEventDelegate @delegate, object userData = null);

        /// <summary>
        /// 取消订阅客户端事件
        /// </summary>
        /// <param name="evid"></param>
        /// <param name="delegate"></param>
        void UnsubscribeEvent(ClientEvent evid, ClientEventDelegate @delegate);

        /// <summary>
        /// 发布一个事件
        /// </summary>
        /// <param name="evArgs"></param>
        void PublishEvent(ClientEventArgs evArgs);

        #endregion

        #region TabEvent

        void SubscribeTabEvent(TabEvent ev, TabEventDelegate @delegate, IClientTab tab = null, object userData = null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ev">要订阅的事件</param>
        /// <param name="delegate"></param>
        /// <param name="tab">要订阅的标签页，如果为空，则订阅所有标签页的事件</param>
        void SubscribeTabEvent(TabEvent ev, string evcond, TabEventDelegate @delegate, IClientTab tab = null, object userData = null);

        void UnsubscribeTabEvent(TabEvent ev, TabEventDelegate @delegate, IClientTab tab = null);

        /// <summary>
        /// 取消指定tab的所有订阅的事件
        /// </summary>
        /// <param name="tab"></param>
        void UnsubscribeTabEvent(IClientTab tab = null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="evArgs"></param>
        void PublishTabEvent(TabEventArgs evArgs);

        #endregion

        #region HotkeyEvent

        /// <summary>
        /// 注册快捷键事件
        /// </summary>
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
        void RegisterHotkey(string hotkey, ClientHotkeyDelegate @delegate, object userData = null);

        /// <summary>
        /// 取消注册快捷键
        /// </summary>
        /// <param name="hotkey">要取消注册的快捷键</param>
        void UnregisterHotkey(string hotkey, ClientHotkeyDelegate @delegate);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hotkey"></param>
        /// <returns>是否执行了快捷键事件</returns>
        bool PublishHotkeyEvent(string hotkey);

        #endregion

        #region Command

        void RegisterCommand(string commandKey, CommandDelegate @delegate, object userData = null);

        void UnregisterCommand(string commandKey);

        bool PublishCommand(CommandArgs cmdArgs);

        #endregion
    }
}
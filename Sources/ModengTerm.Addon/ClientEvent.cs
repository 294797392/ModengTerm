using ModengTerm.Addon.Interactive;
using ModengTerm.Base.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Addon
{
    /// <summary>
    /// 定义客户端通用事件
    /// 所有事件在UI线程触发
    /// </summary>
    public enum ClientEvent
    {
        // 所有CLIENT开头的事件类型，是通用事件类型。0 - 100 通用

        /// <summary>
        /// 当应用程序启动之后触发
        /// </summary>
        CLIENT_INITIALIZED = 0,

        /// <summary>
        /// 会话状态改变事件
        /// </summary>
        CLIENT_TAB_STATUS_CHANGED = 1,

        /// <summary>
        /// 选中的Tab页面改变事件
        /// </summary>
        CLIENT_ACTIVE_TAB_CHANGED = 2,

        /// <summary>
        /// 会话被打开之后触发的事件
        /// </summary>
        CLIENT_TAB_OPENED = 3,

        /// <summary>
        /// 会话被关闭之后触发的事件
        /// </summary>
        CLIENT_TAB_CLOSED = 4,
    }

    public abstract class ClientEventArgs
    {
        public abstract ClientEvent Type { get; }
    }

    public class ClientEventClientInitialized : ClientEventArgs
    {
        public override ClientEvent Type => ClientEvent.CLIENT_INITIALIZED;
    }

    public class ClientEventTabStatusChanged : ClientEventArgs
    {
        public override ClientEvent Type => ClientEvent.CLIENT_TAB_STATUS_CHANGED;

        public SessionStatusEnum OldStatus { get; set; }

        public SessionStatusEnum NewStatus { get; set; }
    }

    public class ClientEventTabOpened : ClientEventArgs
    {
        public override ClientEvent Type => ClientEvent.CLIENT_TAB_OPENED;

        /// <summary>
        /// 会话类型
        /// </summary>
        public SessionTypeEnum SessionType { get; set; }

        /// <summary>
        /// 被打开的Tab页面
        /// </summary>
        public IClientTab OpenedTab { get; set; }
    }

    public class ClientEventActiveTabChanged : ClientEventArgs 
    {
        public override ClientEvent Type => ClientEvent.CLIENT_ACTIVE_TAB_CHANGED;

        /// <summary>
        /// 选中之前的Tab页面
        /// </summary>
        public IClientTab RemovedTab { get; set; }

        /// <summary>
        /// 选中之后的Tab页面
        /// </summary>
        public IClientTab AddedTab { get; set; }
    }
}

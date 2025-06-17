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
    /// 定义主应用程序触发的事件
    /// </summary>
    public enum HostEvent
    {
        // 所有HOST开头的事件类型，是通用事件类型。0 - 100 通用

        /// <summary>
        /// 会话状态改变事件
        /// </summary>
        HOST_SESSION_STATUS_CHANGED = 0,

        /// <summary>
        /// 选中的Tab页面改变事件
        /// </summary>
        HOST_ACTIVE_TAB_CHANGED = 1,

        /// <summary>
        /// 会话被打开之后触发的事件
        /// </summary>
        HOST_TAB_OPENED = 2,

        /// <summary>
        /// 当应用程序启动之后触发
        /// </summary>
        HOST_APP_INITIALIZED = 3,
    }

    public abstract class HostEventArgs
    {

    }

    public class StatusChangedEventArgs : HostEventArgs
    {
        public SessionStatusEnum OldStatus { get; set; }

        public SessionStatusEnum NewStatus { get; set; }
    }

    public class TabOpenedEventArgs : HostEventArgs
    {
        /// <summary>
        /// 会话类型
        /// </summary>
        public SessionTypeEnum Type { get; set; }

        /// <summary>
        /// 被打开的Tab页面
        /// </summary>
        public IHostTab OpenedTab { get; set; }
    }

    public class ActiveTabChangedEventArgs : HostEventArgs 
    {
        /// <summary>
        /// 选中之前的Tab页面
        /// </summary>
        public IHostTab RemovedTab { get; set; }

        /// <summary>
        /// 选中之后的Tab页面
        /// </summary>
        public IHostTab AddedTab { get; set; }
    }
}

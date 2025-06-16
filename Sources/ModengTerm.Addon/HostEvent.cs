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

        // Shell会话事件 101 - 200
        //SHELL_SESSION_OPENED = 101,

        //#region 所有会话类型的通用命令

        ///// <summary>
        ///// 当分组发生改变的时候出发
        ///// </summary>
        //public const string CMD_GROUP_CHANGED = "CMD_GROUP_CHANGED";
        //public const string CMD_SELECTED_SESSION_CHANGED = "CMD_SELECTED_SESSION_CHANGED";
        //public const string CMD_SESSION_DELETED = "CMD_SESSION_DELETED";
        //public const string SESSION_STATUS_CHANGED = "SESSION_STATUS_CHANGED";

        //#endregion

        //#region 终端相关的命令

        ///// <summary>
        ///// 当打开终端类型的会话之后触发
        ///// </summary>
        //public const string TERM_SESSION_OPENED = "TERM_SESSION_OPENED";
        ///// <summary>
        ///// 每次渲染结束出发
        ///// </summary>
        //public const string TERM_RENDERED = "TERM_RENDERED";
        ///// <summary>
        ///// 用户输入结束触发
        ///// </summary>
        //public const string TERM_USER_INPUT = "TERM_USER_INPUT";

        //#endregion
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

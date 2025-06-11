using ModengTerm.Base.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Base
{
    /// <summary>
    /// OpenedSessionVM触发的事件类型
    /// </summary>
    public enum EventType
    {
        // 0 - 100 通用
        COMMON_SESSION_STATUS_CHANGED = 0,
        COMMON_SELECTED_SESSION_CHANGED = 1,
        
        // Shell会话 101 - 200
        SHELL_SESSION_OPENED = 101,

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

    public class StatusChangedEventArgs : EventArgs
    {
        public SessionStatusEnum OldStatus { get; set; }

        public SessionStatusEnum NewStatus { get; set; }
    }
}

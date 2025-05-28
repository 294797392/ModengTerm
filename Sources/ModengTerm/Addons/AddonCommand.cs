using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// 预定义的命令
namespace ModengTerm.Addons
{
    /// <summary>
    /// 通用的系统级别的命令
    /// </summary>
    public static class GlobalCommands
    {
        /// <summary>
        /// 当分组发生改变的时候出发
        /// </summary>
        public const string CMD_GROUP_CHANGED = "app.cmd.group_changed";

        public const string CMD_SELECTED_SESSION_CHANGED = "app.cmd.selected_session_changed";

        public const string CMD_SESSION_DELETED = "app.cmd.session_deleted";

        /// <summary>
        /// 当打开Shell会话之后触发
        /// </summary>
        public const string CMD_SHELL_SESSION_OPENED = "app.cmd.shell_session_opened";
    }

    /// <summary>
    /// 终端命令
    /// </summary>
    public static class ShellSessionCommands
    {
        /// <summary>
        /// 每次渲染结束出发
        /// </summary>
        public const string CMD_RENDERED = "term.cmd.rendered";

        /// <summary>
        /// 用户输入结束触发
        /// </summary>
        public const string CMD_USER_INPUT = "term.cmd.user_input";
    }
}

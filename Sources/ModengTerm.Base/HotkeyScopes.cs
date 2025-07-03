using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Base
{
    /// <summary>
    /// 定义快捷键的范围
    /// </summary>
    public enum HotkeyScopes
    {
        /// <summary>
        /// 整个客户端
        /// </summary>
        Client,

        /// <summary>
        /// 当前激活的是ShellTab的时候才能触发
        /// </summary>
        ClientShellTab,

        /// <summary>
        /// 当前激活的是SftpTab的时候才能触发
        /// </summary>
        ClientSftpTab
    }
}

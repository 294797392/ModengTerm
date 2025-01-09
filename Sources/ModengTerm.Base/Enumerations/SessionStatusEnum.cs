using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModengTerm.Base.Enumerations
{
    /// <summary>
    /// 表示会话的连接状态
    /// </summary>
    public enum SessionStatusEnum
    {
        /// <summary>
        /// 连接已断开
        /// </summary>
        Disconnected,

        /// <summary>
        /// 连接中
        /// </summary>
        Connecting,

        /// <summary>
        /// 已连接
        /// </summary>
        Connected,

        /// <summary>
        /// 连接失败
        /// </summary>
        ConnectError,
    }
}
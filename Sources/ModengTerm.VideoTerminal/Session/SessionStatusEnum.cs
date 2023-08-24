using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XTerminal.Session
{
    /// <summary>
    /// 表示会话的连接状态
    /// </summary>
    public enum SessionStatusEnum
    {
        /// <summary>
        /// 数据流处于初始化状态
        /// </summary>
        Connecting,

        /// <summary>
        /// 所有准备工作已就绪，可以开始解析终端数据流了
        /// </summary>
        Connected,

        /// <summary>
        /// 断开连接
        /// </summary>
        Disconnected
    }
}
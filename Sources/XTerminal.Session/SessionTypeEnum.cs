using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XTerminal.Sessions
{
    /// <summary>
    /// 标识会话类型
    /// </summary>
    public enum SessionTypeEnum
    {
        /// <summary>
        /// Windows命令行
        /// </summary>
        Win32CommandLine,

        /// <summary>
        /// 是一个SSH远程主机
        /// </summary>
        SSH,

        /// <summary>
        /// 使用libvt库实现的ssh客户端
        /// </summary>
        libvtssh,

        /// <summary>
        /// 是一个串口设备
        /// </summary>
        SerialPort
    }
}
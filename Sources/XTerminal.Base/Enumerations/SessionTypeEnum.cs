using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XTerminal.Base.Enumerations
{
    /// <summary>
    /// 标识会话类型
    /// </summary>
    public enum SessionTypeEnum
    {
        /// <summary>
        /// Windows命令行
        /// </summary>
        Win32CommandLine = 0,

        /// <summary>
        /// 是一个SSH远程主机
        /// </summary>
        SSH = 1,

        /// <summary>
        /// 使用libvt库实现的ssh客户端
        /// </summary>
        libvtssh = 2,

        /// <summary>
        /// 是一个串口设备
        /// </summary>
        SerialPort = 3,

        /// <summary>
        /// SFTP会话
        /// </summary>
        SFTP
    }
}
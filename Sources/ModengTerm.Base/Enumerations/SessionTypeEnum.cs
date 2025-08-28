using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModengTerm.Base.Enumerations
{
    /// <summary>
    /// 标识会话类型
    /// </summary>
    public enum SessionTypeEnum
    {
        /// <summary>
        /// 本地控制台会话
        /// </summary>
        LocalConsole,

        /// <summary>
        /// 是一个SSH远程主机
        /// </summary>
        Ssh,

        /// <summary>
        /// 是一个串口设备
        /// </summary>
        SerialPort,

        /// <summary>
        /// SFTP会话
        /// </summary>
        Sftp,

        /// <summary>
        /// Tcp会话
        /// </summary>
        Tcp
    }
}
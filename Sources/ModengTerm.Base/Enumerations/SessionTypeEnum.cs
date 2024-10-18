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
        /// 主机命令行
        /// </summary>
        CommandLine,

        /// <summary>
        /// 是一个SSH远程主机
        /// </summary>
        SSH,

        /// <summary>
        /// 是一个串口设备
        /// </summary>
        SerialPort,

        /// <summary>
        /// SFTP会话
        /// </summary>
        SFTP,

        /// <summary>
        /// Tcp会话
        /// </summary>
        RawTcp,

        /// <summary>
        /// SSH回放
        /// </summary>
        //SSHPlayback,
    }
}
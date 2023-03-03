using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XTerminal.Channels
{
    /// <summary>
    /// 标识客户端的类型
    /// </summary>
    public enum VTChannelTypes
    {
        /// <summary>
        /// 是一个SSH远程主机
        /// </summary>
        SSH,

        /// <summary>
        /// 是一个串口设备
        /// </summary>
        SerialPort
    }
}
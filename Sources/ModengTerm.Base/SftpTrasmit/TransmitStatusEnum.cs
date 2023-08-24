using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XTerminal.Base.SftpTrasmit
{
    /// <summary>
    /// 定义Sftp传输状态
    /// </summary>
    public enum TransmitStatusEnum
    {
        /// <summary>
        /// 已经入队
        /// </summary>
        Queued,

        /// <summary>
        /// 传输中
        /// </summary>
        Transmitting,

        /// <summary>
        /// 传输出现错误
        /// </summary>
        Error,

        /// <summary>
        /// 传输成功
        /// </summary>
        Success
    }
}

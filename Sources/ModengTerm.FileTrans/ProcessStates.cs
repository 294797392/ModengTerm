using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.FileTrans
{
    /// <summary>
    /// 定义文件的处理状态
    /// </summary>
    public enum ProcessStates
    {
        /// <summary>
        /// 等待入队
        /// </summary>
        WaitQueued,

        /// <summary>
        /// 已经入队
        /// </summary>
        Queued,

        /// <summary>
        /// 传输失败
        /// </summary>
        Failure,

        /// <summary>
        /// 传输成功
        /// </summary>
        Completed,

        /// <summary>
        /// 开始传输
        /// 在传输开始之前触发
        /// </summary>
        StartTransfer,

        /// <summary>
        /// 有部分数据成功传输
        /// </summary>
        BytesTransfered,
    }
}

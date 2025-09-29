using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Ftp
{
    /// <summary>
    /// 定义任务的处理状态
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
        /// 在执行处理之前触发
        /// </summary>
        Starting,

        /// <summary>
        /// 处理失败
        /// 当上传对象是文件夹时，只有等文件夹里的所有子项上传结束才可能会改变成此状态
        /// </summary>
        Failure,

        /// <summary>
        /// 处理成功
        /// 当上传对象是文件夹时，只有等文件夹里的所有子项上传结束才可能会改变成此状态
        /// </summary>
        Success,

        /// <summary>
        /// 处理进度改变
        /// </summary>
        ProgressChanged,
    }
}

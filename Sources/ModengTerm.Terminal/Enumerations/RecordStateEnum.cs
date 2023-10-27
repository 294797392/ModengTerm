using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal.Enumerations
{
    /// <summary>
    /// 定义录制状态
    /// </summary>
    public enum RecordStateEnum
    {
        /// <summary>
        /// 停止录制
        /// </summary>
        Stop,

        /// <summary>
        /// 开始录制
        /// </summary>
        Start,

        /// <summary>
        /// 暂停录制
        /// </summary>
        Pause,

        /// <summary>
        /// 继续录制
        /// </summary>
        Resume
    }
}

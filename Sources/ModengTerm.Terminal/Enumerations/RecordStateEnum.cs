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
        /// 已经停止录制
        /// </summary>
        Stop,

        /// <summary>
        /// 正在录制
        /// </summary>
        Recording,

        /// <summary>
        /// 已经暂停录制
        /// </summary>
        Pause,
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal.Enumerations
{
    /// <summary>
    /// 定义回放状态
    /// </summary>
    public enum PlaybackStatusEnum
    {
        /// <summary>
        /// 空闲，还没播放
        /// </summary>
        Idle,

        /// <summary>
        /// 正在播放中
        /// </summary>
        Playing,
    }
}

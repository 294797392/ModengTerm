using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XTerminal.Base.Enumerations
{
    /// <summary>
    /// 定义光标闪烁速度等级
    /// 每种速度对应的间隔时间请参考XTermDefaultValues.XXXXBlinkInterval
    /// </summary>
    public enum VTCursorSpeeds
    {
        /// <summary>
        /// 低速
        /// </summary>
        LowSpeed,

        /// <summary>
        /// 中速
        /// </summary>
        NormalSpeed,

        /// <summary>
        /// 高速闪烁
        /// </summary>
        HighSpeed
    }
}

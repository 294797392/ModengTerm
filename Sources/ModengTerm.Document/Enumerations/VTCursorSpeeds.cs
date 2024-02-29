using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Document.Enumerations
{
    /// <summary>
    /// 定义光标闪烁速度
    /// 枚举的值就是闪烁间隔时间，单位毫秒
    /// </summary>
    public enum VTCursorSpeeds
    {
        /// <summary>
        /// 高速闪烁
        /// </summary>
        HighSpeed = 300,

        /// <summary>
        /// 中速
        /// </summary>
        NormalSpeed = 600,

        /// <summary>
        /// 低速
        /// </summary>
        LowSpeed = 900,
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XTerminal.Base.Enumerations
{
    /// <summary>
    /// 定义光标类型
    /// </summary>
    public enum VTCursorStyles
    {
        None,

        /// <summary>
        /// 光标是一条竖线
        /// </summary>
        Line,

        /// <summary>
        /// 光标是一个矩形块
        /// </summary>
        Block,

        /// <summary>
        /// 光标是半个下划线
        /// </summary>
        Underscore
    }
}

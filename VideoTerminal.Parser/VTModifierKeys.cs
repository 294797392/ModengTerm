using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XTerminalParser
{
    /// <summary>
    /// 修饰键
    /// </summary>
    public enum VTModifierKeys
    {
        None,

        /// <summary>
        /// 左侧Alt键
        /// </summary>
        LeftAlt = 1,
        
        /// <summary>
        /// 左侧Shift键
        /// </summary>
        LeftShift = 2,
        
        /// <summary>
        /// 左侧CTRL键
        /// </summary>
        LeftControl = 4,

        /// <summary>
        /// 右侧Alt键
        /// </summary>
        RightAlt = 8,

        /// <summary>
        /// 右侧Shift键
        /// </summary>
        RightShift = 16,

        /// <summary>
        /// 右侧Control键
        /// </summary>
        RightControl = 32
    }
}

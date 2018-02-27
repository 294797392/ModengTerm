using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XTerminal.Terminal
{
    /// <summary>
    /// 存储按下的键的信息
    /// </summary>
    public struct KeyInfo
    {
        /// <summary>
        /// 按下的键的ascll码
        /// </summary>
        public char PressedKey;

        /// <summary>
        /// 是否按下了Control键
        /// </summary>
        public bool IsCtrlPressed;
    }
}
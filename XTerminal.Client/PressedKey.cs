using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace XTerminal
{
    /// <summary>
    /// 存储按下的键的信息
    /// </summary>
    public struct PressedKey
    {
        /// <summary>
        /// 是否是大写字母
        /// </summary>
        public bool IsUpperCase;

        /// <summary>
        /// 按下的键
        /// </summary>
        public Key Key;

        /// <summary>
        /// 是否按下了Control键
        /// </summary>
        public bool IsControlPressed;

        /// <summary>
        /// 是否按下了Shift键
        /// </summary>
        public bool IsShiftPressed;
    }
}
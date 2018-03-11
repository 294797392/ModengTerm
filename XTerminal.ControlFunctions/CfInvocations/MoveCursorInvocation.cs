using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AsciiControlFunctions.CfInvocations
{
    /// <summary>
    /// 移动光标调用
    /// </summary>
    public struct MoveCursorInvocation : ICfInvocation
    {
        /// <summary>
        /// 光标移动的方向
        /// </summary>
        public enum CursorDirectionEnum
        {
            Auto,
            Top,
            Left,
            Right,
            Bottom
        }

        /// <summary>
        /// 当Direction为Auto的时候，要设置的光标X坐标
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// 当Direction为Auto的时候，要设置的光标Y坐标
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        /// 光标移动的次数
        /// </summary>
        public int Times { get; set; }

        /// <summary>
        /// 光标移动方向
        /// </summary>
        public CursorDirectionEnum Direction { get; set; }
    }
}
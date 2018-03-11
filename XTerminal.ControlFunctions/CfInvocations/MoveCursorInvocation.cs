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
            Custom,
            Up,
            Left,
            Right,
            Down
        }

        /// <summary>
        /// 当Direction为Auto的时候，要设置的光标X坐标
        /// Row
        /// </summary>
        public int X;

        /// <summary>
        /// 当Direction为Auto的时候，要设置的光标Y坐标
        /// Column
        /// </summary>
        public int Y;

        /// <summary>
        /// 光标移动的次数
        /// </summary>
        public int Times;

        /// <summary>
        /// 光标移动方向
        /// </summary>
        public CursorDirectionEnum Direction;
    }
}
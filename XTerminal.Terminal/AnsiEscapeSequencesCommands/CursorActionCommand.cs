using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XTerminal.Terminal.AnsiEscapeSequencesCommands
{
    /// <summary>
    /// 操作光标命令
    /// </summary>
    public class CursorActionCommand: IEscapeSequencesCommand
    {
        /// <summary>
        /// 当Direction为Auto的时候，要设置的光标X坐标
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// 当Direction为Auto的时候，要设置的光标Y坐标
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        /// 光标移动的行数
        /// </summary>
        public int MoveLength { get; set; }

        /// <summary>
        /// 光标移动方向
        /// </summary>
        public CursorDirectionEnum Direction { get; set; }
    }

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
}
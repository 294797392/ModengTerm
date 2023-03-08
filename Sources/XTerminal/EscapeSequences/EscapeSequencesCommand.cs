using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace XTerminal.Terminal.EscapeSequences
{
    /// <summary>
    /// Enable scrolling from row {start} to row {end}.
    /// </summary>
    public class ScrollActionCommand : IEscapeSequencesCommand
    {
        public int StartRow { get; set; }

        public int EndRow { get; set; }
    }

    public class NormalTextCommand : IEscapeSequencesCommand
    {
        public string Text { get; set; }

        public NormalTextCommand(string txt)
        {
            this.Text = txt;
        }
    }

    /// <summary>
    /// 操作光标命令
    /// </summary>
    public class CursorActionCommand : IEscapeSequencesCommand
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

    /// <summary>
    /// 显示带颜色的文本
    /// </summary>
    public class ColorizedTextCommand : IEscapeSequencesCommand
    {
        /// <summary>
        /// 文本装饰
        /// </summary>
        public List<TextDecorationEnum> Decorations { get; set; }

        /// <summary>
        /// 要显示的文本
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// 前景色
        /// </summary>
        public string Foreground { get; set; }

        /// <summary>
        /// 背景色
        /// </summary>
        public string Background { get; set; }

        public ColorizedTextCommand(string txt)
        {
            this.Decorations = new List<TextDecorationEnum>();
            this.Text = txt;
        }

        public ColorizedTextCommand()
        {
            this.Decorations = new List<TextDecorationEnum>();
        }

        /// <summary>
        /// 文字装饰
        /// </summary>
        public enum TextDecorationEnum
        {
            Bright,
            Dim,
            Underscore,
            Blink,
            Reverse,
            Hidden,
            ResetAllAttributes
        }
    }
}
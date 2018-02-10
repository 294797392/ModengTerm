using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XTerminal.Terminal.AnsiEscapeSequencesCommands
{
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
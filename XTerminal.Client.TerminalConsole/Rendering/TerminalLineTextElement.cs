using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.TextFormatting;

namespace XTerminal.Client.TerminalConsole.Rendering
{
    /// <summary>
    /// 表示一行里的一个字符串元素
    /// </summary>
    public class TerminalLineTextElement : TerminalLineElement
    {
        public string Text { get; set; }

        public override TextRun CreateTextRun()
        {
            return new TextCharacters(this.Text, base.ColumnIndex, Text.Length, base.properties);
        }

        public override void InputChar(char c)
        {
            this.Text += c;
        }
    }
}
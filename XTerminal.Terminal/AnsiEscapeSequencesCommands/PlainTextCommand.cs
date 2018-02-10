using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XTerminal.Terminal.AnsiEscapeSequencesCommands
{
    public class PlainTextCommand: IEscapeSequencesCommand
    {
        public string Text { get; set; }

        public PlainTextCommand(string txt)
        {
            this.Text = txt;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XTerminal.Terminal.AnsiEscapeSequencesCommands
{
    /// <summary>
    /// Enable scrolling from row {start} to row {end}.
    /// </summary>
    public class ScrollActionCommand: IEscapeSequencesCommand
    {
        public int StartRow { get; set; }

        public int EndRow { get; set; }
    }
}
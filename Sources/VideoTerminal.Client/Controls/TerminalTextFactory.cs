using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XTerminal.Controls
{
    public static class TerminalLineFactory
    {
        public static TerminalText Create(double offsetX, double offsetY, string text)
        {
            TerminalText termLine = new TerminalText(offsetX, offsetY, text);
            return termLine;
        }

        public static TerminalText Render(double offsetX, double offsetY, string text)
        {
            TerminalText termLine = TerminalLineFactory.Create(offsetX, offsetY, text);
            termLine.PerformRender();
            return termLine;
        }

        public static TerminalText Create(double offsetX, double offsetY)
        {
            return new TerminalText(offsetX, offsetY);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XTerminal.Render
{
    public static class TerminalLineFactory
    {
        public static TerminalLine Create(double offsetX, double offsetY, string text)
        {
            TerminalLine termLine = new TerminalLine(offsetX, offsetY, text);
            return termLine;
        }

        public static TerminalLine Render(double offsetX, double offsetY, string text)
        {
            TerminalLine termLine = TerminalLineFactory.Create(offsetX, offsetY, text);
            termLine.PerformRender();
            return termLine;
        }

        public static TerminalLine Create(double offsetX, double offsetY)
        {
            return new TerminalLine(offsetX, offsetY);
        }
    }
}

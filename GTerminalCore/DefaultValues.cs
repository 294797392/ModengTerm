using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GardeniaTerminalCore
{
    public static class DefaultValues
    {
        public static Encoding DefaultEncoding = Encoding.UTF8;
        public const uint TerminalColumns = 80;
        public const uint TerminalRows = 24;
        public const string TerminalName = "xterm-256color";
    }
}
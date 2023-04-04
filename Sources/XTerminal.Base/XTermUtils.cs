using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTerminal.Base.Enumerations;

namespace XTerminal.Base
{
    public static class XTermUtils
    {
        public static string GetTermName(TerminalTypeEnum type)
        {
            switch (type)
            {
                case TerminalTypeEnum.VT100: return "vt100";
                case TerminalTypeEnum.VT220: return "vt220";
                case TerminalTypeEnum.XTerm: return "xterm";
                case TerminalTypeEnum.XTerm256Color: return "xterm-256color";
                default:
                    throw new NotImplementedException();
            }
        }
    }
}

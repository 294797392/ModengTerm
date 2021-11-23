using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VideoTerminal.Parser
{
    public interface VTWindow
    {
        void SetTitle(string title);
        void SetIconName(string name);
    }
}

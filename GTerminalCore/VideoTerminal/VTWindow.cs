using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GardeniaTerminalCore
{
    public interface VTWindow
    {
        void SetTitle(string title);
        void SetIconName(string name);
    }
}

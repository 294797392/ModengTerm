using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GTerminalCore
{
    public interface IVTScreen
    {
        void PrintText(char text);
        void PrintText(string text);
    }
}

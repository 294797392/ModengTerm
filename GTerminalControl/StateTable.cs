using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GTerminalControl
{
    public static class StateTable
    {
        private static Dictionary<byte, VTState> Table = new Dictionary<byte, VTState>() 
        {
            { 0, VTState.Ground }
        };
    }
}
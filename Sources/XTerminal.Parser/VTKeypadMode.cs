using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XTerminal.Parser
{
    public enum VTKeypadMode
    {
        ApplicationMode,

        /// <summary>
        /// DECKPNM
        /// The auxiliary keypad keys will send ASCII codes corresponding to the characters engraved on the keys
        /// </summary>
        NumericMode
    }
}

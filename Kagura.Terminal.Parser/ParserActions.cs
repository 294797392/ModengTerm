using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VideoTerminal.Parser
{
    public enum ParserActions
    {
        /// <summary>
        /// OSC状态结束
        /// </summary>
        OSCCompleted,

        /// <summary>
        /// OSC动作
        /// </summary>
        OSCAction,
    }
}

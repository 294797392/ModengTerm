using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XTerminalCore.Invocations
{
    /// <summary>
    /// 执行单字符ControlFunction所需的信息
    /// </summary>
    public struct SingleCharacterInvocation : ICfInvocation
    {
        /// <summary>
        /// 要执行的动作
        /// </summary>
        public int Action;
    }
}
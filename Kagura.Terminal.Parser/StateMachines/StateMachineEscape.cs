using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VideoTerminal.Parser.StateMachines
{
    /// <summary>
    /// 处理转义字符的状态机
    /// 
    /// 进入该状态机说明遇到了一个转义字符
    /// 转义字符是ESC
    /// </summary>
    public class StateMachineEscape : StateMachine
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("StateMachineEscape");

        public override int Run()
        {
            this.Context.State = VTStates.Escape;

            return ParserCode.SUCCESS;
        }
    }
}


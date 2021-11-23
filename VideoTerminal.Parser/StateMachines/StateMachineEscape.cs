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

        public override VTStates Run()
        {
            byte ch = this.Context.CurrentChar;

            if (ASCIIChars.IsC0Code(ch))
            {
            }
            else if (ASCIIChars.IsDelete(ch))
            {
                // 忽略Delete字符
            }



            return VTStates.Escape;
        }
    }
}


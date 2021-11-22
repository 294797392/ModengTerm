using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VideoTerminal.Parser.StateMachines
{
    /// <summary>
    /// This is the initial state of the parser, and the state used to consume all characters other than components of escape and control sequences
    /// 这是解析器的初始状态，用来处理除了转义字符和控制字符之外的所有其他字符
    /// 
    /// 这个状态叫做基态（Ground, 基本状态）
    /// </summary>
    public class StateMachineGround : StateMachine
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("StateMachineGround");

        public override int Run()
        {
            return ParserCode.SUCCESS;
        }
    }
}

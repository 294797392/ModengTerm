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
    /// 
    /// Ground状态机执行两个操作：
    /// 1. 执行C0控制字符集的动作
    /// 2. 打印其他可见字符
    /// </summary>
    public class StateMachineGround : StateMachine
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("StateMachineGround");

        public override VTStates Run()
        {
            byte ch = this.Context.CurrentChar;

            if (ASCIIChars.IsC0Code(ch) || ASCIIChars.IsDelete(ch))
            {
                // 执行动作
                this.NotifyEvent(ParserEvents.ExecuteAction);
            }
            else 
            {
                // 除了C0字符和删除键，其他所有的字符全部都打印
                this.NotifyEvent(ParserEvents.Print);
            }

            return VTStates.Ground;
        }
    }
}

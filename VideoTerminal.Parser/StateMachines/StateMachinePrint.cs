using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VideoTerminal.Parser.StateMachines
{
    public class StateMachinePrint : StateMachine
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("StateMachinePrint");

        public override int Run()
        {
            Console.Write((char)this.Context.CurrentChar);
            //logger.InfoFormat("打印字符:{0}", this.Context.CurrentChar);
            return ParserCode.SUCCESS;
        }
    }
}

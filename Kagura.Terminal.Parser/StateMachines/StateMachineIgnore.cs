using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VideoTerminal.Parser.StateMachines
{
    public class StateMachineIgnore : StateMachine
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("StateMachineIgnore");

        public override int Run()
        {
            throw new NotImplementedException();
        }
    }
}

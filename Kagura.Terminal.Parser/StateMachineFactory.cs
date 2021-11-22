using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VideoTerminal.Parser.StateMachines;

namespace VideoTerminal.Parser
{
    public static class StateMachineFactory
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("StateMachineFactory");

        public static StateMachine Create(StateTable table, StateMachineID smid)
        {
            switch (smid)
            {
                case StateMachineID.CASE_GROUND_STATE: return new StateMachineGround();
                case StateMachineID.CASE_ESC: return new StateMachineEscape();
                case StateMachineID.CASE_PRINT: return new StateMachinePrint();
                case StateMachineID.SMID_OSC: return new StateMachineOSC();

                default:
                    logger.ErrorFormat("未实现{0}:{1}的状态机处理器", table.State, smid);
                    throw new NotImplementedException(string.Format("未实现{0}:{1}的状态机处理器", table, smid));
            }
        }
    }
}


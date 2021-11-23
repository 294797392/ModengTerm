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

        public static StateMachine Create(StateTable table, VTStates state)
        {
            switch (state)
            {
                case VTStates.Ground: return new StateMachineGround();
                case VTStates.Escape: return new StateMachineEscape();
                case VTStates.OSC: return new StateMachineOSC();

                default:
                    logger.ErrorFormat("未实现{0}:{1}的状态机处理器", table.State, state);
                    throw new NotImplementedException(string.Format("未实现{0}:{1}的状态机处理器", table, state));
            }
        }

        public static StateMachine Create(VTStates state)
        {
            switch (state)
            {
                case VTStates.Ground: return new StateMachineGround();
                case VTStates.Escape: return new StateMachineEscape();
                case VTStates.OSC: return new StateMachineOSC();

                default:
                    logger.ErrorFormat("未实现{0}的状态机处理器", state);
                    throw new NotImplementedException(string.Format("未实现{0}的状态机处理器", state));
            }
        }
    }
}


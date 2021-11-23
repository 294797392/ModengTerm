using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VideoTerminal.Parser
{
    /// <summary>
    /// 标识一个处理终端字符状态的状态机
    /// </summary>
    public abstract class StateMachine
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("StateMachine");

        /// <summary>
        /// 状态机的事件
        /// </summary>
        internal event Action<StateMachine, ParserEvents> Event;

        /// <summary>
        /// 当前状态机的上下文信息
        /// </summary>
        internal VTParserContext Context { get; set; }

        /// <summary>
        /// 运行该状态机
        /// </summary>
        /// <returns>下次要转换到的状态</returns>
        public abstract VTStates Run();

        internal void NotifyEvent(ParserEvents evt)
        {
            if (this.Event != null)
            {
                this.Event(this, evt);
            }
        }
    }
}

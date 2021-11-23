using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VideoTerminal.Parser
{
    /// <summary>
    /// 定义解析器的事件类型
    /// </summary>
    public enum ParserActions
    {
        /// <summary>
        /// 执行一个C0字符集的动作
        /// </summary>
        ExecuteAction,

        /// <summary>
        /// 打印字符
        /// </summary>
        Print,

        /// <summary>
        /// OSC状态结束
        /// </summary>
        OSCCompleted,
    }
}

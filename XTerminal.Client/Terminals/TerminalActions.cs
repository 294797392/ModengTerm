using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XTerminal.Terminals
{
    /// <summary>
    /// 动作类型枚举
    /// 注释中的数字是8进制的ascii码
    /// </summary>
    public enum TerminalActions
    {
    }

    /// <summary>
    /// 解析了主机响应的数据之后，返回给Terminal要执行的动作
    /// </summary>
    public abstract class AbstractTerminalAction
    {
        public TerminalActions Type { get; }
    }

    public class PredefinedAction : AbstractTerminalAction
    {

    }
}
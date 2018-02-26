using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XTerminal.Connections;

namespace XTerminal.Simulators
{
    /// <summary>
    /// 终端模拟器
    /// 1.解析用户从键盘输入的字符
    /// 2.发送数据到真正的远程终端
    /// 3.接收终端响应的信息并解析成客户端可执行的命令
    /// 4.连接远程终端
    /// </summary>
    public interface ITerminalSimulator
    {
        event Action<object, IEnumerable<AbstractTerminalCommand>> CommandReceived;

        event Action<object, TerminalConnectionStatus> StatusChanged;

        ConnectionProtocols Protocols { get; set; }

        int ConnectionTerminal(IConnectionAuthorition authorition);

        int DisconnectTerminal();

        /// <summary>
        /// 处理用户从键盘输入的原始字符
        /// 做处理，并发送到远程终端
        /// </summary>
        void HandleUserInput(string key);
    }
}
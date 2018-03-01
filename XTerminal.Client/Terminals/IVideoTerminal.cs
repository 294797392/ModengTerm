using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using XTerminal.Connections;
using XTerminal.Terminal;

namespace XTerminal.Terminals
{
    /// <summary>
    /// 表示一个视频终端
    /// 1.解析用户从键盘输入的字符
    /// 2.发送数据到真正的远程终端
    /// 3.接收远程主机响应的信息并解析成终端可执行的动作
    /// 4.连接远程终端
    /// </summary>
    public interface IVideoTerminal
    {
        event Action<object, IEnumerable<AbstractTerminalAction>> CommandReceived;

        event Action<object, TerminalConnectionStatus> StatusChanged;

        ConnectionProtocols Protocols { get; set; }

        int ConnectionTerminal(IConnectionAuthorition authorition);

        int DisconnectTerminal();

        /// <summary>
        /// 处理用户从键盘输入的原始字符
        /// 做处理，并发送到远程终端
        /// </summary>
        void ProcessKeyDown(PressedKey key);
        
        void ProcessReceivedData(byte[] data);
    }
}
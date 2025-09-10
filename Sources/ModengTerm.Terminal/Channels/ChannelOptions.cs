using ModengTerm.Base.Enumerations.Terminal;
using ModengTerm.Base.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModengTerm.Base.DataModels.Ssh;
using System.IO.Ports;
using ModengTerm.Base.Enumerations.Ssh;

namespace ModengTerm.Terminal.Engines
{
    /// <summary>
    /// 初始化会话通道锁需要的数据
    /// </summary>
    public abstract class ChannelOptions
    {
        public abstract SessionTypeEnum SessionType { get; }

        /// <summary>
        /// 终端的行数
        /// </summary>
        public int Row { get; set; }

        /// <summary>
        /// 终端的列数
        /// </summary>
        public int Column { get; set; }

        /// <summary>
        /// 读取缓冲区的大小
        /// </summary>
        public int ReceiveBufferSize { get; set; }
    }

    public class LocalConsoleChannelOptions : ChannelOptions
    {
        public override SessionTypeEnum SessionType => SessionTypeEnum.Console;

        /// <summary>
        /// 如果该会话是LocalConsole会话
        /// 指定要使用的Win32控制台引擎
        /// </summary>
        public Win32ConsoleEngineEnum ConsoleEngin { get; set; }

        /// <summary>
        /// 要启动的进程名字
        /// 例如cmd.exe
        /// </summary>
        public string StartupPath { get; set; }

        /// <summary>
        /// 进程参数
        /// </summary>
        public string Arguments { get; set; }

        /// <summary>
        /// 初始目录
        /// </summary>
        public string StartupDir { get; set; }
    }

    public class SshChannelOptions : ChannelOptions
    {
        public override SessionTypeEnum SessionType => SessionTypeEnum.Ssh;

        /// <summary>
        /// 如果该会话是Ssh
        /// 指定要使用的Ssh验证方式
        /// </summary>
        public SSHAuthTypeEnum AuthenticationType { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string PrivateKeyId { get; set; }

        public string Passphrase { get; set; }

        public string ServerAddress { get; set; }

        public int ServerPort { get; set; }

        public string TerminalType { get; set; }

        public List<PortForward> PortForwards { get; set; }
    }

    public class SerialPortChannelOptions : ChannelOptions
    {
        public override SessionTypeEnum SessionType => SessionTypeEnum.SerialPort;

        public string PortName { get; set; }

        public int BaudRate { get; set; }

        public int DataBits { get; set; }

        public StopBits StopBits { get; set; }

        public Parity Parity { get; set; }

        public Handshake Handshake { get; set; }
    }

    public class TcpChannelOptions : ChannelOptions
    {
        public override SessionTypeEnum SessionType => SessionTypeEnum.Tcp;

        /// <summary>
        /// 服务器还是客户端
        /// </summary>
        public RawTcpTypeEnum Type { get; set; }

        /// <summary>
        /// 客户端：要连接的服务器的IP地址
        /// 服务器：要绑定到的网络接口的IP地址
        /// </summary>
        public string IPAddress { get; set; }

        /// <summary>
        /// 客户端：要连接的服务器的端口号
        /// 服务器：要监听的端口号
        /// </summary>
        public int Port { get; set; }
    }
}

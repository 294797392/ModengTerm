using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XTerminal.Connections
{
    /// <summary>
    /// 维护与远程终端的一个连接
    /// 1.管理与远程终端的连接
    /// 2.发送和接收远程终端的原始数据
    /// </summary>
    public interface IConnection
    {
        event Action<object, byte[]> DataReceived;

        event Action<object, TerminalConnectionStatus> StatusChanged;

        ConnectionProtocols Protocol { get; }

        IConnectionAuthorition Authorition { get; set; }

        TerminalConnectionStatus Status { get; }

        bool Initialize();

        bool Release();

        bool Connect();

        bool Disconnect();

        bool SendData(byte[] data);

        bool SendData(byte value);
    }
}
using GTerminalControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GTerminalConnection
{
    /// <summary>
    /// 管理虚拟终端与远程主机连接的接口
    /// </summary>
    public interface IConnection : IVTStream
    {
        ConnectionProtocols Protocol { get; }

        IConnectionAuthorition Authorition { get; set; }

        bool Connect();

        bool Disconnect();
    }
}
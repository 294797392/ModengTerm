using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GTerminalConnection
{
    /// <summary>
    /// 管理与远程主机的连接
    /// </summary>
    public interface IConnection : IVTStream
    {
        ConnectionProtocols Protocol { get; }

        IConnectionAuthorition Authorition { get; set; }

        bool Connect();

        bool Disconnect();
    }
}
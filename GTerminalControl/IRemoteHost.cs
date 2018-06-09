using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GTerminalControl
{
    /// <summary>
    /// 提供控制远程主机的接口
    /// </summary>
    public interface IRemoteHost
    {
        event Action<object, RemoteHostState> StatusChanged;

        /// <summary>
        /// 终端数据流
        /// </summary>
        IVTStream Stream { get; }

        bool Initialize();

        bool Release();

        bool Connect();

        bool Disconnect();

        bool SendData(byte[] data);

        bool SendData(byte data);
    }
}
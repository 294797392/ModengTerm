using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GTerminalControl
{
    public interface IRemoteHost
    {
        event Action<object, byte[]> DataReceived;

        bool Initialize();

        bool Release();

        bool Connect();

        bool Disconnect();

        bool SendData(byte[] data);

        bool SendData(byte data);
    }
}
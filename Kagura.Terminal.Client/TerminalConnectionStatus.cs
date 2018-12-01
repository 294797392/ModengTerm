using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kagura.Terminal.Client
{
    public enum TerminalConnectionStatus
    {
        Connecting,
        Connected,
        Disconnecting,
        Disconnected,
        ConnectError
    }
}
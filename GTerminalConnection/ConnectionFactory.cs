using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GTerminalConnection
{
    public static class ConnectionFactory
    {
        public static IConnection Create(ConnectionProtocols protocol)
        {
            switch (protocol)
            {
                case ConnectionProtocols.SSH:
                    return new SSHConnection();

                default:
                    throw new NotImplementedException();
            }
        }
    }
}
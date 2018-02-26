using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XTerminal.Connections
{
    public static class ConnectionFactory
    {
        public static IConnection Create(ConnectionProtocols protocol)
        {
            switch (protocol)
            {
                case ConnectionProtocols.Ssh:
                    return new SshConnection();

                default:
                    throw new NotImplementedException();
            }
        }
    }
}
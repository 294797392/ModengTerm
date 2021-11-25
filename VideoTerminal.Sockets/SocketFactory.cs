using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VideoTerminal.Sockets
{
    public static class SocketFactory
    {
        public static SocketBase Create(SocketTypes type, SocketAuthorition authorition)
        {
            switch (type)
            {
                case SocketTypes.SSH:
                    return new SSHSocket(authorition);

                default:
                    throw new NotImplementedException();
            }
        }

        public static SocketBase CreateSSHSocket(string ip, int port, string userName, string password)
        {
            SSHSocketAuthorition authorition = new SSHSocketAuthorition() 
            {
                ServerAddress = ip,
                ServerPort = port,
                UserName = userName,
                Password = password,
                TerminalName = "xterm-256color"
            };

            return SocketFactory.Create(SocketTypes.SSH, authorition);
        }
    }
}

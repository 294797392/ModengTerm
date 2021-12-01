using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VideoTerminal.Clients
{
    public static class ClientFactory
    {
        public static ClientBase Create(ClientTypes type, ClientAuthorition authorition)
        {
            switch (type)
            {
                case ClientTypes.SSH:
                    return new SSHClient(authorition);

                default:
                    throw new NotImplementedException();
            }
        }

        public static SSHClientAuthorition CreateSSHClientAuthorition(string ip, int port, string userName, string password)
        {
            return new SSHClientAuthorition() 
            {
                ServerAddress = ip,
                ServerPort = port,
                UserName = userName,
                Password = password,
                TerminalName = "xterm-256color"
            };
        }

        public static ClientBase CreateSSHClient(string ip, int port, string userName, string password)
        {
            ClientAuthorition authorition = CreateSSHClientAuthorition(ip, port, userName, password);
            return ClientFactory.Create(ClientTypes.SSH, authorition);
        }
    }
}

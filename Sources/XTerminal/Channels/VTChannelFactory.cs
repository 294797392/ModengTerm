using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VideoTerminal.Options;

namespace XTerminal.Channels
{
    public static class VTChannelFactory
    {
        public static VTChannel Create(VTInitialOptions options)
        {
            switch (options.ChannelType)
            {
                case VTChannelTypes.SSH: return new SSHChannel(options);
                default:
                    throw new NotImplementedException();
            }
        }

        //public static SSHChannelAuthorition CreateSSHClientAuthorition(string ip, int port, string userName, string password)
        //{
        //    return new SSHChannelAuthorition()
        //    {
        //        ServerAddress = ip,
        //        ServerPort = port,
        //        UserName = userName,
        //        Password = password,
        //        TerminalName = "xterm-256color"
        //    };
        //}

        //public static VTChannel CreateSSHClient(string ip, int port, string userName, string password)
        //{
        //    ChannelAuthorition authorition = CreateSSHClientAuthorition(ip, port, userName, password);
        //    return VTChannelFactory.Create(VTChannelTypes.SSH, authorition);
        //}
    }
}

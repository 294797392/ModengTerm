using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using VideoTerminal.Parser;
using VideoTerminal.Sockets;

namespace VideoTerminalConsole
{
    class Program
    {
        private static string ExternalLog4netConfig = "log4net.xml";

        static void Main(string[] args)
        {
            FileInfo configFile = new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ExternalLog4netConfig));
            if (configFile.Exists)
            {
                log4net.Config.XmlConfigurator.ConfigureAndWatch(configFile);
            }

            SSHSocket socket = new SSHSocket()
            {
                Authorition = new SSHSocketAuthorition()
                {
                    ServerAddress = "10.0.8.99",
                    UserName = "oheiheiheiheihei",
                    Password = "18612538605",
                    ServerPort = 22,
                    TerminalName = "xterm-256color"
                }
            };
            socket.StatusChanged += Socket_StatusChanged;
            socket.Connect();

            while (true)
            {
                string line = Console.ReadLine();
                socket.Write(Encoding.ASCII.GetBytes(line));
                socket.Write((byte)'\n');
            }
        }

        private static void Socket_StatusChanged(object sender, SocketState state)
        {
            Console.WriteLine("SSHClient StatusChanged, {0}", state);

            switch (state)
            {
                case SocketState.Ready:
                    {
                        VTParser main = new VTParser();
                        main.Run(sender as SSHSocket);
                        break;
                    }
            }
        }
    }
}

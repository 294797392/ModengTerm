using DotNEToolkit;
using DotNEToolkit.Extentions;
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

        private static VideoTerminal.Parser.VTKeyboard keyboard;

        static void Main(string[] args)
        {
            FileInfo configFile = new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ExternalLog4netConfig));
            if (configFile.Exists)
            {
                log4net.Config.XmlConfigurator.ConfigureAndWatch(configFile);
            }

            SocketBase socket = SocketFactory.CreateSSHSocket("10.0.8.99", 22, "oheiheiheiheihei", "18612538605");
            socket.StatusChanged += Socket_StatusChanged;
            socket.Connect();

            char[] chars = new char[1];
            while (true)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);

                VTKeys vtKey = VTKeyConverter.ConvertConsoleKey(key.Key);
                byte[] bytes = keyboard.TranslateKey(vtKey, VTModifierKeys.None);
                //chars[0] = key.KeyChar;
                //byte[] bytes = Encoding.UTF8.GetBytes(chars);

                socket.Write(bytes);
            }
        }

        private static void Socket_StatusChanged(object sender, SocketState state)
        {
            Console.WriteLine("SSHClient StatusChanged, {0}", state);

            switch (state)
            {
                case SocketState.Connected:
                    {
                        VTParser parser = new VTParser();
                        parser.Socket = sender as SocketBase;
                        parser.VideoTermianl = new ConsoleVT();
                        keyboard = parser.Keyboard;
                        parser.Initialize();
                        break;
                    }
            }
        }
    }
}

using DotNEToolkit;
using DotNEToolkit.Extentions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using VideoTerminal.Clients;
using VideoTerminal.Interface;
using VideoTerminal.Parser;

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

            ConsoleVT vt = new ConsoleVT();
            SSHClientAuthorition authorition = ClientFactory.CreateSSHClientAuthorition("10.0.8.99", 22, "oheiheiheiheihei", "18612538605");
            VTApplication vtApp = VTApplication.Run(authorition, vt);

            while (true)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;

namespace XTerminal.UnitTest
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        App()
        {
            //MessageBox.Show(((int)';').ToString());

            string log4netPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log4net.xml");
            if (File.Exists(log4netPath))
            {
                log4net.Config.XmlConfigurator.ConfigureAndWatch(new FileInfo(log4netPath));

                //MessageBox.Show(Convert.ToInt32("10011011", 2).ToString());
                //MessageBox.Show(((int)'A').ToString());
            }
        }
    }
}
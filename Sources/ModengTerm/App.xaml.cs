using ModengTerm;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using XTerminal.Base;

namespace XTerminal
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("App");

        public static double PixelsPerDip = 0;

        static App()
        {
            DotNEToolkit.Log4net.InitializeLog4net();
        }

        public App()
        {
        }

        private static void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            logger.Error(e.Exception);
            MessageBox.Show("客户端运行出现错误, 请联系作者");
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Application.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
            PixelsPerDip = VisualTreeHelper.GetDpi(new DrawingVisual()).PixelsPerDip;

            MTermApp.Context.Initialize("app.json");
        }
    }
}

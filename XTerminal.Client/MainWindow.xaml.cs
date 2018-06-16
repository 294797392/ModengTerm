using GTerminalConnection;
using GTerminalCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace XTerminalClient
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            SshConnection connection = new SshConnection();
            connection.Authorition = new SshConnectionAuthorition()
            {
                UserName = "zyf",
                Password = "18612538605",
                ServerAddress = "192.168.42.243",
                ServerPort = 22
            };
            GVideoTerminal gvt = new GVideoTerminal(connection);
            connection.Connect();
            VTConsole.VT = gvt;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            Console.WriteLine(e.Key.ToString());
        }
    }
}
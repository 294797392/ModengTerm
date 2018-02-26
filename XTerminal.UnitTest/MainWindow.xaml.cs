using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using XTerminal.Terminal;

namespace XTerminal.UnitTest
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

        }

        protected override void OnTextInput(TextCompositionEventArgs e)
        {
            base.OnTextInput(e);

            //TextList.HandleTextInput(e);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //if (ShellControl.Terminal == null)
            //{
            //    SSHTerminal terminal = new SSHTerminal();
            //    terminal.Authorition = new SSHTerminalAuthorition()
            //    {
            //        UserName = "zyf",
            //        Password = "18612538605",
            //        ServerAddress = "192.168.2.215",
            //        ServerPort = 22
            //    };
            //    ShellControl.Terminal = terminal;
            //}
        }
    }
}

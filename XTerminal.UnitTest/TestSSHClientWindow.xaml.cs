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
using System.Windows.Shapes;
using XTerminal.Terminal;

namespace XTerminal.UnitTest
{
    /// <summary>
    /// TestSSHClientWindow.xaml 的交互逻辑
    /// </summary>
    public partial class TestSSHClientWindow : Window
    {
        private SSHTerminal xtermTerminal;

        public TestSSHClientWindow()
        {
            InitializeComponent();
        }

        private void ButtonConnect_Click(object sender, RoutedEventArgs e)
        {
            if (this.xtermTerminal == null)
            {
                this.xtermTerminal = new SSHTerminal();
                this.xtermTerminal.Authorition = new SSHTerminalAuthorition()
                {
                    ServerPort = 22,
                    ServerAddress = "lizuo",
                    UserName = "lizuo",
                    Password = "18612538605"
                };
                this.xtermTerminal.DataReceived += this.XtermTerminal_DataReceived;
            }

            this.xtermTerminal.Connect();
        }


        private void ButtonSend_Click(object sender, RoutedEventArgs e)
        {
            this.xtermTerminal.Send(TextBoxCmd.Text + "\r");
        }

        private void XtermTerminal_DataReceived(object sender, byte[] data, string text)
        {
            base.Dispatcher.Invoke(new Action(() => 
            {
                TextBoxMessage.AppendText(Encoding.ASCII.GetString(data));
                ScrollViewer.ScrollToEnd();
            }));
        }
    }
}
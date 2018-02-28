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
using XTerminal.Connections;

namespace XTerminal.UnitTest
{
    /// <summary>
    /// TestSSHClientWindow.xaml 的交互逻辑
    /// </summary>
    public partial class TestSSHClientWindow : Window
    {
        private SshConnection xtermTerminal;

        public TestSSHClientWindow()
        {
            InitializeComponent();
        }

        private void ButtonConnect_Click(object sender, RoutedEventArgs e)
        {
            if (this.xtermTerminal == null)
            {
                this.xtermTerminal = new SshConnection();
                this.xtermTerminal.Authorition = new SshConnectionAuthorition()
                {
                    ServerPort = 22,
                    ServerAddress = "192.168.2.215",
                    UserName = "zyf",
                    Password = "18612538605"
                };
                this.xtermTerminal.DataReceived += XtermTerminal_DataReceived;
            }

            this.xtermTerminal.Connect();
        }

        private void XtermTerminal_DataReceived(object arg1, byte[] arg2)
        {
            base.Dispatcher.Invoke(new Action(() =>
            {
                string text = Encoding.ASCII.GetString(arg2);
                TextBoxMessage.AppendText(text);
                ScrollViewer.ScrollToEnd();
            }));
        }

        private void ButtonSend_Click(object sender, RoutedEventArgs e)
        {
            byte[] data = Encoding.ASCII.GetBytes("vim\n");
            //int ascii = Convert.ToByte("040", 8);
            this.xtermTerminal.SendData(data);

            //byte[] ascii = Encoding.ASCII.GetBytes("A");
            //this.xtermTerminal.SendData(ascii);
        }

        //private void XtermTerminal_CommandReceived(object sender, IEnumerable<IEscapeSequencesCommand> cmds)
        //{
        //    base.Dispatcher.Invoke(new Action(() => 
        //    {
        //        //TextBoxMessage.AppendText(Encoding.ASCII.GetString(data));
        //        //ScrollViewer.ScrollToEnd();
        //    }));
        //}
    }
}
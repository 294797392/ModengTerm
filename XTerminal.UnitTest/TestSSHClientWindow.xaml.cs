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
using XTerminal.CharacterParsers;
using XTerminal.Connections;
using XTerminal.Terminal;

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
                    ServerAddress = "192.168.2.200",
                    UserName = "zyf",
                    Password = "18612538605",
                    TerminalName = TerminalNames.TerminalXTerm
                };
                this.xtermTerminal.DataReceived += XtermTerminal_DataReceived;
            }

            this.xtermTerminal.Connect();
        }

        private void XtermTerminal_DataReceived(object arg1, byte[] arg2)
        {
            base.Dispatcher.Invoke(new Action(() =>
            {
                foreach (byte c in arg2)
                {
                    if (ControlFunctions.IsControlFunction(c))
                    {
                        Console.WriteLine("ControlFunction:{0}", c);
                    }
                }

                string text = Encoding.ASCII.GetString(arg2);

                TextBoxMessage.AppendText(text);
                ScrollViewer.ScrollToEnd();
            }));
        }

        private void ButtonSend_Click(object sender, RoutedEventArgs e)
        {
            byte[] data = Encoding.ASCII.GetBytes(string.Format("{0}\n", TextBoxCmd.Text));
            //int ascii = Convert.ToByte("040", 8);
            this.xtermTerminal.SendData(data);

            //byte[] ascii = Encoding.ASCII.GetBytes("A");
            //this.xtermTerminal.SendData(ascii);
        }

        private void ButtonDown_Click(object sender, RoutedEventArgs e)
        {
            byte byte1 = CharacterUtils.BitCombinations(9, 11);
            byte byte2 = (byte)'1';
            byte byte3 = (byte)'B';
            this.xtermTerminal.SendData(new byte[] { 27, (byte)'[', (byte)'B' });
        }

        private void ButtonUp_Click(object sender, RoutedEventArgs e)
        {
            byte byte1 = CharacterUtils.BitCombinations(9, 11);
            byte byte2 = (byte)'1';
            byte byte3 = (byte)'A';
            this.xtermTerminal.SendData(new byte[] { 27, (byte)'[', (byte)'A' });
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
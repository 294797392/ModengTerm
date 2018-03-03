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
using XTerminal.Terminals;

namespace XTerminal.UnitTest
{
    /// <summary>
    /// TestSSHClientWindow.xaml 的交互逻辑
    /// </summary>
    public partial class TestSSHClientWindow : Window
    {
        private AbstractTerminal terminal;

        public TestSSHClientWindow()
        {
            InitializeComponent();
        }

        private void ButtonConnect_Click(object sender, RoutedEventArgs e)
        {
            if (this.terminal == null)
            {
                this.terminal = new VT100Terminal();
                IConnectionAuthorition authorition = new SshConnectionAuthorition()
                {
                    ServerPort = 22,
                    ServerAddress = "192.168.2.200",
                    UserName = "zyf",
                    Password = "18612538605",
                    TerminalName = TerminalNames.TerminalXTerm
                };
                this.terminal.CommandReceived += Terminal_CommandReceived;
                this.terminal.ConnectionTerminal(authorition);
            }
        }

        private void Terminal_CommandReceived(object sender, IEnumerable<AbstractTerminalAction> commands, byte[] chars)
        {
            base.Dispatcher.Invoke(new Action(() =>
            {
                //foreach (byte c in arg2)
                //{
                //    if (ControlFunctions.IsControlFunction(c))
                //    {
                //        Console.WriteLine("ControlFunction:{0}", c);
                //    }
                //}

                string text = Encoding.ASCII.GetString(chars);

                TextBoxMessage.AppendText(text);
                ScrollViewer.ScrollToEnd();
            }));
        }

        private void ButtonSend_Click(object sender, RoutedEventArgs e)
        {
            byte[] data = Encoding.ASCII.GetBytes(string.Format("{0}\n", TextBoxCmd.Text));
            //int ascii = Convert.ToByte("040", 8);
            //this.terminal.ProcessKeyDown(data);

            //byte[] ascii = Encoding.ASCII.GetBytes("A");
            //this.xtermTerminal.SendData(ascii);
        }

        private void ButtonDown_Click(object sender, RoutedEventArgs e)
        {
            byte byte1 = CharacterUtils.BitCombinations(9, 11);
            byte byte2 = (byte)'1';
            byte byte3 = (byte)'B';
            //this.terminal.SendData(new byte[] { 27, (byte)'[', (byte)'B' });
        }

        private void ButtonUp_Click(object sender, RoutedEventArgs e)
        {
            byte byte1 = CharacterUtils.BitCombinations(9, 11);
            byte byte2 = (byte)'1';
            byte byte3 = (byte)'A';
            //this.terminal.SendData(new byte[] { 27, (byte)'[', (byte)'A' });
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);

            PressedKey key;
            key.Key = Utils.Utils.WPFKey2TerminalKey(e.Key);
            key.IsControlPressed = (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control;
            key.IsShiftPressed = (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift;
            key.IsUpperCase = false;

            if (key.Key != Keys.Null)
            {
                this.terminal.ProcessInputKey(key);
            }
            else
            {
                Console.WriteLine("未处理的按键:{0}", e.Key);
            }

            e.Handled = true;
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
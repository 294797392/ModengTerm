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
using VideoTerminal.Parser;
using VideoTerminal.Sockets;

namespace VideoTerminal
{
    public class Test : RichTextBox 
    {
        public Test()
        {
        }

        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {
            base.OnPreviewTextInput(e);

            Console.WriteLine(e.Text);

            e.Handled = true;
        }
    }

    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("MainWindow");

        #endregion

        #region 实例变量

        private SocketBase socket;
        private VTParser parser;

        #endregion

        public MainWindow()
        {
            InitializeComponent();

            this.socket = SocketFactory.CreateSSHSocket("10.0.8.99", 22, "oheiheiheiheihei", "18612538605");
            this.socket.StatusChanged += this.Socket_StatusChanged;

            this.parser = new VTParser();
            this.parser.Socket = this.socket;
            this.parser.VideoTermianl = VTConsole;
            this.parser.Initialize();

            this.socket.Connect();
        }

        private void Socket_StatusChanged(object sender, SocketState state)
        {
            logger.InfoFormat("Socket状态改变, {0}", state);
        }
    }
}
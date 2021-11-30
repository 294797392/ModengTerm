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
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VideoTerminal.Parser;
using VideoTerminal.Sockets;

namespace VideoTerminal
{
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
            this.parser.VideoTermianl = Terminal;
            this.parser.Initialize();

            this.socket.Connect();

            //Terminal.TestRender();
        }

        public const int WM_IME_CHAR = 0x0286;
        public const int WM_CHAR = 0x0102;
        public const int WM_UNICHAR = 0x0109;
        public const int WM_IME_STARTCOMPOSITION = 0x010D;
        public const int WM_IME_ENDCOMPOSITION = 0x010E;
        public const int WM_IME_COMPOSITION = 0x010F;

        public const int WM_IME_NOTIFY = 0x0282;
        public const int WM_IME_REQUEST = 0x0288;

        public IntPtr WpfHandleWinowMsg(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case WM_IME_NOTIFY:
                    {
                        Console.WriteLine("WM_IME_NOTIFY");
                        break;
                    }

                case WM_IME_REQUEST:
                    {
                        Console.WriteLine("WM_IME_REQUEST");
                        break;
                    }

                case WM_IME_STARTCOMPOSITION:
                    {
                        Console.WriteLine("WM_IME_STARTCOMPOSITION");
                        break;
                    }

                case WM_IME_ENDCOMPOSITION:
                    {
                        Console.WriteLine("WM_IME_ENDCOMPOSITION");
                        break;
                    }

                case WM_IME_COMPOSITION:
                    {
                        Console.WriteLine("WM_IME_COMPOSITION");
                        break;
                    }

                case WM_UNICHAR:
                    {
                        Console.WriteLine("WM_UNICHAR");
                        break;
                    }

                case WM_IME_CHAR:
                    {
                        Console.WriteLine("WM_IME_CHAR");
                        break;
                    }

                case WM_CHAR:
                    {
                        Console.WriteLine("WM_CHAR");
                        break;
                    }


                case 0x0100:
                case 0x0101:
                case 513:
                case 514:
                case 32:
                case 132:
                case 0x200:
                    {
                        break;
                    }

                default:
                    logger.InfoFormat(msg.ToString());
                    break;
            }

            return IntPtr.Zero;
        }

        private void Socket_StatusChanged(object sender, SocketState state)
        {
            logger.InfoFormat("Socket状态改变, {0}", state);
        }
    }
}

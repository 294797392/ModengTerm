using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using VideoTerminal.Options;
using XTerminal.Channels;

namespace XTerminal
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

        #endregion

        #region 构造方法

        public MainWindow()
        {
            InitializeComponent();

            //Terminal.TestRender();

            this.InitializeWindow();
        }

        #endregion

        #region 实例方法

        private void InitializeWindow()
        {
            VideoTerminal videoTermianl = new VideoTerminal();
            videoTermianl.Monitor = ConsoleMonitor.DrawingCanvas;
            videoTermianl.InputDevice = ConsoleMonitor;
            videoTermianl.Initialize(VTInitialOptions.Home);
        }

        #endregion
    }
}

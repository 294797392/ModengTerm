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
using VideoTerminal.Clients;
using VideoTerminal.Parser;

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

        public MainWindow()
        {
            InitializeComponent();

            //Terminal.TestRender();

            this.InitializeWindow();
        }

        private void InitializeWindow()
        {
            SSHClientAuthorition authorition = ClientFactory.CreateSSHClientAuthorition("10.0.8.99", 22, "oheiheiheiheihei", "18612538605");
            VTApplication vtApp = VTApplication.Run(authorition, Terminal);
        }
    }
}

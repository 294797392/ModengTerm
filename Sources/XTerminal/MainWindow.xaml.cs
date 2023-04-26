using DotNEToolkit;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using XTerminal.Session.Property;
using XTerminal.ViewModels;
using XTerminal.Windows;

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

            this.InitializeWindow();
        }

        #endregion

        #region 实例方法

        private void InitializeWindow()
        {
        }

        #endregion

        #region 事件处理器

        private void ButtonCreateSession_Click(object sender, RoutedEventArgs e)
        {
            CreateSessionWindow window = new CreateSessionWindow();
            window.Owner = this;
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            if ((bool)window.ShowDialog())
            {
                
            }
        }

        #endregion
    }
}

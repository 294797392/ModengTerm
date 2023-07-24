using DotNEToolkit;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using WPFToolkit.MVVM;
using WPFToolkit.Utility;
using XTerminal.Base;
using XTerminal.Base.DataModels;
using XTerminal.ViewModels;
using XTerminal.Windows;

namespace XTerminal
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SessionListWindow : Window
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("MainWindow");

        #endregion

        #region 属性

        /// <summary>
        /// 当前选中的会话
        /// </summary>
        public XTermSession SelectedSession { get; private set; }

        #endregion

        #region 实例变量

        #endregion

        #region 构造方法

        public SessionListWindow()
        {
            InitializeComponent();

            this.InitializeWindow();
        }

        #endregion

        #region 实例方法

        private void InitializeWindow()
        {
            List<XTermSession> sessions = XTermApp.Context.ServiceAgent.GetSessions();

            BindableCollection<XTermSessionVM> sessionVMs = new BindableCollection<XTermSessionVM>();

            foreach (XTermSession session in sessions)
            {
                XTermSessionVM sessionVM = new XTermSessionVM(session);
                sessionVMs.Add(sessionVM);
            }

            DataGridSessionList.DataContext = sessionVMs;
        }

        private void OpenSession(XTermSessionVM sessionVM)
        {
            this.SelectedSession = sessionVM.Session;

            base.DialogResult = true;
        }

        #endregion

        #region 事件处理器

        private void ButtonCreateSession_Click(object sender, RoutedEventArgs e)
        {
            CreateSessionWindow window = new CreateSessionWindow();
            window.Owner = this;
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            if (!(bool)window.ShowDialog())
            {
                return;
            }

            XTermSession session = window.Session;

            // 在数据库里新建会话
            int code = XTermApp.Context.ServiceAgent.AddSession(session);
            if (code != ResponseCode.SUCCESS)
            {
                MessageBoxUtils.Error("新建会话失败, 错误码 = {0}", code);
                return;
            }

            this.SelectedSession = session;

            base.DialogResult = true;
        }

        private void ButtonOpenSession_Click(object sender, RoutedEventArgs e)
        {
            DataGridSessionList_MouseDoubleClick(null, null);
        }

        private void DataGridSessionList_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            XTermSessionVM sessionVM = DataGridSessionList.SelectedItem as XTermSessionVM;
            if (sessionVM == null)
            {
                MessageBoxUtils.Info("请选择要打开的会话");
                return;
            }

            this.OpenSession(sessionVM);
        }

        #endregion
    }
}

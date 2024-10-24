﻿using ModengTerm.Base;
using ModengTerm.Base.DataModels;
using ModengTerm.Controls;
using ModengTerm.ViewModels;
using System.Collections.Generic;
using System.Windows;
using WPFToolkit.MVVM;
using XTerminal.Windows;

namespace ModengTerm.Windows
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SessionListWindow : MdWindow
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("MainWindow");

        #endregion

        #region 属性

        /// <summary>
        /// 当前选中的会话
        /// </summary>
        public XTermSession SelectedSession { get; private set; }

        /// <summary>
        /// 当前选中的会话ViewModel
        /// </summary>
        public XTermSessionVM SelectedSessionVM { get; private set; }

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
            List<XTermSession> sessions = MTermApp.Context.ServiceAgent.GetSessions();

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
            this.SelectedSessionVM = sessionVM;
            this.SelectedSession = sessionVM.Session;

            base.DialogResult = true;
        }

        #endregion

        #region 事件处理器

        private void ButtonCreateSession_Click(object sender, RoutedEventArgs e)
        {
            CreateSessionOptionTreeWindow window = new CreateSessionOptionTreeWindow();
            window.Owner = this;
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            if (!(bool)window.ShowDialog())
            {
                return;
            }

            XTermSession session = window.Session;

            // 在数据库里新建会话
            int code = MTermApp.Context.ServiceAgent.AddSession(session);
            if (code != ResponseCode.SUCCESS)
            {
                MTMessageBox.Error("新建会话失败, {0}, {1}", code, ResponseCode.GetMessage(code));
                return;
            }

            this.SelectedSession = session;

            base.DialogResult = true;
        }

        private void ButtonDeleteSession_Click(object sender, RoutedEventArgs e)
        {
            XTermSessionVM selectedSession = DataGridSessionList.SelectedItem as XTermSessionVM;
            if (selectedSession == null)
            {
                MTMessageBox.Info("请选择要删除的会话");
                return;
            }

            if (!MTMessageBox.Confirm("确定要删除{0}吗", selectedSession.Name))
            {
                return;
            }

            int code = MTermApp.Context.ServiceAgent.DeleteSession(selectedSession.ID.ToString());
            if (code != ResponseCode.SUCCESS)
            {
                MTMessageBox.Error("删除会话失败, {0}, {1}", code, ResponseCode.GetMessage(code));
                return;
            }

            BindableCollection<XTermSessionVM> sessionVMs = DataGridSessionList.DataContext as BindableCollection<XTermSessionVM>;
            sessionVMs.Remove(selectedSession);

            DataGridSessionList.SelectedIndex = 0;
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
                return;
            }

            this.OpenSession(sessionVM);
        }

        #endregion
    }
}

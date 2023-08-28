using ModengTerm.Terminal.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WPFToolkit.Utility;
using XTerminal.Base;
using XTerminal.Base.DataModels;
using XTerminal.Base.Enumerations;
using XTerminal.UserControls;
using XTerminal.ViewModels;
using XTerminal.Windows;

namespace XTerminal
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private OpenedSessionDataTemplateSelector templateSelector;

        public MainWindow()
        {
            InitializeComponent();

            this.InitializeWindow();
        }

        #region 实例方法

        private void InitializeWindow()
        {
            this.templateSelector = new OpenedSessionDataTemplateSelector();
            this.templateSelector.DataTemplateOpenedSession = this.FindResource("DataTemplateOpenedSession") as DataTemplate;
            this.templateSelector.DataTemplateOpenSession = this.FindResource("DataTemplateOpenSession") as DataTemplate;
            ListBoxOpenedSessionTab.ItemTemplateSelector = this.templateSelector;
        }

        private void OpenSession()
        {
            SessionListWindow sessionListWindow = new SessionListWindow();
            sessionListWindow.Owner = this;
            sessionListWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            if ((bool)sessionListWindow.ShowDialog())
            {
                XTermSession session = sessionListWindow.SelectedSession;

                this.OpenSession(session);
            }
        }

        private void OpenSession(XTermSession session)
        {
            OpenedSessionVM openedSession = XTermApp.Context.OpenSession(session);
            ContentControlTerminal.Content = openedSession.Content;

            SessionContent sessionContent = openedSession.Content as SessionContent;
            if (sessionContent != null)
            {
                sessionContent.Open(openedSession);
            }
        }

        /// <summary>
        /// 切换要显示的Session
        /// </summary>
        /// <param name="switchTo">要切换显示的Session</param>
        private void SwitchSession(OpenedSessionVM switchTo)
        {
            if (ContentControlTerminal.Content != switchTo.Content)
            {
                ContentControlTerminal.Content = switchTo.Content;
            }
        }

        #endregion

        #region 事件处理器

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.OpenSession();
        }

        private void ListBoxOpenedSessionTab_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OpenedSessionVM selectedTabItem = ListBoxOpenedSessionTab.SelectedItem as OpenedSessionVM;
            if (selectedTabItem == null)
            {
                return;
            }

            if (selectedTabItem is OpenSessionVM)
            {
                // 点击的是打开Session按钮，返回到上一个选中的SessionTabItem
                if (e.RemovedItems.Count > 0)
                {
                    ListBoxOpenedSessionTab.SelectedItem = e.RemovedItems[0];
                }

                this.OpenSession();
            }
            else
            {
                this.SwitchSession(selectedTabItem);
            }
        }

        // 关闭会话
        private void PathClose_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;
            VideoTerminal openedSession = element.Tag as VideoTerminal;
            if (openedSession == null)
            {
                return;
            }

            XTermApp.Context.CloseSession(openedSession);
        }

        private void MenuItemOpenSession_Click(object sender, RoutedEventArgs e)
        {
            this.OpenSession();
        }

        private void MenuItemCreateSession_Click(object sender, RoutedEventArgs e)
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
            int code = XTermApp.Context.ServiceAgent.AddSession(session);
            if (code != ResponseCode.SUCCESS)
            {
                MessageBoxUtils.Error("新建会话失败, 错误码 = {0}", code);
                return;
            }

            // 打开会话
            this.OpenSession(session);
        }

        private void MenuItemAbout_Click(object sender, RoutedEventArgs e)
        {
            AboutWindow aboutWindow = new AboutWindow();
            aboutWindow.Owner = this;
            aboutWindow.ShowDialog();
        }

        private void CheckBoxEnableDebugMode_CheckedChanged(object sender, RoutedEventArgs e)
        {
            //VTDebug.Enabled = CheckBoxEnableDebugMode.IsChecked.Value;
        }

        private void MenuItemDebugWindow_Click(object sender, RoutedEventArgs e)
        {
            SessionContent sessionContent = ContentControlTerminal.Content as SessionContent;
            VideoTerminal terminalSession = sessionContent.DataContext as VideoTerminal;
            if (terminalSession == null)
            {
                return;
            }

            DebugWindow debugWindow = new DebugWindow();
            debugWindow.VideoTerminal = terminalSession;
            debugWindow.Owner = this;
            debugWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            debugWindow.ShowDialog();
        }

        #endregion
    }

    public class OpenedSessionDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate DataTemplateOpenedSession { get; set; }
        public DataTemplate DataTemplateOpenSession { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is OpenSessionVM)
            {
                return this.DataTemplateOpenSession;
            }
            else
            {
                return this.DataTemplateOpenedSession;
            }
        }
    }
}

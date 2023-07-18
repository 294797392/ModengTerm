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
using XTerminal.UserControls;
using XTerminal.ViewModels;

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
            if ((bool)sessionListWindow.ShowDialog())
            {
                XTermSession session = sessionListWindow.SelectedSession;

                // 创建TerminalControl
                TerminalScreenUserControl terminalControl = new TerminalScreenUserControl();
                ContentControlTerminal.Content = terminalControl;

                // 打开Session
                int code = XTermApp.Context.OpenSession(session, terminalControl);
                if (code != ResponseCode.SUCCESS)
                {
                    MessageBoxUtils.Error("打开会话失败, {0}", ResponseCode.GetMessage(code));
                }
            }
        }

        /// <summary>
        /// 切换要显示的Session
        /// </summary>
        /// <param name="openedSessionVM"></param>
        private void SwitchSession(OpenedSessionVM openedSessionVM)
        {
            TerminalScreenUserControl terminalUserControl = openedSessionVM.TerminalScreen as TerminalScreenUserControl;
            if (ContentControlTerminal.Content == terminalUserControl)
            {
                return;
            }

            ContentControlTerminal.Content = terminalUserControl;
        }

        #endregion

        #region 事件处理器

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.OpenSession();
        }

        private void ListBoxOpenedSessionTab_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SessionTabItemVM selectedTabItem = ListBoxOpenedSessionTab.SelectedItem as SessionTabItemVM;
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
            else if (selectedTabItem is OpenedSessionVM)
            {
                this.SwitchSession(selectedTabItem as OpenedSessionVM);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        // 关闭会话
        private void PathClose_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;
            OpenedSessionVM openedSession = element.Tag as OpenedSessionVM;
            if (openedSession == null)
            {
                return;
            }

            XTermApp.Context.CloseSession(openedSession);
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
            else if (item is OpenedSessionVM)
            {
                return this.DataTemplateOpenedSession;
            }
            else
            {
                return base.SelectTemplate(item, container);
            }
        }
    }
}

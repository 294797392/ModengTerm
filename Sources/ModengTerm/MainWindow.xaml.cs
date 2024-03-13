using ModengTerm;
using ModengTerm.Base;
using ModengTerm.Base.DataModels;
using ModengTerm.Controls;
using ModengTerm.Rendering;
using ModengTerm.Terminal;
using ModengTerm.Terminal.ViewModels;
using ModengTerm.ViewModels;
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
using XTerminal.Windows;

namespace XTerminal
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        #region 实例变量

        private OpenedSessionDataTemplateSelector templateSelector;
        private UserInput userInput;

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
            this.userInput = new UserInput();
            this.templateSelector = new OpenedSessionDataTemplateSelector();
            this.templateSelector.DataTemplateOpenedSession = this.FindResource("DataTemplateOpenedSession") as DataTemplate;
            this.templateSelector.DataTemplateOpenSession = this.FindResource("DataTemplateOpenSession") as DataTemplate;
            ListBoxOpenedSessionTab.ItemTemplateSelector = this.templateSelector;
        }

        private void CreateSession()
        {
            SessionListWindow sessionListWindow = new SessionListWindow();
            sessionListWindow.Owner = this;
            sessionListWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            if ((bool)sessionListWindow.ShowDialog())
            {
                XTermSession session = sessionListWindow.SelectedSession;

                MTermApp.Context.OpenSession(session, ContentControlSession);
            }
        }

        /// <summary>
        /// 向SSH服务器发送输入
        /// </summary>
        /// <param name="userInput"></param>
        private void SendUserInput(ShellSessionVM sendTo, UserInput userInput)
        {
            if (sendTo.SendAll)
            {
                foreach (ShellSessionVM shellSession in MTermApp.Context.OpenedTerminals)
                {
                    shellSession.SendInput(userInput);
                }
            }
            else
            {
                sendTo.SendInput(userInput);
            }
        }

        #endregion

        #region 公开接口

        public void SendToAllTerminal(string text)
        {
            foreach (ShellSessionVM shellSession in MTermApp.Context.OpenedTerminals)
            {
                shellSession.SendInput(text);
            }
        }

        #endregion

        #region 事件处理器

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.CreateSession();
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
                else
                {
                    // 如果当前没有任何一个打开的Session，那么重置选中状态，以便于下次可以继续触发SelectionChanged事件
                    ListBoxOpenedSessionTab.SelectedItem = null;
                }

                this.CreateSession();
            }
            else
            {
                ContentControlSession.Content = selectedTabItem.Content;
            }
        }

        // 关闭会话
        private void PathClose_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;
            ShellSessionVM openedSession = element.Tag as ShellSessionVM;
            if (openedSession == null)
            {
                return;
            }

            MTermApp.Context.CloseSession(openedSession);

            MTermApp.Context.SelectedOpenedSession = MTermApp.Context.OpenedTerminals.FirstOrDefault();

            if (MTermApp.Context.SelectedOpenedSession == null)
            {
                ContentControlSession.Content = null;
                ListBoxOpenedSessionTab.SelectedItem = null;
            }
        }

        private void MenuItemOpenSession_Click(object sender, RoutedEventArgs e)
        {
            this.CreateSession();
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
            int code = MTermApp.Context.ServiceAgent.AddSession(session);
            if (code != ResponseCode.SUCCESS)
            {
                MessageBoxUtils.Error("新建会话失败, 错误码 = {0}", code);
                return;
            }

            // 打开会话
            this.CreateSession();
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
            //ISessionContent sessionContent = ContentControlSession.Content as ISessionContent;
            //VideoTerminal terminalSession = sessionContent.DataContext as VideoTerminal;
            //if (terminalSession == null)
            //{
            //    return;
            //}

            //DebugWindow debugWindow = new DebugWindow();
            //debugWindow.VideoTerminal = terminalSession;
            //debugWindow.Owner = this;
            //debugWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            //debugWindow.ShowDialog();
        }




        /// <summary>
        /// 输入中文的时候会触发该事件
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {
            base.OnPreviewTextInput(e);

            ShellSessionVM videoTerminal = ListBoxOpenedSessionTab.SelectedItem as ShellSessionVM;
            if (videoTerminal == null)
            {
                return;
            }

            this.userInput.CapsLock = Console.CapsLock;
            this.userInput.Key = VTKeys.GenericText;
            this.userInput.Text = e.Text;
            this.userInput.Modifiers = VTModifierKeys.None;

            this.SendUserInput(videoTerminal, this.userInput);

            e.Handled = true;
        }

        /// <summary>
        /// 从键盘上按下按键的时候会触发
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);

            ShellSessionVM shellSession = ListBoxOpenedSessionTab.SelectedItem as ShellSessionVM;
            if (shellSession == null)
            {
                return;
            }

            if (e.Key == Key.ImeProcessed)
            {
                // 这些字符交给输入法处理了
            }
            else
            {
                switch (e.Key)
                {
                    case Key.Tab:
                    case Key.Up:
                    case Key.Down:
                    case Key.Left:
                    case Key.Right:
                    case Key.Space:
                        {
                            // 防止焦点移动到其他控件上了
                            e.Handled = true;
                            break;
                        }
                }

                if (e.Key != Key.ImeProcessed)
                {
                    e.Handled = true;
                }

                VTKeys vtKey = TermUtils.ConvertToVTKey(e.Key);
                this.userInput.CapsLock = Console.CapsLock;
                this.userInput.Key = vtKey;
                this.userInput.Text = null;
                this.userInput.Modifiers = (VTModifierKeys)e.KeyboardDevice.Modifiers;
                this.SendUserInput(shellSession, this.userInput);
            }

            e.Handled = true;
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

using ModengTerm.Base;
using ModengTerm.Base.DataModels;
using ModengTerm.Base.Enumerations;
using ModengTerm.Base.Enumerations.Terminal;
using ModengTerm.Base.ServiceAgents;
using ModengTerm.Controls;
using ModengTerm.Terminal;
using ModengTerm.Terminal.Enumerations;
using ModengTerm.Terminal.ViewModels;
using ModengTerm.UserControls;
using ModengTerm.ViewModels;
using ModengTerm.ViewModels.Terminals;
using ModengTerm.Windows;
using ModengTerm.Windows.SSH;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WPFToolkit.MVVM;
using WPFToolkit.Utility;
using XTerminal.Windows;

namespace ModengTerm
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        #region 实例变量

        private OpenedSessionDataTemplateSelector templateSelector;
        private OpenedSessionItemContainerStyleSelector itemContainerStyleSelector;
        private VTKeyboardInput userInput;
        private MainWindowVM mainWindowVM;
        private OpenedSessionsVM openedSessionsVM;
        private ServiceAgent serviceAgent;
        private HomePageUserControl homePageUserControl;

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
            this.serviceAgent = MTermApp.Context.ServiceAgent;

            this.mainWindowVM = MTermApp.Context.MainWindowVM;
            base.DataContext = this.mainWindowVM;

            this.userInput = new VTKeyboardInput();
            this.templateSelector = new OpenedSessionDataTemplateSelector();
            this.templateSelector.DataTemplateOpenedSession = this.FindResource("DataTemplateOpenedSession") as DataTemplate;
            this.templateSelector.DataTemplateOpenSession = this.FindResource("DataTemplateOpenSession") as DataTemplate;
            ListBoxOpenedSession.ItemTemplateSelector = this.templateSelector;

            this.itemContainerStyleSelector = new OpenedSessionItemContainerStyleSelector();
            this.itemContainerStyleSelector.StyleOpenedSession = this.FindResource("StyleListBoxItemOpenedSession") as Style;
            this.itemContainerStyleSelector.StyleOpenSession = this.FindResource("StyleListBoxItemOpenSession") as Style;
            ListBoxOpenedSession.ItemContainerStyleSelector = this.itemContainerStyleSelector;

            this.openedSessionsVM = this.mainWindowVM.OpenedSessionsVM;
            this.openedSessionsVM.OnSessionOpened += OpenedSessionsVM_OnSessionOpened;
            ListBoxOpenedSession.DataContext = this.openedSessionsVM;
            ListBoxOpenedSession.AddHandler(ListBox.MouseWheelEvent, new MouseWheelEventHandler(this.ListBoxOpenedSession_MouseWheel), true);
        }

        private void ShowSessionListWindow()
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

        /// <summary>
        /// 执行打开会话动作
        /// </summary>
        /// <param name="session">要打开的会话</param>
        /// <param name="addToRecent">是否加入到最新打开的会话列表里</param>
        private void OpenSession(XTermSession session, bool addToRecent = true)
        {
            ISessionContent content = this.openedSessionsVM.OpenSession(session);
            ContentControlSession.Content = content;
            ScrollViewerOpenedSession.ScrollToRightEnd();

            // 增加到最近打开列表里
            if (addToRecent)
            {
                this.mainWindowVM.AddToRecentSession(session);
            }
        }

        private void OpenDefaultSession()
        {
            XTermSession defaultSession = MTermApp.Context.Manifest.DefaultSession;
            if (defaultSession == null)
            {
                return;
            }

            string cmdPath = System.IO.Path.Combine(Environment.SystemDirectory, "cmd.exe");
            defaultSession.SetOption<string>(OptionKeyEnum.CMD_STARTUP_PATH, cmdPath);

            // 如果是Win10或者更高版本的操作系统，那么使用PseudoConsoleAPI
            if (MTermUtils.IsWin10())
            {
                defaultSession.SetOption<CmdDriverEnum>(OptionKeyEnum.CMD_DRIVER, CmdDriverEnum.Win10PseudoConsoleApi);
            }
            else
            {
                // 如果是Win10以下的系统，使用winpty.dll
                defaultSession.SetOption<CmdDriverEnum>(OptionKeyEnum.CMD_DRIVER, CmdDriverEnum.winpty);
            }

            this.OpenSession(defaultSession, false);
        }

        #endregion

        #region 公开接口

        public void SendToAllTerminal(string text)
        {
            foreach (ShellSessionVM shellSession in this.openedSessionsVM.ShellSessions)
            {
                shellSession.SendText(text);
            }
        }

        #endregion

        #region 事件处理器


        /// <summary>
        /// 当会话被打开之后触发
        /// 会话在OnLoad事件里打开
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="openedSession">被打开的会话</param>
        private void OpenedSessionsVM_OnSessionOpened(OpenedSessionsVM arg1, OpenedSessionVM openedSession)
        {
            this.mainWindowVM.TitleMenus.Clear();
            this.mainWindowVM.TitleMenus.AddRange(openedSession.ContextMenus);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //this.CreateSession();

            // 直接打开Windows命令行，可以更快速的进入工作状态
            // TODO：做成可选项，可以直接打开命令行，也能打开会话列表

            this.OpenDefaultSession();
        }

        private void ListBoxOpenedSession_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.mainWindowVM.TitleMenus.Clear();

            SessionItemVM selectedSession = ListBoxOpenedSession.SelectedItem as SessionItemVM;
            if (selectedSession == null)
            {
                return;
            }

            if (selectedSession is OpenSessionVM)
            {
                if (e.RemovedItems.Count > 0)
                {
                    // 点击的是打开Session按钮，返回到上一个选中的SessionTabItem
                    ListBoxOpenedSession.SelectedItem = e.RemovedItems[0];
                }
                else
                {
                    // 如果当前没有任何一个打开的Session，那么重置选中状态，以便于下次可以继续触发SelectionChanged事件
                    ListBoxOpenedSession.SelectedItem = null;
                }
            }
            else
            {
                OpenedSessionVM openedSessionVM = selectedSession as OpenedSessionVM;
                ContentControlSession.Content = openedSessionVM.Content;
            }

            // 如果选中的会话是Shell会话并且显示了查找窗口，那么搜索选中的会话
            if (selectedSession is ShellSessionVM)
            {
                ShellSessionVM shellSession = selectedSession as ShellSessionVM;

                if (FindWindowMgr.WindowShown)
                {
                    FindWindowMgr.Show(shellSession);
                }

                IVideoTerminal vt = shellSession.VideoTerminal;
                if (vt != null)
                {
                    vt.ActiveDocument.EventInput.OnLoaded();
                }
            }

            if (selectedSession is OpenedSessionVM)
            {
                OpenedSessionVM openedSessionVM = selectedSession as OpenedSessionVM;
                if (openedSessionVM.ContextMenus != null)
                {
                    this.mainWindowVM.TitleMenus.AddRange(openedSessionVM.ContextMenus);
                }
            }
        }

        private void ListBoxOpenedSession_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            double offset = ScrollViewerOpenedSession.HorizontalOffset;

            if (e.Delta > 0)
            {
                ScrollViewerOpenedSession.ScrollToHorizontalOffset(offset - 50);
            }
            else
            {
                ScrollViewerOpenedSession.ScrollToHorizontalOffset(offset + 50);
            }
        }

        private void ButtonOpenSession_Click(object sender, RoutedEventArgs e)
        {
            this.ShowSessionListWindow();
        }

        private void ButtonCloseSession_Click(object sender, RoutedEventArgs e)
        {
            FrameworkElement frameworkElement = sender as FrameworkElement;
            OpenedSessionVM openedSessionVM = frameworkElement.DataContext as OpenedSessionVM;

            this.openedSessionsVM.CloseSession(openedSessionVM);

            this.openedSessionsVM.SelectedSession = this.openedSessionsVM.SessionList.OfType<OpenedSessionVM>().FirstOrDefault();

            if (this.openedSessionsVM.SelectedSession == null)
            {
                ContentControlSession.Content = null;
                ListBoxOpenedSession.SelectedItem = null;
            }

            if (this.openedSessionsVM.SessionList.Count == 1 &&
                this.openedSessionsVM.SessionList[0] is OpenSessionVM)
            {
                if (this.homePageUserControl == null)
                {
                    this.homePageUserControl = new HomePageUserControl();
                }
                ContentControlSession.Content = this.homePageUserControl;

                //this.ShowSessionListWindow();
            }
        }



        private void MenuItemOpenSession_Click(object sender, RoutedEventArgs e)
        {
            this.ShowSessionListWindow();
        }

        private void MenuItemCreateSession_Click(object sender, RoutedEventArgs e)
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
            int code = MTermApp.Context.ServiceAgent.AddSession(session);
            if (code != ResponseCode.SUCCESS)
            {
                MessageBoxUtils.Error("新建会话失败, 错误码 = {0}", code);
                return;
            }

            // 打开会话
            this.OpenSession(session, false);
        }

        private void MenuItemGroupManager_Click(object sender, RoutedEventArgs e)
        {
            GroupManagerWindow window = new GroupManagerWindow();
            window.Owner = this;
            window.ShowDialog();
        }

        private void MenuItemAbout_Click(object sender, RoutedEventArgs e)
        {
            AboutWindow aboutWindow = new AboutWindow();
            aboutWindow.Owner = this;
            aboutWindow.ShowDialog();
        }

        private void MenuItemOpenRecentSessions_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            RecentlySessionVM recentSession = menuItem.DataContext as RecentlySessionVM;

            XTermSession session = this.serviceAgent.GetSession(recentSession.SessionId);
            if (session == null)
            {
                if (MTMessageBox.Confirm("会话不存在, 是否从列表里删除?"))
                {
                    this.mainWindowVM.DeleteRecentSession(recentSession);
                    return;
                }
            }

            this.OpenSession(session, false);
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = e.OriginalSource as MenuItem;
            SessionContextMenu contextMenu = menuItem.DataContext as SessionContextMenu;
            contextMenu.Execute();
        }



        private void ButtonMinmizedWindow_Click(object sender, RoutedEventArgs e)
        {
            base.WindowState = WindowState.Minimized;
        }

        private void ButtonMaxmizedWindow_Click(object sender, RoutedEventArgs e)
        {
            if (base.WindowState == WindowState.Normal)
            {
                base.WindowState = WindowState.Maximized;
            }
            else
            {
                base.WindowState = WindowState.Normal;
            }
        }

        private void ButtonCloseWindow_Click(object sender, RoutedEventArgs e)
        {
            base.Close();
        }


        ///// <summary>
        ///// 输入中文的时候会触发该事件
        ///// </summary>
        ///// <param name="e"></param>
        //protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        //{
        //    base.OnPreviewTextInput(e);

        //    InputSessionVM selectedSession = ListBoxOpenedSession.SelectedItem as InputSessionVM;
        //    if (selectedSession == null)
        //    {
        //        return;
        //    }

        //    this.userInput.CapsLock = Console.CapsLock;
        //    this.userInput.Key = VTKeys.GenericText;
        //    this.userInput.Text = e.Text;
        //    this.userInput.Modifiers = VTModifierKeys.None;

        //    this.SendUserInput(selectedSession, this.userInput);

        //    e.Handled = true;
        //}

        ///// <summary>
        ///// 从键盘上按下按键的时候会触发
        ///// </summary>
        ///// <param name="e"></param>
        //protected override void OnPreviewKeyDown(KeyEventArgs e)
        //{
        //    base.OnPreviewKeyDown(e);

        //    InputSessionVM selectedSession = ListBoxOpenedSession.SelectedItem as InputSessionVM;
        //    if (selectedSession == null)
        //    {
        //        return;
        //    }

        //    if (e.Key == Key.ImeProcessed)
        //    {
        //        // 这些字符交给输入法处理了
        //    }
        //    else
        //    {
        //        switch (e.Key)
        //        {
        //            case Key.Tab:
        //            case Key.Up:
        //            case Key.Down:
        //            case Key.Left:
        //            case Key.Right:
        //            case Key.Space:
        //                {
        //                    // 防止焦点移动到其他控件上了
        //                    e.Handled = true;
        //                    break;
        //                }
        //        }

        //        if (e.Key != Key.ImeProcessed)
        //        {
        //            e.Handled = true;
        //        }

        //        VTKeys vtKey = TermUtils.ConvertToVTKey(e.Key);
        //        this.userInput.CapsLock = Console.CapsLock;
        //        this.userInput.Key = vtKey;
        //        this.userInput.Text = null;
        //        this.userInput.Modifiers = (VTModifierKeys)e.KeyboardDevice.Modifiers;
        //        this.SendUserInput(selectedSession, this.userInput);
        //    }

        //    e.Handled = true;
        //}

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 1)
            {
                base.DragMove();
            }
            else if (e.ClickCount == 2)
            {
                if (base.WindowState == WindowState.Normal)
                {
                    base.WindowState = WindowState.Maximized;
                }
                else
                {
                    base.WindowState = WindowState.Normal;
                }
            }
        }


        /// <summary>
        /// 点击顶部的工具栏菜单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListBoxToolbarMenus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //SMenuItem menuItem = ListBoxToolbarMenus.SelectedItem as SMenuItem;
            //if (menuItem == null) 
            //{
            //    return;
            //}

            //menuItem.ClickDelegate();

            //ListBoxToolbarMenus.SelectedItem = null; // 使下次可以继续点击
        }




        #endregion

        #region 命令响应

        private void SendCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            QuickCommandVM shellCommand = e.Parameter as QuickCommandVM;
            if (shellCommand == null)
            {
                return;
            }

            InputSessionVM inputSession = ListBoxOpenedSession.SelectedItem as InputSessionVM;
            if (inputSession == null)
            {
                return;
            }

            switch (shellCommand.Type)
            {
                case CommandTypeEnum.PureText:
                    {
                        string command = shellCommand.Command;
                        if (shellCommand.AutoCRLF)
                        {
                            command = string.Format("{0}\r\n", command);
                        }

                        inputSession.SendText(command);
                        break;
                    }

                case CommandTypeEnum.HexData:
                    {
                        byte[] bytes;
                        if (!MTermUtils.TryParseHexString(shellCommand.Command, out bytes))
                        {
                            MTMessageBox.Info("发送失败, 十六进制数据格式错误");
                            return;
                        }

                        inputSession.SendRawData(bytes);
                        break;
                    }

                default:
                    throw new NotImplementedException();
            }
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

    public class OpenedSessionItemContainerStyleSelector : StyleSelector
    {
        public Style StyleOpenedSession { get; set; }
        public Style StyleOpenSession { get; set; }

        public override Style SelectStyle(object item, DependencyObject container)
        {
            if (item is OpenSessionVM)
            {
                return this.StyleOpenSession;
            }
            else
            {
                return this.StyleOpenedSession;
            }
        }
    }
}
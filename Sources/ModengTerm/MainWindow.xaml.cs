using ModengTerm;
using ModengTerm.Base;
using ModengTerm.Base.DataModels;
using ModengTerm.Base.Enumerations;
using ModengTerm.Base.Enumerations.Terminal;
using ModengTerm.Base.ServiceAgents;
using ModengTerm.Controls;
using ModengTerm.Document;
using ModengTerm.Terminal;
using ModengTerm.Terminal.Enumerations;
using ModengTerm.Terminal.ViewModels;
using ModengTerm.ViewModels;
using ModengTerm.ViewModels.Terminals;
using ModengTerm.Windows;
using ModengTerm.Windows.SSH;
using ModengTerm.Windows.Terminals;
using System;
using System.Collections.Generic;
using System.IO;
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
using WPFToolkit.Utils;
using XTerminal.Base;
using XTerminal.Base.DataModels;
using XTerminal.Base.Enumerations;
using XTerminal.UserControls;
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
        private VTKeyInput userInput;
        private MainWindowVM mainWindowVM;
        private OpenedSessionsVM sessionListVM;
        private ServiceAgent serviceAgent;

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

            this.userInput = new VTKeyInput();
            this.templateSelector = new OpenedSessionDataTemplateSelector();
            this.templateSelector.DataTemplateOpenedSession = this.FindResource("DataTemplateOpenedSession") as DataTemplate;
            this.templateSelector.DataTemplateOpenSession = this.FindResource("DataTemplateOpenSession") as DataTemplate;
            ListBoxOpenedSession.ItemTemplateSelector = this.templateSelector;

            this.itemContainerStyleSelector = new OpenedSessionItemContainerStyleSelector();
            this.itemContainerStyleSelector.StyleOpenedSession = this.FindResource("StyleListBoxItemOpenedSession") as Style;
            this.itemContainerStyleSelector.StyleOpenSession = this.FindResource("StyleListBoxItemOpenSession") as Style;
            ListBoxOpenedSession.ItemContainerStyleSelector = this.itemContainerStyleSelector;

            this.sessionListVM = this.mainWindowVM.OpenedSessionsVM;
            ListBoxOpenedSession.DataContext = this.sessionListVM;
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
            ISessionContent content = this.sessionListVM.OpenSession(session);
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
            foreach (ShellSessionVM shellSession in this.sessionListVM.ShellSessions)
            {
                shellSession.SendText(text);
            }
        }

        #endregion

        #region 事件处理器

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //this.CreateSession();

            // 直接打开Windows命令行，可以更快速的进入工作状态
            // TODO：做成可选项，可以直接打开命令行，也能打开会话列表

            this.OpenDefaultSession();
        }

        private void ListBoxOpenedSession_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
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

            this.sessionListVM.CloseSession(openedSessionVM);

            this.sessionListVM.SelectedSession = this.sessionListVM.SessionList.OfType<OpenedSessionVM>().FirstOrDefault();

            if (this.sessionListVM.SelectedSession == null)
            {
                ContentControlSession.Content = null;
                ListBoxOpenedSession.SelectedItem = null;
            }

            if (this.sessionListVM.SessionList.Count == 1 &&
                this.sessionListVM.SessionList[0] is OpenSessionVM)
            {
                this.ShowSessionListWindow();
            }
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
            this.OpenSession(session, false);
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

        private void MenuItemOpenSession_Click(object sender, RoutedEventArgs e)
        {
            this.ShowSessionListWindow();
        }

        private void MenuItemFind_Click(object sender, RoutedEventArgs e)
        {
            ShellSessionVM shellSession = ListBoxOpenedSession.SelectedItem as ShellSessionVM;
            if (shellSession == null)
            {
                MTMessageBox.Info("请选中要搜索的终端");
                return;
            }

            FindWindowMgr.Show(shellSession);
        }

        private void MenuItemCopySelected_Click(object sender, RoutedEventArgs e)
        {
            ShellSessionVM shellSession = ListBoxOpenedSession.SelectedItem as ShellSessionVM;
            if (shellSession == null)
            {
                return;
            }

            shellSession.CopySelection();
        }

        private void MenuItemSaveSelection_Click(object sender, RoutedEventArgs e)
        {
            ShellSessionVM shellSession = ListBoxOpenedSession.SelectedItem as ShellSessionVM;
            if (shellSession == null)
            {
                return;
            }

            shellSession.SaveSelection();
        }

        private void MenuItemSaveViewport_Click(object sender, RoutedEventArgs e)
        {
            ShellSessionVM shellSession = ListBoxOpenedSession.SelectedItem as ShellSessionVM;
            if (shellSession == null)
            {
                return;
            }

            shellSession.SaveViewport();
        }

        private void MenuItemSaveAllDocument_Click(object sender, RoutedEventArgs e)
        {
            ShellSessionVM shellSession = ListBoxOpenedSession.SelectedItem as ShellSessionVM;
            if (shellSession == null)
            {
                return;
            }

            shellSession.SaveAllDocument();
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

        private void MenuItemPortForward_Click(object sender, RoutedEventArgs e)
        {
            ShellSessionVM shellSession = ListBoxOpenedSession.SelectedItem as ShellSessionVM;
            if (shellSession == null)
            {
                MTMessageBox.Info("请选择要查看的会话");
                return;
            }

            if (shellSession.Session.Type != (int)SessionTypeEnum.SSH)
            {
                MTMessageBox.Info("该会话没有转发信息");
                return;
            }

            PortForwardStatusWindow portForwardStatusWindow = new PortForwardStatusWindow(shellSession);
            portForwardStatusWindow.Owner = this;
            portForwardStatusWindow.Show();
        }

        private void MenuItemSendAll_Click(object sender, RoutedEventArgs e)
        {
            ShellSessionVM shellSession = ListBoxOpenedSession.SelectedItem as ShellSessionVM;
            if (shellSession == null)
            {
                MTMessageBox.Info("请选择要设置的会话");
                return;
            }

            shellSession.OpenSyncInputConfigurationWindow();
        }

        private void MenuItemShellCommand_Click(object sender, RoutedEventArgs e)
        {
            ShellSessionVM shellSession = ListBoxOpenedSession.SelectedItem as ShellSessionVM;
            if (shellSession == null)
            {
                MTMessageBox.Info("请选择要设置的会话");
                return;
            }

            shellSession.OpenCreateShellCommandWindow();
        }

        private void MenuItemStartLog_Click(object sender, RoutedEventArgs e)
        {
            ShellSessionVM shellSession = ListBoxOpenedSession.SelectedItem as ShellSessionVM;
            if (shellSession == null)
            {
                MTMessageBox.Info("请选择要启动的会话");
                return;
            }

            shellSession.StartLogger();
        }

        private void MenuItemStopLog_Click(object sender, RoutedEventArgs e)
        {
            ShellSessionVM shellSession = ListBoxOpenedSession.SelectedItem as ShellSessionVM;
            if (shellSession == null)
            {
                return;
            }

            shellSession.StopLogger();
        }

        private void MenuItemPauseLog_Click(object sender, RoutedEventArgs e)
        {
            ShellSessionVM shellSession = ListBoxOpenedSession.SelectedItem as ShellSessionVM;
            if (shellSession == null)
            {
                return;
            }

            shellSession.PauseLogger();
        }

        private void MenuItemResumeLog_Click(object sender, RoutedEventArgs e)
        {
            ShellSessionVM shellSession = ListBoxOpenedSession.SelectedItem as ShellSessionVM;
            if (shellSession == null)
            {
                return;
            }

            shellSession.ResumeLogger();
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
            ShellCommandVM shellCommand = e.Parameter as ShellCommandVM;
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
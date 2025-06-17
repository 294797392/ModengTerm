using DotNEToolkit;
using log4net.Core;
using log4net.Repository.Hierarchy;
using ModengTerm.Addon;
using ModengTerm.Addon.Interactive;
using ModengTerm.Addons;
using ModengTerm.Base;
using ModengTerm.Base.DataModels;
using ModengTerm.Base.Definitions;
using ModengTerm.Base.Enumerations;
using ModengTerm.Base.ServiceAgents;
using ModengTerm.Terminal;
using ModengTerm.Terminal.Enumerations;
using ModengTerm.Themes;
using ModengTerm.UserControls;
using ModengTerm.ViewModel;
using ModengTerm.ViewModel.Panel;
using ModengTerm.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;

namespace ModengTerm
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window, IHostWindow
    {
        #region 类变量

        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger("MainWindow");

        #endregion

        #region 实例变量

        private OpenedSessionDataTemplateSelector templateSelector;
        private OpenedSessionItemContainerStyleSelector itemContainerStyleSelector;
        private VTKeyboardInput userInput;
        private MainWindowVM mainWindowVM;
        private ServiceAgent serviceAgent;
        private DefaultMainUserControl defaultMainUserControl;
        private List<AddonModule> addons;

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
            this.serviceAgent = VTApp.Context.ServiceAgent;

            this.mainWindowVM = MainWindowVM.GetInstance();
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

            ListBoxOpenedSession.AddHandler(ListBox.MouseWheelEvent, new MouseWheelEventHandler(this.ListBoxOpenedSession_MouseWheel), true);

            this.InitializeAddon();
            this.RaiseAddonEvent(HostEvent.HOST_APP_INITIALIZED);
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

        private void OpenDefaultSession()
        {
            XTermSession defaultSession = VTApp.Context.Manifest.DefaultSession;
            if (defaultSession == null)
            {
                return;
            }

            string cmdPath = System.IO.Path.Combine(Environment.SystemDirectory, "cmd.exe");
            defaultSession.SetOption<string>(OptionKeyEnum.CMD_STARTUP_PATH, cmdPath);

            // 如果是Win10或者更高版本的操作系统，那么使用PseudoConsoleAPI
            if (VTBaseUtils.IsWin10())
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

        private void CloseSession(OpenedSessionVM session)
        {
            ISessionContent content = session.Content as ISessionContent;
            if (VTBaseUtils.IsTerminal((SessionTypeEnum)content.Session.Type))
            {
                UserControl userControl = content as UserControl;
            }
            content.Close();

            this.mainWindowVM.SessionList.Remove(session);
        }

        /// <summary>
        /// 初始化插件列表
        /// </summary>
        private void InitializeAddon()
        {
            List<AddonModule> addons = new List<AddonModule>();

            #region 创建插件实例

            foreach (AddonDefinition definition in VTApp.Context.Manifest.Addons)
            {
                AddonModule addon = null;

                try
                {
                    addon = ConfigFactory<AddonModule>.CreateInstance(definition.ClassEntry);

                    ActiveContext context = new ActiveContext()
                    {
                        Definition = definition,
                        StorageService = new SqliteStorageService(),
                        Factory = HostFactory.GetFactory(),
                        HostWindow = this
                    };

                    addon.Active(context);

                    addons.Add(addon);
                }
                catch (Exception ex)
                {
                    logger.Error("创建插件实例异常", ex);
                    continue;
                }
            }

            #endregion

            this.addons = addons;
        }

        private void RaiseAddonEvent(HostEvent evType, HostEventArgs evArgs = null)
        {
            foreach (AddonModule addon in this.addons)
            {
                AddonEventHandler handler;
                if (!addon.RegisteredEvent.TryGetValue(evType, out handler))
                {
                    continue;
                }

                handler.Invoke(evType, evArgs);
            }
        }

        private void RaiseAddonCommand(CommandArgs cmdArgs)
        {
            AddonModule addon = this.addons.FirstOrDefault(v => v.ID == cmdArgs.AddonId);
            if (addon == null)
            {
                logger.ErrorFormat("查找插件失败, {0}", cmdArgs.AddonId);
                return;
            }

            AddonCommandHandler handler;
            if (!addon.RegisteredCommand.TryGetValue(cmdArgs.Command, out handler))
            {
                logger.WarnFormat("插件未注册命令, {0}, {1}", addon.ID, cmdArgs.Command);
                return;
            }

            handler.Invoke(cmdArgs);
        }

        #endregion

        #region 事件处理器

        /// <summary>
        /// 会话触发的所有事件
        /// </summary>
        /// <param name="session"></param>
        /// <param name="code"></param>
        /// <param name="e"></param>
        private void OpenedSessionVM_Notify(OpenedSessionVM session, HostEvent evType, HostEventArgs evArgs)
        {
            foreach (AddonModule addon in this.addons)
            {
                AddonEventHandler handler;
                if (!addon.RegisteredEvent.TryGetValue(evType, out handler))
                {
                    continue;
                }

                handler(evType, evArgs);
            }
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            // 直接打开Windows命令行，可以更快速的进入工作状态
            // TODO：做成可选项，可以直接打开命令行，也能打开会话列表

            // 在Loaded事件里执行OpenDefaultSession会导致高度计算不正确的问题，ContentRendered事件在Loaded事件触发之后触发
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

            #region 触发OpenedSessionVM的OnLoaded或OnUnload事件

            OpenedSessionVM removedSession = null, addedSession = null;

            if (e.RemovedItems.Count > 0)
            {
                removedSession = e.RemovedItems[0] as OpenedSessionVM;
                if (removedSession != null)
                {
                    removedSession.OnUnload();
                }
            }

            if (e.AddedItems.Count > 0)
            {
                addedSession = e.AddedItems[0] as OpenedSessionVM;
                if (addedSession != null)
                {
                    addedSession.OnLoaded();
                }
            }

            #endregion

            // 触发Tab选中改变事件
            ActiveTabChangedEventArgs activeTabChanged = new ActiveTabChangedEventArgs()
            {
                AddedTab = addedSession,
                RemovedTab = removedSession
            };
            this.RaiseAddonEvent(HostEvent.HOST_ACTIVE_TAB_CHANGED, activeTabChanged);
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

            this.CloseSession(openedSessionVM);

            this.mainWindowVM.SelectedSession = this.mainWindowVM.SessionList.OfType<OpenedSessionVM>().FirstOrDefault();

            if (this.mainWindowVM.SelectedSession == null)
            {
                ContentControlSession.Content = null;
                ListBoxOpenedSession.SelectedItem = null;
            }

            if (this.mainWindowVM.SessionList.Count == 1 &&
                this.mainWindowVM.SessionList[0] is OpenSessionVM)
            {
                if (this.defaultMainUserControl == null)
                {
                    this.defaultMainUserControl = new DefaultMainUserControl();
                }
                ContentControlSession.Content = this.defaultMainUserControl;

                //this.ShowSessionListWindow();
            }
        }



        private void MenuItemOpenRecentSessions_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = e.OriginalSource as MenuItem;
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
            ContextMenuVM contextMenu = menuItem.DataContext as ContextMenuVM;
            CommandArgs.Instance.AddonId = contextMenu.AddonId;
            CommandArgs.Instance.Command = contextMenu.Command;
            CommandArgs.Instance.ActiveTab = ListBoxOpenedSession.SelectedItem as IHostTab;
            this.RaiseAddonCommand(CommandArgs.Instance);
        }


        private void ButtonSwitchTheme_Click(object sender, RoutedEventArgs e)
        {
            PopupThemes.IsOpen = true;
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

        private void ListBoxItemAppTheme_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            PopupThemes.IsOpen = false;
        }

        private void ListBoxThemes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AppThemeVM appTheme = ListBoxThemes.SelectedItem as AppThemeVM;
            if (appTheme == null)
            {
                return;
            }

            ThemeManager.ApplyTheme(appTheme.Uri);
        }



        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);

            // 如果此时是TextBox在输入，那么不做处理，事件继续交给TextBox
            if (e.OriginalSource is TextBox)
            {
                return;
            }

            // 让当前显示的终端获取焦点
            ISessionContent content = ContentControlSession.Content as ISessionContent;
            if (content == null)
            {
                return;
            }

            if (content.HasInputFocus())
            {
                return;
            }

            // 此时说明焦点没有在终端上
            if (!content.SetInputFocus())
            {
                logger.ErrorFormat("设置SessionContent焦点失败");
            }
        }

        private void MenuItemLog_CheckedChanged(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;

            bool isChecked = menuItem.IsChecked;
            string loggerName = menuItem.Tag.ToString();

            Hierarchy hierarchy = log4net.LogManager.GetRepository() as Hierarchy;
            Logger logger = hierarchy.Exists(loggerName) as Logger;

            if (isChecked)
            {
                logger.Level = Level.All;
            }
            else
            {
                logger.Level = Level.Off;
            }
        }

        #endregion

        #region IHostWindow

        /// <summary>
        /// 执行打开会话动作
        /// </summary>
        /// <param name="session">要打开的会话</param>
        /// <param name="addToRecent">是否加入到最新打开的会话列表里</param>
        public void OpenSession(XTermSession session, bool addToRecent = true)
        {
            ISessionContent content = SessionContentFactory.Create(session);
            ContentControlSession.Content = content;

            OpenedSessionVM openedSessionVM = OpenedSessionVMFactory.Create(session);
            openedSessionVM.ID = session.ID;
            openedSessionVM.Name = session.Name;
            openedSessionVM.Description = session.Description;
            openedSessionVM.Content = content as DependencyObject;
            openedSessionVM.ServiceAgent = VTApp.Context.ServiceAgent;
            openedSessionVM.Notify += this.OpenedSessionVM_Notify;
            openedSessionVM.Initialize();

            // 先加到打开列表里，这样在打开列表里就不会重复添加会话的上下文菜单
            int index = this.mainWindowVM.SessionList.IndexOf(MainWindowVM.OpenSessionVM);
            this.mainWindowVM.SessionList.Insert(index, openedSessionVM);
            this.mainWindowVM.SelectedSession = openedSessionVM;

            ScrollViewerOpenedSession.ScrollToRightEnd();

            int code = content.Open(openedSessionVM);
            if (code != ResponseCode.SUCCESS)
            {
                logger.ErrorFormat("打开会话失败, {0}", code);
            }

            // 增加到最近打开列表里
            if (addToRecent)
            {
                this.mainWindowVM.AddToRecentSession(session);
            }

            // 触发打开事件
            TabOpenedEventArgs tabOpened = new TabOpenedEventArgs()
            {
                Type = (SessionTypeEnum)session.Type,
                OpenedTab = openedSessionVM
            };
            this.RaiseAddonEvent(HostEvent.HOST_TAB_OPENED, tabOpened);
        }

        /// <summary>
        /// 显示或隐藏Panel
        /// </summary>
        /// <param name="panelId">要显示或隐藏的PanelId</param>
        public void VisiblePanel(string panelId)
        {
            //this.mainWindowVM.PanelContainer.VisiblePanel(panelId);
        }

        /// <summary>
        /// 获取当前激活的Shell
        /// </summary>
        /// <returns></returns>
        public T GetActiveTab<T>() where T : IHostTab
        {
            if (ListBoxOpenedSession.SelectedItem is T)
            {
                return (T)ListBoxOpenedSession.SelectedItem;
            }

            return default(T);
        }

        /// <summary>
        /// 获取指定类型的所有会话Shell对象
        /// </summary>
        /// <returns></returns>
        public List<IHostTab> GetAllTabs()
        {
            return this.mainWindowVM.SessionList.OfType<IHostTab>().ToList();
        }

        public List<T> GetAllTabs<T>() where T : IHostTab
        {
            return this.mainWindowVM.SessionList.OfType<T>().ToList();
        }

        public void AddSidePanel(ISidePanel panel)
        {
            PanelContainer container = this.mainWindowVM.PanelContainers[panel.Dock];
            container.Panels.Add(panel as SidePanel);
        }

        public void RemoveSidePanel(ISidePanel panel)
        {
            PanelContainer container = this.mainWindowVM.PanelContainers[panel.Dock];
            container.Panels.Remove(panel as SidePanel);
        }

        #endregion

        #region 命令响应

        private void SendCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            throw new RefactorImplementedException();

            //CommandVM shellCommand = e.Parameter as CommandVM;
            //if (shellCommand == null)
            //{
            //    return;
            //}

            //InputSessionVM inputSession = ListBoxOpenedSession.SelectedItem as InputSessionVM;
            //if (inputSession == null)
            //{
            //    return;
            //}

            //switch (shellCommand.Type)
            //{
            //    case CommandTypeEnum.PureText:
            //        {
            //            string command = shellCommand.Command;
            //            inputSession.SendText(command);
            //            break;
            //        }

            //    case CommandTypeEnum.HexData:
            //        {
            //            byte[] bytes;
            //            if (!VTBaseUtils.TryParseHexString(shellCommand.Command, out bytes))
            //            {
            //                MTMessageBox.Info("发送失败, 十六进制数据格式错误");
            //                return;
            //            }

            //            inputSession.SendRawData(bytes);
            //            break;
            //        }

            //    default:
            //        throw new NotImplementedException();
            //}
        }

        private void OpenSessionCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            XTermSession session = e.Parameter as XTermSession;
            if (session == null)
            {
                logger.ErrorFormat("OpenSessionCommand缺少参数");
                return;
            }

            this.OpenSession(session);
        }

        #endregion

        #region 解决最大化覆盖任务栏问题

        private void Window_SourceInitialized(object sender, EventArgs e)
        {
            System.IntPtr handle = (new WindowInteropHelper(this)).Handle;
            HwndSource.FromHwnd(handle).AddHook(new HwndSourceHook(WindowProc));
        }

        private static System.IntPtr WindowProc(System.IntPtr hwnd, int msg, System.IntPtr wParam, System.IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case 0x0024:
                    WmGetMinMaxInfo(hwnd, lParam);
                    handled = true;
                    break;
            }
            return (System.IntPtr)0;
        }

        /// <summary>
        /// 获得并设置窗体大小信息
        /// </summary>
        private static void WmGetMinMaxInfo(System.IntPtr hwnd, System.IntPtr lParam)
        {
            MINMAXINFO mmi = (MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));
            int MONITOR_DEFAULTTONEAREST = 0x00000002;
            System.IntPtr monitor = MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);
            if (monitor != System.IntPtr.Zero)
            {
                MONITORINFO monitorInfo = new MONITORINFO();
                GetMonitorInfo(monitor, monitorInfo);
                RECT rcWorkArea = monitorInfo.rcWork;
                RECT rcMonitorArea = monitorInfo.rcMonitor;
                mmi.ptMaxPosition.x = Math.Abs(rcWorkArea.left - rcMonitorArea.left);
                mmi.ptMaxPosition.y = Math.Abs(rcWorkArea.top - rcMonitorArea.top);
                mmi.ptMaxSize.y = Math.Abs(rcWorkArea.bottom - rcWorkArea.top);

                // 这行如果不注释掉在多显示器情况下显示会有问题！ 
                mmi.ptMaxSize.x = Math.Abs(rcWorkArea.right - rcWorkArea.left);

                // 可设置窗体的最小尺寸
                // mmi.ptMinTrackSize.x = 300;
                // mmi.ptMinTrackSize.y = 200;
            }
            Marshal.StructureToPtr(mmi, lParam, true);
        }

        /// <summary>
        /// POINT aka POINTAPI
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            /// <summary>
            /// x coordinate of point.
            /// </summary>
            public int x;
            /// <summary>
            /// y coordinate of point.
            /// </summary>
            public int y;

            /// <summary>
            /// Construct a point of coordinates (x,y).
            /// </summary>
            public POINT(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }

        /// <summary>
        /// 窗体大小信息
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct MINMAXINFO
        {
            public POINT ptReserved;
            public POINT ptMaxSize;
            public POINT ptMaxPosition;
            public POINT ptMinTrackSize;
            public POINT ptMaxTrackSize;
        };

        /// <summary> Win32 </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 0)]
        public struct RECT
        {
            /// <summary> Win32 </summary>
            public int left;
            /// <summary> Win32 </summary>
            public int top;
            /// <summary> Win32 </summary>
            public int right;
            /// <summary> Win32 </summary>
            public int bottom;

            /// <summary> Win32 </summary>
            public static readonly RECT Empty = new RECT();

            /// <summary> Win32 </summary>
            public int Width
            {
                get { return Math.Abs(right - left); }  // Abs needed for BIDI OS
            }
            /// <summary> Win32 </summary>
            public int Height
            {
                get { return bottom - top; }
            }

            /// <summary> Win32 </summary>
            public RECT(int left, int top, int right, int bottom)
            {
                this.left = left;
                this.top = top;
                this.right = right;
                this.bottom = bottom;
            }


            /// <summary> Win32 </summary>
            public RECT(RECT rcSrc)
            {
                this.left = rcSrc.left;
                this.top = rcSrc.top;
                this.right = rcSrc.right;
                this.bottom = rcSrc.bottom;
            }

            /// <summary> Win32 </summary>
            public bool IsEmpty
            {
                get
                {
                    // BUGBUG : On Bidi OS (hebrew arabic) left > right
                    return left >= right || top >= bottom;
                }
            }
            /// <summary> Return a user friendly representation of this struct </summary>
            public override string ToString()
            {
                if (this == RECT.Empty) { return "RECT {Empty}"; }
                return "RECT { left : " + left + " / top : " + top + " / right : " + right + " / bottom : " + bottom + " }";
            }

            /// <summary> Determine if 2 RECT are equal (deep compare) </summary>
            public override bool Equals(object obj)
            {
                if (!(obj is Rect)) { return false; }
                return (this == (RECT)obj);
            }

            /// <summary>Return the HashCode for this struct (not garanteed to be unique)</summary>
            public override int GetHashCode()
            {
                return left.GetHashCode() + top.GetHashCode() + right.GetHashCode() + bottom.GetHashCode();
            }


            /// <summary> Determine if 2 RECT are equal (deep compare)</summary>
            public static bool operator ==(RECT rect1, RECT rect2)
            {
                return (rect1.left == rect2.left && rect1.top == rect2.top && rect1.right == rect2.right && rect1.bottom == rect2.bottom);
            }

            /// <summary> Determine if 2 RECT are different(deep compare)</summary>
            public static bool operator !=(RECT rect1, RECT rect2)
            {
                return !(rect1 == rect2);
            }


        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class MONITORINFO
        {
            /// <summary>
            /// </summary>            
            public int cbSize = Marshal.SizeOf(typeof(MONITORINFO));

            /// <summary>
            /// </summary>            
            public RECT rcMonitor = new RECT();

            /// <summary>
            /// </summary>            
            public RECT rcWork = new RECT();

            /// <summary>
            /// </summary>            
            public int dwFlags = 0;
        }

        [DllImport("user32")]
        internal static extern bool GetMonitorInfo(IntPtr hMonitor, MONITORINFO lpmi);

        [DllImport("User32")]
        internal static extern IntPtr MonitorFromWindow(IntPtr handle, int flags);

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
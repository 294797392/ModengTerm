using ModengTerm.Addon;
using ModengTerm.Addon.Interactive;
using ModengTerm.Addon.Service;
using ModengTerm.Base;
using ModengTerm.Base.DataModels;
using ModengTerm.Base.Enumerations;
using ModengTerm.Document;
using ModengTerm.Document.Utility;
using ModengTerm.Terminal;
using ModengTerm.ViewModel;
using ModengTerm.ViewModel.Panels;
using ModengTerm.ViewModel.Terminal;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace ModengTerm.UserControls.TerminalUserControls
{
    /// <summary>
    /// TerminalContentUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class ShellSessionUserControl : UserControl, ISessionContent
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("TerminalUserControl");

        #endregion

        #region 实例变量

        private ShellSessionVM shellSession;
        private VideoTerminal videoTerminal;
        private VTKeyboardInput userInput;
        private IClientEventRegistry eventRegistry;

        /// <summary>
        /// 鼠标滚轮滚动一次，滚动几行
        /// </summary>
        private int scrollDelta;
        private bool clickToCursor;
        private RightClickActions rightClickActions;

        #endregion

        #region 属性

        public XTermSession Session { get; set; }

        #endregion

        #region 构造方法

        public ShellSessionUserControl()
        {
            InitializeComponent();

            this.InitializeUserControl();
        }

        #endregion

        #region 实例方法

        private void InitializeUserControl()
        {
            // https://learn.microsoft.com/zh-cn/dotnet/desktop/wpf/advanced/focus-overview?view=netframeworkdesktop-4.8
            // 必须设置Focusable=true，调用Focus才生效
            // 为使元素获得键盘焦点，必须将基元素的 Focusable 和 IsVisible 属性设置为 true。 某些类（例如 Panel 基类）默认将 Focusable 设置为 false；因此，如果要此类元素能够获得键盘焦点，必须将 Focusable 设置为 true。
            GridTerminal.Focusable = true;

            this.Background = Brushes.Transparent;
            this.userInput = new VTKeyboardInput();

            ClientFactory factory = ClientFactory.GetFactory();
            this.eventRegistry = factory.GetEventRegistry();
        }

        private Point GetPositionRelativeToTerminal()
        {
            VTDocument document = this.videoTerminal.ActiveDocument;
            TerminalControl terminalControl = document.GFactory as TerminalControl;
            return Mouse.GetPosition(terminalControl.DrawArea);
        }

        private void MouseWheelScroll(MouseWheelEventArgs e)
        {
            VTDocument document = this.videoTerminal.ActiveDocument;

            int oldScroll = document.Scrollbar.Value;
            int scrollMax = document.Scrollbar.Maximum;
            int newScroll = 0; // 最终要滚动到的值

            if (e.Delta > 0)
            {
                // 向上滚动

                // 先判断是不是已经滚动到顶了
                if (document.ScrollAtTop)
                {
                    // 滚动到顶直接返回
                    return;
                }

                if (oldScroll < scrollDelta)
                {
                    // 一次可以全部滚完并且还有剩余
                    newScroll = document.Scrollbar.Minimum;
                }
                else
                {
                    newScroll = oldScroll - scrollDelta;
                }
            }
            else
            {
                // 向下滚动

                if (document.ScrollAtBottom)
                {
                    // 滚动到底直接返回
                    return;
                }

                // 剩余可以往下滚动的行数
                int remainScroll = scrollMax - oldScroll;

                if (remainScroll >= scrollDelta)
                {
                    newScroll = oldScroll + scrollDelta;
                }
                else
                {
                    // 直接滚动到底
                    newScroll = scrollMax;
                }
            }

            VTScrollData scrollData = document.ScrollTo(newScroll);
            if (scrollData == null)
            {
                return;
            }

            // 重新渲染
            document.RequestInvalidate();
        }

        /// <summary>
        /// 让scrollbar的值变成整数
        /// </summary>
        /// <param name="scrollbar"></param>
        /// <returns></returns>
        private int GetScrollValue(ScrollBar scrollbar)
        {
            var newvalue = (int)Math.Round(scrollbar.Value, 0);
            if (newvalue > scrollbar.Maximum)
            {
                newvalue = (int)Math.Round(scrollbar.Maximum, 0);
            }
            return newvalue;
        }

        private void ProcessVT200MouseMode(MouseButtonEventArgs e)
        {
            int buttonType = 0;
            switch (e.ChangedButton)
            {
                case MouseButton.Left: buttonType = 0; break;
                case MouseButton.Right: buttonType = 1; break;
                case MouseButton.Middle: buttonType = 2; break;
                default:
                    return;
            }

            bool released = e.ButtonState == MouseButtonState.Released;
            Point point = this.GetPositionRelativeToTerminal();
            this.videoTerminal.HandleVT200MouseMode(point.X, point.Y, buttonType, released);
        }

        #endregion

        #region 事件处理器

        private void DrawArea_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.videoTerminal == null)
            {
                return;
            }

            this.videoTerminal.Resize(e.NewSize.ToVTSize());

            // 确保DrawingArea1的大小和位置跟DocumentControl里的DrawingArea一致
            // 插件可以通过DrawingArea1来绘图
            DrawingArea drawingArea = sender as DrawingArea;
            DrawingArea1.Width = e.NewSize.Width;
            DrawingArea1.Height = e.NewSize.Height;
            DrawingArea1.Margin = drawingArea.Margin;
        }

        /// <summary>
        /// 重写了这个事件后，就会触发鼠标相关的事件
        /// </summary>
        /// 参考AvalonEdit
        /// <param name="hitTestParameters"></param>
        /// <returns></returns>
        protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
        {
            return new PointHitTestResult(this, hitTestParameters.HitPoint);
        }

        private void ContextMenu_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = e.OriginalSource as MenuItem;
            ContextMenuVM contextMenu = menuItem.DataContext as ContextMenuVM;
            CommandArgs.Instance.AddonId = contextMenu.AddonId;
            CommandArgs.Instance.Command = contextMenu.Command;
            CommandArgs.Instance.ActiveTab = this.shellSession;
            MCommands.ExecuteAddonCommand.Execute(CommandArgs.Instance, Application.Current.MainWindow);
        }

        private void GridDocument_KeyDown(object sender, KeyEventArgs e)
        {
            // 1. 不继续传播到TextInput事件
            // 2. 如果按了Tab或者方向键，焦点不会移动到其他控件上
            e.Handled = true;

            if (e.Key == Key.ImeProcessed)
            {
                return;
            }

            VTKeys vtKey = DrawingUtils.ConvertToVTKey(e.Key);

            this.userInput.CapsLock = Console.CapsLock;
            this.userInput.Key = vtKey;
            this.userInput.Modifiers = DrawingUtils.ConvertToVTModifierKeys(e.KeyboardDevice.Modifiers);
            this.userInput.FromIMEInput = false;
            this.shellSession.SendInput(this.userInput);
        }

        private void GridDocument_TextInput(object sender, TextCompositionEventArgs e)
        {
            this.userInput.Text = e.Text;
            this.userInput.FromIMEInput = true;
            this.shellSession.SendInput(this.userInput);
        }

        private void GridTerminal_Loaded(object sender, RoutedEventArgs e)
        {
            // Loaded事件里让控件获取焦点，这样才能触发OnKeyDown和OnTextInput事件
            Keyboard.Focus(GridTerminal);
        }

        private void GridDocument_LostFocus(object sender, RoutedEventArgs e)
        {
            if (this.videoTerminal != null)
            {
                this.videoTerminal.FocusChanged(false);
            }
        }

        private void GridDocument_GotFocus(object sender, RoutedEventArgs e)
        {
            if (this.videoTerminal != null)
            {
                this.videoTerminal.FocusChanged(true);
            }
        }

        #region Terminal事件

        private void GridTerminal_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (GridTerminal.IsMouseCaptured)
                {
                    GridTerminal.CaptureMouse();
                }

                Point position = this.GetPositionRelativeToTerminal();
                VTDocument document = this.videoTerminal.ActiveDocument;
                document.PerformSelection(position.X, position.Y);
            }
        }

        private void GridTerminal_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            this.MouseWheelScroll(e);
        }

        private void GridTerminal_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // 获取焦点，才能收到OnKeyDown和OnTextInput回调
            if (!GridTerminal.IsKeyboardFocused)
            {
                Keyboard.Focus(GridTerminal);
            }

            if (this.videoTerminal.IsVT200MouseMode)
            {
                this.ProcessVT200MouseMode(e);
            }

            if (e.ChangedButton == MouseButton.Left)
            {
                VTDocument document = this.videoTerminal.ActiveDocument;
                if (!document.Selection.IsEmpty)
                {
                    document.Selection.Clear();
                    document.Selection.RequestInvalidate();
                }

                if (e.ClickCount == 1)
                {
                    if (this.clickToCursor)
                    {
                        Point pos = this.GetPositionRelativeToTerminal();
                        this.videoTerminal.HandleClickToCursor(pos.X, pos.Y);
                    }
                }
                else
                {
                    // 双击就是选中单词
                    // 三击就是选中整行内容

                    Point position = this.GetPositionRelativeToTerminal();
                    double x = position.X;
                    double y = position.Y;
                    int startIndex = 0, count = 0, logicalRow = 0;

                    VTextLine textLine = HitTestHelper.HitTestVTextLine(document, y, out logicalRow);
                    if (textLine == null)
                    {
                        return;
                    }

                    switch (e.ClickCount)
                    {
                        case 2:
                            {
                                // 选中单词
                                int characterIndex;
                                int columnIndex;
                                VTextRange characterRange;
                                HitTestHelper.HitTestVTCharacter(textLine, x, out characterIndex, out characterRange, out columnIndex);
                                if (characterIndex == -1)
                                {
                                    return;
                                }
                                VTDocUtils.GetSegement(textLine.Characters, characterIndex, out startIndex, out count);
                                document.Selection.SelectRange(textLine, logicalRow, startIndex, count);
                                break;
                            }

                        case 3:
                            {
                                // 选中一整行
                                document.Selection.SelectRow(textLine, logicalRow);
                                break;
                            }

                        default:
                            break;
                    }
                }
            }
            else if (e.ChangedButton == MouseButton.Right)
            {
                if (this.rightClickActions == RightClickActions.FastCopyPaste)
                {
                    if (!this.videoTerminal.HasSelection)
                    {
                        // 粘贴剪贴板里的内容
                        string text = Clipboard.GetText();
                        if (string.IsNullOrEmpty(text))
                        {
                            return;
                        }

                        this.shellSession.Send(text);
                    }
                    else
                    {
                        this.shellSession.CopySelection();
                        this.videoTerminal.UnSelectAll();
                    }
                }
            }
        }

        private void GridTerminal_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (this.videoTerminal.IsVT200MouseMode)
            {
                this.ProcessVT200MouseMode(e);
            }

            if (e.ChangedButton == MouseButton.Left)
            {
                if (GridTerminal.IsMouseCaptured)
                {
                    GridTerminal.ReleaseMouseCapture();
                }
            }
        }

        private void Scrollbar_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                ScrollBar scrollbar = sender as ScrollBar;

                int newscroll = this.GetScrollValue(scrollbar);
                VTDocument document = this.videoTerminal.ActiveDocument;
                document.ScrollTo(newscroll);
                document.RequestInvalidate();

                e.Handled = true;
            }
        }

        #endregion



        private void ButtonOptions_Checked(object sender, RoutedEventArgs e)
        {
            ButtonOptions.ContextMenu.IsOpen = true;
        }

        private void ButtonSend_Click(object sender, RoutedEventArgs e)
        {
            ShellSessionVM shellSession = this.shellSession;
            if (shellSession == null)
            {
                return;
            }

            string text = ComboBoxHistoryCommands.Text;
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            if (MenuItemHexInput.IsChecked)
            {
                byte[] bytes;
                if (!VTBaseUtils.TryParseHexString(text, out bytes))
                {
                    MTMessageBox.Info("请输入正确的十六进制数据");
                    return;
                }

                throw new RefactorImplementedException();
                //shellSession.SendRawData(bytes);
            }
            else
            {
                if (MenuItemSendCRLF.IsChecked)
                {
                    text = string.Format("{0}\r\n", text);
                }

                throw new RefactorImplementedException();
                //shellSession.SendText(text);
            }

            shellSession.HistoryCommands.Add(text);
        }

        private void ButtonClear_Click(object sender, RoutedEventArgs e)
        {
            ComboBoxHistoryCommands.Text = string.Empty;
        }



        private void VideoTerminal_RequestChangeWindowSize(IVideoTerminal arg1, double deltaX, double deltaY)
        {
            Window window = Window.GetWindow(this);

            window.Width += deltaX;
            window.Height += deltaY;

            if (window.WindowState != WindowState.Normal)
            {
                window.WindowState = WindowState.Normal;
            }

            logger.InfoFormat("RequestChangeWindowSize, deltaX = {0}, deltaY = {1}, width = {2}, height = {3}", deltaX, deltaY, window.Width, window.Height);
        }

        private void VideoTerminal_RequestChangeVisible(IVideoTerminal arg1, VTDocumentTypes type, bool visible)
        {
            VTDocument document = type == VTDocumentTypes.AlternateDocument ? this.videoTerminal.AlternateDocument : this.videoTerminal.MainDocument;
            TerminalControl terminalControl = type == VTDocumentTypes.AlternateDocument ? DocumentAlternate : DocumentMain;

            terminalControl.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
            if (visible)
            {
                VTCursorTimer.Context.SetCursor(document.Cursor);
            }
        }


        private void ButtonCloseOverlayPanel_Click(object sender, RoutedEventArgs e)
        {
            FrameworkElement frameworkElement = sender as FrameworkElement;
            OverlayPanelVM overlayPanel = frameworkElement.DataContext as OverlayPanelVM;
            overlayPanel.Close();
        }

        #endregion

        #region ISessionContent

        public int Open(OpenedSessionVM sessionVM)
        {
            this.scrollDelta = this.Session.GetOption<int>(PredefinedOptions.CURSOR_SCROLL_DELTA);
            this.clickToCursor = this.Session.GetOption<bool>(PredefinedOptions.TERM_ADV_CLICK_TO_CURSOR);
            this.rightClickActions = this.Session.GetOption<RightClickActions>(PredefinedOptions.TERM_RIGHT_CLICK_ACTION);

            // 背景不放在Dispatcher里渲染，不然会出现背景闪烁一下的现象
            string background = this.Session.GetOption<string>(PredefinedOptions.THEME_BACK_COLOR);
            BorderBackground.Background = DrawingUtils.GetBrush(background);
            string base64Image = this.Session.GetOption<string>(PredefinedOptions.THEME_BACK_IMAGE_DATA);
            if (!string.IsNullOrEmpty(base64Image))
            {
                try
                {
                    byte[] imageBytes = Convert.FromBase64String(base64Image);
                    ImageSource imageSource = DrawingUtils.ImageBytes2ImageSource(imageBytes);
                    ImageBackground.Source = imageSource;
                }
                catch (Exception ex)
                {
                    logger.Error("加载背景图片异常", ex);
                }

                ImageBackground.Opacity = this.Session.GetOption<double>(PredefinedOptions.THEME_BACK_IMAGE_OPACITY);
            }

            this.shellSession = sessionVM as ShellSessionVM;
            this.shellSession.DrawingContext = DrawingArea1;

            // DispatcherPriority.Loaded保证DrawArea不为空
            base.Dispatcher.BeginInvoke(new Action(() =>
            {
                double padding = this.Session.GetOption<double>(PredefinedOptions.THEME_PADDING);
                DocumentAlternate.Visibility = Visibility.Collapsed;
                DocumentAlternate.Padding = new Thickness(padding);
                DocumentAlternate.DrawArea.SizeChanged += DrawArea_SizeChanged;
                DocumentAlternate.Scrollbar.MouseMove += this.Scrollbar_MouseMove;
                DocumentMain.Padding = new Thickness(padding);
                DocumentMain.DrawArea.SizeChanged += DrawArea_SizeChanged;
                DocumentMain.Scrollbar.MouseMove += this.Scrollbar_MouseMove;

                // https://gitee.com/zyfalreadyexsit/terminal/issues/ICG9KR
                // 不要直接使用Document的DrawAreaSize属性，DrawAreaSize可能不准确！
                // 手动计算终端宽度和高度，这个高度和宽度可能也不准确。
                // 解决方法是在每次渲染服务端数据之后，重新设置一下终端高度，确保终端高度正确
                double width = GridTerminal.ActualWidth;
                double height = GridTerminal.ActualHeight;

                this.shellSession.MainDocument = DocumentMain;
                this.shellSession.AlternateDocument = DocumentAlternate;
                this.shellSession.Width = width;
                this.shellSession.Height = height;
                this.shellSession.Open();

                this.videoTerminal = this.shellSession.VideoTerminal as VideoTerminal;
                this.videoTerminal.RequestChangeWindowSize += VideoTerminal_RequestChangeWindowSize;
                this.videoTerminal.RequestChangeVisible += this.VideoTerminal_RequestChangeVisible;

                base.DataContext = this.shellSession;

            }), System.Windows.Threading.DispatcherPriority.Loaded);

            return ResponseCode.SUCCESS;
        }

        public void Close()
        {
            DocumentAlternate.DrawArea.SizeChanged -= DrawArea_SizeChanged;
            DocumentAlternate.Scrollbar.MouseMove -= this.Scrollbar_MouseMove;
            DocumentMain.DrawArea.SizeChanged -= DrawArea_SizeChanged;
            DocumentMain.Scrollbar.MouseMove -= this.Scrollbar_MouseMove;

            this.videoTerminal.RequestChangeWindowSize -= VideoTerminal_RequestChangeWindowSize;
            this.videoTerminal.RequestChangeVisible -= this.VideoTerminal_RequestChangeVisible;
            this.videoTerminal = null;

            this.shellSession.Close();
            this.shellSession.Release();
        }

        public bool SetInputFocus()
        {
            return GridTerminal.Focus();
        }

        public bool HasInputFocus()
        {
            return GridTerminal.IsFocused;
        }

        #endregion
    }
}

using ModengTerm.Addon.Interactive;
using ModengTerm.Base;
using ModengTerm.Base.DataModels;
using ModengTerm.Base.Enumerations;
using ModengTerm.Document;
using ModengTerm.Terminal;
using ModengTerm.ViewModel;
using ModengTerm.ViewModel.Panel;
using ModengTerm.ViewModel.Terminal;
using System;
using System.Windows;
using System.Windows.Controls;
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

        #region 公开事件

        public event Action<ISessionContent> Ready;

        #endregion

        #region 实例变量

        private ShellSessionVM shellSession;
        private AutoCompletionVM autoCompleteVM;
        private VideoTerminal videoTerminal;
        private VTKeyboardInput userInput;

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
            GridDocument.Focusable = true;

            this.Background = Brushes.Transparent;
            this.userInput = new VTKeyboardInput();
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
            throw new RefactorImplementedException();

            //MenuItem menuItem = e.OriginalSource as MenuItem;
            //ContextMenuVM contextMenu = menuItem.DataContext as ContextMenuVM;
            //CommandArgs.Instance.AddonId = contextMenu.AddonId;
            //CommandArgs.Instance.Command = contextMenu.Command;
            //VTApp.Context.RaiseAddonCommand(CommandArgs.Instance);
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

            // 如果启用了自动完成功能，那么先把按键事件传递给自动完成功能
            if (this.autoCompleteVM.Enabled)
            {
                if (!this.autoCompleteVM.OnKeyDown(vtKey))
                {
                    return;
                }
            }

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

        private void GridDocument_Loaded(object sender, RoutedEventArgs e)
        {
            // Loaded事件里让控件获取焦点，这样才能触发OnKeyDown和OnTextInput事件
            Keyboard.Focus(GridDocument);
        }

        private void GridDocument_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // 获取焦点，才能收到OnKeyDown和OnTextInput回调
            Keyboard.Focus(GridDocument);

            if (e.ChangedButton == MouseButton.Right)             
            {
                BehaviorRightClicks brc = this.Session.GetOption<BehaviorRightClicks>(OptionKeyEnum.BEHAVIOR_RIGHT_CLICK);
                if (brc == BehaviorRightClicks.FastCopyPaste)
                {
                    if (!this.videoTerminal.HasSelection)
                    {
                        // 粘贴剪贴板里的内容
                        string text = Clipboard.GetText();
                        if (string.IsNullOrEmpty(text))
                        {
                            return;
                        }

                        this.shellSession.SendText(text);
                    }
                    else
                    {
                        this.shellSession.CopySelection();
                        this.videoTerminal.UnSelectAll();
                    }
                }
            }
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



        private void VideoTerminal_DocumentChanged(IVideoTerminal arg1, VTDocument oldDocument, VTDocument newDocument)
        {
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



        private void AutoCompletionUserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            UIElement element = sender as UIElement;
            if (element.Visibility == Visibility.Collapsed)
            {
                // 重新获取焦点，以便于可以接收键盘输入
                Keyboard.Focus(GridDocument);
            }
        }

        private void AutoCompletionUserControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // 模拟输入Enter
            this.autoCompleteVM.OnKeyDown(VTKeys.Enter);
        }

        private void ButtonCloseOverlayPanel_Click(object sender, RoutedEventArgs e)
        {
            FrameworkElement frameworkElement = sender as FrameworkElement;
            OverlayPanel overlayPanel = frameworkElement.DataContext as OverlayPanel;
            overlayPanel.Close();
        }

        #endregion

        #region ISessionContent

        public int Open(OpenedSessionVM sessionVM)
        {
            // 背景不放在Dispatcher里渲染，不然会出现背景闪烁一下的现象
            string background = this.Session.GetOption<string>(OptionKeyEnum.THEME_BACKGROUND_COLOR);
            BorderBackground.Background = DrawingUtils.GetBrush(background);
            string base64Image = this.Session.GetOption<string>(OptionKeyEnum.THEME_BACKGROUND_IMAGE_DATA, OptionDefaultValues.THEME_BACKGROUND_IMAGE_DATA);
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

                ImageBackground.Opacity = this.Session.GetOption<double>(OptionKeyEnum.THEME_BACKGROUND_IMAGE_OPACITY, OptionDefaultValues.THEME_BACKGROUND_IMAGE_OPACITY);
            }

            this.shellSession = sessionVM as ShellSessionVM;
            this.shellSession.DrawingContext = DrawingArea1;

            // DispatcherPriority.Loaded保证DrawArea不为空
            base.Dispatcher.BeginInvoke(new Action(() =>
            {
                double padding = this.Session.GetOption<double>(OptionKeyEnum.SSH_THEME_DOCUMENT_PADDING);
                DocumentAlternate.SetPadding(padding);
                DocumentAlternate.DrawArea.SizeChanged += DrawArea_SizeChanged;
                DocumentMain.SetPadding(padding);
                DocumentMain.DrawArea.SizeChanged += DrawArea_SizeChanged;

                // 不要直接使用Document的DrawAreaSize属性，DrawAreaSize可能不准确！
                // 手动计算终端宽度和高度，这个高度和宽度可能也不准确。
                // 解决方法是在每次收到服务端数据之后，重新设置一下终端高度，确保终端高度正确
                double width = GridDocument.ActualWidth - padding * 2 - 11 - 30;  // 11是滚动条的宽度，30是右边栏的宽度
                double height = GridDocument.ActualHeight - padding * 2;
                //logger.InfoFormat("width = {0}, height = {1}", width, height);

                this.shellSession.MainDocument = DocumentMain;
                this.shellSession.AlternateDocument = DocumentAlternate;
                this.shellSession.Width = width;
                this.shellSession.Height = height;
                this.shellSession.Open();

                this.autoCompleteVM = this.shellSession.AutoCompletionVM;

                this.videoTerminal = this.shellSession.VideoTerminal as VideoTerminal;
                this.videoTerminal.OnDocumentChanged += VideoTerminal_DocumentChanged;
                this.videoTerminal.RequestChangeWindowSize += VideoTerminal_RequestChangeWindowSize;

                // 自动完成列表和文本行对齐
                AutoCompletionUserControl.Margin = new Thickness(padding);
                AutoCompletionUserControl.DataContext = this.shellSession.AutoCompletionVM;

                base.DataContext = this.shellSession;

            }), System.Windows.Threading.DispatcherPriority.Loaded);

            return ResponseCode.SUCCESS;
        }

        public void Close()
        {
            DocumentAlternate.DrawArea.SizeChanged -= DrawArea_SizeChanged;
            DocumentMain.DrawArea.SizeChanged -= DrawArea_SizeChanged;

            this.videoTerminal.OnDocumentChanged -= VideoTerminal_DocumentChanged;
            this.videoTerminal.RequestChangeWindowSize -= VideoTerminal_RequestChangeWindowSize;
            this.videoTerminal = null;

            this.shellSession.Close();
            this.shellSession.Release();
        }

        public bool SetInputFocus()
        {
            return GridDocument.Focus();
        }

        public bool HasInputFocus()
        {
            return GridDocument.IsFocused;
        }

        #endregion
    }
}

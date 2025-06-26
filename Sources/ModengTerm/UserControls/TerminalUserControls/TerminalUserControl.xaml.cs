using ModengTerm.Addon.Interactive;
using ModengTerm.Base;
using ModengTerm.Base.DataModels;
using ModengTerm.Base.Enumerations;
using ModengTerm.Document;
using ModengTerm.Terminal;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace ModengTerm.UserControls.TerminalUserControls
{
    /// <summary>
    /// TerminalUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class TerminalUserControl : UserControl
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("TerminalUserControl");

        #region 实例变量

        private int scrollDelta;

        private VideoTerminal videoTerminal;

        #endregion

        #region 公开属性

        public TerminalControl MainDocument { get { return DocumentMain; } }

        public TerminalControl AlternateDocument { get { return DocumentAlternate; } }

        #endregion

        #region 构造方法

        public TerminalUserControl()
        {
            InitializeComponent();
        }

        #endregion

        #region 实例方法

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

        private Point GetPositionRelativeToTerminal()
        {
            VTDocument document = this.videoTerminal.ActiveDocument;
            TerminalControl terminalControl = document.GFactory as TerminalControl;
            return Mouse.GetPosition(terminalControl.DrawArea);
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

        private void GridTerminal_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            this.MouseWheelScroll(e);
        }

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

        #endregion

        #region 公开接口

        public void Initialize(XTermSession session, VideoTerminal videoTerminal)
        {
            this.videoTerminal = videoTerminal;

            this.scrollDelta = session.GetOption<int>(OptionKeyEnum.MOUSE_SCROLL_DELTA, OptionDefaultValues.MOUSE_SCROLL_DELTA);

            // 背景不放在Dispatcher里渲染，不然会出现背景闪烁一下的现象
            string background = session.GetOption<string>(OptionKeyEnum.THEME_BACKGROUND_COLOR);
            BorderBackground.Background = DrawingUtils.GetBrush(background);
            string base64Image = session.GetOption<string>(OptionKeyEnum.THEME_BACKGROUND_IMAGE_DATA, OptionDefaultValues.THEME_BACKGROUND_IMAGE_DATA);
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

                ImageBackground.Opacity = session.GetOption<double>(OptionKeyEnum.THEME_BACKGROUND_IMAGE_OPACITY, OptionDefaultValues.THEME_BACKGROUND_IMAGE_OPACITY);
            }

            // DispatcherPriority.Loaded保证DrawArea不为空
            base.Dispatcher.BeginInvoke(new Action(() =>
            {
                double padding = session.GetOption<double>(OptionKeyEnum.SSH_THEME_DOCUMENT_PADDING);
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
                // 解决方法是在每次收到服务端数据之后，重新设置一下终端高度，确保终端高度正确
                double width = GridTerminal.ActualWidth;
                double height = GridTerminal.ActualHeight;

                videoTerminal.RequestChangeWindowSize += this.VideoTerminal_RequestChangeWindowSize;
                videoTerminal.RequestChangeVisible += this.VideoTerminal_RequestChangeVisible;

            }), System.Windows.Threading.DispatcherPriority.Loaded);
        }

        public void Release() 
        {
            videoTerminal.RequestChangeWindowSize -= this.VideoTerminal_RequestChangeWindowSize;
            videoTerminal.RequestChangeVisible -= this.VideoTerminal_RequestChangeVisible;

            DocumentAlternate.DrawArea.SizeChanged -= DrawArea_SizeChanged;
            DocumentAlternate.Scrollbar.MouseMove -= this.Scrollbar_MouseMove;
            DocumentMain.DrawArea.SizeChanged -= DrawArea_SizeChanged;
            DocumentMain.Scrollbar.MouseMove -= this.Scrollbar_MouseMove;
        }

        public void FeedData(byte[] buffer, int size) 
        {
            this.videoTerminal.ProcessRead(buffer, size);
        }

        #endregion
    }
}

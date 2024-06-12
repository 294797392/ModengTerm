using DotNEToolkit;
using Microsoft.Win32;
using ModengTerm;
using ModengTerm.Base;
using ModengTerm.Base.DataModels;
using ModengTerm.Base.Enumerations;
using ModengTerm.Controls;
using ModengTerm.Document;
using ModengTerm.Document.Drawing;
using ModengTerm.Document.Rendering;
using ModengTerm.Terminal;
using ModengTerm.Terminal.DataModels;
using ModengTerm.Terminal.Enumerations;
using ModengTerm.Terminal.Loggering;
using ModengTerm.Terminal.ViewModels;
using ModengTerm.ViewModels;
using ModengTerm.Windows;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPFToolkit.Utility;

namespace ModengTerm.Terminal.UserControls
{
    /// <summary>
    /// TerminalContentUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class TerminalContentUserControl : UserControl, ISessionContent
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("TerminalUserControl");

        #endregion

        #region 实例变量

        private ShellSessionVM shellSession;
        private IVideoTerminal videoTerminal;

        #endregion

        #region 属性

        public XTermSession Session { get; set; }

        public IVideoTerminal VideoTerminal { get; set; }

        #endregion

        #region 构造方法

        public TerminalContentUserControl()
        {
            InitializeComponent();

            this.InitializeUserControl();
        }

        #endregion

        #region 实例方法

        private void InitializeUserControl()
        {
#if !DEBUG
            ToggleButtonLoggerSetting.Visibility = Visibility.Collapsed;
#endif

            this.Background = Brushes.Transparent;
        }

        /// <summary>
        /// 获取当前正在显示的文档的事件输入器
        /// </summary>
        /// <returns></returns>
        private VTEventInput GetActiveEventInput()
        {
            return this.videoTerminal.ActiveDocument.EventInput;
        }

        private WPFDocument GetActiveDocument()
        {
            return this.videoTerminal.IsAlternate ?
                DocumentAlternate : DocumentMain;
        }

        private MouseData GetMouseData(object sender, MouseButtonEventArgs e)
        {
            WPFDocument document = this.GetActiveDocument();
            Point mousePosition = e.GetPosition(document);
            MouseData mouseData = new MouseData(mousePosition.X, mousePosition.Y, e.ClickCount, (sender as FrameworkElement).IsMouseCaptured);

            return mouseData;
        }

        private MouseData GetMouseData(object sender, MouseEventArgs e)
        {
            WPFDocument document = this.GetActiveDocument();
            Point mousePosition = e.GetPosition(document);
            MouseData mouseData = new MouseData(mousePosition.X, mousePosition.Y, 0, (sender as FrameworkElement).IsMouseCaptured);

            return mouseData;
        }

        private void HandleCaptureAction(object sender, MouseData mouseData)
        {
            switch (mouseData.CaptureAction)
            {
                case MouseData.CaptureActions.None:
                    {
                        break;
                    }

                case MouseData.CaptureActions.Capture:
                    {
                        (sender as FrameworkElement).CaptureMouse();
                        break;
                    }

                case MouseData.CaptureActions.ReleaseCapture:
                    {
                        (sender as FrameworkElement).ReleaseMouseCapture();
                        break;
                    }

                default:
                    throw new NotImplementedException();
            }
        }

        #endregion

        #region 事件处理器

        private void Surface_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            this.Focus();

            MouseData mouseData = this.GetMouseData(sender, e);
            VTEventInput eventInput = this.GetActiveEventInput();
            eventInput.OnMouseMove(mouseData);
            this.HandleCaptureAction(sender, mouseData);
        }

        private void Surface_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MouseData mouseData = this.GetMouseData(sender, e);
            VTEventInput eventInput = this.GetActiveEventInput();
            eventInput.OnMouseUp(mouseData);
            this.HandleCaptureAction(sender, mouseData);
        }

        private void Surface_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MouseData mouseData = this.GetMouseData(sender, e);
            VTEventInput eventInput = this.GetActiveEventInput();
            eventInput.OnMouseDown(mouseData);
            this.HandleCaptureAction(sender, mouseData);
        }


        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            VTEventInput eventInput = this.GetActiveEventInput();
            eventInput.OnMouseWheel(e.Delta > 0);

            e.Handled = true;
        }

        private void TerminalContentUserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.videoTerminal.OnSizeChanged(e.PreviousSize.ToVTSize(), e.NewSize.ToVTSize());
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
            ShellFunctionMenu functionMenu = menuItem.DataContext as ShellFunctionMenu;
            if (functionMenu == null)
            {
                return;
            }

            if (functionMenu.Execute == null)
            {
                return;
            }

            functionMenu.Execute();
        }

        #endregion

        #region ISessionContent

        public int Open()
        {
            string background = this.Session.GetOption<string>(OptionKeyEnum.THEME_BACKGROUND_COLOR);
            BorderBackground.Background = DrawingUtils.GetBrush(background);

            double margin = this.Session.GetOption<double>(OptionKeyEnum.SSH_THEME_CONTENT_MARGIN);
            DocumentAlternate.Margin = new Thickness(margin);
            DocumentAlternate.Surface.PreviewMouseLeftButtonDown += Surface_PreviewMouseLeftButtonDown;
            DocumentAlternate.Surface.PreviewMouseLeftButtonUp += Surface_PreviewMouseLeftButtonUp;
            DocumentAlternate.Surface.PreviewMouseMove += Surface_PreviewMouseMove;
            DocumentMain.Margin = new Thickness(margin);
            DocumentMain.Surface.PreviewMouseLeftButtonDown += Surface_PreviewMouseLeftButtonDown;
            DocumentMain.Surface.PreviewMouseLeftButtonUp += Surface_PreviewMouseLeftButtonUp;
            DocumentMain.Surface.PreviewMouseMove += Surface_PreviewMouseMove;

            this.shellSession = this.DataContext as ShellSessionVM;
            this.shellSession.MainDocument = DocumentMain;
            this.shellSession.AlternateDocument = DocumentAlternate;
            this.shellSession.Open();

            this.videoTerminal = this.shellSession.VideoTerminal;

            this.SizeChanged += TerminalContentUserControl_SizeChanged;

            return ResponseCode.SUCCESS;
        }

        public void Close()
        {
            this.SizeChanged -= TerminalContentUserControl_SizeChanged;

            DocumentAlternate.Surface.PreviewMouseLeftButtonDown -= Surface_PreviewMouseLeftButtonDown;
            DocumentAlternate.Surface.PreviewMouseLeftButtonUp -= Surface_PreviewMouseLeftButtonUp;
            DocumentAlternate.Surface.PreviewMouseMove -= Surface_PreviewMouseMove;
            DocumentMain.Surface.PreviewMouseLeftButtonDown -= Surface_PreviewMouseLeftButtonDown;
            DocumentMain.Surface.PreviewMouseLeftButtonUp -= Surface_PreviewMouseLeftButtonUp;
            DocumentMain.Surface.PreviewMouseMove -= Surface_PreviewMouseMove;

            this.shellSession.Close();
            this.videoTerminal = null;
        }

        #endregion
    }
}

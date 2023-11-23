using DotNEToolkit;
using Microsoft.Win32;
using ModengTerm;
using ModengTerm.Base;
using ModengTerm.Base.Enumerations;
using ModengTerm.Controls;
using ModengTerm.Rendering;
using ModengTerm.Terminal;
using ModengTerm.Terminal.DataModels;
using ModengTerm.Terminal.Document;
using ModengTerm.Terminal.Enumerations;
using ModengTerm.Terminal.Loggering;
using ModengTerm.Terminal.Rendering;
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
    public partial class TerminalContentUserControl : SessionContent, IDrawingTerminal
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("TerminalUserControl");

        #endregion

        #region 实例变量

        private ShellSessionVM shellSession;
        private IVideoTerminal videoTerminal;

        #endregion

        #region 属性

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

        #endregion

        #region 事件处理器

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            this.Focus();
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

        #region IDrawingWindow

        public IDrawingDocument CreateCanvas()
        {
            DrawingDocument drawingDocument = new DrawingDocument();
            return drawingDocument;
        }

        public void InsertCanvas(int index, IDrawingDocument document)
        {
            this.Dispatcher.Invoke(() =>
            {
                FrameworkElement element = document as FrameworkElement;
                GridDocumentList.Children.Insert(0, element);

                // 添加到子节点里之后马上加载模板，不然后面新建DrawingObject的时候模板还没加载，找不到drawArea
                element.ApplyTemplate();
            });
        }

        public void RemoveDocument(IDrawingDocument document)
        {
            DrawingDocument drawingDocument = document as DrawingDocument;
            GridDocumentList.Children.Remove(drawingDocument);
        }

        public void VisibleCanvas(IDrawingDocument document, bool visible)
        {
            base.Dispatcher.Invoke(() =>
            {
                (document as FrameworkElement).Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
            });
        }

        public VTypeface GetTypeface(double fontSize, string fontFamily)
        {
            Typeface typeface = DrawingUtils.GetTypeface(fontFamily);
            FormattedText formattedText = new FormattedText(" ", CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeface,
                fontSize, Brushes.Black, null, TextFormattingMode.Display, DrawingUtils.PixelPerDpi);

            return new VTypeface()
            {
                FontSize = fontSize,
                FontFamily = fontFamily,
                Height = formattedText.Height,
                SpaceWidth = formattedText.WidthIncludingTrailingWhitespace
            };
        }

        public VTSize GetSize()
        {
            return new VTSize(GridDocumentList.ActualWidth, GridDocumentList.ActualHeight);
        }

        #endregion

        #region SessionContent

        protected override int OnOpen(OpenedSessionVM viewModel)
        {
            this.shellSession = viewModel as ShellSessionVM;
            this.shellSession.Open();

            this.videoTerminal = this.shellSession.VideoTerminal;

            return ResponseCode.SUCCESS;
        }

        protected override void OnClose()
        {
            this.shellSession.Close();
            this.videoTerminal = null;
        }

        #endregion
    }
}

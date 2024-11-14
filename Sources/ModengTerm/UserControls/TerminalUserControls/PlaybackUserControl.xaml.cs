using ModengTerm.Base.DataModels;
using ModengTerm.Base.Enumerations;
using ModengTerm.Document;
using ModengTerm.Document.Enumerations;
using ModengTerm.Terminal;
using ModengTerm.Terminal.Session;
using ModengTerm.Terminal.ViewModels;
using ModengTerm.UserControls.TerminalUserControls.Rendering;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ModengTerm.UserControls.Terminals
{
    /// <summary>
    /// PlaybackUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class PlaybackUserControl : UserControl
    {
        #region 实例变量

        private XTermSession session;
        private PlaybackSessionVM playbackVM;
        private IVideoTerminal videoTerminal;

        #endregion

        #region 构造方法

        public PlaybackUserControl()
        {
            InitializeComponent();

            this.InitializeUserControl();
        }

        #endregion

        #region 实例方法

        private void InitializeUserControl() { }

        /// <summary>
        /// 获取当前正在显示的文档的事件输入器
        /// </summary>
        /// <returns></returns>
        private VTEventInput GetActiveEventInput()
        {
            return this.videoTerminal.ActiveDocument.EventInput;
        }

        private DocumentControl GetActiveDocument()
        {
            return this.videoTerminal.IsAlternate ?
                DocumentAlternate : DocumentMain;
        }

        private MouseData GetMouseData(object sender, MouseButtonEventArgs e)
        {
            DocumentControl document = this.GetActiveDocument();
            Point mousePosition = e.GetPosition(document);
            MouseData mouseData = new MouseData(mousePosition.X, mousePosition.Y, e.ClickCount, (sender as FrameworkElement).IsMouseCaptured);

            return mouseData;
        }

        private MouseData GetMouseData(object sender, MouseEventArgs e)
        {
            DocumentControl document = this.GetActiveDocument();
            DrawingArea canvas = document.DrawArea;
            Point mousePosition = e.GetPosition(canvas);
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

        private void DrawArea_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            MouseData mouseData = this.GetMouseData(sender, e);
            VTEventInput eventInput = this.GetActiveEventInput();
            eventInput.OnMouseMove(mouseData);
            this.HandleCaptureAction(sender, mouseData);
        }

        private void DrawArea_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MouseData mouseData = this.GetMouseData(sender, e);
            VTEventInput eventInput = this.GetActiveEventInput();
            eventInput.OnMouseUp(mouseData);
            this.HandleCaptureAction(sender, mouseData);
        }

        private void DrawArea_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MouseData mouseData = this.GetMouseData(sender, e);
            VTEventInput eventInput = this.GetActiveEventInput();
            eventInput.OnMouseDown(mouseData);
            this.HandleCaptureAction(sender, mouseData);
        }

        private void DrawArea_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            // 右键直接复制选中内容
            VTParagraph paragraph = this.videoTerminal.CreateParagraph(ParagraphTypeEnum.Selected, ParagraphFormatEnum.PlainText);
            if (paragraph.IsEmpty)
            {
                return;
            }

            // 把数据设置到Windows剪贴板里
            System.Windows.Clipboard.SetText(paragraph.Content);

            this.videoTerminal.UnSelectAll();
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            VTEventInput eventInput = this.GetActiveEventInput();
            eventInput.OnMouseWheel(e.Delta > 0);

            e.Handled = true;
        }

        #endregion

        #region 公开接口

        public void Open(Playback playback)
        {
            this.session = playback.Session;

            string background = session.GetOption<string>(OptionKeyEnum.THEME_BACKGROUND_COLOR);
            BorderBackground.Background = DrawingUtils.GetBrush(background);

            // 不要直接使用Document的DrawAreaSize属性，DrawAreaSize可能不准确！
            // 在设置完Padding之后，DrawAreaSize的宽度和高度有可能不会实时变化
            // 根据Padding手动计算终端宽度和高度
            double padding = session.GetOption<double>(OptionKeyEnum.SSH_THEME_DOCUMENT_PADDING);
            double width = DocumentMain.DrawAreaSize.Width - padding * 2;
            double height = DocumentMain.DrawAreaSize.Height - padding * 2;

            DocumentAlternate.SetPadding(padding);
            DocumentAlternate.DrawArea.PreviewMouseLeftButtonDown += DrawArea_PreviewMouseLeftButtonDown;
            DocumentAlternate.DrawArea.PreviewMouseLeftButtonUp += DrawArea_PreviewMouseLeftButtonUp;
            DocumentAlternate.DrawArea.PreviewMouseMove += DrawArea_PreviewMouseMove;
            DocumentAlternate.DrawArea.PreviewMouseRightButtonDown += DrawArea_PreviewMouseRightButtonDown;
            DocumentMain.SetPadding(padding);
            DocumentMain.DrawArea.PreviewMouseLeftButtonDown += DrawArea_PreviewMouseLeftButtonDown;
            DocumentMain.DrawArea.PreviewMouseLeftButtonUp += DrawArea_PreviewMouseLeftButtonUp;
            DocumentMain.DrawArea.PreviewMouseMove += DrawArea_PreviewMouseMove;
            DocumentMain.DrawArea.PreviewMouseRightButtonDown += DrawArea_PreviewMouseRightButtonDown;

            PlaybackOptions playbackOptions = new PlaybackOptions()
            {
                Session = session,
                AlternateDocument = DocumentAlternate,
                MainDocument = DocumentMain,
                Height = height,
                Width = width,
                Playback = playback
            };
            this.playbackVM = new PlaybackSessionVM();
            this.playbackVM.Open(playbackOptions);

            this.videoTerminal = this.playbackVM.VideoTerminal;
        }

        public void Close()
        {
            this.playbackVM.Close();

            DocumentAlternate.DrawArea.PreviewMouseLeftButtonDown -= DrawArea_PreviewMouseLeftButtonDown;
            DocumentAlternate.DrawArea.PreviewMouseLeftButtonUp -= DrawArea_PreviewMouseLeftButtonUp;
            DocumentAlternate.DrawArea.PreviewMouseMove -= DrawArea_PreviewMouseMove;
            DocumentAlternate.DrawArea.PreviewMouseRightButtonDown -= DrawArea_PreviewMouseRightButtonDown;
            DocumentMain.DrawArea.PreviewMouseLeftButtonDown -= DrawArea_PreviewMouseLeftButtonDown;
            DocumentMain.DrawArea.PreviewMouseLeftButtonUp -= DrawArea_PreviewMouseLeftButtonUp;
            DocumentMain.DrawArea.PreviewMouseMove -= DrawArea_PreviewMouseMove;
            DocumentMain.DrawArea.PreviewMouseRightButtonDown -= DrawArea_PreviewMouseRightButtonDown;
        }

        #endregion
    }
}

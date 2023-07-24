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
using XTerminal.Base;
using XTerminal.Document;
using XTerminal.Document.Rendering;
using XTerminal.Rendering;
using XTerminal.Session;

namespace XTerminal.UserControls
{
    /// <summary>
    /// TerminalUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class TerminalScreenUserControl : UserControl, ITerminalScreen
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("TerminalUserControl");

        #endregion

        #region 公开事件

        public event Action<ITerminalScreen, VTInputEvent> InputEvent;

        public event Action<ITerminalScreen, int> ScrollChanged;

        /// <summary>
        /// 鼠标移动的时候触发
        /// </summary>
        public event Action<ITerminalScreen, VTPoint> VTMouseMove;

        /// <summary>
        /// 鼠标按下的时候触发
        /// </summary>
        public event Action<ITerminalScreen, VTPoint> VTMouseDown;

        /// <summary>
        /// 鼠标抬起的时候触发
        /// </summary>
        public event Action<ITerminalScreen, VTPoint> VTMouseUp;

        public event Action<ITerminalScreen, bool> VTMouseWheel;

        #endregion

        #region 实例变量

        private VTInputEvent inputEvent;
        private int scrollbarCursorDownValue;
        private bool cursorDown;

        #endregion

        #region 属性

        /// <summary>
        /// 该屏幕所显示的终端
        /// </summary>
        public VideoTerminal VideoTerminal { get; set; }

        #endregion

        #region 构造方法

        public TerminalScreenUserControl()
        {
            InitializeComponent();

            this.InitializeUserControl();
        }

        #endregion

        #region 实例方法

        private void InitializeUserControl()
        {
            this.inputEvent = new VTInputEvent();
            this.Background = Brushes.Transparent;
            this.Focusable = true;
            base.Cursor = Cursors.IBeam;
        }

        private void NotifyInputEvent(VTInputEvent evt)
        {
            if (this.InputEvent != null)
            {
                this.InputEvent(this, evt);
            }
        }

        private void HandleScrollEvent()
        {
            if (!this.cursorDown)
            {
                return;
            }

            int newValue = Convert.ToInt32(SliderScrolbar.Value);
            if (newValue != this.scrollbarCursorDownValue)
            {
                if (this.ScrollChanged != null)
                {
                    this.ScrollChanged(this, newValue);
                }
            }

            this.scrollbarCursorDownValue = newValue;
        }

        /// <summary>
        /// 复制
        /// </summary>
        public void Copy()
        {
            this.VideoTerminal.CopySelection();
        }

        /// <summary>
        /// 粘贴
        /// </summary>
        private void Paste()
        {
            this.VideoTerminal.Paste();
        }

        #endregion

        #region 公开接口

        #endregion

        #region 事件处理器

        /// <summary>
        /// 输入中文的时候会触发该事件
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {
            base.OnPreviewTextInput(e);

            this.inputEvent.CapsLock = Console.CapsLock;
            this.inputEvent.Key = VTKeys.None;
            this.inputEvent.Text = e.Text;
            this.inputEvent.Modifiers = VTModifierKeys.None;
            this.NotifyInputEvent(this.inputEvent);

            e.Handled = true;

            //Console.WriteLine(e.Text);
        }

        /// <summary>
        /// 从键盘上按下按键的时候会触发
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);

            //Console.WriteLine(e.Key);

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

                VTKeys vtKey = WPFRenderUtils.ConvertToVTKey(e.Key);
                this.inputEvent.CapsLock = Console.CapsLock;
                this.inputEvent.Key = vtKey;
                this.inputEvent.Text = null;
                this.inputEvent.Modifiers = (VTModifierKeys)e.KeyboardDevice.Modifiers;
                this.NotifyInputEvent(this.inputEvent);
            }
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            this.Focus();
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            this.VTMouseWheel(this, e.Delta > 0);
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


        private void SliderScrolbar_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            this.cursorDown = true;
            this.scrollbarCursorDownValue = Convert.ToInt32(SliderScrolbar.Value);
        }

        private void SliderScrolbar_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            this.HandleScrollEvent();
        }

        private void SliderScrolbar_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            this.HandleScrollEvent();
            this.cursorDown = false;
        }


        private void ContentControlSurface_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ContentControlSurface.CaptureMouse();
            Point p = e.GetPosition(this);
            this.VTMouseDown(this, new VTPoint(p.X, p.Y));
        }

        private void ContentControlSurface_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (ContentControlSurface.IsMouseCaptured)
            {
                Point p = e.GetPosition(this);
                this.VTMouseMove(this, new VTPoint(p.X, p.Y));
            }
        }

        private void ContentControlSurface_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Point p = e.GetPosition(this);
            this.VTMouseUp(this, new VTPoint(p.X, p.Y));
            ContentControlSurface.ReleaseMouseCapture();
        }


        private void MenuItemCopy_Click(object sender, RoutedEventArgs e)
        {
            this.Copy();
        }

        private void MenuItemPaste_Click(object sender, RoutedEventArgs e)
        {
            this.Paste();
        }

        #endregion

        #region ITerminalScreen

        public VTRect GetBoundary()
        {
            Point leftTop = ContentControlSurface.PointToScreen(new Point(0, 0));
            return new VTRect(leftTop.X, leftTop.Y, this.ActualWidth, this.ActualHeight);
        }

        public ITerminalSurface CreateSurface()
        {
            DrawingSurface canvas = new DrawingSurface();
            return canvas;
        }

        public void AddSurface(ITerminalSurface surface)
        {
            ContentControlSurface.Content = surface;
        }

        public void SwitchSurface(ITerminalSurface remove, ITerminalSurface add)
        {
            this.Dispatcher.Invoke(() =>
            {
                //int index = this.Children.IndexOf(remove as DrawingCanvas);
                //this.Children.RemoveAt(index);
                //this.Children.Insert(index, add as DrawingCanvas);
            });
        }

        /// <summary>
        /// 更新滚动信息
        /// </summary>
        /// <param name="maximum">滚动条的最大值</param>
        public void UpdateScrollInfo(int maximum)
        {
            this.SliderScrolbar.Maximum = maximum;
        }

        public void ScrollToEnd(ScrollOrientation orientation)
        {
            switch (orientation)
            {
                case ScrollOrientation.Down:
                    {
                        this.SliderScrolbar.Value = this.SliderScrolbar.Maximum;
                        break;
                    }

                case ScrollOrientation.Up:
                    {
                        this.SliderScrolbar.Value = this.SliderScrolbar.Minimum;
                        break;
                    }

                default:
                    throw new NotImplementedException();
            }
        }

        public void ScrollTo(int scrollValue)
        {
            this.SliderScrolbar.Value = scrollValue;
        }

        #endregion
    }
}

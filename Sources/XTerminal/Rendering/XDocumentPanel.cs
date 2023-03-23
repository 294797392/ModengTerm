using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;
using System.Windows.Media.TextFormatting;
using XTerminal.Document.Rendering;
using XTerminal.Base;
using XTerminal.Document;

namespace XTerminal.Rendering
{
    /// <summary>
    /// 显示器控件
    /// </summary>
    public class XDocumentPanel : Grid, IDocumentCanvasPanel
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("XDocumentPanel");

        #endregion

        #region 公开事件

        public event Action<IDocumentCanvasPanel, VTInputEvent> InputEvent;

        public event Action<IDocumentCanvasPanel, int> ScrollChanged;

        /// <summary>
        /// 鼠标移动的时候触发
        /// </summary>
        public event Action<IDocumentCanvasPanel, VTPoint> VTMouseMove;

        /// <summary>
        /// 鼠标按下的时候触发
        /// </summary>
        public event Action<IDocumentCanvasPanel, VTPoint> VTMouseDown;

        /// <summary>
        /// 鼠标抬起的时候触发
        /// </summary>
        public event Action<IDocumentCanvasPanel, VTPoint> VTMouseUp;

        #endregion

        #region 实例变量

        private VTInputEvent inputEvent;
        private Slider scrollbar;
        private int scrollbarCursorDownValue;
        private bool cursorDown;

        #endregion

        #region 属性

        /// <summary>
        /// 滚动条
        /// </summary>
        public Slider Scrollbar
        {
            get { return this.scrollbar; }
            set
            {
                this.scrollbar = value;
                this.scrollbar.PreviewMouseMove += Scrollbar_PreviewMouseMove;
                this.scrollbar.PreviewMouseLeftButtonDown += Scrollbar_MouseLeftButtonDown;
                this.scrollbar.PreviewMouseLeftButtonUp += Scrollbar_MouseLeftButtonUp;
            }
        }

        #endregion

        #region 构造方法

        public XDocumentPanel()
        {
            this.inputEvent = new VTInputEvent();
            this.Background = Brushes.Transparent;
            this.Focusable = true;
        }

        #endregion

        #region 实例方法

        private void NotifyInputEvent(VTInputEvent evt)
        {
            if (this.InputEvent != null)
            {
                this.InputEvent(this, evt);
            }
        }

        private void HandleMouseScrollEvent()
        {
            if (!this.cursorDown)
            {
                return;
            }

            int newValue = Convert.ToInt32(this.scrollbar.Value);
            if (newValue != this.scrollbarCursorDownValue)
            {
                if (this.ScrollChanged != null)
                {
                    this.ScrollChanged(this, newValue);
                }
            }

            this.scrollbarCursorDownValue = newValue;
        }

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



        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseDown(e);

            Point p = e.GetPosition(this);
            this.VTMouseDown(this, new VTPoint(p.X, p.Y));
        }

        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            base.OnPreviewMouseMove(e);

            Point p = e.GetPosition(this);
            this.VTMouseMove(this, new VTPoint(p.X, p.Y));
        }

        protected override void OnPreviewMouseUp(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseUp(e);

            Point p = e.GetPosition(this);
            this.VTMouseUp(this, new VTPoint(p.X, p.Y));
        }


        private void Scrollbar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.cursorDown = true;
            this.scrollbarCursorDownValue = Convert.ToInt32(this.scrollbar.Value);
        }

        private void Scrollbar_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.HandleMouseScrollEvent();
            this.cursorDown = false;
        }

        private void Scrollbar_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            this.HandleMouseScrollEvent();
        }

        #endregion

        #region IDocumentCanvasPanel

        public IDocumentCanvas CreateCanvas()
        {
            XDocumentCanvas canvas = new XDocumentCanvas();
            return canvas;
        }

        public void AddCanvas(IDocumentCanvas canvas)
        {
            this.Children.Add(canvas as XDocumentCanvas);
        }

        /// <summary>
        /// 更新滚动信息
        /// </summary>
        /// <param name="maximum">滚动条的最大值</param>
        public void UpdateScrollInfo(int maximum)
        {
            this.Scrollbar.Maximum = maximum;
        }

        /// <summary>
        /// 滚动到某一个历史行
        /// 默认把历史行设置为滚动之后的窗口中的第一行
        /// </summary>
        /// <param name="historyLine">要滚动到的历史行</param>
        public void ScrollToHistoryLine(VTHistoryLine historyLine)
        {
            this.Scrollbar.Value = historyLine.Row;
        }

        public void ScrollToEnd(ScrollOrientation orientation)
        {
            switch (orientation)
            {
                case ScrollOrientation.Down:
                    {
                        this.Scrollbar.Value = this.Scrollbar.Maximum;
                        break;
                    }

                case ScrollOrientation.Up:
                    {
                        this.Scrollbar.Value = this.Scrollbar.Minimum;
                        break;
                    }

                default:
                    throw new NotImplementedException();
            }
        }

        #endregion
    }
}

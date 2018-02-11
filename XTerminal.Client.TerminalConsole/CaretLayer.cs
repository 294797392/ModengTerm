using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace XTerminal.Client.TerminalConsole
{
    public class CaretLayer : FrameworkElement
    {
        #region 实例变量

        private DrawingVisual caretVisual;
        private DispatcherTimer caretTimer;

        #endregion

        #region 属性

        public static readonly DependencyProperty CaretWidthProperty = DependencyProperty.Register("CaretWidth", typeof(double), typeof(CaretLayer), new PropertyMetadata(DefaultValues.CaretWidth));
        public double CaretWidth
        {
            get
            {
                return (double)base.GetValue(CaretWidthProperty);
            }
            set
            {
                base.SetValue(CaretWidthProperty, value);
            }
        }

        public static readonly DependencyProperty CaretHeightProperty = DependencyProperty.Register("CaretHeight", typeof(double), typeof(CaretLayer), new PropertyMetadata(DefaultValues.CaretHeight));
        public double CaretHeight
        {
            get
            {
                return (double)base.GetValue(CaretHeightProperty);
            }
            set
            {
                base.SetValue(CaretHeightProperty, value);
            }
        }

        public static readonly DependencyProperty CaretPositionProperty = DependencyProperty.Register("CaretPosition", typeof(Vector), typeof(CaretLayer), new PropertyMetadata(new Vector(), new PropertyChangedCallback(CaretPositionPropertyChangedCallback)));
        public Vector CaretPosition
        {
            get
            {
                return (Vector)base.GetValue(CaretPositionProperty);
            }
            set
            {
                base.SetValue(CaretPositionProperty, value);
            }
        }

        #endregion

        #region 构造方法

        public CaretLayer()
        {
            this.InitializeLayer();
        }

        #endregion

        #region 实例方法

        private void InitializeLayer()
        {
            this.caretVisual = new DrawingVisual();
            var drawCtx = this.caretVisual.RenderOpen();
            drawCtx.DrawRectangle(DefaultValues.CaretBrush, null, new Rect(0, 0, this.CaretWidth, this.CaretHeight));
            drawCtx.Close();
            base.AddVisualChild(this.caretVisual);

            this.caretTimer = new DispatcherTimer(DispatcherPriority.Render);
            this.caretTimer.Interval = TimeSpan.FromMilliseconds(GetCaretBlinkTime());
            this.caretTimer.Tick += this.CaretTimer_Tick;
            this.caretTimer.Start();
        }

        [DllImport("user32.dll")]
        public static extern uint GetCaretBlinkTime();

        #endregion

        #region 重写方法

        protected override int VisualChildrenCount
        {
            get
            {
                return 1;
            }
        }

        protected override Visual GetVisualChild(int index)
        {
            return this.caretVisual;
        }

        #endregion


        #region 事件处理器

        private static void CaretPositionPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as CaretLayer).OnCaretPositionChanged(e.NewValue, e.OldValue);
        }
        protected void OnCaretPositionChanged(object newValue, object oldValue)
        {
            var position = (Vector)newValue;
            this.caretVisual.Offset = position;
        }

        private void CaretTimer_Tick(object sender, EventArgs e)
        {
            if (this.caretVisual.Opacity == 1)
            {
                this.caretVisual.Opacity = 0;
            }
            else
            {
                this.caretVisual.Opacity = 1;
            }
        }

        #endregion
    }
}
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using XTerminal.Terminal;

namespace XTerminal.Client.TerminalConsole
{
    /// <summary>
    /// 使用DrawingVisual显示Terminal里的每一行
    /// 为了使用DrawingVisual对象，需要为这些对象创建一个宿主容器。该宿主容器对象必须派生自FrameworkElement类
    /// 宿主容器对象负责管理它的可视对象集合。 这需要宿主容器为派生的 FrameworkElement 类实现成员重写。
    /// 必须重写的两个成员：
    /// GetVisualChild：从子元素集合返回指定索引处的子级。
    /// VisualChildrenCount：获取此元素内部的可视子元素的数目。
    /// 
    /// 
    /// 
    /// TODO:如何计算最后一行相对于控件最上方的位置？目前使用的方法是：GetLastLineOffsetY()
    /// </summary>
    public class RenderLayer : FrameworkElement
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("TerminalTextLayer");

        #endregion

        #region 实例变量

        private AutoResetEvent renderEvt;
        private Thread renderThread;
        private List<TerminalLine> lines;
        private TerminalLine currentLine;
        private int previewMeasuredTotalLines; // 上次测量控件应该占据的空间大小的时候的行数
        private ScrollViewer scrollViewer;

        #endregion

        #region 属性

        public static readonly DependencyProperty FontSizeProperty = DependencyProperty.Register("FontSize", typeof(double), typeof(RenderLayer), new PropertyMetadata(12d));
        public double FontSize
        {
            get
            {
                return (double)base.GetValue(FontSizeProperty);
            }
            set
            {
                base.SetValue(FontSizeProperty, value);
            }
        }

        public static readonly DependencyProperty LineMarginProperty = DependencyProperty.Register("LineMargin", typeof(Thickness), typeof(RenderLayer), new PropertyMetadata(DefaultValues.LineMargin));
        public Thickness LineMargin
        {
            get
            {
                return (Thickness)base.GetValue(LineMarginProperty);
            }
            set
            {
                base.SetValue(LineMarginProperty, value);
            }
        }

        #endregion

        #region 构造方法

        public RenderLayer()
        {
            this.InitializeLayer();
        }

        #endregion

        #region 实例方法

        private void InitializeLayer()
        {
            base.HorizontalAlignment = HorizontalAlignment.Left;
            base.VerticalAlignment = VerticalAlignment.Top;

            this.lines = new List<TerminalLine>();
            this.renderEvt = new AutoResetEvent(false);

            this.renderThread = new Thread(this.RenderThreadProcess);
            this.renderThread.IsBackground = true;
            this.renderThread.Start();
        }

        internal void HandleCommandInput(IEnumerable<IEscapeSequencesCommand> cmds)
        {
        }

        internal void HandleTextInput(string txt)
        {
            if (txt.Equals("\r") || this.currentLine == null)
            {
                double offsetY = this.GetLastLineOffsetY();
                TerminalLine line = new TerminalLine();
                line.Offset = new Vector(0.0, offsetY);
                line.Margin = this.LineMargin;
                this.lines.Add(line);
                base.AddVisualChild(line);
                this.currentLine = line;
            }

            Typeface face = new Typeface(new FontFamily(), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
            TerminalText text = new TerminalText(txt, CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, face, this.FontSize, Brushes.Black, new NumberSubstitution(), TextFormattingMode.Ideal);
            this.currentLine.AddText(text);
            this.renderEvt.Set();
            this.MeasureSize();
        }

        /// <summary>
        /// 获取最后一行的Y坐标偏移
        /// </summary>
        /// <returns></returns>
        internal double GetLastLineOffsetY()
        {
            double offsetY = 0;
            foreach (TerminalLine line in this.lines)
            {
                offsetY += line.Height;
                offsetY += line.Margin.Top;
                offsetY += line.Margin.Bottom;
            }
            return offsetY;
        }

        /// <summary>
        /// 获取最后一行的最后一个字符的X坐标偏移量
        /// </summary>
        /// <returns></returns>
        internal double GetLastLineLastCharOffsetX()
        {
            TerminalLine lastLine = this.lines.LastOrDefault();
            if (lastLine == null)
            {
                return 0;
            }

            return lastLine.Width;
        }

        /// <summary>
        /// 根据当前所显示的文本，计算出控件应该占据的空间大小
        /// </summary>
        private void MeasureSize()
        {
            if (this.previewMeasuredTotalLines != this.lines.Count)
            {
                this.previewMeasuredTotalLines = this.lines.Count;
                double width = 0, height = 0;
                foreach (TerminalLine line in this.lines)
                {
                    height += line.Height;
                    height += line.Margin.Top;
                    height += line.Margin.Bottom;
                }
                base.Height = height;

                if (this.scrollViewer == null)
                {
                    this.scrollViewer = base.Parent as ScrollViewer;
                }
                if (this.scrollViewer != null)
                {
                    this.scrollViewer.ScrollToEnd();
                }
            }
        }

        #endregion

        #region 重写方法

        protected override int VisualChildrenCount
        {
            get { return this.lines.Count; }
        }

        protected override Visual GetVisualChild(int index)
        {
            if (index > this.lines.Count)
            {
                throw new Exception("Error, VisualChild NotFound");
            }

            return this.lines[index];
        }

        #endregion

        #region 事件处理器

        private void RenderThreadProcess(object state)
        {
            while (true)
            {
                this.renderEvt.WaitOne(); // 等待渲染通知

                Console.WriteLine("渲染");

                base.Dispatcher.Invoke(new Action(this.currentLine.Draw));
            }
        }

        #endregion
    }
}
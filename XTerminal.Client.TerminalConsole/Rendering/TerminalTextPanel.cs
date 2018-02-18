using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;
using XTerminal.Terminal;

namespace XTerminal.Client.TerminalConsole.Rendering
{
    /// <summary>
    /// 处理输入输出
    /// 管理文本渲染
    /// 
    /// 使用DrawingVisual显示Terminal里的每一行
    /// 为了使用DrawingVisual对象，需要为这些对象创建一个宿主容器。该宿主容器对象必须派生自FrameworkElement类
    /// 宿主容器对象负责管理它的可视对象集合。 这需要宿主容器为派生的 FrameworkElement 类实现成员重写。
    /// 必须重写的两个成员：
    /// GetVisualChild：从子元素集合返回指定索引处的子级。
    /// VisualChildrenCount：获取此元素内部的可视子元素的数目。
    /// </summary>
    public class TerminalTextPanel : Panel
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("TerminalTextLayer");

        #endregion

        #region 实例变量

        private List<TerminalLine> lines;
        private TerminalLine currentLine;
        private int previewMeasuredTotalLines; // 上次测量控件应该占据的空间大小的时候的行数
        private ScrollViewer scrollViewer;
        private TextFormatter textFormatter;

        #endregion

        #region 属性

        public static readonly DependencyProperty FontSizeProperty = DependencyProperty.Register("FontSize", typeof(double), typeof(TerminalTextPanel), new PropertyMetadata(12d));
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

        public static readonly DependencyProperty LineMarginProperty = DependencyProperty.Register("LineMargin", typeof(Thickness), typeof(TerminalTextPanel), new PropertyMetadata(DefaultValues.LineMargin));
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

        public TerminalTextPanel()
        {
            this.InitializePanel();
        }

        #endregion

        #region 实例方法

        private void InitializePanel()
        {
            base.HorizontalAlignment = HorizontalAlignment.Stretch;
            base.VerticalAlignment = VerticalAlignment.Stretch;
            base.Background = Brushes.Transparent;

            this.lines = new List<TerminalLine>();

            this.textFormatter = TextFormatter.Create();
        }

        internal void HandleCommandInput(IEnumerable<IEscapeSequencesCommand> cmds)
        {
        }

        internal void HandleTextInput(TextCompositionEventArgs e)
        {
            if (e.Text == "\n" || e.Text == "\r" || e.Text == "\r\n" || this.currentLine == null)
            {
                // 处理换行符
                this.currentLine = this.CreateTerminalLine();
                base.AddVisualChild(this.currentLine.TextVisual);
            }
            else
            {
                // 处理普通字符
                this.currentLine.InputChar(e.Text[0]);
            }
        }

        private TerminalLine CreateTerminalLine()
        {
            TerminalTextRunProperties textRunProperties = new TerminalTextRunProperties();
            textRunProperties.typeface = new Typeface(DefaultValues.FontFamily, DefaultValues.FontStyle, DefaultValues.FontWeight, DefaultValues.FontStretch);
            textRunProperties.cultureInfo = CultureInfo.CurrentCulture;
            textRunProperties.fontRenderingEmSize = DefaultValues.FontSize;

            TerminalTextParagraphProperties paragraphProperties = new TerminalTextParagraphProperties();
            paragraphProperties.defaultTextRunProperties = textRunProperties;

            TerminalLineTextElement textElement = new TerminalLineTextElement();
            textElement.ColumnIndex = 0;

            TerminalLineTextSource textSource = new TerminalLineTextSource();
            textSource.lineElements = new List<TerminalLineElement>();
            textSource.lineElements.Add(textElement);
            TextLine textLine = this.textFormatter.FormatLine(textSource, 0, 1024, paragraphProperties, null);
            TerminalLine line = new TerminalLine(textLine, textSource);
            this.lines.Add(line);
            return line;
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

            return this.lines[index].TextVisual;
        }

        #endregion

        #region 事件处理器

        #endregion
    }
}
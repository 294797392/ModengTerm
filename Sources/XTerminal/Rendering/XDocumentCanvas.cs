using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using XTerminal.Document;
using XTerminal.Document.Rendering;

namespace XTerminal.Rendering
{
    /// <summary>
    /// 用来显示字符的容器
    /// </summary>
    public class XDocumentCanvas : FrameworkElement, IDocumentCanvas
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("DocumentRenderer");

        #endregion

        #region 实例变量

        private ScrollViewer scrollViewer;

        private VisualCollection visuals;

        private DocumentCanvasOptions options;

        private List<IDocumentDrawable> drawableLines;
        private DrawableCursor drawableCursor;

        #endregion

        #region 属性

        // Provide a required override for the VisualChildrenCount property.
        protected override int VisualChildrenCount
        {
            get { return this.visuals.Count; }
        }

        #endregion

        #region 构造方法

        public XDocumentCanvas()
        {
            this.visuals = new VisualCollection(this);
        }

        #endregion

        #region 重写方法

        // Provide a required override for the GetVisualChild method.
        protected override Visual GetVisualChild(int index)
        {
            return this.visuals[index];
        }

        #endregion

        #region 实例方法

        private bool EnsureScrollViewer()
        {
            if (this.scrollViewer == null)
            {
                this.scrollViewer = this.Parent as ScrollViewer;
            }

            return this.scrollViewer != null;
        }

        #endregion

        #region IDocumentCanvas

        public void Initialize(DocumentCanvasOptions options)
        {
            this.options = options;

            this.drawableLines = new List<IDocumentDrawable>();
            for (int i = 0; i < options.Rows; i++)
            {
                DrawableLine drawableLine = new DrawableLine(i);
                this.visuals.Add(drawableLine);
                this.drawableLines.Add(drawableLine);
            }

            this.drawableCursor = new DrawableCursor();
            this.visuals.Add(this.drawableCursor);
        }

        /// <summary>
        /// 获取行的渲染对象
        /// 行数量等于可视区域的行数量
        /// </summary>
        /// <returns></returns>
        public List<IDocumentDrawable> GetDrawableLines()
        {
            return this.drawableLines;
        }

        /// <summary>
        /// 获取光标的渲染对象
        /// </summary>
        /// <returns></returns>
        public IDocumentDrawable GetDrawableCursor()
        {
            return this.drawableCursor;
        }

        public VTElementMetrics MeasureLine(VTextLine textLine, int maxCharacters)
        {
            string text = textLine.GetText();
            if (maxCharacters > 0 && text.Length >= maxCharacters)
            {
                text = text.Substring(0, maxCharacters);
            }

            Typeface typeface = WPFRenderUtils.GetTypeface(VTextStyle.Default);
            FormattedText formattedText = new FormattedText(text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeface, VTextStyle.Default.FontSize, Brushes.Black, null, TextFormattingMode.Display, App.PixelsPerDip);

            VTElementMetrics metrics = new VTElementMetrics() 
            {
                Height = formattedText.Height,
                Width = formattedText.Width,
                WidthIncludingWhitespace = formattedText.WidthIncludingTrailingWhitespace
            };

            return metrics;
        }

        public void DrawDrawable(IDocumentDrawable drawable)
        {
            XDocumentDrawable drawingVisual = drawable as XDocumentDrawable;
            drawingVisual.Draw();
        }

        public void UpdatePosition(IDocumentDrawable drawable, double offsetX, double offsetY)
        {
            XDocumentDrawable drawingVisual = drawable as XDocumentDrawable;
            drawingVisual.Offset = new Vector(offsetX, offsetY);
        }

        public void SetOpacity(IDocumentDrawable drawable, double opacity)
        {
            XDocumentDrawable drawingVisual = drawable as XDocumentDrawable;
            drawingVisual.Opacity = opacity;
        }

        #endregion
    }
}

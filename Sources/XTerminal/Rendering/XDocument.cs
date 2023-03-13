using System;
using System.Collections.Generic;
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
    public class XDocument : FrameworkElement, IDocumentRenderer
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("DocumentRenderer");

        #endregion

        #region 实例变量

        private ScrollViewer scrollViewer;

        private VisualCollection visuals;

        private double fullWidth;
        private double fullHeight;

        private DocumentRendererOptions options;

        private VTElementMetrics blankCharacterMetrics;

        private List<IDocumentDrawable> drawableLines;
        private IDocumentDrawable drawableCursor;

        #endregion

        #region 属性

        // Provide a required override for the VisualChildrenCount property.
        protected override int VisualChildrenCount
        {
            get { return this.visuals.Count; }
        }

        #endregion

        #region 构造方法

        public XDocument()
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

        protected override Size MeasureOverride(Size constraint)
        {
            //return base.MeasureOverride(constraint);
            return new Size(this.fullWidth, this.fullHeight);
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

        /// <summary>
        /// 获取没有被使用的DrawingVisual
        /// </summary>
        /// <returns></returns>
        private DrawableLine RequestDrawingLine()
        {
            return this.visuals.Cast<DrawableLine>().FirstOrDefault(v => v.OwnerElement == null);
        }

        #endregion

        #region IDocumentRenderer

        public void Initialize(DocumentRendererOptions options)
        {
            this.options = options;

            this.drawableLines = new List<IDocumentDrawable>();
            this.blankCharacterMetrics = TerminalUtils.UpdateTextMetrics(" ", VTextStyle.Default);
            for (int i = 0; i < options.Rows; i++)
            {
                DrawableLine drawableLine = new DrawableLine() { Row = i };
                this.visuals.Add(drawableLine);
                this.drawableLines.Add(drawableLine);
            }
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

        public VTElementMetrics MeasureText(string text, VTextStyle style)
        {
            return TerminalUtils.UpdateTextMetrics(text, style);
        }

        public void Resize(double width, double height)
        {
            this.fullWidth = width;
            this.fullHeight = height;
            this.InvalidateMeasure();
        }

        //public void ScrollToEnd(ScrollOrientation orientation)
        //{
        //    if (!this.EnsureScrollViewer())
        //    {
        //        return;
        //    }

        //    switch (orientation)
        //    {
        //        case ScrollOrientation.Bottom:
        //            {
        //                this.scrollViewer.ScrollToEnd();
        //                break;
        //            }

        //        case ScrollOrientation.Left:
        //            {
        //                this.scrollViewer.ScrollToLeftEnd();
        //                break;
        //            }

        //        case ScrollOrientation.Right:
        //            {
        //                this.scrollViewer.ScrollToRightEnd();
        //                break;
        //            }

        //        case ScrollOrientation.Top:
        //            {
        //                this.scrollViewer.ScrollToTop();
        //                break;
        //            }

        //        default:
        //            throw new NotImplementedException();
        //    }
        //}

        #endregion
    }
}

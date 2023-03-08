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
using XTerminal.Terminal;

namespace XTerminal.Document
{
    /// <summary>
    /// 用来显示字符的容器
    /// </summary>
    public class CharacterCanvas : FrameworkElement, IVTMonitor
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("WPFPresentationDevice");

        #endregion

        #region 实例变量

        private ScrollViewer scrollViewer;

        private VisualCollection visuals;

        private double fullWidth;
        private double fullHeight;

        #endregion

        #region 属性

        // Provide a required override for the VisualChildrenCount property.
        protected override int VisualChildrenCount
        {
            get { return this.visuals.Count; }
        }

        #endregion

        #region 构造方法

        public CharacterCanvas()
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

        #endregion

        #region IVTMonitor

        /// <summary>
        /// 渲染一行
        /// </summary>
        /// <param name="textLine"></param>
        public void DrawLine(VTextLine textLine)
        {
            DrawingLine drawingLine = textLine.DrawingObject as DrawingLine;
            if (drawingLine == null)
            {
                drawingLine = new DrawingLine();
                drawingLine.TextLine = textLine;
                textLine.DrawingObject = drawingLine;
                this.visuals.Add(drawingLine);
            }

            drawingLine.Draw();
        }

        public void DrawDocument(VTDocument document)
        {
            this.visuals.Clear();

            double width = 0, height = 0;

            VTextLine next = document.FirstLine;
            while (next != null)
            {
                DrawingLine drawingLine = next.DrawingObject as DrawingLine;
                this.visuals.Add(drawingLine);

                this.DrawLine(next);

                width = Math.Max(width, next.Bounds.RightBottom.X);
                height = Math.Max(height, next.Bounds.RightBottom.Y);

                next = next.NextLine;
            }

            Console.WriteLine("width = {0}, height = {1}", width, height);

            this.Resize(width, height);
            this.ScrollToEnd(ScrollOrientation.Bottom);
        }

        public VTextMetrics MeasureText(string text, VTextStyle style)
        {
            return TerminalUtils.UpdateTextMetrics(text, style);
        }

        public void Resize(double width, double height)
        {
            this.fullWidth = width;
            this.fullHeight = height;
            this.InvalidateMeasure();
        }

        public void ScrollToEnd(ScrollOrientation orientation)
        {
            if (!this.EnsureScrollViewer())
            {
                return;
            }

            switch (orientation)
            {
                case ScrollOrientation.Bottom:
                    {
                        this.scrollViewer.ScrollToEnd();
                        break;
                    }

                case ScrollOrientation.Left:
                    {
                        this.scrollViewer.ScrollToLeftEnd();
                        break;
                    }

                case ScrollOrientation.Right:
                    {
                        this.scrollViewer.ScrollToRightEnd();
                        break;
                    }

                case ScrollOrientation.Top:
                    {
                        this.scrollViewer.ScrollToTop();
                        break;
                    }

                default:
                    throw new NotImplementedException();
            }
        }

        #endregion
    }
}

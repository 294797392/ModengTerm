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
    public class DocumentRenderer : FrameworkElement, IDocumentRenderer
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("WPFPresentationDevice");

        #endregion

        #region 实例变量

        private ScrollViewer scrollViewer;

        private VisualCollection visuals;

        private double fullWidth;
        private double fullHeight;

        private DocumentRendererOptions options;

        private VTextMetrics blankCharacterMetrics;

        #endregion

        #region 属性

        // Provide a required override for the VisualChildrenCount property.
        protected override int VisualChildrenCount
        {
            get { return this.visuals.Count; }
        }

        #endregion

        #region 构造方法

        public DocumentRenderer()
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
        private DrawingLine RequestDrawingLine()
        {
            return this.visuals.Cast<DrawingLine>().FirstOrDefault(v => v.Data == null);
        }

        #endregion

        #region IDocumentRenderer

        public void Initialize(DocumentRendererOptions options)
        {
            this.options = options;

            this.blankCharacterMetrics = TerminalUtils.UpdateTextMetrics(" ", VTextStyle.Default);
            for (int i = 0; i < options.Rows; i++)
            {
                DrawingLine drawingLine = new DrawingLine() { Row = i };
                this.visuals.Add(drawingLine);
            }
        }

        public VTextMetrics MeasureText(string text, VTextStyle style)
        {
            return TerminalUtils.UpdateTextMetrics(text, style);
        }

        public void RenderDocument(VTDocument vtDocument)
        {
            ViewableDocument document = vtDocument.ViewableArea;

            // 当前行的Y方向偏移量
            double offsetY = 0;

            VTextLine next = document.FirstLine;

            while (next != null)
            {
                // 首先获取当前行的DrawingObject
                DrawingLine drawingLine = next.DrawingElement as DrawingLine;
                if (drawingLine == null)
                {
                    drawingLine = this.RequestDrawingLine();
                    if (drawingLine == null)
                    {
                        // 不应该发生
                        logger.FatalFormat("没有空闲的DrawingLine了");
                        return;
                    }
                    drawingLine.Data = next;
                    next.DrawingElement = drawingLine;
                }

                // 此时说明需要重新排版
                next.OffsetY = offsetY;

                if (next.IsCharacterDirty)
                {
                    // 此时说明该行有字符变化，需要重绘
                    // 重绘的时候会也会Arrange
                    if (drawingLine.Data != next)
                    {
                        drawingLine.Data = next;
                    }
                    drawingLine.Draw();
                    next.IsCharacterDirty = false;
                }
                else
                {
                    // 字符没有变化，那么只重新测量然后更新一下布局就好了
                    string text = next.BuildText();
                    next.Metrics = this.MeasureText(text, VTextStyle.Default);
                    drawingLine.Offset = new Vector(next.OffsetX, next.OffsetY);
                }

                // 更新下一个文本行的Y偏移量
                offsetY += next.Metrics.Height;

                // 如果最后一行渲染完毕了，那么就退出
                if (next == document.LastLine)
                {
                    break;
                }

                next = next.NextLine;
            }
        }

        public void RenderElement(IDrawingElement drawingObject)
        {
            DrawingObject drawingObject1 = drawingObject as DrawingObject;
            if (drawingObject1 == null)
            {
                logger.ErrorFormat("RenderElement失败, 要重绘的对象不存在");
                return;
            }

            this.Dispatcher.Invoke(() => 
            {
                drawingObject1.Draw();
            });
        }

        public void Reset()
        {
            IEnumerable<DrawingLine> drawingLines = this.visuals.Cast<DrawingLine>();

            foreach (DrawingLine drawingLine in drawingLines)
            {
                drawingLine.Reset();
            }

            this.Resize(0, 0);
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

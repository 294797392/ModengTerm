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
using XTerminalBase;
using XTerminalBase.IVideoTerminal;

namespace XTerminal.WPFRenderer
{
    public class WPFPresentaionDevice : FrameworkElement, IPresentationDevice
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

        public WPFPresentaionDevice()
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

        #region IPresentationDevice

        ///// <summary>
        ///// 渲染一行
        ///// </summary>
        ///// <param name="textLine"></param>
        //public void DrawLine(VTextLine textLine)
        //{
        //    VisualLine visualLine;
        //    if (!this.visualMap.TryGetValue(textLine.Row, out visualLine))
        //    {
        //        visualLine = new VisualLine();
        //        visualLine.TextLine = textLine;
        //        this.visualMap[textLine.Row] = visualLine;
        //        this.visuals.Add(visualLine);
        //    }

        //    visualLine.Draw();
        //}

        public void DrawText(List<VTextBlock> textBlocks)
        {
            foreach (VTextBlock textBlock in textBlocks)
            {
                this.DrawText(textBlock);
            }
        }

        public void DrawText(VTextBlock textBlock)
        {
            DrawingText textVisual = textBlock.DrawingObject as DrawingText;
            if (textVisual == null)
            {
                textVisual = new DrawingText(textBlock);
                this.visuals.Add(textVisual);

                textBlock.DrawingObject = textVisual;
            }

            textVisual.Draw();
        }

        public void DeleteText(List<VTextBlock> textBlocks)
        {
            foreach (VTextBlock textBlock in textBlocks)
            {
                this.DeleteText(textBlock);
            }
        }

        public void DeleteText(VTextBlock textBlock)
        {
            DrawingText textVisual = textBlock.DrawingObject as DrawingText;
            if(textVisual == null)
            {
                this.visuals.Remove(textVisual);
                textBlock.DrawingObject = null;
            }
        }

        public VTextBlockMetrics MeasureText(VTextBlock textBlock)
        {
            TerminalUtils.UpdateTextMetrics(textBlock);
            return textBlock.Metrics;
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

        public bool CanVerticallyScroll { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool CanHorizontallyScroll { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public double ExtentWidth => throw new NotImplementedException();

        public double ExtentHeight => throw new NotImplementedException();

        public double ViewportWidth => throw new NotImplementedException();

        public double ViewportHeight => throw new NotImplementedException();

        public double HorizontalOffset => throw new NotImplementedException();

        public double VerticalOffset => throw new NotImplementedException();

        public ScrollViewer ScrollOwner { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void LineDown()
        {
            throw new NotImplementedException();
        }

        public void LineLeft()
        {
            throw new NotImplementedException();
        }

        public void LineRight()
        {
            throw new NotImplementedException();
        }

        public void LineUp()
        {
            throw new NotImplementedException();
        }

        public Rect MakeVisible(Visual visual, Rect rectangle)
        {
            throw new NotImplementedException();
        }

        public void MouseWheelDown()
        {
            throw new NotImplementedException();
        }

        public void MouseWheelLeft()
        {
            throw new NotImplementedException();
        }

        public void MouseWheelRight()
        {
            throw new NotImplementedException();
        }

        public void MouseWheelUp()
        {
            throw new NotImplementedException();
        }

        public void PageDown()
        {
            throw new NotImplementedException();
        }

        public void PageLeft()
        {
            throw new NotImplementedException();
        }

        public void PageRight()
        {
            throw new NotImplementedException();
        }

        public void PageUp()
        {
            throw new NotImplementedException();
        }

        public void SetHorizontalOffset(double offset)
        {
            throw new NotImplementedException();
        }

        public void SetVerticalOffset(double offset)
        {
            throw new NotImplementedException();
        }
    }
}

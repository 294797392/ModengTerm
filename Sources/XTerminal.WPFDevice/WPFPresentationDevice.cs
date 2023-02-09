using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using XTerminalDevice;
using XTerminalDevice.Interface;

namespace XTerminal.WPFDevice
{
    public class WPFPresentationDevice : Panel, IPresentationDevice
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("WPFPresentationDevice");

        #endregion

        #region 实例变量

        private ScrollViewer scrollViewer;

        private Dictionary<int, TextVisual> textVisuals;

        private Typeface typeface;
        private double pixelPerDip;

        private double fullWidth;
        private double fullHeight;

        #endregion

        #region 属性

        // Provide a required override for the VisualChildrenCount property.
        protected override int VisualChildrenCount
        {
            get { return this.textVisuals.Count; }
        }

        #endregion

        #region 构造方法

        public WPFPresentationDevice()
        {
            this.textVisuals = new Dictionary<int, TextVisual>();
            this.typeface = new Typeface(new FontFamily("Ya Hei"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
            this.pixelPerDip = VisualTreeHelper.GetDpi(this).PixelsPerDip;
        }

        #endregion

        #region 重写方法

        // Provide a required override for the GetVisualChild method.
        protected override Visual GetVisualChild(int index)
        {
            return this.textVisuals[index];
        }

        protected override Size MeasureOverride(Size constraint)
        {
            Console.WriteLine("Measure");
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

        public void DrawText(List<VTextBlock> textBlocks)
        {
            foreach (VTextBlock textBlock in textBlocks)
            {
                this.DrawText(textBlock);
            }
        }

        public void DrawText(VTextBlock textBlock)
        {
            Dispatcher.Invoke(() =>
            {
                TextVisual textVisual;
                if (!this.textVisuals.TryGetValue(textBlock.Index, out textVisual))
                {
                    textVisual = new TextVisual(textBlock);
                    textVisual.PixelsPerDip = this.pixelPerDip;
                    textVisual.Typeface = this.typeface;

                    this.AddVisualChild(textVisual); // 可视对象的父子关系会影响到命中测试的结果

                    this.textVisuals[textBlock.Index] = textVisual;
                }

                textVisual.Draw();
            });
        }

        public VTextBlockMetrics MeasureText(VTextBlock textBlock)
        {
            FormattedText formattedText = TerminalUtils.CreateFormattedText(textBlock, this.typeface, this.pixelPerDip);
            TerminalUtils.UpdateTextMetrics(textBlock, formattedText);
            return textBlock.Metrics;
        }

        public void Resize(double width, double height)
        {
            this.fullWidth = width;
            this.fullHeight = height;
            Dispatcher.Invoke(() =>
            {
                this.InvalidateMeasure();
            });
        }

        public void ScrollToEnd(ScrollOrientation orientation)
        {
            Dispatcher.Invoke(() => 
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
            });
        }

        #endregion
    }
}

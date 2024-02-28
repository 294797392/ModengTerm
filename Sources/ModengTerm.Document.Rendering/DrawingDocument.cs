using ModengTerm.Document.Drawing;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace ModengTerm.Document.Rendering
{
    [TemplatePart(Name = "PART_DrawingSurface", Type = typeof(DrawingSurface))]
    [TemplatePart(Name = "PART_Scrollbar", Type = typeof(ScrollBar))]
    public class DrawingDocument : Control, IDrawingDocument
    {
        #region 实例变量

        private DrawingSurface surface;
        private ScrollBar scrollbar;
        private VTDocumentHandler eventHandler;

        #endregion

        #region 属性

        /// <summary>
        /// 用来渲染内容的区域
        /// </summary>
        public DrawingSurface ContentArea { get { return this.surface; } }

        public VTScrollbar Scrollbar { get; private set; }

        public double PaddingSize
        {
            get { return this.Padding.Left; }
            set 
            {
                this.Padding = new Thickness(value);
            }
        }

        public VTSize Size
        {
            get
            {
                return new VTSize(this.ActualWidth, this.ActualHeight);
            }
        }

        #endregion

        #region 构造方法

        public DrawingDocument()
        {
            this.Style = Application.Current.FindResource("StyleDrawingDocument") as Style;
        }

        #endregion

        #region IDrawingDocument

        public IDrawingObject CreateDrawingObject(DrawingObjectTypes types)
        {
            IDrawingObject drawingObject = DrawingObjectFactory.CreateDrawingObject(types);

            this.surface.AddVisual(drawingObject as DrawingObject);

            return drawingObject;
        }

        public void DeleteDrawingObject(IDrawingObject drawingObject)
        {
            drawingObject.Release();
            surface.RemoveVisual(drawingObject as DrawingObject);
        }

        public void DeleteDrawingObjects()
        {
            List<IDrawingObject> drawingObjects = surface.GetAllVisual().Cast<IDrawingObject>().ToList();

            foreach (IDrawingObject drawingObject in drawingObjects)
            {
                DeleteDrawingObject(drawingObject);
            }
        }

        public VTRect GetContentRect()
        {
            Point leftTop = surface.TranslatePoint(new Point(), this);
            return new VTRect(leftTop.X, leftTop.Y, surface.ActualWidth, surface.ActualHeight);
        }

        public void AddHandler(VTDocumentHandler eventHandler)
        {
            this.eventHandler = eventHandler;
        }

        public VTypeface GetTypeface(double fontSize, string fontFamily)
        {
            Typeface typeface = DrawingUtils.GetTypeface(fontFamily);
            FormattedText formattedText = new FormattedText(" ", CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeface,
                fontSize, Brushes.Black, null, TextFormattingMode.Display, DrawingUtils.PixelPerDpi);

            return new VTypeface()
            {
                FontSize = fontSize,
                FontFamily = fontFamily,
                Height = formattedText.Height,
                SpaceWidth = formattedText.WidthIncludingTrailingWhitespace
            };
        }

        #endregion

        #region 事件处理器

        private void Surface_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.eventHandler == null)
            {
                return;
            }

            FrameworkElement frameworkElement = sender as FrameworkElement;
            Point p = e.GetPosition(frameworkElement);
            frameworkElement.CaptureMouse();
            this.eventHandler.OnMouseDown(this, new VTPoint(p.X, p.Y), e.ClickCount);

            // 不设置为true的话会立即出发MosueLeftButtonUp事件，不知道为什么
            e.Handled = true;
        }

        private void Surface_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.eventHandler == null)
            {
                return;
            }

            FrameworkElement frameworkElement = sender as FrameworkElement;
            Point p = e.GetPosition(frameworkElement);
            this.eventHandler.OnMouseUp(this, new VTPoint(p.X, p.Y));
            frameworkElement.ReleaseMouseCapture();
        }

        private void Surface_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.eventHandler == null)
            {
                return;
            }

            FrameworkElement frameworkElement = sender as FrameworkElement;
            if (!frameworkElement.IsMouseCaptured)
            {
                return;
            }

            Point p = e.GetPosition(frameworkElement);
            this.eventHandler.OnMouseMove(this, new VTPoint(p.X, p.Y));
        }

        private void Surface_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.eventHandler == null)
            {
                return;
            }

            if (!e.WidthChanged && !e.HeightChanged)
            {
                return;
            }

            FrameworkElement frameworkElement = sender as FrameworkElement;
            this.eventHandler.OnSizeChanged(this, new VTSize(frameworkElement.ActualWidth, frameworkElement.ActualHeight));
        }

        private void Surface_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (this.eventHandler == null)
            {
                return;
            }

            this.eventHandler.OnMouseWheel(this, e.Delta > 0);
        }

        private void Scrollbar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (this.eventHandler == null)
            {
                return;
            }

            this.eventHandler.OnScrollChanged(this, (int)e.NewValue);
        }

        #endregion

        #region 重写方法

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.surface = base.Template.FindName("PART_DrawingSurface", this) as DrawingSurface;
            surface.MouseLeftButtonDown += Surface_MouseLeftButtonDown;
            surface.MouseLeftButtonUp += Surface_MouseLeftButtonUp;
            surface.MouseMove += Surface_MouseMove;
            surface.SizeChanged += Surface_SizeChanged;
            surface.MouseWheel += Surface_MouseWheel;

            this.scrollbar = base.Template.FindName("PART_Scrollbar", this) as ScrollBar;
            this.scrollbar.ValueChanged += Scrollbar_ValueChanged;

            this.Scrollbar = new VTScrollbarImpl(this.scrollbar);
        }

        #endregion
    }
}

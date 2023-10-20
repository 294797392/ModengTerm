using ModengTerm.Terminal.Document;
using ModengTerm.Terminal.Rendering;
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

namespace ModengTerm.Rendering
{
    [TemplatePart(Name = "PART_ContentArea", Type = typeof(ContentArea))]
    [TemplatePart(Name = "PART_Scrollbar", Type = typeof(DrawingScrollbar))]
    public class DrawingDocument : Control, IDrawingDocument
    {
        #region 实例变量

        private ContentArea contentArea;
        private DrawingScrollbar scrollbar;
        private VTEventHandler eventHandler;

        #endregion

        #region 属性

        /// <summary>
        /// 用来渲染内容的区域
        /// </summary>
        public ContentArea ContentArea { get { return this.contentArea; } }

        public IDrawingScrollbar Scrollbar { get { return this.scrollbar; } }

        public double ContentMargin
        {
            get { return (double)GetValue(ContentMarginProperty); }
            set { SetValue(ContentMarginProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DocumentPadding.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContentMarginProperty =
            DependencyProperty.Register("ContentMargin", typeof(double), typeof(DrawingDocument), new PropertyMetadata(0.0D));

        public bool ScrollbarVisible
        {
            get { return (bool)GetValue(ScrollbarVisibleProperty); }
            set { SetValue(ScrollbarVisibleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ScrollbarVisible.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ScrollbarVisibleProperty =
            DependencyProperty.Register("ScrollbarVisible", typeof(bool), typeof(DrawingDocument), new PropertyMetadata(false));

        #endregion

        #region 构造方法

        public DrawingDocument()
        {
            this.Style = Application.Current.FindResource("StyleDrawingDocument") as Style;
        }

        #endregion

        #region IDrawingDocument

        public TDrawingObject CreateDrawingObject<TDrawingObject>(VTDocumentElements type) where TDrawingObject : IDrawingObject
        {
            TDrawingObject drawingObject = DrawingObjectFactory.CreateDrawingObject<TDrawingObject>(type);
            contentArea.AddVisual(drawingObject as DrawingObject);
            return drawingObject;
        }

        public void DeleteDrawingObject(IDrawingObject drawingObject)
        {
            drawingObject.Release();
            contentArea.RemoveVisual(drawingObject as DrawingObject);
        }

        public void DeleteDrawingObjects()
        {
            List<IDrawingObject> drawingObjects = contentArea.GetAllVisual().Cast<IDrawingObject>().ToList();

            foreach (IDrawingObject drawingObject in drawingObjects)
            {
                DeleteDrawingObject(drawingObject);
            }
        }

        public VTRect GetContentRect()
        {
            Point leftTop = contentArea.TranslatePoint(new Point(), this);
            return new VTRect(leftTop.X, leftTop.Y, contentArea.ActualWidth, contentArea.ActualHeight);
        }

        public void AddEventHandler(VTEventHandler eventHandler)
        {
            this.eventHandler = eventHandler;
        }

        #endregion

        #region 事件处理器

        private void ContentArea_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.eventHandler == null)
            {
                return;
            }

            FrameworkElement frameworkElement = sender as FrameworkElement;
            Point p = e.GetPosition(frameworkElement);
            frameworkElement.CaptureMouse();
            this.eventHandler.OnMouseDown(new VTPoint(p.X, p.Y), e.ClickCount);
        }

        private void ContentArea_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.eventHandler == null)
            {
                return;
            }

            FrameworkElement frameworkElement = sender as FrameworkElement;
            Point p = e.GetPosition(frameworkElement);
            this.eventHandler.OnMouseUp(new VTPoint(p.X, p.Y));
            frameworkElement.ReleaseMouseCapture();
        }

        private void ContentArea_MouseMove(object sender, MouseEventArgs e)
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
            this.eventHandler.OnMouseMove(new VTPoint(p.X, p.Y));
        }

        private void ContentArea_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.eventHandler == null)
            {
                return;
            }

            FrameworkElement frameworkElement = sender as FrameworkElement;
            this.eventHandler.OnSizeChanged(new VTSize(frameworkElement.ActualWidth, frameworkElement.ActualHeight));
        }

        private void ContentArea_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (this.eventHandler == null)
            {
                return;
            }

            this.eventHandler.OnMouseWheel(e.Delta > 0);
        }

        private void Scrollbar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (this.eventHandler == null)
            {
                return;
            }

            this.eventHandler.OnScrollChanged((int)e.NewValue);
        }

        #endregion

        #region 重写方法

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.contentArea = Template.FindName("PART_ContentArea", this) as ContentArea;
            contentArea.MouseLeftButtonDown += ContentArea_MouseLeftButtonDown;
            contentArea.MouseLeftButtonUp += ContentArea_MouseLeftButtonUp;
            contentArea.MouseMove += ContentArea_MouseMove;
            contentArea.SizeChanged += ContentArea_SizeChanged;
            contentArea.MouseWheel += ContentArea_MouseWheel;

            this.scrollbar = Template.FindName("PART_Scrollbar", this) as DrawingScrollbar;
            this.scrollbar.ValueChanged += Scrollbar_ValueChanged;
        }

        #endregion
    }

    public class ContentPaddingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double padding = (double)value;
            return new Thickness(padding);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

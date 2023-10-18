using ModengTerm.Terminal.Document;
using ModengTerm.Terminal.Rendering;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace ModengTerm.Rendering
{
    [TemplatePart(Name = "PART_Document", Type = typeof(DrawingArea))]
    [TemplatePart(Name = "PART_Scrollbar", Type = typeof(DrawingArea))]
    public class DrawingDocument : Control, IDrawingDocument
    {
        #region 实例变量

        private DrawingArea documentArea;
        private DrawingArea scrollbarArea;

        #endregion

        #region 属性

        public double DocumentPadding
        {
            get { return (double)GetValue(DocumentPaddingProperty); }
            set { SetValue(DocumentPaddingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DocumentPadding.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DocumentPaddingProperty =
            DependencyProperty.Register("DocumentPadding", typeof(double), typeof(DrawingDocument), new PropertyMetadata(0.0D));

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
            if (type == VTDocumentElements.Scrollbar)
            {
                scrollbarArea.AddVisual(drawingObject as DrawingObject);
            }
            else
            {
                documentArea.AddVisual(drawingObject as DrawingObject);
            }
            return drawingObject;
        }

        public void DeleteDrawingObject(IDrawingObject drawingObject)
        {
            drawingObject.Release();
            documentArea.RemoveVisual(drawingObject as DrawingObject);
        }

        public void DeleteDrawingObjects()
        {
            List<IDrawingObject> drawingObjects = documentArea.GetAllVisual().Cast<IDrawingObject>().ToList();

            foreach (IDrawingObject drawingObject in drawingObjects)
            {
                DeleteDrawingObject(drawingObject);
            }
        }

        public VTRect GetDocumentRect()
        {
            Point leftTop = documentArea.TranslatePoint(new Point(), this);
            return new VTRect(leftTop.X, leftTop.Y, documentArea.ActualWidth, documentArea.ActualHeight);
        }

        #endregion

        #region 事件处理器

        #endregion

        #region 重写方法

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            documentArea = Template.FindName("PART_Document", this) as DrawingArea;
            scrollbarArea = Template.FindName("PART_Scrollbar", this) as DrawingArea;
        }

        #endregion
    }

    public class DocumentPaddingConverter : IValueConverter
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

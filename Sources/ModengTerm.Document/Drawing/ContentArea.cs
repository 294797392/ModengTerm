using ModengTerm.Terminal.Document;
using ModengTerm.Terminal.Rendering;
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

namespace ModengTerm.Document.Rendering
{
    /// <summary>
    /// 用来渲染终端输出的表面
    /// </summary>
    public class ContentArea : FrameworkElement
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("ContentArea");

        #endregion

        #region 实例变量

        private VisualCollection visuals;

        #endregion

        #region 属性

        public Brush Background
        {
            get { return (Brush)GetValue(BackgroundProperty); }
            set { SetValue(BackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Background.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BackgroundProperty =
            DependencyProperty.Register("Background", typeof(Brush), typeof(ContentArea), new PropertyMetadata(Brushes.Transparent));



        // Provide a required override for the VisualChildrenCount property.
        protected override int VisualChildrenCount
        {
            get { return visuals.Count; }
        }

        #endregion

        #region 构造方法

        public ContentArea()
        {
            visuals = new VisualCollection(this);
        }

        #endregion

        #region 重写方法

        // Provide a required override for the GetVisualChild method.
        protected override Visual GetVisualChild(int index)
        {
            return visuals[index];
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            drawingContext.DrawRectangle(Background, null, new Rect(0, 0, ActualWidth, ActualHeight));
        }

        #endregion

        public void AddVisual(DrawingObject drawingObject)
        {
            visuals.Add(drawingObject);
        }

        public void RemoveVisual(DrawingObject drawingObject)
        {
            visuals.Remove(drawingObject);
        }

        public List<Visual> GetAllVisual()
        {
            return visuals.Cast<Visual>().ToList();
        }

        public TVisual GetVisual<TVisual>() where TVisual : DrawingObject
        {
            return visuals.OfType<TVisual>().FirstOrDefault();
        }
    }
}

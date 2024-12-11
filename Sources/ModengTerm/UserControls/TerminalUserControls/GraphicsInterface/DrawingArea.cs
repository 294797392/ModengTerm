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

namespace ModengTerm.UserControls.TerminalUserControls.Rendering
{
    /// <summary>
    /// 渲染文档的区域
    /// </summary>
    public class DrawingArea : FrameworkElement
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
            DependencyProperty.Register("Background", typeof(Brush), typeof(DrawingArea), new PropertyMetadata(Brushes.Transparent));



        // Provide a required override for the VisualChildrenCount property.
        protected override int VisualChildrenCount
        {
            get { return this.visuals.Count; }
        }

        #endregion

        #region 构造方法

        public DrawingArea()
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

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            drawingContext.DrawRectangle(this.Background, null, new Rect(0, 0, this.ActualWidth, this.ActualHeight));
        }

        #endregion

        public void AddVisual(DrawingObject drawingObject)
        {
            this.visuals.Add(drawingObject);
        }

        public void RemoveVisual(DrawingObject drawingObject)
        {
            this.visuals.Remove(drawingObject);
        }

        public List<Visual> GetAllVisual()
        {
            return this.visuals.Cast<Visual>().ToList();
        }

        public TVisual GetVisual<TVisual>() where TVisual : DrawingObject
        {
            return this.visuals.OfType<TVisual>().FirstOrDefault();
        }
    }
}

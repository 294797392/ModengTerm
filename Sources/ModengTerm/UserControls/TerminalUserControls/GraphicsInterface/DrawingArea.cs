using ModengTerm.Addon.Interactive;
using ModengTerm.Document.Graphics;
using ModengTerm.ViewModel.Terminal;
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

namespace ModengTerm.UserControls.TerminalUserControls
{
    /// <summary>
    /// 渲染文档的区域
    /// </summary>
    public class DrawingArea : FrameworkElement, IDrawingContext
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("ContentArea");

        #endregion

        #region 实例变量

        private VisualCollection visuals;

        #endregion

        #region 属性

        /// <summary>
        /// 这个属性的意义在于可以在调试的时候设置一个背景色，看到DrawArea的位置和大小
        /// </summary>
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

        public VisualCollection Visuals { get { return this.visuals; } }

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

        protected override void OnRender(System.Windows.Media.DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            drawingContext.DrawRectangle(this.Background, null, new Rect(0, 0, this.ActualWidth, this.ActualHeight));
        }

        #endregion

        #region IDrawingContext

        /// <summary>
        /// 创建一个绘图对象
        /// </summary>
        /// <returns></returns>
        public GraphicsObject CreateGraphicsObject()
        {
            DrawingObject drawingObject = new DrawingObject();
            this.visuals.Add(drawingObject);
            return drawingObject;
        }

        /// <summary>
        /// 删除一个绘图对象
        /// </summary>
        /// <param name="drawingObject"></param>
        public void DeleteGraphicsObject(GraphicsObject graphicsObject)
        {
            this.visuals.Remove(graphicsObject as DrawingObject);
        }

        #endregion
    }
}

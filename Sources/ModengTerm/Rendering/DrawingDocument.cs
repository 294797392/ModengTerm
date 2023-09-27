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
using XTerminal.Document;
using XTerminal.Document.Rendering;

namespace ModengTerm.Rendering
{
    /// <summary>
    /// 用来渲染终端输出的表面
    /// </summary>
    public class DrawingDocument : FrameworkElement, IDrawingDocument
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("DrawingDocument");

        #endregion

        #region 实例变量

        private VisualCollection visuals;

        #endregion

        #region 属性

        // Provide a required override for the VisualChildrenCount property.
        protected override int VisualChildrenCount
        {
            get { return this.visuals.Count; }
        }

        #endregion

        #region 构造方法

        public DrawingDocument()
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

            // 调试用，看这个Surface有多大
            //drawingContext.DrawRectangle(Brushes.Red, new Pen(Brushes.Black, 1), new Rect(0, 0, this.ActualWidth, this.ActualHeight));
        }

        #endregion

        #region 实例方法

        private IDrawingObject EnsureDrawingObject(VTDocumentElement documentElement)
        {
            IDrawingObject drawingObject = documentElement.DrawingObject as IDrawingObject;
            if (drawingObject == null)
            {
                drawingObject = DrawingObjectFactory.CreateDrawingObject(documentElement);
                drawingObject.Initialize(documentElement);
                documentElement.DrawingObject = drawingObject;
                this.visuals.Add(drawingObject as DrawingVisual);
            }
            return drawingObject;
        }

        #endregion

        #region IDrawingDocument

        public IDrawingObject CreateDrawingObject(VTDocumentElement documentElement)
        {
            return this.EnsureDrawingObject(documentElement);
        }

        public void DeleteDrawingObject(IDrawingObject drawingObject)
        {
            drawingObject.Release();
            this.visuals.Remove(drawingObject as DrawingObject);
        }

        public void DeleteDrawingObjects()
        {
            List<DrawingObject> drawingObjects = this.visuals.Cast<DrawingObject>().ToList();

            foreach (DrawingObject drawingObject in drawingObjects)
            {
                this.DeleteDrawingObject(drawingObject);
            }
        }

        #endregion

        #region 事件处理器


        #endregion
    }
}

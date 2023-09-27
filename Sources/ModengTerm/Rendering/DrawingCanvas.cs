using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using XTerminal.Document;
using XTerminal.Document.Rendering;

namespace ModengTerm.Rendering
{
    /// <summary>
    /// 可以画任意图形的Canvas
    /// </summary>
    public class DrawingCanvas : DrawingDocument
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("DrawingDocument");

        #endregion

        #region 实例变量

        private VisualCollection visuals;

        private RenderingOptions options;

        #endregion

        #region 属性

        // Provide a required override for the VisualChildrenCount property.
        protected override int VisualChildrenCount
        {
            get { return this.visuals.Count; }
        }

        #endregion

        #region 构造方法

        public DrawingCanvas()
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

        private DrawingObject EnsureDrawingObject(VTDocumentElement documentElement)
        {
            DrawingObject drawingObject = documentElement.DrawingObject as DrawingObject;
            if (drawingObject == null)
            {
                drawingObject = DrawingObjectFactory.CreateDrawingObject(documentElement.Type);
                drawingObject.Initialize(documentElement);
                documentElement.DrawingObject = drawingObject;
                this.visuals.Add(drawingObject);
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
            foreach (DrawingObject drawingObject in this.visuals.Cast<DrawingObject>())
            {
                drawingObject.Release();
            }
            this.visuals.Clear();
        }

        #endregion

        #region 事件处理器


        #endregion
    }
}

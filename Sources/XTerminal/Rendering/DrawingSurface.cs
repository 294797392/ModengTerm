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
using VideoTerminal.Rendering;
using XTerminal.Document;
using XTerminal.Document.Rendering;

namespace XTerminal.Rendering
{
    /// <summary>
    /// 用来渲染终端输出的表面
    /// </summary>
    public class DrawingSurface : FrameworkElement, ITerminalSurface
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("DocumentRenderer");

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

        public DrawingSurface()
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

        #endregion

        #region 实例方法

        private DrawingObject CreateDrawingObject(VTDocumentElements type)
        {
            switch (type)
            {
                case VTDocumentElements.Cursor: return new DrawingCursor();
                case VTDocumentElements.SelectionRange: return new DrawingSelection();
                case VTDocumentElements.TextLine: return new DrawingLine();
                default:
                    throw new NotImplementedException();
            }
        }

        private DrawingObject EnsureDrawingObject(VTDocumentElement documentElement)
        {
            DrawingObject drawingObject = documentElement.DrawingContext as DrawingObject;
            if (drawingObject == null)
            {
                drawingObject = this.CreateDrawingObject(documentElement.Type);
                drawingObject.Initialize(documentElement);
                documentElement.DrawingContext = drawingObject;
                this.visuals.Add(drawingObject);
            }
            return drawingObject;
        }

        #endregion

        #region ITerminalSurface

        public void Delete(VTDocumentElement drawable)
        {
            DrawingObject drawingObject = drawable.DrawingContext as DrawingObject;
            if(drawingObject == null)
            {
                return;
            }

            this.visuals.Remove(drawingObject);
            drawingObject.Release();
            drawable.DrawingContext = null;
        }

        public void Draw(VTDocumentElement drawable)
        {
            DrawingObject drawingObject = this.EnsureDrawingObject(drawable);
            drawingObject.Draw();
        }

        public void Arrange(VTDocumentElement drawable)
        {
            DrawingObject drawingObject = this.EnsureDrawingObject(drawable);
            drawingObject.Offset = new Vector(drawable.OffsetX, drawable.OffsetY);
        }

        public void SetOpacity(VTDocumentElement drawable, double opacity)
        {
            DrawingObject drawingObject = this.EnsureDrawingObject(drawable);
            drawingObject.Opacity = opacity;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            // 调试用，看这个Surface有多大
            //drawingContext.DrawRectangle(Brushes.Red, new Pen(Brushes.Black, 1), new Rect(0, 0, this.ActualWidth, this.ActualHeight));
        }

        #endregion
    }
}

using ModengTerm.Document.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Document.Geometry
{
    public class VTRectangleGeometry
    {
        public double X { get; set; }

        public double Y { get; set; }

        public double Width { get; set; }

        public double Height { get; set; }

        /// <summary>
        /// 背景颜色
        /// </summary>
        public VTColor BackColor { get; set; }
    }

    public class VTRectangle : VTGeometry
    {
        #region 实例变量

        private List<VTRectangleGeometry> rectangles;

        #endregion

        #region 属性

        public override DrawingObjectTypes Type => DrawingObjectTypes.Rectangle;

        #endregion

        #region 构造方法

        public VTRectangle(VTDocument ownerDocument) :
            base(ownerDocument)
        {
        }

        #endregion

        #region VTGeometry

        public void AddGeometry(VTRectangleGeometry rectangleGeometry)
        {
            this.rectangles.Add(rectangleGeometry);
            this.SetDirtyFlags(VTDirtyFlags.RenderDirty, true);
        }

        public void Clear() 
        {
            this.rectangles.Clear();
            this.SetDirtyFlags(VTDirtyFlags.RenderDirty, true);
        }

        #endregion

        #region VTElement

        protected override void OnInitialize(IDrawingObject drawingObject)
        {
            this.rectangles = new List<VTRectangleGeometry>();
            IDrawingRectangle drawingRectangle = drawingObject as IDrawingRectangle;
            drawingRectangle.Rectangles = this.rectangles;
        }

        protected override void OnRelease()
        {
            this.rectangles.Clear();
        }

        protected override void OnRender()
        {
            IDrawingRectangle drawingRectangle = this.DrawingObject as IDrawingRectangle;

            drawingRectangle.Draw();
        }

        #endregion
    }
}

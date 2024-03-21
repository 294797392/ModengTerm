using ModengTerm.Document.Drawing;
using ModengTerm.Document.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ModengTerm.Document.Rendering
{
    public class DrawingRectangle : DrawingObject, IDrawingRectangle
    {
        #region 实例变量

        #endregion

        #region IDrawingHighlight

        public List<VTRectangleGeometry> Rectangles { get; set; }

        #endregion

        protected override void OnInitialize()
        {
        }

        protected override void OnRelease()
        {
        }

        protected override void OnDraw(DrawingContext dc)
        {
            foreach (VTRectangleGeometry rectangle in this.Rectangles)
            {
                Brush brush = DrawingUtils.GetBrush(rectangle.BackColor);
                dc.DrawRectangle(brush, null, new System.Windows.Rect(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height));
            }
        }
    }
}

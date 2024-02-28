using ModengTerm.Terminal.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace ModengTerm.Document.Rendering
{
    public class DrawingBookmark : DrawingObject, IDrawingBookmark
    {
        private static PathFigureCollection BookmarkPathFigures = Application.Current.FindResource("SVGIcon4") as PathFigureCollection;

        private DrawingBrush drawingBrush;

        public string Color { get; set; }

        public double Width { get; set; }

        public double Height { get; set; }

        public double OffsetX { get; set; }

        public double OffsetY { get; set; }

        protected override void OnInitialize()
        {
            PathGeometry pathGeometry = new PathGeometry();
            pathGeometry.Figures = BookmarkPathFigures;

            GeometryDrawing geometryDrawing = new GeometryDrawing();
            geometryDrawing.Geometry = pathGeometry;
            geometryDrawing.Brush = DrawingUtils.GetBrush(this.Color);

            DrawingBrush drawingBrush = new DrawingBrush();
            drawingBrush.Drawing = geometryDrawing;

            this.drawingBrush = drawingBrush;
        }

        protected override void OnRelease()
        {
        }

        protected override void OnDraw(DrawingContext dc)
        {
            dc.DrawRectangle(this.drawingBrush, null, new Rect(this.OffsetX, this.OffsetY, this.Width, this.Height));
        }
    }
}

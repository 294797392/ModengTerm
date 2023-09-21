using ModengTerm.Terminal.Document.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using XTerminal.Document;

namespace ModengTerm.Rendering
{
    public class DrawingRectangle : DrawingObject
    {
        private Pen pen;
        private Brush backBrush;

        private VTDocumentRectangle rectangle;

        protected override void OnInitialize(VTDocumentElement documentElement)
        {
            this.rectangle = documentElement as VTDocumentRectangle;

            Brush penBrush = DrawingUtils.GetBrush(this.rectangle.BorderColor);
            this.pen = new Pen(penBrush, this.rectangle.BorderWidth);
            this.backBrush = DrawingUtils.GetBrush(this.rectangle.Background);
        }

        protected override void OnDraw(DrawingContext dc)
        {
            dc.DrawRectangle(this.backBrush, this.pen, new Rect(0, 0, this.rectangle.Width, this.rectangle.Height));
        }
    }
}

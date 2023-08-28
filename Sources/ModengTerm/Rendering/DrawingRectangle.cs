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
        private static readonly Pen Pen = new Pen(Brushes.Black, 1);

        private VTDocumentRectangle rectangle;

        protected override void OnInitialize(VTDocumentElement documentElement)
        {
            this.rectangle = documentElement as VTDocumentRectangle;
        }

        protected override void Draw(DrawingContext dc)
        {
            dc.DrawRectangle(Brushes.Transparent, Pen, new Rect(0, 0, this.rectangle.Width, this.rectangle.Height));
        }
    }
}

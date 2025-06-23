using ModengTerm.Document;
using ModengTerm.Document.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ModengTerm.UnitTest.Drawing
{
    public class FakeGI : GraphicsFactory
    {
        private GraphicsScrollbar ScrollbarGraphics;

        public VTSize TerminalSize 
        {
            get;set;
        }

        public FakeGI()
        {
            ScrollbarGraphics = new FakeScrollbar();
            this.TerminalSize = new VTSize(1000, 1000);
        }

        public GraphicsObject CreateDrawingObject()
        {
            return new FakeGIObject();
        }

        public void DeleteDrawingObject(GraphicsObject drawingObject)
        {
        }

        public void DeleteDrawingObjects()
        {
        }

        public VTypeface GetTypeface(double fontSize, string fontFamily)
        {
            return new VTypeface() { Height = 10, Width = 20 };
        }

        public GraphicsScrollbar GetScrollbarDrawingObject()
        {
            return this.ScrollbarGraphics;
        }
    }
}

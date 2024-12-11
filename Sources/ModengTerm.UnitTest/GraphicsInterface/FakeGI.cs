using ModengTerm.Document;
using ModengTerm.Document.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.UnitTest.Drawing
{
    public class FakeGI : GraphicsInterface
    {
        public string Name { get; set; }

        public VTScrollbar Scrollbar { get; private set; }

        public bool Visible { get; set; }

        public VTSize DrawAreaSize { get; set; }

        public FakeGI()
        {
            Scrollbar = new FakeScrollbar();
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

        public void SetPadding(double padding)
        {
        }
    }
}

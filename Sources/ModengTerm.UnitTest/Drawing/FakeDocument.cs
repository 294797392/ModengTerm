using ModengTerm.Document;
using ModengTerm.Document.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.UnitTest.Drawing
{
    public class FakeDocument : IDocument
    {
        public string Name { get; set; }

        public VTScrollbar Scrollbar { get; private set; }

        public bool Visible { get; set; }

        public VTSize DrawAreaSize { get; set; }

        public FakeDocument()
        {
            Scrollbar = new FakeScrollbar();
        }

        public IDocumentObject CreateDrawingObject()
        {
            return new FakeDocumentObject();
        }

        public void DeleteDrawingObject(IDocumentObject drawingObject)
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

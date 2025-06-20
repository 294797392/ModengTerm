using ModengTerm.Document;
using ModengTerm.Document.Drawing;
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
        public string Name { get; set; }

        public GraphicsScrollbar ScrollbarGraphics { get; private set; }

        public bool Visible { get; set; }

        public VTSize TerminalSize 
        {
            get;set;
        }

        public bool GIMouseCaptured => false;

        public VTModifierKeys PressedModifierKey => VTModifierKeys.None;

        public FakeGI()
        {
            ScrollbarGraphics = new FakeScrollbar();
            this.TerminalSize = new VTSize(1000, 1000);
        }

        public event Action<GraphicsFactory, MouseData> GIMouseDown;
        public event Action<GraphicsFactory, MouseData> GIMouseUp;
        public event Action<GraphicsFactory, MouseData> GIMouseMove;
        public event Action<GraphicsFactory, MouseWheelData> GIMouseWheel;
        public event Action<GraphicsFactory> GILoaded;
        public event Action<GraphicsFactory, ScrollChangedData> GIScrollChanged;

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

        public bool GICaptureMouse()
        {
            return false;
        }

        public void GIRleaseMouseCapture()
        {
        }

        public void RaiseMouseDown(MouseData mouseData) 
        {
            this.GIMouseDown?.Invoke(this, mouseData);
        }
    }
}

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
    public class FakeGI : GraphicsInterface
    {
        public string Name { get; set; }

        public VTScrollbar Scrollbar { get; private set; }

        public bool Visible { get; set; }

        public VTSize DrawAreaSize 
        {
            get;set;
        }

        public bool GIMouseCaptured => false;

        public VTModifierKeys PressedModifierKey => VTModifierKeys.None;

        public FakeGI()
        {
            Scrollbar = new FakeScrollbar();
            this.DrawAreaSize = new VTSize(1000, 1000);
        }

        public event Action<GraphicsInterface, MouseData> GIMouseDown;
        public event Action<GraphicsInterface, MouseData> GIMouseUp;
        public event Action<GraphicsInterface, MouseData> GIMouseMove;
        public event Action<GraphicsInterface, MouseWheelData> GIMouseWheel;
        public event Action<GraphicsInterface> GILoaded;
        public event Action<GraphicsInterface, ScrollChangedData> GIScrollChanged;

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

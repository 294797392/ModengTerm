using ModengTerm.Terminal.Document;
using ModengTerm.Terminal.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal
{
    public delegate void MouseDownHandler(IDrawingWindow vt, VTPoint location, int clickCount);
    public delegate void MouseMoveHandler(IDrawingWindow vt, VTPoint location);
    public delegate void MouseUpHandler(IDrawingWindow vt, VTPoint location);
    public delegate void MouseWheelHandler(IDrawingWindow vt, bool upper);
    public delegate void SizeChangedHandler(IDrawingWindow vt, VTRect vtRect);
}

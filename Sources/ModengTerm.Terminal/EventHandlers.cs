using ModengTerm.Terminal.Document;
using ModengTerm.Terminal.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal
{
    public delegate void MouseDownHandler(IDrawingTerminal vt, VTPoint location, int clickCount);
    public delegate void MouseMoveHandler(IDrawingTerminal vt, VTPoint location);
    public delegate void MouseUpHandler(IDrawingTerminal vt, VTPoint location);
    public delegate void MouseWheelHandler(IDrawingTerminal vt, bool upper);
    public delegate void SizeChangedHandler(IDrawingTerminal vt, VTRect vtRect);
}

using ModengTerm.Terminal.Document;
using ModengTerm.Terminal.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal
{
    public delegate void MouseDownHandler(IDrawingVideoTerminal vt, VTPoint location, int clickCount);
    public delegate void MouseMoveHandler(IDrawingVideoTerminal vt, VTPoint location);
    public delegate void MouseUpHandler(IDrawingVideoTerminal vt, VTPoint location);
    public delegate void MouseWheelHandler(IDrawingVideoTerminal vt, bool upper);
    public delegate void SizeChangedHandler(IDrawingVideoTerminal vt, VTRect vtRect);
}

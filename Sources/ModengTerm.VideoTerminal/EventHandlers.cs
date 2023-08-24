using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTerminal.Document;
using XTerminal.Document.Rendering;

namespace ModengTerm.VideoTerminal
{
    public delegate void MouseDownHandler(IVideoTerminal vt, VTPoint location, int clickCount);
    public delegate void MouseMoveHandler(IVideoTerminal vt, VTPoint location);
    public delegate void MouseUpHandler(IVideoTerminal vt, VTPoint location);
    public delegate void MouseWheelHandler(IVideoTerminal vt, bool upper);
    public delegate void SizeChangedHandler(IVideoTerminal vt, VTRect vtRect);
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTerminal.Document;
using XTerminal.Document.Rendering;

namespace ModengTerm.ViewModels
{
    public partial class VideoTerminal
    {
        public void OnDrawingMouseDown(IVideoTerminal vt, VTPoint location, int clickCount)
        { }

        public void OnDrawingMouseMove(IVideoTerminal vt, VTPoint location)
        { }

        public void OnDrawingMouseUp(IVideoTerminal vt, VTPoint location)
        { }

        public void OnDrawingMouseWheel(IVideoTerminal vt, bool upper)
        { }

        public void OnDrawingSizeChanged(IVideoTerminal vt, VTRect vtRect)
        { }
    }
}

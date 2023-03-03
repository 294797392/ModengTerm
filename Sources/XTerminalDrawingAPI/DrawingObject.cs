using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XTerminal.Drawing
{
    public abstract class DrawingObject
    {
        public abstract void DrawLine(IDrawingCanvas canvas);
    }
}

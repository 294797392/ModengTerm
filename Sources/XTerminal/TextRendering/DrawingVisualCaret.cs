using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace VideoTerminal.TextRendering
{
    public class DrawingVisualCaret : DrawingVisual
    {
        private Pen caretPen;
        private bool blinkState;

        public DrawingVisualCaret()
        {
            this.caretPen = new Pen(Brushes.Black, 2);
        }

        public void Render()
        {
            using (DrawingContext dc = base.RenderOpen())
            {
                if (this.blinkState)
                {
                    dc.DrawLine(this.caretPen, new Point(10, 10), new Point(10, 20));
                }
            }

            this.blinkState = !this.blinkState;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace XTerminal.Controls
{
    public class TerminalCaret : TerminalVisual
    {
        #region 实例变量

        private Pen caretPen;
        private bool blinkState;

        #endregion

        public TerminalCaret()
        {
            this.caretPen = new Pen(Brushes.Black, 2);
        }

        public void Render()
        {
            using (DrawingContext dc = base.RenderOpen())
            {
                if (this.blinkState)
                {
                    //dc.DrawText()
                    dc.DrawLine(this.caretPen, new Point(10, 10), new Point(10, 20));
                }
            }

            this.blinkState = !this.blinkState;
        }
    }
}

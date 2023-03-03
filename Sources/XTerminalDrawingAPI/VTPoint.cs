using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XTerminal.Drawing
{
    public struct VTPoint
    {
        public double X { get; set; }

        public double Y { get; set; }

        public VTPoint(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }
    }
}

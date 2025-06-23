using ModengTerm.Document.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.UnitTest.Drawing
{
    public class FakeScrollbar : GraphicsScrollbar
    {
        public bool Visible { get; set; }
        public double Maximum { get; set; }
        public double Value { get; set; }
        public int ViewportRow { get; set; }
    }
}

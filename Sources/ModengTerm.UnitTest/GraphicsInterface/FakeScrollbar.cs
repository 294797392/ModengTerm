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
        public override bool Visible { get; set; }
        public override double Maximum { get; set; }
        public override double Value { get; set; }
        public override int ViewportRow { get; set; }
    }
}

using ModengTerm.Terminal;
using ModengTerm.Terminal.Document;
using ModengTerm.Terminal.Document.Graphics;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using XTerminal;
using XTerminal.Document;

namespace ModengTerm.Rendering
{
    public class DrawingMatchesLine : DrawingObject
    {
        private VTMatchesLine matchesLine;

        protected override void OnInitialize()
        {
            this.matchesLine = this.documentElement as VTMatchesLine;
        }

        protected override void OnRelease()
        {
        }

        protected override void OnDraw(DrawingContext dc)
        {
            foreach (VTFormattedText textBlock in this.matchesLine.TextBlocks)
            {
                FormattedText formattedText = DrawingUtils.CreateFormattedText(textBlock, dc);

                dc.DrawText(formattedText, new Point(textBlock.OffsetX, 0));
            }
        }
    }
}

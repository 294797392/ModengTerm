using ModengTerm.Terminal;
using ModengTerm.Terminal.Document;
using ModengTerm.Terminal.Rendering;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using XTerminal;

namespace ModengTerm.Rendering
{
    public class DrawingMatchesLine : DrawingObject, IDrawingMatchesLine
    {
        public List<VTFormattedText> TextBlocks { get; set; }

        protected override void OnInitialize()
        {
        }

        protected override void OnRelease()
        {
        }

        protected override void OnDraw(DrawingContext dc)
        {
            foreach (VTFormattedText textBlock in this.TextBlocks)
            {
                FormattedText formattedText = DrawingUtils.CreateFormattedText(textBlock, dc);

                dc.DrawText(formattedText, new Point(textBlock.OffsetX, 0));
            }
        }
    }
}

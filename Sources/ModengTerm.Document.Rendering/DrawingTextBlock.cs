using ModengTerm.Document.Drawing;
using ModengTerm.Terminal;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace ModengTerm.Document.Rendering
{
    public class DrawingTextBlock : DrawingObject, IDrawingTextBlock
    {
        public VTFormattedText FormattedText { get; set; }

        protected override void OnInitialize()
        {
        }

        protected override void OnRelease()
        {
        }

        protected override void OnDraw(DrawingContext dc)
        {
            FormattedText formattedText = DrawingUtils.CreateFormattedText(this.FormattedText, dc);

            dc.DrawText(formattedText, new Point(this.FormattedText.OffsetX, 0));
        }
    }
}

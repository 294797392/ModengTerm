using ModengTerm.Document;
using ModengTerm.Document.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.UnitTest.Drawing
{
    public class FakeDocumentObject : IDocumentObject
    {
        public void Arrange(double x, double y)
        {
        }

        public void Clear()
        {
        }

        public void DrawRectangle(VTRect vtRect, VTPen vtPen, VTColor backColor)
        {
        }

        public void DrawRectangles(List<VTRect> vtRects, VTPen vtPen, VTColor vtColor)
        {
        }

        public VTextMetrics DrawText(VTFormattedText vtFormattedText)
        {
            return new VTextMetrics();
        }

        public VTextRange MeasureText(VTFormattedText vtFormattedText, int startIndex, int count)
        {
            return new VTextRange();
        }

        public void SetOpacity(double opacity)
        {
        }
    }
}

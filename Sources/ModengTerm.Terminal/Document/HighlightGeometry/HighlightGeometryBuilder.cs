using ModengTerm.Terminal.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal.HighlightGeometry
{
    public abstract class HighlightGeometryBuilder
    {
        public abstract List<VTRect> BuildHighlightGeometry(VTDocument document, VTextLine firstLine);
    }
}

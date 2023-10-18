using ModengTerm.Terminal.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal.Rendering
{
    public interface IDrawingMatchesLine : IDrawingObject
    {
        List<VTFormattedText> TextBlocks { get; set; }
    }
}

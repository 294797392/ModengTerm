using ModengTerm.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Document.Drawing
{
    public interface IDrawingTextBlock : IDrawingObject
    {
        VTFormattedText FormattedText { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTerminal.Document;

namespace ModengTerm.Rendering
{
    /// <summary>
    /// 提供创建绘图元素的静态类
    /// </summary>
    public static class DrawingObjectFactory
    {
        public static DrawingObject CreateDrawingObject(VTDocumentElements type)
        {
            switch (type)
            {
                case VTDocumentElements.Cursor: return new DrawingCursor();
                case VTDocumentElements.SelectionRange: return new DrawingSelection();
                case VTDocumentElements.TextLine: return new DrawingLine();
                case VTDocumentElements.Rectangle: return new DrawingRectangle();
                case VTDocumentElements.MatchesLine: return new DrawingMatchesLine();
                default:
                    throw new NotImplementedException();
            }
        }
    }
}

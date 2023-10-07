using ModengTerm.Rendering.Wallpaper;
using ModengTerm.Terminal.Document;
using ModengTerm.Terminal.Enumerations;
using ModengTerm.Terminal.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Rendering
{
    /// <summary>
    /// 提供创建绘图元素的静态类
    /// </summary>
    public static class DrawingObjectFactory
    {
        public static IDrawingObject CreateDrawingObject(VTDocumentElement documentElement)
        {
            switch (documentElement.Type)
            {
                case VTDocumentElements.Cursor: return new DrawingCursor();
                case VTDocumentElements.SelectionRange: return new DrawingSelection();
                case VTDocumentElements.TextLine: return new DrawingLine();
                case VTDocumentElements.Rectangle: return new DrawingRectangle();
                case VTDocumentElements.MatchesLine: return new DrawingMatchesLine();
                case VTDocumentElements.Wallpaper: return new DrawingWallpaper();

                default:
                    throw new NotImplementedException();
            }
        }
    }
}

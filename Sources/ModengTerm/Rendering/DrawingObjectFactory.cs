using ModengTerm.Rendering.Background;
using ModengTerm.Rendering.Wallpaper;
using ModengTerm.Terminal.Document;
using ModengTerm.Terminal.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTerminal.Document;
using XTerminal.Document.Rendering;

namespace ModengTerm.Rendering
{
    /// <summary>
    /// 提供创建绘图元素的静态类
    /// </summary>
    public static class DrawingObjectFactory
    {
        private static IDrawingObject CreateWallpaperDrawingObject(VTDocumentElement documentElement)
        {
            VTWallpaper wallpaper = documentElement as VTWallpaper;

            switch (wallpaper.PaperType)
            {
                case WallpaperTypeEnum.Image: return new DrawingImagedWallpaper();
                case WallpaperTypeEnum.Color: return new DrawingColoredWallpaper();
                case WallpaperTypeEnum.Live: return new DrawingWallpaperLive();
                default:
                    throw new NotImplementedException();
            }
        }

        public static IDrawingObject CreateDrawingObject(VTDocumentElement documentElement)
        {
            switch (documentElement.Type)
            {
                case VTDocumentElements.Cursor: return new DrawingCursor();
                case VTDocumentElements.SelectionRange: return new DrawingSelection();
                case VTDocumentElements.TextLine: return new DrawingLine();
                case VTDocumentElements.Rectangle: return new DrawingRectangle();
                case VTDocumentElements.MatchesLine: return new DrawingMatchesLine();
                case VTDocumentElements.Wallpaper: return DrawingObjectFactory.CreateWallpaperDrawingObject(documentElement);

                default:
                    throw new NotImplementedException();
            }
        }
    }
}

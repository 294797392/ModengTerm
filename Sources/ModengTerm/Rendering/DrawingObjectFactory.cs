using ModengTerm.Rendering.Background;
using ModengTerm.Terminal.Document;
using ModengTerm.Terminal.Enumerations;
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
        private static DrawingObject CreateWallpaperDrawingObject(VTDocumentElement documentElement)
        {
            VTWallpaper wallpaper = documentElement as VTWallpaper;

            switch ((WallpaperTypeEnum)wallpaper.Wallpaper.Type)
            {
                case WallpaperTypeEnum.PureColor: return new DrawingWallpaperPureColor();
                case WallpaperTypeEnum.Live: return new DrawingWallpaperLive();
                default:
                    throw new NotImplementedException();
            }
        }

        public static DrawingObject CreateDrawingObject(VTDocumentElement documentElement)
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

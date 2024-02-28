using ModengTerm.Document.Drawing;
using ModengTerm.Rendering.Wallpaper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Document.Rendering
{
    /// <summary>
    /// 提供创建绘图元素的静态类
    /// </summary>
    public static class DrawingObjectFactory
    {
        public static IDrawingObject CreateDrawingObject(DrawingObjectTypes type)
        {
            switch (type)
            {
                case DrawingObjectTypes.Cursor: return new DrawingCursor();
                case DrawingObjectTypes.Selection: return new DrawingSelection();
                case DrawingObjectTypes.TextLine: return new DrawingLine();
                case DrawingObjectTypes.TextBlock: return new DrawingTextBlock();
                case DrawingObjectTypes.Wallpaper: return new DrawingWallpaper();

                default:
                    throw new NotImplementedException(string.Format("{0}", type));
            }
        }
    }
}

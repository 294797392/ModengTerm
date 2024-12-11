using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal.Rendering
{
    public interface IDrawingBookmark : IDrawingObject
    {
        /// <summary>
        /// 书签的颜色
        /// </summary>
        string Color { get; set; }

        /// <summary>
        /// 书签宽度
        /// </summary>
        double Width { get; set; }

        /// <summary>
        /// 书签高度
        /// </summary>
        double Height { get; set; }

        double OffsetX { get; set; }

        double OffsetY { get; set; }
    }
}

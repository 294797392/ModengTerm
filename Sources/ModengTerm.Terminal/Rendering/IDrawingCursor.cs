using ModengTerm.Terminal.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTerminal.Base.Enumerations;

namespace ModengTerm.Terminal.Rendering
{
    public interface IDrawingCursor : IDrawingObject
    {
        /// <summary>
        /// 光标样式
        /// </summary>
        VTCursorStyles Style { get; set; }

        /// <summary>
        /// 光标颜色
        /// </summary>
        string Color { get; set; }

        /// <summary>
        /// 光标大小
        /// </summary>
        VTRect Size { get; set; }
    }
}

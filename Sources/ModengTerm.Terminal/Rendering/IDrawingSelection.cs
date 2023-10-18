using ModengTerm.Terminal.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal.Rendering
{
    public interface IDrawingSelection : IDrawingObject
    {
        /// <summary>
        /// 选项区域
        /// </summary>
        List<VTRect> Geometry { get; set; }
    }
}

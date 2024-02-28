using ModengTerm.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Document.Drawing
{
    public interface IDrawingSelection : IDrawingObject
    {
        /// <summary>
        /// 选项区域
        /// </summary>
        List<VTRect> Geometry { get; set; }
    }
}

using ModengTerm.Document.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Document.Drawing
{
    /// <summary>
    /// 要高亮显示的区域
    /// </summary>
    public interface IDrawingRectangle : IDrawingObject
    {
        /// <summary>
        /// 要显示的所有高亮区域
        /// </summary>
        public List<VTRectangleGeometry> Rectangles { get; set; }
    }
}

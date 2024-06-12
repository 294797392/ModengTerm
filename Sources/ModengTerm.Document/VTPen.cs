using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Document
{
    public class VTPen
    {
        /// <summary>
        /// 笔的颜色
        /// </summary>
        public VTColor Color { get; set; }

        /// <summary>
        /// 边框宽度
        /// </summary>
        public int Width { get; set; }

        public VTPen(VTColor color , int width)
        {
            Color = color;
            Width = width;
        }
    }
}

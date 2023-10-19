using ModengTerm.Terminal.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal.Rendering
{
    public interface IDrawingScrollbar : IDrawingObject
    {
        /// <summary>
        /// 最多可以滚动到的值
        /// </summary>
        double Maximum { get; set; }

        /// <summary>
        /// 当前滚动到的值
        /// </summary>
        double Value { get; set; }

        /// <summary>
        /// 可视区域的行数
        /// </summary>
        int ViewportRow { get; set; }
    }
}

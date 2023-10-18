using ModengTerm.Terminal.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal.Rendering
{
    /// <summary>
    /// 存储Scrollbar的样式
    /// </summary>
    public class ScrollbarStyle
    {
        public string ThumbColor { get; set; }

        public string TrackColor { get; set; }

        public string ButtonColor { get; set; }

        /// <summary>
        /// 滚动条宽度
        /// </summary>
        public double Width { get; set; }
    }

    public interface IDrawingScrollbar : IDrawingObject
    {
        /// <summary>
        /// 滚动条样式
        /// </summary>
        ScrollbarStyle Style { get; set; }

        /// <summary>
        /// 滚动条高度
        /// </summary>
        double Height { get; set; }

        /// <summary>
        /// 滚动条宽度
        /// </summary>
        double Width { get; set; }

        /// <summary>
        /// 滚动条下面的按钮大小
        /// </summary>
        VTRect IncreaseRect { get; set; }

        /// <summary>
        /// 滚动条上面的按钮大小
        /// </summary>
        VTRect DecreaseRect { get; set; }

        /// <summary>
        /// 滚动条的大小
        /// </summary>
        VTRect ThumbRect { get; set; }
    }
}

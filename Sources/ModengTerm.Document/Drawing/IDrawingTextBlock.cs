using ModengTerm.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Document.Drawing
{
    public interface IDrawingTextBlock : IDrawingObject
    {
        /// <summary>
        /// 要显示的文本内容
        /// </summary>
        VTFormattedText FormattedText { get; set; }
    }
}

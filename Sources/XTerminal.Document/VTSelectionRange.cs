using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTerminal.Document.Rendering;

namespace XTerminal.Document
{
    /// <summary>
    /// 存储鼠标选中的文本信息
    /// </summary>
    public class VTSelectionRange : VTDocumentElement
    {
        public override Drawables Type => Drawables.SelectionRange;

        /// <summary>
        /// 每一行就是一个矩形
        /// </summary>
        public List<VTRect> LineBounds { get; private set; }

        public VTSelectionRange()
        {
            this.LineBounds = new List<VTRect>();
        }
    }
}

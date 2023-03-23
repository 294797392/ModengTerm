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
    public class VTextSelection : VTDocumentElement
    {
        public override Drawables Type => Drawables.SelectionRange;

        /// <summary>
        /// 每一行就是一个矩形
        /// </summary>
        public List<VTRect> LineBounds { get; private set; }

        /// <summary>
        /// 所选内容的开始位置
        /// </summary>
        public VTextPointer Start { get; private set; }

        /// <summary>
        /// 所选内容的结束位置
        /// </summary>
        public VTextPointer End { get; private set; }

        public VTextSelection()
        {
            this.LineBounds = new List<VTRect>();
            this.Start = new VTextPointer();
            this.End = new VTextPointer();
        }

        /// <summary>
        /// 重置选中的状态
        /// </summary>
        public void Reset()
        {
            this.LineBounds.Clear();
            this.Start.IsEmpty = true;
            this.End.IsEmpty = true;
        }
    }
}

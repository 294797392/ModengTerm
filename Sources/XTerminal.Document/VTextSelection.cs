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
        public override VTDocumentElements Type => VTDocumentElements.SelectionRange;

        /// <summary>
        /// 选中的文本范围
        /// </summary>
        public List<VTRect> Ranges { get; private set; }

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
            this.Ranges = new List<VTRect>();
            this.Start = new VTextPointer();
            this.End = new VTextPointer();
        }

        /// <summary>
        /// 重置选中的状态
        /// </summary>
        public void Reset()
        {
            this.Ranges.Clear();

            this.Start.IsCharacterHit = false;
            this.Start.CharacterIndex = -1;
            this.Start.Line = null;

            this.End.IsCharacterHit = false;
            this.End.CharacterIndex = -1;
            this.End.Line = null;
        }
    }
}

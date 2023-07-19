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
        private StringBuilder textBuilder;

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

        /// <summary>
        /// 指示当前选中的内容是否为空
        /// </summary>
        public bool IsEmpty { get { return this.Ranges.Count == 0; } }

        public VTextSelection()
        {
            this.Ranges = new List<VTRect>();
            this.Start = new VTextPointer();
            this.End = new VTextPointer();
            this.textBuilder = new StringBuilder();
        }

        /// <summary>
        /// 重置选中的状态
        /// </summary>
        public void Reset()
        {
            this.Ranges.Clear();

            this.Start.CharacterIndex = -1;
            this.Start.LineHit = null;

            this.End.CharacterIndex = -1;
            this.End.LineHit = null;
        }

        /// <summary>
        /// 获取选中的原始文本字符串
        /// </summary>
        /// <returns></returns>
        public string GetText()
        {
            if (this.IsEmpty)
            {
                return string.Empty;
            }

            VTHistoryLine firstLine = this.Start.LineHit;
            VTHistoryLine lastLine = this.End.LineHit;

            // 当前只选中了一行
            if (firstLine == lastLine)
            {
                return firstLine.Text.Substring(this.Start.CharacterIndex, this.End.CharacterIndex - this.Start.CharacterIndex);
            }

            // 当前选中了多行，那么每行的数据都要复制

            this.textBuilder.Clear();

            VTHistoryLine currentLine = firstLine;
            while (currentLine != null)
            {
                if (currentLine == firstLine)
                {
                    // 第一行
                    this.textBuilder.AppendLine(currentLine.Text.Substring(this.Start.CharacterIndex));
                }
                else if (currentLine == lastLine)
                {
                    // 最后一行
                    this.textBuilder.AppendLine(currentLine.Text.Substring(0, this.End.CharacterIndex + 1));
                    break;
                }
                else
                {
                    // 中间的行
                    this.textBuilder.AppendLine(currentLine.Text);
                }

                currentLine = currentLine.NextLine;
            }

            return this.textBuilder.ToString();
        }
    }
}

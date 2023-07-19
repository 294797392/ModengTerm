using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTerminal.Document.Rendering;

namespace XTerminal.Document
{
    /// <summary>
    /// �洢���ѡ�е��ı���Ϣ
    /// </summary>
    public class VTextSelection : VTDocumentElement
    {
        private StringBuilder textBuilder;

        public override VTDocumentElements Type => VTDocumentElements.SelectionRange;

        /// <summary>
        /// ѡ�е��ı���Χ
        /// </summary>
        public List<VTRect> Ranges { get; private set; }

        /// <summary>
        /// ��ѡ���ݵĿ�ʼλ��
        /// </summary>
        public VTextPointer Start { get; private set; }

        /// <summary>
        /// ��ѡ���ݵĽ���λ��
        /// </summary>
        public VTextPointer End { get; private set; }

        /// <summary>
        /// ָʾ��ǰѡ�е������Ƿ�Ϊ��
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
        /// ����ѡ�е�״̬
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
        /// ��ȡѡ�е�ԭʼ�ı��ַ���
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

            // ��ǰֻѡ����һ��
            if (firstLine == lastLine)
            {
                return firstLine.Text.Substring(this.Start.CharacterIndex, this.End.CharacterIndex - this.Start.CharacterIndex);
            }

            // ��ǰѡ���˶��У���ôÿ�е����ݶ�Ҫ����

            this.textBuilder.Clear();

            VTHistoryLine currentLine = firstLine;
            while (currentLine != null)
            {
                if (currentLine == firstLine)
                {
                    // ��һ��
                    this.textBuilder.AppendLine(currentLine.Text.Substring(this.Start.CharacterIndex));
                }
                else if (currentLine == lastLine)
                {
                    // ���һ��
                    this.textBuilder.AppendLine(currentLine.Text.Substring(0, this.End.CharacterIndex + 1));
                    break;
                }
                else
                {
                    // �м����
                    this.textBuilder.AppendLine(currentLine.Text);
                }

                currentLine = currentLine.NextLine;
            }

            return this.textBuilder.ToString();
        }
    }
}

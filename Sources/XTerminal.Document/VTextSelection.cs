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
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger("VTextSelection");

        private StringBuilder textBuilder;

        /// <summary>
        /// �Ƿ���Ҫ�ػ�
        /// </summary>
        public bool IsDirty { get; private set; }

        public override VTDocumentElements Type => VTDocumentElements.SelectionRange;

        /// <summary>
        /// ѡ�����ݵļ��α�ʾ��ʽ
        /// </summary>
        public List<VTRect> Geometry { get; private set; }

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
        public bool IsEmpty { get { return this.Start.CharacterIndex < 0 || this.End.CharacterIndex < 0; } }

        public VTextSelection()
        {
            this.Geometry = new List<VTRect>();
            this.Start = new VTextPointer();
            this.End = new VTextPointer();
            this.textBuilder = new StringBuilder();

            this.Start.CharacterIndex = -1;
            this.End.CharacterIndex = -1;
        }

        /// <summary>
        /// ����ѡ�е�״̬
        /// </summary>
        public void Reset()
        {
            this.OffsetY = 0;
            this.OffsetX = 0;

            this.Geometry.Clear();

            this.Start.CharacterIndex = -1;
            this.Start.PhysicsRow = -1;

            this.End.CharacterIndex = -1;
            this.End.PhysicsRow = -1;
        }

        /// <summary>
        /// ͨ����ʷ�����ݻ�ȡѡ�е��ı�����
        /// </summary>
        /// <param name="historyLines">��ʷ���б�</param>
        /// <param name="selection">ѡ�е��ı���Ϣ</param>
        /// <returns></returns>
        public string GetText(Dictionary<int, VTHistoryLine> historyLines)
        {
            // �ҵ�ѡ�е���ʼ�кͽ����е���Ϣ
            VTHistoryLine firstLine, lastLine;
            if (!historyLines.TryGetValue(this.Start.PhysicsRow, out firstLine) ||
                !historyLines.TryGetValue(this.End.PhysicsRow, out lastLine))
            {
                logger.ErrorFormat("��ȡѡ�е��ı�����ʧ��, ��һ�л����һ��Ϊ��");
                return string.Empty;
            }

            // ��ǰֻѡ����һ��
            if (firstLine == lastLine)
            {
                // ע��Ҫ��������������ѡ�е����
                // �������Ǵ����������ѡ�У���ôStart����Selection���ұߣ�End����Selection�����
                int startCharacterIndex = Math.Min(this.Start.CharacterIndex, this.End.CharacterIndex);
                int endCharacterIndex = Math.Max(this.Start.CharacterIndex, this.End.CharacterIndex);
                return firstLine.Text.Substring(startCharacterIndex, endCharacterIndex - startCharacterIndex + 1);
            }

            // ���֮ǰѡ�е�����
            this.textBuilder.Clear();

            // ����������ѡ�У���Ҫ��������
            bool reverse = this.Start.PhysicsRow > this.End.PhysicsRow;

            // ��ǰѡ���˶��У���ôÿ�е����ݶ�Ҫ����
            VTHistoryLine currentLine = reverse ? lastLine : firstLine;
            while (currentLine != null)
            {
                if (currentLine == firstLine)
                {
                    if (reverse)
                    {
                        // ���������������ѡ�У���ô�������һ��
                        this.textBuilder.AppendLine(currentLine.Text.Substring(0, this.Start.CharacterIndex + 1));
                        break;
                    }
                    else
                    {
                        // ��һ��
                        this.textBuilder.AppendLine(currentLine.Text.Substring(this.Start.CharacterIndex));
                    }
                }
                else if (currentLine == lastLine)
                {
                    if (reverse)
                    {
                        // ���������������ѡ�У���ô���ǵ�һ��
                        this.textBuilder.AppendLine(currentLine.Text.Substring(this.End.CharacterIndex));
                    }
                    else
                    {
                        // ���һ��
                        this.textBuilder.AppendLine(currentLine.Text.Substring(0, this.End.CharacterIndex + 1));
                        break;
                    }
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

        public void SetDirty(bool isDirty)
        {
            if (this.IsDirty != isDirty)
            {
                this.IsDirty = isDirty;
            }
        }

        #region ʵ������

        #endregion
    }
}

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
        public bool IsEmpty { get { return this.Geometry.Count == 0; } }

        public VTextSelection()
        {
            this.Geometry = new List<VTRect>();
            this.Start = new VTextPointer();
            this.End = new VTextPointer();
            this.textBuilder = new StringBuilder();
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
            this.Start.CharacterBounds = VTRect.Empty;

            this.End.CharacterIndex = -1;
            this.End.PhysicsRow = -1;
            this.Start.CharacterBounds = VTRect.Empty;
        }

        /// <summary>
        /// ͨ����ʷ�����ݻ�ȡѡ�е��ı�����
        /// </summary>
        /// <param name="historyLines">��ʷ���б�</param>
        /// <param name="selection">ѡ�е��ı���Ϣ</param>
        /// <returns></returns>
        public string GetText(Dictionary<int, VTHistoryLine> historyLines)
        {
            if (this.IsEmpty)
            {
                return string.Empty;
            }

            // �ҵ�ѡ�еĵ�һ�к����һ�е���Ϣ
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

        /// <summary>
        /// ����ѡ�����ݵļ���ͼ��
        /// </summary>
        public void BuildGeometry()
        {
            this.Geometry.Clear();

            VTextPointer startPointer = this.Start;
            VTextPointer endPointer = this.End;

            // �ж���ʼλ�û��߽���λ���Ƿ���Surface��

            // ���������ƶ�����
            TextPointerPositions pointerPosition = VTextSelectionHelper.GetTextPointerPosition(startPointer, endPointer);

            switch (pointerPosition)
            {
                case TextPointerPositions.Original:
                    {
                        break;
                    }

                // �������������ͬһ�����ƶ�
                case TextPointerPositions.Right:
                case TextPointerPositions.Left:
                    {
                        VTRect rect1 = startPointer.CharacterBounds;
                        VTRect rect2 = endPointer.CharacterBounds;

                        double xmin = Math.Min(rect1.Left, rect2.Left);
                        double xmax = Math.Max(rect1.Right, rect2.Right);
                        double x = xmin;
                        double y = startPointer.OffsetY;
                        double width = xmax - xmin;
                        double height = rect1.Height;

                        VTRect bounds = new VTRect(x, y, width, height);
                        this.Geometry.Add(bounds);
                        break;
                    }

                // ����������������ƶ�
                default:
                    {
                        // �����ϱߺ��±ߵľ���
                        VTextPointer topPointer = startPointer.PhysicsRow < endPointer.PhysicsRow ? startPointer : endPointer;
                        VTextPointer bottomPointer = startPointer.PhysicsRow < endPointer.PhysicsRow ? endPointer : startPointer;

                        //logger.FatalFormat("top = {0}, bottom = {1}", topPointer.Row, bottomPointer.Row);

                        // �����Panel����ʼѡ�б߽��ͽ���ѡ�еı߽��
                        VTRect topBounds = topPointer.CharacterBounds;
                        VTRect bottomBounds = bottomPointer.CharacterBounds;

                        // ��һ�еľ���
                        this.Geometry.Add(new VTRect(topBounds.X, topPointer.OffsetY, 9999, topBounds.Height));

                        // �м�ľ���
                        double y = topPointer.OffsetY + topBounds.Height;
                        double height = bottomPointer.OffsetY - (topPointer.OffsetY + topBounds.Height);
                        this.Geometry.Add(new VTRect(0, y, 9999, height));

                        // ���һ�еľ���
                        this.Geometry.Add(new VTRect(0, bottomPointer.OffsetY, bottomBounds.Right, bottomBounds.Height));
                        break;
                    }
            }
        }
    }
}

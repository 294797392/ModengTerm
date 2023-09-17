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
        #region �����

        private static readonly VTRect NullRect = new VTRect(-1, -1, -1, -1);

        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger("VTextSelection");

        #endregion

        #region ʵ������

        private StringBuilder textBuilder;

        private VTDocument document;
        private int startPhysicsRow;
        private int endPhysicsRow;
        private VTRect container;

        #endregion

        #region ����

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

        #endregion

        #region ���췽��

        public VTextSelection()
        {
            this.Geometry = new List<VTRect>();
            this.Start = new VTextPointer();
            this.End = new VTextPointer();
            this.textBuilder = new StringBuilder();

            this.Start.CharacterIndex = -1;
            this.End.CharacterIndex = -1;
        }

        #endregion

        /// <summary>
        /// ���ѡ�е�����
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

            this.document = null;
            this.startPhysicsRow = -1;
            this.endPhysicsRow = -1;

            this.container = NullRect;

            this.SetArrangeDirty(true);
        }

        /// <summary>
        /// �����ı�ѡ�еķ�Χ
        /// </summary>
        /// <param name="document">Ҫ���ĸ��ĵ�������ѡ�з�Χ</param>
        /// <param name="container">Canvas����ڵ�����ʾ����Ļ��λ��</param>
        public void SetRange(VTDocument document, VTRect container, int startPhysicsRow, int startCharacterIndex, int endPhysicsRow, int endCharacterIndex)
        {
            this.Start.PhysicsRow = startPhysicsRow;
            this.Start.CharacterIndex = startCharacterIndex;
            this.End.PhysicsRow = endPhysicsRow;
            this.End.CharacterIndex = endCharacterIndex;
            this.document = document;
            this.startPhysicsRow = document.FirstLine.PhysicsRow;
            this.endPhysicsRow = document.LastLine.PhysicsRow;
            this.container = container;

            this.UpdateRange(document, container);
        }

        /// <summary>
        /// ���ݵ�ǰ��TextPointer��Ϣ����ѡ���������״
        /// ��ȻTextPointer����ֵ��һ���ģ����ǵ��ƶ��˹�����֮��ѡ���������ʾ�Ͳ�һ����
        /// </summary>
        /// <param name="document"></param>
        /// <param name="container"></param>
        public void UpdateRange(VTDocument document, VTRect container)
        {
            this.Geometry.Clear();

            // ��������ѡ�е���ͬһ�е����
            if (this.Start.PhysicsRow == this.End.PhysicsRow)
            {
                // �ҵ���Ӧ���ı���
                VTextLine textLine = document.FindLine(this.Start.PhysicsRow);
                if (textLine == null)
                {
                    // ��ѡ����һ��֮��Ȼ����б��ƶ�����Ļ���ˣ�������������
                    return;
                }

                this.SetArrangeDirty(true);

                VTextPointer leftPointer = this.Start.CharacterIndex < this.End.CharacterIndex ? this.Start : this.End;
                VTextPointer rightPointer = this.Start.CharacterIndex < this.End.CharacterIndex ? this.End : this.Start;

                VTRect leftBounds = textLine.MeasureCharacter(leftPointer.CharacterIndex);
                VTRect rightBounds = textLine.MeasureCharacter(rightPointer.CharacterIndex);

                double x = leftBounds.Left;
                double y = textLine.OffsetY;
                double width = rightBounds.Right - leftBounds.Left;
                double height = textLine.Height;

                VTRect bounds = new VTRect(x, y, width, height);
                this.Geometry.Add(bounds);
                return;
            }

            this.SetArrangeDirty(true);

            // ���洦��ѡ���˶��е�״̬
            VTextPointer topPointer = this.Start.PhysicsRow > this.End.PhysicsRow ? this.End : this.Start;
            VTextPointer bottomPointer = this.Start.PhysicsRow > this.End.PhysicsRow ? this.Start : this.End;

            VTextLine topLine = document.FindLine(topPointer.PhysicsRow);
            VTextLine bottomLine = document.FindLine(bottomPointer.PhysicsRow);

            if (topLine != null && bottomLine != null)
            {
                // ��ʱ˵��ѡ�е����ݶ�����Ļ��
                // �����ϱߺ��±ߵľ���
                VTRect topBounds = topLine.MeasureCharacter(topPointer.CharacterIndex);
                VTRect bottomBounds = bottomLine.MeasureCharacter(bottomPointer.CharacterIndex);

                // ��һ�еľ���
                this.Geometry.Add(new VTRect(topBounds.X, topLine.OffsetY, container.Width - topBounds.X, topLine.Height));

                // �м�ľ���
                double y = topLine.OffsetY + topBounds.Height;
                double height = bottomLine.OffsetY - (topLine.OffsetY + topBounds.Height);
                this.Geometry.Add(new VTRect(0, y, container.Width, height));

                // ���һ�еľ���
                this.Geometry.Add(new VTRect(0, bottomLine.OffsetY, bottomBounds.Right, bottomLine.Height));
                return;
            }

            if (topLine != null && bottomLine == null)
            {
                // ѡ�е�������һ���ֱ��Ƶ���Ļ���ˣ������������ƶ�
                VTRect topBounds = topLine.MeasureCharacter(topPointer.CharacterIndex);

                // ��һ�еľ���
                this.Geometry.Add(new VTRect(topBounds.X, topLine.OffsetY, container.Width - topBounds.X, topLine.Height));

                // ʣ�µľ���
                double height = document.LastLine.Bounds.Bottom - topLine.Bounds.Bottom;
                this.Geometry.Add(new VTRect(0, topLine.Bounds.Bottom, container.Width, height));
                return;
            }

            if (topLine == null && bottomLine != null)
            {
                // ѡ�е�������һ���ֱ��Ƶ���Ļ���ˣ������������ƶ�
                VTRect bottomBounds = bottomLine.MeasureCharacter(bottomPointer.CharacterIndex);

                // ���һ�еľ���
                this.Geometry.Add(new VTRect(0, bottomLine.OffsetY, bottomBounds.Right, bottomLine.Height));

                // ʣ�µľ���
                this.Geometry.Add(new VTRect(0, 0, container.Width, bottomLine.OffsetY));
                return;
            }

            if (topPointer.PhysicsRow < document.FirstLine.PhysicsRow &&
                bottomPointer.PhysicsRow > document.LastLine.PhysicsRow)
            {
                // ���������˵����ǰ��ʾ�����ݱ�ȫ��ѡ����
                this.Geometry.Add(new VTRect(0, 0, container.Width, document.LastLine.Bounds.Bottom));
                return;
            }
        }

        public override void RequestInvalidate()
        {
            if (this.arrangeDirty)
            {
                this.DrawingObject.Draw();

                this.arrangeDirty = false;
            }
        }
    }
}

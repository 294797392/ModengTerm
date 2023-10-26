using ModengTerm.Terminal.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal.Document
{
    /// <summary>
    /// �洢���ѡ�е��ı���Ϣ
    /// </summary>
    public class VTextSelection : VTDocumentElement<IDrawingSelection>
    {
        #region �����

        private static readonly VTRect NullRect = new VTRect(-1, -1, -1, -1);

        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger("VTextSelection");

        #endregion

        #region ʵ������

        private bool dirty;

        private int firstRow;
        private int lastRow;
        private int firstRowCharacterIndex;
        private int lastRowCharacterIndex;

        #endregion

        #region ����

        public override VTDocumentElements Type => VTDocumentElements.SelectionRange;

        /// <summary>
        /// ѡ�����ݵļ��α�ʾ��ʽ
        /// </summary>
        public List<VTRect> Geometry { get { return this.DrawingObject.Geometry; } set { this.DrawingObject.Geometry = value; } }

        /// <summary>
        /// ָʾ��ǰѡ�е������Ƿ�Ϊ��
        /// </summary>
        public bool IsEmpty { get { return this.firstRowCharacterIndex < 0 || this.lastRowCharacterIndex < 0; } }

        public int FirstRow
        {
            get { return this.firstRow; }
            set
            {
                if (this.firstRow != value)
                {
                    this.firstRow = value;
                    this.SetDirty(true);
                }
            }
        }

        public int LastRow
        {
            get { return this.lastRow; }
            set
            {
                if (this.lastRow != value)
                {
                    this.lastRow = value;
                    this.SetDirty(true);
                }
            }
        }

        public int FirstRowCharacterIndex
        {
            get { return this.firstRowCharacterIndex; }
            set
            {
                if (this.firstRowCharacterIndex != value)
                {
                    this.firstRowCharacterIndex = value;
                    this.SetDirty(true);
                }
            }
        }

        public int LastRowCharacterIndex
        {
            get { return this.lastRowCharacterIndex; }
            set
            {
                if (this.lastRowCharacterIndex != value)
                {
                    this.lastRowCharacterIndex = value;
                    this.SetDirty(true);
                }
            }
        }

        #endregion

        #region ���췽��

        public VTextSelection(VTDocument ownerDocument) :
            base(ownerDocument)
        {
        }

        #endregion

        #region ʵ������

        public void SetDirty(bool dirty)
        {
            if (this.dirty != dirty)
            {
                this.dirty = dirty;
            }
        }

        /// <summary>
        /// ���ݵ�ǰ��TextPointer��Ϣ����ѡ���������״
        /// ��ȻTextPointer����ֵ��һ���ģ����ǵ��ƶ��˹�����֮��ѡ���������ʾ�Ͳ�һ����
        /// </summary>
        /// <param name="document"></param>
        /// <param name="container"></param>
        private void UpdateGeometry()
        {
            VTDocument document = this.ownerDocument;

            VTextPointer Start = new VTextPointer(this.firstRow, this.firstRowCharacterIndex);
            VTextPointer End = new VTextPointer(this.lastRow, this.lastRowCharacterIndex);

            VTRect container = document.DrawingObject.GetContentRect();

            this.Geometry.Clear();

            // ��������ѡ�е���ͬһ�е����
            if (Start.PhysicsRow == End.PhysicsRow)
            {
                // �ҵ���Ӧ���ı���
                VTextLine textLine = document.FindLine(Start.PhysicsRow);
                if (textLine == null)
                {
                    // ��ѡ����һ��֮��Ȼ����б��ƶ�����Ļ���ˣ�������������
                    return;
                }

                VTextPointer leftPointer = Start.CharacterIndex < End.CharacterIndex ? Start : End;
                VTextPointer rightPointer = Start.CharacterIndex < End.CharacterIndex ? End : Start;

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

            // ���洦��ѡ���˶��е�״̬
            VTextPointer topPointer = Start.PhysicsRow > End.PhysicsRow ? End : Start;
            VTextPointer bottomPointer = Start.PhysicsRow > End.PhysicsRow ? Start : End;

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

        #endregion

        /// <summary>
        /// ���ѡ�е�����
        /// </summary>
        public void Reset()
        {
            this.OffsetY = 0;
            this.OffsetX = 0;

            this.Geometry.Clear();

            this.FirstRow = -1;
            this.LastRow = -1;
            this.FirstRowCharacterIndex = -1;
            this.LastRowCharacterIndex = -1;

            this.SetDirty(true);
        }

        public void Normalize(out int topRow, out int bottomRow, out int startIndex, out int endIndex)
        {
            VTextPointer Start = new VTextPointer(this.firstRow, this.firstRowCharacterIndex);
            VTextPointer End = new VTextPointer(this.lastRow, this.lastRowCharacterIndex);

            if (Start.PhysicsRow == End.PhysicsRow)
            {
                topRow = Start.PhysicsRow;
                bottomRow = End.PhysicsRow;

                // ע��Ҫ��������������ѡ�е����
                // �������Ǵ����������ѡ�У���ôStart����Selection���ұߣ�End����Selection�����
                startIndex = Math.Min(Start.CharacterIndex, End.CharacterIndex);
                endIndex = Math.Max(Start.CharacterIndex, End.CharacterIndex);
            }
            else
            {
                // Ҫ��������������ѡ�е����
                // �������������ѡ�У���ô��ʱ�����VTextPointer����ʼ�������VTextPointer�ǽ���
                if (Start.PhysicsRow > End.PhysicsRow)
                {
                    topRow = End.PhysicsRow;
                    bottomRow = Start.PhysicsRow;
                    startIndex = End.CharacterIndex;
                    endIndex = Start.CharacterIndex;
                }
                else
                {
                    topRow = Start.PhysicsRow;
                    bottomRow = End.PhysicsRow;
                    startIndex = Start.CharacterIndex;
                    endIndex = End.CharacterIndex;
                }
            }
        }

        #region VTElement

        protected override void OnInitialize()
        {
            this.Geometry = new List<VTRect>();

            this.FirstRow = -1;
            this.LastRow = -1;
            this.FirstRowCharacterIndex = -1;
            this.LastRowCharacterIndex = -1;

            this.Geometry = new List<VTRect>();
        }

        protected override void OnRelease()
        {
        }

        public override void RequestInvalidate()
        {
            if (this.dirty)
            {
                this.Geometry.Clear();

                if (!this.IsEmpty)
                {
                    this.UpdateGeometry();
                }

                this.DrawingObject.Draw();

                this.dirty = false;
            }
        }

        #endregion
    }
}

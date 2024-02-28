using ModengTerm.Document.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Document
{
    /// <summary>
    /// �洢���ѡ�е��ı���Ϣ
    /// </summary>
    public class VTextSelection : VTElement
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
        private List<VTRect> geometries;

        #endregion

        #region ����

        public override DrawingObjectTypes Type => DrawingObjectTypes.Selection;

        /// <summary>
        /// ָʾ��ǰѡ�е������Ƿ�Ϊ��
        /// </summary>
        public bool IsEmpty { get { return firstRowCharacterIndex < 0 || lastRowCharacterIndex < 0; } }

        /// <summary>
        /// ѡ������ĵ�һ�е������к�
        /// </summary>
        public int FirstRow
        {
            get { return firstRow; }
            set
            {
                if (firstRow != value)
                {
                    firstRow = value;
                    this.SetDirtyFlags(VTDirtyFlags.RenderDirty, true);
                }
            }
        }

        /// <summary>
        /// ѡ����������һ�е������к�
        /// </summary>
        public int LastRow
        {
            get { return lastRow; }
            set
            {
                if (lastRow != value)
                {
                    lastRow = value;
                    this.SetDirtyFlags(VTDirtyFlags.RenderDirty, true);
                }
            }
        }

        public int FirstRowCharacterIndex
        {
            get { return firstRowCharacterIndex; }
            set
            {
                if (firstRowCharacterIndex != value)
                {
                    firstRowCharacterIndex = value;
                    this.SetDirtyFlags(VTDirtyFlags.RenderDirty, true);
                }
            }
        }

        public int LastRowCharacterIndex
        {
            get { return lastRowCharacterIndex; }
            set
            {
                if (lastRowCharacterIndex != value)
                {
                    lastRowCharacterIndex = value;
                    this.SetDirtyFlags(VTDirtyFlags.RenderDirty, true);
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

        /// <summary>
        /// ���ݵ�ǰ��TextPointer��Ϣ����ѡ���������״
        /// ��ȻTextPointer����ֵ��һ���ģ����ǵ��ƶ��˹�����֮��ѡ���������ʾ�Ͳ�һ����
        /// </summary>
        /// <param name="document"></param>
        /// <param name="container"></param>
        private void UpdateGeometry()
        {
            VTextPointer Start = new VTextPointer(firstRow, firstRowCharacterIndex);
            VTextPointer End = new VTextPointer(lastRow, lastRowCharacterIndex);

            VTDocument document = this.OwnerDocument;
            VTSize container = document.DrawingObject.Size;

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
                geometries.Add(bounds);
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
                geometries.Add(new VTRect(topBounds.X, topLine.OffsetY, container.Width - topBounds.X, topLine.Height));

                // �м�ľ���
                double y = topLine.OffsetY + topBounds.Height;
                double height = bottomLine.OffsetY - (topLine.OffsetY + topBounds.Height);
                geometries.Add(new VTRect(0, y, container.Width, height));

                // ���һ�еľ���
                geometries.Add(new VTRect(0, bottomLine.OffsetY, bottomBounds.Right, bottomLine.Height));
                return;
            }

            if (topLine != null && bottomLine == null)
            {
                // ѡ�е�������һ���ֱ��Ƶ���Ļ���ˣ������������ƶ�
                VTRect topBounds = topLine.MeasureCharacter(topPointer.CharacterIndex);

                // ��һ�еľ���
                geometries.Add(new VTRect(topBounds.X, topLine.OffsetY, container.Width - topBounds.X, topLine.Height));

                // ʣ�µľ���
                double height = document.LastLine.Bounds.Bottom - topLine.Bounds.Bottom;
                geometries.Add(new VTRect(0, topLine.Bounds.Bottom, container.Width, height));
                return;
            }

            if (topLine == null && bottomLine != null)
            {
                // ѡ�е�������һ���ֱ��Ƶ���Ļ���ˣ������������ƶ�
                VTRect bottomBounds = bottomLine.MeasureCharacter(bottomPointer.CharacterIndex);

                // ���һ�еľ���
                geometries.Add(new VTRect(0, bottomLine.OffsetY, bottomBounds.Right, bottomLine.Height));

                // ʣ�µľ���
                geometries.Add(new VTRect(0, 0, container.Width, bottomLine.OffsetY));
                return;
            }

            if (topPointer.PhysicsRow < document.FirstLine.PhysicsRow &&
                bottomPointer.PhysicsRow > document.LastLine.PhysicsRow)
            {
                // ���������˵����ǰ��ʾ�����ݱ�ȫ��ѡ����
                geometries.Add(new VTRect(0, 0, container.Width, document.LastLine.Bounds.Bottom));
                return;
            }
        }

        #endregion

        #region �����ӿ�

        /// <summary>
        /// ���ѡ�е�����
        /// </summary>
        public void Reset()
        {
            OffsetY = 0;
            OffsetX = 0;

            this.geometries.Clear();

            FirstRow = -1;
            LastRow = -1;
            FirstRowCharacterIndex = -1;
            LastRowCharacterIndex = -1;
        }

        public void Normalize(out int topRow, out int bottomRow, out int startIndex, out int endIndex)
        {
            VTextPointer Start = new VTextPointer(firstRow, firstRowCharacterIndex);
            VTextPointer End = new VTextPointer(lastRow, lastRowCharacterIndex);

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

        #endregion

        #region VTElement

        protected override void OnInitialize(IDrawingObject drawingObject)
        {
            this.geometries = new List<VTRect>();

            FirstRow = -1;
            LastRow = -1;
            FirstRowCharacterIndex = -1;
            LastRowCharacterIndex = -1;
        }

        protected override void OnRelease()
        {
        }

        protected override void OnRender()
        {
            if (this.IsEmpty)
            {
                return;
            }

            UpdateGeometry();
            DrawingObject.Draw();
        }

        #endregion
    }
}

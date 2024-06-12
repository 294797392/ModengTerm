using ModengTerm.Document.Drawing;
using ModengTerm.Document.Utility;
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

        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger("VTextSelection");

        #endregion

        #region ʵ������

        private int startRow;
        private int endRow;
        private int startColumn;
        private int endColumn;
        private List<VTRect> geometries;
        private VTColor backColor;

        #endregion

        #region ����

        public override DrawingObjectTypes Type => DrawingObjectTypes.Selection;

        /// <summary>
        /// ָʾ��ǰѡ�е������Ƿ�Ϊ��
        /// </summary>
        public bool IsEmpty { get { return startColumn < 0 || endColumn < 0; } }

        /// <summary>
        /// ѡ������ĵ�һ�е������к�
        /// </summary>
        public int StartRow
        {
            get { return startRow; }
            set
            {
                if (startRow != value)
                {
                    startRow = value;
                    this.SetDirtyFlags(VTDirtyFlags.RenderDirty, true);
                }
            }
        }

        /// <summary>
        /// ѡ������ĵ�һ�еĵ�һ���ַ�
        /// </summary>
        public int StartColumn
        {
            get { return startColumn; }
            set
            {
                if (startColumn != value)
                {
                    startColumn = value;
                    this.SetDirtyFlags(VTDirtyFlags.RenderDirty, true);
                }
            }
        }

        /// <summary>
        /// ѡ����������һ�е������к�
        /// </summary>
        public int EndRow
        {
            get { return endRow; }
            set
            {
                if (endRow != value)
                {
                    endRow = value;
                    this.SetDirtyFlags(VTDirtyFlags.RenderDirty, true);
                }
            }
        }

        /// <summary>
        /// ѡ����������һ�е����һ���ַ�
        /// </summary>
        public int EndColumn
        {
            get { return endColumn; }
            set
            {
                if (endColumn != value)
                {
                    endColumn = value;
                    this.SetDirtyFlags(VTDirtyFlags.RenderDirty, true);
                }
            }
        }

        /// <summary>
        /// ѡ���������ɫ
        /// </summary>
        public string Color { get; set; }

        #endregion

        #region ���췽��

        public VTextSelection(VTDocument ownerDocument) :
            base(ownerDocument)
        {
        }

        #endregion

        #region ʵ������

        #endregion

        #region �����ӿ�

        /// <summary>
        /// ���ݵ�ǰ��TextPointer��Ϣ����ѡ���������״
        /// ѡ��������Ҫ�����漸��ʱ�����£�
        /// 1. �ڵ�ǰҳ��ѡ�в�������Ҫ����
        /// 2. ��ǰҳ�����ѡ�������ҹ����˹�����֮��Ҳ��Ҫ����
        /// </summary>
        /// <param name="document"></param>
        /// <param name="container"></param>
        public void UpdateGeometry()
        {
            this.SetDirtyFlags(VTDirtyFlags.RenderDirty, true);

            this.geometries.Clear();

            VTextPointer Start = new VTextPointer(startRow, startColumn);
            VTextPointer End = new VTextPointer(endRow, endColumn);

            VTDocument document = this.OwnerDocument;
            VTSize container = document.Renderer.Size;

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

                // ��������ѡ�е���һ���ַ������
                if (Start.CharacterIndex == End.CharacterIndex)
                {
                    // ѡ�е���һ���ַ�
                    VTextRange bounds1 = textLine.MeasureCharacter(Start.CharacterIndex);
                    geometries.Add(VTRect.CreateFromTextRange(bounds1, textLine.OffsetY));
                    return;
                }

                VTextPointer leftPointer = Start.CharacterIndex < End.CharacterIndex ? Start : End;
                VTextPointer rightPointer = Start.CharacterIndex < End.CharacterIndex ? End : Start;

                VTextRange bounds = textLine.MeasureTextRange(leftPointer.CharacterIndex, rightPointer.CharacterIndex - leftPointer.CharacterIndex + 1);
                geometries.Add(VTRect.CreateFromTextRange(bounds, textLine.OffsetY));
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
                VTextRange topBounds = topLine.MeasureCharacter(topPointer.CharacterIndex);
                VTextRange bottomBounds = bottomLine.MeasureCharacter(bottomPointer.CharacterIndex);

                // ��һ�еľ���
                geometries.Add(new VTRect(topBounds.OffsetX, topBounds.OffsetY, container.Width - topBounds.OffsetX, topLine.Height));

                // �м�ľ���
                double y = topLine.OffsetY + topBounds.Height;
                double height = bottomLine.OffsetY - (topLine.OffsetY + topBounds.Height);
                geometries.Add(new VTRect(0, y, container.Width, height));

                // ���һ�еľ���
                geometries.Add(new VTRect(0, bottomLine.OffsetY, bottomBounds.OffsetX + bottomBounds.Width, bottomLine.Height));
                return;
            }

            if (topLine != null && bottomLine == null)
            {
                // ѡ�е�������һ���ֱ��Ƶ���Ļ���ˣ������������ƶ�
                VTextRange topBounds = topLine.MeasureCharacter(topPointer.CharacterIndex);

                // ��һ�еľ���
                geometries.Add(new VTRect(topBounds.OffsetX, topLine.OffsetY, container.Width - topBounds.OffsetX, topLine.Height));

                // ʣ�µľ���
                double height = document.LastLine.Bounds.Bottom - topLine.Bounds.Bottom;
                geometries.Add(new VTRect(0, topLine.Bounds.Bottom, container.Width, height));
                return;
            }

            if (topLine == null && bottomLine != null)
            {
                // ѡ�е�������һ���ֱ��Ƶ���Ļ���ˣ������������ƶ�
                VTextRange bottomBounds = bottomLine.MeasureCharacter(bottomPointer.CharacterIndex);

                // 
                geometries.Add(new VTRect(0, 0, container.Width, bottomBounds.OffsetY));

                // ���һ�еľ���
                geometries.Add(new VTRect(0, bottomBounds.OffsetY, bottomBounds.OffsetX + bottomBounds.Width, bottomLine.Height));
                return;
            }

            if (topPointer.PhysicsRow < document.Scrollbar.ScrollValue &&
                bottomPointer.PhysicsRow >= document.Scrollbar.ScrollValue + document.ViewportRow - 1)
            {
                // ѡ������ĵ�һ���ڵ�ǰ��ʾ�ĵ�һ��֮ǰ
                // ѡ����������һ���ڵ�ǰ��ʾ�����һ��֮��
                this.geometries.Add(new VTRect(0, 0, container.Width, document.LastLine.Bounds.Bottom));
                return;
            }

            // ��ѡ������󣬲���ѡ�����򲻴��ڵ�ǰҳ����
            // ʲô������
        }

        /// <summary>
        /// ���ѡ�е�����
        /// </summary>
        public void Clear()
        {
            OffsetY = 0;
            OffsetX = 0;

            StartRow = -1;
            EndRow = -1;
            StartColumn = -1;
            EndColumn = -1;

            this.geometries.Clear();

            this.SetDirtyFlags(VTDirtyFlags.RenderDirty, true);
        }

        public void Normalize(out int topRow, out int bottomRow, out int startIndex, out int endIndex)
        {
            VTextPointer Start = new VTextPointer(startRow, startColumn);
            VTextPointer End = new VTextPointer(endRow, endColumn);

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

        protected override void OnInitialize()
        {
            this.geometries = new List<VTRect>();

            StartRow = -1;
            EndRow = -1;
            StartColumn = -1;
            EndColumn = -1;

            this.backColor = VTColor.CreateFromRgbKey(this.Color);
        }

        protected override void OnRelease()
        {
        }

        protected override void OnRender()
        {
            this.DrawingObject.DrawRectangles(this.geometries, null, this.backColor);
        }

        #endregion
    }
}

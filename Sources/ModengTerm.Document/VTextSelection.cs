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

        private List<VTRect> geometries;
        private VTColor backColor;

        #endregion

        #region ����

        public override DrawingObjectTypes Type => DrawingObjectTypes.Selection;

        /// <summary>
        /// ָʾ��ǰѡ�е������Ƿ�Ϊ��
        /// </summary>
        public bool IsEmpty { get { return this.StartPointer.ColumnIndex < 0 || this.EndPointer.ColumnIndex < 0; } }

        public VTextPointer StartPointer { get; set; }

        public VTextPointer EndPointer { get; set; }

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
        /// ѡ��ĳһ��
        /// </summary>
        /// <param name="textLine"></param>
        /// <param name="logicalRow"></param>
        public void SelectRow(VTextLine textLine, int logicalRow)
        {
            VTDocument document = this.OwnerDocument;
            VTScrollInfo scrollInfo = this.OwnerDocument.Scrollbar;

            this.StartPointer.PhysicsRow = scrollInfo.ScrollValue + logicalRow;
            this.StartPointer.CharacterIndex = 0;
            this.StartPointer.ColumnIndex = 0;

            this.EndPointer.PhysicsRow = scrollInfo.ScrollValue + logicalRow;
            this.EndPointer.CharacterIndex = textLine.Characters.Count - 1;
            this.EndPointer.ColumnIndex = document.ViewportColumn - 1;

            this.UpdateGeometry();
            this.RequestInvalidate();
        }

        /// <summary>
        /// ѡ��һ�����ĳ������
        /// </summary>
        /// <param name="textLine"></param>
        /// <param name="logicalRow"></param>
        /// <param name="startCharacterIndex"></param>
        /// <param name="characterCount"></param>
        public void SelectRange(VTextLine textLine, int logicalRow, int startCharacterIndex, int characterCount)
        {
            VTDocument document = this.OwnerDocument;
            VTScrollInfo scrollInfo = this.OwnerDocument.Scrollbar;

            this.StartPointer.PhysicsRow = scrollInfo.ScrollValue + logicalRow;
            this.StartPointer.CharacterIndex = startCharacterIndex;
            this.StartPointer.ColumnIndex = textLine.FindCharacterColumn(this.StartPointer.CharacterIndex);

            this.EndPointer.PhysicsRow = scrollInfo.ScrollValue + logicalRow;
            this.EndPointer.CharacterIndex = startCharacterIndex + characterCount - 1;
            this.EndPointer.ColumnIndex = textLine.FindCharacterColumn(this.EndPointer.CharacterIndex);

            this.UpdateGeometry();
            this.RequestInvalidate();
        }

        /// <summary>
        /// ѡ��ȫ�����ı��������������ı���
        /// </summary>
        public void SelectAll()
        {
            VTDocument document = this.OwnerDocument;
            VTScrollInfo scrollInfo = this.OwnerDocument.Scrollbar;
            VTHistory history = document.History;

            VTHistoryLine startHistoryLine = history.FirstLine;
            VTHistoryLine lastHistoryLine = history.LastLine;
            int firstRow = 0;
            int lastRow = history.Lines - 1;
            int lastCharacterIndex = Math.Max(0, lastHistoryLine.Characters.Count - 1);

            this.StartPointer.PhysicsRow = firstRow;
            this.StartPointer.CharacterIndex = 0;
            this.StartPointer.ColumnIndex = 0;

            this.EndPointer.PhysicsRow = lastRow;
            this.EndPointer.CharacterIndex = lastCharacterIndex;
            this.EndPointer.ColumnIndex = document.ViewportColumn - 1;

            this.UpdateGeometry();
            this.RequestInvalidate();
        }

        /// <summary>
        /// ѡ�е�ǰ��ʾ����������ı�
        /// </summary>
        public void SelectViewport()
        {
            VTDocument document = this.OwnerDocument;
            VTScrollInfo scrollInfo = document.Scrollbar;

            this.StartPointer.PhysicsRow = scrollInfo.FirstPhysicsRow;
            this.StartPointer.CharacterIndex = 0;
            this.StartPointer.ColumnIndex = 0;

            this.EndPointer.PhysicsRow = scrollInfo.LastPhysicsRow;
            this.EndPointer.CharacterIndex = scrollInfo.LastPhysicsRow;
            this.EndPointer.ColumnIndex = document.ViewportColumn - 1;

            // ������ʾѡ������
            this.UpdateGeometry();
            this.RequestInvalidate();
        }


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

            VTextPointer startPointer = this.StartPointer;
            int startRow = startPointer.PhysicsRow;
            int startColumn = startPointer.ColumnIndex;
            int startCharacterIndex = startPointer.CharacterIndex;
            VTextPointer endPointer = this.EndPointer;
            int endRow = endPointer.PhysicsRow;
            int endColumn = endPointer.ColumnIndex;
            int endCharacterIndex = endPointer.CharacterIndex;

            VTDocument document = this.OwnerDocument;
            double charWidth = document.Typeface.SpaceWidth;
            VTSize displaySize = document.Renderer.Size;

            // ��������ѡ�е���ͬһ�е����
            if (startRow == endRow)
            {
                //logger.InfoFormat("startColumn:{0}, startCharIndex:{1},endColumn:{2},endCharIndex:{3}", this.startColumn, this.startCharacterIndex, this.endColumn, this.endCharacterIndex);

                // �ҵ���Ӧ���ı���
                VTextLine textLine = document.FindLine(startPointer.PhysicsRow);
                if (textLine == null)
                {
                    // ��ѡ����һ��֮��Ȼ����б��ƶ�����Ļ���ˣ�������������
                    return;
                }

                // ����ѡ�е���ͬһ���ַ������
                if (startColumn == endColumn)
                {
                    if (startCharacterIndex > -1 && endCharacterIndex > -1)
                    {
                        VTextRange textRange = textLine.MeasureCharacter(startCharacterIndex);
                        geometries.Add(new VTRect(textRange.Left, textRange.Top, textRange.Width, textRange.Height));
                    }
                    else
                    {
                        geometries.Add(new VTRect(startColumn * charWidth, textLine.OffsetY, charWidth, textLine.Height));
                    }
                    return;
                }

                VTextPointer leftPointer, rightPointer;
                if (startColumn > endColumn)
                {
                    leftPointer = endPointer;
                    rightPointer = startPointer;
                }
                else
                {
                    leftPointer = startPointer;
                    rightPointer = endPointer;
                }

                double left = 0;
                double top = textLine.OffsetY;
                double width = 0;
                double height = textLine.Height;

                if (leftPointer.CharacterIndex == -1)
                {
                    // ���û�ַ�
                    int leftColumn = leftPointer.ColumnIndex;
                    left = leftColumn * charWidth;
                }
                else
                {
                    // ������ַ�
                    left = textLine.MeasureCharacter(leftPointer.CharacterIndex).Left;
                }

                if (rightPointer.CharacterIndex == -1)
                {
                    int leftColumn = leftPointer.ColumnIndex;
                    int rightColumn = rightPointer.ColumnIndex;
                    width = ((rightColumn - leftColumn) + 1) * charWidth;
                }
                else
                {
                    width = textLine.MeasureCharacter(rightPointer.CharacterIndex).Right - left;
                }

                //logger.InfoFormat("{0},{1},{2}", leftPointer.CharacterIndex, rightPointer.CharacterIndex, width);

                geometries.Add(new VTRect(left, top, width, height));
                return;
            }

            // ���洦��ѡ���˶��е�״̬
            VTextPointer topPointer = startPointer.PhysicsRow > endPointer.PhysicsRow ? endPointer : startPointer;
            VTextPointer bottomPointer = startPointer.PhysicsRow > endPointer.PhysicsRow ? startPointer : endPointer;

            VTextLine topLine = document.FindLine(topPointer.PhysicsRow);
            VTextLine bottomLine = document.FindLine(bottomPointer.PhysicsRow);

            if (topLine != null && bottomLine != null)
            {
                // ��ʱ˵��ѡ�е����ݶ�����Ļ��
                // �����ϱߺ��±ߵľ���

                VTRect topRect = new VTRect();
                VTRect bottomRect = new VTRect();

                if (topPointer.CharacterIndex == -1)
                {
                    // ��һ�еľ���
                    double left = topPointer.ColumnIndex * charWidth;
                    double width = displaySize.Width - left;
                    topRect = new VTRect(left, topLine.OffsetY, width, topLine.Height);
                }
                else
                {
                    VTextRange topBounds = topLine.MeasureCharacter(topPointer.CharacterIndex);

                    // ��һ�еľ���
                    topRect = new VTRect(topBounds.Left, topBounds.Top, displaySize.Width - topBounds.Left, topLine.Height);
                }

                if (bottomPointer.CharacterIndex == -1)
                {
                    // ���һ�еľ���
                    double width = (bottomPointer.ColumnIndex + 1) * charWidth;
                    bottomRect = new VTRect(0, bottomLine.OffsetY, width, bottomLine.Height);
                }
                else
                {
                    // ���һ�еľ���
                    VTextRange bottomBounds = bottomLine.MeasureCharacter(bottomPointer.CharacterIndex);
                    bottomRect = new VTRect(0, bottomLine.OffsetY, bottomBounds.Left + bottomBounds.Width, bottomLine.Height);
                }

                VTRect middleRect = new VTRect(0, topRect.Bottom, displaySize.Width, bottomRect.Top - topRect.Bottom);

                this.geometries.Add(topRect);
                this.geometries.Add(middleRect);
                this.geometries.Add(bottomRect);

                return;
            }

            if (topLine != null && bottomLine == null)
            {
                // ѡ�е�������һ���ֱ��Ƶ�������
                VTRect topRect = new VTRect();
                VTRect bottomRect = new VTRect();

                if (topPointer.CharacterIndex == -1)
                {
                    // ��һ�еľ���
                    double left = topPointer.ColumnIndex * charWidth;
                    double width = displaySize.Width - left;
                    topRect = new VTRect(left, topLine.OffsetY, width, topLine.Height);
                }
                else
                {
                    VTextRange topBounds = topLine.MeasureCharacter(topPointer.CharacterIndex);

                    // ��һ�еľ���
                    topRect = new VTRect(topBounds.Left, topBounds.Top, displaySize.Width - topBounds.Left, topLine.Height);
                }

                bottomRect = new VTRect(0, topRect.Bottom, displaySize.Width, displaySize.Height - topRect.Bottom);

                this.geometries.Add(topRect);
                this.geometries.Add(bottomRect);

                return;
            }

            if (topLine == null && bottomLine != null)
            {
                // ѡ�е�������һ���ֱ��Ƶ���Ļ������

                VTRect topRect = new VTRect();
                VTRect bottomRect = new VTRect();

                if (bottomPointer.CharacterIndex == -1)
                {
                    // ���һ�еľ���
                    double width = (bottomPointer.ColumnIndex + 1) * charWidth;
                    bottomRect = new VTRect(0, bottomLine.OffsetY, width, bottomLine.Height);
                }
                else
                {
                    // ���һ�еľ���
                    VTextRange bottomBounds = bottomLine.MeasureCharacter(bottomPointer.CharacterIndex);
                    bottomRect = new VTRect(0, bottomLine.OffsetY, bottomBounds.Left + bottomBounds.Width, bottomLine.Height);
                }

                topRect = new VTRect(0, 0, displaySize.Width, bottomRect.Top);

                this.geometries.Add(topRect);
                this.geometries.Add(bottomRect);

                return;
            }

            if (topPointer.PhysicsRow < document.Scrollbar.ScrollValue &&
                bottomPointer.PhysicsRow >= document.Scrollbar.ScrollValue + document.ViewportRow - 1)
            {
                // ѡ������ĵ�һ���ڵ�ǰ��ʾ�ĵ�һ��֮ǰ
                // ѡ����������һ���ڵ�ǰ��ʾ�����һ��֮��
                this.geometries.Add(new VTRect(0, 0, displaySize.Width, document.LastLine.Bounds.Bottom));
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
            this.OffsetY = 0;
            this.OffsetX = 0;

            this.StartPointer.CharacterIndex = -1;
            this.StartPointer.ColumnIndex = -1;
            this.StartPointer.PhysicsRow = -1;

            this.EndPointer.CharacterIndex = -1;
            this.EndPointer.ColumnIndex = -1;
            this.EndPointer.PhysicsRow = -1;

            this.geometries.Clear();

            this.SetDirtyFlags(VTDirtyFlags.RenderDirty, true);
        }

        #endregion

        #region VTElement

        protected override void OnInitialize()
        {
            this.geometries = new List<VTRect>();

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

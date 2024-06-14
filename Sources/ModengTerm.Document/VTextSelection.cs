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
    /// 存储鼠标选中的文本信息
    /// </summary>
    public class VTextSelection : VTElement
    {
        #region 类变量

        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger("VTextSelection");

        #endregion

        #region 实例变量

        private List<VTRect> geometries;
        private VTColor backColor;

        #endregion

        #region 属性

        public override DrawingObjectTypes Type => DrawingObjectTypes.Selection;

        /// <summary>
        /// 指示当前选中的内容是否为空
        /// </summary>
        public bool IsEmpty { get { return this.StartPointer.ColumnIndex < 0 || this.EndPointer.ColumnIndex < 0; } }

        public VTextPointer StartPointer { get; set; }

        public VTextPointer EndPointer { get; set; }

        /// <summary>
        /// 选中区域的颜色
        /// </summary>
        public string Color { get; set; }

        #endregion

        #region 构造方法

        public VTextSelection(VTDocument ownerDocument) :
            base(ownerDocument)
        {
        }

        #endregion

        #region 实例方法

        #endregion

        #region 公开接口

        /// <summary>
        /// 选中某一行
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
        /// 选中一行里的某个区域
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
        /// 选中全部的文本（包含滚动的文本）
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
        /// 选中当前显示区域的所有文本
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

            // 立即显示选中区域
            this.UpdateGeometry();
            this.RequestInvalidate();
        }


        /// <summary>
        /// 根据当前的TextPointer信息更新选中区域的形状
        /// 选中区域需要在下面几个时机更新：
        /// 1. 在当前页面选中部分区域要更新
        /// 2. 当前页面存在选中区域并且滚动了滚动条之后也需要更新
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

            // 单独处理选中的是同一行的情况
            if (startRow == endRow)
            {
                //logger.InfoFormat("startColumn:{0}, startCharIndex:{1},endColumn:{2},endCharIndex:{3}", this.startColumn, this.startCharacterIndex, this.endColumn, this.endCharacterIndex);

                // 找到对应的文本行
                VTextLine textLine = document.FindLine(startPointer.PhysicsRow);
                if (textLine == null)
                {
                    // 当选中了一行之后，然后该行被移动到屏幕外了，会出现这种情况
                    return;
                }

                // 处理选中的是同一个字符的情况
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
                    // 左边没字符
                    int leftColumn = leftPointer.ColumnIndex;
                    left = leftColumn * charWidth;
                }
                else
                {
                    // 左边有字符
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

            // 下面处理选中了多行的状态
            VTextPointer topPointer = startPointer.PhysicsRow > endPointer.PhysicsRow ? endPointer : startPointer;
            VTextPointer bottomPointer = startPointer.PhysicsRow > endPointer.PhysicsRow ? startPointer : endPointer;

            VTextLine topLine = document.FindLine(topPointer.PhysicsRow);
            VTextLine bottomLine = document.FindLine(bottomPointer.PhysicsRow);

            if (topLine != null && bottomLine != null)
            {
                // 此时说明选中的内容都在屏幕里
                // 构建上边和下边的矩形

                VTRect topRect = new VTRect();
                VTRect bottomRect = new VTRect();

                if (topPointer.CharacterIndex == -1)
                {
                    // 第一行的矩形
                    double left = topPointer.ColumnIndex * charWidth;
                    double width = displaySize.Width - left;
                    topRect = new VTRect(left, topLine.OffsetY, width, topLine.Height);
                }
                else
                {
                    VTextRange topBounds = topLine.MeasureCharacter(topPointer.CharacterIndex);

                    // 第一行的矩形
                    topRect = new VTRect(topBounds.Left, topBounds.Top, displaySize.Width - topBounds.Left, topLine.Height);
                }

                if (bottomPointer.CharacterIndex == -1)
                {
                    // 最后一行的矩形
                    double width = (bottomPointer.ColumnIndex + 1) * charWidth;
                    bottomRect = new VTRect(0, bottomLine.OffsetY, width, bottomLine.Height);
                }
                else
                {
                    // 最后一行的矩形
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
                // 选中的内容有一部分被移到下面了
                VTRect topRect = new VTRect();
                VTRect bottomRect = new VTRect();

                if (topPointer.CharacterIndex == -1)
                {
                    // 第一行的矩形
                    double left = topPointer.ColumnIndex * charWidth;
                    double width = displaySize.Width - left;
                    topRect = new VTRect(left, topLine.OffsetY, width, topLine.Height);
                }
                else
                {
                    VTextRange topBounds = topLine.MeasureCharacter(topPointer.CharacterIndex);

                    // 第一行的矩形
                    topRect = new VTRect(topBounds.Left, topBounds.Top, displaySize.Width - topBounds.Left, topLine.Height);
                }

                bottomRect = new VTRect(0, topRect.Bottom, displaySize.Width, displaySize.Height - topRect.Bottom);

                this.geometries.Add(topRect);
                this.geometries.Add(bottomRect);

                return;
            }

            if (topLine == null && bottomLine != null)
            {
                // 选中的内容有一部分被移到屏幕上面了

                VTRect topRect = new VTRect();
                VTRect bottomRect = new VTRect();

                if (bottomPointer.CharacterIndex == -1)
                {
                    // 最后一行的矩形
                    double width = (bottomPointer.ColumnIndex + 1) * charWidth;
                    bottomRect = new VTRect(0, bottomLine.OffsetY, width, bottomLine.Height);
                }
                else
                {
                    // 最后一行的矩形
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
                // 选中区域的第一行在当前显示的第一行之前
                // 选中区域的最后一行在当前显示的最后一行之后
                this.geometries.Add(new VTRect(0, 0, displaySize.Width, document.LastLine.Bounds.Bottom));
                return;
            }

            // 有选中区域后，并且选中区域不存在当前页面里
            // 什么都不做
        }

        /// <summary>
        /// 清除选中的区域
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

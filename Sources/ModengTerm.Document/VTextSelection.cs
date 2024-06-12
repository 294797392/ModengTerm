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

        private int startRow;
        private int endRow;
        private int startColumn;
        private int endColumn;
        private List<VTRect> geometries;
        private VTColor backColor;

        #endregion

        #region 属性

        public override DrawingObjectTypes Type => DrawingObjectTypes.Selection;

        /// <summary>
        /// 指示当前选中的内容是否为空
        /// </summary>
        public bool IsEmpty { get { return startColumn < 0 || endColumn < 0; } }

        /// <summary>
        /// 选中区域的第一行的物理行号
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
        /// 选中区域的第一行的第一个字符
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
        /// 选中区域的最后一行的物理行号
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
        /// 选中区域的最后一行的最后一个字符
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

            VTextPointer Start = new VTextPointer(startRow, startColumn);
            VTextPointer End = new VTextPointer(endRow, endColumn);

            VTDocument document = this.OwnerDocument;
            VTSize container = document.Renderer.Size;

            // 单独处理选中的是同一行的情况
            if (Start.PhysicsRow == End.PhysicsRow)
            {
                // 找到对应的文本行
                VTextLine textLine = document.FindLine(Start.PhysicsRow);
                if (textLine == null)
                {
                    // 当选中了一行之后，然后该行被移动到屏幕外了，会出现这种情况
                    return;
                }

                // 单独处理选中的是一个字符的情况
                if (Start.CharacterIndex == End.CharacterIndex)
                {
                    // 选中的是一个字符
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

            // 下面处理选中了多行的状态
            VTextPointer topPointer = Start.PhysicsRow > End.PhysicsRow ? End : Start;
            VTextPointer bottomPointer = Start.PhysicsRow > End.PhysicsRow ? Start : End;

            VTextLine topLine = document.FindLine(topPointer.PhysicsRow);
            VTextLine bottomLine = document.FindLine(bottomPointer.PhysicsRow);

            if (topLine != null && bottomLine != null)
            {
                // 此时说明选中的内容都在屏幕里
                // 构建上边和下边的矩形
                VTextRange topBounds = topLine.MeasureCharacter(topPointer.CharacterIndex);
                VTextRange bottomBounds = bottomLine.MeasureCharacter(bottomPointer.CharacterIndex);

                // 第一行的矩形
                geometries.Add(new VTRect(topBounds.OffsetX, topBounds.OffsetY, container.Width - topBounds.OffsetX, topLine.Height));

                // 中间的矩形
                double y = topLine.OffsetY + topBounds.Height;
                double height = bottomLine.OffsetY - (topLine.OffsetY + topBounds.Height);
                geometries.Add(new VTRect(0, y, container.Width, height));

                // 最后一行的矩形
                geometries.Add(new VTRect(0, bottomLine.OffsetY, bottomBounds.OffsetX + bottomBounds.Width, bottomLine.Height));
                return;
            }

            if (topLine != null && bottomLine == null)
            {
                // 选中的内容有一部分被移到屏幕外了，滚动条往上移动
                VTextRange topBounds = topLine.MeasureCharacter(topPointer.CharacterIndex);

                // 第一行的矩形
                geometries.Add(new VTRect(topBounds.OffsetX, topLine.OffsetY, container.Width - topBounds.OffsetX, topLine.Height));

                // 剩下的矩形
                double height = document.LastLine.Bounds.Bottom - topLine.Bounds.Bottom;
                geometries.Add(new VTRect(0, topLine.Bounds.Bottom, container.Width, height));
                return;
            }

            if (topLine == null && bottomLine != null)
            {
                // 选中的内容有一部分被移到屏幕外了，滚动条往下移动
                VTextRange bottomBounds = bottomLine.MeasureCharacter(bottomPointer.CharacterIndex);

                // 
                geometries.Add(new VTRect(0, 0, container.Width, bottomBounds.OffsetY));

                // 最后一行的矩形
                geometries.Add(new VTRect(0, bottomBounds.OffsetY, bottomBounds.OffsetX + bottomBounds.Width, bottomLine.Height));
                return;
            }

            if (topPointer.PhysicsRow < document.Scrollbar.ScrollValue &&
                bottomPointer.PhysicsRow >= document.Scrollbar.ScrollValue + document.ViewportRow - 1)
            {
                // 选中区域的第一行在当前显示的第一行之前
                // 选中区域的最后一行在当前显示的最后一行之后
                this.geometries.Add(new VTRect(0, 0, container.Width, document.LastLine.Bounds.Bottom));
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

                // 注意要处理鼠标从右向左选中的情况
                // 如果鼠标是从右向左进行选中，那么Start就是Selection的右边，End就是Selection的左边
                startIndex = Math.Min(Start.CharacterIndex, End.CharacterIndex);
                endIndex = Math.Max(Start.CharacterIndex, End.CharacterIndex);
            }
            else
            {
                // 要考虑鼠标从下往上选中的情况
                // 如果鼠标从下往上选中，那么此时下面的VTextPointer是起始，上面的VTextPointer是结束
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

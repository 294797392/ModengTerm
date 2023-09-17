using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTerminal.Document.Rendering;

namespace XTerminal.Document
{
    /// <summary>
    /// 存储鼠标选中的文本信息
    /// </summary>
    public class VTextSelection : VTDocumentElement
    {
        #region 类变量

        private static readonly VTRect NullRect = new VTRect(-1, -1, -1, -1);

        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger("VTextSelection");

        #endregion

        #region 实例变量

        private StringBuilder textBuilder;

        private VTDocument document;
        private int startPhysicsRow;
        private int endPhysicsRow;
        private VTRect container;

        #endregion

        #region 属性

        public override VTDocumentElements Type => VTDocumentElements.SelectionRange;

        /// <summary>
        /// 选中内容的几何表示形式
        /// </summary>
        public List<VTRect> Geometry { get; private set; }

        /// <summary>
        /// 所选内容的开始位置
        /// </summary>
        public VTextPointer Start { get; private set; }

        /// <summary>
        /// 所选内容的结束位置
        /// </summary>
        public VTextPointer End { get; private set; }

        /// <summary>
        /// 指示当前选中的内容是否为空
        /// </summary>
        public bool IsEmpty { get { return this.Start.CharacterIndex < 0 || this.End.CharacterIndex < 0; } }

        #endregion

        #region 构造方法

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
        /// 清除选中的区域
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
        /// 设置文本选中的范围
        /// </summary>
        /// <param name="document">要在哪个文档上设置选中范围</param>
        /// <param name="container">Canvas相对于电脑显示器屏幕的位置</param>
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
        /// 根据当前的TextPointer信息更新选中区域的形状
        /// 虽然TextPointer的数值是一样的，但是当移动了滚动条之后，选中区域的显示就不一样了
        /// </summary>
        /// <param name="document"></param>
        /// <param name="container"></param>
        public void UpdateRange(VTDocument document, VTRect container)
        {
            this.Geometry.Clear();

            // 单独处理选中的是同一行的情况
            if (this.Start.PhysicsRow == this.End.PhysicsRow)
            {
                // 找到对应的文本行
                VTextLine textLine = document.FindLine(this.Start.PhysicsRow);
                if (textLine == null)
                {
                    // 当选中了一行之后，然后该行被移动到屏幕外了，会出现这种情况
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

            // 下面处理选中了多行的状态
            VTextPointer topPointer = this.Start.PhysicsRow > this.End.PhysicsRow ? this.End : this.Start;
            VTextPointer bottomPointer = this.Start.PhysicsRow > this.End.PhysicsRow ? this.Start : this.End;

            VTextLine topLine = document.FindLine(topPointer.PhysicsRow);
            VTextLine bottomLine = document.FindLine(bottomPointer.PhysicsRow);

            if (topLine != null && bottomLine != null)
            {
                // 此时说明选中的内容都在屏幕里
                // 构建上边和下边的矩形
                VTRect topBounds = topLine.MeasureCharacter(topPointer.CharacterIndex);
                VTRect bottomBounds = bottomLine.MeasureCharacter(bottomPointer.CharacterIndex);

                // 第一行的矩形
                this.Geometry.Add(new VTRect(topBounds.X, topLine.OffsetY, container.Width - topBounds.X, topLine.Height));

                // 中间的矩形
                double y = topLine.OffsetY + topBounds.Height;
                double height = bottomLine.OffsetY - (topLine.OffsetY + topBounds.Height);
                this.Geometry.Add(new VTRect(0, y, container.Width, height));

                // 最后一行的矩形
                this.Geometry.Add(new VTRect(0, bottomLine.OffsetY, bottomBounds.Right, bottomLine.Height));
                return;
            }

            if (topLine != null && bottomLine == null)
            {
                // 选中的内容有一部分被移到屏幕外了，滚动条往上移动
                VTRect topBounds = topLine.MeasureCharacter(topPointer.CharacterIndex);

                // 第一行的矩形
                this.Geometry.Add(new VTRect(topBounds.X, topLine.OffsetY, container.Width - topBounds.X, topLine.Height));

                // 剩下的矩形
                double height = document.LastLine.Bounds.Bottom - topLine.Bounds.Bottom;
                this.Geometry.Add(new VTRect(0, topLine.Bounds.Bottom, container.Width, height));
                return;
            }

            if (topLine == null && bottomLine != null)
            {
                // 选中的内容有一部分被移到屏幕外了，滚动条往下移动
                VTRect bottomBounds = bottomLine.MeasureCharacter(bottomPointer.CharacterIndex);

                // 最后一行的矩形
                this.Geometry.Add(new VTRect(0, bottomLine.OffsetY, bottomBounds.Right, bottomLine.Height));

                // 剩下的矩形
                this.Geometry.Add(new VTRect(0, 0, container.Width, bottomLine.OffsetY));
                return;
            }

            if (topPointer.PhysicsRow < document.FirstLine.PhysicsRow &&
                bottomPointer.PhysicsRow > document.LastLine.PhysicsRow)
            {
                // 这种情况下说明当前显示的内容被全部选择了
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

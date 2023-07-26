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
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger("VTextSelection");

        private StringBuilder textBuilder;

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
        public bool IsEmpty { get { return this.Geometry.Count == 0; } }

        public VTextSelection()
        {
            this.Geometry = new List<VTRect>();
            this.Start = new VTextPointer();
            this.End = new VTextPointer();
            this.textBuilder = new StringBuilder();
        }

        /// <summary>
        /// 重置选中的状态
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
        /// 通过历史行数据获取选中的文本内容
        /// </summary>
        /// <param name="historyLines">历史行列表</param>
        /// <param name="selection">选中的文本信息</param>
        /// <returns></returns>
        public string GetText(Dictionary<int, VTHistoryLine> historyLines)
        {
            if (this.IsEmpty)
            {
                return string.Empty;
            }

            // 找到选中的第一行和最后一行的信息
            VTHistoryLine firstLine, lastLine;
            if (!historyLines.TryGetValue(this.Start.PhysicsRow, out firstLine) ||
                !historyLines.TryGetValue(this.End.PhysicsRow, out lastLine))
            {
                logger.ErrorFormat("获取选中的文本内容失败, 第一行或最后一行为空");
                return string.Empty;
            }

            // 当前只选中了一行
            if (firstLine == lastLine)
            {
                return firstLine.Text.Substring(this.Start.CharacterIndex, this.End.CharacterIndex - this.Start.CharacterIndex);
            }

            // 当前选中了多行，那么每行的数据都要复制

            this.textBuilder.Clear();

            VTHistoryLine currentLine = firstLine;
            while (currentLine != null)
            {
                if (currentLine == firstLine)
                {
                    // 第一行
                    this.textBuilder.AppendLine(currentLine.Text.Substring(this.Start.CharacterIndex));
                }
                else if (currentLine == lastLine)
                {
                    // 最后一行
                    this.textBuilder.AppendLine(currentLine.Text.Substring(0, this.End.CharacterIndex + 1));
                    break;
                }
                else
                {
                    // 中间的行
                    this.textBuilder.AppendLine(currentLine.Text);
                }

                currentLine = currentLine.NextLine;
            }

            return this.textBuilder.ToString();
        }

        /// <summary>
        /// 构建选中内容的几何图形
        /// </summary>
        public void BuildGeometry()
        {
            this.Geometry.Clear();

            VTextPointer startPointer = this.Start;
            VTextPointer endPointer = this.End;

            // 判断起始位置或者结束位置是否在Surface外

            // 先算鼠标的移动方向
            TextPointerPositions pointerPosition = VTextSelectionHelper.GetTextPointerPosition(startPointer, endPointer);

            switch (pointerPosition)
            {
                case TextPointerPositions.Original:
                    {
                        break;
                    }

                // 这两个是鼠标在同一行上移动
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

                // 其他的是鼠标上下移动
                default:
                    {
                        // 构建上边和下边的矩形
                        VTextPointer topPointer = startPointer.PhysicsRow < endPointer.PhysicsRow ? startPointer : endPointer;
                        VTextPointer bottomPointer = startPointer.PhysicsRow < endPointer.PhysicsRow ? endPointer : startPointer;

                        //logger.FatalFormat("top = {0}, bottom = {1}", topPointer.Row, bottomPointer.Row);

                        // 相对于Panel的起始选中边界框和结束选中的边界框
                        VTRect topBounds = topPointer.CharacterBounds;
                        VTRect bottomBounds = bottomPointer.CharacterBounds;

                        // 第一行的矩形
                        this.Geometry.Add(new VTRect(topBounds.X, topPointer.OffsetY, 9999, topBounds.Height));

                        // 中间的矩形
                        double y = topPointer.OffsetY + topBounds.Height;
                        double height = bottomPointer.OffsetY - (topPointer.OffsetY + topBounds.Height);
                        this.Geometry.Add(new VTRect(0, y, 9999, height));

                        // 最后一行的矩形
                        this.Geometry.Add(new VTRect(0, bottomPointer.OffsetY, bottomBounds.Right, bottomBounds.Height));
                        break;
                    }
            }
        }
    }
}

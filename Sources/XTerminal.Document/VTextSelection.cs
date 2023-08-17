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

        /// <summary>
        /// 是否需要重绘
        /// </summary>
        public bool IsDirty { get; private set; }

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

        public VTextSelection()
        {
            this.Geometry = new List<VTRect>();
            this.Start = new VTextPointer();
            this.End = new VTextPointer();
            this.textBuilder = new StringBuilder();

            this.Start.CharacterIndex = -1;
            this.End.CharacterIndex = -1;
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

            this.End.CharacterIndex = -1;
            this.End.PhysicsRow = -1;
        }

        /// <summary>
        /// 通过历史行数据获取选中的文本内容
        /// </summary>
        /// <param name="historyLines">历史行列表</param>
        /// <param name="selection">选中的文本信息</param>
        /// <returns></returns>
        public string GetText(Dictionary<int, VTHistoryLine> historyLines)
        {
            // 找到选中的起始行和结束行的信息
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
                // 注意要处理鼠标从右向左选中的情况
                // 如果鼠标是从右向左进行选中，那么Start就是Selection的右边，End就是Selection的左边
                int startCharacterIndex = Math.Min(this.Start.CharacterIndex, this.End.CharacterIndex);
                int endCharacterIndex = Math.Max(this.Start.CharacterIndex, this.End.CharacterIndex);
                return firstLine.Text.Substring(startCharacterIndex, endCharacterIndex - startCharacterIndex + 1);
            }

            // 清空之前选中的数据
            this.textBuilder.Clear();

            // 鼠标从下往上选中，需要单独处理
            bool reverse = this.Start.PhysicsRow > this.End.PhysicsRow;

            // 当前选中了多行，那么每行的数据都要复制
            VTHistoryLine currentLine = reverse ? lastLine : firstLine;
            while (currentLine != null)
            {
                if (currentLine == firstLine)
                {
                    if (reverse)
                    {
                        // 如果是鼠标从下往上选中，那么就是最后一行
                        this.textBuilder.AppendLine(currentLine.Text.Substring(0, this.Start.CharacterIndex + 1));
                        break;
                    }
                    else
                    {
                        // 第一行
                        this.textBuilder.AppendLine(currentLine.Text.Substring(this.Start.CharacterIndex));
                    }
                }
                else if (currentLine == lastLine)
                {
                    if (reverse)
                    {
                        // 如果是鼠标从下往上选中，那么就是第一行
                        this.textBuilder.AppendLine(currentLine.Text.Substring(this.End.CharacterIndex));
                    }
                    else
                    {
                        // 最后一行
                        this.textBuilder.AppendLine(currentLine.Text.Substring(0, this.End.CharacterIndex + 1));
                        break;
                    }
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

        public void SetDirty(bool isDirty)
        {
            if (this.IsDirty != isDirty)
            {
                this.IsDirty = isDirty;
            }
        }

        #region 实例方法

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XTerminal.Document
{
    /// <summary>
    /// 用来存储历史的行记录
    /// </summary>
    public class VTHistoryLine
    {
        /// <summary>
        /// 文本的测量信息
        /// </summary>
        public VTElementMetrics Metrics { get; set; }

        /// <summary>
        /// 行索引，从0开始
        /// </summary>
        public int Row { get; private set; }

        /// <summary>
        /// 该行的文本
        /// </summary>
        public string Text { get; private set; }

        /// <summary>
        /// 该行文本的样式
        /// </summary>
        public List<VTextAttribute> TextAttributes { get; private set; }

        /// <summary>
        /// 上一行
        /// </summary>
        public VTHistoryLine PreviousLine { get; internal set; }

        /// <summary>
        /// 下一行
        /// </summary>
        public VTHistoryLine NextLine { get; internal set; }

        public void Update(VTextLine line)
        {
            this.Metrics = line.Metrics;
            this.Text = line.GetText();
        }

        /// <summary>
        /// 从VTextLine创建一个VTHistoryLine
        /// </summary>
        /// <param name="fromLine"></param>
        /// <returns></returns>
        public static VTHistoryLine Create(int row, VTHistoryLine previousLine, VTextLine fromLine)
        {
            VTHistoryLine historyLine = new VTHistoryLine()
            {
                Metrics =  fromLine.Metrics,
                Text = fromLine.GetText(),
                TextAttributes = fromLine.Attributes.ToList(),
                Row = row,
                PreviousLine = previousLine
            };

            if (previousLine != null)
            {
                previousLine.NextLine = historyLine;
            }

            return historyLine;
        }
    }
}

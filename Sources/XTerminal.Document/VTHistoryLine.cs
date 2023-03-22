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
        /// 从VTextLine创建一个VTHistoryLine
        /// </summary>
        /// <param name="fromLine"></param>
        /// <returns></returns>
        public static VTHistoryLine Create(int row, VTextLine fromLine)
        {
            return new VTHistoryLine()
            {
                Metrics =  fromLine.Metrics,
                Text = fromLine.GetText(),
                TextAttributes = fromLine.Attributes.ToList(),
                Row = row,
            };
        }
    }
}

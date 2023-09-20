using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTerminal.Document;

namespace ModengTerm.Terminal.Document
{
    /// <summary>
    /// 存储查找结果
    /// </summary>
    public class VTFindResult
    {
        /// <summary>
        /// 存储包含关键字的行
        /// </summary>
        public List<VTHistoryLine> HistoryLines { get; private set; }

        public VTFindResult()
        {
            this.HistoryLines = new List<VTHistoryLine>();
        }
    }
}

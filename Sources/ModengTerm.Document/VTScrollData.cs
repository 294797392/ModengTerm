using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Document
{
    /// <summary>
    /// 滚动事件参数
    /// 当滚动滚动条的时候触发
    /// </summary>
    public class VTScrollData
    {
        /// <summary>
        /// 滚动之前的滚动条的值
        /// </summary>
        public int OldScroll { get; set; }

        /// <summary>
        /// 滚动之后的滚动条的值
        /// </summary>
        public int NewScroll { get; set; }

        /// <summary>
        /// 被删除的行
        /// </summary>
        public List<VTHistoryLine> RemovedLines { get; set; }

        /// <summary>
        /// 新增加的行
        /// </summary>
        public List<VTHistoryLine> AddedLines { get; set; }

        public VTScrollData()
        {
            RemovedLines = new List<VTHistoryLine>();
            AddedLines = new List<VTHistoryLine>();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal.Document
{
    public class VTMatches
    {
        /// <summary>
        /// 关键字长度
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// 匹配到的关键字在该行中的索引位置
        /// </summary>
        public int Index { get; set; }

        public VTMatches(int length, int index)
        {
            this.Length = length;
            this.Index = index;
        }
    }
}

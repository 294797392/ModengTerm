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

        /// <summary>
        /// 匹配的文本对应的格式化文本信息，动态生成
        /// 渲染的时候会用到
        /// </summary>
        public VTFormattedText FormattedText { get; set; }

        public VTMatches(int length, int index)
        {
            this.Length = length;
            this.Index = index;
        }
    }
}

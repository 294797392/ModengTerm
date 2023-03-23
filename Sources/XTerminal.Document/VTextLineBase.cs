using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XTerminal.Document
{
    public interface VTextLineBase
    {
        /// <summary>
        /// 文本行的属性
        /// </summary>
        List<VTextAttribute> Attributes { get; }

        /// <summary>
        /// 文本行的测量信息
        /// </summary>
        VTElementMetrics Metrics { get; }

        /// <summary>
        /// 获取该文本行的内容
        /// </summary>
        /// <returns></returns>
        string Text { get; }
    }
}

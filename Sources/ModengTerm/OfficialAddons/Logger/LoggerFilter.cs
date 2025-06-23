using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal.Loggering
{
    public abstract class LoggerFilter
    {
        /// <summary>
        /// 过滤关键字
        /// </summary>
        public string FilterText { get; set; }

        /// <summary>
        /// 过滤器类型
        /// </summary>
        public abstract FilterTypeEnum Type { get; }

        /// <summary>
        /// 对文本进行过滤
        /// </summary>
        /// <returns>
        /// true表示可以保存
        /// false表示不能保存
        /// </returns>
        public abstract bool Filter(string text);
    }
}

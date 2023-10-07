using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal.Loggering
{
    /// <summary>
    /// 日志内容过滤类型
    /// </summary>
    public enum FilterTypeEnum
    {
        /// <summary>
        /// 不过滤
        /// </summary>
        None,

        /// <summary>
        /// 关键字过滤
        /// </summary>
        PlainText,

        /// <summary>
        /// 使用正则表达式过滤
        /// </summary>
        Regexp
    }
}
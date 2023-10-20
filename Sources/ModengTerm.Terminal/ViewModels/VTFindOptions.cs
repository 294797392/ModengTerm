using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal.ViewModels
{
    /// <summary>
    /// 查找范围
    /// </summary>
    public enum FindScopes
    {
        /// <summary>
        /// 只查找当前显示的内容
        /// </summary>
        Document,

        /// <summary>
        /// 查找所有内容
        /// </summary>
        All
    }

    /// <summary>
    /// 从哪里开始查找
    /// </summary>
    public enum FindStartups
    {
        /// <summary>
        /// 从第一行开始向下查找
        /// </summary>
        FromBegin,

        /// <summary>
        /// 从最后一行开始向上查找
        /// </summary>
        FromEnd,
    }
}

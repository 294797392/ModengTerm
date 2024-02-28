using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Document.Enumerations
{
    /// <summary>
    /// 定义滚动操作的选项
    /// </summary>
    public enum ScrollOptions
    {
        /// <summary>
        /// 把要滚动到的行放在最上面
        /// </summary>
        ScrollToTop,

        /// <summary>
        /// 把要滚动到的行放在最低面
        /// </summary>
        ScrollToBottom,

        /// <summary>
        /// 把要滚动到的行放在最中间
        /// </summary>
        ScrollToMiddle,
    }
}

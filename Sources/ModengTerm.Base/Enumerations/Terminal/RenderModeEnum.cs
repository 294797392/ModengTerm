using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Base.Enumerations.Terminal
{
    /// <summary>
    /// 指定终端数据的渲染方式
    /// </summary>
    public enum RenderModeEnum
    {
        /// <summary>
        /// 默认渲染为终端数据
        /// </summary>
        Default,

        /// <summary>
        /// 渲染为类似于hexdump的16进制数据
        /// </summary>
        Hexdump
    }
}

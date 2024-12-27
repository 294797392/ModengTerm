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
        /// 按照终端控制序列解码并渲染
        /// </summary>
        Default,

        /// <summary>
        /// 解码为16进制渲染
        /// </summary>
        Hexdump,

        /// <summary>
        /// 解码为字符串渲染
        /// </summary>
        String
    }
}

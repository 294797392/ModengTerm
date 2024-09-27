using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal.Enumerations
{
    public enum PortForwardStatusEnum
    {
        /// <summary>
        /// 已打开
        /// </summary>
        Opened,

        /// <summary>
        /// 已关闭
        /// </summary>
        Closed,

        /// <summary>
        /// 打开失败
        /// </summary>
        OpenError
    }
}

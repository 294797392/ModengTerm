using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Base.Enumerations.Ssh
{
    /// <summary>
    /// 指定端口转发类型
    /// </summary>
    public enum PortForwardTypeEnum
    {
        /// <summary>
        /// 本地端口转发
        /// </summary>
        Local,

        /// <summary>
        /// 远程端口转发
        /// </summary>
        Remote,

        /// <summary>
        /// 动态端口转发
        /// </summary>
        Dynamic
    }
}

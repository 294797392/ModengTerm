using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal.Enumerations
{
    /// <summary>
    /// 定义用户和主机之间的交互状态
    /// </summary>
    public enum InteractionStateEnum
    {
        /// <summary>
        /// 用户输入数据
        /// </summary>
        UserInput,

        /// <summary>
        /// 多次收到数据
        /// </summary>
        Receive
    }
}

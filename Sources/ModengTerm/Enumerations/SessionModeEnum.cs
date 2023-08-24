using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XTerminal.Enumerations
{
    /// <summary>
    /// 终端模式
    /// </summary>
    public enum SessionModeEnum
    {
        /// <summary>
        /// 禁用输入
        /// </summary>
        DisbaleInput,

        /// <summary>
        /// 画图模式，可以在终端上画图
        /// </summary>
        Drawing,
    }
}

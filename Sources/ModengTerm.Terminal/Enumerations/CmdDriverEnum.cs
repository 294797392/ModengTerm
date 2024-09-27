using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal.Enumerations
{
    /// <summary>
    /// 表示命令行驱动程序
    /// </summary>
    public enum CmdDriverEnum
    {
        /// <summary>
        /// 使用winpty驱动命令行
        /// </summary>
        winpty,

        /// <summary>
        /// 使用Windows10的PseudoConsoleApi驱动命令行
        /// </summary>
        Win10PseudoConsoleApi
    }
}

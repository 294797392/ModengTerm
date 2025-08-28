using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Base.Enumerations.Terminal
{
    /// <summary>
    /// 表示实现Win32控制台的引擎
    /// </summary>
    public enum Win32ConsoleEngineEnum
    {
        /// <summary>
        /// 自动选择
        /// 如果操作系统版本高于等于Win10，那么使用PseudoConsole，否则使用winpty
        /// </summary>
        Auto,

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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XTerminal.Base
{
    /// <summary>
    /// 提供一些记录日志的工具函数
    /// </summary>
    public static class VTDebug
    {
        /// <summary>
        /// 是否启用调试模式
        /// </summary>
        public static bool Enabled { get; set; }

        /// <summary>
        /// 记录输入日志
        /// </summary>
        /// <param name="bytes"></param>
        public static void WriteInput(byte[] bytes)
        {
            if (!Enabled)
            {
                return;
            }
        }

        /// <summary>
        /// 记录输出日志
        /// </summary>
        /// <param name="output"></param>
        public static void WriteOutput(byte[] output)
        {
            if (!Enabled)
            {
                return;
            }
        }

        public static void WriteAction()
        {

        }
    }
}
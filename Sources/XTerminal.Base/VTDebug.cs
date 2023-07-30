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
        private static log4net.ILog logger = log4net.LogManager.GetLogger("VTDebug");

        /// <summary>
        /// 是否启用调试模式
        /// </summary>
        public static bool Enabled { get; set; }

        static VTDebug()
        {
            Enabled = true;
        }

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

        public static void WriteAction(string format, params object[] param)
        {
            if (!Enabled)
            {
                return;
            }

            logger.InfoFormat("Action:{0}", string.Format(format, param));
        }
    }
}
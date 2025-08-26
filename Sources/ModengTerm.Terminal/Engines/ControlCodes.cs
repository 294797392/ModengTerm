using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal.Engines
{
    /// <summary>
    /// 定义SSH会话支持的命令
    /// </summary>
    public static class SSHControlCodes
    {
        /// <summary>
        /// 获取端口转发状态
        /// </summary>
        public const int GetForwardPortStates = 1;

        /// <summary>
        /// 启动全部端口转发
        /// </summary>
        public const int StartAllPortForward = 2;

        /// <summary>
        /// 停止全部端口转发
        /// </summary>
        public const int StopAllPortForward = 3;

        /// <summary>
        /// 启动指定的端口转发
        /// </summary>
        public const int StartPortForward = 4;

        /// <summary>
        /// 停止指定的端口转发
        /// </summary>
        public const int StopPortForward = 5;
    }
}

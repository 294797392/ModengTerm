using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.ViewModel.Ftp
{
    /// <summary>
    /// 定义客户端和服务器枚举
    /// </summary>
    public enum FtpRoleEnum
    {
        /// <summary>
        /// 服务器
        /// </summary>
        Remote,

        /// <summary>
        /// 客户端
        /// 客户端只运行在Windows上
        /// 如果使用GTK跨平台开发，那么需要使用宏定义解决Windows和Linux环境下的路径问题
        /// </summary>
        Local,
    }
}

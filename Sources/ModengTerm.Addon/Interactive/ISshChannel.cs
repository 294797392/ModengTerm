using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Addon.Interactive
{
    /// <summary>
    /// 定义Ssh客户端可以执行的动作
    /// </summary>
    public interface ISshChannel
    {
        /// <summary>
        /// 在远程主机上执行一个脚本
        /// </summary>
        /// <param name="script">要执行的脚本</param>
        /// <returns>执行脚本的返回值</returns>
        string ExecuteScript(string script);
    }
}

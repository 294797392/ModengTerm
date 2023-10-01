using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XTerminal.Base.Enumerations
{
    /// <summary>
    /// 连接SSH服务器的身份验证方式
    /// </summary>
    public enum SSHAuthTypeEnum
    {
        /// <summary>
        /// 密码验证
        /// </summary>
        Password,

        /// <summary>
        /// 不需要验证，直接输入IP地址和端口号就可以
        /// </summary>
        None,

        /// <summary>
        /// 私钥验证
        /// </summary>
        PrivateKey
    }

}

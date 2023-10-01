using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XTerminal.Base.Enumerations
{
    public enum SFTPAuthTypeEnum
    {
        /// <summary>
        /// 不需要验证，直接输入IP地址和端口号就可以
        /// </summary>
        None,

        /// <summary>
        /// 密码验证
        /// </summary>
        Password,

        /// <summary>
        /// 私钥验证
        /// </summary>
        PrivateKey
    }
}

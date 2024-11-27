using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Base.Enumerations
{
    /// <summary>
    /// Adb登录方式
    /// </summary>
    public enum AdbLoginTypeEnum
    {
        /// <summary>
        /// 用户名和密码
        /// </summary>
        UserNamePassword,

        /// <summary>
        /// 只是用密码登录
        /// </summary>
        Password,

        /// <summary>
        /// 不需要登录
        /// </summary>
        None
    }
}

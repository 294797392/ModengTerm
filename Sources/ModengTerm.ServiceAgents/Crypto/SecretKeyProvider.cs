using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.ServiceAgents.Crypto
{
    public abstract class SecretKey
    {
        
    }

    /// <summary>
    /// 提供数据加密密钥
    /// </summary>
    public abstract class SecretKeyProvider
    {
        /// <summary>
        /// 获取加密密钥
        /// </summary>
        /// <returns></returns>
        public abstract SecretKey GetSecretKey();
    }
}

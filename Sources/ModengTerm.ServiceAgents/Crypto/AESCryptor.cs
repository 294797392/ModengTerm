using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.ServiceAgents.Crypto
{
    /// <summary>
    /// 存储AES密钥信息
    /// </summary>
    public class AESecretKey : SecretKey
    {
        /// <summary>
        /// 使用AES256加密的密钥，32个字节
        /// </summary>
        public byte[] Key { get; set; }
    }

    /// <summary>
    /// 对本地数据进行加密
    /// </summary>
    public class AESCryptor : Cryptor
    {
        protected override void OnInitialize()
        {
        }

        protected override void OnRelease()
        {
        }

        public override T Decrypt<T>(string s)
        {
            throw new NotImplementedException();
        }

        public override string Encrypt(object o)
        {
            throw new NotImplementedException();
        }
    }
}

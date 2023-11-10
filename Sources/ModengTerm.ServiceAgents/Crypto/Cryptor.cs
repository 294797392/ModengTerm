using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.ServiceAgents.Crypto
{
    /// <summary>
    /// 提供对数据的加密解密方法
    /// </summary>
    public abstract class Cryptor
    {
        /// <summary>
        /// 加密使用的密钥
        /// </summary>
        protected SecretKey secretKey;

        /// <summary>
        /// 提供加密密钥
        /// </summary>
        public SecretKeyProvider Provider { get; set; }

        #region 公开接口

        /// <summary>
        /// 初始化加解密
        /// </summary>
        /// <param name="provider"></param>
        public void Initialize()
        {
            this.secretKey = this.Provider.GetSecretKey();

            this.OnInitialize();
        }

        /// <summary>
        /// 释放加解密占用的资源
        /// </summary>
        public void Release()
        {
            this.OnRelease();
        }

        /// <summary>
        /// 对对象进行加密
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public abstract string Encrypt(object o);

        /// <summary>
        /// 解密某个对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="s"></param>
        /// <returns></returns>
        public abstract T Decrypt<T>(string s);

        #endregion

        protected abstract void OnInitialize();
        protected abstract void OnRelease();

        public static Cryptor Create(SecretKeyTypeEnum type)
        {
            Cryptor cryptor = null;
            SecretKeyProvider provider = null;

            switch (type)
            {
                case SecretKeyTypeEnum.AES256_Cloud:
                    {
                        cryptor = new AESCryptor();
                        provider = new AESCloudSecretKeyProvider();
                        break;
                    }

                case SecretKeyTypeEnum.AES256_Local:
                    {
                        cryptor = new AESCryptor();
                        provider = new AESLocalSecretKeyProvider();
                        break;
                    }

                default:
                    throw new NotImplementedException();
            }

            cryptor.Provider = provider;

            return cryptor;
        }
    }
}

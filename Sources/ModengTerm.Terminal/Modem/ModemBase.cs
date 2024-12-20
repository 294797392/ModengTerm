using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal.Modem
{
    public abstract class ModemBase
    {
        #region 公开接口

        public void Initialize()
        {
            this.OnInitialize();
        }

        public void Release()
        {
            this.OnRelease();
        }

        /// <summary>
        /// 处理收到的服务器数据
        /// </summary>
        /// <param name="data">从服务器接收到的数据</param>
        /// <param name="length">从服务器接收到的数据长度</param>
        public void OnDataReceived(byte[] data, int length)
        {

        }

        #endregion

        #region 受保护方法

        /// <summary>
        /// 从接收缓冲区里读取指定的数据
        /// </summary>
        /// <param name="buffer">存储收到的数据的缓冲区</param>
        /// <param name="size">要接收的数据长度</param>
        /// <returns>
        /// -1：接收失败
        /// 0：接收
        /// </returns>
        protected int Read(byte[] buffer, int size)
        {

        }

        #endregion

        #region 抽象方法

        protected abstract void OnInitialize();
        protected abstract void OnRelease();

        /// <summary>
        /// 从本地主机发送文件到远程主机
        /// </summary>
        /// <returns></returns>
        public abstract int Send(string filePath);

        /// <summary>
        /// 远程主机发送文件到本地主机
        /// </summary>
        /// <returns></returns>
        public abstract int Receive();

        #endregion
    }
}

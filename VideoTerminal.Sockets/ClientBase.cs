using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace VideoTerminal.Clients
{
    /// <summary>
    /// 管理与远程主机的连接
    /// </summary>
    public abstract class ClientBase
    {
        #region 常量

        /// <summary>
        /// 默认的接收缓冲区大小
        /// </summary>
        public const int DefaultReadBufferSize = 256;

        #endregion

        #region 公开事件

        public event Action<object, ClientState> StatusChanged;

        /// <summary>
        /// 当收到数据流的时候触发
        /// </summary>
        public event Action<ClientBase, byte[]> DataReceived;

        #endregion

        #region 属性

        public abstract ClientTypes Protocol { get; }

        /// <summary>
        /// 连接Socket所需要的验证信息
        /// </summary>
        public ClientAuthorition Authorition { get; private set; }

        #endregion

        #region 构造方法

        public ClientBase(ClientAuthorition authorition)
        {
            this.Authorition = authorition;
        }

        #endregion

        #region 公开接口

        public abstract bool Connect();

        public abstract bool Disconnect();

        /// <summary>
        /// 从Socket中读取一段数据
        /// </summary>
        /// <param name="bytes">保存读取的数据的缓冲区</param>
        /// <returns>读取的数据长度</returns>
        public abstract int Read(byte[] bytes);

        /// <summary>
        /// 从Socket中读取一个字节的数据
        /// </summary>
        /// <returns></returns>
        public abstract byte Read();

        /// <summary>
        /// 向Socket写入一个字节的数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public abstract bool Write(byte data);

        /// <summary>
        /// 向Socket里写入一段数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public abstract bool Write(byte[] data);

        #endregion

        #region 实例方法

        protected void NotifyStatusChanged(ClientState state)
        {
            if (this.StatusChanged != null)
            {
                this.StatusChanged(this, state);
            }
        }

        protected void NotifyDataReceived(byte[] bytes)
        {
            if (this.DataReceived != null)
            {
                this.DataReceived(this, bytes);
            }
        }

        #endregion
    }
}
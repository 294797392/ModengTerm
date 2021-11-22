using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace VideoTerminal.Sockets
{
    /// <summary>
    /// 管理与远程主机的连接
    /// </summary>
    public abstract class SocketBase
    {
        /// <summary>
        /// 默认的接收缓冲区大小
        /// </summary>
        public const int DefaultReadBufferSize = 256;

        #region 公开事件

        public event Action<object, SocketState> StatusChanged;

        /// <summary>
        /// 当收到数据流的时候触发
        /// </summary>
        public event Action<SocketBase, byte[]> DataReceived;

        #endregion

        #region 属性

        /// <summary>
        /// EOF为True时，VideoTerminal会停止解析
        /// 在与远程主机断开连接的时候，应该把这个值设为True，否则为False
        /// </summary>
        public abstract bool EOF { get; }

        public SocketTypes Protocol { get; }

        public SocketAuthorition Authorition { get; set; }

        #endregion

        #region 公开接口

        public abstract bool Connect();

        public abstract bool Disconnect();

        public abstract string Execute(string command);

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

        public static SocketBase Create(SocketTypes protocol)
        {
            switch (protocol)
            {
                case SocketTypes.SSH:
                    return new SSHSocket();

                default:
                    throw new NotImplementedException();
            }
        }

        protected void NotifyStatusChanged(SocketState state)
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
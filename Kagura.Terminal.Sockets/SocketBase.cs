using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kagura.Terminal.Sockets
{
    /// <summary>
    /// 管理与远程主机的连接
    /// </summary>
    public abstract class SocketBase
    {
        public event Action<object, SocketState> StatusChanged;

        public SocketProtocols Protocol { get; }

        public SocketAuthorition Authorition { get; set; }

        public abstract bool Connect();

        public abstract bool Disconnect();

        public abstract string Execute(string command);

        /// <summary>
        /// 从Socket中读取一段数据
        /// </summary>
        /// <param name="size">要读取的数据大小</param>
        /// <returns></returns>
        public abstract byte[] Read(int size);

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

        /// <summary>
        /// EOF为True时，VideoTerminal会停止解析
        /// 在与远程主机断开连接的时候，应该把这个值设为True，否则为False
        /// </summary>
        public abstract bool EOF { get; }

        public static SocketBase Create(SocketProtocols protocol)
        {
            switch (protocol)
            {
                case SocketProtocols.SSH:
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
    }
}
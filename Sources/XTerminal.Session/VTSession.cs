using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using XTerminal.Sessions;

namespace XTerminal.Session
{
    /// <summary>
    /// 管理与远程主机的连接
    /// </summary>
    public abstract class VTSession
    {
        #region 常量

        /// <summary>
        /// 默认的接收缓冲区大小
        /// </summary>
        public const int DefaultReadBufferSize = 256;

        #endregion

        #region 公开事件

        public event Action<object, VTSessionStateEnum> StatusChanged;

        /// <summary>
        /// 当收到数据流的时候触发
        /// </summary>
        public event Action<VTSession, byte[]> DataReceived;

        #endregion

        #region 实例变量

        protected VTInitialOptions options;

        #endregion

        #region 属性

        /// <summary>
        /// 终端可以显示的行数
        /// </summary>
        public int Rows { get { return this.options.TerminalProperties.Rows; } }

        /// <summary>
        /// 终端可以显示的列数
        /// </summary>
        public int Columns { get { return this.options.TerminalProperties.Columns; } }

        /// <summary>
        /// 通道类型
        /// </summary>
        public SessionTypeEnum Type { get { return this.options.ChannelType; } }

        #endregion

        #region 构造方法

        public VTSession(VTInitialOptions options)
        {
            this.options = options;
        }

        #endregion

        #region 公开接口

        public int Initialize()
        {
            return this.OnInitialize();
        }

        public void Release()
        {
            this.OnRelease();
        }

        public abstract int Connect();

        public abstract void Disconnect();

        /// <summary>
        /// 向Socket里写入一段数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public abstract int Write(byte[] data);

        protected abstract int OnInitialize();

        protected abstract void OnRelease();

        #endregion

        #region 实例方法

        protected void NotifyStatusChanged(VTSessionStateEnum state)
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
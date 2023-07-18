using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using XTerminal.Base;
using XTerminal.Sessions;

namespace XTerminal.Session
{
    /// <summary>
    /// 管理与远程主机的连接
    /// </summary>
    public abstract class SessionBase
    {
        #region 常量

        /// <summary>
        /// 默认的接收缓冲区大小
        /// </summary>
        public const int DefaultReadBufferSize = 256;

        #endregion

        #region 公开事件

        public event Action<object, SessionStatusEnum> StatusChanged;

        /// <summary>
        /// 当收到数据流的时候触发
        /// </summary>
        public event Action<SessionBase, byte[]> DataReceived;

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
        public SessionTypeEnum Type { get { return this.options.SessionType; } }

        #endregion

        #region 构造方法

        public SessionBase(VTInitialOptions options)
        {
            this.options = options;
        }

        #endregion

        #region 公开接口

        public abstract int Connect();

        public abstract void Disconnect();

        /// <summary>
        /// 处理输入数据
        /// </summary>
        /// <param name="ievt"></param>
        /// <returns></returns>
        public abstract int Input(VTInputEvent ievt);

        /// <summary>
        /// 往会话里写入数据
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public abstract int Write(byte[] bytes);

        #endregion

        #region 实例方法

        protected void NotifyStatusChanged(SessionStatusEnum state)
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
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

        #endregion

        #region 实例变量

        protected VTInitialOptions options;

        /// <summary>
        /// 当前会话状态
        /// </summary>
        private SessionStatusEnum status;

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

        /// <summary>
        /// 打开会话
        /// </summary>
        /// <returns></returns>
        public abstract int Open();

        /// <summary>
        /// 关闭会话
        /// </summary>
        public abstract void Close();

        /// <summary>
        /// 往会话里写入数据
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public abstract int Write(byte[] bytes);

        /// <summary>
        /// 从会话里同步读取数据
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns>
        /// 返回读取到的字节数，如果读取失败则返回-1
        /// </returns>
        internal abstract int Read(byte[] buffer);

        #endregion

        #region 实例方法

        protected void NotifyStatusChanged(SessionStatusEnum status)
        {
            if (this.status == status)
            {
                return;
            }

            this.status = status;

            if (this.StatusChanged != null)
            {
                this.StatusChanged(this, status);
            }
        }

        #endregion
    }
}
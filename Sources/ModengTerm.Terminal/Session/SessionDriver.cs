using ModengTerm.Base;
using ModengTerm.Base.DataModels;
using ModengTerm.Base.Enumerations;

namespace ModengTerm.Terminal.Session
{
    /// <summary>
    /// 管理与远程主机的连接
    /// SessionDriver的所有操作都是阻塞的
    /// 由SessionTransport封装为非阻塞接口
    /// 上层调用SessionTransport接口来和SessionDriver打交道
    /// </summary>
    public abstract class SessionDriver
    {
        #region 常量

        /// <summary>
        /// 默认的接收缓冲区大小
        /// </summary>
        public const int DefaultReadBufferSize = 256;

        #endregion

        #region 公开事件

        #endregion

        #region 实例变量

        protected XTermSession session;

        #endregion

        #region 属性

        /// <summary>
        /// 通道类型
        /// </summary>
        public SessionTypeEnum Type { get { return (SessionTypeEnum)this.session.Type; } }

        #endregion

        #region 构造方法

        public SessionDriver(XTermSession session)
        {
            this.session = session;
        }

        #endregion

        #region 公开接口

        /// <summary>
        /// 打开会话
        /// 该操作是同步操作
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
        /// <param name="buffer"></param>
        /// <returns>写入是否成功</returns>
        public abstract void Write(byte[] buffer);

        /// <summary>
        /// 从会话里同步读取数据
        /// 读取失败抛异常，由SessionTransport统一处理
        /// </summary>
        /// <param name="buffer">保存读取到的数据的缓冲区</param>
        /// <returns>
        /// 返回读取到的字节数，如果读取失败则返回-1或者直接抛异常
        /// 如果返回0表示所有数据读取结束，没有更多的数据可读了
        /// </returns>
        internal abstract int Read(byte[] buffer);

        /// <summary>
        /// 重置终端大小
        /// </summary>
        /// <param name="row">新的行数</param>
        /// <param name="col">新的列数</param>
        public abstract void Resize(int row, int col);

        public virtual int Control(int command, object parameter, out object result) 
        {
            result = null;

            return ResponseCode.NOT_SUPPORTED;
        }

        #endregion

        #region 实例方法

        #endregion
    }
}
using ModengTerm.Base;
using ModengTerm.Document;
using ModengTerm.Terminal.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ModengTerm.Terminal.Modules
{
    /// <summary>
    /// 模块用来实现终端的某个功能
    /// 模块实例可以一直存在，手动调用Start和Stop启用或停止模块的运行
    /// </summary>
    public abstract class VTModuleBase
    {
        #region 实例变量

        protected SessionTransport transport;

        #endregion

        #region 属性

        /// <summary>
        /// 模块类型
        /// </summary>
        public abstract VTModuleTypes Type { get; }

        /// <summary>
        /// 模块状态
        /// </summary>
        public VTModuleStatus Status { get; protected set; }

        public SessionTransport SessionTransport
        {
            get { return transport; }
            set
            {
                if (this.transport != value)
                {
                    this.transport = value;
                }
            }
        }

        #endregion

        #region 公开接口

        /// <summary>
        /// 开始运行模块
        /// </summary>
        public void Start()
        {
            this.transport.DataReceived += Transport_DataReceived;

            this.OnStart();

            this.Status = VTModuleStatus.Started;
        }

        /// <summary>
        /// 停止运行模块
        /// </summary>
        public void Stop()
        {
            this.transport.DataReceived -= this.Transport_DataReceived;

            this.OnStop();

            this.Status = VTModuleStatus.Stopped;
        }

        #endregion

        #region 受保护方法

        #endregion

        #region 抽象方法

        protected abstract void OnStart();
        protected abstract void OnStop();

        /// <summary>
        /// 当收到服务器数据的时候触发
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="size"></param>
        public abstract void OnDataReceived(byte[] buffer, int size);

        #endregion

        #region 事件处理器

        private void Transport_DataReceived(SessionTransport transport, byte[] buffer, int size)
        {
            this.OnDataReceived(buffer, size);
        }

        #endregion
    }
}

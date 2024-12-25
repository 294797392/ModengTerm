using DotNEToolkit;
using ModengTerm.Base.DataModels;
using ModengTerm.Terminal.Session;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal.Modem
{
    /// <summary>
    /// 定义XYZModem三种传输方式通用接口
    /// </summary>
    public abstract class ModemBase
    {
        #region 事件

        public event Action<ModemBase, double> ProgressChanged;

        #endregion

        #region 实例变量

        #endregion

        #region 属性

        /// <summary>
        /// 与主机进行通信的接口
        /// </summary>
        public IHostStream HostStream { get; set; }

        public XTermSession Session { get; set; }

        #endregion

        #region 公开接口

        #endregion

        #region 受保护方法

        protected void NotifyProgressChanged(double progress) 
        {
            this.ProgressChanged?.Invoke(this, progress);
        }

        #endregion

        #region 抽象方法

        /// <summary>
        /// 从本地主机发送文件到远程主机
        /// </summary>
        /// <returns></returns>
        public abstract int Send(Stream stream);

        /// <summary>
        /// 远程主机发送文件到本地主机
        /// </summary>
        /// <returns></returns>
        public abstract int Receive(Stream stream);

        #endregion
    }
}

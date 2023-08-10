using DotNEToolkit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFToolkit.MVVM;
using XTerminal.Base;
using XTerminal.Base.DataModels;
using XTerminal.Base.Enumerations;
using XTerminal.Document.Rendering;
using XTerminal.Session;

namespace XTerminal.ViewModels
{
    /// <summary>
    /// 运行时的会话信息
    /// </summary>
    public class TerminalSessionVM : OpenedSessionVM
    {
        #region 实例变量

        private SessionStatusEnum status;
        private VideoTerminal videoTerminal;
        private Base.DataModels.XTermSession session;

        #endregion

        #region 公开事件

        /// <summary>
        /// 会话状态改变的时候触发
        /// </summary>
        public event Action<TerminalSessionVM, SessionStatusEnum> StatusChanged;

        #endregion

        #region 属性

        /// <summary>
        /// 当前状态
        /// </summary>
        public SessionStatusEnum Status
        {
            get { return this.status; }
            set
            {
                if (this.status != value)
                {
                    this.status = value;
                    this.NotifyPropertyChanged("Status");
                }
            }
        }

        /// <summary>
        /// 会话类型
        /// </summary>
        public SessionTypeEnum Type { get; set; }

        /// <summary>
        /// 用来显示输出的终端屏幕
        /// </summary>
        public ITerminalScreen TerminalScreen { get; set; }

        /// <summary>
        /// 该会话所维护的终端
        /// </summary>
        public VideoTerminal VideoTerminal { get { return this.videoTerminal; } }

        #endregion

        #region 构造方法

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="session">要打开的会话对象</param>
        public TerminalSessionVM(XTermSession session)
        {
            this.session = session;
            this.ID = Guid.NewGuid().ToString();
            this.Name = session.Name;
            this.Description = session.Description;
            this.Type = (SessionTypeEnum)session.SessionType;
        }

        #endregion

        #region 公开接口

        public void Open()
        {
            this.videoTerminal = new VideoTerminal();
            this.videoTerminal.SessionStatusChanged += this.VideoTerminal_SessionStatusChanged;
            this.videoTerminal.TerminalScreen = this.TerminalScreen;
            this.videoTerminal.Initialize(this.session);
        }

        public void Close()
        {
            if (this.videoTerminal == null)
            {
                return;
            }
            this.videoTerminal.SessionStatusChanged -= this.VideoTerminal_SessionStatusChanged;
            this.videoTerminal.Release();
        }

        #endregion

        #region 实例方法

        #endregion

        #region 事件处理器

        private void VideoTerminal_SessionStatusChanged(VideoTerminal videoTerminal, SessionStatusEnum status)
        {
            this.Status = status;
        }

        #endregion
    }
}

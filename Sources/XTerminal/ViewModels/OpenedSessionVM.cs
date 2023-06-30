using DotNEToolkit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFToolkit.MVVM;
using XTerminal.Base.DataModels;
using XTerminal.Document.Rendering;
using XTerminal.Session;
using XTerminal.Session.Property;
using XTerminal.Sessions;

namespace XTerminal.ViewModels
{
    /// <summary>
    /// 运行时的会话信息
    /// </summary>
    public class OpenedSessionVM : ViewModelBase
    {
        #region 实例变量

        private SessionStatusEnum status;
        private VideoTerminal videoTerminal;
        private Base.DataModels.XTermSession session;

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
        /// 画板容器
        /// </summary>
        public IDrawingCanvasPanel CanvasPanel { get; set; }

        #endregion

        #region 构造方法

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="session">要打开的会话对象</param>
        public OpenedSessionVM(Base.DataModels.XTermSession session)
        {
            this.session = session;

            this.videoTerminal = new VideoTerminal();
            this.videoTerminal.SessionStatusChanged += this.VideoTerminal_SessionStatusChanged;
            this.videoTerminal.CanvasPanel = this.CanvasPanel;
        }

        #endregion

        #region 公开接口

        public void Open()
        {
            VTInitialOptions initialOptions = new VTInitialOptions()
            {
                //SessionType = (SessionTypeEnum)this.session.Type,
                //SessionProperties = this.DeserializeSessionProperty((SessionTypeEnum)this.session.Type, this.session.Properties),
                //TerminalProperties = new TerminalProperties()
                //{
                //    Columns = this.session.Column,
                //    Rows = this.session.Row,
                //    Type = TerminalTypeEnum.XTerm
                //},
                //ReadBufferSize = 8192,
            };

            this.videoTerminal.Initialize(initialOptions);
        }

        public void Close()
        { }

        #endregion

        #region 实例方法

        private SessionProperties DeserializeSessionProperty(SessionTypeEnum sessionType, string propertyString)
        {
            switch (sessionType)
            {
                case SessionTypeEnum.libvtssh:
                case SessionTypeEnum.SSH:
                    return JSONHelper.Parse<SSHSessionProperties>(propertyString);

                default:
                    throw new NotImplementedException();
            }
        }

        #endregion

        #region 事件处理器

        private void VideoTerminal_SessionStatusChanged(VideoTerminal videoTerminal, SessionStatusEnum status)
        {
            this.Status = status;
        }

        #endregion
    }
}

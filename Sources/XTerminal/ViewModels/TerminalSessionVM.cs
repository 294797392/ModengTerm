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
    /// 管理终端类型的Session运行时状态
    /// </summary>
    public class TerminalSessionVM : OpenedSessionVM
    {
        #region 实例变量

        private SessionStatusEnum status;
        private VideoTerminal videoTerminal;
        private XTermSession session;
        private int rowSize;
        private int colSize;

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
        /// 该会话所维护的终端
        /// </summary>
        public VideoTerminal VideoTerminal { get { return this.videoTerminal; } }

        /// <summary>
        /// 当前终端显示的行数
        /// </summary>
        public int RowSize
        {
            get { return this.rowSize; }
            set
            {
                if (this.rowSize != value)
                {
                    this.rowSize = value;
                    this.NotifyPropertyChanged("RowSize");
                }
            }
        }

        /// <summary>
        /// 当前终端显示的列数
        /// </summary>
        public int ColumnSize
        {
            get { return this.colSize; }
            set
            {
                if(this.colSize != value)
                {
                    this.colSize = value;
                    this.NotifyPropertyChanged("ColumnSize");
                }
            }
        }

        #endregion

        #region 构造方法

        /// <summary>
        /// 构造方法
        /// </summary>
        public TerminalSessionVM()
        {
        }

        #endregion

        #region 公开接口

        public override int Open(XTermSession session)
        {
            this.session = session;

            this.videoTerminal = new VideoTerminal();
            this.videoTerminal.SessionStatusChanged += this.VideoTerminal_SessionStatusChanged;
            this.videoTerminal.SizeChanged += this.VideoTerminal_SizeChanged;
            this.videoTerminal.TerminalScreen = this.Content as ITerminalScreen;
            this.videoTerminal.Initialize(this.session);

            return ResponseCode.SUCCESS;
        }

        public override void Close()
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

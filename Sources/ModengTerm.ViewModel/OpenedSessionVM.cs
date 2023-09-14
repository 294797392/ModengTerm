using ModengTerm.Base.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WPFToolkit.MVVM;
using XTerminal.Base.DataModels;
using XTerminal.Base.Enumerations;

namespace XTerminal.ViewModels
{
    public abstract class OpenedSessionVM : ItemViewModel
    {
        #region 公开事件

        /// <summary>
        /// 会话状态改变的时候触发
        /// </summary>
        public event Action<OpenedSessionVM, SessionStatusEnum> StatusChanged;

        #endregion

        #region 实例变量

        private SessionStatusEnum status;

        #endregion

        #region 属性

        /// <summary>
        /// 界面上的控件
        /// </summary>
        public DependencyObject Content { get; set; }

        /// <summary>
        /// 对应的会话信息
        /// </summary>
        public XTermSession Session { get; private set; }

        /// <summary>
        /// 与会话的连接状态
        /// </summary>
        public SessionStatusEnum Status
        {
            get { return this.status; }
            private set
            {
                if (this.status != value)
                {
                    this.status = value;
                    this.NotifyPropertyChanged("Status");
                }
            }
        }

        #endregion

        public int Open(XTermSession session)
        {
            this.Session = session;

            return this.OnOpen(session);
        }

        public void Close()
        {
            this.OnClose();
        }

        protected abstract int OnOpen(XTermSession sessionInfo);
        protected abstract void OnClose();

        protected void NotifyStatusChanged(SessionStatusEnum status)
        {
            if (this.status == status)
            {
                return;
            }

            this.Status = status;

            if (this.StatusChanged != null)
            {
                this.StatusChanged(this, status);
            }
        }
    }
}

using ModengTerm.Base.DataModels;
using ModengTerm.Base.Enumerations;
using ModengTerm.ServiceAgents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WPFToolkit.MVVM;
using XTerminal.Base.DataModels;
using XTerminal.Base.Enumerations;

namespace ModengTerm.ViewModels
{
    public abstract class OpenedSessionVM : SessionItemVM
    {
        #region 公开事件

        /// <summary>
        /// 会话状态改变的时候触发
        /// </summary>
        public event Action<OpenedSessionVM, SessionStatusEnum> StatusChanged;

        #endregion

        #region 实例变量

        private SessionStatusEnum status;
        private DependencyObject content;

        #endregion

        #region 属性

        /// <summary>
        /// 界面上的控件
        /// </summary>
        public DependencyObject Content 
        {
            get { return this.content; }
            set
            {
                if (this.content != value)
                {
                    this.content = value;
                    this.NotifyPropertyChanged("Content");
                }
            }
        }

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

        /// <summary>
        /// 访问服务的代理
        /// </summary>
        public ServiceAgent ServiceAgent { get; set; }

        #endregion

        public OpenedSessionVM(XTermSession session)
        {
            this.Session = session;
        }

        public int Open()
        {
            return this.OnOpen();
        }

        public void Close()
        {
            this.OnClose();
        }

        protected abstract int OnOpen();
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

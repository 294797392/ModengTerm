using ModengTerm.Base.DataModels;
using ModengTerm.Base.Enumerations;
using ModengTerm.Base.ServiceAgents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WPFToolkit.MVVM;

namespace ModengTerm.ViewModels
{
    public class SMenuItem : ViewModelBase
    {
        public delegate void SMenuItemClickDelegate();

        private string icon;

        public string Icon
        {
            get { return this.icon; }
            set
            {
                if (this.icon != value) 
                {
                    this.icon = value;
                    this.NotifyPropertyChanged("Icon");
                }
            }
        }

        public SMenuItemClickDelegate ClickDelegate { get; private set; }

        public SMenuItem(string name, string icon, SMenuItemClickDelegate clickDelegate)
        {
            this.Name = name;
            this.Icon = icon;
            ClickDelegate = clickDelegate;
        }
    }

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

        /// <summary>
        /// 该会话的工具栏菜单
        /// </summary>
        public BindableCollection<SMenuItem> ToolbarMenus { get; private set; }

        #endregion

        #region 构造方法

        public OpenedSessionVM(XTermSession session)
        {
            this.Session = session;
            this.ToolbarMenus = new BindableCollection<SMenuItem>();
        }

        #endregion

        #region 公开接口

        public int Open()
        {
            return this.OnOpen();
        }

        public void Close()
        {
            this.OnClose();
        }

        #endregion

        #region 抽象方法

        protected abstract int OnOpen();
        protected abstract void OnClose();

        #endregion

        #region 实例方法

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

        #endregion
    }
}

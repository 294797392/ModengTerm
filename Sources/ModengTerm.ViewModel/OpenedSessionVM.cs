using ModengTerm.Addon;
using ModengTerm.Addon.Interactive;
using ModengTerm.Base;
using ModengTerm.Base.DataModels;
using ModengTerm.Base.Enumerations;
using ModengTerm.Base.ServiceAgents;
using System.Windows;
using WPFToolkit.MVVM;

namespace ModengTerm.ViewModel
{
    /// <summary>
    /// 表示一个被打开的会话
    /// </summary>
    public abstract class OpenedSessionVM : SessionItemVM, IClientTab
    {
        #region 公开事件

        /// <summary>
        /// 所有的会话事件都通过这个事件触发
        /// </summary>
        public event Action<OpenedSessionVM, ClientEvent, ClientEventArgs> Notify;

        #endregion

        #region 实例变量

        private SessionStatusEnum status;
        private DependencyObject content;

        private StatusChangedEventArgs statusChangedEventArgs;

        #endregion

        #region 属性

        /// <summary>
        /// 访问服务的代理
        /// </summary>
        public ServiceAgent ServiceAgent { get; set; }

        /// <summary>
        /// 界面上的控件
        /// </summary>
        public DependencyObject Content
        {
            get { return content; }
            set
            {
                if (content != value)
                {
                    content = value;
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
            get { return status; }
            private set
            {
                if (status != value)
                {
                    status = value;
                    this.NotifyPropertyChanged("Status");
                }
            }
        }

        public SessionTypeEnum Type { get { return (SessionTypeEnum)this.Session.Type; } }

        #endregion

        #region 构造方法

        public OpenedSessionVM(XTermSession session)
        {
            this.Session = session;
            this.statusChangedEventArgs = new StatusChangedEventArgs();
        }

        #endregion

        #region 公开接口

        /// <summary>
        /// 在所有属性都赋值之后，Open之前调用
        /// 和构造函数相比有个不同的地方，Initialize调用的时候所有属性都赋值了，而构造函数调用的时候则没有
        /// </summary>
        public void Initialize()
        {
            // init

            this.OnInitialize();
        }

        public void Release()
        {
            this.OnRelease();

            // release
        }

        public int Open()
        {
            // 创建面板容器
            return OnOpen();
        }

        public void Close()
        {
            OnClose();
        }

        /// <summary>
        /// 当显示到界面上之后触发
        /// </summary>
        public void OnLoaded()
        {
        }

        /// <summary>
        /// 当从界面上移除之后触发
        /// </summary>
        public void OnUnload()
        {
        }

        #endregion

        #region IHostTab

        public T GetOption<T>(OptionKeyEnum key, T defaultValue)
        {
            return this.Session.GetOption<T>(key, defaultValue);
        }

        #endregion

        #region 抽象方法

        protected abstract void OnInitialize();
        protected abstract void OnRelease();
        protected abstract int OnOpen();
        protected abstract void OnClose();

        #endregion

        #region 受保护方法

        /// <summary>
        /// 由子类调用，通知父类会话状态改变
        /// 该方法在UI线程运行
        /// </summary>
        /// <param name="status"></param>
        protected void RaiseStatusChanged(SessionStatusEnum status)
        {
            if (this.status == status)
            {
                return;
            }

            SessionStatusEnum oldStatus = this.status;
            SessionStatusEnum newStatus = status;

            this.Status = status;

            this.statusChangedEventArgs.OldStatus = oldStatus;
            this.statusChangedEventArgs.NewStatus = newStatus;
            this.Notify?.Invoke(this, ClientEvent.CLIENT_TAB_STATUS_CHANGED, this.statusChangedEventArgs);

            //throw new RefactorImplementedException();
            //// 通知所有PanelContent，会话状态改变了
            //IEnumerable<SessionPanelContentVM> panelContents = PanelContainer.MenuItems.Where(v => v.ContentVM != null).Select(v => v.ContentVM).Cast<SessionPanelContentVM>();
            //foreach (SessionPanelContentVM panelContent in panelContents)
            //{
            //    panelContent.OnSessionStatusChanged(status);
            //}
        }

        #endregion

        #region 实例方法

        #endregion

        #region 事件处理器

        /// <summary>
        /// 在PanelContentVM初始化之前触发
        /// </summary>
        /// <param name="menuItemVM"></param>
        /// <param name="viewModel"></param>
        private void PanelItem_ContentInitializing(MenuItemVM menuItemVM, ViewModelBase viewModel, DependencyObject view)
        {
            throw new RefactorImplementedException();
            //SessionPanel sessionPanelContentVM = viewModel as SessionPanel;
            //sessionPanelContentVM.Session = Session;
            //sessionPanelContentVM.ServiceAgent = ServiceAgent;
        }

        #endregion
    }
}

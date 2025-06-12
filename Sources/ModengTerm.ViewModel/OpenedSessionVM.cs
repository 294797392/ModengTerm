using ModengTerm.Addon.Interactive;
using ModengTerm.Addon.ViewModel;
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
    public abstract class OpenedSessionVM : SessionItemVM, IPanel
    {
        #region 公开事件

        public event Action<OpenedSessionVM, EventType, EventArgs> Notify;

        #endregion

        #region 实例变量

        private SessionStatusEnum status;
        private DependencyObject content;

        private StatusChangedEventArgs statusChangedEventArgs;

        #endregion

        #region 属性

        public string Id { get; private set; }

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

        /// <summary>
        /// 侧边栏窗口
        /// </summary>
        public PanelContainer PanelContainer { get; private set; }

        #endregion

        #region 构造方法

        public OpenedSessionVM(XTermSession session)
        {
            this.Session = session;
            this.Id = session.ID;
            this.statusChangedEventArgs = new StatusChangedEventArgs();
        }

        #endregion

        #region 公开接口

        /// <summary>
        /// 在所有属性都赋值之后，Open之前调用
        /// </summary>
        public void Initialize()
        {
            InitializePanels();
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
            SessionPanel panel = this.GetActivePanel();
            if (panel != null)
            {
                panel.OnLoaded();
            }
        }

        /// <summary>
        /// 当从界面上移除之后触发
        /// </summary>
        public void OnUnload()
        {
            SessionPanel panel = this.GetActivePanel();
            if (panel != null)
            {
                panel.OnUnload();
            }
        }

        #endregion

        #region IPanel

        public void VisiblePanel(string panelId)
        {
            PanelContainer.ChangeVisible(panelId);
        }

        #endregion

        #region 抽象方法

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
            this.Notify?.Invoke(this, EventType.COMMON_SESSION_STATUS_CHANGED, this.statusChangedEventArgs);

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

        /// <summary>
        /// 获取当前显示的PanelContent
        /// </summary>
        /// <returns></returns>
        private SessionPanel GetActivePanel()
        {
            if (this.PanelContainer == null)
            {
                return null;
            }

            return this.PanelContainer.SelectedItem as SessionPanel;
        }

        private void InitializePanels()
        {
            //// 加载所有插件要显示的面板
            //List<AddonContext> contexts = MTermApp.Context.AddonContexts;

            //List<PanelItemDefinition> panels = new List<PanelItemDefinition>();

            //foreach (AddonContext context in contexts)
            //{
            //    AddonDefinition definition = context.Definition;

            //    foreach (PanelItemDefinition panelDefinition in definition.SessionPanels)
            //    {
            //        if (panelDefinition.SessionTypes.Count == 0)
            //        {
            //            // 如果没有过滤，那么就都可以加载
            //            panels.Add(panelDefinition);
            //        }
            //        else
            //        {
            //            if (panelDefinition.SessionTypes.Contains(Session.Type))
            //            {
            //                panels.Add(panelDefinition);
            //            }
            //        }
            //    }
            //}

            //PanelContainer = VTClientUtils.CreatePanelContainerVM(panels);
        }

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

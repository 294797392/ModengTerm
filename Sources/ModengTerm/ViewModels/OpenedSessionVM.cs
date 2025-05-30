using ModengTerm.Addons;
using ModengTerm.Base.DataModels;
using ModengTerm.Base.Definitions;
using ModengTerm.Base.Enumerations;
using ModengTerm.Base.ServiceAgents;
using ModengTerm.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WPFToolkit.MVVM;

namespace ModengTerm.ViewModels
{
    /// <summary>
    /// 表示一个被打开的会话
    /// </summary>
    public abstract class OpenedSessionVM : SessionItemVM, IAddonSession
    {
        #region 公开事件

        /// <summary>
        /// 连接状态改变的时候触发
        /// </summary>
        public event Action<OpenedSessionVM, SessionStatusEnum> StatusChanged;

        #endregion

        #region 实例变量

        private SessionStatusEnum status;
        private DependencyObject content;

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
        /// 侧边栏窗口
        /// </summary>
        public PanelContainerVM PanelContainer { get; private set; }

        #endregion

        #region 构造方法

        public OpenedSessionVM(XTermSession session)
        {
            this.Session = session;
            this.Id = session.ID;
        }

        #endregion

        #region 公开接口

        /// <summary>
        /// 在所有属性都赋值之后，Open之前调用
        /// </summary>
        public void Initialize()
        {
            this.InitializePanels();
            this.AddRemovePanelItemEvent(true);
        }

        public int Open()
        {
            // 创建面板容器
            return this.OnOpen();
        }

        public void Close()
        {
            this.OnClose();

            this.AddRemovePanelItemEvent(false);
        }

        /// <summary>
        /// 当显示到界面上之后触发
        /// </summary>
        public void OnLoaded()
        {
            SessionPanelContentVM panelContent = this.GetActivePanelContent();
            if (panelContent != null)
            {
                panelContent.OnLoaded();
            }
        }

        /// <summary>
        /// 当从界面上移除之后触发
        /// </summary>
        public void OnUnload()
        {
            SessionPanelContentVM panelContent = this.GetActivePanelContent();
            if (panelContent != null)
            {
                panelContent.OnUnload();
            }
        }

        #endregion

        #region IAddonSession

        public void VisiblePanel(string panelId) 
        {
            this.PanelContainer.ChangeVisible(panelId);
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

            this.Status = status;

            // 通知所有PanelContent，会话状态改变了
            IEnumerable<SessionPanelContentVM> panelContents = this.PanelContainer.MenuItems.Where(v => v.ContentVM != null).Select(v => v.ContentVM).Cast<SessionPanelContentVM>();
            foreach (SessionPanelContentVM panelContent in panelContents)
            {
                panelContent.OnSessionStatusChanged(status);
            }
        }

        #endregion

        #region 实例方法

        /// <summary>
        /// 获取当前显示的PanelContent
        /// </summary>
        /// <returns></returns>
        private SessionPanelContentVM GetActivePanelContent()
        {
            if (this.PanelContainer == null)
            {
                return null;
            }

            MenuItemVM selectedItem = this.PanelContainer.SelectedMenu;
            if (selectedItem == null)
            {
                return null;
            }

            return selectedItem.ContentVM as SessionPanelContentVM;
        }

        /// <summary>
        /// 注册或反注册PanelItem的事件
        /// </summary>
        /// <param name="add"></param>
        private void AddRemovePanelItemEvent(bool add)
        {
            foreach (MenuItemVM menuItem in this.PanelContainer.MenuItems)
            {
                if (add)
                {
                    // 注册这个事件的目的是为了把OpenedSessionVM的实例传递给SessionPanelContentVM
                    menuItem.ContentInitializing += PanelItem_ContentInitializing;
                }
                else
                {
                    menuItem.ContentInitializing -= PanelItem_ContentInitializing;
                }
            }
        }

        private void InitializePanels()
        {
            // 加载所有插件要显示的面板
            List<AddonContext> contexts = MTermApp.Context.AddonContexts;

            List<PanelItemDefinition> panels = new List<PanelItemDefinition>();

            foreach (AddonContext context in contexts)
            {
                AddonDefinition definition = context.Definition;

                foreach (PanelItemDefinition panelDefinition in definition.SessionPanels)
                {
                    if (panelDefinition.SessionTypes.Count == 0)
                    {
                        // 如果没有过滤，那么就都可以加载
                        panels.Add(panelDefinition);
                    }
                    else
                    {
                        if (panelDefinition.SessionTypes.Contains(this.Session.Type))
                        {
                            panels.Add(panelDefinition);
                        }
                    }
                }
            }

            this.PanelContainer = VTClientUtils.CreatePanelContainerVM(panels);
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
            SessionPanelContentVM sessionPanelContentVM = viewModel as SessionPanelContentVM;
            sessionPanelContentVM.Session = this.Session;
            sessionPanelContentVM.OpenedSessionVM = this;
            sessionPanelContentVM.ServiceAgent = this.ServiceAgent;
        }

        #endregion
    }
}

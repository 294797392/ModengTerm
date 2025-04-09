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
    public abstract class OpenedSessionVM : SessionItemVM
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

        /// <summary>
        /// 访问服务的代理
        /// </summary>
        public ServiceAgent ServiceAgent { get; set; }

        /// <summary>
        /// 该会话的侧边栏窗口
        /// </summary>
        public PanelDefinition PanelDefinition { get; set; }

        /// <summary>
        /// 该会话的菜单列表
        /// </summary>
        public List<MenuItemDefinition> MenuItems { get; set; }

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
        /// 该会话的右键菜单
        /// </summary>
        public BindableCollection<ContextMenuVM> ContextMenus { get; private set; }

        /// <summary>
        /// 该会话的标题菜单
        /// </summary>
        public BindableCollection<ContextMenuVM> TitleMenus { get; private set; }

        /// <summary>
        /// 侧边栏窗口
        /// </summary>
        public PanelVM Panel { get; private set; }

        #endregion

        #region 构造方法

        public OpenedSessionVM(XTermSession session)
        {
            this.Session = session;
        }

        #endregion

        #region 公开接口

        /// <summary>
        /// 在所有数据都准备好之后，Open之前调用
        /// </summary>
        public void Initialize() 
        {
            // 在Initialize里实例化TitleMenus, 保证ListBoxOpenedSession_SelectionChanged触发的时候，TitleMenus不为空
            this.ContextMenus = new BindableCollection<ContextMenuVM>();
            this.TitleMenus = new BindableCollection<ContextMenuVM>();
            List<MenuItemRelation> titleMenuRelations = this.MenuItems.Select(v => new MenuItemRelation(v.TitleParentID, v)).ToList();
            this.TitleMenus.AddRange(VTClientUtils.CreateContextMenuVM(titleMenuRelations));
            List<MenuItemRelation> contextMenuRelations = this.MenuItems.Select(v => new MenuItemRelation(v.ContextParentID, v)).ToList();
            this.ContextMenus.AddRange(VTClientUtils.CreateContextMenuVM(contextMenuRelations));
        }

        public int Open()
        {
            // 给所有的插件页面使用的通用属性赋值
            this.Panel = VTClientUtils.PanelDefinition2PanelVM(this.PanelDefinition);
            this.AddRemovePanelItemEvent(true);
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
            IEnumerable<SessionPanelContentVM> panelContents = this.Panel.MenuItems.Where(v => v.ContentVM != null).Select(v => v.ContentVM).Cast<SessionPanelContentVM>();
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
            if (this.Panel == null)
            {
                return null;
            }

            MenuItemVM selectedItem = this.Panel.SelectedMenu;
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
            foreach (MenuItemVM menuItem in this.Panel.MenuItems)
            {
                if (add)
                {
                    menuItem.ContentInitializing += PanelItem_ContentInitializing;
                }
                else
                {
                    menuItem.ContentInitializing -= PanelItem_ContentInitializing;
                }
            }
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

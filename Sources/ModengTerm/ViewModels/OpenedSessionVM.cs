using ModengTerm.Base.DataModels;
using ModengTerm.Base.Enumerations;
using ModengTerm.Base.ServiceAgents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents.DocumentStructures;
using WPFToolkit.MVVM;

namespace ModengTerm.ViewModels
{
    /// <summary>
    /// 表示一个被打开的会话
    /// </summary>
    public abstract class OpenedSessionVM : SessionItemVM
    {
        private class MenuItem
        {
            public string ParentID { get; private set; }

            public ContextMenuDefinition MenuDefinition { get; private set; }

            public MenuItem(string parentID, ContextMenuDefinition menuDefinition)
            {
                this.ParentID = parentID;
                this.MenuDefinition = menuDefinition;
            }
        }

        #region 公开事件

        /// <summary>
        /// 会话状态改变的时候触发
        /// </summary>
        public event Action<OpenedSessionVM, SessionStatusEnum> StatusChanged;

        #endregion

        #region 实例变量

        private SessionStatusEnum status;
        private DependencyObject content;

        /// <summary>
        /// 保存当前显示的所有菜单列表
        /// </summary>
        protected List<ContextMenuVM> contextMenus;

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
        /// 该会话的右键菜单
        /// </summary>
        public BindableCollection<ContextMenuVM> ContextMenus { get; private set; }

        /// <summary>
        /// 该会话的标题菜单
        /// </summary>
        public BindableCollection<ContextMenuVM> TitleMenus { get; private set; }

        #endregion

        #region 构造方法

        public OpenedSessionVM(XTermSession session)
        {
            this.Session = session;
        }

        #endregion

        #region 公开接口

        public int Open()
        {
            this.ContextMenus = new BindableCollection<ContextMenuVM>();
            this.TitleMenus = new BindableCollection<ContextMenuVM>();

            List<ContextMenuDefinition> menuDefinitions = this.OnCreateContextMenu();
            List<MenuItem> titleMenuItems = menuDefinitions.Select(v => new MenuItem(v.TitleParentID, v)).ToList();
            this.InitializeMenuVM(titleMenuItems, this.TitleMenus);
            List<MenuItem> contextMenuItems = menuDefinitions.Select(v => new MenuItem(v.ContextParentID, v)).ToList();
            this.InitializeMenuVM(contextMenuItems, this.ContextMenus);

            return this.OnOpen();
        }

        public void Close()
        {
            this.OnClose();
        }

        /// <summary>
        /// 当显示到界面上之后触发
        /// </summary>
        public void Load() { }

        /// <summary>
        /// 当从界面上移除之后触发
        /// </summary>
        public void Unload() { }

        #endregion

        #region 抽象方法

        protected abstract int OnOpen();
        protected abstract void OnClose();

        /// <summary>
        /// 创建该会话的上下文菜单
        /// </summary>
        /// <returns></returns>
        protected abstract List<ContextMenuDefinition> OnCreateContextMenu();

        #endregion

        #region 实例方法

        /// <summary>
        /// 
        /// </summary>
        /// <param name="map">ParentID -> ContextMenuDefinition</param>
        /// <param name="menus"></param>
        private void InitializeMenuVM(List<MenuItem> menuItems, BindableCollection<ContextMenuVM> menus)
        {
            bool firstInit = false;

            if (this.contextMenus == null)
            {
                firstInit = true;
                this.contextMenus = new List<ContextMenuVM>();
            }

            // MenuId -> ContetMenuVM
            Dictionary<string, ContextMenuVM> menuCaches = new Dictionary<string, ContextMenuVM>();

            foreach (ContextMenuDefinition menuDefinition in menuItems.Select(v => v.MenuDefinition))
            {
                ContextMenuVM contextMenuVM = new ContextMenuVM(menuDefinition);
                menuCaches.Add(menuDefinition.ID, contextMenuVM);
            }

            foreach (MenuItem menuItem in menuItems)
            {
                ContextMenuDefinition definition = menuItem.MenuDefinition;
                string menuId = definition.ID;
                string parentID = menuItem.ParentID;

                if (parentID == "-1")
                {
                    continue;
                }

                if (definition.SupportedSessionTypes.Count > 0)
                {
                    if (!definition.SupportedSessionTypes.Contains((SessionTypeEnum)this.Session.Type))
                    {
                        continue;
                    }
                }

                ContextMenuVM contextMenuVM = menuCaches[menuId];

                if (string.IsNullOrWhiteSpace(parentID))
                {
                    // 根节点
                    menus.Add(contextMenuVM);
                }
                else
                {
                    // 子节点
                    ContextMenuVM parentVM = menuCaches[parentID];
                    parentVM.Children.Add(contextMenuVM);
                }

                if (firstInit)
                {
                    this.contextMenus.Add(contextMenuVM);
                }
            }
        }

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

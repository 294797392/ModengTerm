using log4net.Repository.Hierarchy;
using ModengTerm.Addons;
using ModengTerm.Base;
using ModengTerm.Base.DataModels;
using ModengTerm.Base.Definitions;
using ModengTerm.Base.Enumerations;
using ModengTerm.Base.ServiceAgents;
using ModengTerm.DataModels;
using ModengTerm.Terminal.ViewModels;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Linq;
using WPFToolkit.MVVM;

namespace ModengTerm.ViewModels
{
    public class MainWindowVM : ViewModelBase
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("MainWindowVM");

        /// <summary>
        /// 最后一个打开会话的按钮
        /// </summary>
        public static readonly OpenSessionVM OpenSessionVM = new OpenSessionVM();

        #endregion

        #region 实例变量

        private ServiceAgent serviceAgent;

        #endregion

        #region 属性

        /// <summary>
        /// 最近打开的会话列表
        /// </summary>
        public BindableCollection<RecentlySessionVM> RecentlyOpenedSession { get; private set; }

        /// <summary>
        /// 打开的所有会话列表
        /// </summary>
        public BindableCollection<SessionItemVM> SessionList { get; private set; }

        /// <summary>
        /// 打开的所有类型是Shell的会话列表
        /// </summary>
        public IEnumerable<ShellSessionVM> ShellSessions { get { return this.SessionList.OfType<ShellSessionVM>(); } }

        /// <summary>
        /// 选中的会话
        /// </summary>
        public SessionItemVM SelectedSession
        {
            get { return this.SessionList.SelectedItem; }
            set
            {
                this.SessionList.SelectedItem = value;
                this.NotifyPropertyChanged("SelectedSession");
            }
        }

        /// <summary>
        /// 会话的右键菜单
        /// </summary>
        public BindableCollection<ContextMenuVM> ContextMenus { get; private set; }

        /// <summary>
        /// 窗口顶部的所有菜单列表
        /// </summary>
        public BindableCollection<ContextMenuVM> TitleMenus { get; private set; }

        /// <summary>
        /// 所有主题列表
        /// </summary>
        public BindableCollection<AppThemeVM> Themes { get; private set; }

        public PanelVM Panel { get; private set; }

        #endregion

        #region 构造方法

        public MainWindowVM()
        {
            this.serviceAgent = MTermApp.Context.ServiceAgent;

            this.SessionList = new BindableCollection<SessionItemVM>();
            this.SessionList.Add(OpenSessionVM);

            List<XTermSession> sessions = this.serviceAgent.GetSessions();
            this.RecentlyOpenedSession = new BindableCollection<RecentlySessionVM>();
            List<RecentlySession> recentSessions = this.serviceAgent.GetRecentSessions();
            foreach (RecentlySession recentSession in recentSessions)
            {
                RecentlySessionVM recentlySessionVM = new RecentlySessionVM(recentSession);
                this.RecentlyOpenedSession.Add(recentlySessionVM);
            }

            this.TitleMenus = new BindableCollection<ContextMenuVM>();
            this.TitleMenus.AddRange(this.InitializeToolbarMenus());
            this.ContextMenus = new BindableCollection<ContextMenuVM>();

            this.Themes = new BindableCollection<AppThemeVM>();
            this.Themes.AddRange(MTermApp.Context.Manifest.AppThemes.Select(v => new AppThemeVM(v)));
            this.Themes.SelectedItem = this.Themes[0];//.FirstOrDefault();

            this.InitializePanels();
        }

        #endregion

        #region 公开接口

        /// <summary>
        /// 把一个会话加入到最近打开的会话列表里
        /// 如果最近的会话列表数量比设定的最大值多，那么会移除最早打开的一个会话
        /// </summary>
        /// <param name="session"></param>
        public void AddToRecentSession(XTermSession session)
        {
            RecentlySession recentlySession = new RecentlySession()
            {
                ID = Guid.NewGuid().ToString(),
                SessionId = session.ID,
                SessionName = session.Name,
            };

            // 保存的最近打开会话超出了最大个数
            // 删除最早保存的会话
            if (this.RecentlyOpenedSession.Count > VTBaseConsts.MaxRecentSessions)
            {
                RecentlySessionVM oldestSession = this.RecentlyOpenedSession[0];
                this.RecentlyOpenedSession.RemoveAt(0);
                this.serviceAgent.DeleteRecentSession(oldestSession.ID.ToString());
            }

            this.RecentlyOpenedSession.Add(new RecentlySessionVM(recentlySession));

            this.serviceAgent.AddRecentSession(recentlySession);
        }

        /// <summary>
        /// 从最近打开的会话列表里删除一个会话
        /// </summary>
        /// <param name="recentlySession"></param>
        public void DeleteRecentSession(RecentlySessionVM recentlySession)
        {
            this.RecentlyOpenedSession.Remove(recentlySession);

            this.serviceAgent.DeleteRecentSession(recentlySession.ID.ToString());
        }

        #endregion

        #region 实例方法

        private void InitializePanels()
        {
            List<PanelItemDefinition> panelItems = new List<PanelItemDefinition>();

            // 会话关联的窗口里包含插件窗口
            foreach (AddonDefinition addon in MTermApp.Context.Manifest.Addons)
            {
                foreach (PanelItemDefinition panelItem in addon.PanelItems)
                {
                    if (panelItem.Owner != (int)PanelItemOwner.Window)
                    {
                        continue;
                    }

                    panelItems.Add(panelItem);
                }
            }

            PanelDefinition panelDefinition = new PanelDefinition();
            panelDefinition.Items.AddRange(panelItems);
            this.Panel = VTClientUtils.PanelDefinition2PanelVM(panelDefinition);
        }

        private List<ContextMenuVM> InitializeToolbarMenus()
        {
            List<MenuItemDefinition> menuItems = new List<MenuItemDefinition>();

            // 先加载所有的根节点菜单
            menuItems.AddRange(MTermApp.Context.Manifest.ToolbarMenus);

            // 再加载所有的插件菜单
            foreach (AddonDefinition addon in MTermApp.Context.Manifest.Addons)
            {
                foreach (MenuItemDefinition menuItem in addon.ToolbarMenus)
                {
                    menuItem.AddonId = addon.ID;
                }

                menuItems.AddRange(addon.ToolbarMenus);
            }

            List<ContextMenuVM> result = new List<ContextMenuVM>();

            foreach (MenuItemDefinition menuItem in menuItems)
            {
                ContextMenuVM cmvm = new ContextMenuVM(menuItem);

                if (string.IsNullOrEmpty(menuItem.ParentId))
                {
                    // 此时说明是根菜单
                    result.Add(cmvm);
                }
                else
                {
                    // 此时说明不是根菜单
                    ContextMenuVM parentVM = result.FirstOrDefault(v => v.ID.ToString() == menuItem.ParentId);
                    parentVM.Children.Add(cmvm);
                }

                // 加载这个菜单的子节点
                this.LoadChildMenuItems(cmvm, menuItem.Children);
            }

            return result;
        }

        private void LoadChildMenuItems(ContextMenuVM parentVM, List<MenuItemDefinition> children)
        {
            foreach (MenuItemDefinition menuItem in children)
            {
                menuItem.AddonId = parentVM.AddonId;

                ContextMenuVM contextMenuVM = new ContextMenuVM(menuItem);
                parentVM.Children.Add(contextMenuVM);
            }
        }

        #endregion

        #region 事件处理器

        #endregion
    }
}

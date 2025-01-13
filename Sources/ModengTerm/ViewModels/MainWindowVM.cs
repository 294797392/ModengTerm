using ModengTerm.Base;
using ModengTerm.Base.DataModels;
using ModengTerm.Base.Definitions;
using ModengTerm.Base.ServiceAgents;
using ModengTerm.DataModels;
using ModengTerm.Terminal.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using WPFToolkit.MVVM;

namespace ModengTerm.ViewModels
{
    public class MainWindowVM : ViewModelBase
    {
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
        public OpenedSessionsVM OpenedSessionsVM { get; private set; }

        /// <summary>
        /// 窗口顶部的菜单列表
        /// </summary>
        public BindableCollection<ContextMenuVM> TitleMenus { get; private set; }

        /// <summary>
        /// 窗口顶部的全局菜单，所有的会话都有的菜单
        /// </summary>
        public BindableCollection<ContextMenuVM> GlobalTitleMenus { get; private set; }

        /// <summary>
        /// 所有主题列表
        /// </summary>
        public BindableCollection<AppThemeVM> Themes { get; private set; }

        public Dictionary<PanelAlignEnum, PanelVM> Panels { get; private set; }

        #endregion

        #region 构造方法

        public MainWindowVM()
        {
            this.serviceAgent = MTermApp.Context.ServiceAgent;

            this.OpenedSessionsVM = new OpenedSessionsVM();

            List<XTermSession> sessions = this.serviceAgent.GetSessions();
            this.RecentlyOpenedSession = new BindableCollection<RecentlySessionVM>();
            List<RecentlySession> recentSessions = this.serviceAgent.GetRecentSessions();
            foreach (RecentlySession recentSession in recentSessions)
            {
                RecentlySessionVM recentlySessionVM = new RecentlySessionVM(recentSession);
                this.RecentlyOpenedSession.Add(recentlySessionVM);
            }

            this.TitleMenus = new BindableCollection<ContextMenuVM>();

            this.Themes = new BindableCollection<AppThemeVM>();
            this.Themes.AddRange(MTermApp.Context.Manifest.AppThemes.Select(v => new AppThemeVM(v)));
            this.Themes.SelectedItem = this.Themes[0];//.FirstOrDefault();

            this.GlobalTitleMenus = new BindableCollection<ContextMenuVM>();
            List<MenuItemRelation> globalMenuItemRelations = MTermApp.Context.Manifest.GlobalTitleMenus.Select(v => new MenuItemRelation(v.TitleParentID, v)).ToList();
            this.GlobalTitleMenus.AddRange(VTClientUtils.CreateContextMenuVM(globalMenuItemRelations));
            this.Panels = VTClientUtils.CreatePanels(MTermApp.Context.Manifest.GlobalTitleMenus);
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

        #region 事件处理器

        private void ContextMenuVisiblePanelContent_Click(ContextMenuVM sender, ShellSessionVM shellSessionVM)
        {
            PanelVM panelVM = this.Panels[sender.PanelAlign];
            panelVM.ChangeVisible(sender.ID.ToString());
        }

        #endregion
    }
}

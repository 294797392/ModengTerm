using DotNEToolkit;
using ModengTerm.Addon.Controls;
using ModengTerm.Base;
using ModengTerm.Base.Addon;
using ModengTerm.Base.DataModels;
using ModengTerm.Base.Metadatas;
using ModengTerm.Base.ServiceAgents;
using ModengTerm.ViewModel.Panels;
using ModengTerm.ViewModel.Terminal;
using System;
using System.Collections.Generic;
using System.Linq;
using WPFToolkit.MVVM;

namespace ModengTerm.ViewModel
{
    public class MainWindowVM : ViewModelBase
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("MainWindowVM");

        /// <summary>
        /// 最后一个打开会话的按钮
        /// </summary>
        public static readonly OpenSessionVM OpenSessionVM = new OpenSessionVM();

        private static MainWindowVM instance;

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
        /// 打开的所有会话列表，包含打开按钮
        /// </summary>
        public BindableCollection<SessionItemVM> SessionList { get; private set; }

        /// <summary>
        /// 打开的所有类型是Shell的会话列表
        /// </summary>
        public IEnumerable<ShellSessionVM> ShellSessions { get { return SessionList.OfType<ShellSessionVM>(); } }

        /// <summary>
        /// 选中的会话
        /// </summary>
        public SessionItemVM SelectedSession
        {
            get { return SessionList.SelectedItem; }
            set
            {
                if (SessionList.SelectedItem != value)
                {
                    SessionList.SelectedItem = value;
                    this.NotifyPropertyChanged("SelectedSession");
                }
            }
        }

        /// <summary>
        /// 窗口顶部的所有菜单列表
        /// </summary>
        public BindableCollection<ContextMenuVM> TitleMenus { get; private set; }

        /// <summary>
        /// 所有主题列表
        /// </summary>
        public BindableCollection<AppThemeVM> Themes { get; private set; }

        /// <summary>
        /// 面板容器
        /// </summary>
        public Dictionary<SidePanelDocks, PanelContainer> PanelContainers { get; private set; }

        #endregion

        #region 构造方法

        public MainWindowVM()
        {
            this.serviceAgent = ServiceAgentFactory.Get();

            this.PanelContainers = new Dictionary<SidePanelDocks, PanelContainer>();
            List<SidePanelDocks> docks = VTBaseUtils.GetEnumValues<SidePanelDocks>();
            foreach (SidePanelDocks dock in docks)
            {
                PanelContainer container = new PanelContainer();
                container.Dock = dock;
                this.PanelContainers[dock] = container;
            }

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
            this.TitleMenus.AddRange(VMUtils.CreateContextMenuVMs(true));

            this.Themes = new BindableCollection<AppThemeVM>();
            this.Themes.AddRange(ClientContext.Context.Manifest.AppThemes.Select(v => new AppThemeVM(v)));
            this.Themes.SelectedItem = this.Themes[0];//.FirstOrDefault();
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
            if (RecentlyOpenedSession.Count > VTBaseConsts.MaxRecentSessions)
            {
                RecentlySessionVM oldestSession = RecentlyOpenedSession[0];
                RecentlyOpenedSession.RemoveAt(0);
                serviceAgent.DeleteRecentSession(oldestSession.ID.ToString());
            }

            RecentlyOpenedSession.Add(new RecentlySessionVM(recentlySession));

            serviceAgent.AddRecentSession(recentlySession);
        }

        /// <summary>
        /// 从最近打开的会话列表里删除一个会话
        /// </summary>
        /// <param name="recentlySession"></param>
        public void DeleteRecentSession(RecentlySessionVM recentlySession)
        {
            RecentlyOpenedSession.Remove(recentlySession);

            serviceAgent.DeleteRecentSession(recentlySession.ID.ToString());
        }

        /// <summary>
        /// 添加一个SidePanel到主界面
        /// </summary>
        /// <param name="spvm"></param>
        public void AddSidePanel(SidePanelVM spvm)
        {
            this.PanelContainers[spvm.Dock].Panels.Add(spvm);
        }

        public void AddSidePanels(IEnumerable<SidePanelVM> spvms)
        {
            foreach (SidePanelVM spvm in spvms)
            {
                this.AddSidePanel(spvm);
            }
        }

        public void RemoveSidePanel(SidePanelVM spvm)
        {
            PanelContainer container = this.PanelContainers[spvm.Dock];
            container.Panels.Remove(spvm);
        }

        public void RemoveSidePanels(IEnumerable<SidePanelVM> spvms)
        {
            foreach (SidePanelVM spvm in spvms)
            {
                this.RemoveSidePanel(spvm);
            }
        }

        /// <summary>
        /// 创建一个SidePanel实例并添加到界面
        /// </summary>
        /// <param name="metadata"></param>
        public void CreateSidePanel(SidePanelMetadata metadata)
        {
            SidePanelVM spvm = VMUtils.CreateSidePanelVM(metadata);

            #region 创建Panel实例

            SidePanel sidePanel = null;

            try
            {
                sidePanel = ConfigFactory<SidePanel>.CreateInstance(spvm.Metadata.ClassName);
                sidePanel.Container = spvm;
                sidePanel.Initialize();
            }
            catch (Exception ex)
            {
                logger.Error("加载SidePanel异常", ex);
                return;
            }

            spvm.Panel = sidePanel;

            #endregion

            spvm.Initialize();

            this.AddSidePanel(spvm);
        }

        public SidePanelVM GetSidePanel(SidePanelMetadata metadata)
        {
            PanelContainer container = this.PanelContainers[metadata.Dock];
            return container.Panels.FirstOrDefault(v => v.Metadata == metadata);
        }

        #endregion

        #region 实例方法

        #endregion

        #region 事件处理器

        #endregion
    }
}

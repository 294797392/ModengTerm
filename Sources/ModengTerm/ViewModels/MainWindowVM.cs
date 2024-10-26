using ModengTerm.Base;
using ModengTerm.Base.DataModels;
using ModengTerm.Base.ServiceAgents;
using ModengTerm.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;
using WPFToolkit.MVVM;

namespace ModengTerm.ViewModels
{
    public class MainWindowVM : ViewModelBase
    {
        #region 实例变量

        private bool shellCommandPanelVisibility;
        private ServiceAgent serviceAgent;

        #endregion

        #region 属性

        public bool ShellCommandPanelVisiblity
        {
            get { return this.shellCommandPanelVisibility; }
            set
            {
                if (this.shellCommandPanelVisibility != value)
                {
                    this.shellCommandPanelVisibility = value;
                    this.NotifyPropertyChanged("ShellCommandPanelVisiblity");
                }
            }
        }

        /// <summary>
        /// 右侧窗格列表
        /// </summary>
        public MenuVM RightPanelMenu { get; private set; }

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
        public BindableCollection<SessionContextMenu> TitleMenus { get; private set; }

        #endregion

        #region 构造方法

        public MainWindowVM()
        {
            this.serviceAgent = MTermApp.Context.ServiceAgent;

            this.OpenedSessionsVM = new OpenedSessionsVM();

            this.RightPanelMenu = new MenuVM();
            this.RightPanelMenu.Initialize(new List<MenuDefinition>()
            {
                new MenuDefinition()
                {
                    Name = "快捷命令",
                    ClassName = "ModengTerm.UserControls.Terminals.ShellCommandUserControl, ModengTerm",
                }
            });
            this.RightPanelMenu.SelectedMenu = this.RightPanelMenu.MenuItems.FirstOrDefault();

            List<XTermSession> sessions = this.serviceAgent.GetSessions();
            this.RecentlyOpenedSession = new BindableCollection<RecentlySessionVM>();
            List<RecentlySession> recentSessions = this.serviceAgent.GetRecentSessions();
            foreach (RecentlySession recentSession in recentSessions)
            {
                RecentlySessionVM recentlySessionVM = new RecentlySessionVM(recentSession);
                this.RecentlyOpenedSession.Add(recentlySessionVM);
            }

            this.TitleMenus = new BindableCollection<SessionContextMenu>();
        }

        #endregion

        #region 公开接口

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
            if (this.RecentlyOpenedSession.Count > MTermConsts.MaxRecentSessions)
            {
                RecentlySessionVM oldestSession = this.RecentlyOpenedSession[0];
                this.RecentlyOpenedSession.RemoveAt(0);
                this.serviceAgent.DeleteRecentSession(oldestSession.ID.ToString());
            }

            this.RecentlyOpenedSession.Add(new RecentlySessionVM(recentlySession));

            this.serviceAgent.AddRecentSession(recentlySession);
        }

        public void DeleteRecentSession(RecentlySessionVM recentlySession)
        {
            this.RecentlyOpenedSession.Remove(recentlySession);

            this.serviceAgent.DeleteRecentSession(recentlySession.ID.ToString());
        }

        #endregion
    }
}

using ModengTerm.Base.DataModels;
using ModengTerm.Controls;
using ModengTerm.ServiceAgents;
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
        public BindableCollection<XTermSession> RecentlyOpenedSession { get; private set; }

        #endregion

        #region 构造方法

        public MainWindowVM()
        {
            this.serviceAgent = MTermApp.Context.ServiceAgent;

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

            this.RecentlyOpenedSession = new BindableCollection<XTermSession>();
        }

        #endregion

        #region 公开接口

        public void AddToRecentSession(XTermSession session)
        {
            this.RecentlyOpenedSession.Add(session);

            this.serviceAgent.AddRecentSession(session.ID);
        }

        #endregion
    }
}

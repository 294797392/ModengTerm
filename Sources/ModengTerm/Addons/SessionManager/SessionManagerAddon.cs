using ModengTerm.Base;
using ModengTerm.Base.DataModels;
using ModengTerm.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WPFToolkit.Utility;

namespace ModengTerm.Addons.SessionManager
{
    public class SessionManagerAddon : AddonBase
    {
        protected override void OnEvent(AddonEventTypes evt, params object[] evp)
        {
        }

        protected override void OnInitialize()
        {
            this.RegisterCommand("SessionManagerAddon.OpenSession", this.OpenSession);
            this.RegisterCommand("SessionManagerAddon.CreateSession", this.CreateSession);
            this.RegisterCommand("SessionManagerAddon.GroupManager", this.GroupManager);
        }

        protected override void OnRelease()
        {
        }

        private void OpenSession(CommandEventArgs e)
        {
            SessionListWindow sessionListWindow = new SessionListWindow();
            sessionListWindow.Owner = e.MainWindow;
            sessionListWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            if ((bool)sessionListWindow.ShowDialog())
            {
                XTermSession session = sessionListWindow.SelectedSession;
                e.Manager.OpenSession(session);
            }
        }

        private void CreateSession(CommandEventArgs e)
        {
            CreateSessionWindow window = new CreateSessionWindow();
            window.Owner = e.MainWindow;
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            if (!(bool)window.ShowDialog())
            {
                return;
            }

            XTermSession session = window.Session;

            // 在数据库里新建会话
            int code = e.ServiceAgent.AddSession(session);
            if (code != ResponseCode.SUCCESS)
            {
                MessageBoxUtils.Error("新建会话失败, 错误码 = {0}", code);
                return;
            }

            // 打开会话
            e.Manager.OpenSession(session);
        }

        private void GroupManager(CommandEventArgs e)
        {
            GroupManagerWindow window = new GroupManagerWindow();
            window.Owner = e.MainWindow;
            window.ShowDialog();
        }
    }
}

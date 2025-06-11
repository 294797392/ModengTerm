using ModengTerm.Addons;
using ModengTerm.Addons.Shell;
using ModengTerm.Base;
using ModengTerm.Base.Addon;
using ModengTerm.Base.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WPFToolkit.Utility;

namespace ModengTerm.OfficialAddons.SessionManager
{
    public class SessionManagerAddon : AddonModule
    {
        protected override void OnActive(ActiveContext e)
        {
            this.RegisterCommand("SessionManagerAddon.OpenSession", this.OpenSession);
            this.RegisterCommand("SessionManagerAddon.CreateSession", this.CreateSession);
            this.RegisterCommand("SessionManagerAddon.GroupManager", this.GroupManager);
        }

        protected override void OnDeactive()
        {
        }

        private void OpenSession(CommandArgs e)
        {
            throw new RefactorImplementedException();
            //SessionListWindow sessionListWindow = new SessionListWindow();
            //sessionListWindow.Owner = e.MainWindow;
            //sessionListWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            //if ((bool)sessionListWindow.ShowDialog())
            //{
            //    IShellService shell = ShellFactory.GetService();
            //    XTermSession session = sessionListWindow.SelectedSession;
            //    shell.OpenSession(session);
            //}
        }

        private void CreateSession(CommandArgs e)
        {
            throw new RefactorImplementedException();
            //CreateSessionWindow window = new CreateSessionWindow();
            //window.Owner = e.MainWindow;
            //window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            //if (!(bool)window.ShowDialog())
            //{
            //    return;
            //}

            //XTermSession session = window.Session;

            //// 在数据库里新建会话
            //int code = e.ServiceAgent.AddSession(session);
            //if (code != ResponseCode.SUCCESS)
            //{
            //    MessageBoxUtils.Error("新建会话失败, 错误码 = {0}", code);
            //    return;
            //}

            //// 打开会话
            //IShellService shell = ShellFactory.GetService();
            //shell.OpenSession(session);
        }

        private void GroupManager(CommandArgs e)
        {
            throw new RefactorImplementedException();
            //GroupManagerWindow window = new GroupManagerWindow();
            //window.Owner = e.MainWindow;
            //window.ShowDialog();
        }
    }
}

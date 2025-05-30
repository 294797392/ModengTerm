using ModengTerm.Base.DataModels;
using ModengTerm.Base.Definitions;
using ModengTerm.Document;
using ModengTerm.Terminal;
using ModengTerm.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ModengTerm.Addons.Shell
{
    public class DefaultShellImpl : AbstractShell
    {
        private Window window;

        public DefaultShellImpl()
        {
            window = Application.Current.MainWindow;
        }

        /// <summary>
        /// 打开会话
        /// </summary>
        /// <param name="session"></param>
        public override void OpenSession(XTermSession session)
        {
            MCommands.OpenSessionCommand.Execute(session, window);
        }

        /// <summary>
        /// 显示或隐藏Panel
        /// </summary>
        /// <param name="panelId">要显示或隐藏的PanelId</param>
        public override void VisiblePanel(string panelId)
        {
            MTermApp.Context.MainWindowVM.PanelContainer.ChangeVisible(panelId);
        }

        /// <summary>
        /// 获取当前显示的会话
        /// </summary>
        /// <returns></returns>
        public override IAddonSession GetCurrentSession()
        {
            return MTermApp.Context.MainWindowVM.SelectedSession as IAddonSession;
        }

        /// <summary>
        /// 获取指定的会话列表
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override List<T> GetSessions<T>()
        {
            return MTermApp.Context.MainWindowVM.SessionList.OfType<T>().ToList();
        }
    }
}

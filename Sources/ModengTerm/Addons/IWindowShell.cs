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

namespace ModengTerm.Addons
{
    /// <summary>
    /// 封装对界面的操作
    /// </summary>
    public class IWindowShell
    {
        private Window window;

        public IWindowShell()
        {
            this.window = Application.Current.MainWindow;
        }

        /// <summary>
        /// 打开会话
        /// </summary>
        /// <param name="session"></param>
        public void OpenSession(XTermSession session)
        {
            MCommands.OpenSessionCommand.Execute(session, this.window);
        }

        /// <summary>
        /// 显示或隐藏Panel
        /// </summary>
        /// <param name="panelId">要显示或隐藏的PanelId</param>
        public void VisiblePanel(string panelId)
        {
            MTermApp.Context.MainWindowVM.LeftPanelContainer.ChangeVisible(panelId);
        }

        /// <summary>
        /// 新增一个Panel
        /// </summary>
        /// <param name="panelDefinition"></param>
        public void AddPanel(PanelVM panel)
        {

        }

        public void RemovePanel(string panelId)
        {

        }

        /// <summary>
        /// 获取当前显示的会话
        /// </summary>
        /// <returns></returns>
        public IAddonSession GetCurrentSession()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 获取指定的会话列表
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public List<T> GetSessions<T>() where T : IAddonSession
        {
            throw new NotImplementedException();
        }
    }
}

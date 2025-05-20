using ModengTerm.Base.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ModengTerm.Addons
{
    /// <summary>
    /// 执行某个动作
    /// </summary>
    public class ApplicationManager
    {
        private Window window;

        public ApplicationManager() 
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
            MTermApp.Context.MainWindowVM.Panel.ChangeVisible(panelId);
        }
    }
}

using ModengTerm.Base;
using ModengTerm.Base.Addon;
using ModengTerm.Base.DataModels;
using ModengTerm.Base.Definitions;
using ModengTerm.Document;
using ModengTerm.Terminal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ModengTerm.Addons.Shell
{
    public class ShellServiceImpl : IShellService
    {
        private Window window;

        public ShellServiceImpl()
        {
            window = Application.Current.MainWindow;
        }

        public override void OpenSession(XTermSession session)
        {
            throw new RefactorImplementedException();
            //MCommands.OpenSessionCommand.Execute(session, window);
        }

        public override void VisiblePanel(string panelId)
        {
            throw new RefactorImplementedException();
            //VTApp.Context.MainWindowVM.PanelContainer.ChangeVisible(panelId);
        }

        public override T GetActiveShell<T>()
        {
            throw new NotImplementedException();
            //return MTermApp.Context.MainWindowVM.SelectedSession;
        }

        public override List<T> GetShellObjects<T>()
        {
            throw new RefactorImplementedException();
            //return VTApp.Context.MainWindowVM.SessionList.OfType<T>().ToList();
        }
    }
}

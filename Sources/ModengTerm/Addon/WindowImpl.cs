using ModengTerm.Addon.Interactive;
using ModengTerm.Base.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Addon
{
    public class WindowImpl : IWindow
    {
        public override T GetActivePanel<T>()
        {
            throw new NotImplementedException();
        }

        public override List<IPanel> GetAllPanels()
        {
            throw new NotImplementedException();
        }

        public override void OpenSession(XTermSession session)
        {
            MCommands.OpenSessionCommand.Execute(session, App.Current.MainWindow);
        }

        public override void VisiblePanel(string panelId)
        {
            throw new NotImplementedException();
        }
    }
}

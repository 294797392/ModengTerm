using ModengTerm.Terminal.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Addons.SessionExplorer
{
    public class SessionExplorerAddon : AddonModule
    {
        protected override void OnInitialize()
        {
            this.RegisterCommand("SessionExplorerAddon.OpenExplorerWindow", OpenExplorerWindow);
        }

        protected override void OnRelease()
        {
        }

        private void OpenExplorerWindow(CommandEventArgs e)
        {
            e.Manager.VisiblePanel("BF1AD31C-0E00-495D-9C19-7687D708B71F");
        }
    }
}

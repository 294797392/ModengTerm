using ModengTerm.Addons;
using ModengTerm.Addons.Shell;
using ModengTerm.Base.Addon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.OfficialAddons.SessionExplorer
{
    public class SessionExplorerAddon : AddonModule
    {
        protected override void OnActive(ActiveContext e)
        {
            this.RegisterCommand("SessionExplorerAddon.OpenExplorerWindow", OpenExplorerWindow);
        }

        protected override void OnDeactive()
        {
        }

        private void OpenExplorerWindow(CommandArgs e)
        {
            IWindow shell = ShellFactory.GetWindow();
            shell.VisiblePanel("BF1AD31C-0E00-495D-9C19-7687D708B71F");
        }
    }
}

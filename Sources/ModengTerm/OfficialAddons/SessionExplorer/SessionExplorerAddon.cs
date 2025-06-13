using ModengTerm.Addon;
using ModengTerm.Addon.Interactive;
using ModengTerm.Addons;
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
            ObjectFactory factory = ObjectFactory.GetFactory();
            IHostWindow window = factory.GetHostWindow();
            window.VisiblePanel("BF1AD31C-0E00-495D-9C19-7687D708B71F");
        }
    }
}

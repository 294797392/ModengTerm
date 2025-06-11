using ModengTerm.Addons;
using ModengTerm.Addons.Shell;
using ModengTerm.Base.Addon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Addon.Official.SystemMonitor
{
    public class SystemMonitorAddon : AddonModule
    {
        protected override void OnActive(ActiveContext e)
        {
            this.RegisterCommand("SystemMonitorAddon.ShowSystemMonitorPanel", ExecuteShowSystemMonitorPanelCommand);
        }

        protected override void OnDeactive()
        {
        }

        private void ExecuteShowSystemMonitorPanelCommand(CommandArgs e)
        {
            IWindow shell = ShellFactory.GetWindow();
            shell.VisiblePanel("A86C3967-8CDC-4D0E-8CB6-010364CFCC23");
        }
    }
}

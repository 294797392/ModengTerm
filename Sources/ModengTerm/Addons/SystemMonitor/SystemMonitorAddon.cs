using ModengTerm.Terminal.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Addons.SystemMonitor
{
    public class SystemMonitorAddon : AddonModule
    {
        protected override void OnInitialize()
        {
            this.RegisterCommand("SystemMonitorAddon.ShowSystemMonitorPanel", ExecuteShowSystemMonitorPanelCommand);
        }

        protected override void OnRelease()
        {
        }

        private void ExecuteShowSystemMonitorPanelCommand(CommandEventArgs e)
        {
            e.Manager.VisiblePanel("A86C3967-8CDC-4D0E-8CB6-010364CFCC23");
        }
    }
}

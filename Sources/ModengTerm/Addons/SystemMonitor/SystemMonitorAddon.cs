using ModengTerm.Terminal.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Addons.SystemMonitor
{
    public class SystemMonitorAddon : AddonBase
    {
        protected override void OnInitialize()
        {
            RegisterCommand("SystemMonitorAddon.ShowSystemMonitorPanel", ExecuteShowSystemMonitorPanelCommand);
        }

        protected override void OnRelease()
        {
        }

        protected override void OnEvent(AddonEventTypes ev, params object[] param)
        {

        }

        private void ExecuteShowSystemMonitorPanelCommand()
        {
            //ShellSessionVM shellSessionVM = MainWindow.SelectedSession as ShellSessionVM;
            //shellSessionVM.Panel.ChangeVisible("A86C3967-8CDC-4D0E-8CB6-010364CFCC23");
        }
    }
}

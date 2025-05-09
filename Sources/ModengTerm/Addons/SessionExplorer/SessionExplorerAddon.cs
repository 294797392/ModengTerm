using ModengTerm.Terminal.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Addons.SessionExplorer
{
    public class SessionExplorerAddon : AddonBase
    {
        protected override void OnInitialize()
        {
            RegisterCommand("SessionExplorerAddon.OpenExplorerWindow", OpenExplorerWindow);
        }

        protected override void OnRelease()
        {
        }

        protected override void OnEvent(AddonEventTypes ev, params object[] param)
        {
        }

        private void OpenExplorerWindow(CommandEventArgs context)
        {
            MTermApp.Context.MainWindowVM.Panel.ChangeVisible("BF1AD31C-0E00-495D-9C19-7687D708B71F");
        }
    }
}

using ModengTerm.Terminal.ViewModels;
using ModengTerm.Windows.Terminals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Addons.BroadcastInput
{
    public class BroadcastInputAddon : AddonBase
    {
        protected override void OnInitialize()
        {
            RegisterCommand("BroadcastInputAddon.OpenBroadcastInputWindow", ExecuteOpenBroadcastInputWindowCommand);
        }

        protected override void OnRelease()
        {
        }

        protected override void OnEvent(AddonEventTypes ev, params object[] param)
        {
        }

        private void ExecuteOpenBroadcastInputWindowCommand()
        {
            //ShellSessionVM shellSessionVM = MainWindow.SelectedSession as ShellSessionVM;
            //BroadcastInputManagerWindow multiInputManagerWindow = new BroadcastInputManagerWindow(shellSessionVM);
            //multiInputManagerWindow.Owner = System.Windows.Application.Current.MainWindow;
            //multiInputManagerWindow.ShowDialog();
        }
    }
}

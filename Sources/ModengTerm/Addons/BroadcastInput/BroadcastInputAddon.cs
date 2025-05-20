using ModengTerm.Terminal.ViewModels;
using ModengTerm.ViewModels;
using ModengTerm.Windows.Terminals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ModengTerm.Addons.BroadcastInput
{
    public class BroadcastInputAddon : AddonBase
    {
        protected override void OnInitialize()
        {
            this.RegisterCommand("BroadcastInputAddon.OpenBroadcastInputWindow", ExecuteOpenBroadcastInputWindowCommand);
        }

        protected override void OnRelease()
        {
        }

        protected override void OnEvent(AddonEventTypes ev, params object[] param)
        {
        }

        private void ExecuteOpenBroadcastInputWindowCommand(CommandEventArgs context)
        {
            ShellSessionVM shellSessionVM = MTermApp.Context.MainWindowVM.SelectedSession as ShellSessionVM;
            BroadcastInputManagerWindow window = new BroadcastInputManagerWindow(shellSessionVM);
            window.Owner = context.MainWindow;
            window.ShowDialog();
        }
    }
}

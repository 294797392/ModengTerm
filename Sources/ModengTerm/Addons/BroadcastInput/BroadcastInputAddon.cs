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
    public class BroadcastInputAddon : AddonModule
    {
        protected override void OnInitialize()
        {
            this.RegisterCommand("BroadcastInputAddon.OpenBroadcastInputWindow", this.OpenBroadcastInputWindow);
        }

        protected override void OnRelease()
        {
        }

        private void OpenBroadcastInputWindow(CommandEventArgs e)
        {
            ShellSessionVM shellSessionVM = e.OpenedSession as ShellSessionVM;
            BroadcastInputManagerWindow window = new BroadcastInputManagerWindow(shellSessionVM);
            window.Owner = e.MainWindow;
            window.ShowDialog();
        }
    }
}

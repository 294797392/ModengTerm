using ModengTerm.Addons.Shell;
using ModengTerm.Document.Enumerations;
using ModengTerm.Terminal.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Addons.QuickInput
{
    public class QuickInputAddon : AddonModule
    {
        protected override void OnActive(ActiveContext e)
        {
            this.RegisterCommand("QuickInputAddon.ShowQuickInputPanel", this.ShowQuickInputPanel);
        }

        protected override void OnDeactive()
        {
        }

        private void ShowQuickInputPanel(CommandArgs e)
        {
            AbstractShell shell = ShellFactory.GetShell();
            IAddonSession session = shell.GetCurrentSession();
            session.VisiblePanel("0C1F6D60-F6ED-4D01-B5B6-1812EA55286A");
        }
    }
}

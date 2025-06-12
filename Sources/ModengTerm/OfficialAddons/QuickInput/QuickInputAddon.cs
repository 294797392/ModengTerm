using ModengTerm.Addon;
using ModengTerm.Addon.Interactive;
using ModengTerm.Addons;
using ModengTerm.Document.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.OfficialAddons.QuickInput
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
            ObjectFactory factory = ObjectFactory.GetFactory();
            IShellPanel shell = factory.GetActivePanel<IShellPanel>();
            shell.VisiblePanel("0C1F6D60-F6ED-4D01-B5B6-1812EA55286A");
        }
    }
}

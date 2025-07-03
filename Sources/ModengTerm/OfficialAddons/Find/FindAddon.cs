using ModengTerm.Addon.Interactive;
using ModengTerm.Addons;
using ModengTerm.Base;
using System.Drawing.Text;

namespace ModengTerm.OfficialAddons.Find
{
    public class FindAddon : AddonModule
    {
        protected override void OnActive(ActiveContext e)
        {
            this.RegisterCommand("FindAddon.Find", this.FindCommandExecuted);
        }

        protected override void OnDeactive()
        {
        }


        private void FindCommandExecuted(CommandArgs e)
        {
            IClientShellTab shellTab = e.ActiveTab as IClientShellTab;
            IOverlayPanel overlayPanel = shellTab.GetOverlayPanel("FindOverlayPanel");
            overlayPanel.SwitchStatus();
        }
    }
}

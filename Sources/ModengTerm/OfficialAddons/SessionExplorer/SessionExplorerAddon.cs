using ModengTerm.Addon;
using ModengTerm.Addon.Interactive;
using ModengTerm.Addons;

namespace ModengTerm.OfficialAddons.SessionExplorer
{
    public class SessionExplorerAddon : AddonModule
    {
        protected override void OnActive(ActiveContext e)
        {
            this.RegisterCommand("SessionExplorerAddon.OpenExplorerWindow", OpenExplorerWindow);
        }

        protected override void OnDeactive()
        {
        }

        private void OpenExplorerWindow(CommandArgs e)
        {
            ISidePanel sidePanel = this.GetSidePanel("ResourceManagerPanel");
            sidePanel.SwitchStatus();
        }
    }
}

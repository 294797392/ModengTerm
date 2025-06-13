using ModengTerm.Addon;
using ModengTerm.Addon.Interactive;
using ModengTerm.Addons;

namespace ModengTerm.OfficialAddons.SessionExplorer
{
    public class SessionExplorerAddon : AddonModule
    {
        protected override void OnActive(ActiveContext e)
        {
            this.RegisterEvent(HostEvent.HOST_APP_INITIALIZED, this.OnAppInitialized);
            this.RegisterCommand("SessionExplorerAddon.OpenExplorerWindow", OpenExplorerWindow);
        }

        protected override void OnDeactive()
        {
        }

        private void OpenExplorerWindow(CommandArgs e)
        {
            IHostSidePanel sidePanel = this.GetSidePanel("ResourceManagerPanel");
            sidePanel.SwitchStatus();
        }

        private void OnAppInitialized(HostEvent evType, HostEventArgs evArgs)
        {
            IHostSidePanel sidePanel = this.GetSidePanel("ResourceManagerPanel");
            this.hostWindow.AddSidePanel(sidePanel);
        }
    }
}

using ModengTerm.Addon;
using ModengTerm.Addon.Interactive;
using ModengTerm.Addons;

namespace ModengTerm.OfficialAddons.SessionExplorer
{
    public class SessionExplorerAddon : AddonModule
    {
        protected override void OnActive(ActiveContext e)
        {
            this.eventRegistry.SubscribeEvent(ClientEvent.CLIENT_INITIALIZED, this.OnClientInitialized);
            this.RegisterCommand("SessionExplorerAddon.OpenExplorerWindow", OpenExplorerWindow);
        }

        protected override void OnDeactive()
        {
            this.eventRegistry.UnsubscribeEvent(ClientEvent.CLIENT_INITIALIZED, this.OnClientInitialized);
        }

        private void OpenExplorerWindow(CommandArgs e)
        {
            ISidePanel sidePanel = this.GetSidePanel("ResourceManagerPanel");
            sidePanel.SwitchStatus();
        }

        private void OnClientInitialized(ClientEventArgs evArgs)
        {
            ISidePanel sidePanel = this.GetSidePanel("ResourceManagerPanel");
            this.client.AddSidePanel(sidePanel);
        }
    }
}

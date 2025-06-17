using ModengTerm.Addon;
using ModengTerm.Addon.Interactive;
using ModengTerm.Addons;

namespace ModengTerm.OfficialAddons.SessionExplorer
{
    public class SessionExplorerAddon : AddonModule
    {
        protected override void OnActive(ActiveContext e)
        {
            this.eventRegistory.SubscribeEvent(ClientEvent.CLIENT_INITIALIZED, this.OnClientInitialized);
            this.RegisterCommand("SessionExplorerAddon.OpenExplorerWindow", OpenExplorerWindow);
        }

        protected override void OnDeactive()
        {
            this.eventRegistory.UnsubscribeEvent(ClientEvent.CLIENT_INITIALIZED, this.OnClientInitialized);
        }

        private void OpenExplorerWindow(CommandArgs e)
        {
            ISidePanel sidePanel = this.GetSidePanel("ResourceManagerPanel");
            sidePanel.SwitchStatus();
        }

        private void OnClientInitialized(ClientEvent evType, ClientEventArgs evArgs)
        {
            ISidePanel sidePanel = this.GetSidePanel("ResourceManagerPanel");
            this.hostWindow.AddSidePanel(sidePanel);
        }
    }
}

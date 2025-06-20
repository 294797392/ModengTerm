using ModengTerm.Addon.Interactive;
using ModengTerm.Addons;

namespace ModengTerm.OfficialAddons.Find
{
    public class FindAddon : AddonModule
    {
        protected override void OnActive(ActiveContext e)
        {
            this.eventRegistry.RegisterHotkey(this, "Esc", HotkeyScopes.ClientShellTab, this.OnEscKeyDown);
            this.eventRegistry.RegisterHotkey(this, "Ctrl+F", HotkeyScopes.ClientShellTab, this.OnAltFKeyDown);
            this.RegisterCommand("FindAddon.Find", this.FindCommandExecuted);
        }

        protected override void OnDeactive()
        {
        }


        private void FindCommandExecuted(CommandArgs e)
        {
            IClientShellTab shellTab = e.ActiveTab as IClientShellTab;
            if (shellTab == null)
            {
                return;
            }

            IOverlayPanel overlayPanel = this.EnsureOverlayPanel("FindOverlayPanel", shellTab);
            overlayPanel.Dock = OverlayPanelDocks.RightTop;
            overlayPanel.SwitchStatus();
        }

        private void OnEscKeyDown()
        {
            IClientShellTab shellTab = this.client.GetActiveTab<IClientShellTab>();
            IOverlayPanel overlayPanel = shellTab.GetOverlayPanel("FindOverlayPanel");

            if (overlayPanel == null)
            {
                return;
            }

            if (overlayPanel.IsOpened)
            {
                overlayPanel.Close();
            }
        }

        private void OnAltFKeyDown() 
        {
            IClientShellTab shellTab = this.client.GetActiveTab<IClientShellTab>();
            IOverlayPanel overlayPanel = this.EnsureOverlayPanel("FindOverlayPanel", shellTab);
            overlayPanel.Dock = OverlayPanelDocks.RightTop;
            overlayPanel.SwitchStatus();
        }
    }
}

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
            this.eventRegistry.SubscribeEvent(ClientEvent.CLIENT_INITIALIZED, this.OnClientInitialized);
            this.RegisterCommand("QuickInputAddon.ShowQuickInputPanel", this.ShowQuickInputPanel);
        }

        protected override void OnDeactive()
        {
            this.eventRegistry.SubscribeEvent(ClientEvent.CLIENT_INITIALIZED, this.OnClientInitialized);
        }

        #region 事件处理器

        private void ShowQuickInputPanel(CommandArgs e)
        {
            ISidePanel sidePanel = this.GetSidePanel("QuickInputSidePanel");
            sidePanel.SwitchStatus();
        }

        private void OnClientInitialized(ClientEventArgs e) 
        {

        }

        #endregion
    }
}

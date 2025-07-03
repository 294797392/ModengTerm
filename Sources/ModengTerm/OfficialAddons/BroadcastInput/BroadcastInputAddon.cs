using ModengTerm.Addon;
using ModengTerm.Addon.Interactive;
using ModengTerm.Addons;
using ModengTerm.Base.Enumerations;
using System.Collections.Generic;
using System.Windows;

namespace ModengTerm.OfficialAddons.BroadcastInput
{
    public class BroadcastInputAddon : AddonModule
    {
        public const string KEY_BROADCAST_LIST = "broadcasts";

        #region 实例变量

        #endregion

        #region AddonModule

        protected override void OnActive(ActiveContext context)
        {
            this.eventRegistry.SubscribeTabEvent("OnTabShellSendUserInput", this.OnShellSendUserInput);
            this.RegisterCommand("BroadcastInputAddon.OpenBroadcastInputWindow", this.OpenBroadcastInputWindow);
        }

        protected override void OnDeactive()
        {
            this.eventRegistry.UnsubscribeTabEvent("OnTabShellSendUserInput", this.OnShellSendUserInput);
        }

        #endregion

        #region 实例方法

        #endregion

        #region 事件处理器

        private void OpenBroadcastInputWindow(CommandArgs e)
        {
            ISidePanel sidePanel = this.GetSidePanel("5A7D79FD-3C0E-44B2-80AE-B0FEE9F3ECAB");
            sidePanel.SwitchStatus();
        }

        private void OnShellSendUserInput(TabEventArgs e, object userData)
        {
            TabEventShellSendUserInput sendData = e as TabEventShellSendUserInput;

            List<ShellTabVM> broadcastTabs = e.Sender.GetData<List<ShellTabVM>>(this, KEY_BROADCAST_LIST);

            if (broadcastTabs == null)
            {
                return;
            }

            foreach (ShellTabVM shellTabVM in broadcastTabs)
            {
                if (!shellTabVM.IsChecked)
                {
                    continue;
                }

                IClientShellTab shellTab = shellTabVM.Tab;

                if (shellTab.Status != SessionStatusEnum.Connected)
                {
                    continue;
                }

                shellTab.Send(sendData.Buffer);
            }
        }

        #endregion
    }
}

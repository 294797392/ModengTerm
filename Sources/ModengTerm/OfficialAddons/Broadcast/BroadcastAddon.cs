using ModengTerm.Addon;
using ModengTerm.Addon.Interactive;
using ModengTerm.Base.Enumerations;
using System.Collections.Generic;
using System.Windows;

namespace ModengTerm.OfficialAddons.Broadcast
{
    public class BroadcastAddon : AddonModule
    {
        public const string KEY_BROADCAST_LIST = "broadcasts";

        #region 实例变量

        #endregion

        #region AddonModule

        protected override void OnActive(ActiveContext context)
        {
            this.eventRegistry.SubscribeTabEvent("OnTabShellSendUserInput", this.OnShellSendUserInput);
        }

        protected override void OnDeactive()
        {
            this.eventRegistry.UnsubscribeTabEvent("OnTabShellSendUserInput", this.OnShellSendUserInput);
        }

        #endregion

        #region 实例方法

        #endregion

        #region 事件处理器

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

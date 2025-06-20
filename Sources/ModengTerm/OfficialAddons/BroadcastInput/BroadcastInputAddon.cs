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
            this.eventRegistry.SubscribeTabEvent(TabEvent.SHELL_SENDDATA, this.OnShellSendData);
            this.eventRegistry.SubscribeTabEvent(TabEvent.SHELL_OPENED, this.OnShellOpened);
            this.RegisterCommand("BroadcastInputAddon.OpenBroadcastInputWindow", this.OpenBroadcastInputWindow);
        }

        protected override void OnDeactive()
        {
            this.eventRegistry.UnsubscribeTabEvent(TabEvent.SHELL_SENDDATA, this.OnShellSendData);
            this.eventRegistry.UnsubscribeTabEvent(TabEvent.SHELL_OPENED, this.OnShellOpened);
        }

        #endregion

        #region 实例方法

        #endregion

        #region 事件处理器

        private void OpenBroadcastInputWindow(CommandArgs e)
        {
            BroadcastInputManagerWindow broadcastInputManagerWindow = new BroadcastInputManagerWindow(e.ActiveTab as IClientShellTab, this);
            broadcastInputManagerWindow.Owner = Application.Current.MainWindow;
            if ((bool)broadcastInputManagerWindow.ShowDialog())
            {
                e.ActiveTab.SetData(this, KEY_BROADCAST_LIST, broadcastInputManagerWindow.BroadcastList);
            }
        }

        private void OnShellOpened(TabEventArgs e)
        {
            TabEventShellOpened tabOpened = e as TabEventShellOpened;
        }

        private void OnShellSendData(TabEventArgs e)
        {
            TabEventShellSendData sendData = e as TabEventShellSendData;

            List<IClientShellTab> broadcastTabs = e.Sender.GetData<List<IClientShellTab>>(this, KEY_BROADCAST_LIST);

            if (broadcastTabs == null)
            {
                return;
            }

            foreach (IClientShellTab shellTab in broadcastTabs)
            {
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

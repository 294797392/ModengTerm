using ModengTerm.Addon;
using ModengTerm.Addon.Interactive;
using ModengTerm.Addons;
using ModengTerm.Base.Enumerations;
using System;
using System.Collections.Generic;

namespace ModengTerm.OfficialAddons.BroadcastInput
{
    public class BroadcastInputAddon : AddonModule
    {
        #region 实例变量

        #endregion

        #region AddonModule

        protected override void OnActive(ActiveContext context)
        {
            this.eventRegistory.SubscribeTabEvent(TabEvent.SHELL_SENDDATA, this.OnShellSendData);
            this.eventRegistory.SubscribeEvent(ClientEvent.CLIENT_TAB_OPENED, this.OnTabOpened);
            this.RegisterCommand("BroadcastInputAddon.OpenBroadcastInputWindow", this.OpenBroadcastInputWindow);
        }

        protected override void OnDeactive()
        {
            this.eventRegistory.UnsubscribeEvent(ClientEvent.CLIENT_TAB_OPENED, this.OnTabOpened);
        }

        #endregion

        #region 事件处理器

        private void OpenBroadcastInputWindow(CommandArgs e)
        {
            //IClientShellTab activeTab = this.client.GetActiveTab<IClientShellTab>();
            //List<IClientShellTab> allTabs = this.client.GetAllTabs<IClientShellTab>();
            //List<BroadcastSessionVM> broadcastSessions = this.broadcastSessions.ToList();

            //BroadcastInputManagerWindow window = new BroadcastInputManagerWindow(broadcastSessions, allTabs);
            //window.StorageService = this.storageService;
            //window.Owner = Application.Current.MainWindow;
            //if ((bool)window.ShowDialog())
            //{
            //    this.LoadBroadcastList();
            //}
        }

        private void OnTabOpened(ClientEventArgs e)
        {
            ClientEventTabOpened tabOpened = e as ClientEventTabOpened;

            switch (tabOpened.SessionType)
            {
                case SessionTypeEnum.SFTP: return;
                case SessionTypeEnum.SSH:
                case SessionTypeEnum.Tcp:
                case SessionTypeEnum.Localhost:
                case SessionTypeEnum.SerialPort:
                    break;

                default:
                    throw new NotImplementedException();
            }

            IClientShellTab activeTab = this.client.GetActiveTab<IClientShellTab>();
            List<IClientShellTab> allTabs = this.client.GetAllTabs<IClientShellTab>();

            List<BroadcastSession> broadcastSessions = this.storageService.GetObjects<BroadcastSession>(activeTab.ID.ToString());

            List<IClientShellTab> broadcastTabs = new List<IClientShellTab>();

            foreach (IClientShellTab tab in allTabs)
            {
                if (tab == activeTab)
                {
                    continue;
                }

                broadcastTabs.Add(tab);
            }

            activeTab.SetData(this, "broadcasts", broadcastTabs);
        }

        private void OnShellSendData(TabEventArgs e)
        {
            TabEventShellSendData sendData = e as TabEventShellSendData;

            List<IClientShellTab> broadcastTabs = e.ClientTab.GetData<List<IClientShellTab>>(this, "broadcasts");

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

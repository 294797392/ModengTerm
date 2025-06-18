using ModengTerm.Addon;
using ModengTerm.Addon.Interactive;
using ModengTerm.Addons;
using ModengTerm.Base;
using ModengTerm.Base.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using WPFToolkit.MVVM;

namespace ModengTerm.OfficialAddons.BroadcastInput
{
    public class BroadcastInputAddon : AddonModule
    {
        #region 实例变量

        private BindableCollection<BroadcastSessionVM> broadcastSessions;

        #endregion

        #region AddonModule

        protected override void OnActive(ActiveContext context)
        {
            this.broadcastSessions = new BindableCollection<BroadcastSessionVM>();

            this.eventRegistory.SubscribeEvent(ClientEvent.CLIENT_TAB_OPENED, this.OnTabOpened);
            this.RegisterCommand("BroadcastInputAddon.OpenBroadcastInputWindow", this.OpenBroadcastInputWindow);
        }

        protected override void OnDeactive()
        {
            this.eventRegistory.UnsubscribeEvent(ClientEvent.CLIENT_TAB_OPENED, this.OnTabOpened);
        }

        #endregion

        #region 实例方法

        private void LoadBroadcastList()
        {
            this.broadcastSessions.Clear();

            IClientShellTab activePanel = this.hostWindow.GetActiveTab<IClientShellTab>();
            List<IClientShellTab> allPanels = this.hostWindow.GetAllTabs<IClientShellTab>();

            List<BroadcastSession> broadcastSessions = this.storageService.GetObjects<BroadcastSession>(activePanel.ID.ToString());

            foreach (IClientShellTab panel in allPanels)
            {
                if (panel == activePanel)
                {
                    continue;
                }

                BroadcastSessionVM broadcastSession = new BroadcastSessionVM()
                {
                    ID = panel.ID.ToString(),
                    Name = panel.Name,
                    BroadcasePanel = panel
                };

                this.broadcastSessions.Add(broadcastSession);
            }
        }

        #endregion

        #region 事件处理器

        private void OpenBroadcastInputWindow(CommandArgs e)
        {
            IClientShellTab activeTab = this.hostWindow.GetActiveTab<IClientShellTab>();
            List<IClientShellTab> allTabs = this.hostWindow.GetAllTabs<IClientShellTab>();
            List<BroadcastSessionVM> broadcastSessions = this.broadcastSessions.ToList();

            BroadcastInputManagerWindow window = new BroadcastInputManagerWindow(broadcastSessions, allTabs);
            window.StorageService = this.storageService;
            window.Owner = Application.Current.MainWindow;
            if ((bool)window.ShowDialog())
            {
                this.LoadBroadcastList();
            }
        }

        /// <summary>
        /// 当用户输入之后触发
        /// </summary>
        /// <param name="e"></param>
        private void OnUserInput(CommandArgs e)
        {
            foreach (BroadcastSessionVM broadcastSession in this.broadcastSessions)
            {
                IClientShellTab broadcastPanel = broadcastSession.BroadcasePanel;

                if (broadcastPanel.Status != SessionStatusEnum.Connected)
                {
                    continue;
                }

                byte[] bytes = e.Argument as byte[];

                broadcastPanel.Send(bytes);
            }
        }

        private void OnTabOpened(ClientEvent evType, ClientEventArgs evArgs)
        {
            ClientEventTabOpened tabOpened = evArgs as ClientEventTabOpened;

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

            this.LoadBroadcastList();
        }

        #endregion
    }
}

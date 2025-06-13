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

            this.RegisterEvent(HostEvent.HOST_SESSION_OPENED, this.OnShellSessionOpened);
            this.RegisterCommand("BroadcastInputAddon.OpenBroadcastInputWindow", this.OpenBroadcastInputWindow);
        }

        protected override void OnDeactive()
        {
        }

        #endregion

        #region 实例方法

        private void LoadBroadcastList()
        {
            this.broadcastSessions.Clear();

            HostFactory factory = HostFactory.GetFactory();
            StorageService storageSvc = factory.GetStorageService();
            IShellTab activePanel = factory.GetActiveTab<IShellTab>();
            List<IShellTab> allPanels = factory.GetAllTabs<IShellTab>();

            List<BroadcastSession> broadcastSessions = storageSvc.GetObjects<BroadcastSession>(activePanel.Id);

            foreach (IShellTab panel in allPanels)
            {
                if (panel == activePanel)
                {
                    continue;
                }

                BroadcastSessionVM broadcastSession = new BroadcastSessionVM()
                {
                    ID = panel.Id,
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
            HostFactory factory = HostFactory.GetFactory();
            IShellTab activePanel = factory.GetActiveTab<IShellTab>();
            List<IShellTab> allPanels = factory.GetAllTabs<IShellTab>();
            List<BroadcastSessionVM> broadcastSessions = this.broadcastSessions.ToList();

            BroadcastInputManagerWindow window = new BroadcastInputManagerWindow(broadcastSessions, allPanels);
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
                IShellTab broadcastPanel = broadcastSession.BroadcasePanel;

                if (broadcastPanel.Status != SessionStatusEnum.Connected)
                {
                    continue;
                }

                byte[] bytes = e.Argument as byte[];

                broadcastPanel.Send(bytes);
            }
        }

        private void OnShellSessionOpened(HostEvent evType, HostEventArgs evArgs)
        {
            SessionOpenedEventArgs soevArgs = evArgs as SessionOpenedEventArgs;

            switch (soevArgs.Type)
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

using ModengTerm.Addon;
using ModengTerm.Addon.Controls;
using ModengTerm.Addon.Interactive;
using ModengTerm.Base.Enumerations;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows.Media.Effects;
using WPFToolkit.MVVM;

namespace ModengTerm.OfficialAddons.Broadcast
{
    /// <summary>
    /// BroadcastInputPanel.xaml 的交互逻辑
    /// </summary>
    public partial class BroadcastPanel : TabedSidePanel
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("BroadcastPanel");

        #region 实例变量

        private BindableCollection<ShellTabVM> broadcastTabs;

        #endregion

        #region 构造方法

        public BroadcastPanel()
        {
            InitializeComponent();
        }

        #endregion

        #region 实例方法

        private ShellTabVM CreateShellTabVM(IClientShellTab shellTab) 
        {
            ShellTabVM stvm = new ShellTabVM()
            {
                ID = shellTab.ID,
                Name = shellTab.Name,
                Tab = shellTab
            };

            return stvm;
        }

        #endregion

        #region SidePanel

        public override void Initialize()
        {
            this.broadcastTabs = new BindableCollection<ShellTabVM>();
            List<IClientShellTab> shellTabs = this.client.GetAllTabs<IClientShellTab>();
            foreach (IClientShellTab shellTab in shellTabs)
            {
                if (shellTab == this.Tab)
                {
                    continue;
                }

                this.broadcastTabs.Add(this.CreateShellTabVM(shellTab));
            }

            DataGridBroadcastList.ItemsSource = this.broadcastTabs;

            this.eventRegistry.SubscribeEvent(ClientEvent.CLIENT_TAB_OPENED, "ssh|local|serial|tcp", this.OnClientTabOpened);
            this.eventRegistry.SubscribeEvent(ClientEvent.CLIENT_TAB_CLOSED, "ssh|local|serial|tcp", this.OnClientTabClosed);
            this.eventRegistry.SubscribeTabEvent(TabEvent.SHELL_USER_INPUT, this.OnTabShellUserInput, this.Tab);
        }

        public override void Release()
        {
            this.eventRegistry.UnsubscribeEvent(ClientEvent.CLIENT_TAB_OPENED, this.OnClientTabOpened);
            this.eventRegistry.UnsubscribeEvent(ClientEvent.CLIENT_TAB_CLOSED, this.OnClientTabClosed);
            this.eventRegistry.UnsubscribeTabEvent(TabEvent.SHELL_USER_INPUT, this.OnTabShellUserInput, this.Tab);
        }

        public override void Load()
        {
            logger.InfoFormat("OnLoaded");
        }

        public override void Unload()
        {
            logger.InfoFormat("OnUnload");
        }

        #endregion

        #region 事件处理器

        private void OnClientTabOpened(ClientEventArgs e, object userData) 
        {
            ClientEventTabOpened tabOpened = e as ClientEventTabOpened;

            if (tabOpened.OpenedTab == this.Tab) 
            {
                return;
            }

            this.broadcastTabs.Add(this.CreateShellTabVM(tabOpened.OpenedTab as IClientShellTab));
        }

        private void OnClientTabClosed(ClientEventArgs e, object userData) 
        {
            ClientEventTabClosed tabClosed = e as ClientEventTabClosed;

            if (tabClosed.ClosedTab == this.Tab)
            {
                return;
            }

            ShellTabVM stvm = this.broadcastTabs.FirstOrDefault(v => v.ID == tabClosed.ClosedTab.ID);
            if (stvm == null) 
            {
                return;
            }

            this.broadcastTabs.Remove(stvm);
        }

        private void OnTabShellUserInput(TabEventArgs e, object userData)
        {
            TabEventShellUserInput shellSendUserInput = e as TabEventShellUserInput;

            IEnumerable<ShellTabVM> broadcastTabs = this.broadcastTabs;

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

                shellTab.Send(shellSendUserInput.Buffer);
            }
        }

        #endregion
    }
}
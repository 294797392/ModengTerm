using ModengTerm.Addon;
using ModengTerm.Addon.Interactive;
using ModengTerm.Addons;
using ModengTerm.Base;
using ModengTerm.Controls;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using WPFToolkit.MVVM;

namespace ModengTerm.OfficialAddons.BroadcastInput
{
    /// <summary>
    /// SendAllConfigurationWindow.xaml 的交互逻辑
    /// </summary>
    public partial class BroadcastInputManagerWindow : MdWindow
    {
        #region 实例变量

        private BindableCollection<IClientShellTab> broadcastTabs;

        /// <summary>
        /// 被删除的广播会话
        /// </summary>
        private List<IClientShellTab> removeList;

        /// <summary>
        /// 新增加的广播会话
        /// </summary>
        private List<IClientShellTab> addList;

        private ClientFactory factory;
        private StorageService storageService;
        private IClient client;
        private IClientShellTab activeTab;
        private AddonModule addonModule;

        #endregion

        #region 属性

        public List<IClientShellTab> BroadcastList { get; private set; }

        #endregion

        #region 构造方法

        /// <summary>
        /// 
        /// </summary>
        /// <param name="broadcastSessions"></param>
        /// <param name="shellSessions">当前打开的所有终端类型的会话</param>
        public BroadcastInputManagerWindow(IClientShellTab activeTab, AddonModule addon)
        {
            InitializeComponent();

            this.activeTab = activeTab;
            this.addonModule = addon;

            this.InitializeWindow();
        }

        #endregion

        #region 实例方法

        private void InitializeWindow()
        {
            this.addList = new List<IClientShellTab>();
            this.removeList = new List<IClientShellTab>();

            this.factory = ClientFactory.GetFactory();
            this.storageService = this.factory.GetStorageService();
            this.client = this.factory.GetClient();

            this.broadcastTabs = new BindableCollection<IClientShellTab>();
            List<IClientShellTab> broadcastList = this.activeTab.GetData<List<IClientShellTab>>(this.addonModule, BroadcastInputAddon.KEY_BROADCAST_LIST);
            if (broadcastList != null)
            {
                this.broadcastTabs.AddRange(broadcastList);
            }

            List<IClientShellTab> shellTabs = this.client.GetAllTabs<IClientShellTab>();
            shellTabs.Remove(this.activeTab);
            ListBoxShellSessionList.DataContext = shellTabs;
            ListBoxBroadcastSessions.DataContext = this.broadcastTabs;
        }

        #endregion

        #region 事件处理器

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            this.BroadcastList = new List<IClientShellTab>();
            this.BroadcastList.AddRange(this.broadcastTabs);

            base.DialogResult = true;
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            base.DialogResult = false;
        }

        private void ButtonAddSlave_Click(object sender, RoutedEventArgs e)
        {
            IClientShellTab shellTab = ListBoxShellSessionList.SelectedItem as IClientShellTab;
            if (shellTab == null)
            {
                return;
            }

            if (this.broadcastTabs.Contains(shellTab))
            {
                return;
            }

            if (this.removeList.Contains(shellTab))
            {
                this.removeList.Remove(shellTab);
            }

            this.addList.Add(shellTab);
            this.broadcastTabs.Add(shellTab);
        }

        private void ButtonRemoveSlave_Click(object sender, RoutedEventArgs e)
        {
            IClientShellTab shellTab = ListBoxShellSessionList.SelectedItem as IClientShellTab;
            if (shellTab == null)
            {
                return;
            }

            if (!this.broadcastTabs.Contains(shellTab))
            {
                return;
            }

            if (this.addList.Contains(shellTab))
            {
                this.addList.Remove(shellTab);
            }

            this.removeList.Add(shellTab);
            this.broadcastTabs.Remove(shellTab);
        }

        private void MenuItemClearSelected_Click(object sender, RoutedEventArgs e)
        {
            this.removeList.Clear();
            this.addList.Clear();

            this.removeList.AddRange(this.broadcastTabs);
            this.broadcastTabs.Clear();
        }

        #endregion
    }
}

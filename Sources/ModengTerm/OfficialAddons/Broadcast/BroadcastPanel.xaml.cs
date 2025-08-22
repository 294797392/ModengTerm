using ModengTerm.Addon;
using ModengTerm.Addon.Interactive;
using ModengTerm.Addon.Panel;
using ModengTerm.Addon.Service;
using System.Collections.Generic;
using System.Linq;

namespace ModengTerm.OfficialAddons.Broadcast
{
    /// <summary>
    /// BroadcastInputPanel.xaml 的交互逻辑
    /// </summary>
    public partial class BroadcastPanel : SidePanelContent
    {
        #region 实例变量

        private ClientFactory factory;
        private IClientEventRegistry eventRegistory;
        private IClient client;

        #endregion

        #region 构造方法

        public BroadcastPanel()
        {
            InitializeComponent();
        }

        #endregion

        #region 实例方法

        #endregion

        #region SidePanelContent

        public override void OnInitialize()
        {
            this.factory = ClientFactory.GetFactory();
            this.eventRegistory = this.factory.GetEventRegistry();
            this.client = this.factory.GetClient();

            this.eventRegistory.SubscribeEvent("onClientTabChanged:ssh|local|serial|tcp", this.OnTabChangedEvent);
        }

        public override void OnRelease()
        {
            this.eventRegistory.UnsubscribeEvent("onClientTabChanged", this.OnTabChangedEvent);
        }

        public override void OnLoaded()
        {
        }

        public override void OnUnload()
        {
        }

        #endregion

        #region 事件处理器

        private void OnTabChangedEvent(ClientEventArgs e, object userData)
        {
            //ClientEventTabChanged tabChanged = e as ClientEventTabChanged;
            //List<IClientShellTab> shellTabs = this.client.GetAllTabs<IClientShellTab>();
            //shellTabs.Remove(tabChanged.NewTab as IClientShellTab);

            //List<ShellTabVM> broadcastTabs = tabChanged.NewTab.GetData<List<ShellTabVM>>(this.OwnerAddon, BroadcastAddon.KEY_BROADCAST_LIST);

            //if (broadcastTabs == null)
            //{
            //    broadcastTabs = new List<ShellTabVM>();

            //    foreach (IClientShellTab shellTab in shellTabs)
            //    {
            //        ShellTabVM vm = new ShellTabVM()
            //        {
            //            ID = shellTab.ID,
            //            Name = shellTab.Name,
            //            Tab = shellTab
            //        };
            //        broadcastTabs.Add(vm);
            //    }

            //    tabChanged.NewTab.SetData(this.OwnerAddon, BroadcastAddon.KEY_BROADCAST_LIST, broadcastTabs);
            //}
            //else
            //{
            //    // 删除被关闭的标签
            //    for (int i = 0; i < broadcastTabs.Count; i++)
            //    {
            //        if (!shellTabs.Contains(broadcastTabs[i].Tab)) 
            //        {
            //            broadcastTabs.RemoveAt(i);
            //            i--;
            //        }
            //    }

            //    // 创建新打开的标签
            //    foreach (IClientShellTab shellTab in shellTabs)
            //    {
            //        if (broadcastTabs.FirstOrDefault(v => v.Tab == shellTab) == null)
            //        {
            //            broadcastTabs.Add(new ShellTabVM()
            //            {
            //                ID = shellTab.ID,
            //                Name = shellTab.Name,
            //                Tab = shellTab
            //            });
            //        }
            //    }
            //}

            //DataGridBroadcastList.ItemsSource = broadcastTabs;
        }

        #endregion
    }
}
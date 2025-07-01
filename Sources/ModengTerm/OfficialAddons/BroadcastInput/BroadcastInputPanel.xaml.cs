using ModengTerm.Addon.Interactive;
using ModengTerm.Addon;
using ModengTerm.Addon.Panel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPFToolkit.MVVM;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace ModengTerm.OfficialAddons.BroadcastInput
{
    /// <summary>
    /// BroadcastInputPanel.xaml 的交互逻辑
    /// </summary>
    public partial class BroadcastInputPanel : SidePanelContent
    {
        #region 实例变量

        private ClientFactory factory;
        private IClientEventRegistry eventRegistory;
        private IClient client;

        #endregion

        #region 构造方法

        public BroadcastInputPanel()
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

            this.eventRegistory.SubscribeTabEvent(TabEvent.TAB_SHELL_CHANGED, this.OnTabEventShellActived);
        }

        public override void OnRelease()
        {
            this.eventRegistory.UnsubscribeTabEvent(TabEvent.TAB_SHELL_CHANGED, this.OnTabEventShellActived);
        }

        public override void OnLoaded()
        {
        }

        public override void OnUnload()
        {
        }

        #endregion

        #region 事件处理器

        private void OnTabEventShellActived(TabEventArgs e)
        {
            TabEventTabShellChanged shellActived = e as TabEventTabShellChanged;
            List<IClientShellTab> shellTabs = this.client.GetAllTabs<IClientShellTab>();
            shellTabs.Remove(shellActived.ActiveTab);

            List<ShellTabVM> broadcastTabs = shellActived.ActiveTab.GetData<List<ShellTabVM>>(this.OwnerAddon, BroadcastInputAddon.KEY_BROADCAST_LIST);

            if (broadcastTabs == null)
            {
                broadcastTabs = new List<ShellTabVM>();

                foreach (IClientShellTab shellTab in shellTabs)
                {
                    ShellTabVM vm = new ShellTabVM()
                    {
                        ID = shellTab.ID,
                        Name = shellTab.Name,
                        Tab = shellTab
                    };
                    broadcastTabs.Add(vm);
                }

                shellActived.ActiveTab.SetData(this.OwnerAddon, BroadcastInputAddon.KEY_BROADCAST_LIST, broadcastTabs);
            }
            else
            {
                // 删除被关闭的标签
                for (int i = 0; i < broadcastTabs.Count; i++)
                {
                    if (!shellTabs.Contains(broadcastTabs[i].Tab)) 
                    {
                        broadcastTabs.RemoveAt(i);
                        i--;
                    }
                }

                // 创建新打开的标签
                foreach (IClientShellTab shellTab in shellTabs)
                {
                    if (broadcastTabs.FirstOrDefault(v => v.Tab == shellTab) == null)
                    {
                        broadcastTabs.Add(new ShellTabVM()
                        {
                            ID = shellTab.ID,
                            Name = shellTab.Name,
                            Tab = shellTab
                        });
                    }
                }
            }

            DataGridBroadcastList.ItemsSource = broadcastTabs;
        }

        #endregion
    }
}
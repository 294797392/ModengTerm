using ModengTerm.Addon;
using ModengTerm.Addon.Controls;
using ModengTerm.Addon.Interactive;
using ModengTerm.Base;
using ModengTerm.OfficialAddons.Broadcast;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WPFToolkit.MVVM;

namespace ModengTerm.OfficialAddons.Snippet
{
    /// <summary>
    /// SnippetPanel.xaml 的交互逻辑
    /// </summary>
    public partial class SnippetPanel : TabedSidePanel
    {
        #region 实例变量

        private BindableCollection<SnippetVM> snippetVMs;

        #endregion

        #region 构造方法

        public SnippetPanel()
        {
            InitializeComponent();

            this.InitializePanel();
        }

        #endregion

        #region 实例方法

        private void InitializePanel()
        {
            ComboBoxSnippetTypes.ItemsSource = VTBaseUtils.GetEnumValues<SnippetTypes>();
            ComboBoxSnippetTypes.SelectedIndex = 0;
        }

        private void SendSnippet(SnippetVM snippetVM)
        {
            if (string.IsNullOrEmpty(snippetVM.Script))
            {
                return;
            }

            IClientShellTab shellTab = this.Tab as IClientShellTab;

            switch (snippetVM.Type)
            {
                case SnippetTypes.Text:
                    {
                        shellTab.Send(snippetVM.Script);
                        break;
                    }

                case SnippetTypes.Hex:
                    {
                        byte[] bytes;
                        if (!VTBaseUtils.TryParseHexString(snippetVM.Script, out bytes))
                        {
                            MTMessageBox.Info("发送失败, 十六进制数据格式错误");
                            return;
                        }

                        shellTab.Send(bytes);

                        break;
                    }

                default:
                    throw new System.NotImplementedException();
            }

        }

        #endregion

        #region 事件处理器

        private void ListBoxSnippets_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // SelectionChanged事件是冒泡事件，会从子元素向父元素传递
            // 设置e.Handled = true阻止SelectionChanged事件继续向PanelContainerUserControl传递
            e.Handled = true;

            SnippetVM snippetVM = ListBoxSnippets.SelectedItem as SnippetVM;
            if (snippetVM == null)
            {
                return;
            }

            TextBoxName.Text = snippetVM.Name;
            TextBoxCommand.Text = snippetVM.Script;
            ComboBoxSnippetTypes.SelectedItem = snippetVM.Type;

            this.SendSnippet(snippetVM);
            ListBoxSnippets.SelectedItem = null; // 下次继续触发
        }

        private void ComboBoxSnippetTypes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void ButtonCreate_Click(object sender, RoutedEventArgs e)
        {
            string name = TextBoxName.Text;
            if (string.IsNullOrEmpty(name))
            {
                MTMessageBox.Info("请输入正确的命令名称");
                return;
            }

            if (ComboBoxSnippetTypes.SelectedItem == null)
            {
                MTMessageBox.Info("请选择命令类型");
                return;
            }

            SnippetTypes snippetType = (SnippetTypes)ComboBoxSnippetTypes.SelectedItem;
            string script = TextBoxCommand.Text;

            Snippet snippet = new Snippet()
            {
                Id = AddonUtils.GetObjectId(),
                Name = name,
                Script = script,
                Type = snippetType,
                TabId = this.Tab.ID.ToString()
            };

            int code = this.storage.AddObject<Snippet>(snippet);
            if (code != ResponseCode.SUCCESS)
            {
                MTMessageBox.Error("新建失败, 错误码 = {0}", code);
                return;
            }

            SnippetVM snippetVM = new SnippetVM(snippet);
            this.snippetVMs.Add(snippetVM);
        }

        private void ButtonSend_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TextBoxCommand.Text))
            {
                MTMessageBox.Info("请输入要发送的命令");
                return;
            }

            SnippetVM snippetVM = new SnippetVM()
            {
                Type = (SnippetTypes)ComboBoxSnippetTypes.SelectedItem,
                Script = TextBoxCommand.Text,
            };

            this.SendSnippet(snippetVM);
        }

        private void ButtonUpdate_Click(object sender, RoutedEventArgs e)
        {
            SnippetVM snippetVM = ListBoxSnippets.SelectedItem as SnippetVM;
            if (snippetVM == null) 
            {
                MTMessageBox.Info("请先选中要更新的命令");
                return;
            }

            if (string.IsNullOrEmpty(TextBoxName.Text))
            {
                MTMessageBox.Info("请输入正确的命令名称");
                return;
            }

            snippetVM.Script = TextBoxCommand.Text;
            snippetVM.Type = (SnippetTypes)ComboBoxSnippetTypes.SelectedItem;

            Snippet snippet = snippetVM.GetSnippet();
            int code = this.storage.UpdateObject<Snippet>(snippet);
            if (code != ResponseCode.SUCCESS) 
            {
                MTMessageBox.Info("更新失败, 错误码 = {0}", code);
                return;
            }
        }

        private void ButtonDeleteSnippet_Click(object sender, RoutedEventArgs e)
        {
            FrameworkElement frameworkElement = sender as FrameworkElement;
            SnippetVM snippetVM = frameworkElement.Tag as SnippetVM;

            if (!MTMessageBox.Confirm("确定要删除{0}吗?", snippetVM.Name))
            {
                return;
            }

            int code = this.storage.DeleteObject<Snippet>(snippetVM.ID.ToString());
            if (code != ResponseCode.SUCCESS) 
            {
                MTMessageBox.Info("删除失败, 错误码 = {0}", code);
                return;
            }

            this.snippetVMs.Remove(snippetVM);
        }

        #endregion

        #region TabedSidePanel

        public override void Initialize()
        {
            this.snippetVMs = new BindableCollection<SnippetVM>();
            List<Snippet> snippets = this.storage.GetObjects<Snippet>(this.Tab.ID.ToString());
            this.snippetVMs.AddRange(snippets.Select(v => new SnippetVM(v)));
            ListBoxSnippets.ItemsSource = this.snippetVMs;
        }

        public override void Release()
        {
            this.snippetVMs.Clear();
        }

        public override void Load()
        {
        }

        public override void Unload()
        {
        }

        #endregion
    }
}

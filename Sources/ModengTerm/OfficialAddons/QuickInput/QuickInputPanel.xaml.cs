using ModengTerm.Addon.Panel;
using ModengTerm.Base;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using WPFToolkit.MVVM;

namespace ModengTerm.OfficialAddons.QuickInput
{
    /// <summary>
    /// QuickCommandUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class QuickInputPanel : SidePanelContent
    {
        #region 实例变量

        #endregion

        #region 构造方法

        public QuickInputPanel()
        {
            InitializeComponent();

            this.InitializeUserControl();
        }

        #endregion

        #region 实例方法

        private void InitializeUserControl()
        {
            if (DesignerProperties.GetIsInDesignMode(this))
            {
                return;
            }
        }

        #endregion

        #region 事件处理器

        private void ButtonCreate_Click(object sender, RoutedEventArgs e)
        {
            throw new RefactorImplementedException();

            //ShellSessionVM shellSessionVM = this.quickInputVM.OpenedSessionVM as ShellSessionVM;

            //CommandManagerWindow window = new CommandManagerWindow(shellSessionVM);
            //window.Owner = Application.Current.MainWindow;
            //if ((bool)window.ShowDialog())
            //{
            //    // 刷新命令列表
            //    this.quickInputVM.ReloadCommandList();
            //}
        }

        private void ListBoxCommands_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            throw new RefactorImplementedException();

            //CommandVM selectedCommand = ListBoxCommands.SelectedItem as CommandVM;
            //if (selectedCommand == null)
            //{
            //    return;
            //}

            //MCommands.SendCommand.Execute(selectedCommand, this);

            //ListBoxCommands.SelectedItem = null; // 下次继续触发
        }

        #endregion

        #region SidePanelContent

        public override void OnInitialize()
        {
        }

        public override void OnRelease()
        {
        }

        public override void OnLoaded()
        {
        }

        public override void OnUnload()
        {
        }

        #endregion
    }
}

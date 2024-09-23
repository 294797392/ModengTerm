using ModengTerm.ServiceAgents;
using ModengTerm.ViewModels.Terminals;
using ModengTerm.Windows.Terminals;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WPFToolkit.MVVM;

namespace ModengTerm.UserControls.Terminals
{
    /// <summary>
    /// QuickCommandUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class ShellCommandUserControl : UserControl
    {
        private BindableCollection<ShellCommandVM> shellCommands;
        private ServiceAgent serviceAgent;

        public ShellCommandUserControl()
        {
            InitializeComponent();

            this.InitializeUserControl();
        }

        private void InitializeUserControl() 
        {
            if (DesignerProperties.GetIsInDesignMode(this))
            {
                return;
            }

            this.serviceAgent = MTermApp.Context.ServiceAgent;

            this.shellCommands = new BindableCollection<ShellCommandVM>();
            this.shellCommands.AddRange(this.serviceAgent.GetShellCommands().Select(v => new ShellCommandVM(v)));
            ListBoxCommands.DataContext = this.shellCommands;
        }

        private void ButtonCreate_Click(object sender, RoutedEventArgs e)
        {
            CreateShellCommandWindow window = new CreateShellCommandWindow();
            window.Owner = MainWindow.GetWindow(this);
            if ((bool)window.ShowDialog())
            {
                this.shellCommands.Clear();
                this.shellCommands.AddRange(this.serviceAgent.GetShellCommands().Select(v => new ShellCommandVM(v)));
            }
        }

        private void ListBoxCommands_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ShellCommandVM selectedCommand = ListBoxCommands.SelectedItem as ShellCommandVM;
            if (selectedCommand == null)
            {
                return;
            }

            MCommands.SendCommand.Execute(selectedCommand, this);

            ListBoxCommands.SelectedItem = null; // 下次继续触发
        }
    }
}

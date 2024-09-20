using ModengTerm.Base.DataModels;
using ModengTerm.ViewModels.Terminals;
using ModengTerm.Windows.Terminals;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using XTerminal;

namespace ModengTerm.UserControls.Terminals
{
    /// <summary>
    /// QuickCommandUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class ShellCommandUserControl : UserControl
    {
        private ShellGlobalVM shellGlobalVM;

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

            this.shellGlobalVM = MTermApp.Context.ShellGlobalVM;
            ListBoxCommands.DataContext = this.shellGlobalVM.Commands;
        }

        private void ButtonCreate_Click(object sender, RoutedEventArgs e)
        {
            CreateShellCommandWindow window = new CreateShellCommandWindow();
            window.Owner = MainWindow.GetWindow(this);
            if ((bool)window.ShowDialog())
            {
                ShellCommand shellCommand = window.Command;

                this.shellGlobalVM.Commands.Add(new ShellCommandVM(shellCommand));
            }
        }

        private void ListBoxCommands_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ShellCommandVM selectedCommand = ListBoxCommands.SelectedItem as ShellCommandVM;
            if (selectedCommand == null)
            {
                return;
            }

            selectedCommand.AutoCRLF = CheckBoxAutoCRLF.IsChecked.Value;

            MCommands.SendCommand.Execute(selectedCommand, this);

            ListBoxCommands.SelectedItem = null; // 下次继续触发
        }
    }
}

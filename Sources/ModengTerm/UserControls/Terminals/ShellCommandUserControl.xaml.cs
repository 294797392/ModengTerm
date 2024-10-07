using ModengTerm.Base.DataModels;
using ModengTerm.Base.ServiceAgents;
using ModengTerm.Terminal.ViewModels;
using ModengTerm.ViewModels.Terminals;
using ModengTerm.Windows.Terminals;
using System.Collections.Generic;
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
        private ServiceAgent serviceAgent;
        private ShellSessionVM shellSession;

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
        }

        private void ButtonCreate_Click(object sender, RoutedEventArgs e)
        {
            this.shellSession.OpenCreateShellCommandWindow();
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

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            this.shellSession = e.NewValue as ShellSessionVM;
        }
    }
}

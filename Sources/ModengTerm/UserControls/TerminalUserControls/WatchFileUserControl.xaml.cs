using ModengTerm.Addons.SystemMonitor;
using ModengTerm.Terminal;
using ModengTerm.Terminal.Session;
using ModengTerm.UserControls.TerminalUserControls.Rendering;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;

namespace ModengTerm.UserControls.TerminalUserControls
{
    /// <summary>
    /// WatchFileUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class WatchFileUserControl : UserControl
    {
        private WatchFileVM ViewModel { get { return this.DataContext as WatchFileVM; } }

        public WatchFileUserControl()
        {
            InitializeComponent();
        }

        private void ButtonAddFile_Click(object sender, RoutedEventArgs e)
        {
        }

        private void ButtonDeleteFile_Click(object sender, RoutedEventArgs e)
        {
        }

        private void ButtonPauseFile_Click(object sender, RoutedEventArgs e)
        {
        }

        private void ButtonClear_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}

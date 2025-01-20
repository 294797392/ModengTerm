using ModengTerm.ViewModels.Terminals.PanelContent;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static ModengTerm.ViewModels.Terminals.PanelContent.WatchFileVM;

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

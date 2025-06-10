using ModengTerm.ViewModel.Session;
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

namespace ModengTerm.UserControls
{
    /// <summary>
    /// SessionTreeViewUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class SessionTreeViewUserControl : UserControl
    {
        public event Action<SessionTreeViewUserControl, object> TreeViewMouseDoubleClick;

        public SessionTreeVM ViewModel
        {
            get { return TreeViewSessionList.DataContext as SessionTreeVM; }
            set
            {
                if (TreeViewSessionList.DataContext != value)
                {
                    TreeViewSessionList.DataContext = value;
                }
            }
        }

        public SessionTreeViewUserControl()
        {
            InitializeComponent();

            this.InitializeUserControl();
        }

        private void InitializeUserControl()
        {

        }

        private void TreeViewSessionList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            this.TreeViewMouseDoubleClick?.Invoke(this, TreeViewSessionList.SelectedItem);
        }
    }
}

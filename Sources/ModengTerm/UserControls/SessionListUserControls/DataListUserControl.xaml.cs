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

namespace ModengTerm.UserControls.SessionListUserControls
{
    /// <summary>
    /// DataListUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class DataListUserControl : SessionListView
    {
        public DataListUserControl()
        {
            InitializeComponent();
        }

        public override void OnLoad()
        {
        }

        public override void OnUnload()
        {
        }
    }
}

using ModengTerm.Controls;
using ModengTerm.ViewModel.Terminal;
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
using System.Windows.Shapes;

namespace ModengTerm.OfficialAddons.Record
{
    /// <summary>
    /// RecordOptionsWindow.xaml 的交互逻辑
    /// </summary>
    public partial class RecordOptionsWindow : MdWindow
    {
        public RecordOptionsWindow()
        {
            InitializeComponent();
        }

        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            RecordOptionsVM recordOptionsVM = base.DataContext as RecordOptionsVM;

            base.DialogResult = true;
        }
    }
}

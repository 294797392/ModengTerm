using Microsoft.Win32;
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

namespace ModengTerm.UserControls.OptionsUserControl
{
    /// <summary>
    /// CommandLineOptionsUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class CommandLineOptionsUserControl : UserControl
    {
        public CommandLineOptionsUserControl()
        {
            InitializeComponent();
        }

        #region 事件处理器

        private void ButtonBrowserCommandLinePath_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = false;
            if ((bool)(dialog.ShowDialog()))
            {
                TextBoxCommandLinePath.Text = dialog.FileName;
            }
        }

        #endregion
    }
}

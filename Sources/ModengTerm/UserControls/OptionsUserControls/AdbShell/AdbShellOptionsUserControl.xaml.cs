using Microsoft.Win32;
using ModengTerm.Windows.SSH;
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

namespace ModengTerm.UserControls.OptionsUserControls.AdbShell
{
    /// <summary>
    /// AdbShellOptionsUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class AdbShellOptionsUserControl : UserControl
    {
        public AdbShellOptionsUserControl()
        {
            InitializeComponent();
        }

        private void ButtonBrowseAdbPath_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if ((bool)openFileDialog.ShowDialog())
            {
                TextBoxAdbPath.Text = openFileDialog.FileName;
            }
        }
    }
}

using DotNEToolkit;
using Microsoft.Win32;
using ModengTerm.Addon.Interactive;
using ModengTerm.Base;
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
        private IClientTab tab;

        public RecordOptionsVM Options { get; private set; }

        public RecordOptionsWindow(IClientTab tab)
        {
            InitializeComponent();

            this.InitializeWindow(tab);
        }

        private void InitializeWindow(IClientTab tab) 
        {
            this.tab = tab;
            this.Options = new RecordOptionsVM();
            string fileName = string.Format("{0}_{1}.mrec", tab.Name, DateTime.Now.ToString(DateTimeFormat.yyyyMMddhhmmss));
            string fullPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
            this.Options.FilePath = fullPath;

            base.DataContext = this.Options;
        }

        private void ButtonStart_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TextBoxLogPath.Text))
            {
                MTMessageBox.Info("请选择录像保存路径");
                return;
            }

            base.DialogResult = true;
        }

        private void ButtonBrowse_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "录像文件(*.mrec)|*.mrec";
            dialog.FileName = this.Options.FilePath;
            if ((bool)dialog.ShowDialog())
            {
                this.Options.FilePath = dialog.FileName;
            }
        }
    }
}

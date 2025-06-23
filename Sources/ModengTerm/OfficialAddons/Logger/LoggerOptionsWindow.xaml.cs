using DotNEToolkit;
using Microsoft.Win32;
using ModengTerm.Addon.Interactive;
using ModengTerm.Base;
using ModengTerm.Controls;
using ModengTerm.Terminal;
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

namespace ModengTerm.OfficialAddons.Logger
{
    /// <summary>
    /// LoggerOptionsWindow.xaml 的交互逻辑
    /// </summary>
    public partial class LoggerOptionsWindow : MdWindow
    {
        private IClientTab activeTab;

        public LoggerOptionsVM Options { get; private set; }

        public LoggerOptionsWindow(IClientTab activeTab)
        {
            InitializeComponent();

            this.InitializeWindow(activeTab);
        }

        private void InitializeWindow(IClientTab activeTab)
        {
            this.activeTab = activeTab;
            this.Options = new LoggerOptionsVM();
            //string fileName = string.Format("{0}_{1}.log", activeTab.Name, DateTime.Now.ToString(DateTimeFormat.yyyyMMddhhmmss));
            //string fullPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
            //this.Options.FilePath = fullPath;
        }

        private void ButtonStart_Click(object sender, RoutedEventArgs e)
        {
            base.DialogResult = true;
        }

        private void ButtonBrowse_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            //dialog.Filter = "文本文件(*.txt)|*.txt|html文件(*.html)|*.html";
            dialog.Filter = "文本文件(*.txt)|*.txt";
            dialog.FileName = string.Format("{0}_{1}", this.activeTab.Name, DateTime.Now.ToString(DateTimeFormat.yyyyMMddhhmmss));
            if ((bool)dialog.ShowDialog())
            {
                this.Options.FilePath = dialog.FileName;
                TextBoxLogPath.Text = dialog.FileName;
            }
        }
    }
}

using DotNEToolkit;
using Microsoft.Win32;
using ModengTerm.Base;
using ModengTerm.Controls;
using ModengTerm.Terminal;
using ModengTerm.Terminal.Loggering;
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

namespace ModengTerm.Windows
{
    /// <summary>
    /// LoggerOptionsWindow.xaml 的交互逻辑
    /// </summary>
    public partial class LoggerOptionsWindow : MdWindow
    {
        private LoggerOptionsVM viewModel;
        private ShellSessionVM shellSession;

        public LoggerOptions Options { get; private set; }

        public LoggerOptionsWindow(ShellSessionVM shellSession)
        {
            InitializeComponent();

            this.InitializeWindow(shellSession);
        }

        private void InitializeWindow(ShellSessionVM shellSession)
        {
            this.shellSession = shellSession;
            this.viewModel = new LoggerOptionsVM(this.shellSession.VideoTerminal);
            base.DataContext = this.viewModel;

            string fileName = string.Format("{0}_{1}.log", this.shellSession.Name, DateTime.Now.ToString(DateTimeFormat.yyyyMMddhhmmss));
            string fullPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
            this.viewModel.FilePath = fullPath;
        }

        private void ButtonStart_Click(object sender, RoutedEventArgs e)
        {
            this.Options = this.viewModel.GetOptions();
            if (this.Options == null)
            {
                return;
            }

            base.DialogResult = true;
        }

        private void ButtonBrowse_Click(object sender, RoutedEventArgs e)
        {
            this.viewModel.BrowseFilePath();
        }
    }
}

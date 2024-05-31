using DotNEToolkit;
using Microsoft.Win32;
using ModengTerm.Base;
using ModengTerm.Controls;
using ModengTerm.Terminal;
using ModengTerm.Terminal.Loggering;
using ModengTerm.Terminal.ViewModels;
using ModengTerm.ViewModels;
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
    public partial class LoggerOptionsWindow : MTermWindow
    {
        private LoggerOptionsVM viewModel;
        private IVideoTerminal vt;

        public LoggerOptions Options { get; private set; }

        public LoggerOptionsWindow(IVideoTerminal vt)
        {
            InitializeComponent();

            this.InitializeWindow(vt);
        }

        private void InitializeWindow(IVideoTerminal vt)
        {
            this.vt = vt;
            this.viewModel = new LoggerOptionsVM(vt);
            base.DataContext = this.viewModel;
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

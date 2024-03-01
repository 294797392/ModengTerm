using ModengTerm.Controls;
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
    /// FindWindow.xaml 的交互逻辑
    /// </summary>
    public partial class FindWindow : MTermWindow
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("FindWindow");

        private FindVM viewModel;
        private bool finding;

        public FindWindow(FindVM vm)
        {
            InitializeComponent();

            this.InitializeWindow(vm);
        }

        private void InitializeWindow(FindVM vm)
        {
            this.viewModel = vm;
            base.DataContext = vm;
        }

        private void FindAsync(bool findOnce)
        {
            if (this.finding)
            {
                return;
            }

            this.viewModel.FindOnce = findOnce;

            Task.Factory.StartNew(() =>
            {
                this.finding = true;

                try
                {
                    this.viewModel.Find();
                }
                catch (Exception ex)
                {
                    logger.Error("查找异常", ex);
                }
                finally
                {
                    this.finding = false;
                }
            });
        }

        private void ButtonFind_Click(object sender, RoutedEventArgs e)
        {
            this.FindAsync(true);
        }

        private void ButtonFindAll_Click(object sender, RoutedEventArgs e)
        {
            this.FindAsync(false);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            this.viewModel.Release();
        }
    }
}

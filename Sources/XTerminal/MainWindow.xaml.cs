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
using XTerminal.Base.DataModels;

namespace XTerminal
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.InitializeWindow();
        }

        private void InitializeWindow()
        {
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SessionListWindow sessionListWindow = new SessionListWindow();
            sessionListWindow.Owner = this;
            if ((bool)sessionListWindow.ShowDialog())
            {
                XTermSession session = sessionListWindow.SelectedSession;

                ContentLayoutUserControl.Get
            }
        }
    }
}

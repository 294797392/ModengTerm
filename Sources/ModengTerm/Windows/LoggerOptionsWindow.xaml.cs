using ModengTerm.Terminal.Loggering;
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
    public partial class LoggerOptionsWindow : Window
    {
        public LoggerOptions Options { get; private set; }

        public LoggerOptionsWindow()
        {
            InitializeComponent();
        }
    }
}

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

namespace XTerminal.Windows
{
    /// <summary>
    /// DebugWindow.xaml 的交互逻辑
    /// </summary>
    public partial class DebugWindow : Window
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("DebugWindow");

        public VideoTerminal VideoTerminal { get; set; }

        public DebugWindow()
        {
            InitializeComponent();
        }

        private void ButtonResizeTerminal_Click(object sender, RoutedEventArgs e)
        {
            logger.InfoFormat("Resize, row = {0}, col = {1}", this.VideoTerminal.RowSize, this.VideoTerminal.ColumnSize + 1);
            this.VideoTerminal.SessionTransport.Resize(this.VideoTerminal.RowSize, this.VideoTerminal.ColumnSize + 1);
        }
    }
}

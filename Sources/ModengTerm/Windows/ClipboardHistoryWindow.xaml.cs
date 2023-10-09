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
    /// ClipboardHistoryWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ClipboardHistoryWindow : Window
    {
        private ClipboardVM clipboardVM;

        public ClipboardHistoryWindow(ClipboardVM clipboardVM)
        {
            InitializeComponent();

            this.InitializeWindow(clipboardVM);
        }

        private void InitializeWindow(ClipboardVM clipboardVM)
        {
            this.clipboardVM = clipboardVM;

            base.DataContext = this.clipboardVM;
        }
    }
}

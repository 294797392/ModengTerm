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
    /// ParagraphsWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ParagraphsWindow : Window
    {
        private ParagraphsVM paragraphsVM;

        public ParagraphsWindow(ParagraphsVM paragraphsVM)
        {
            InitializeComponent();

            this.InitializeWindow(paragraphsVM);
        }

        private void InitializeWindow(ParagraphsVM paragraphsVM)
        {
            this.paragraphsVM = paragraphsVM;

            base.DataContext = this.paragraphsVM;
        }
    }
}

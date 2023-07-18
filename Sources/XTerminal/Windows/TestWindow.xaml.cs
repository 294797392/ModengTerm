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
using XTerminal.Session;

namespace XTerminal.Windows
{
    /// <summary>
    /// Window.xaml 的交互逻辑
    /// </summary>
    public partial class TestWindow : Window
    {
        public TestWindow()
        {
            InitializeComponent();

            DocumentPanel.Scrollbar = SliderScrolbar;

            VideoTerminal videoTermianl = new VideoTerminal();
            videoTermianl.SurfacePanel = DocumentPanel;
            videoTermianl.Initialize(VTInitialOptions.Home);
        }
    }
}

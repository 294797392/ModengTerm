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
using System.Windows.Navigation;
using System.Windows.Shapes;
using XTerminal.Document.Rendering;
using XTerminal.Session;

namespace XTerminal.UserControls
{
    /// <summary>
    /// TerminalUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class TerminalUserControl : UserControl
    {
        public IDrawingCanvasPanel CanvasPanel { get { return DrawingCanvasPanel; } }

        public TerminalUserControl()
        {
            InitializeComponent();

            this.InitializeUserControl();
        }

        private void InitializeUserControl()
        {
            DrawingCanvasPanel.Scrollbar = SliderScrolbar;

            //VideoTerminal videoTermianl = new VideoTerminal();
            //videoTermianl.CanvasPanel = DocumentPanel;
            //videoTermianl.Initialize(VTInitialOptions.Home);
        }
    }
}

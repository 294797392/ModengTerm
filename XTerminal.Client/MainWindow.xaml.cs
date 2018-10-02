using GardeniaTerminalCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace XTerminalClient
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window, VTWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            VideoTerminal terminal = VideoTerminal.Create();
            terminal.Screen = VTConsole;
            terminal.Window = this;
            terminal.Open();
            VTConsole.VT = terminal;
        }

        public void SetIconName(string name)
        {
        }

        public void SetTitle(string title)
        {
            Title = title;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            Console.WriteLine(e.Key.ToString());
        }
    }
}
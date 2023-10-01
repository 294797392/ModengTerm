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
using XTerminal.Base.DataModels;
using XTerminal.Base.Enumerations;
using XTerminal.UserControls.ViewportFiller;

namespace XTerminal.UserControls
{
    /// <summary>
    /// ViewportUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class ViewportUserControl : UserControl
    {
        private IViewportFiller currentFiller;

        public ViewportUserControl()
        {
            InitializeComponent();
        }

        public int Open(XTermSession session)
        {
            IViewportFiller viewportFiller = this.CreateFiller(session);
            GridRoot.Children.Add(viewportFiller as UIElement);
            this.currentFiller = viewportFiller;
            return viewportFiller.Open(session);
        }

        public void Close()
        {
            this.currentFiller.Close();
            GridRoot.Children.Clear();
        }
    }
}

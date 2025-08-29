using ModengTerm.Addon.Controls;
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

namespace ModengTerm.UserControls.OptionsUserControl.Terminal
{
    /// <summary>
    /// TerminalOptionsUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class TerminalOptionsUserControl : UserControl, IPreferencePanel
    {
        public TerminalOptionsUserControl()
        {
            InitializeComponent();
        }

        public Dictionary<string, object> GetOptions()
        {
            throw new NotImplementedException();
        }

        public void SetOptions(Dictionary<string, object> options)
        {
            throw new NotImplementedException();
        }
    }
}

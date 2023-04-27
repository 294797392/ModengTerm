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
using XTerminal.Session.Property;
using XTerminal.ViewModels;

namespace XTerminal.UserControls
{
    /// <summary>
    /// SSHPropertiesUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class SSHPropertiesUserControl : UserControl
    {
        public SSHPropertiesUserControl()
        {
            InitializeComponent();
        }

        private void ComboBoxAuthList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SSHAuthEnum authType = (SSHAuthEnum)ComboBoxAuthList.SelectedItem;

            switch (authType)
            {
                case SSHAuthEnum.None:
                    {
                        RowDefinitionPassword.Height = new GridLength(0);
                        RowDefinitionUserName.Height = new GridLength(0);
                        break;
                    }

                case SSHAuthEnum.Password:
                    {
                        RowDefinitionPassword.Height = new GridLength(35);
                        RowDefinitionUserName.Height = new GridLength(35);
                        break;
                    }

                case SSHAuthEnum.PulicKey:
                    {
                        RowDefinitionPassword.Height = new GridLength(0);
                        RowDefinitionUserName.Height = new GridLength(0);
                        break;
                    }

                default:
                    throw new NotImplementedException();
            }
        }
    }
}

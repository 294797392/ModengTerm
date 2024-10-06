using Microsoft.Win32;
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

namespace XTerminal.UserControls.OptionsUserControl
{
    /// <summary>
    /// SSHOptionsUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class SSHOptionsUserControl : UserControl
    {
        #region 构造方法

        public SSHOptionsUserControl()
        {
            InitializeComponent();
        }

        #endregion

        #region 事件处理器

        private void ComboBoxAuthList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboBoxAuthList.SelectedItem == null)
            {
                return;
            }

            SSHAuthTypeEnum authType = (SSHAuthTypeEnum)ComboBoxAuthList.SelectedItem;

            switch (authType)
            {
                case SSHAuthTypeEnum.None:
                    {
                        RowDefinitionUserName.Height = new GridLength(0);
                        RowDefinitionPassword.Height = new GridLength(0);
                        RowDefinitionPublicKey.Height = new GridLength(0);
                        RowDefinitionPassphrase.Height = new GridLength(0);
                        break;
                    }

                case SSHAuthTypeEnum.Password:
                    {
                        RowDefinitionUserName.Height = new GridLength(35);
                        RowDefinitionPassword.Height = new GridLength(35);
                        RowDefinitionPublicKey.Height = new GridLength(0);
                        RowDefinitionPassphrase.Height = new GridLength(0);
                        break;
                    }

                case SSHAuthTypeEnum.PrivateKey:
                    {
                        RowDefinitionUserName.Height = new GridLength(35);
                        RowDefinitionPassword.Height = new GridLength(0);
                        RowDefinitionPublicKey.Height = new GridLength(35);
                        RowDefinitionPassphrase.Height = new GridLength(35);
                        break;
                    }

                default:
                    throw new NotImplementedException();
            }
        }

        private void ButtonBrowsePrivateKeyFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if ((bool)openFileDialog.ShowDialog())
            {
                TextBoxSSHPrivateKey.Text = openFileDialog.FileName;
            }
        }

        #endregion

        #region 事件处理器

        private void PasswordBoxSSHPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordBox passwordBox = sender as PasswordBox;

            passwordBox.Tag = passwordBox.Password;
        }

        #endregion
    }
}

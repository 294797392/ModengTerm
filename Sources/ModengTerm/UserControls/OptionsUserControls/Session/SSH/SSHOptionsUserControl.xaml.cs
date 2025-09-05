using Microsoft.Win32;
using ModengTerm.Addon.Controls;
using ModengTerm.Base;
using ModengTerm.Base.DataModels;
using ModengTerm.Base.Enumerations.Ssh;
using ModengTerm.Base.ServiceAgents;
using ModengTerm.Windows.SSH;
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
using WPFToolkit.Utility;

namespace ModengTerm.UserControls.OptionsUserControl.SSH
{
    /// <summary>
    /// SSHOptionsUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class SSHOptionsUserControl : UserControl, IPreferencePanel
    {
        #region 构造方法

        public SSHOptionsUserControl()
        {
            InitializeComponent();

            this.InitializeUserControl();
        }

        private void InitializeUserControl() 
        {
            ComboBoxAuthList.ItemsSource = VTBaseUtils.GetEnumValues<SSHAuthTypeEnum>();
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
            PrivateKeyManagerWindow window = new PrivateKeyManagerWindow();
            window.Owner = Window.GetWindow(this);
            if ((bool)window.ShowDialog())
            {
                TextBoxPrivateKey.Text = window.SelectedPrivateKey.Name;
                TextBoxPrivateKey.Tag = window.SelectedPrivateKey.ID;
            }
        }

        private void PasswordBoxSSHPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordBox passwordBox = sender as PasswordBox;
            passwordBox.Tag = passwordBox.Password;
        }

        #endregion

        #region IPreferencePanel

        public Dictionary<string, object> GetOptions()
        {
            if (!VTBaseUtils.IsValidIpAddress(TextBoxServerAddress.Text))
            {
                MTMessageBox.Info("请输入主机名称");
                return null;
            }

            if (!VTBaseUtils.IsValidNetworkPort(TextBoxServerPort.Text))
            {
                MTMessageBox.Info("请输入正确的端口号");
                return null;
            }

            if (string.IsNullOrEmpty(TextBoxUserName.Text))
            {
                MTMessageBox.Info("请输入用户名");
                return null;
            }

            SSHAuthTypeEnum authType = (SSHAuthTypeEnum)ComboBoxAuthList.SelectedItem;
            switch (authType)
            {
                case SSHAuthTypeEnum.None:
                    {
                        break;
                    }

                case SSHAuthTypeEnum.Password:
                    {
                        if (string.IsNullOrEmpty(PasswordBoxPassword.Password))
                        {
                            MTMessageBox.Info("请输入密码");
                            return null;
                        }

                        break;
                    }

                case SSHAuthTypeEnum.PrivateKey:
                    {
                        if (TextBoxPrivateKey.Tag == null)
                        {
                            MTMessageBox.Info("请选择密钥文件");
                            return null;
                        }

                        // 密钥密码可以为空，如果密钥需要密码，并且用户在新建会话的时候没有输入密码，那么在打开会话的时候提示用户输入密码

                        break;
                    }

                default:
                    throw new NotImplementedException();
            }

            Dictionary<string, object> options = new Dictionary<string, object>()
            {
                { PredefinedOptions.SSH_SERVER_ADDR, TextBoxServerAddress.Text },
                { PredefinedOptions.SSH_SERVER_PORT, Convert.ToUInt16(TextBoxServerPort.Text) },
                { PredefinedOptions.SSH_USER_NAME, TextBoxUserName.Text },
                { PredefinedOptions.SSH_PASSWORD, PasswordBoxPassword.Password },
                { PredefinedOptions.SSH_PRIVATE_KEY_ID, TextBoxPrivateKey.Tag },
                { PredefinedOptions.SSH_Passphrase,  PasswordBoxPassphrase.Password },
                { PredefinedOptions.SSH_AUTH_TYPE, ComboBoxAuthList.SelectedItem },
            };

            return options;
        }

        public void SetOptions(Dictionary<string, object> options)
        {
            ServiceAgent serviceAgent = ServiceAgentFactory.Get();
            List<PrivateKey> privateKeys = serviceAgent.GetAllPrivateKey();
            PrivateKey privateKey = privateKeys.FirstOrDefault(v => v.ID == options.GetOptions<string>(PredefinedOptions.SSH_PRIVATE_KEY_ID));

            TextBoxServerAddress.Text = options.GetOptions<string>(PredefinedOptions.SSH_SERVER_ADDR);
            TextBoxServerPort.Text = options.GetOptions<string>(PredefinedOptions.SSH_SERVER_PORT);
            TextBoxUserName.Text = options.GetOptions<string>(PredefinedOptions.SSH_USER_NAME);
            PasswordBoxPassword.Password = options.GetOptions<string>(PredefinedOptions.SSH_PASSWORD);
            TextBoxPrivateKey.Text = privateKey == null ? string.Empty : privateKey.Name;
            TextBoxPrivateKey.Tag = privateKey == null ? string.Empty : privateKey.ID;
            PasswordBoxPassphrase.Password = options.GetOptions<string>(PredefinedOptions.SSH_Passphrase);
            ComboBoxAuthList.SelectedItem = options.GetOptions<SSHAuthTypeEnum>(PredefinedOptions.SSH_AUTH_TYPE);
        }

        #endregion
    }
}

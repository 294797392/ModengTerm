using DotNEToolkit;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
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
using WPFToolkit.MVVM;
using WPFToolkit.Utility;
using XTerminal.Base;
using XTerminal.Base.DataModels;
using XTerminal.Base.Definitions;
using XTerminal.Base.Enumerations;
using XTerminal.ViewModels;

namespace XTerminal.Windows
{
    /// <summary>
    /// CreateSessionWindow.xaml 的交互逻辑
    /// </summary>
    public partial class CreateSessionWindow : Window
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("CreateSessionWindow");

        #endregion

        #region 实例变量

        private BindableCollection<SessionTypeVM> sessionTypeList;

        #endregion

        #region 属性

        /// <summary>
        /// 获取当前窗口所编辑的会话
        /// </summary>
        public XTermSession Session { get; private set; }

        #endregion

        #region 构造方法

        public CreateSessionWindow()
        {
            InitializeComponent();

            this.InitializeWindow();
        }

        #endregion

        #region 实例方法

        private void InitializeWindow()
        {
            this.CollapsedAllGrid();

            TextBoxSessionName.Text = string.Format("新建会话_{0}", DateTime.Now.ToString(DateTimeFormat.yyyyMMddhhmmss));

            // 初始化SSH验证方式列表
            ComboBoxAuthList.ItemsSource = Enum.GetValues(typeof(SSHAuthTypeEnum));
            ComboBoxAuthList.SelectedItem = SSHAuthTypeEnum.Password;
            TextBoxSSHPort.Text = XTermDefaultValues.DefaultSSHPort.ToString();

            // 初始化会话类型列表
            this.sessionTypeList = new BindableCollection<SessionTypeVM>();
            List<SessionDefinition> sessions = XTermApp.Context.ServiceAgent.GetSessionDefinitions();
            foreach (SessionDefinition session in sessions)
            {
                this.sessionTypeList.Add(new SessionTypeVM(session));
            }
            ComboBoxSessionTypes.ItemsSource = this.sessionTypeList;
            ComboBoxSessionTypes.SelectedIndex = 0;

            // 初始化外观设置
            ComboBoxFontFamily.ItemsSource = XTermApp.Context.Manifest.FontFamilyList;
            ComboBoxFontFamily.SelectedIndex = 0;
            ComboBoxForeground.ItemsSource = XTermApp.Context.Manifest.ForegroundList;
            ComboBoxForeground.SelectedIndex = 0;
            ComboBoxFontSize.ItemsSource = XTermApp.Context.Manifest.FontSizeList;
            ComboBoxFontSize.SelectedIndex = 0;
            ComboBoxThemes.ItemsSource = XTermApp.Context.Manifest.ThemeList;
            ComboBoxThemes.SelectedIndex = 0;

            // 串口波特率列表
            ComboBoxSerialPortBaudRate.ItemsSource = XTermDefaultValues.DefaultSerialPortBaudRates;
        }

        private void CollapsedAllGrid()
        {
            GridSessionSSH.Visibility = Visibility.Collapsed;
            GridSessionSerialPort.Visibility = Visibility.Collapsed;
        }

        #endregion

        #region 事件处理器

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SessionTypeVM sessionType = ComboBoxSessionTypes.SelectedItem as SessionTypeVM;
            if (sessionType == null)
            {
                return;
            }

            this.CollapsedAllGrid();

            switch (sessionType.Type)
            {
                case SessionTypeEnum.libvtssh:
                case SessionTypeEnum.SSH:
                    {
                        GridSessionSSH.Visibility = Visibility.Visible;
                        break;
                    }

                case SessionTypeEnum.SerialPort:
                    {
                        ComboBoxSerialPortNames.ItemsSource = SerialPort.GetPortNames();
                        GridSessionSerialPort.Visibility = Visibility.Visible;
                        break;
                    }

                case SessionTypeEnum.Win32CommandLine:
                    {
                        break;
                    }

                default:
                    {
                        throw new NotImplementedException();
                    }
            }
        }

        private void ComboBoxAuthList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
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

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            int row, column;
            if (!int.TryParse(TextBoxTerminalRows.Text, out row))
            {
                return;
            }

            if (!int.TryParse(TextBoxTerminalColumns.Text, out column))
            {
                return;
            }

            string sessionName = TextBoxSessionName.Text;
            if (string.IsNullOrEmpty(sessionName))
            {
                MessageBoxUtils.Info("请输入会话名称");
                return;
            }

            SessionTypeVM sessionType = ComboBoxSessionTypes.SelectedItem as SessionTypeVM;
            if (sessionType == null)
            {
                return;
            }

            AppearanceOptions appearanceOptions = this.GetAppearanceOptions();
            if (appearanceOptions == null) 
            {
                return;
            }

            // 保存新创建的Session信息
            XTermSession session = new XTermSession()
            {
                ID = Guid.NewGuid().ToString(),
                Name = TextBoxSessionName.Text,
                CreationTime = DateTime.Now,
                TerminalOptions = new TerminalOptions()
                {
                    Rows = row,
                    Columns = column,
                    Type = (int)XTermDefaultValues.DefaultTerminalType
                },
                SessionType = (int)sessionType.Type,
                InputEncoding = XTermDefaultValues.DefaultInputEncoding,
                OutputBufferSize = XTermDefaultValues.DefaultOutptBufferSize,
                MouseOptions = new MouseOptions()
                {
                    CursorStyle = VTCursorStyles.Line,
                    CursorInterval = XTermDefaultValues.DefaultCursorBlinkInterval,
                    ScrollDelta = XTermDefaultValues.DefaultScrollSensitivity
                },
                AppearanceOptions = appearanceOptions
            };

            switch (sessionType.Type)
            {
                case SessionTypeEnum.SSH:
                    {
                        string hostName = TextBoxSSHHostName.Text;
                        if (string.IsNullOrEmpty(hostName))
                        {
                            MessageBoxUtils.Info("请输入主机名称");
                            return;
                        }

                        int port;
                        if (!int.TryParse(TextBoxSSHPort.Text, out port) ||
                            port < XTermDefaultValues.MIN_PORT || port > XTermDefaultValues.MAX_PORT)
                        {
                            MessageBoxUtils.Info("请输入正确的端口号");
                            return;
                        }

                        if (ComboBoxAuthList.SelectedItem == null)
                        {
                            MessageBoxUtils.Info("请选择身份验证方式");
                            return;
                        }

                        string userName = TextBoxSSHUserName.Text;
                        if (string.IsNullOrEmpty(userName))
                        {
                            MessageBoxUtils.Info("请输入用户名");
                            return;
                        }

                        string password = string.Empty;
                        string privateKey = string.Empty;
                        string passphrase = string.Empty;

                        SSHAuthTypeEnum authType = (SSHAuthTypeEnum)ComboBoxAuthList.SelectedItem;
                        switch (authType)
                        {
                            case SSHAuthTypeEnum.None:
                                {
                                    break;
                                }

                            case SSHAuthTypeEnum.Password:
                                {

                                    password = PasswordBoxSSHPassword.Password;
                                    if (string.IsNullOrEmpty(password))
                                    {
                                        MessageBoxUtils.Info("请输入密码");
                                        return;
                                    }

                                    break;
                                }

                            case SSHAuthTypeEnum.PrivateKey:
                                {
                                    string privateKeyFile = TextBoxSSHPrivateKey.Text;
                                    if (string.IsNullOrEmpty(privateKeyFile))
                                    {
                                        MessageBoxUtils.Info("请选择密钥文件");
                                        return;
                                    }

                                    if (!File.Exists(privateKeyFile))
                                    {
                                        MessageBoxUtils.Info("密钥文件不存在");
                                        return;
                                    }

                                    // 密钥密码可以为空
                                    passphrase = PasswordBoxSSHPassphrase.Password;
                                    privateKey = File.ReadAllText(privateKeyFile);

                                    break;
                                }

                            default:
                                throw new NotImplementedException();
                        }

                        session.ConnectionOptions = new ConnectionOptions()
                        {
                            SSHAuthType = (int)authType,
                            ServerAddress = hostName,
                            ServerPort = port,
                            UserName = userName,
                            Password = password,
                            PrivateKey = privateKey,
                            Passphrase = passphrase
                        };
                        break;
                    }

                case SessionTypeEnum.SerialPort:
                    {
                        string portName = ComboBoxSerialPortNames.Text;
                        if (string.IsNullOrEmpty(portName))
                        {
                            MessageBoxUtils.Info("请输入端口号");
                            return;
                        }

                        int baudRate;
                        if (string.IsNullOrEmpty(ComboBoxSerialPortBaudRate.Text) ||
                            !int.TryParse(ComboBoxSerialPortBaudRate.Text, out baudRate))
                        {
                            MessageBoxUtils.Info("请输入波特率");
                            return;
                        }

                        session.ConnectionOptions = new ConnectionOptions()
                        {
                            ServerAddress = portName,
                            BaudRate = baudRate
                        };
                        break;
                    }

                case SessionTypeEnum.Win32CommandLine:
                    {
                        break;
                    }

                default:
                    throw new NotImplementedException();
            }

            this.Session = session;

            base.DialogResult = true;
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            base.DialogResult = false;
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
    }
}

using DotNEToolkit;
using System;
using System.Collections.Generic;
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
using XTerminal.Base.DataModels.Session;
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

            // 初始化SSH验证方式列表
            ComboBoxAuthList.ItemsSource = Enum.GetValues(typeof(SSHAuthTypeEnum));
            ComboBoxAuthList.SelectedIndex = 0;
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
                        RowDefinitionPassword.Height = new GridLength(0);
                        RowDefinitionUserName.Height = new GridLength(0);
                        break;
                    }

                case SSHAuthTypeEnum.Password:
                    {
                        RowDefinitionPassword.Height = new GridLength(35);
                        RowDefinitionUserName.Height = new GridLength(35);
                        break;
                    }

                case SSHAuthTypeEnum.PulicKey:
                    {
                        RowDefinitionPassword.Height = new GridLength(0);
                        RowDefinitionUserName.Height = new GridLength(0);
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
                    Type = (int)TerminalTypeEnum.VT100
                },
                SessionType = (int)sessionType.Type,
                InputEncoding = XTermDefaultValues.DefaultInputEncoding,
                OutputBufferSize = XTermDefaultValues.DefaultOutptBufferSize,
                MouseOptions = new MouseOptions() 
                {
                    CursorStyle = VTCursorStyles.Line,
                    CursorInterval = XTermDefaultValues.DefaultCursorBlinkInterval,
                    ScrollDelta = XTermDefaultValues.DefaultScrollSensitivity
                }
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

                        string password = string.Empty;
                        string userName = string.Empty;

                        SSHAuthTypeEnum authType = (SSHAuthTypeEnum)ComboBoxAuthList.SelectedItem;
                        switch (authType)
                        {
                            case SSHAuthTypeEnum.None:
                                {
                                    break;
                                }

                            case SSHAuthTypeEnum.Password:
                                {
                                    userName = TextBoxSSHUserName.Text;
                                    if (string.IsNullOrEmpty(userName))
                                    {
                                        MessageBoxUtils.Info("请输入用户名");
                                        return;
                                    }

                                    password = PasswordBoxSSHPassword.Password;
                                    if (string.IsNullOrEmpty(password))
                                    {
                                        MessageBoxUtils.Info("请输入密码");
                                        return;
                                    }

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
                            Password = password
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

        #endregion
    }
}

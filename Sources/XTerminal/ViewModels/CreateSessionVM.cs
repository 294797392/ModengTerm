using DotNEToolkit;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFToolkit.MVVM;
using WPFToolkit.Utility;
using XTerminal.Base;
using XTerminal.Base.DataModels;
using XTerminal.Base.Definitions;
using XTerminal.Base.Enumerations;

namespace XTerminal.ViewModels
{
    public class CreateSessionVM : ViewModelBase
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("CreateSessionVM");

        #region 实例变量

        private string sshPort;
        private string sshAddress;
        private string sshUserName;
        private string sshPassword;
        private string sshPassphrase;
        private string sshPrivateKeyFile;

        private string terminalRows;
        private string terminalColumns;

        private SessionTypeVM selectedSessionType;
        private OptionTreeVM optionTreeVM;

        private string mouseScrollDelta;

        private string sftpServerInitialDir;
        private string sftpClientInitialDir;

        #endregion

        #region 属性

        /// <summary>
        /// 当前选择的会话类型
        /// </summary>
        public SessionTypeVM SelectedSessionType
        {
            get { return this.selectedSessionType; }
            set
            {
                if (this.selectedSessionType != value)
                {
                    this.selectedSessionType = value;
                    this.NotifyPropertyChanged("SelectedSessionType");

                    // 根据不同的会话类型，切换不同的配置选项树形列表
                    switch (value.Type)
                    {
                        case SessionTypeEnum.libvtssh:
                        case SessionTypeEnum.SerialPort:
                        case SessionTypeEnum.SSH:
                        case SessionTypeEnum.Win32CommandLine:
                            {
                                this.OptionTreeVM = XTermApp.Context.TerminalOptionsTreeVM;
                                break;
                            }

                        case SessionTypeEnum.SFTP:
                            {
                                this.OptionTreeVM = XTermApp.Context.SFTPOptionsTreeVM;
                                break;
                            }

                        default:
                            throw new NotImplementedException();
                    }
                }
            }
        }

        /// <summary>
        /// 会话类型列表
        /// </summary>
        public ObservableCollection<SessionTypeVM> SessionTypeList { get; private set; }

        /// <summary>
        /// 端口号列表
        /// </summary>
        public BindableCollection<string> PortList { get; private set; }

        /// <summary>
        /// 波特率列表
        /// </summary>
        public BindableCollection<string> BaudRateList { get; private set; }

        /// <summary>
        /// SSH的身份验证方式
        /// </summary>
        public BindableCollection<SSHAuthTypeEnum> SSHAuthTypeList { get; private set; }

        public BindableCollection<FontFamilyDefinition> FontFamilyList { get; private set; }
        public BindableCollection<FontSizeDefinition> FontSizeList { get; private set; }
        public BindableCollection<ColorDefinition> ForegroundList { get; private set; }

        /// <summary>
        /// SSH端口号
        /// </summary>
        public string SSHServerPort
        {
            get { return this.sshPort; }
            set
            {
                if (this.sshPort != value)
                {
                    this.sshPort = value;
                    this.NotifyPropertyChanged("SSHServerPort");
                }
            }
        }

        public string SSHServerAddress
        {
            get { return this.sshAddress; }
            set
            {
                if (this.sshAddress != value)
                {
                    this.sshAddress = value;
                    this.NotifyPropertyChanged("SSHServerAddress");
                }
            }
        }

        public string SSHUserName
        {
            get { return this.sshUserName; }
            set
            {
                if (this.sshUserName != value)
                {
                    this.sshUserName = value;
                    this.NotifyPropertyChanged("SSHUserName");
                }
            }
        }

        public string SSHPassword
        {
            get { return this.sshPassword; }
            set
            {
                if (this.sshPassword != value)
                {
                    this.sshPassword = value;
                    this.NotifyPropertyChanged("SSHPassword");
                }
            }
        }

        public string SSHPassphrase
        {
            get { return this.sshPassphrase; }
            set
            {
                if (this.sshPassphrase != value)
                {
                    this.sshPassphrase = value;
                    this.NotifyPropertyChanged("SSHPassphrase");
                }
            }
        }

        public string SSHPrivateKeyFile
        {
            get { return this.sshPrivateKeyFile; }
            set
            {
                if (this.sshPrivateKeyFile != value)
                {
                    this.sshPrivateKeyFile = value;
                    this.NotifyPropertyChanged("SSHPrivateKeyFile");
                }
            }
        }

        /// <summary>
        /// 终端行数
        /// </summary>
        public string TerminalRows
        {
            get { return this.terminalRows; }
            set
            {
                if (this.terminalRows != value)
                {
                    this.terminalRows = value;
                    this.NotifyPropertyChanged("TerminalRows");
                }
            }
        }

        /// <summary>
        /// 终端列数
        /// </summary>
        public string TerminalColumns
        {
            get { return this.terminalColumns; }
            set
            {
                if (this.terminalColumns != value)
                {
                    this.terminalColumns = value;
                    this.NotifyPropertyChanged("TerminalColumns");
                }
            }
        }

        /// <summary>
        /// 当前要显示的选项配置树形列表ViewModel
        /// </summary>
        public OptionTreeVM OptionTreeVM
        {
            get { return this.optionTreeVM; }
            private set
            {
                if (this.optionTreeVM != value)
                {
                    this.optionTreeVM = value;
                    this.NotifyPropertyChanged("OptionTreeVM");
                }
            }
        }

        /// <summary>
        /// 终端类型列表
        /// </summary>
        public BindableCollection<TerminalTypeEnum> TerminalTypeList { get; private set; }

        public string MouseScrollDelta
        {
            get { return this.mouseScrollDelta; }
            set
            {
                if (this.mouseScrollDelta != value)
                {
                    this.mouseScrollDelta = value;
                    this.NotifyPropertyChanged("MouseScrollDelta");
                }
            }
        }


        /// <summary>
        /// SFTP初始目录
        /// </summary>
        public string SFTPServerInitialDirectory
        {
            get { return this.sftpServerInitialDir; }
            set
            {
                if (this.sftpServerInitialDir != value)
                {
                    this.sftpServerInitialDir = value;
                    this.NotifyPropertyChanged("SFTPInitialDirectory");
                }
            }
        }

        /// <summary>
        /// SFTP客户端初始化目录
        /// </summary>
        public string SFTPClientInitialDirectory
        {
            get { return this.sftpClientInitialDir; }
            set
            {
                if (this.sftpClientInitialDir != value)
                {
                    this.sftpClientInitialDir = value;
                    this.NotifyPropertyChanged("SFTPClientInitialDirectory");
                }
            }
        }

        #endregion

        #region 构造方法

        public CreateSessionVM()
        {
            this.Name = string.Format("新建会话_{0}", DateTime.Now.ToString(DateTimeFormat.yyyyMMddhhmmss));

            this.SessionTypeList = new BindableCollection<SessionTypeVM>();
            List<SessionDefinition> sessions = XTermApp.Context.ServiceAgent.GetSessionDefinitions();
            foreach (SessionDefinition session in sessions)
            {
                this.SessionTypeList.Add(new SessionTypeVM(session));
            }
            this.SelectedSessionType = this.SessionTypeList.FirstOrDefault();

            this.TerminalRows = XTermDefaultValues.TerminalRows.ToString();
            this.TerminalColumns = XTermDefaultValues.TerminalColumns.ToString();
            this.TerminalTypeList = new BindableCollection<TerminalTypeEnum>();
            this.TerminalTypeList.AddRange(Enum.GetValues(typeof(TerminalTypeEnum)).Cast<TerminalTypeEnum>());
            this.TerminalTypeList.SelectedItem = XTermDefaultValues.DefaultTerminalType;

            this.SSHAuthTypeList = new BindableCollection<SSHAuthTypeEnum>();
            this.SSHAuthTypeList.AddRange(Enum.GetValues(typeof(SSHAuthTypeEnum)).Cast<SSHAuthTypeEnum>());
            this.SSHAuthTypeList.SelectedItem = this.SSHAuthTypeList.FirstOrDefault();
            this.SSHServerPort = XTermDefaultValues.DefaultSSHPort.ToString();

            this.PortList = new BindableCollection<string>();
            this.PortList.AddRange(SerialPort.GetPortNames());
            this.PortList.SelectedItem = this.PortList.FirstOrDefault();

            this.BaudRateList = new BindableCollection<string>();
            this.BaudRateList.AddRange(XTermDefaultValues.DefaultSerialPortBaudRates);
            this.BaudRateList.SelectedItem = this.BaudRateList.FirstOrDefault();

            this.FontFamilyList = new BindableCollection<FontFamilyDefinition>();
            this.FontFamilyList.AddRange(XTermApp.Context.Manifest.FontFamilyList);
            this.FontFamilyList.SelectedItem = this.FontFamilyList.FirstOrDefault();
            this.FontSizeList = new BindableCollection<FontSizeDefinition>();
            this.FontSizeList.AddRange(XTermApp.Context.Manifest.FontSizeList);
            this.FontSizeList.SelectedItem = this.FontSizeList.FirstOrDefault();
            this.ForegroundList = new BindableCollection<ColorDefinition>();
            this.ForegroundList.AddRange(XTermApp.Context.Manifest.ForegroundList);
            this.ForegroundList.SelectedItem = this.ForegroundList.FirstOrDefault();

            this.MouseScrollDelta = XTermDefaultValues.DefaultScrollDelta.ToString();

            this.SFTPServerInitialDirectory = XTermDefaultValues.SFTPServerInitialDirectory;
            this.SFTPClientInitialDirectory = XTermDefaultValues.SFTPClientInitialDirectory;

        }

        #endregion

        #region 实例方法

        private bool GetSSHSessionOptions(XTermSession session)
        {
            if (string.IsNullOrEmpty(this.SSHServerAddress))
            {
                MessageBoxUtils.Info("请输入主机名称");
                return false;
            }

            int port;
            if (!int.TryParse(this.sshPort, out port) ||
                port < XTermDefaultValues.MIN_PORT || port > XTermDefaultValues.MAX_PORT)
            {
                MessageBoxUtils.Info("请输入正确的端口号");
                return false;
            }

            if (string.IsNullOrEmpty(this.SSHUserName))
            {
                MessageBoxUtils.Info("请输入用户名");
                return false;
            }

            SSHAuthTypeEnum authType = this.SSHAuthTypeList.SelectedItem;
            switch (authType)
            {
                case SSHAuthTypeEnum.None:
                    {
                        break;
                    }

                case SSHAuthTypeEnum.Password:
                    {
                        if (string.IsNullOrEmpty(this.SSHPassword))
                        {
                            MessageBoxUtils.Info("请输入密码");
                            return false;
                        }

                        break;
                    }

                case SSHAuthTypeEnum.PrivateKey:
                    {
                        if (string.IsNullOrEmpty(this.SSHPrivateKeyFile))
                        {
                            MessageBoxUtils.Info("请选择密钥文件");
                            return false;
                        }

                        if (!File.Exists(this.SSHPrivateKeyFile))
                        {
                            MessageBoxUtils.Info("密钥文件不存在");
                            return false;
                        }

                        // 密钥密码可以为空

                        break;
                    }

                default:
                    throw new NotImplementedException();
            }

            session.SetOption<string>(OptionKeyEnum.SSH_SERVER_ADDR, this.SSHServerAddress);
            session.SetOption<int>(OptionKeyEnum.SSH_SERVER_PORT, port);
            session.SetOption<string>(OptionKeyEnum.SSH_SERVER_USER_NAME, this.SSHUserName);
            session.SetOption<string>(OptionKeyEnum.SSH_SERVER_PASSWORD, this.SSHPassword);
            session.SetOption<string>(OptionKeyEnum.SSH_SERVER_PRIVATE_KEY_FILE, this.SSHPrivateKeyFile);
            session.SetOption<string>(OptionKeyEnum.SSH_SERVER_Passphrase, this.SSHPassphrase);
            session.SetOption<int>(OptionKeyEnum.SSH_AUTH_TYPE, (int)authType);

            return true;
        }

        private bool GetSerialPortOptions(XTermSession session)
        {
            string portName = this.PortList.SelectedItem;
            if (string.IsNullOrEmpty(portName))
            {
                MessageBoxUtils.Info("请输入正确的端口号");
                return false;
            }

            int baudRate;
            string baudRateText = this.BaudRateList.SelectedItem;
            if (string.IsNullOrEmpty(baudRateText) ||
                !int.TryParse(baudRateText, out baudRate))
            {
                MessageBoxUtils.Info("请输入正确的波特率");
                return false;
            }

            session.SetOption<string>(OptionKeyEnum.SERIAL_PORT_NAME, portName);
            session.SetOption<int>(OptionKeyEnum.SERIAL_PORT_BAUD_RATE, baudRate);

            return true;
        }

        private bool GetThemeOptions(XTermSession session)
        {
            if (this.FontFamilyList.SelectedItem == null)
            {
                MessageBoxUtils.Info("请选择字体");
                return false;
            }

            if (this.ForegroundList.SelectedItem == null)
            {
                MessageBoxUtils.Info("请选择字体颜色");
                return false;
            }

            if (this.FontSizeList.SelectedItem == null)
            {
                MessageBoxUtils.Info("请选择字号");
                return false;
            }

            session.SetOption<string>(OptionKeyEnum.SSH_THEME_FONT_FAMILY, this.FontFamilyList.SelectedItem.Value);
            session.SetOption<string>(OptionKeyEnum.SSH_THEME_FONT_COLOR, this.ForegroundList.SelectedItem.Value);
            session.SetOption<int>(OptionKeyEnum.SSH_THEME_FONT_SIZE, this.FontSizeList.SelectedItem.Value);
            session.SetOption<int>(OptionKeyEnum.CURSOR_STYLE, (int)VTCursorStyles.Line);
            session.SetOption<int>(OptionKeyEnum.CURSOR_INTERVAL, XTermDefaultValues.CURSOR_BLINK_INTERVAL);

            return true;
        }

        private bool GetTerminalOptions(XTermSession session)
        {
            int row, column;
            if (!int.TryParse(this.TerminalRows, out row))
            {
                MessageBoxUtils.Info("请输入正确的终端行数");
                return false;
            }

            if (!int.TryParse(this.TerminalColumns, out column))
            {
                MessageBoxUtils.Info("请输入正确的终端列数");
                return false;
            }

            TerminalTypeEnum terminalType = this.TerminalTypeList.SelectedItem;

            session.SetOption<int>(OptionKeyEnum.SSH_TERM_ROW, row);
            session.SetOption<int>(OptionKeyEnum.SSH_TERM_COL, column);
            session.SetOption<string>(OptionKeyEnum.SSH_TERM_TYPE, terminalType.ToString());
            session.SetOption<string>(OptionKeyEnum.WRITE_ENCODING, XTermDefaultValues.DefaultOutputEncoding);
            session.SetOption<int>(OptionKeyEnum.READ_BUFFER_SIZE, XTermDefaultValues.DefaultReadBufferSize);

            return true;
        }

        private bool GetMouseOptions(XTermSession session)
        {
            int scrollDelta;
            if (!int.TryParse(this.MouseScrollDelta, out scrollDelta))
            {
                scrollDelta = XTermDefaultValues.DefaultScrollDelta;
            }

            session.SetOption<int>(OptionKeyEnum.MOUSE_SCROLL_DELTA, scrollDelta);

            return true;
        }

        private bool GetSFTPOptions(XTermSession session)
        {
            string serverInitialDir = this.SFTPServerInitialDirectory;
            if (string.IsNullOrEmpty(serverInitialDir))
            {
                serverInitialDir = XTermDefaultValues.SFTPServerInitialDirectory;
            }

            string clientInitialDir = this.SFTPClientInitialDirectory;
            if (string.IsNullOrEmpty(clientInitialDir) ||
                !Directory.Exists(clientInitialDir))
            {
                clientInitialDir = XTermDefaultValues.SFTPClientInitialDirectory;
            }

            session.SetOption<string>(OptionKeyEnum.SFTP_SERVER_INITIAL_DIRECTORY, serverInitialDir);
            session.SetOption<string>(OptionKeyEnum.SFTP_CLIENT_INITIAL_DIRECTORY, clientInitialDir);

            return true;
        }



        private bool CollectOptions(XTermSession session)
        {
            SessionTypeVM sessionType = this.SelectedSessionType;
            if (sessionType == null)
            {
                MessageBoxUtils.Info("请选择会话类型");
                return false;
            }

            switch (sessionType.Type)
            {
                case SessionTypeEnum.libvtssh:
                case SessionTypeEnum.SSH:
                    {
                        if (!this.GetSSHSessionOptions(session))
                        {
                            return false;
                        }
                        break;
                    }

                case SessionTypeEnum.Win32CommandLine:
                    {
                        break;
                    }

                case SessionTypeEnum.SerialPort:
                    {
                        if (!this.GetSerialPortOptions(session))
                        {
                            return false;
                        }
                        break;
                    }

                case SessionTypeEnum.SFTP:
                    {
                        if(!this.GetSFTPOptions(session))
                        {
                            return false;
                        }
                        return true;
                    }

                default:
                    throw new NotImplementedException();
            }

            if (!this.GetThemeOptions(session) ||
                !this.GetTerminalOptions(session) ||
                !this.GetMouseOptions(session))
            {
                return false;
            }

            return true;
        }

        #endregion

        public XTermSession GetSession()
        {
            if (string.IsNullOrEmpty(this.Name))
            {
                MessageBoxUtils.Info("请输入会话名称");
                return null;
            }

            SessionTypeVM sessionType = this.SelectedSessionType;
            if (sessionType == null)
            {
                MessageBoxUtils.Info("请选择会话类型");
                return null;
            }

            XTermSession session = new XTermSession()
            {
                ID = Guid.NewGuid().ToString(),
                Name = this.Name,
                SessionType = (int)sessionType.Type
            };

            if (!this.CollectOptions(session))
            {
                return null;
            }

            return session;
        }
    }
}

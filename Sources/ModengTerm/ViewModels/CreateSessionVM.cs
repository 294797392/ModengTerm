using DotNEToolkit;
using ModengTerm.Base;
using ModengTerm.Base.DataModels;
using ModengTerm.Base.ServiceAgents;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing.Text;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using WPFToolkit.MVVM;
using WPFToolkit.Utility;
using XTerminal.Base;
using XTerminal.Base.DataModels;
using XTerminal.Base.Definitions;
using XTerminal.Base.Enumerations;

namespace ModengTerm.ViewModels
{
    public class CreateSessionVM : ViewModelBase
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("CreateSessionVM");

        /// <summary>
        /// 优先选择这些字体
        /// </summary>
        public static readonly List<string> DefaultFontFamilies = new List<string>()
        {
            "宋体", "微软雅黑", "Yahei", "Consolas"
        };

        #region 实例变量

        private string sshPort;
        private string sshAddress;
        private string sshUserName;
        private string sshPassword;
        private string sshPassphrase;
        private string sshPrivateKeyFile;

        private string terminalRows;
        private string terminalColumns;
        private string maxScrollback;

        private SessionTypeVM selectedSessionType;
        private OptionTreeVM optionTreeVM;

        private string mouseScrollDelta;

        private string sftpServerInitialDir;
        private string sftpClientInitialDir;

        private ServiceAgent serviceAgent;

        private Brush backgroundBrush;
        private Brush foregroundBrush;

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
                                this.OptionTreeVM = this.TerminalOptionsTreeVM;
                                break;
                            }

                        case SessionTypeEnum.SFTP:
                            {
                                this.OptionTreeVM = this.SFTPOptionsTreeVM;
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
        /// SSH的身份验证方式
        /// </summary>
        public BindableCollection<SSHAuthTypeEnum> SSHAuthTypeList { get; private set; }

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

        public string MaxScrollback 
        {
            get { return this.maxScrollback; }
            set
            {
                if (this.maxScrollback != value) 
                {
                    this.maxScrollback = value;
                    this.NotifyPropertyChanged("MaxScrollback");
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

        public OptionTreeVM TerminalOptionsTreeVM { get; set; }

        public OptionTreeVM SFTPOptionsTreeVM { get; set; }

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

        #region 主题相关

        /// <summary>
        /// 支持的主题列表
        /// </summary>
        public BindableCollection<Theme> ThemeList { get; private set; }

        public Brush BackgroundBrush
        {
            get { return this.backgroundBrush; }
            set
            {
                if (this.backgroundBrush != value)
                {
                    this.backgroundBrush = value;
                    this.NotifyPropertyChanged("BackgroundBrush");
                }
            }
        }

        public Brush ForegroundBrush
        {
            get { return this.foregroundBrush; }
            set
            {
                if (this.foregroundBrush != value)
                {
                    this.foregroundBrush = value;
                    this.NotifyPropertyChanged("ForegroundBrush");
                }
            }
        }

        public BindableCollection<FontFamilyDefinition> FontFamilyList { get; private set; }
        public BindableCollection<FontSizeDefinition> FontSizeList { get; private set; }

        public BindableCollection<VTCursorStyles> CursorStyles { get; private set; }

        public BindableCollection<VTCursorSpeeds> CursorSpeeds { get; private set; }

        /// <summary>
        /// 光标颜色
        /// </summary>
        public BindableCollection<ColorDefinition> CursorColors { get; private set; }

        #endregion

        #region 串口相关

        /// <summary>
        /// 端口号列表
        /// </summary>
        public BindableCollection<string> PortList { get; private set; }

        /// <summary>
        /// 波特率列表
        /// </summary>
        public BindableCollection<string> BaudRateList { get; private set; }

        public BindableCollection<int> DataBitsList { get; private set; }

        public BindableCollection<StopBits> StopBitsList { get; private set; }

        public BindableCollection<Parity> ParityList { get; private set; }

        public BindableCollection<Handshake> HandshakeList { get; private set; }

        #endregion

        #endregion

        #region 构造方法

        public CreateSessionVM(ServiceAgent serviceAgent)
        {
            this.serviceAgent = serviceAgent;

            MTermManifest appManifest = serviceAgent.GetManifest();

            this.Name = string.Format("新建会话_{0}", DateTime.Now.ToString(DateTimeFormat.yyyyMMddhhmmss));

            // 加载参数树形列表
            this.SFTPOptionsTreeVM = new OptionTreeVM();
            this.TerminalOptionsTreeVM = new OptionTreeVM();
            this.LoadOptionsTree(this.SFTPOptionsTreeVM, appManifest.FTPOptionList);
            this.LoadOptionsTree(this.TerminalOptionsTreeVM, appManifest.TerminalOptionList);
            this.OptionTreeVM = this.TerminalOptionsTreeVM;

            this.SessionTypeList = new BindableCollection<SessionTypeVM>();
            foreach (SessionDefinition session in appManifest.SessionList)
            {
                this.SessionTypeList.Add(new SessionTypeVM(session));
            }
            this.SelectedSessionType = this.SessionTypeList.FirstOrDefault();

            #region 终端

            this.TerminalRows = MTermConsts.TerminalRows.ToString();
            this.TerminalColumns = MTermConsts.TerminalColumns.ToString();
            this.TerminalTypeList = new BindableCollection<TerminalTypeEnum>();
            this.TerminalTypeList.AddRange(Enum.GetValues(typeof(TerminalTypeEnum)).Cast<TerminalTypeEnum>());
            this.TerminalTypeList.SelectedItem = MTermConsts.DefaultTerminalType;
            this.MaxScrollback = MTermConsts.DefaultTerminalScrollback.ToString();

            #endregion

            this.SSHAuthTypeList = new BindableCollection<SSHAuthTypeEnum>();
            this.SSHAuthTypeList.AddRange(Enum.GetValues(typeof(SSHAuthTypeEnum)).Cast<SSHAuthTypeEnum>());
            this.SSHAuthTypeList.SelectedItem = this.SSHAuthTypeList.FirstOrDefault();
            this.SSHServerPort = MTermConsts.DefaultSSHPort.ToString();

            #region 串口

            this.PortList = new BindableCollection<string>();
            this.PortList.AddRange(SerialPort.GetPortNames());
            this.PortList.SelectedItem = this.PortList.FirstOrDefault();

            this.BaudRateList = new BindableCollection<string>();
            this.BaudRateList.AddRange(MTermConsts.DefaultSerialPortBaudRates);
            this.BaudRateList.SelectedItem = this.BaudRateList.FirstOrDefault();

            this.DataBitsList = new BindableCollection<int>();
            this.DataBitsList.AddRange(MTermConsts.DefaultSerialPortDataBits);
            this.DataBitsList.SelectedItem = this.DataBitsList.LastOrDefault(); // LastOrDfault是8

            this.StopBitsList = new BindableCollection<StopBits>();
            this.StopBitsList.AddRange(Enum.GetValues(typeof(StopBits)).Cast<StopBits>());
            this.StopBitsList.SelectedItem = StopBits.One;

            this.ParityList = new BindableCollection<Parity>();
            this.ParityList.AddRange(Enum.GetValues(typeof(Parity)).Cast<Parity>());
            this.ParityList.SelectedItem = Parity.None;

            this.HandshakeList = new BindableCollection<Handshake>();
            this.HandshakeList.AddRange(Enum.GetValues(typeof(Handshake)).Cast<Handshake>());
            this.HandshakeList.SelectedItem = Handshake.None;

            #endregion

            #region Theme

            this.FontFamilyList = new BindableCollection<FontFamilyDefinition>();
            this.FontFamilyList.AddRange(appManifest.FontFamilyList);
            // 加载系统已安装的所有字体
            InstalledFontCollection installedFont = new InstalledFontCollection();
            this.FontFamilyList.AddRange(installedFont.Families.Select(v => new FontFamilyDefinition() { Name = v.Name, Value = v.Name }));
            this.FontFamilyList.SelectedItem = this.GetDefaultFontFamily();

            this.FontSizeList = new BindableCollection<FontSizeDefinition>();
            this.FontSizeList.AddRange(appManifest.FontSizeList);
            this.FontSizeList.SelectedItem = this.FontSizeList.FirstOrDefault();

            this.CursorSpeeds = new BindableCollection<VTCursorSpeeds>();
            this.CursorSpeeds.AddRange(Enum.GetValues(typeof(VTCursorSpeeds)).Cast<VTCursorSpeeds>());
            this.CursorSpeeds.SelectedItem = MTermConsts.DefaultCursorBlinkSpeed;

            this.CursorStyles = new BindableCollection<VTCursorStyles>();
            this.CursorStyles.AddRange(Enum.GetValues(typeof(VTCursorStyles)).Cast<VTCursorStyles>());
            this.CursorStyles.SelectedItem = MTermConsts.DefaultCursorStyle;

            this.CursorColors = new BindableCollection<ColorDefinition>();
            this.CursorColors.AddRange(appManifest.ColorList);
            this.CursorColors.SelectedItem = this.CursorColors.FirstOrDefault();

            // 最后加载ThemeList，因为在ThemeList_SelectionChanged事件里需要用到其他Theme字段的数据
            this.ThemeList = new BindableCollection<Theme>();
            this.ThemeList.AddRange(appManifest.ThemeList);
            this.ThemeList.SelectionChanged += ThemeList_SelectionChanged;
            this.ThemeList.SelectedItem = this.ThemeList.FirstOrDefault();

            #endregion

            this.MouseScrollDelta = MTermConsts.DefaultScrollDelta.ToString();

            this.SFTPServerInitialDirectory = MTermConsts.SFTPServerInitialDirectory;
            this.SFTPClientInitialDirectory = MTermConsts.SFTPClientInitialDirectory;
        }

        #endregion

        #region 实例方法

        private FontFamilyDefinition GetDefaultFontFamily()
        {
            foreach (string fontFamily in DefaultFontFamilies)
            {
                FontFamilyDefinition defaultFont = this.FontFamilyList.FirstOrDefault(v => v.Value == fontFamily);
                if (defaultFont != null) 
                {
                    return defaultFont;
                }
            }

            return null;
        }

        private void LoadChildrenOptions(OptionTreeNodeVM parentNode, List<OptionDefinition> children)
        {
            foreach (OptionDefinition option in children)
            {
                OptionTreeNodeVM vm = new OptionTreeNodeVM(parentNode.Context, option)
                {
                    ID = option.ID,
                    Name = option.Name,
                    EntryClass = option.EntryClass,
                    IsExpanded = true
                };

                parentNode.AddChildNode(vm);

                this.LoadChildrenOptions(vm, option.Children);
            }
        }

        private void LoadOptionsTree(OptionTreeVM treeVM, List<OptionDefinition> options)
        {
            foreach (OptionDefinition option in options)
            {
                OptionTreeNodeVM vm = new OptionTreeNodeVM(treeVM.Context, option)
                {
                    ID = option.ID,
                    Name = option.Name,
                    EntryClass = option.EntryClass,
                    IsExpanded = true
                };

                treeVM.AddRootNode(vm);

                this.LoadChildrenOptions(vm, option.Children);
            }

            // 默认选中第一个节点
            TreeNodeViewModel firstNode = treeVM.Roots.FirstOrDefault();
            if (firstNode != null)
            {
                firstNode.IsSelected = true;
            }
        }

        private string GetTerminalName(TerminalTypeEnum type)
        {
            switch (type)
            {
                case TerminalTypeEnum.VT100: return "vt100";
                case TerminalTypeEnum.VT220: return "vt220";
                case TerminalTypeEnum.XTerm: return "xterm";
                case TerminalTypeEnum.XTerm256Color: return "xterm-256color";
                default:
                    throw new NotImplementedException();
            }
        }

        private bool GetSSHSessionOptions(XTermSession session)
        {
            if (string.IsNullOrEmpty(this.SSHServerAddress))
            {
                MessageBoxUtils.Info("请输入主机名称");
                return false;
            }

            int port;
            if (!int.TryParse(this.sshPort, out port) ||
                port < MTermConsts.MIN_PORT || port > MTermConsts.MAX_PORT)
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
            session.SetOption<int>(OptionKeyEnum.SSH_SERVER_AUTH_TYPE, (int)authType);

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
            session.SetOption<int>(OptionKeyEnum.SERIAL_PORT_DATA_BITS, this.DataBitsList.SelectedItem);
            session.SetOption<StopBits>(OptionKeyEnum.SERIAL_PORT_STOP_BITS, this.StopBitsList.SelectedItem);
            session.SetOption<Parity>(OptionKeyEnum.SERIAL_PORT_PARITY, this.ParityList.SelectedItem);
            session.SetOption<Handshake>(OptionKeyEnum.SERIAL_PORT_HANDSHAKE, this.HandshakeList.SelectedItem);

            return true;
        }

        private bool GetThemeOptions(XTermSession session)
        {
            if (this.FontFamilyList.SelectedItem == null)
            {
                MessageBoxUtils.Info("请选择字体");
                return false;
            }

            if (this.FontSizeList.SelectedItem == null)
            {
                MessageBoxUtils.Info("请选择字号");
                return false;
            }

            if (this.ThemeList.SelectedItem == null)
            {
                MessageBoxUtils.Info("请选择主题");
                return false;
            }

            session.SetOption<string>(OptionKeyEnum.SSH_THEME_FONT_FAMILY, this.FontFamilyList.SelectedItem.Value);
            session.SetOption<int>(OptionKeyEnum.SSH_THEME_FONT_SIZE, this.FontSizeList.SelectedItem.Value);
            session.SetOption<string>(OptionKeyEnum.SSH_THEME_BACK_COLOR, this.ThemeList.SelectedItem.BackgroundColor);
            session.SetOption<string>(OptionKeyEnum.SSH_THEME_FORE_COLOR, this.ThemeList.SelectedItem.ForegroundColor);
            session.SetOption<int>(OptionKeyEnum.SSH_THEME_CURSOR_STYLE, (int)this.CursorStyles.SelectedItem);
            session.SetOption<int>(OptionKeyEnum.SSH_THEME_CURSOR_SPEED, (int)this.CursorSpeeds.SelectedItem);
            session.SetOption<string>(OptionKeyEnum.SSH_THEME_CURSOR_COLOR, this.CursorColors.SelectedItem.Value);
            session.SetOption<string>(OptionKeyEnum.SSH_THEME_ID, this.ThemeList.SelectedItem.ID);
            session.SetOption<Dictionary<string, string>>(OptionKeyEnum.SSH_TEHEM_COLOR_TABLE, this.ThemeList.SelectedItem.ColorTable);

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

            int scrollback;
            if (!int.TryParse(this.MaxScrollback, out scrollback))
            {
                MessageBoxUtils.Info("请输入正确的回滚行数");
                return false;
            }

            TerminalTypeEnum terminalType = this.TerminalTypeList.SelectedItem;

            session.SetOption<int>(OptionKeyEnum.SSH_TERM_ROW, row);
            session.SetOption<int>(OptionKeyEnum.SSH_TERM_COL, column);
            session.SetOption<string>(OptionKeyEnum.SSH_TERM_TYPE, this.GetTerminalName(terminalType));
            session.SetOption<TerminalSizeModeEnum>(OptionKeyEnum.SSH_TERM_SIZE_MODE, TerminalSizeModeEnum.AutoFit);
            session.SetOption<string>(OptionKeyEnum.WRITE_ENCODING, MTermConsts.DefaultWriteEncoding);
            session.SetOption<int>(OptionKeyEnum.READ_BUFFER_SIZE, MTermConsts.DefaultReadBufferSize);
            session.SetOption<int>(OptionKeyEnum.TERM_MAX_SCROLLBACK, scrollback);

            return true;
        }

        private bool GetMouseOptions(XTermSession session)
        {
            int scrollDelta;
            if (!int.TryParse(this.MouseScrollDelta, out scrollDelta))
            {
                scrollDelta = MTermConsts.DefaultScrollDelta;
            }

            session.SetOption<int>(OptionKeyEnum.MOUSE_SCROLL_DELTA, scrollDelta);

            return true;
        }

        private bool GetSFTPOptions(XTermSession session)
        {
            string serverInitialDir = this.SFTPServerInitialDirectory;
            if (string.IsNullOrEmpty(serverInitialDir))
            {
                serverInitialDir = MTermConsts.SFTPServerInitialDirectory;
            }

            string clientInitialDir = this.SFTPClientInitialDirectory;
            if (string.IsNullOrEmpty(clientInitialDir) ||
                !Directory.Exists(clientInitialDir))
            {
                clientInitialDir = MTermConsts.SFTPClientInitialDirectory;
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
                        if (!this.GetSFTPOptions(session))
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

        #region 事件处理器

        private void ThemeList_SelectionChanged(Theme oldValue, Theme newValue)
        {
            if (newValue == null)
            {
                return;
            }

            this.BackgroundBrush = MTermUtils.GetBrush(newValue.BackgroundColor);
            this.ForegroundBrush = MTermUtils.GetBrush(newValue.ForegroundColor);
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
                CreationTime = DateTime.Now,
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

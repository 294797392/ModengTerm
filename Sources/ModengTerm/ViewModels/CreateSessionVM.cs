using DotNEToolkit;
using ModengTerm.Base;
using ModengTerm.Base.DataModels;
using ModengTerm.Base.Definitions;
using ModengTerm.Base.Enumerations;
using ModengTerm.Base.Enumerations.Terminal;
using ModengTerm.Base.ServiceAgents;
using ModengTerm.Document;
using ModengTerm.Document.Enumerations;
using ModengTerm.Terminal;
using ModengTerm.Terminal.DataModels;
using ModengTerm.Terminal.Enumerations;
using ModengTerm.UserControls.TerminalUserControls.Rendering;
using ModengTerm.ViewModels.CreateSession;
using ModengTerm.ViewModels.Session;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing.Text;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Windows.Media;
using WPFToolkit.MVVM;
using WPFToolkit.Utility;
using XTerminal.Base.Definitions;
using XTerminal.Base.Enumerations;

namespace ModengTerm.ViewModels
{
    /// <summary>
    /// 新建Session窗口的ViewModel
    /// </summary>
    public class CreateSessionVM : ViewModelBase
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("CreateSessionVM");

        #endregion

        #region 实例变量

        private string sshPort;
        private string sshAddress;
        private string sshUserName;
        private string sshPassword;
        private string sshPassphrase;
        private string sshPrivateKeyId;

        private string terminalRows;
        private string terminalColumns;
        private string maxScrollback;
        private string maxCliboardHistory;

        private SessionTypeVM selectedSessionType;
        private OptionMenuVM optionTreeVM;

        private string mouseScrollDelta;

        private string sftpServerInitialDir;
        private string sftpClientInitialDir;

        private ServiceAgent serviceAgent;

        private MTermManifest appManifest;
        private TerminalManifest terminalManifest;

        private bool bookmarkVisible;

        private Color backColor;
        private Color fontColor;
        private Color highlightFontColor;
        private Color highlightBackColor;
        private Color cursorColor;
        private Color selectionColor;

        #region 命令行

        private string startupPath;
        private string startupArgument;
        private string startupDir;

        #endregion

        #region 终端

        private bool disableBell;

        #endregion

        #region 串口

        private string selectedPort;

        #endregion


        private string rawTcpAddress;
        private int rawTcpPort;

        /// <summary>
        /// 当前选中的菜单节点
        /// </summary>
        private OptionMenuItemVM selectedMenuNode;

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

                    switch (value.Type)
                    {
                        case SessionTypeEnum.SFTP:
                            {
                                break;
                            }

                        default:
                            {
                                // 根据不同的会话类型，切换不同的配置选项树形列表
                                OptionMenuItemVM selectedNode;
                                if (this.OptionTreeVM.TryGetItem(value.MenuId, out selectedNode)) 
                                {
                                    if (this.selectedMenuNode != null)
                                    {
                                        this.selectedMenuNode.IsVisible = false;
                                        this.selectedMenuNode.IsSelected = false;
                                    }

                                    selectedNode.IsVisible = true;
                                    selectedNode.IsSelected = true;
                                    this.selectedMenuNode = selectedNode;
                                }

                                break;
                            }
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
        /// 所有的会话分组列表
        /// </summary>
        public SessionTreeVM SessionGroups { get; private set; }

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

        public string SSHPrivateKeyId
        {
            get { return this.sshPrivateKeyId; }
            set
            {
                if (this.sshPrivateKeyId != value)
                {
                    this.sshPrivateKeyId = value;
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
        /// 剪贴板历史记录最大数
        /// </summary>
        public string MaxClipboardHistory
        {
            get
            {
                return this.maxCliboardHistory;
            }
            set
            {
                if (this.maxCliboardHistory != value)
                {
                    this.maxCliboardHistory = value;
                    this.NotifyPropertyChanged("MaxClipboardHistory");
                }
            }
        }

        /// <summary>
        /// 当前显示的配置树形列表ViewModel
        /// </summary>
        public OptionMenuVM OptionTreeVM
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

        public OptionMenuVM TerminalOptionsTreeVM { get; private set; }

        public OptionMenuVM SFTPOptionsTreeVM { get; private set; }

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

        #region 会话 - 系统监控

        /// <summary>
        /// 更新频率列表
        /// </summary>
        public BindableCollection<WatchFrequencyEnum> WatchFrequencies { get; private set; }

        #endregion

        #region 命令行

        public string StartupPath
        {
            get { return this.startupPath; }
            set
            {
                if (this.startupPath != value)
                {
                    this.startupPath = value;
                    this.NotifyPropertyChanged("StartupPath");
                }
            }
        }

        public string StartupArgument
        {
            get { return this.startupArgument; }
            set
            {
                if (this.startupArgument != value)
                {
                    this.startupArgument = value;
                    this.NotifyPropertyChanged("StartupArgument");
                }
            }
        }

        public string StartupDirectory
        {
            get { return this.startupDir; }
            set
            {
                if (this.startupDir != value)
                {
                    this.startupDir = value;
                    this.NotifyPropertyChanged("StartupDirectory");
                }
            }
        }

        public BindableCollection<CmdDriverEnum> CmdDrivers { get; private set; }

        #endregion

        #region 终端

        public bool DisableBell
        {
            get { return this.disableBell; }
            set
            {
                if (this.disableBell != value)
                {
                    this.disableBell = value;
                    this.NotifyPropertyChanged("DisableBell");
                }
            }
        }

        #endregion

        #region 终端 - 行为

        public BindableCollection<BehaviorRightClicks> BehaviorRightClicks { get; private set; }

        #endregion

        #region 主题相关

        /// <summary>
        /// 支持的主题列表
        /// </summary>
        public BindableCollection<ThemePackage> ThemeList { get; private set; }

        public BindableCollection<FontFamilyDefinition> FontFamilyList { get; private set; }
        public BindableCollection<FontSizeDefinition> FontSizeList { get; private set; }

        public BindableCollection<VTCursorStyles> CursorStyles { get; private set; }

        public BindableCollection<VTCursorSpeeds> CursorSpeeds { get; private set; }

        /// <summary>
        /// 光标颜色
        /// </summary>
        public Color CursorColor
        {
            get { return this.cursorColor; }
            set
            {
                if (this.cursorColor != value)
                {
                    this.cursorColor = value;
                    this.NotifyPropertyChanged("CursorColor");
                }
            }
        }

        /// <summary>
        /// 前景色
        /// </summary>
        public Color FontColor
        {
            get { return this.fontColor; }
            set
            {
                if (this.fontColor != value)
                {
                    this.fontColor = value;
                    this.NotifyPropertyChanged("FontColor");
                }
            }
        }

        /// <summary>
        /// 背景色
        /// </summary>
        public Color BackColor
        {
            get { return this.backColor; }
            set
            {
                if (this.backColor != value)
                {
                    this.backColor = value;
                    this.NotifyPropertyChanged("BackColor");
                }
            }
        }

        public Color HighlightBackColor
        {
            get { return this.highlightBackColor; }
            set
            {
                if (this.highlightBackColor != value)
                {
                    this.highlightBackColor = value;
                    this.NotifyPropertyChanged("HighlightBackColor");
                }
            }
        }

        public Color HighlightFontColor
        {
            get { return this.highlightFontColor; }
            set
            {
                if (this.highlightFontColor != value)
                {
                    this.highlightFontColor = value;
                    this.NotifyPropertyChanged("HighlightFontColor");
                }
            }
        }

        /// <summary>
        /// 选中颜色
        /// </summary>
        public Color SelectionColor
        {
            get { return this.selectionColor; }
            set
            {
                if (this.selectionColor != value)
                {
                    this.selectionColor = value;
                    this.NotifyPropertyChanged("SelectionColor");
                }
            }
        }


        public string ScrollbarThumbColor { get; set; }
        public string ScrollbarButtonColor { get; set; }
        public string ScrollbarTrackColor { get; set; }

        /// <summary>
        /// 是否显示书签
        /// </summary>
        public bool BookmarkVisible
        {
            get { return this.bookmarkVisible; }
            set
            {
                if (this.bookmarkVisible != value)
                {
                    this.bookmarkVisible = value;
                    this.NotifyPropertyChanged("BookmarkVisible");
                }
            }
        }

        #endregion

        #region 串口相关

        /// <summary>
        /// 端口号列表
        /// </summary>
        public BindableCollection<string> PortList { get; private set; }

        public string SelectedPort 
        {
            get { return this.selectedPort; }
            set
            {
                if (this.selectedPort != value) 
                {
                    this.selectedPort = value;
                    this.NotifyPropertyChanged("SelectedPort");
                }
            }
        }

        /// <summary>
        /// 波特率列表
        /// </summary>
        public BindableCollection<string> BaudRateList { get; private set; }

        public string SelectedBaudRate { get; set; }

        public BindableCollection<int> DataBitsList { get; private set; }

        public BindableCollection<StopBits> StopBitsList { get; private set; }

        public BindableCollection<Parity> ParityList { get; private set; }

        public BindableCollection<Handshake> HandshakeList { get; private set; }

        #endregion

        #region Tcp

        public BindableCollection<RawTcpTypeEnum> RawTcpTypes { get; private set; }

        public string RawTcpAddress
        {
            get { return this.rawTcpAddress; }
            set
            {
                if (this.rawTcpAddress != value)
                {
                    this.rawTcpAddress = value;
                    this.NotifyPropertyChanged("RawTcpAddress");
                }
            }
        }

        public int RawTcpPort
        {
            get { return this.rawTcpPort; }
            set
            {
                if (this.rawTcpPort != value)
                {
                    this.rawTcpPort = value;
                    this.NotifyPropertyChanged("RawTcpPort");
                }
            }
        }

        #endregion

        #region 端口转发

        /// <summary>
        /// 所有端口转发列表
        /// </summary>
        public BindableCollection<PortForward> PortForwards { get; private set; }

        #endregion

        #region AdbShell

        public BindableCollection<AdbLoginTypeEnum> AdbLoginTypes { get; private set; }

        public string AdbUserName { get; set; }
        public string AdbUserNamePrompt { get; set; }
        public string AdbPassword { get; set; }
        public string AdbPasswordPrompt { get; set; }
        public string AdbShellPrompt { get; set; }
        public string AdbPath { get; set; }
        public string AdbLoginTimeout { get; set; }

        #endregion

        #endregion

        #region 构造方法

        public CreateSessionVM(ServiceAgent serviceAgent)
        {
            this.serviceAgent = serviceAgent;

            MTermManifest appManifest = MTermApp.Context.Manifest;
            TerminalManifest terminalManifest = VTermUtils.GetManifest();

            this.appManifest = appManifest;
            this.terminalManifest = terminalManifest;

            //this.Name = string.Format("新建会话_{0}", DateTime.Now.ToString(DateTimeFormat.yyyyMMddhhmmss));
            this.Name = "新建会话";

            #region 加载参数树形列表

            // 加载参数树形列表
            this.SFTPOptionsTreeVM = new OptionMenuVM();
            this.TerminalOptionsTreeVM = new OptionMenuVM();
            this.TerminalOptionsTreeVM.Initialize(appManifest.TerminalOptionMenu);
            this.TerminalOptionsTreeVM.ExpandAll();
            this.OptionTreeVM = this.TerminalOptionsTreeVM;

            #endregion

            #region 会话类型和会话分组

            this.SessionTypeList = new BindableCollection<SessionTypeVM>();
            foreach (SessionDefinition session in appManifest.SessionList)
            {
                this.SessionTypeList.Add(new SessionTypeVM(session));
                // 隐藏会话类型对应的菜单，等选中的时候再显示
                OptionMenuItemVM treeNodeViewModel;
                if (this.TerminalOptionsTreeVM.TryGetItem(session.MenuId, out treeNodeViewModel)) 
                {
                    treeNodeViewModel.IsVisible = false;
                }
            }
            // 选择第一个会话
            this.SelectedSessionType = this.SessionTypeList.FirstOrDefault();

            this.SessionGroups = MTermApp.Context.CreateSessionTreeVM(true, true);
            this.SessionGroups.ExpandAll();

            #endregion

            #region 会话 - 系统监控

            this.WatchFrequencies = new BindableCollection<WatchFrequencyEnum>();
            this.WatchFrequencies.AddRange(MTermUtils.GetEnumValues<WatchFrequencyEnum>());
            this.WatchFrequencies.SelectedItem = WatchFrequencyEnum.Normal;

            #endregion

            #region 命令行

            this.StartupPath = Path.Combine(Environment.SystemDirectory, "cmd.exe");
            this.StartupDirectory = AppDomain.CurrentDomain.BaseDirectory;
            this.CmdDrivers = new BindableCollection<CmdDriverEnum>();
            this.CmdDrivers.AddRange(MTermUtils.GetEnumValues<CmdDriverEnum>());
            // 如果是Win10或更高版本那么默认选择PseudoConsoleApi
            if (MTermUtils.IsWin10())
            {
                this.CmdDrivers.SelectedItem = CmdDriverEnum.Win10PseudoConsoleApi;
            }
            else
            {
                this.CmdDrivers.SelectedItem = CmdDriverEnum.winpty;
            }

            #endregion

            #region 终端

            #region 终端

            this.TerminalRows = MTermConsts.TerminalRows.ToString();
            this.TerminalColumns = MTermConsts.TerminalColumns.ToString();
            this.TerminalTypeList = new BindableCollection<TerminalTypeEnum>();
            this.TerminalTypeList.AddRange(MTermUtils.GetEnumValues<TerminalTypeEnum>());
            this.TerminalTypeList.SelectedItem = MTermConsts.DefaultTerminalType;
            this.MaxScrollback = MTermConsts.DefaultTerminalScrollback.ToString();
            this.MaxClipboardHistory = MTermConsts.DefaultMaxClipboardHistory.ToString();

            #endregion

            #region 行为

            this.BehaviorRightClicks = new BindableCollection<BehaviorRightClicks>();
            this.BehaviorRightClicks.AddRange(MTermUtils.GetEnumValues<BehaviorRightClicks>());
            this.BehaviorRightClicks.SelectedItem = Base.Enumerations.BehaviorRightClicks.ContextMenu;

            #endregion

            #endregion

            #region 端口转发

            this.PortForwards = new BindableCollection<PortForward>();

            #endregion

            #region SSH

            this.SSHAuthTypeList = new BindableCollection<SSHAuthTypeEnum>();
            this.SSHAuthTypeList.AddRange(MTermUtils.GetEnumValues<SSHAuthTypeEnum>());
            this.SSHAuthTypeList.SelectedItem = this.SSHAuthTypeList.FirstOrDefault();
            this.SSHServerPort = MTermConsts.DefaultSSHPort.ToString();

            #endregion

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
            this.StopBitsList.AddRange(MTermUtils.GetEnumValues<StopBits>());
            this.StopBitsList.SelectedItem = StopBits.One;

            this.ParityList = new BindableCollection<Parity>();
            this.ParityList.AddRange(MTermUtils.GetEnumValues<Parity>());
            this.ParityList.SelectedItem = Parity.None;

            this.HandshakeList = new BindableCollection<Handshake>();
            this.HandshakeList.AddRange(MTermUtils.GetEnumValues<Handshake>());
            this.HandshakeList.SelectedItem = Handshake.None;

            #endregion

            #region Tcp

            this.RawTcpTypes = new BindableCollection<RawTcpTypeEnum>();
            this.RawTcpTypes.AddRange(MTermUtils.GetEnumValues<RawTcpTypeEnum>());

            #endregion

            #region AdbShell

            this.AdbLoginTypes = new BindableCollection<AdbLoginTypeEnum>();
            this.AdbLoginTypes.AddRange(MTermUtils.GetEnumValues<AdbLoginTypeEnum>());
            this.AdbPath = "adb.exe";
            this.AdbLoginTimeout = MTermConsts.DefaultAdbLoginTimeout.ToString();

            #endregion

            #region Theme

            this.ThemeList = new BindableCollection<ThemePackage>();
            this.ThemeList.AddRange(terminalManifest.DefaultThemes);
            this.ThemeList.SelectedItem = this.ThemeList.FirstOrDefault();
            this.ThemeList.SelectionChanged += ThemeList_SelectionChanged;
            ThemePackage selectedTheme = this.ThemeList.SelectedItem;

            this.FontFamilyList = new BindableCollection<FontFamilyDefinition>();
            this.FontFamilyList.AddRange(terminalManifest.FontFamilyList);

            this.FontSizeList = new BindableCollection<FontSizeDefinition>();
            this.FontSizeList.AddRange(terminalManifest.FontSizeList);

            this.CursorSpeeds = new BindableCollection<VTCursorSpeeds>();
            this.CursorSpeeds.AddRange(MTermUtils.GetEnumValues<VTCursorSpeeds>());

            this.CursorStyles = new BindableCollection<VTCursorStyles>();
            this.CursorStyles.AddRange(MTermUtils.GetEnumValues<VTCursorStyles>());

            this.SwitchTheme(selectedTheme);

            #endregion

            this.MouseScrollDelta = MTermConsts.DefaultScrollDelta.ToString();

            this.SFTPServerInitialDirectory = MTermConsts.SFTPServerInitialDirectory;
            this.SFTPClientInitialDirectory = MTermConsts.SFTPClientInitialDirectory;
        }

        #endregion

        #region 实例方法

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

        private bool GetSSHOptions(XTermSession session)
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
                        if (string.IsNullOrEmpty(this.SSHPrivateKeyId))
                        {
                            MessageBoxUtils.Info("请选择密钥文件");
                            return false;
                        }

                        // 密钥密码可以为空，如果密钥需要密码，并且用户在新建会话的时候没有输入密码，那么在打开会话的时候提示用户输入密码

                        break;
                    }

                default:
                    throw new NotImplementedException();
            }

            session.SetOption<string>(OptionKeyEnum.SSH_ADDR, this.SSHServerAddress);
            session.SetOption<int>(OptionKeyEnum.SSH_PORT, port);
            session.SetOption<string>(OptionKeyEnum.SSH_USER_NAME, this.SSHUserName);
            session.SetOption<string>(OptionKeyEnum.SSH_PASSWORD, this.SSHPassword);
            session.SetOption<string>(OptionKeyEnum.SSH_PRIVATE_KEY_FILE, this.SSHPrivateKeyId);
            session.SetOption<string>(OptionKeyEnum.SSH_Passphrase, this.SSHPassphrase);
            session.SetOption<int>(OptionKeyEnum.SSH_AUTH_TYPE, (int)authType);
            session.SetOption<List<PortForward>>(OptionKeyEnum.SSH_PORT_FORWARDS, this.PortForwards.ToList());

            return true;
        }

        private bool GetSerialPortOptions(XTermSession session)
        {
            string portName = this.SelectedPort;
            if (string.IsNullOrEmpty(portName))
            {
                MessageBoxUtils.Info("请输入正确的端口号");
                return false;
            }

            int baudRate;
            string baudRateText = this.SelectedBaudRate;
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

        private bool GetTerminalThemeOptions(XTermSession session)
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

            session.SetOption<string>(OptionKeyEnum.THEME_ID, this.ThemeList.SelectedItem.ID);
            session.SetOption<string>(OptionKeyEnum.THEME_FONT_FAMILY, this.FontFamilyList.SelectedItem.Value);
            session.SetOption<int>(OptionKeyEnum.THEME_FONT_SIZE, this.FontSizeList.SelectedItem.Value);

            // 壁纸配置
            session.SetOption<WallpaperTypeEnum>(OptionKeyEnum.THEME_BACKGROUND_TYPE, WallpaperTypeEnum.Color);
            session.SetOption<string>(OptionKeyEnum.THEME_BACKGROUND_URI, DrawingUtils.GetRgbKey(this.BackColor));
            session.SetOption<string>(OptionKeyEnum.THEME_BACKGROUND_COLOR, DrawingUtils.GetRgbKey(this.BackColor));
            //session.SetOption<EffectTypeEnum>(OptionKeyEnum.THEME_BACKGROUND_EFFECT, EffectTypeEnum.None);

            session.SetOption<string>(OptionKeyEnum.THEME_FONT_COLOR, DrawingUtils.GetRgbKey(this.FontColor));
            session.SetOption<int>(OptionKeyEnum.THEME_CURSOR_STYLE, (int)this.CursorStyles.SelectedItem);
            session.SetOption<int>(OptionKeyEnum.THEME_CURSOR_SPEED, (int)this.CursorSpeeds.SelectedItem);
            session.SetOption<string>(OptionKeyEnum.THEME_CURSOR_COLOR, DrawingUtils.GetRgbKey(this.CursorColor));
            session.SetOption<VTColorTable>(OptionKeyEnum.TEHEM_COLOR_TABLE, this.ThemeList.SelectedItem.ColorTable);
            session.SetOption<string>(OptionKeyEnum.THEME_FIND_HIGHLIGHT_BACKCOLOR, DrawingUtils.GetRgbKey(this.HighlightBackColor));
            session.SetOption<string>(OptionKeyEnum.THEME_FIND_HIGHLIGHT_FONTCOLOR, DrawingUtils.GetRgbKey(this.highlightFontColor));

            session.SetOption<string>(OptionKeyEnum.THEME_BOOKMARK_COLOR, this.ThemeList.SelectedItem.BookmarkColor);

            session.SetOption<string>(OptionKeyEnum.THEME_SELECTION_COLOR, DrawingUtils.GetRgbKey(this.SelectionColor));

            return true;
        }

        private bool GetTerminalOptions(XTermSession session)
        {
            int row, column;
            if (!int.TryParse(this.TerminalRows, out row))
            {
                MTMessageBox.Info("请输入正确的终端行数");
                return false;
            }

            if (!int.TryParse(this.TerminalColumns, out column))
            {
                MTMessageBox.Info("请输入正确的终端列数");
                return false;
            }

            int scrollback;
            if (!int.TryParse(this.MaxScrollback, out scrollback))
            {
                MTMessageBox.Info("请输入正确的回滚行数");
                return false;
            }

            int maxCliboardHistory;
            if (!int.TryParse(this.MaxClipboardHistory, out maxCliboardHistory))
            {
                MTMessageBox.Info("请输入正确的剪贴板历史记录数");
                return false;
            }

            TerminalTypeEnum terminalType = this.TerminalTypeList.SelectedItem;

            session.SetOption<int>(OptionKeyEnum.SSH_TERM_ROW, row);
            session.SetOption<int>(OptionKeyEnum.SSH_TERM_COL, column);
            session.SetOption<string>(OptionKeyEnum.SSH_TERM_TYPE, this.GetTerminalName(terminalType));
            session.SetOption<TerminalSizeModeEnum>(OptionKeyEnum.SSH_TERM_SIZE_MODE, TerminalSizeModeEnum.AutoFit);
            session.SetOption<string>(OptionKeyEnum.SSH_WRITE_ENCODING, MTermConsts.DefaultWriteEncoding);
            session.SetOption<int>(OptionKeyEnum.SSH_READ_BUFFER_SIZE, MTermConsts.DefaultReadBufferSize);
            session.SetOption<int>(OptionKeyEnum.TERM_MAX_ROLLBACK, scrollback);
            session.SetOption<int>(OptionKeyEnum.TERM_MAX_CLIPBOARD_HISTORY, maxCliboardHistory);
            session.SetOption<double>(OptionKeyEnum.SSH_THEME_DOCUMENT_PADDING, MTermConsts.DefaultContentMargin);
            session.SetOption<bool>(OptionKeyEnum.SSH_BOOKMARK_VISIBLE, this.BookmarkVisible);
            session.SetOption<bool>(OptionKeyEnum.TERM_DISABLE_BELL, this.DisableBell);

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

        private bool GetTerminalBehaviorOptions(XTermSession session)
        {
            session.SetOption<BehaviorRightClicks>(OptionKeyEnum.BEHAVIOR_RIGHT_CLICK, this.BehaviorRightClicks.SelectedItem);

            return true;
        }

        private bool GetCommandlineOptions(XTermSession session)
        {
            if (string.IsNullOrEmpty(this.StartupPath))
            {
                MessageBoxUtils.Info("请选择命令行程序");
                return false;
            }

            if (!File.Exists(this.StartupPath))
            {
                MessageBoxUtils.Info("命令行程序不存在, 请重新选择");
                return false;
            }

            session.SetOption<string>(OptionKeyEnum.CMD_STARTUP_PATH, this.StartupPath);
            session.SetOption<string>(OptionKeyEnum.CMD_STARTUP_ARGUMENT, this.StartupArgument);
            session.SetOption<string>(OptionKeyEnum.CMD_STARTUP_DIR, this.StartupDirectory);
            session.SetOption<CmdDriverEnum>(OptionKeyEnum.CMD_DRIVER, this.CmdDrivers.SelectedItem);

            return true;
        }

        private bool GetTcpOptions(XTermSession session)
        {
            if (this.RawTcpTypes.SelectedItem == RawTcpTypeEnum.Client)
            {
                IPAddress ipaddr;
                if (!IPAddress.TryParse(this.RawTcpAddress, out ipaddr))
                {
                    MTMessageBox.Info("请输入正确的IP地址");
                    return false;
                }
            }

            if (!MTermUtils.IsValidNetworkPort(this.RawTcpPort))
            {
                MTMessageBox.Info("请输入正确的端口号");
                return false;
            }

            session.SetOption<RawTcpTypeEnum>(OptionKeyEnum.RAW_TCP_TYPE, this.RawTcpTypes.SelectedItem);
            session.SetOption<string>(OptionKeyEnum.RAW_TCP_ADDRESS, this.RawTcpAddress);
            session.SetOption<int>(OptionKeyEnum.RAW_TCP_PORT, this.RawTcpPort);

            return true;
        }

        private bool GetAdbShellOptions(XTermSession session) 
        {
            int timeout;
            if (!int.TryParse(this.AdbLoginTimeout, out timeout))
            {
                MTMessageBox.Info("请输入正确的超时时间");
                return false;
            }

            session.SetOption<string>(OptionKeyEnum.ADBSH_ADB_PATH, this.AdbPath);
            session.SetOption<AdbLoginTypeEnum>(OptionKeyEnum.ADBSH_LOGIN_TYPE, this.AdbLoginTypes.SelectedItem);
            session.SetOption<string>(OptionKeyEnum.ADBSH_USERNAME, this.AdbUserName);
            session.SetOption<string>(OptionKeyEnum.ADBSH_USERNAME_PROMPT, this.AdbUserNamePrompt);
            session.SetOption<string>(OptionKeyEnum.ADBSH_PASSWORD, this.AdbPassword);
            session.SetOption<string>(OptionKeyEnum.ADBSH_PASSWORD_PROMPT, this.AdbPasswordPrompt);
            session.SetOption<int>(OptionKeyEnum.ADBSH_LOGIN_TIMEOUT, timeout);
            session.SetOption<string>(OptionKeyEnum.ADBSH_SH_PROMPT, this.AdbShellPrompt);
            session.SetOption<int>(OptionKeyEnum.ADBSH_START_SVR_TIMEOUT, MTermConsts.DefaultAdbStartServerTimeout);

            return true;
        }

        private bool GetSystemWatchOptions(XTermSession session)
        {
            session.SetOption<WatchFrequencyEnum>(OptionKeyEnum.WATCH_FREQUENCY, this.WatchFrequencies.SelectedItem);
            return true;
        }


        private void SwitchTheme(ThemePackage theme)
        {
            // 加载系统已安装的所有字体
            this.FontFamilyList.Clear();
            InstalledFontCollection installedFont = new InstalledFontCollection();
            foreach (FontFamilyDefinition ffd in this.terminalManifest.FontFamilyList)
            {
                if (installedFont.Families.FirstOrDefault(v => v.Name == ffd.Value) != null)
                {
                    this.FontFamilyList.Add(ffd);
                }
            }
            // 如果所有的预定义字体都不存在于当前系统里安装的字体，那么把当前系统里的所有字体加进去
            if (this.FontFamilyList.Count == 0)
            {
                this.FontFamilyList.AddRange(installedFont.Families.Select(v => new FontFamilyDefinition() { Name = v.Name, Value = v.Name }));
            }
            this.FontFamilyList.SelectedItem = this.FontFamilyList.FirstOrDefault();

            this.FontSizeList.SelectedItem = this.FontSizeList.FirstOrDefault();

            this.CursorSpeeds.SelectedItem = MTermConsts.DefaultCursorBlinkSpeed;

            this.CursorStyles.SelectedItem = MTermConsts.DefaultCursorStyle;

            this.FontColor = DrawingUtils.GetColor(theme.FontColor);
            this.BackColor = DrawingUtils.GetColor(theme.BackColor);
            this.CursorColor = DrawingUtils.GetColor(theme.CursorColor);

            this.CursorColor = DrawingUtils.GetColor(theme.CursorColor);
            this.FontColor = DrawingUtils.GetColor(theme.FontColor);
            this.BackColor = DrawingUtils.GetColor(theme.BackColor);

            this.HighlightBackColor = DrawingUtils.GetColor(theme.HighlightBackColor);
            this.HighlightFontColor = DrawingUtils.GetColor(theme.HighlightFontColor);

            this.SelectionColor = DrawingUtils.GetColor(theme.SelectionColor);
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
                case SessionTypeEnum.SFTP:
                    {
                        if (!this.GetSFTPOptions(session))
                        {
                            return false;
                        }
                        return true;
                    }

                case SessionTypeEnum.SSH:
                    {
                        if (!this.GetSSHOptions(session))
                        {
                            return false;
                        }
                        break;
                    }

                case SessionTypeEnum.Localhost:
                    {
                        if (!this.GetCommandlineOptions(session))
                        {
                            return false;
                        }
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

                case SessionTypeEnum.Tcp:
                    {
                        if (!this.GetTcpOptions(session))
                        {
                            return false;
                        }
                        break;
                    }

                case SessionTypeEnum.AdbShell:
                    {
                        if (!this.GetAdbShellOptions(session))
                        {
                            return false;
                        }
                        break;
                    }

                default:
                    throw new NotImplementedException();
            }

            if (!this.GetTerminalThemeOptions(session) ||
                !this.GetTerminalOptions(session) ||
                !this.GetMouseOptions(session) ||
                !this.GetTerminalBehaviorOptions(session))
            {
                return false;
            }

            return true;
        }

        #endregion

        #region 事件处理器

        private void ThemeList_SelectionChanged(ThemePackage oldTheme, ThemePackage newTheme)
        {
            if (newTheme == null)
            {
                return;
            }

            this.SwitchTheme(newTheme);
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

            string groupId = string.Empty;
            if (this.SessionGroups.Context.SelectedItem != null &&
                this.SessionGroups.Context.SelectedItem.Data != MTermConsts.RootGroup)
            {
                groupId = SessionGroups.Context.SelectedItem.ID.ToString();
            }

            XTermSession session = new XTermSession()
            {
                ID = Guid.NewGuid().ToString(),
                CreationTime = DateTime.Now,
                Name = this.Name,
                Type = (int)sessionType.Type,
                GroupId = groupId
            };

            List<OptionContentVM> contentVMs = this.optionTreeVM.Context.AllItems.Where(v => v.IsVisible).Select(v => v.ContentVM).OfType<OptionContentVM>().ToList();
            foreach (OptionContentVM contentVM in contentVMs)
            {
                if (!contentVM.SaveOptions(session))
                {
                    return null;
                }
            }

            if (!this.CollectOptions(session))
            {
                return null;
            }

            return session;
        }
    }
}

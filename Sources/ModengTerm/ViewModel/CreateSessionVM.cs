using log4net.Appender;
using ModengTerm.Addon;
using ModengTerm.Addon.Controls;
using ModengTerm.Base;
using ModengTerm.Base.DataModels;
using ModengTerm.Base.DataModels.Ssh;
using ModengTerm.Base.Definitions;
using ModengTerm.Base.Enumerations;
using ModengTerm.Base.Enumerations.Ssh;
using ModengTerm.Base.Enumerations.Terminal;
using ModengTerm.Base.Metadatas;
using ModengTerm.ViewModel.CreateSession;
using ModengTerm.ViewModel.Session;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Net;
using WPFToolkit.MVVM;
using WPFToolkit.Utility;

namespace ModengTerm.ViewModel
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
        private PreferenceTreeVM optionTreeVM;

        private string mouseScrollDelta;

        private string sftpServerInitialDir;
        private string sftpClientInitialDir;

        private bool bookmarkVisible;

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
        private MenuItemVM selectedMenuNode;

        /// <summary>
        /// 当前选择的SessionType -> SessionType要显示的首选项界面列表
        /// </summary>
        private Dictionary<SessionTypeEnum, List<PreferenceNodeVM>> sessionType2Preference;

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
                    if (this.selectedSessionType != null)
                    {
                        #region 隐藏之前选择的会话的首选项页面

                        List<PreferenceNodeVM> preferenceItems;
                        if (this.sessionType2Preference.TryGetValue(this.selectedSessionType.Type, out preferenceItems))
                        {
                            foreach (PreferenceNodeVM preferenceItem in preferenceItems)
                            {
                                preferenceItem.IsVisible = false;
                            }
                        }

                        #endregion
                    }

                    if (value != null)
                    {
                        #region 显示选择的会话的首选项页面

                        List<PreferenceNodeVM> preferenceItems;
                        if (this.sessionType2Preference.TryGetValue(value.Type, out preferenceItems))
                        {
                            foreach (PreferenceNodeVM preferenceItem in preferenceItems)
                            {
                                preferenceItem.IsVisible = true;
                            }
                        }

                        #endregion

                        // 选中第一个有界面的节点
                        PreferenceNodeVM node = preferenceItems.FirstOrDefault(v => !string.IsNullOrEmpty(v.ClassName));
                        if (node != null)
                        {
                            node.IsSelected = true;
                        }
                    }

                    this.selectedSessionType = value;
                    this.NotifyPropertyChanged("SelectedSessionType");
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
            get { return sshPort; }
            set
            {
                if (sshPort != value)
                {
                    sshPort = value;
                    this.NotifyPropertyChanged("SSHServerPort");
                }
            }
        }

        public string SSHServerAddress
        {
            get { return sshAddress; }
            set
            {
                if (sshAddress != value)
                {
                    sshAddress = value;
                    this.NotifyPropertyChanged("SSHServerAddress");
                }
            }
        }

        public string SSHUserName
        {
            get { return sshUserName; }
            set
            {
                if (sshUserName != value)
                {
                    sshUserName = value;
                    this.NotifyPropertyChanged("SSHUserName");
                }
            }
        }

        public string SSHPassword
        {
            get { return sshPassword; }
            set
            {
                if (sshPassword != value)
                {
                    sshPassword = value;
                    this.NotifyPropertyChanged("SSHPassword");
                }
            }
        }

        public string SSHPassphrase
        {
            get { return sshPassphrase; }
            set
            {
                if (sshPassphrase != value)
                {
                    sshPassphrase = value;
                    this.NotifyPropertyChanged("SSHPassphrase");
                }
            }
        }

        public string SSHPrivateKeyId
        {
            get { return sshPrivateKeyId; }
            set
            {
                if (sshPrivateKeyId != value)
                {
                    sshPrivateKeyId = value;
                    this.NotifyPropertyChanged("SSHPrivateKeyFile");
                }
            }
        }

        /// <summary>
        /// 终端行数
        /// </summary>
        public string TerminalRows
        {
            get { return terminalRows; }
            set
            {
                if (terminalRows != value)
                {
                    terminalRows = value;
                    this.NotifyPropertyChanged("TerminalRows");
                }
            }
        }

        /// <summary>
        /// 终端列数
        /// </summary>
        public string TerminalColumns
        {
            get { return terminalColumns; }
            set
            {
                if (terminalColumns != value)
                {
                    terminalColumns = value;
                    this.NotifyPropertyChanged("TerminalColumns");
                }
            }
        }

        public string MaxScrollback
        {
            get { return maxScrollback; }
            set
            {
                if (maxScrollback != value)
                {
                    maxScrollback = value;
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
                return maxCliboardHistory;
            }
            set
            {
                if (maxCliboardHistory != value)
                {
                    maxCliboardHistory = value;
                    this.NotifyPropertyChanged("MaxClipboardHistory");
                }
            }
        }

        /// <summary>
        /// 当前显示的配置树形列表ViewModel
        /// </summary>
        public PreferenceTreeVM PreferenceTreeVM
        {
            get { return optionTreeVM; }
            private set
            {
                if (optionTreeVM != value)
                {
                    optionTreeVM = value;
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
            get { return mouseScrollDelta; }
            set
            {
                if (mouseScrollDelta != value)
                {
                    mouseScrollDelta = value;
                    this.NotifyPropertyChanged("MouseScrollDelta");
                }
            }
        }

        /// <summary>
        /// SFTP初始目录
        /// </summary>
        public string SFTPServerInitialDirectory
        {
            get { return sftpServerInitialDir; }
            set
            {
                if (sftpServerInitialDir != value)
                {
                    sftpServerInitialDir = value;
                    this.NotifyPropertyChanged("SFTPInitialDirectory");
                }
            }
        }

        /// <summary>
        /// SFTP客户端初始化目录
        /// </summary>
        public string SFTPClientInitialDirectory
        {
            get { return sftpClientInitialDir; }
            set
            {
                if (sftpClientInitialDir != value)
                {
                    sftpClientInitialDir = value;
                    this.NotifyPropertyChanged("SFTPClientInitialDirectory");
                }
            }
        }

        #region 命令行

        public string StartupPath
        {
            get { return startupPath; }
            set
            {
                if (startupPath != value)
                {
                    startupPath = value;
                    this.NotifyPropertyChanged("StartupPath");
                }
            }
        }

        public string StartupArgument
        {
            get { return startupArgument; }
            set
            {
                if (startupArgument != value)
                {
                    startupArgument = value;
                    this.NotifyPropertyChanged("StartupArgument");
                }
            }
        }

        public string StartupDirectory
        {
            get { return startupDir; }
            set
            {
                if (startupDir != value)
                {
                    startupDir = value;
                    this.NotifyPropertyChanged("StartupDirectory");
                }
            }
        }

        public BindableCollection<Win32ConsoleEngineEnum> CmdDrivers { get; private set; }

        #endregion

        #region 终端

        public bool DisableBell
        {
            get { return disableBell; }
            set
            {
                if (disableBell != value)
                {
                    disableBell = value;
                    this.NotifyPropertyChanged("DisableBell");
                }
            }
        }

        #endregion

        #region 终端 - 行为

        public BindableCollection<RightClickActions> BehaviorRightClicks { get; private set; }

        #endregion

        #region 主题相关

        public string ScrollbarThumbColor { get; set; }
        public string ScrollbarButtonColor { get; set; }
        public string ScrollbarTrackColor { get; set; }

        /// <summary>
        /// 是否显示书签
        /// </summary>
        public bool BookmarkVisible
        {
            get { return bookmarkVisible; }
            set
            {
                if (bookmarkVisible != value)
                {
                    bookmarkVisible = value;
                    this.NotifyPropertyChanged("BookmarkVisible");
                }
            }
        }

        #endregion

        #region Tcp

        public BindableCollection<RawTcpTypeEnum> RawTcpTypes { get; private set; }

        public string RawTcpAddress
        {
            get { return rawTcpAddress; }
            set
            {
                if (rawTcpAddress != value)
                {
                    rawTcpAddress = value;
                    this.NotifyPropertyChanged("RawTcpAddress");
                }
            }
        }

        public int RawTcpPort
        {
            get { return rawTcpPort; }
            set
            {
                if (rawTcpPort != value)
                {
                    rawTcpPort = value;
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

        #endregion

        #region 构造方法

        public CreateSessionVM()
        {
            this.Name = "新建会话";
            this.sessionType2Preference = new Dictionary<SessionTypeEnum, List<PreferenceNodeVM>>();

            #region 加载参数树形列表

            // 加载参数树形列表
            this.PreferenceTreeVM = new PreferenceTreeVM();
            this.InitializePreferenceTree();
            this.PreferenceTreeVM.ExpandAll();

            #endregion

            #region 会话类型和会话分组

            this.SessionTypeList = new BindableCollection<SessionTypeVM>();
            foreach (SessionMetadata session in VTBaseConsts.SessionMetadatas)
            {
                this.SessionTypeList.Add(new SessionTypeVM(session));
            }
            // 选择第一个会话
            this.SelectedSessionType = this.SessionTypeList.FirstOrDefault();

            this.SessionGroups = VMUtils.CreateSessionTreeVM(true, true);
            this.SessionGroups.ExpandAll();

            #endregion

            #region 命令行

            StartupPath = Path.Combine(Environment.SystemDirectory, "cmd.exe");
            StartupDirectory = AppDomain.CurrentDomain.BaseDirectory;
            CmdDrivers = new BindableCollection<Win32ConsoleEngineEnum>();
            CmdDrivers.AddRange(VTBaseUtils.GetEnumValues<Win32ConsoleEngineEnum>());
            // 如果是Win10或更高版本那么默认选择PseudoConsoleApi
            if (VTBaseUtils.IsWin10())
            {
                CmdDrivers.SelectedItem = Win32ConsoleEngineEnum.Win10PseudoConsoleApi;
            }
            else
            {
                CmdDrivers.SelectedItem = Win32ConsoleEngineEnum.winpty;
            }

            #endregion

            #region 终端

            #region 终端

            TerminalRows = VTBaseConsts.TerminalRows.ToString();
            TerminalColumns = VTBaseConsts.TerminalColumns.ToString();
            TerminalTypeList = new BindableCollection<TerminalTypeEnum>();
            TerminalTypeList.AddRange(VTBaseUtils.GetEnumValues<TerminalTypeEnum>());
            TerminalTypeList.SelectedItem = VTBaseConsts.DefaultTerminalType;
            MaxScrollback = VTBaseConsts.DefaultTerminalScrollback.ToString();
            MaxClipboardHistory = VTBaseConsts.DefaultMaxClipboardHistory.ToString();

            #endregion

            #region 行为

            BehaviorRightClicks = new BindableCollection<RightClickActions>();
            BehaviorRightClicks.AddRange(VTBaseUtils.GetEnumValues<RightClickActions>());
            BehaviorRightClicks.SelectedItem = RightClickActions.ContextMenu;

            #endregion

            #endregion

            #region 端口转发

            PortForwards = new BindableCollection<PortForward>();

            #endregion

            #region SSH

            SSHAuthTypeList = new BindableCollection<SSHAuthTypeEnum>();
            SSHAuthTypeList.AddRange(VTBaseUtils.GetEnumValues<SSHAuthTypeEnum>());
            SSHAuthTypeList.SelectedItem = SSHAuthTypeList.FirstOrDefault();
            SSHServerPort = VTBaseConsts.DefaultSSHPort.ToString();

            #endregion

            #region Tcp

            RawTcpTypes = new BindableCollection<RawTcpTypeEnum>();
            RawTcpTypes.AddRange(VTBaseUtils.GetEnumValues<RawTcpTypeEnum>());

            #endregion

            MouseScrollDelta = VTBaseConsts.DefaultScrollDelta.ToString();

            SFTPServerInitialDirectory = VTBaseConsts.SFTPServerInitialDirectory;
            SFTPClientInitialDirectory = VTBaseConsts.SFTPClientInitialDirectory;
        }

        #endregion

        #region 实例方法

        private void InitializePreferenceTree()
        {
            foreach (AddonMetadata metadata in ClientContext.Context.Manifest.Addons)
            {
                if (metadata.Preferences.Count == 0)
                {
                    continue;
                }

                this.LoadChildPreference(metadata, null, metadata.Preferences);
            }
        }

        private void LoadChildPreference(AddonMetadata addonMetadata, PreferenceNodeVM parent, List<PreferenceMetadata> children)
        {
            foreach (PreferenceMetadata metadata in children)
            {
                PreferenceNodeVM pivm = new PreferenceNodeVM(this.PreferenceTreeVM.Context);
                pivm.ID = metadata.ID;
                pivm.Name = metadata.Name;
                pivm.ClassName = metadata.ClassName;
                pivm.AddonMetadata = addonMetadata;
                pivm.IsVisible = false;
                pivm.Level = parent == null ? 0 : parent.Level + 1;

                // 加载树形列表的时候就把配置项里的变量解析掉，这样就不用每次保存的时候再解析一遍了
                foreach (KeyValuePair<string, object> kv in metadata.DefaultOptions)
                {
                    if (kv.Value is string)
                    {
                        // 如果默认配置项是字符串类型，那么判断是否包含变量
                        pivm.DefaultOptions[kv.Key] = VTBaseUtils.ReplaceVariable(kv.Value.ToString());
                    }
                    else
                    {
                        pivm.DefaultOptions[kv.Key] = kv.Value;
                    }
                }

                // 保存不同的会话所需要显示的配置项界面列表
                foreach (SessionTypeEnum scope in metadata.Scopes)
                {
                    List<PreferenceNodeVM> pivms;
                    if (!this.sessionType2Preference.TryGetValue(scope, out pivms))
                    {
                        pivms = new List<PreferenceNodeVM>();
                        this.sessionType2Preference[scope] = pivms;
                    }

                    pivms.Add(pivm);
                }

                // 加载子级配置项
                this.LoadChildPreference(addonMetadata, pivm, metadata.Children);

                // 配置项添加到树里
                if (parent == null)
                {
                    this.PreferenceTreeVM.AddRootNode(pivm);
                }
                else
                {
                    parent.Add(pivm);
                }
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

        private bool GetSSHOptions(XTermSession session)
        {
            if (string.IsNullOrEmpty(SSHServerAddress))
            {
                MessageBoxUtils.Info("请输入主机名称");
                return false;
            }

            int port;
            if (!int.TryParse(sshPort, out port) ||
                port < VTBaseConsts.MIN_PORT || port > VTBaseConsts.MAX_PORT)
            {
                MessageBoxUtils.Info("请输入正确的端口号");
                return false;
            }

            if (string.IsNullOrEmpty(SSHUserName))
            {
                MessageBoxUtils.Info("请输入用户名");
                return false;
            }

            SSHAuthTypeEnum authType = SSHAuthTypeList.SelectedItem;
            switch (authType)
            {
                case SSHAuthTypeEnum.None:
                    {
                        break;
                    }

                case SSHAuthTypeEnum.Password:
                    {
                        if (string.IsNullOrEmpty(SSHPassword))
                        {
                            MessageBoxUtils.Info("请输入密码");
                            return false;
                        }

                        break;
                    }

                case SSHAuthTypeEnum.PrivateKey:
                    {
                        if (string.IsNullOrEmpty(SSHPrivateKeyId))
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

            //session.SetOption<string>(OptionKeyEnum.SSH_SERVER_ADDR, SSHServerAddress);
            //session.SetOption<int>(OptionKeyEnum.SSH_SERVER_PORT, port);
            //session.SetOption<string>(OptionKeyEnum.SSH_USER_NAME, SSHUserName);
            //session.SetOption<string>(OptionKeyEnum.SSH_PASSWORD, SSHPassword);
            //session.SetOption<string>(OptionKeyEnum.SSH_PRIVATE_KEY_FILE, SSHPrivateKeyId);
            //session.SetOption<string>(OptionKeyEnum.SSH_Passphrase, SSHPassphrase);
            //session.SetOption<int>(OptionKeyEnum.SSH_AUTH_TYPE, (int)authType);
            //session.SetOption<List<PortForward>>(OptionKeyEnum.SSH_PORT_FORWARDS, PortForwards.ToList());

            return true;
        }

        private bool GetTerminalOptions(XTermSession session)
        {
            int row, column;
            if (!int.TryParse(TerminalRows, out row))
            {
                MTMessageBox.Info("请输入正确的终端行数");
                return false;
            }

            if (!int.TryParse(TerminalColumns, out column))
            {
                MTMessageBox.Info("请输入正确的终端列数");
                return false;
            }

            int scrollback;
            if (!int.TryParse(MaxScrollback, out scrollback))
            {
                MTMessageBox.Info("请输入正确的回滚行数");
                return false;
            }

            int maxCliboardHistory;
            if (!int.TryParse(MaxClipboardHistory, out maxCliboardHistory))
            {
                MTMessageBox.Info("请输入正确的剪贴板历史记录数");
                return false;
            }

            TerminalTypeEnum terminalType = TerminalTypeList.SelectedItem;

            //session.SetOption<int>(OptionKeyEnum.SSH_TERM_ROW, row);
            //session.SetOption<int>(OptionKeyEnum.SSH_TERM_COL, column);
            //session.SetOption<string>(OptionKeyEnum.SSH_TERM_TYPE, GetTerminalName(terminalType));
            //session.SetOption<TerminalSizeModeEnum>(OptionKeyEnum.SSH_TERM_SIZE_MODE, TerminalSizeModeEnum.AutoFit);
            //session.SetOption<string>(OptionKeyEnum.TERM_WRITE_ENCODING, VTBaseConsts.DefaultWriteEncoding);
            //session.SetOption<int>(OptionKeyEnum.SSH_READ_BUFFER_SIZE, VTBaseConsts.DefaultReadBufferSize);
            //session.SetOption<int>(OptionKeyEnum.TERM_MAX_ROLLBACK, scrollback);
            //session.SetOption<int>(OptionKeyEnum.TERM_MAX_CLIPBOARD_HISTORY, maxCliboardHistory);
            //session.SetOption<double>(OptionKeyEnum.SSH_THEME_DOCUMENT_PADDING, VTBaseConsts.DefaultContentMargin);
            ////session.SetOption<bool>(OptionKeyEnum.SSH_BOOKMARK_VISIBLE, BookmarkVisible);
            //session.SetOption<bool>(OptionKeyEnum.TERM_DISABLE_BELL, DisableBell);

            return true;
        }

        private bool GetMouseOptions(XTermSession session)
        {
            int scrollDelta;
            if (!int.TryParse(MouseScrollDelta, out scrollDelta))
            {
                scrollDelta = VTBaseConsts.DefaultScrollDelta;
            }

            //session.SetOption<int>(OptionKeyEnum.MOUSE_SCROLL_DELTA, scrollDelta);

            return true;
        }

        private bool GetSFTPOptions(XTermSession session)
        {
            string serverInitialDir = SFTPServerInitialDirectory;
            if (string.IsNullOrEmpty(serverInitialDir))
            {
                serverInitialDir = VTBaseConsts.SFTPServerInitialDirectory;
            }

            string clientInitialDir = SFTPClientInitialDirectory;
            if (string.IsNullOrEmpty(clientInitialDir) ||
                !Directory.Exists(clientInitialDir))
            {
                clientInitialDir = VTBaseConsts.SFTPClientInitialDirectory;
            }

            //session.SetOption<string>(OptionKeyEnum.SFTP_SERVER_INITIAL_DIRECTORY, serverInitialDir);
            //session.SetOption<string>(OptionKeyEnum.SFTP_CLIENT_INITIAL_DIRECTORY, clientInitialDir);

            return true;
        }

        private bool GetTerminalBehaviorOptions(XTermSession session)
        {
            //session.SetOption<RightClickActions>(OptionKeyEnum.BEHAVIOR_RIGHT_CLICK, BehaviorRightClicks.SelectedItem);

            return true;
        }

        private bool GetCommandlineOptions(XTermSession session)
        {
            if (string.IsNullOrEmpty(StartupPath))
            {
                MessageBoxUtils.Info("请选择命令行程序");
                return false;
            }

            if (!File.Exists(StartupPath))
            {
                MessageBoxUtils.Info("命令行程序不存在, 请重新选择");
                return false;
            }

            //session.SetOption<string>(OptionKeyEnum.CMD_STARTUP_PATH, StartupPath);
            //session.SetOption<string>(OptionKeyEnum.CMD_STARTUP_ARGUMENT, StartupArgument);
            //session.SetOption<string>(OptionKeyEnum.CMD_STARTUP_DIR, StartupDirectory);
            //session.SetOption<Win32ConsoleEngineEnum>(OptionKeyEnum.CMD_CONSOLE_ENGINE, CmdDrivers.SelectedItem);

            return true;
        }

        private bool GetTcpOptions(XTermSession session)
        {
            if (RawTcpTypes.SelectedItem == RawTcpTypeEnum.Client)
            {
                IPAddress ipaddr;
                if (!IPAddress.TryParse(RawTcpAddress, out ipaddr))
                {
                    MTMessageBox.Info("请输入正确的IP地址");
                    return false;
                }
            }

            if (!VTBaseUtils.IsValidNetworkPort(RawTcpPort))
            {
                MTMessageBox.Info("请输入正确的端口号");
                return false;
            }

            //session.SetOption<RawTcpTypeEnum>(OptionKeyEnum.RAW_TCP_TYPE, RawTcpTypes.SelectedItem);
            //session.SetOption<string>(OptionKeyEnum.RAW_TCP_ADDRESS, RawTcpAddress);
            //session.SetOption<int>(OptionKeyEnum.RAW_TCP_PORT, RawTcpPort);

            return true;
        }

        private bool CollectOptions(XTermSession session)
        {
            SessionTypeVM sessionType = SelectedSessionType;
            if (sessionType == null)
            {
                MessageBoxUtils.Info("请选择会话类型");
                return false;
            }

            switch (sessionType.Type)
            {
                case SessionTypeEnum.Sftp:
                    {
                        if (!GetSFTPOptions(session))
                        {
                            return false;
                        }
                        return true;
                    }

                case SessionTypeEnum.Ssh:
                    {
                        if (!GetSSHOptions(session))
                        {
                            return false;
                        }
                        break;
                    }

                case SessionTypeEnum.LocalConsole:
                    {
                        if (!GetCommandlineOptions(session))
                        {
                            return false;
                        }
                        break;
                    }

                case SessionTypeEnum.Tcp:
                    {
                        if (!GetTcpOptions(session))
                        {
                            return false;
                        }
                        break;
                    }

                default:
                    throw new NotImplementedException();
            }

            if (!GetTerminalOptions(session) ||
                !GetMouseOptions(session) ||
                !GetTerminalBehaviorOptions(session))
            {
                return false;
            }

            return true;
        }

        private bool SaveAllOptions(XTermSession session)
        {
            List<PreferenceNodeVM> preferenceNodes = this.sessionType2Preference[this.selectedSessionType.Type];

            foreach (PreferenceNodeVM pNode in preferenceNodes)
            {
                Dictionary<string, object> sessionOptions;
                if (!session.Options.TryGetValue(pNode.AddonMetadata.ID, out sessionOptions))
                {
                    sessionOptions = new Dictionary<string, object>();
                    session.Options[pNode.AddonMetadata.ID] = sessionOptions;
                }

                IPreferencePanel preferencePanel = pNode.FrameworkElement as IPreferencePanel;
                if (preferencePanel == null)
                {
                    // 如果没有打开过配置项界面，或者配置项界面没有实现IPreferencePanel，那么保存默认配置项
                    foreach (KeyValuePair<string, object> kv in pNode.DefaultOptions)
                    {
                        sessionOptions[kv.Key] = kv.Value;
                    }
                    continue;
                }

                Dictionary<string, object> options = preferencePanel.GetOptions();
                if (options == null)
                {
                    return false;
                }

                foreach (KeyValuePair<string, object> kv in options)
                {
                    sessionOptions[kv.Key] = kv.Value;
                }
            }

            return true;
        }

        #endregion

        #region 事件处理器

        #endregion

        public XTermSession GetSession()
        {
            if (string.IsNullOrEmpty(this.Name))
            {
                MTMessageBox.Info("请输入会话名称");
                return null;
            }

            SessionTypeVM sessionType = SelectedSessionType;
            if (sessionType == null)
            {
                MTMessageBox.Info("请选择会话类型");
                return null;
            }

            string groupId = string.Empty;
            if (SessionGroups.Context.SelectedItem != null &&
                SessionGroups.Context.SelectedItem.Data != VTBaseConsts.RootGroup)
            {
                groupId = SessionGroups.Context.SelectedItem.ID.ToString();
            }

            XTermSession session = new XTermSession();
            session.ID = Guid.NewGuid().ToString();
            session.CreationTime = DateTime.Now;
            session.Name = this.Name;
            session.Type = (int)sessionType.Type;
            session.GroupId = groupId;

            if (!SaveAllOptions(session))
            {
                return null;
            }

            if (!CollectOptions(session))
            {
                return null;
            }

            return session;
        }
    }
}

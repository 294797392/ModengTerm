using ModengTerm.Addon;
using ModengTerm.Addon.Interactive;
using ModengTerm.Base;
using ModengTerm.Base.DataModels;
using ModengTerm.Base.DataModels.Ssh;
using ModengTerm.Base.Definitions;
using ModengTerm.Base.Enumerations;
using ModengTerm.Base.Enumerations.Ssh;
using ModengTerm.Base.Enumerations.Terminal;
using ModengTerm.Document;
using ModengTerm.Document.Enumerations;
using ModengTerm.Document.Graphics;
using ModengTerm.Document.Utility;
using ModengTerm.Terminal;
using ModengTerm.Terminal.DataModels;
using ModengTerm.Terminal.Engines;
using ModengTerm.Terminal.Keyboard;
using ModengTerm.Terminal.Modem;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using WPFToolkit.MVVM;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

namespace ModengTerm.ViewModel.Terminal
{
    public class ShellSessionVM : OpenedSessionVM, IClientShellTab
    {
        #region 类变量

        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger("ShellSessionVM");

        #endregion

        #region 实例变量

        private int viewportRow;
        private int viewportColumn;

        /// <summary>
        /// 与终端进行通信的信道
        /// </summary>
        private ChannelTransport channelTransport;

        /// <summary>
        /// 终端引擎
        /// </summary>
        private VideoTerminal videoTerminal;

        private Encoding writeEncoding;
        private Encoding readEncoding;

        /// <summary>
        /// 是否正在运行
        /// </summary>
        private bool isRunning;

        private string uri;

        private int totalRows;

        private Visibility contextMenuVisibility;

        private bool inputPanelVisible;

        private bool modemRunning;

        private List<ScriptItem> scriptItems;

        private TabEventShellUserInput tabEventSendUserInput;
        private TabEventShellRendered tabEventShellRendered;
        private TabEventStatusChanged tabEventStatusChanged;

        #endregion

        #region 属性

        /// <summary>
        /// 可视区域的行数
        /// </summary>
        public int ViewportRow
        {
            get { return viewportRow; }
            set
            {
                if (viewportRow != value)
                {
                    viewportRow = value;
                    NotifyPropertyChanged("ViewportRow");
                }
            }
        }

        /// <summary>
        /// 可视区域的列数
        /// </summary>
        public int ViewportColumn
        {
            get { return viewportColumn; }
            set
            {
                if (viewportColumn != value)
                {
                    viewportColumn = value;
                    NotifyPropertyChanged("ViewportColumn");
                }
            }
        }

        /// <summary>
        /// SSH主机的Uri
        /// </summary>
        public string Uri
        {
            get { return uri; }
            private set
            {
                if (uri != value)
                {
                    uri = value;
                    NotifyPropertyChanged("Uri");
                }
            }
        }

        public GraphicsFactory MainDocument { get; set; }
        public GraphicsFactory AlternateDocument { get; set; }

        public IDrawingContext DrawingContext { get; set; }

        public IVideoTerminal VideoTerminal { get { return this.videoTerminal; } }

        /// <summary>
        /// 获取或设置终端宽度，单位是像素
        /// </summary>
        public double Width { get; set; }

        /// <summary>
        /// 获取或设置终端高度，单位是像素
        /// </summary>
        public double Height { get; set; }

        /// <summary>
        /// 总行数
        /// </summary>
        public int TotalRows
        {
            get { return totalRows; }
            set
            {
                if (totalRows != value)
                {
                    totalRows = value;
                    NotifyPropertyChanged("TotalRows");
                }
            }
        }

        /// <summary>
        /// 会话的右键菜单
        /// </summary>
        public BindableCollection<ContextMenuVM> ContextMenus { get; private set; }

        /// <summary>
        /// 控制右键菜单的显示和隐藏
        /// </summary>
        public Visibility ContextMenuVisibility
        {
            get { return contextMenuVisibility; }
            set
            {
                if (contextMenuVisibility != value)
                {
                    contextMenuVisibility = value;
                    NotifyPropertyChanged("ContextMenuVisibility");
                }
            }
        }

        /// <summary>
        /// 保存该会话输入的历史记录
        /// </summary>
        public BindableCollection<string> HistoryCommands { get; private set; }

        /// <summary>
        /// 是否显示输入栏
        /// </summary>
        public bool InputPanelVisible
        {
            get { return inputPanelVisible; }
            set
            {
                if (inputPanelVisible != value)
                {
                    inputPanelVisible = value;
                    NotifyPropertyChanged("InputPanelVisible");
                }
            }
        }

        #endregion

        #region 构造方法

        public ShellSessionVM(XTermSession session) :
            base(session)
        {
            this.tabEventSendUserInput = new TabEventShellUserInput() { Sender = this };
            this.tabEventShellRendered = new TabEventShellRendered() { Sender = this };
            this.tabEventStatusChanged = new TabEventStatusChanged() { Sender = this };
        }

        #endregion

        #region OpenedSessionVM Member

        protected override void OnInitialize()
        {
        }

        protected override void OnRelease()
        {
        }

        protected override int OnOpen()
        {
            //scriptItems = Session.GetOption(OptionKeyEnum.LOGIN_SCRIPT_ITEMS, new List<ScriptItem>());
            HistoryCommands = new BindableCollection<string>();

            writeEncoding = Encoding.GetEncoding(this.session.GetOption<string>(PredefinedOptions.TERM_ENCODING));
            readEncoding = Encoding.GetEncoding(this.session.GetOption<string>(PredefinedOptions.TERM_ENCODING));

            #region 初始化上下文菜单

            ContextMenus = new BindableCollection<ContextMenuVM>();
            ContextMenus.AddRange(VMUtils.CreateContextMenuVMs(false));

            RightClickActions brc = Session.GetOption<RightClickActions>(PredefinedOptions.TERM_RIGHT_CLICK_ACTION);
            if (brc == RightClickActions.ContextMenu)
            {
                ContextMenuVisibility = Visibility.Visible;
            }
            else
            {
                ContextMenuVisibility = Visibility.Collapsed;
            }

            #endregion

            #region 初始化终端

            #region 计算终端的行数和列数

            string fontFamily = this.session.GetOption<string>(PredefinedOptions.THEME_FONT_FAMILY);
            double fontSize = this.session.GetOption<double>(PredefinedOptions.THEME_FONT_SIZE);
            VTypeface typeface = this.MainDocument.GetTypeface(fontSize, fontFamily);
            typeface.ForegroundColor = this.session.GetOption<string>(PredefinedOptions.THEME_FONT_COLOR);
            VTSize displaySize = new VTSize(this.Width, this.Height);
            TerminalSizeModeEnum sizeMode = this.session.GetOption<TerminalSizeModeEnum>(PredefinedOptions.TERM_SIZE_MODE);
            int rows = this.session.GetOption<int>(PredefinedOptions.TERM_ROW);
            int cols = this.session.GetOption<int>(PredefinedOptions.TERM_COL);
            if (sizeMode == TerminalSizeModeEnum.AutoFit)
            {
                /// 如果SizeMode等于Fixed，那么就使用DefaultViewportRow和DefaultViewportColumn
                /// 如果SizeMode等于AutoFit，那么动态计算行和列
                VTDocUtils.CalculateAutoFitSize(displaySize, typeface, out rows, out cols);
            }

            #endregion

            VTOptions options = new VTOptions()
            {
                Session = Session,
                SessionTransport = new ChannelTransport(),
                AlternateDocument = AlternateDocument,
                MainDocument = MainDocument,
                Row = rows,
                Column = cols,
                Typeface = typeface
            };

            VideoTerminal videoTerminal = new VideoTerminal();
            videoTerminal.OnViewportChanged += VideoTerminal_ViewportChanged;
            videoTerminal.DisableBell = Session.GetOption<bool>(PredefinedOptions.TERM_DISABLE_BELL);
            videoTerminal.Initialize(options);
            this.videoTerminal = videoTerminal;

            #endregion

            #region 连接终端通道

            ChannelOptions channelOptions = this.CreateChannelOptions(rows, cols);

            // 连接SSH服务器
            ChannelTransport transport = options.SessionTransport;
            transport.StatusChanged += SessionTransport_StatusChanged;
            transport.DataReceived += SessionTransport_DataReceived;
            transport.Initialize(channelOptions);
            transport.OpenAsync();

            this.channelTransport = transport;

            #endregion

            Uri = InitializeURI();

            this.isRunning = true;

            return ResponseCode.SUCCESS;
        }

        protected override void OnClose()
        {
            if (!this.isRunning)
            {
                return;
            }

            channelTransport.StatusChanged -= SessionTransport_StatusChanged;
            channelTransport.DataReceived -= SessionTransport_DataReceived;
            channelTransport.Close();
            channelTransport.Release();

            videoTerminal.OnViewportChanged -= VideoTerminal_ViewportChanged;
            videoTerminal.Release();

            isRunning = false;
        }

        #endregion

        #region 实例方法

        private ChannelOptions CreateChannelOptions(int rows, int cols)
        {
            int recvBufferSize = this.session.GetOption<int>(PredefinedOptions.TERM_READ_BUFFER_SIZE);

            switch ((SessionTypeEnum)this.session.Type)
            {
                case SessionTypeEnum.SerialPort:
                    {
                        return new SerialPortChannelOptions()
                        {
                            ReceiveBufferSize = recvBufferSize,
                            Column = cols,
                            Row = rows,
                            StopBits = this.session.GetOption<StopBits>(PredefinedOptions.SERIAL_PORT_STOP_BITS),
                            BaudRate = this.session.GetOption<int>(PredefinedOptions.SERIAL_PORT_BAUD_RATE),
                            DataBits = this.session.GetOption<int>(PredefinedOptions.SERIAL_PORT_DATA_BITS),
                            Handshake = this.session.GetOption<Handshake>(PredefinedOptions.SERIAL_PORT_HANDSHAKE),
                            Parity = this.session.GetOption<Parity>(PredefinedOptions.SERIAL_PORT_PARITY),
                            PortName = this.session.GetOption<string>(PredefinedOptions.SERIAL_PORT_NAME)
                        };
                    }

                case SessionTypeEnum.Ssh:
                    {
                        return new SshChannelOptions()
                        {
                            ReceiveBufferSize = recvBufferSize,
                            Column = cols,
                            Row = rows,
                            AuthenticationType = this.session.GetOption<SSHAuthTypeEnum>(PredefinedOptions.SSH_AUTH_TYPE),
                            UserName = this.session.GetOption<string>(PredefinedOptions.SSH_USER_NAME),
                            Password = this.session.GetOption<string>(PredefinedOptions.SSH_PASSWORD),
                            PrivateKeyId = this.session.GetOption<string>(PredefinedOptions.SSH_PRIVATE_KEY_ID),
                            Passphrase = this.session.GetOption<string>(PredefinedOptions.SSH_Passphrase),
                            ServerPort = this.session.GetOption<int>(PredefinedOptions.SSH_SERVER_PORT),
                            ServerAddress = this.session.GetOption<string>(PredefinedOptions.SSH_SERVER_ADDR),
                            TerminalType = this.session.GetOption<string>(PredefinedOptions.TERM_TYPE),
                            PortForwards = this.session.GetOption<List<PortForward>>(PredefinedOptions.SSH_PORT_FORWARDS)
                        };
                    }

                case SessionTypeEnum.LocalConsole:
                    {
                        return new LocalConsoleChannelOptions()
                        {
                            ReceiveBufferSize = recvBufferSize,
                            Column = cols,
                            Row = rows,
                            ConsoleEngin = this.session.GetOption<Win32ConsoleEngineEnum>(PredefinedOptions.CONSOLE_ENGINE),
                            StartupDir = this.session.GetOption<string>(PredefinedOptions.CONSOLE_STARTUP_DIR),
                            StartupPath = this.session.GetOption<string>(PredefinedOptions.CONSOLE_STARTUP_PATH),
                            Arguments = this.session.GetOption<string>(PredefinedOptions.CONSOLE_STARTUP_ARGUMENT)
                        };
                    }

                case SessionTypeEnum.Tcp:
                    {
                        return new TcpChannelOptions()
                        {
                            ReceiveBufferSize = recvBufferSize,
                            Column = cols,
                            Row = rows,
                            Type = this.session.GetOption<RawTcpTypeEnum>(PredefinedOptions.RAW_TCP_TYPE),
                            IPAddress = this.session.GetOption<string>(PredefinedOptions.RAW_TCP_ADDRESS),
                            Port = this.session.GetOption<int>(PredefinedOptions.RAW_TCP_PORT)
                        };
                    }

                default:
                    {
                        throw new NotImplementedException();
                    }
            }
        }

        private string InitializeURI()
        {
            string uri = string.Empty;

            switch ((SessionTypeEnum)Session.Type)
            {
                case SessionTypeEnum.Tcp:
                    {
                        string addr = this.Session.GetOption<string>(PredefinedOptions.RAW_TCP_ADDRESS);
                        string port = this.Session.GetOption<string>(PredefinedOptions.RAW_TCP_PORT);
                        uri = string.Format("tcp://{0}:{1}", addr, port);
                        break;
                    }

                case SessionTypeEnum.LocalConsole:
                    {
                        string cmdPath = Session.GetOption<string>(PredefinedOptions.CONSOLE_STARTUP_PATH);
                        uri = string.Format("{0}", cmdPath);
                        break;
                    }

                case SessionTypeEnum.Ssh:
                    {
                        string userName = Session.GetOption<string>(PredefinedOptions.SSH_USER_NAME);
                        string hostName = Session.GetOption<string>(PredefinedOptions.SSH_SERVER_ADDR);
                        int port = Session.GetOption<int>(PredefinedOptions.SSH_SERVER_PORT);
                        uri = string.Format("ssh://{0}@{1}:{2}", userName, hostName, port);
                        break;
                    }

                case SessionTypeEnum.SerialPort:
                    {
                        string portName = Session.GetOption<string>(PredefinedOptions.SERIAL_PORT_NAME);
                        int baudRate = Session.GetOption<int>(PredefinedOptions.SERIAL_PORT_BAUD_RATE);
                        uri = string.Format("serialPort://{0} {1}", portName, baudRate);
                        break;
                    }

                default:
                    throw new NotImplementedException();
            }

            return uri;
        }

        /// <summary>
        /// 执行最终的发送数据动作
        /// </summary>
        /// <param name="bytes"></param>
        private void PerformSend(byte[] bytes)
        {
            VTDebug.Context.WriteInteractive(VTSendTypeEnum.UserInput, bytes);

            this.videoTerminal.ProcessWrite(bytes);
        }

        private void HandleScript()
        {
            return;

            if (scriptItems.Count == 0)
            {
                return;
            }

            VTDocument document = videoTerminal.ActiveDocument;
            int cursorPhysicsRow = document.Cursor.PhysicsRow;

            VTHistoryLine historyLine;
            if (!document.History.TryGetHistory(cursorPhysicsRow, out historyLine))
            {
                logger.ErrorFormat("执行script失败, 没有找到光标所在历史记录, {0}", cursorPhysicsRow);
                return;
            }

            string text = VTDocUtils.CreatePlainText(historyLine.Characters);

            ScriptItem scriptItem = scriptItems[0];

            if (!text.Contains(scriptItem.Expect))
            {
                return;
            }

            string terminator = string.Empty;

            switch ((LineTerminators)scriptItem.Terminator)
            {
                case LineTerminators.None: break;
                case LineTerminators.LF: terminator = "\n"; break;
                case LineTerminators.CR: terminator = "\r"; break;
                case LineTerminators.CRLF: terminator = "\r\n"; break;
                default: throw new NotImplementedException();
            }

            string send = string.Format("{0}{1}", scriptItem.Send, terminator);

            // 这里不同步向其他会话发送，单独发送到本会话
            byte[] bytes = writeEncoding.GetBytes(send);
            int code = channelTransport.Write(bytes);
            if (code != ResponseCode.SUCCESS)
            {
                logger.ErrorFormat("执行script失败, 发送数据失败, {0}", code);
            }
            else
            {
                scriptItems.RemoveAt(0);
            }
        }

        /// <summary>
        /// 开始运行Modem传输
        /// </summary>
        /// <param name="sr"></param>
        /// <param name="modemType"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void StartModem(SendReceive sr, ModemTypeEnum modemType)
        {
            List<string> filePaths = new List<string>();

            switch (sr)
            {
                case SendReceive.Send:
                    {
                        OpenFileDialog openFileDialog = new OpenFileDialog();
                        openFileDialog.Multiselect = modemType != ModemTypeEnum.XModem;
                        if (!(bool)openFileDialog.ShowDialog())
                        {
                            return;
                        }

                        filePaths.AddRange(openFileDialog.FileNames);

                        break;
                    }

                case SendReceive.Receive:
                    {
                        if (modemType == ModemTypeEnum.XModem)
                        {
                            SaveFileDialog saveFileDialog = new SaveFileDialog();
                            if (!(bool)saveFileDialog.ShowDialog())
                            {
                                return;
                            }

                            filePaths.Add(saveFileDialog.FileName);
                        }
                        else
                        {
                            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
                            if (folderBrowserDialog.ShowDialog() != DialogResult.OK)
                            {
                                return;
                            }

                            filePaths.Add(folderBrowserDialog.SelectedPath);
                        }

                        break;
                    }

                default:
                    throw new NotImplementedException();
            }

            modemRunning = true;

            ModemTransferVM viewModel = new ModemTransferVM()
            {
                SendReceive = sr,
                Type = modemType,
                Session = Session,
                FilePaths = filePaths,
                Transport = channelTransport
            };
            viewModel.ProgressChanged += ViewModel_ProgressChanged;
            viewModel.StartAsync();

            //ModemWindow modemWindow = new ModemWindow();
            //modemWindow.Owner = Application.Current.MainWindow;
            //modemWindow.DataContext = viewModel;
            //modemWindow.Show();
        }

        /// <summary>
        /// 匹配一行，如果有匹配成功则返回匹配后的数据
        /// </summary>
        /// <param name="textLine">要搜索的行</param>
        /// <returns></returns>
        private List<VTextRange> FindMatches(FindOptions options, VTextLine textLine)
        {
            bool regexp = options.Regexp;
            string keyword = options.Keyword;
            bool caseSensitive = options.CaseSensitive;

            string text = VTDocUtils.CreatePlainText(textLine.Characters);

            if (regexp)
            {
                RegexOptions regexOptions = caseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase;

                // 用正则表达式搜索
                MatchCollection matches = null;
                try
                {
                    matches = Regex.Matches(text, keyword, regexOptions);
                }
                catch (Exception ex)
                {
                    // 避免输入的正则表达式不正确导致RegexParseException异常
                    return null;
                }

                if (matches.Count == 0)
                {
                    return null;
                }

                List<VTextRange> textRanges = new List<VTextRange>();

                foreach (Match match in matches)
                {
                    VTextRange textRange = textLine.MeasureTextRange(match.Index, match.Length);
                    textRanges.Add(textRange);
                }

                return textRanges;
            }
            else
            {
                StringComparison stringComparison = caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

                // 直接文本匹配
                // 注意一行文本里可能会有多个地方匹配，要把所有匹配的地方都找到

                int startIndex = 0;

                // 存储匹配的字符索引
                int matchedIndex = 0;

                if ((matchedIndex = text.IndexOf(keyword, 0, stringComparison)) == -1)
                {
                    // 没找到
                    return null;
                }

                List<VTextRange> vtMatches = new List<VTextRange>();

                VTextRange textRange = textLine.MeasureTextRange(matchedIndex, keyword.Length);
                vtMatches.Add(textRange);

                startIndex = matchedIndex + keyword.Length;

                // 找到了继续找
                while ((matchedIndex = text.IndexOf(keyword, startIndex, stringComparison)) >= 0)
                {
                    textRange = textLine.MeasureTextRange(matchedIndex, keyword.Length);
                    vtMatches.Add(textRange);

                    startIndex = matchedIndex + keyword.Length;
                }

                return vtMatches;
            }
        }

        private IEnumerable<VTHistoryLine> GetNewLines(int firstRow, int lastRow)
        {
            if (firstRow > lastRow)
            {
                // 此时说明在同一行输入了数据
                return null;
            }

            VTDocument document = this.videoTerminal.ActiveDocument;
            VTHistory history = document.History;

            // endPhysicalRow是倒数第二行，因为倒数第一行有可能没有打印完成
            IEnumerable<VTHistoryLine> newLines;
            if (!history.TryGetHistories(firstRow, lastRow, out newLines))
            {
                logger.ErrorFormat("查找NewLines失败");
                return null;
            }

            return newLines;
        }

        #endregion

        #region 公开接口

        /// <summary>
        /// 通过键盘输入发送数据
        /// </summary>
        /// <param name="kbdInput">用户输入信息</param>
        public void SendInput(VTKeyboardInput kbdInput)
        {
            if (channelTransport.Status != SessionStatusEnum.Connected)
            {
                return;
            }

            // 要发送的数据
            byte[] bytes = null;

            if (kbdInput.FromIMEInput)
            {
                bytes = writeEncoding.GetBytes(kbdInput.Text);
            }
            else
            {
                KeyboardBase keyboard = videoTerminal.Keyboard;

                bytes = keyboard.TranslateInput(kbdInput);
            }

            if (bytes == null)
            {
                return;
            }

            kbdInput.SendBytes = bytes;

            this.PerformSend(bytes);

            this.tabEventSendUserInput.Buffer = bytes;
            base.RaiseTabEvent(this.tabEventSendUserInput);
        }

        public int Control(int command, object parameter, out object result)
        {
            return channelTransport.Control(command, parameter, out result);
        }

        public int Control(int code)
        {
            object result;
            return Control(code, null, out result);
        }

        public int Control(int code, object parameter)
        {
            object result;
            return Control(code, parameter, out result);
        }

        #endregion

        #region 事件处理器

        private void ViewModel_ProgressChanged(ModemTransferVM sender, double progress, int code)
        {
            if (progress < 0 || progress >= 100)
            {
                // 传输结束
                sender.ProgressChanged -= ViewModel_ProgressChanged;
                modemRunning = false;
            }
        }

        private void VideoTerminal_ViewportChanged(IVideoTerminal vt, int newRow, int newColumn)
        {
            ViewportRow = newRow;
            ViewportColumn = newColumn;
        }

        private void SessionTransport_DataReceived(ChannelTransport transport, byte[] buffer, int size)
        {
            VTDebug.Context.WriteRawRead(buffer, size);

            // 如果正在运行Modem传输，那么不处理数据
            if (modemRunning)
            {
                return;
            }

            // 窗口持续改变大小的时候可能导致Render和SizeChanged事件一起运行，产生多线程修改VTDocument的bug
            // 所以这里把Render放在UI线程处理
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                try
                {
                    // 渲染的新行 = 渲染之前光标物理行数和渲染之后光标物理行数之间的所有行数
                    VTCursor mainCursor = this.videoTerminal.MainDocument.Cursor;
                    int firstRow = mainCursor.PhysicsRow;

                    this.videoTerminal.ProcessRead(buffer, size);

                    int lastRow = mainCursor.PhysicsRow;

                    // https://gitee.com/zyfalreadyexsit/terminal/issues/ICG9KR
                    // 解决刚打开会话之后，获取不到实际窗口大小导致终端行和列不正确的问题
                    VTSize termsize = this.videoTerminal.ActiveDocument.GFactory.TerminalSize;
                    this.videoTerminal.Resize(termsize);

                    this.tabEventShellRendered.Buffer = buffer;
                    this.tabEventShellRendered.Length = size;
                    this.tabEventShellRendered.Timestamp = DateTime.Now.ToFileTime();
                    this.tabEventShellRendered.NewLines.Clear();
                    IEnumerable<VTHistoryLine> newLines = this.GetNewLines(firstRow, lastRow - 1); // lastRow - 1 不包含光标所在行，因为光标所在行有可能还没打印结束
                    if (newLines != null)
                    {
                        this.tabEventShellRendered.NewLines.AddRange(newLines);
                    }

                    base.RaiseTabEvent(this.tabEventShellRendered);
                }
                catch (Exception ex)
                {
                    logger.Error("Render异常", ex);
                }

            }, System.Windows.Threading.DispatcherPriority.Background);

            VTDocument mainDocument = this.videoTerminal.MainDocument;
            this.TotalRows = mainDocument.History.Lines;
        }

        private void SessionTransport_StatusChanged(object client, SessionStatusEnum status)
        {
            logger.InfoFormat("会话状态发生改变, {0}", status);

            byte[] bytesDisplay = null;

            switch (status)
            {
                case SessionStatusEnum.Connected:
                    {
                        break;
                    }

                case SessionStatusEnum.Connecting:
                    {
                        bytesDisplay = readEncoding.GetBytes("连接主机中...\r\n");
                        break;
                    }

                case SessionStatusEnum.ConnectError:
                    {
                        bytesDisplay = readEncoding.GetBytes("与主机连接失败...\r\n");
                        break;
                    }

                case SessionStatusEnum.Disconnected:
                    {
                        bytesDisplay = readEncoding.GetBytes("与主机断开连接...\r\n");
                        break;
                    }

                default:
                    throw new NotImplementedException();
            }

            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                if (bytesDisplay != null)
                {
                    videoTerminal.ProcessRead(bytesDisplay, bytesDisplay.Length);
                }

                if (this.Status != status)
                {
                    this.tabEventStatusChanged.OldStatus = this.Status;
                    this.tabEventStatusChanged.NewStatus = status;
                    this.RaiseTabEvent(this.tabEventStatusChanged);

                    this.Status = status;
                }
            });
        }

        #endregion

        #region IClientShellTab

        public void Send(byte[] bytes)
        {
            if (this.channelTransport.Status != SessionStatusEnum.Connected)
            {
                return;
            }

            this.PerformSend(bytes);
        }

        /// <summary>
        /// 发送纯文本数据
        /// </summary>
        /// <param name="text"></param>
        public void Send(string text)
        {
            byte[] bytes = writeEncoding.GetBytes(text);

            this.PerformSend(bytes);
        }

        public VTParagraph GetParagraph(VTParagraphOptions options)
        {
            throw new RefactorImplementedException();
        }

        /// <summary>
        /// 保存指定内容到文件
        /// </summary>
        /// <param name="paragraphType"></param>
        /// <param name="format"></param>
        /// <param name="filePath"></param>
        public void SaveToFile(ParagraphTypeEnum paragraphType, ParagraphFormatEnum format, string filePath)
        {
            try
            {
                VTParagraph paragraph = videoTerminal.CreateParagraph(paragraphType, format);
                File.WriteAllText(filePath, paragraph.Content);
            }
            catch (Exception ex)
            {
                logger.Error("保存日志异常", ex);
                MTMessageBox.Error("保存失败");
            }
        }

        /// <summary>
        /// 复制当前选中的内容
        /// </summary>
        public void CopySelection()
        {
            VTParagraph paragraph = this.videoTerminal.CreateParagraph(ParagraphTypeEnum.Selected, ParagraphFormatEnum.PlainText);
            if (paragraph.IsEmpty)
            {
                return;
            }

            // 把数据设置到Windows剪贴板里
            System.Windows.Clipboard.SetText(paragraph.Content);
        }

        public void ClearScreen()
        {
            VTDocument document = this.videoTerminal.ActiveDocument;
            document.DeleteViewoprt();
            document.SetCursorLogical(0, 0);
            document.RequestInvalidate();
        }

        public IOverlayPanel GetOverlayPanel(string id)
        {
            return this.OverlayPanels.FirstOrDefault(v => v.ID.ToString() == id);
        }

        public List<VTextRange> FindMatches(FindOptions options)
        {
            if (this.videoTerminal == null)
            {
                return null;
            }

            if (string.IsNullOrEmpty(options.Keyword))
            {
                return null;
            }

            List<VTextRange> result = new List<VTextRange>();

            VTDocument activeDocument = this.videoTerminal.ActiveDocument;

            VTextLine current = activeDocument.FirstLine;

            while (current != null)
            {
                List<VTextRange> matches = FindMatches(options, current);
                if (matches != null)
                {
                    result.AddRange(matches);
                }

                current = current.NextLine;
            }

            return result;
        }

        public ISshChannel GetSshEngine()
        {
            return this.channelTransport.Engine as ISshChannel;
        }

        #endregion
    }
}

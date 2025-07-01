using ModengTerm.Addon;
using ModengTerm.Addon.Interactive;
using ModengTerm.Base;
using ModengTerm.Base.DataModels;
using ModengTerm.Base.Enumerations;
using ModengTerm.Document;
using ModengTerm.Document.Enumerations;
using ModengTerm.Document.Graphics;
using ModengTerm.Document.Utility;
using ModengTerm.Terminal;
using ModengTerm.Terminal.DataModels;
using ModengTerm.Terminal.Enumerations;
using ModengTerm.Terminal.Keyboard;
using ModengTerm.Terminal.Modem;
using ModengTerm.Terminal.Session;
using ModengTerm.ViewModel.Panel;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
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
        private SessionTransport sessionTransport;

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

        /// <summary>
        /// 提供剪贴板功能
        /// </summary>
        private VTClipboard clipboard;

        private string uri;

        private int totalRows;

        private Visibility contextMenuVisibility;

        private AutoCompletionVM autoCompletionVM;

        private bool inputPanelVisible;

        private bool modemRunning;

        private List<ScriptItem> scriptItems;

        private TabEventShellSendUserInput tabEventSendUserInput;
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
        /// 向外部公开终端模拟器的控制接口
        /// </summary>
        public IVideoTerminal VideoTerminal { get { return videoTerminal; } }

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

        /// <summary>
        /// 获取或设置终端宽度
        /// </summary>
        public double Width { get; set; }

        /// <summary>
        /// 获取或设置终端高度
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
        /// 自动完成功能ViewModel
        /// </summary>
        public AutoCompletionVM AutoCompletionVM
        {
            get { return autoCompletionVM; }
            private set
            {
                if (autoCompletionVM != value)
                {
                    autoCompletionVM = value;
                    NotifyPropertyChanged("AutoCompletionVM");
                }
            }
        }

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

        public SessionTransport Transport { get { return sessionTransport; } }

        public BindableCollection<OverlayPanel> OverlayPanels { get; private set; }

        #endregion

        #region 构造方法

        public ShellSessionVM(XTermSession session) :
            base(session)
        {
            this.OverlayPanels = new BindableCollection<OverlayPanel>();
            this.tabEventSendUserInput = new TabEventShellSendUserInput() { Sender = this };
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
            scriptItems = Session.GetOption(OptionKeyEnum.LOGIN_SCRIPT_ITEMS, new List<ScriptItem>());
            HistoryCommands = new BindableCollection<string>();

            writeEncoding = Encoding.GetEncoding(Session.GetOption(OptionKeyEnum.TERM_WRITE_ENCODING, OptionDefaultValues.TERM_WRITE_ENCODING));
            readEncoding = Encoding.GetEncoding(Session.GetOption(OptionKeyEnum.TERM_READ_ENCODING, OptionDefaultValues.TERM_READ_ENCODING));
            clipboard = new VTClipboard()
            {
                MaximumHistory = Session.GetOption<int>(OptionKeyEnum.TERM_MAX_CLIPBOARD_HISTORY)
            };

            #region 初始化上下文菜单

            ContextMenus = new BindableCollection<ContextMenuVM>();
            ContextMenus.AddRange(VMUtils.CreateContextMenuVMs(false));

            RightClickActions brc = Session.GetOption<RightClickActions>(OptionKeyEnum.BEHAVIOR_RIGHT_CLICK);
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

            VTOptions options = new VTOptions()
            {
                Session = Session,
                SessionTransport = new SessionTransport(),
                AlternateDocument = AlternateDocument,
                MainDocument = MainDocument,
                Width = Width,
                Height = Height
            };

            VideoTerminal videoTerminal = new VideoTerminal();
            videoTerminal.OnViewportChanged += VideoTerminal_ViewportChanged;
            videoTerminal.DisableBell = Session.GetOption<bool>(OptionKeyEnum.TERM_DISABLE_BELL);
            videoTerminal.Initialize(options);
            this.videoTerminal = videoTerminal;

            #endregion

            #region 加载自动完成列表功能

            AutoCompletionVM = new AutoCompletionVM();
            AutoCompletionVM.Initialize(this);
            AutoCompletionVM.Enabled = Session.GetOption(OptionKeyEnum.TERM_ADVANCE_AUTO_COMPLETION_ENABLED, OptionDefaultValues.TERM_ADVANCE_AUTO_COMPLETION_ENABLED);

            #endregion

            #region 连接终端通道

            // 连接SSH服务器
            SessionTransport transport = options.SessionTransport;
            transport.StatusChanged += SessionTransport_StatusChanged;
            transport.DataReceived += SessionTransport_DataReceived;
            transport.Initialize(Session);
            transport.OpenAsync();

            sessionTransport = transport;

            #endregion

            Uri = InitializeURI();

            isRunning = true;

            return ResponseCode.SUCCESS;
        }

        protected override void OnClose()
        {
            if (!this.isRunning)
            {
                return;
            }

            AutoCompletionVM.Release();

            sessionTransport.StatusChanged -= SessionTransport_StatusChanged;
            sessionTransport.DataReceived -= SessionTransport_DataReceived;
            sessionTransport.Close();
            sessionTransport.Release();

            videoTerminal.OnViewportChanged -= VideoTerminal_ViewportChanged;
            videoTerminal.Release();

            // 释放剪贴板
            clipboard.Release();

            #region 释放OverlayPanel

            foreach (OverlayPanel overlayPanel in this.OverlayPanels)
            {
                overlayPanel.Release();
            }

            this.OverlayPanels.Clear();

            #endregion

            isRunning = false;
        }

        #endregion

        #region 实例方法

        private string InitializeURI()
        {
            string uri = string.Empty;

            switch ((SessionTypeEnum)Session.Type)
            {
                case SessionTypeEnum.Tcp:
                    {
                        string addr = this.Session.GetOption<string>(OptionKeyEnum.RAW_TCP_ADDRESS);
                        string port = this.Session.GetOption<string>(OptionKeyEnum.RAW_TCP_PORT);
                        uri = string.Format("tcp://{0}:{1}", addr, port);
                        break;
                    }

                case SessionTypeEnum.Localhost:
                    {
                        string cmdPath = Session.GetOption<string>(OptionKeyEnum.CMD_STARTUP_PATH);
                        uri = string.Format("{0}", cmdPath);
                        break;
                    }

                case SessionTypeEnum.SSH:
                    {
                        string userName = Session.GetOption<string>(OptionKeyEnum.SSH_USER_NAME);
                        string hostName = Session.GetOption<string>(OptionKeyEnum.SSH_ADDR);
                        int port = Session.GetOption<int>(OptionKeyEnum.SSH_PORT);
                        uri = string.Format("ssh://{0}@{1}:{2}", userName, hostName, port);
                        break;
                    }

                case SessionTypeEnum.SerialPort:
                    {
                        string portName = Session.GetOption<string>(OptionKeyEnum.SERIAL_PORT_NAME);
                        int baudRate = Session.GetOption<int>(OptionKeyEnum.SERIAL_PORT_BAUD_RATE);
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
            int code = sessionTransport.Write(bytes);
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
                Transport = sessionTransport
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
            if (sessionTransport.Status != SessionStatusEnum.Connected)
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
            return sessionTransport.Control(command, parameter, out result);
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

        private void SessionTransport_DataReceived(SessionTransport transport, byte[] buffer, int size)
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
            });

            VTDocument mainDocument = this.videoTerminal.MainDocument;
            this.TotalRows = mainDocument.History.Lines;

            HandleScript();
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

        private void SelectAll()
        {
            videoTerminal.SelectAll();
        }

        /// <summary>
        /// 显示剪贴板历史记录
        /// </summary>
        private void ClipboardHistory()
        {
            ClipboardParagraphSource clipboardParagraphSource = new ClipboardParagraphSource(clipboard);
            clipboardParagraphSource.Session = Session;

            ClipboardVM clipboardVM = new ClipboardVM(clipboardParagraphSource, this);

            //ParagraphsWindow paragraphsWindow = new ParagraphsWindow(clipboardVM);
            //paragraphsWindow.Title = "剪贴板历史";
            //paragraphsWindow.Owner = Window.GetWindow(this.Content);
            //paragraphsWindow.Show();
        }

        /// <summary>
        /// 选中的内容添加到收藏夹
        /// </summary>
        private void AddFavorites()
        {
            VTParagraph paragraph = videoTerminal.CreateParagraph(ParagraphTypeEnum.Selected, ParagraphFormatEnum.PlainText);
            if (paragraph.IsEmpty)
            {
                return;
            }

            Favorites favorites = new Favorites()
            {
                ID = Guid.NewGuid().ToString(),
                Typeface = videoTerminal.ActiveDocument.Typeface,
                SessionID = Session.ID,
            };

            throw new NotImplementedException();

            //int code = this.TerminalAgent.AddFavorites(favorites);
            //if (code != ResponseCode.SUCCESS)
            //{
            //    MTMessageBox.Info("保存失败");
            //}
        }

        /// <summary>
        /// 显示收藏夹列表
        /// </summary>
        private void FaviritesList()
        {
            throw new NotImplementedException();

            //FavoritesParagraphSource favoritesParagraphSource = new FavoritesParagraphSource(this.TerminalAgent);
            //favoritesParagraphSource.Session = this.Session;

            //FavoritesVM favoritesVM = new FavoritesVM(favoritesParagraphSource, this);
            //favoritesVM.SendToAllTerminalDlg = this.SendToAllCallback;

            //ParagraphsWindow paragraphsWindow = new ParagraphsWindow(favoritesVM);
            //paragraphsWindow.Title = "收藏夹列表";
            //paragraphsWindow.Owner = Window.GetWindow(this.Content);
            //paragraphsWindow.Show();
        }

        #endregion

        #region IShellTab

        public void Send(byte[] bytes)
        {
            if (this.sessionTransport.Status != SessionStatusEnum.Connected)
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

            clipboard.SetData(paragraph);

            // 把数据设置到Windows剪贴板里
            System.Windows.Clipboard.SetText(paragraph.Content);
        }

        public void ClearScreen()
        {
            VTDocument document = VideoTerminal.ActiveDocument;
            document.DeleteViewoprt();
            document.SetCursorLogical(0, 0);
            document.RequestInvalidate();
        }

        public void AddOverlayPanel(IOverlayPanel panel)
        {
            OverlayPanel overlayPanel = panel as OverlayPanel;
            overlayPanel.OwnerTab = this;

            this.OverlayPanels.Add(overlayPanel);
        }

        public void RemoveOverlayPanel(IOverlayPanel panel)
        {
            OverlayPanel overlayPanel = panel as OverlayPanel;
            this.OverlayPanels.Remove(overlayPanel);
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

        #endregion
    }
}

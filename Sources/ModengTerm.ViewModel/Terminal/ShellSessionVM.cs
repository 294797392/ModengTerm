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
using ModengTerm.Terminal.Loggering;
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

        private RecordStatusEnum recordStatus;
        private PlaybackStatusEnum playbackStatus;
        private PlaybackStream playbackStream;

        private string uri;

        private int totalRows;

        private Visibility contextMenuVisibility;

        private AutoCompletionVM autoCompletionVM;

        private bool inputPanelVisible;

        private bool modemRunning;

        private List<ScriptItem> scriptItems;

        private LoggerManager logMgr;

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

        public GraphicsInterface MainDocument { get; set; }
        public GraphicsInterface AlternateDocument { get; set; }

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
        /// 录像状态
        /// </summary>
        public RecordStatusEnum RecordStatus
        {
            get { return recordStatus; }
            set
            {
                if (recordStatus != value)
                {
                    recordStatus = value;
                    NotifyPropertyChanged("RecordStatus");
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
            //logMgr = MTermApp.Context.LoggerManager;

            scriptItems = Session.GetOption(OptionKeyEnum.LOGIN_SCRIPT_ITEMS, new List<ScriptItem>());
            HistoryCommands = new BindableCollection<string>();

            RecordStatus = RecordStatusEnum.Stop;
            writeEncoding = Encoding.GetEncoding(Session.GetOption(OptionKeyEnum.TERM_WRITE_ENCODING, OptionDefaultValues.TERM_WRITE_ENCODING));
            readEncoding = Encoding.GetEncoding(Session.GetOption(OptionKeyEnum.TERM_READ_ENCODING, OptionDefaultValues.TERM_READ_ENCODING));
            clipboard = new VTClipboard()
            {
                MaximumHistory = Session.GetOption<int>(OptionKeyEnum.TERM_MAX_CLIPBOARD_HISTORY)
            };

            #region 初始化上下文菜单

            ContextMenus = new BindableCollection<ContextMenuVM>();
            ContextMenus.AddRange(VMUtils.CreateContextMenuVMs(false));

            BehaviorRightClicks brc = Session.GetOption<BehaviorRightClicks>(OptionKeyEnum.BEHAVIOR_RIGHT_CLICK);
            if (brc == BehaviorRightClicks.ContextMenu)
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
            videoTerminal.OnLineFeed += VideoTerminal_LineFeed;
            videoTerminal.OnDocumentChanged += VideoTerminal_DocumentChanged;
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

            // 停止对终端的日志记录
            StopLogger();

            // 停止录制
            StopRecord();

            sessionTransport.StatusChanged -= SessionTransport_StatusChanged;
            sessionTransport.DataReceived -= SessionTransport_DataReceived;
            sessionTransport.Close();
            sessionTransport.Release();

            videoTerminal.OnViewportChanged -= VideoTerminal_ViewportChanged;
            videoTerminal.OnLineFeed -= VideoTerminal_LineFeed;
            videoTerminal.OnDocumentChanged -= VideoTerminal_DocumentChanged;
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

            videoTerminal.ProcessWrite(bytes);
        }

        /// <summary>
        /// 向同步输入的会话发送数据
        /// </summary>
        /// <param name="bytes">要发送的数据</param>
        private void SendSyncInput(byte[] bytes)
        {
            // TODO：删除

            //foreach (BroadcastSessionVM syncInput in this.SyncInputSessions)
            //{
            //    ShellSessionVM shellSession = syncInput.ShellSessionVM;
            //    if (shellSession == null)
            //    {
            //        shellSession = MTermApp.Context.MainWindowVM.ShellSessions.FirstOrDefault(v => v.ID == syncInput.ID);
            //        syncInput.ShellSessionVM = shellSession;
            //    }

            //    if (shellSession != null && shellSession.Status == SessionStatusEnum.Connected)
            //    {
            //        shellSession.PerformSend(bytes);
            //    }
            //}
        }

        /// <summary>
        /// 处理录像
        /// </summary>
        /// <param name="bytes">收到的数据</param>
        /// <param name="size">收到的数据长度</param>
        /// <exception cref="NotImplementedException"></exception>
        private void HandleRecord(byte[] bytes, int size)
        {
            switch (recordStatus)
            {
                case RecordStatusEnum.Pause:
                    {
                        break;
                    }

                case RecordStatusEnum.Stop:
                    {
                        break;
                    }

                case RecordStatusEnum.Recording:
                    {
                        // 拷贝回放数据
                        byte[] frameData = new byte[size];
                        Buffer.BlockCopy(bytes, 0, frameData, 0, frameData.Length);

                        // 写入回放帧
                        PlaybackFrame frame = new PlaybackFrame()
                        {
                            Timestamp = DateTime.Now.ToFileTime(),
                            Data = frameData
                        };
                        playbackStream.WriteFrame(frame);

                        break;
                    }

                default:
                    throw new NotImplementedException();
            }
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

            videoTerminal.RaiseKeyboardInput(kbdInput);

            PerformSend(bytes);

            SendSyncInput(bytes);
        }

        ///// <summary>
        ///// 发送原始字节数据
        ///// </summary>
        ///// <param name="rawData"></param>
        //public override void SendRawData(byte[] rawData)
        //{
        //    PerformSend(rawData);

        //    SendSyncInput(rawData);
        //}

        /// <summary>
        /// 发送纯文本数据
        /// </summary>
        /// <param name="text"></param>
        public void SendText(string text)
        {
            byte[] bytes = writeEncoding.GetBytes(text);

            PerformSend(bytes);

            SendSyncInput(bytes);
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


        /// <summary>
        /// 开始录像
        /// </summary>
        /// <param name="fileName">录像名称</param>
        public void StartRecord(string fileName)
        {
            if (recordStatus == RecordStatusEnum.Recording)
            {
                logger.WarnFormat("StartRecord: 当前正在录像中");
                return;
            }

            Playback playbackFile = new Playback()
            {
                ID = Guid.NewGuid().ToString(),
                Name = fileName,
                Session = Session
            };

            // 先打开录像文件
            playbackStream = new PlaybackStream();
            int code = playbackStream.OpenWrite(playbackFile);
            if (code != ResponseCode.SUCCESS)
            {
                MTMessageBox.Error("打开录像文件失败, {0}", ResponseCode.GetMessage(code));
                return;
            }

            // 然后保存录像记录
            code = ServiceAgent.AddPlayback(playbackFile);
            if (code != ResponseCode.SUCCESS)
            {
                MTMessageBox.Error("录制失败, 保存录制记录失败, {0}", ResponseCode.GetMessage(code));
                playbackStream.Close();
                return;
            }

            RecordStatus = RecordStatusEnum.Recording;
        }

        /// <summary>
        /// 停止录像
        /// </summary>
        public void StopRecord()
        {
            if (recordStatus == RecordStatusEnum.Stop)
            {
                return;
            }

            // TODO：此时文件可能正在被写入，playbackStream里做了异常处理，所以直接这么写
            // 需要优化
            playbackStream.Close();

            RecordStatus = RecordStatusEnum.Stop;
        }

        public void StartLogger(IVideoTerminal videoTerminal, LoggerOptions loggerOptions)
        {
            logMgr.Start(videoTerminal, loggerOptions);
        }

        public void StopLogger()
        {
            //throw new RefactorImplementedException();
            //logMgr.Stop(videoTerminal);
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

        private void VideoTerminal_LineFeed(IVideoTerminal vt, bool isAlternate, int oldPhysicsRow, VTHistoryLine historyLine)
        {
            if (isAlternate)
            {
                return;
            }

            int totalRows = oldPhysicsRow + 1;

            if (totalRows > TotalRows)
            {
                TotalRows = totalRows;
            }
        }

        private void VideoTerminal_DocumentChanged(IVideoTerminal arg1, VTDocument oldDocument, VTDocument newDocument)
        {
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
                    videoTerminal.ProcessRead(buffer, size);

                    // https://gitee.com/zyfalreadyexsit/terminal/issues/ICG9KR
                    // 解决刚打开会话之后，获取不到实际窗口大小导致终端行和列不正确的问题
                    VTSize graphicsSize = videoTerminal.ActiveDocument.GraphicsInterface.DrawAreaSize;
                    videoTerminal.Resize(graphicsSize);

                    TabEventShellRendered tabEvent = new TabEventShellRendered()
                    {
                        Buffer = buffer,
                        Length = size
                    };
                    base.RaiseTabEvent(tabEvent);
                }
                catch (Exception ex)
                {
                    logger.Error("Render异常", ex);
                }
            });

            HandleRecord(buffer, size);

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
                    ClientEventTabStatusChanged statusChanged = new ClientEventTabStatusChanged()
                    {
                        OldStatus = this.Status,
                        NewStatus = status
                    };
                    this.RaiseClientEvent(statusChanged);

                    this.Status = status;
                }
            });
        }

        public void PauseLogger()
        {
            logMgr.Pause(videoTerminal);
        }

        public void ResumeLogger()
        {
            logMgr.Resume(videoTerminal);
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

        /// <summary>
        /// 暂停录像
        /// </summary>
        private void PauseRecord()
        {
            if (recordStatus == RecordStatusEnum.Pause)
            {
                return;
            }

            RecordStatus = RecordStatusEnum.Pause;
        }

        /// <summary>
        /// 继续录像
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        private void ResumeRecord()
        {
            if (recordStatus == RecordStatusEnum.Recording)
            {
                return;
            }

            switch (recordStatus)
            {
                case RecordStatusEnum.Stop:
                    {
                        break;
                    }

                case RecordStatusEnum.Recording:
                    {
                        break;
                    }

                case RecordStatusEnum.Pause:
                    {
                        RecordStatus = RecordStatusEnum.Recording;
                        break;
                    }

                default:
                    throw new NotImplementedException();
            }
        }

        #endregion

        #region IShellTab

        public void Send(byte[] bytes)
        { }

        public void Send(string text)
        { }

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

    public class ShellSessionContextMenu
    {
        public ShellSessionContextMenu()
        {
        }

        //private void ContextMenuOpenPortForwardWindow_Click(ContextMenuVM sender, ShellSessionVM shellSessionVM)
        //{
        //    PortForwardStatusWindow portForwardStatusWindow = new PortForwardStatusWindow(shellSessionVM);
        //    portForwardStatusWindow.Owner = App.Current.MainWindow;
        //    portForwardStatusWindow.Show();
        //}

        /// <summary>
        /// 把当前选中的内容添加到快捷输入列表里
        /// </summary>
        private void ContextMenuAddToQuickCommands_Click(ContextMenuVM sender, ShellSessionVM shellSessionVM)
        {
            throw new NotImplementedException();

            //VTParagraph selectedParagraph = shellSessionVM.VideoTerminal.CreateParagraph(ParagraphTypeEnum.Selected, ParagraphFormatEnum.PlainText);
            //if (selectedParagraph.IsEmpty)
            //{
            //    return;
            //}

            //ShellCommand shellCommand = new ShellCommand()
            //{
            //    ID = Guid.NewGuid().ToString(),
            //    SessionId = shellSessionVM.Session.ID,
            //    Name = string.Format("未命名_{0}", DateTime.Now.ToString(DateTimeFormat.yyyyMMddhhmmss)),
            //    Type = (int)CommandTypeEnum.PureText,
            //    Command = selectedParagraph.Content
            //};

            //int code = shellSessionVM.ServiceAgent.AddShellCommand(shellCommand);
            //if (code != ResponseCode.SUCCESS)
            //{
            //    MTMessageBox.Info("新建快捷命令失败, {0}", code);
            //    return;
            //}

            //QuickCommandVM qcvm = new QuickCommandVM(shellCommand);
            //this.ShellCommands.Add(qcvm);
        }

        private void ContextMenuCreateQuickCommand_Click(ContextMenuVM sender, ShellSessionVM shellSessionVM)
        {
            throw new NotImplementedException();
            //this.OpenCreateShellCommandWindow();
        }

        private void ContextMenuXModemSend_Click(ContextMenuVM sender, ShellSessionVM shellSessionVM)
        {
            throw new NotImplementedException();
            //this.StartModem(SendReceive.Send, ModemTypeEnum.XModem);
        }

        private void ContextMenuXModemReceive_Click(ContextMenuVM sender, ShellSessionVM shellSessionVM)
        {
            throw new NotImplementedException();
            //this.StartModem(SendReceive.Receive, ModemTypeEnum.XModem);
        }

        private void ContextMenuYModemSend_Click(ContextMenuVM sender, ShellSessionVM shellSessionVM)
        {
            throw new NotImplementedException();
            //this.StartModem(SendReceive.Send, ModemTypeEnum.YModem);
        }
    }
}

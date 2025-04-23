using Microsoft.Win32;
using ModengTerm.Base;
using ModengTerm.Base.DataModels;
using ModengTerm.Base.Enumerations;
using ModengTerm.Document;
using ModengTerm.Document.Drawing;
using ModengTerm.Document.Enumerations;
using ModengTerm.Document.Utility;
using ModengTerm.Terminal.DataModels;
using ModengTerm.Terminal.Enumerations;
using ModengTerm.Terminal.Keyboard;
using ModengTerm.Terminal.Loggering;
using ModengTerm.Terminal.Modem;
using ModengTerm.Terminal.Session;
using ModengTerm.Terminal.Windows;
using ModengTerm.ViewModels;
using ModengTerm.ViewModels.Terminals;
using ModengTerm.Windows;
using ModengTerm.Windows.SSH;
using ModengTerm.Windows.Terminals;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using WPFToolkit.MVVM;

namespace ModengTerm.Terminal.ViewModels
{
    public class ShellSessionVM : InputSessionVM
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
                if (this.viewportRow != value)
                {
                    this.viewportRow = value;
                    this.NotifyPropertyChanged("ViewportRow");
                }
            }
        }

        /// <summary>
        /// 可视区域的列数
        /// </summary>
        public int ViewportColumn
        {
            get { return this.viewportColumn; }
            set
            {
                if (this.viewportColumn != value)
                {
                    this.viewportColumn = value;
                    this.NotifyPropertyChanged("ViewportColumn");
                }
            }
        }

        /// <summary>
        /// 向外部公开终端模拟器的控制接口
        /// </summary>
        public IVideoTerminal VideoTerminal { get { return this.videoTerminal; } }

        /// <summary>
        /// SSH主机的Uri
        /// </summary>
        public string Uri
        {
            get { return this.uri; }
            private set
            {
                if (this.uri != value)
                {
                    this.uri = value;
                    this.NotifyPropertyChanged("Uri");
                }
            }
        }

        public GraphicsInterface MainDocument { get; set; }
        public GraphicsInterface AlternateDocument { get; set; }

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
            get { return this.totalRows; }
            set
            {
                if (this.totalRows != value)
                {
                    this.totalRows = value;
                    this.NotifyPropertyChanged("TotalRows");
                }
            }
        }

        /// <summary>
        /// 控制右键菜单的显示和隐藏
        /// </summary>
        public Visibility ContextMenuVisibility
        {
            get { return this.contextMenuVisibility; }
            set
            {
                if (this.contextMenuVisibility != value)
                {
                    this.contextMenuVisibility = value;
                    this.NotifyPropertyChanged("ContextMenuVisibility");
                }
            }
        }

        /// <summary>
        /// 保存该会话输入的历史记录
        /// </summary>
        public BindableCollection<string> HistoryCommands { get; private set; }

        /// <summary>
        /// 要同步输入的会话列表
        /// </summary>
        public BindableCollection<SyncInputSessionVM> SyncInputSessions { get; private set; }

        /// <summary>
        /// 自动完成功能ViewModel
        /// </summary>
        public AutoCompletionVM AutoCompletionVM
        {
            get { return this.autoCompletionVM; }
            private set
            {
                if (this.autoCompletionVM != value)
                {
                    this.autoCompletionVM = value;
                    this.NotifyPropertyChanged("AutoCompletionVM");
                }
            }
        }

        /// <summary>
        /// 录像状态
        /// </summary>
        public RecordStatusEnum RecordStatus
        {
            get { return this.recordStatus; }
            set
            {
                if (this.recordStatus != value)
                {
                    this.recordStatus = value;
                    this.NotifyPropertyChanged("RecordStatus");
                }
            }
        }

        /// <summary>
        /// 是否显示输入栏
        /// </summary>
        public bool InputPanelVisible
        {
            get { return this.inputPanelVisible; }
            set
            {
                if (this.inputPanelVisible != value)
                {
                    this.inputPanelVisible = value;
                    this.NotifyPropertyChanged("InputPanelVisible");
                }
            }
        }

        public SessionTransport Transport { get { return this.sessionTransport; } }

        #endregion

        #region 构造方法

        public ShellSessionVM(XTermSession session) :
            base(session)
        {
        }

        #endregion

        #region OpenedSessionVM Member

        protected override int OnOpen()
        {
            this.logMgr = MTermApp.Context.LoggerManager;

            this.scriptItems = this.Session.GetOption<List<ScriptItem>>(OptionKeyEnum.LOGIN_SCRIPT_ITEMS, new List<ScriptItem>());
            this.HistoryCommands = new BindableCollection<string>();

            this.SyncInputSessions = new BindableCollection<SyncInputSessionVM>();

            this.RecordStatus = RecordStatusEnum.Stop;
            this.writeEncoding = Encoding.GetEncoding(this.Session.GetOption<string>(OptionKeyEnum.TERM_WRITE_ENCODING, OptionDefaultValues.TERM_WRITE_ENCODING));
            this.readEncoding = Encoding.GetEncoding(this.Session.GetOption<string>(OptionKeyEnum.TERM_READ_ENCODING, OptionDefaultValues.TERM_READ_ENCODING));
            this.clipboard = new VTClipboard()
            {
                MaximumHistory = this.Session.GetOption<int>(OptionKeyEnum.TERM_MAX_CLIPBOARD_HISTORY)
            };

            #region 初始化上下文菜单

            BehaviorRightClicks brc = this.Session.GetOption<BehaviorRightClicks>(OptionKeyEnum.BEHAVIOR_RIGHT_CLICK);
            if (brc == BehaviorRightClicks.ContextMenu)
            {
                this.ContextMenuVisibility = Visibility.Visible;
            }
            else
            {
                this.ContextMenuVisibility = Visibility.Collapsed;
            }

            #endregion

            #region 初始化终端

            VTOptions options = new VTOptions()
            {
                Session = this.Session,
                SessionTransport = new SessionTransport(),
                AlternateDocument = this.AlternateDocument,
                MainDocument = this.MainDocument,
                Width = this.Width,
                Height = this.Height
            };

            VideoTerminal videoTerminal = new VideoTerminal();
            videoTerminal.OnViewportChanged += this.VideoTerminal_ViewportChanged;
            videoTerminal.OnLineFeed += VideoTerminal_LineFeed;
            videoTerminal.OnDocumentChanged += VideoTerminal_DocumentChanged;
            videoTerminal.DisableBell = this.Session.GetOption<bool>(OptionKeyEnum.TERM_DISABLE_BELL);
            videoTerminal.Initialize(options);
            this.videoTerminal = videoTerminal;

            #endregion

            #region 加载自动完成列表功能

            this.AutoCompletionVM = new AutoCompletionVM();
            this.AutoCompletionVM.Initialize(this);
            this.AutoCompletionVM.Enabled = this.Session.GetOption<bool>(OptionKeyEnum.TERM_ADVANCE_AUTO_COMPLETION_ENABLED, OptionDefaultValues.TERM_ADVANCE_AUTO_COMPLETION_ENABLED);

            #endregion

            #region 连接终端通道

            // 连接SSH服务器
            SessionTransport transport = options.SessionTransport;
            transport.StatusChanged += this.SessionTransport_StatusChanged;
            transport.DataReceived += this.SessionTransport_DataReceived;
            transport.Initialize(this.Session);
            transport.OpenAsync();

            this.sessionTransport = transport;

            #endregion

            this.Uri = this.InitializeURI();

            this.isRunning = true;

            return ResponseCode.SUCCESS;
        }

        protected override void OnClose()
        {
            if (!this.isRunning)
            {
                return;
            }

            this.AutoCompletionVM.Release();

            // 停止对终端的日志记录
            this.StopLogger();

            // 停止录制
            this.StopRecord();

            this.sessionTransport.StatusChanged -= this.SessionTransport_StatusChanged;
            this.sessionTransport.DataReceived -= this.SessionTransport_DataReceived;
            this.sessionTransport.Close();
            this.sessionTransport.Release();

            this.videoTerminal.OnViewportChanged -= this.VideoTerminal_ViewportChanged;
            this.videoTerminal.OnLineFeed -= this.VideoTerminal_LineFeed;
            this.videoTerminal.OnDocumentChanged -= this.VideoTerminal_DocumentChanged;
            this.videoTerminal.Release();

            // 释放剪贴板
            this.clipboard.Release();
            this.SyncInputSessions.Clear();

            this.isRunning = false;
        }

        #endregion

        #region 实例方法

        private string InitializeURI()
        {
            string uri = string.Empty;

            switch ((SessionTypeEnum)this.Session.Type)
            {
                case SessionTypeEnum.Localhost:
                    {
                        string cmdPath = this.Session.GetOption<string>(OptionKeyEnum.CMD_STARTUP_PATH);
                        uri = string.Format("{0}", cmdPath);
                        break;
                    }

                case SessionTypeEnum.SSH:
                    {
                        string userName = this.Session.GetOption<string>(OptionKeyEnum.SSH_USER_NAME);
                        string hostName = this.Session.GetOption<string>(OptionKeyEnum.SSH_ADDR);
                        int port = this.Session.GetOption<int>(OptionKeyEnum.SSH_PORT);
                        uri = string.Format("ssh://{0}@{1}:{2}", userName, hostName, port);
                        break;
                    }

                case SessionTypeEnum.SerialPort:
                    {
                        string portName = this.Session.GetOption<string>(OptionKeyEnum.SERIAL_PORT_NAME);
                        int baudRate = this.Session.GetOption<int>(OptionKeyEnum.SERIAL_PORT_BAUD_RATE);
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

        /// <summary>
        /// 向同步输入的会话发送数据
        /// </summary>
        /// <param name="bytes">要发送的数据</param>
        private void SendSyncInput(byte[] bytes)
        {
            foreach (SyncInputSessionVM syncInput in this.SyncInputSessions)
            {
                ShellSessionVM shellSession = syncInput.ShellSessionVM;
                if (shellSession == null)
                {
                    shellSession = MTermApp.Context.MainWindowVM.ShellSessions.FirstOrDefault(v => v.ID == syncInput.ID);
                    syncInput.ShellSessionVM = shellSession;
                }

                if (shellSession != null && shellSession.Status == SessionStatusEnum.Connected)
                {
                    shellSession.PerformSend(bytes);
                }
            }
        }

        /// <summary>
        /// 处理录像
        /// </summary>
        /// <param name="bytes">收到的数据</param>
        /// <param name="size">收到的数据长度</param>
        /// <exception cref="NotImplementedException"></exception>
        private void HandleRecord(byte[] bytes, int size)
        {
            switch (this.recordStatus)
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
                        this.playbackStream.WriteFrame(frame);

                        break;
                    }

                default:
                    throw new NotImplementedException();
            }
        }

        private void HandleScript()
        {
            if (this.scriptItems.Count == 0)
            {
                return;
            }

            VTDocument document = this.videoTerminal.ActiveDocument;
            int cursorPhysicsRow = document.Cursor.PhysicsRow;

            VTHistoryLine historyLine;
            if (!document.History.TryGetHistory(cursorPhysicsRow, out historyLine))
            {
                logger.ErrorFormat("执行script失败, 没有找到光标所在历史记录, {0}", cursorPhysicsRow);
                return;
            }

            string text = VTDocUtils.CreatePlainText(historyLine.Characters);

            ScriptItem scriptItem = this.scriptItems[0];

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
            byte[] bytes = this.writeEncoding.GetBytes(send);
            int code = this.sessionTransport.Write(bytes);
            if (code != ResponseCode.SUCCESS)
            {
                logger.ErrorFormat("执行script失败, 发送数据失败, {0}", code);
            }
            else
            {
                this.scriptItems.RemoveAt(0);
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
                        if (!((bool)openFileDialog.ShowDialog()))
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
                            System.Windows.Forms.FolderBrowserDialog folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
                            if (folderBrowserDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
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

            this.modemRunning = true;

            ModemTransferVM viewModel = new ModemTransferVM()
            {
                SendReceive = sr,
                Type = modemType,
                Session = this.Session,
                FilePaths = filePaths,
                Transport = this.sessionTransport
            };
            viewModel.ProgressChanged += ViewModel_ProgressChanged;
            viewModel.StartAsync();

            //ModemWindow modemWindow = new ModemWindow();
            //modemWindow.Owner = Application.Current.MainWindow;
            //modemWindow.DataContext = viewModel;
            //modemWindow.Show();
        }

        #endregion

        #region 公开接口

        /// <summary>
        /// 通过键盘输入发送数据
        /// </summary>
        /// <param name="kbdInput">用户输入信息</param>
        public override void SendInput(VTKeyboardInput kbdInput)
        {
            if (this.sessionTransport.Status != SessionStatusEnum.Connected)
            {
                return;
            }

            // 要发送的数据
            byte[] bytes = null;

            if (kbdInput.FromIMEInput)
            {
                bytes = this.writeEncoding.GetBytes(kbdInput.Text);
            }
            else
            {
                KeyboardBase keyboard = this.videoTerminal.Keyboard;

                bytes = keyboard.TranslateInput(kbdInput);
            }

            if (bytes == null)
            {
                return;
            }

            kbdInput.SendBytes = bytes;

            this.videoTerminal.RaiseKeyboardInput(kbdInput);

            this.PerformSend(bytes);

            this.SendSyncInput(bytes);
        }

        /// <summary>
        /// 发送原始字节数据
        /// </summary>
        /// <param name="rawData"></param>
        public override void SendRawData(byte[] rawData)
        {
            this.PerformSend(rawData);

            this.SendSyncInput(rawData);
        }

        /// <summary>
        /// 发送纯文本数据
        /// </summary>
        /// <param name="text"></param>
        public override void SendText(string text)
        {
            byte[] bytes = this.writeEncoding.GetBytes(text);

            this.PerformSend(bytes);

            this.SendSyncInput(bytes);
        }

        public int Control(int command, object parameter, out object result)
        {
            return this.sessionTransport.Control(command, parameter, out result);
        }

        public int Control(int code)
        {
            object result;
            return this.Control(code, null, out result);
        }

        public int Control(int code, object parameter)
        {
            object result;
            return this.Control(code, parameter, out result);
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

            this.clipboard.SetData(paragraph);

            // 把数据设置到Windows剪贴板里
            System.Windows.Clipboard.SetText(paragraph.Content);
        }

        /// <summary>
        /// 开始录像
        /// </summary>
        /// <param name="fileName">录像名称</param>
        public void StartRecord(string fileName)
        {
            if (this.recordStatus == RecordStatusEnum.Recording)
            {
                logger.WarnFormat("StartRecord: 当前正在录像中");
                return;
            }

            Playback playbackFile = new Playback()
            {
                ID = Guid.NewGuid().ToString(),
                Name = fileName,
                Session = this.Session
            };

            // 先打开录像文件
            this.playbackStream = new PlaybackStream();
            int code = this.playbackStream.OpenWrite(playbackFile);
            if (code != ResponseCode.SUCCESS)
            {
                MTMessageBox.Error("打开录像文件失败, {0}", ResponseCode.GetMessage(code));
                return;
            }

            // 然后保存录像记录
            code = this.ServiceAgent.AddPlayback(playbackFile);
            if (code != ResponseCode.SUCCESS)
            {
                MTMessageBox.Error("录制失败, 保存录制记录失败, {0}", ResponseCode.GetMessage(code));
                this.playbackStream.Close();
                return;
            }

            this.RecordStatus = RecordStatusEnum.Recording;
        }

        /// <summary>
        /// 停止录像
        /// </summary>
        public void StopRecord()
        {
            if (this.recordStatus == RecordStatusEnum.Stop)
            {
                return;
            }

            // TODO：此时文件可能正在被写入，playbackStream里做了异常处理，所以直接这么写
            // 需要优化
            this.playbackStream.Close();

            this.RecordStatus = RecordStatusEnum.Stop;
        }

        /// <summary>
        /// 保存指定数据到文件
        /// </summary>
        /// <param name="paragraphType"></param>
        /// <param name="format"></param>
        /// <param name="filePath"></param>
        public void SaveToFile(ParagraphTypeEnum paragraphType, ParagraphFormatEnum format, string filePath)
        {
            try
            {
                VTParagraph paragraph = this.videoTerminal.CreateParagraph(paragraphType, format);
                File.WriteAllText(filePath, paragraph.Content);
            }
            catch (Exception ex)
            {
                logger.Error("保存日志异常", ex);
                MTMessageBox.Error("保存失败");
            }
        }

        public void StartLogger(IVideoTerminal videoTerminal, LoggerOptions loggerOptions)
        {
            this.logMgr.Start(videoTerminal, loggerOptions);
        }

        public void StopLogger()
        {
            this.logMgr.Stop(this.videoTerminal);
        }

        /// <summary>
        /// 把剪贴板里的数据粘贴到终端
        /// </summary>
        public void Paste() 
        {
            string text = System.Windows.Clipboard.GetText();
            this.SendText(text);
        }

        #endregion

        #region 事件处理器

        private void ViewModel_ProgressChanged(ModemTransferVM sender, double progress, int code)
        {
            if (progress < 0 || progress >= 100)
            {
                // 传输结束
                sender.ProgressChanged -= this.ViewModel_ProgressChanged;
                this.modemRunning = false;
            }
        }

        private void VideoTerminal_ViewportChanged(IVideoTerminal vt, int newRow, int newColumn)
        {
            this.ViewportRow = newRow;
            this.ViewportColumn = newColumn;
        }

        private void VideoTerminal_LineFeed(IVideoTerminal vt, bool isAlternate, int oldPhysicsRow, VTHistoryLine historyLine)
        {
            if (isAlternate)
            {
                return;
            }

            int totalRows = oldPhysicsRow + 1;

            if (totalRows > this.TotalRows)
            {
                this.TotalRows = totalRows;
            }
        }

        private void VideoTerminal_DocumentChanged(IVideoTerminal arg1, VTDocument oldDocument, VTDocument newDocument)
        {
        }

        private void SessionTransport_DataReceived(SessionTransport client, byte[] buffer, int size)
        {
            VTDebug.Context.WriteRawRead(buffer, size);

            // 如果正在运行Modem传输，那么不处理数据
            if (this.modemRunning)
            {
                return;
            }

            // 窗口持续改变大小的时候可能导致Render和SizeChanged事件一起运行，产生多线程修改VTDocument的bug
            // 所以这里把Render放在UI线程处理
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                try
                {
                    this.videoTerminal.ProcessRead(buffer, size);
                }
                catch (Exception ex)
                {
                    logger.Error("Render异常", ex);
                }
            });

            this.HandleRecord(buffer, size);

            this.HandleScript();
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
                        bytesDisplay = this.readEncoding.GetBytes("连接主机中...\r\n");
                        break;
                    }

                case SessionStatusEnum.ConnectError:
                    {
                        bytesDisplay = this.readEncoding.GetBytes("与主机连接失败...\r\n");
                        break;
                    }

                case SessionStatusEnum.Disconnected:
                    {
                        bytesDisplay = this.readEncoding.GetBytes("与主机断开连接...\r\n");
                        break;
                    }

                default:
                    throw new NotImplementedException();
            }

            App.Current.Dispatcher.Invoke(() =>
            {
                this.RaiseStatusChanged(status);

                if (bytesDisplay != null)
                {
                    this.videoTerminal.ProcessRead(bytesDisplay, bytesDisplay.Length);
                }
            });
        }

        public void PauseLogger()
        {
            this.logMgr.Pause(this.videoTerminal);
        }

        public void ResumeLogger()
        {
            this.logMgr.Resume(this.videoTerminal);
        }

        private void SelectAll()
        {
            this.videoTerminal.SelectAll();
        }

        /// <summary>
        /// 显示剪贴板历史记录
        /// </summary>
        private void ClipboardHistory()
        {
            ClipboardParagraphSource clipboardParagraphSource = new ClipboardParagraphSource(this.clipboard);
            clipboardParagraphSource.Session = this.Session;

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
            VTParagraph paragraph = this.videoTerminal.CreateParagraph(ParagraphTypeEnum.Selected, ParagraphFormatEnum.PlainText);
            if (paragraph.IsEmpty)
            {
                return;
            }

            Favorites favorites = new Favorites()
            {
                ID = Guid.NewGuid().ToString(),
                Typeface = this.videoTerminal.ActiveDocument.Typeface,
                SessionID = this.Session.ID,
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
            if (this.recordStatus == RecordStatusEnum.Pause)
            {
                return;
            }

            this.RecordStatus = RecordStatusEnum.Pause;
        }

        /// <summary>
        /// 继续录像
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        private void ResumeRecord()
        {
            if (this.recordStatus == RecordStatusEnum.Recording)
            {
                return;
            }

            switch (this.recordStatus)
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
                        this.RecordStatus = RecordStatusEnum.Recording;
                        break;
                    }

                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// 发送到所有窗口
        /// </summary>
        private void SendToAll()
        {
            foreach (ShellSessionVM shellSession in MTermApp.Context.MainWindowVM.ShellSessions)
            {
                if (shellSession == this)
                {
                    continue;
                }

                SyncInputSessionVM syncInput = this.SyncInputSessions.FirstOrDefault(v => v.ID == shellSession.ID);
                if (syncInput == null)
                {
                    syncInput = new SyncInputSessionVM()
                    {
                        ID = shellSession.ID,
                        Name = shellSession.Name,
                        Description = shellSession.Description
                    };
                    this.SyncInputSessions.Add(syncInput);
                }
            }
        }

        #endregion
    }

    public class ShellSessionContextMenu
    {
        public ShellSessionContextMenu()
        {
        }

        private void ContextMenuOpenPortForwardWindow_Click(ContextMenuVM sender, ShellSessionVM shellSessionVM)
        {
            PortForwardStatusWindow portForwardStatusWindow = new PortForwardStatusWindow(shellSessionVM);
            portForwardStatusWindow.Owner = App.Current.MainWindow;
            portForwardStatusWindow.Show();
        }

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

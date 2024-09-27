using DotNEToolkit;
using Microsoft.Win32;
using ModengTerm.Base;
using ModengTerm.Base.DataModels;
using ModengTerm.Base.Enumerations;
using ModengTerm.Document;
using ModengTerm.Document.Drawing;
using ModengTerm.Document.Enumerations;
using ModengTerm.ServiceAgents.DataModels;
using ModengTerm.Terminal.Callbacks;
using ModengTerm.Terminal.DataModels;
using ModengTerm.Terminal.Enumerations;
using ModengTerm.Terminal.Loggering;
using ModengTerm.Terminal.Parsing;
using ModengTerm.Terminal.Session;
using ModengTerm.Terminal.Windows;
using ModengTerm.ViewModels;
using ModengTerm.ViewModels.Terminals;
using ModengTerm.Windows;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using WPFToolkit.MVVM;
using WPFToolkit.Utility;

namespace ModengTerm.Terminal.ViewModels
{
    public class ShellContextMenu : ViewModelBase
    {
        private bool canChecked;
        private bool isChecked;

        public BindableCollection<ShellContextMenu> Children { get; set; }

        public bool CanChecked
        {
            get { return this.canChecked; }
            set
            {
                if (this.canChecked != value)
                {
                    this.canChecked = value;
                    this.NotifyPropertyChanged("CanChecked");
                }
            }
        }

        public bool IsChecked
        {
            get { return this.isChecked; }
            set
            {
                if (this.isChecked != value)
                {
                    this.isChecked = value;
                    this.NotifyPropertyChanged("IsChecked");
                }
            }
        }

        public ExecuteShellFunctionCallback Execute { get; private set; }

        public ShellContextMenu(string name)
        {
            this.ID = Guid.NewGuid().ToString();
            this.Name = name;
        }

        public ShellContextMenu(string name, ExecuteShellFunctionCallback execute)
        {
            this.ID = Guid.NewGuid().ToString();
            this.Name = name;
            this.Execute = execute;
        }

        public ShellContextMenu(string name, ExecuteShellFunctionCallback execute, bool canChecked) :
            this(name, execute)
        {
            this.canChecked = canChecked;
        }
    }

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

        private RecordStatusEnum recordState;

        /// <summary>
        /// 是否正在运行
        /// </summary>
        private bool isRunning;

        /// <summary>
        /// 提供剪贴板功能
        /// </summary>
        private VTClipboard clipboard;

        private PlaybackStatusEnum playbackStatus;
        private PlaybackStream playbackStream;

        /// <summary>
        /// 数据流解析器
        /// </summary>
        private VTParser vtParser;
        private string uri;

        private int totalRows;

        private Visibility contextMenuVisibility;

        private bool sendCommandPanelVisible;

        private BindableCollection<ShellCommandVM> shellCommands;

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

        /// <summary>
        /// 该终端的菜单状态
        /// </summary>
        public BindableCollection<ShellContextMenu> FunctionMenus { get; private set; }

        /// <summary>
        /// 发送到所有窗口的委托，由外部赋值
        /// </summary>
        public SendToAllTerminalCallback SendToAllCallback { get; set; }

        /// <summary>
        /// 日志记录器
        /// </summary>
        public LoggerManager LoggerManager { get; set; }

        public IDocument MainDocument { get; set; }
        public IDocument AlternateDocument { get; set; }

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
        /// 是否显示发送命令窗口
        /// </summary>
        public bool SendCommandPanelVisible
        {
            get { return this.sendCommandPanelVisible; }
            set
            {
                if (this.sendCommandPanelVisible != value)
                {
                    this.sendCommandPanelVisible = value;
                    this.NotifyPropertyChanged("SendCommandPanelVisible");
                }
            }
        }

        /// <summary>
        /// 该会话的所有快捷命令
        /// </summary>
        public BindableCollection<ShellCommandVM> ShellCommands 
        {
            get { return this.shellCommands; }
            private set
            {
                if (this.shellCommands != value) 
                {
                    this.shellCommands = value;
                    this.NotifyPropertyChanged("ShellCommands");
                }
            }
        }

        #endregion

        #region 构造方法

        public ShellSessionVM(XTermSession session) :
            base(session)
        {
            this.HistoryCommands = new BindableCollection<string>();
        }

        #endregion

        #region OpenedSessionVM Member

        protected override int OnOpen()
        {
            this.recordState = RecordStatusEnum.Stop;
            this.writeEncoding = Encoding.GetEncoding(this.Session.GetOption<string>(OptionKeyEnum.SSH_WRITE_ENCODING));
            this.clipboard = new VTClipboard()
            {
                MaximumHistory = this.Session.GetOption<int>(OptionKeyEnum.TERM_MAX_CLIPBOARD_HISTORY)
            };

            this.ShellCommands = new BindableCollection<ShellCommandVM>();
            this.ShellCommands.AddRange(this.ServiceAgent.GetShellCommands(this.Session.ID).Select(v => new ShellCommandVM(v)));

            #region 初始化工具栏菜单

            this.ToolbarMenus.Add(new SMenuItem("端口转发", "pack://application:,,,/ModengTerm;component/Images/portForward.png", this.OpenPortForwardWindow));

            #endregion

            #region 初始化右键菜单

            BehaviorRightClicks brc = this.Session.GetOption<BehaviorRightClicks>(OptionKeyEnum.BEHAVIOR_RIGHT_CLICK);
            if (brc == BehaviorRightClicks.ContextMenu)
            {
                this.InitializeContextMenu();
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
                MainDocument = this.MainDocument
            };

            VideoTerminal videoTerminal = new VideoTerminal();
            videoTerminal.ViewportChanged += this.VideoTerminal_ViewportChanged;
            videoTerminal.LinePrinted += VideoTerminal_LinePrinted;
            videoTerminal.Initialize(options);
            this.videoTerminal = videoTerminal;

            this.vtParser = new VTParser();
            this.vtParser.DispatchHandler = this.videoTerminal;
            this.vtParser.Initialize();

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

            // 停止对终端的日志记录
            this.StopLogger();

            // 停止录制
            this.StopRecord();

            this.sessionTransport.StatusChanged -= this.SessionTransport_StatusChanged;
            this.sessionTransport.DataReceived -= this.SessionTransport_DataReceived;
            this.sessionTransport.Close();
            this.sessionTransport.Release();

            this.videoTerminal.ViewportChanged -= this.VideoTerminal_ViewportChanged;
            this.videoTerminal.Release();

            // 释放剪贴板
            this.clipboard.Release();

            this.vtParser.Release();

            this.isRunning = false;
        }

        #endregion

        #region 实例方法

        private void InitializeContextMenu()
        {
            this.FunctionMenus = new BindableCollection<ShellContextMenu>()
                {
                    new ShellContextMenu("复制", this.CopySelection),
                    new ShellContextMenu("粘贴", this.Paste),
                    new ShellContextMenu("全选", this.SelectAll),
                    new ShellContextMenu("查找...",  this.Find),
                    new ShellContextMenu("日志")
                    {
                        Children = new BindableCollection<ShellContextMenu>()
                        {
                            new ShellContextMenu("启动",  this.StartLogger),
                            new ShellContextMenu("停止",  this.StopLogger),
                            new ShellContextMenu("暂停",  this.PauseLogger),
                            new ShellContextMenu("继续",  this.ResumeLogger)
                        }
                    },
                    //new ShellFunctionMenu("查看剪贴板历史", this.ClipboardHistory),
                    //new ShellFunctionMenu("收藏夹")
                    //{
                    //    Children = new BindableCollection<ShellFunctionMenu>()
                    //    {
                    //        new ShellFunctionMenu("加入收藏夹", ShellFunctionEnum.AddFavorites, this.AddFavorites),
                    //        new ShellFunctionMenu("查看收藏夹", ShellFunctionEnum.FaviritesList, this.FaviritesList),
                    //    }
                    //},
                    //new ShellFunctionMenu("书签")
                    //{
                    //    Children = new BindableCollection<ShellFunctionMenu>()
                    //    {
                    //        new ShellFunctionMenu("新建书签", ShellFunctionEnum.AddBookmark, this.AddBookmark),
                    //        new ShellFunctionMenu("删除书签", ShellFunctionEnum.RemoveBookmark, this.RemoveBookmark),
                    //        new ShellFunctionMenu("查看书签列表", ShellFunctionEnum.BookmarkList, this.BookmarkList),
                    //        new ShellFunctionMenu("显示书签栏", ShellFunctionEnum.DisplayBookmark, this.DisplayBookmark),
                    //        new ShellFunctionMenu("隐藏书签栏", ShellFunctionEnum.HidenBookmark, this.HidenBookmark),
                    //    }
                    //},
                    //new ShellFunctionMenu("录屏")
                    //{
                    //    Children = new BindableCollection<ShellFunctionMenu>()
                    //    {
                    //        new ShellFunctionMenu("启动录制", this.StartRecord),
                    //        new ShellFunctionMenu("停止录制", this.StopRecord),
                    //        new ShellFunctionMenu("暂停录制", this.PauseRecord),
                    //        new ShellFunctionMenu("恢复录制", this.ResumeRecord),
                    //        new ShellFunctionMenu("打开回放", this.OpenRecord)
                    //    }
                    //},
                    new ShellContextMenu("保存")
                    {
                        Children = new BindableCollection<ShellContextMenu>()
                        {
                            new ShellContextMenu("选中内容", this.SaveSelection),
                            new ShellContextMenu("当前屏幕内容", this.SaveViewport),
                            new ShellContextMenu("所有内容", this.SaveAllDocument),
                        }
                    },
                    new ShellContextMenu("发送到所有会话", this.SendToAll, true),
                    //new ShellFunctionMenu("交互测试用例")
                    //{
                    //    Children = new BindableCollection<ShellFunctionMenu>()
                    //    {
                    //        new ShellFunctionMenu("记录", ShellFunctionEnum.SaveInteractve, this.SaveInteractive),
                    //    }
                    //},
                    //new ShellFunctionMenu("记录调试日志")
                    //{
                    //    Children = new BindableCollection<ShellFunctionMenu>()
                    //    {
                    //        new ShellFunctionMenu("Interactive"),
                    //        new ShellFunctionMenu("vttestCode"),
                    //        new ShellFunctionMenu("RawRead")
                    //    }
                    //}
                    new ShellContextMenu("关于", this.About)
                };
        }

        private ParagraphFormatEnum FilterIndex2FileType(int filterIndex)
        {
            switch (filterIndex)
            {
                case 1: return ParagraphFormatEnum.PlainText;
                case 2: return ParagraphFormatEnum.HTML;

                default:
                    throw new NotImplementedException();
            }
        }

        private void SaveToFile(ParagraphTypeEnum paragraphType)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "文本文件(*.txt)|*.txt|html文件(*.html)|*.html";
            saveFileDialog.FileName = string.Format("{0}_{1}", this.Session.Name, DateTime.Now.ToString(DateTimeFormat.yyyyMMddhhmmss));
            if ((bool)saveFileDialog.ShowDialog())
            {
                ParagraphFormatEnum fileType = this.FilterIndex2FileType(saveFileDialog.FilterIndex);

                try
                {
                    VTParagraph paragraph = this.videoTerminal.CreateParagraph(paragraphType, fileType);
                    File.WriteAllText(saveFileDialog.FileName, paragraph.Content);
                }
                catch (Exception ex)
                {
                    logger.Error("保存日志异常", ex);
                    MessageBoxUtils.Error("保存失败");
                }
            }
        }

        private string InitializeURI()
        {
            string uri = string.Empty;

            switch ((SessionTypeEnum)this.Session.Type)
            {
                case SessionTypeEnum.CommandLine:
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

        #endregion

        #region 公开接口

        /// <summary>
        /// 模拟用户输入发送数据
        /// </summary>
        /// <param name="keyInput">用户输入信息</param>
        public override void SendInput(VTKeyInput keyInput)
        {
            if (this.sessionTransport.Status != SessionStatusEnum.Connected)
            {
                return;
            }

            VTKeyboard keyboard = this.videoTerminal.Keyboard;

            byte[] bytes = keyboard.TranslateInput(keyInput);
            if (bytes == null)
            {
                return;
            }

            VTDebug.Context.WriteInteractive(VTSendTypeEnum.UserInput, bytes);

            // 这里输入的都是键盘按键
            int code = this.sessionTransport.Write(bytes);
            if (code != ResponseCode.SUCCESS)
            {
                logger.ErrorFormat("发送数据失败, {0}", ResponseCode.GetMessage(code));
                return;
            }

            this.videoTerminal.Renderer.OnInteractionStateChanged(InteractionStateEnum.UserInput);
        }

        /// <summary>
        /// 发送原始字节数据
        /// </summary>
        /// <param name="rawData"></param>
        public override void SendRawData(byte[] rawData)
        {
            int code = this.sessionTransport.Write(rawData);
            if (code != ResponseCode.SUCCESS)
            {
                logger.ErrorFormat("发送数据失败, {0}", ResponseCode.GetMessage(code));
                return;
            }

            this.videoTerminal.Renderer.OnInteractionStateChanged(InteractionStateEnum.UserInput);
        }

        /// <summary>
        /// 发送纯文本数据
        /// </summary>
        /// <param name="text"></param>
        public override void SendText(string text)
        {
            byte[] bytes = this.writeEncoding.GetBytes(text);
            int code = this.sessionTransport.Write(bytes);
            if (code != ResponseCode.SUCCESS)
            {
                logger.ErrorFormat("发送数据失败, {0}", ResponseCode.GetMessage(code));
                return;
            }

            this.videoTerminal.Renderer.OnInteractionStateChanged(InteractionStateEnum.UserInput);
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

        #endregion

        #region 事件处理器

        private void VideoTerminal_ViewportChanged(IVideoTerminal vt, int newRow, int newColumn)
        {
            this.ViewportRow = newRow;
            this.ViewportColumn = newColumn;
        }

        private void VideoTerminal_LinePrinted(IVideoTerminal vt, VTHistoryLine historyLine)
        {
            VTHistory history = vt.MainDocument.History;

            this.TotalRows = history.Lines;
        }

        private void SessionTransport_DataReceived(SessionTransport client, byte[] bytes, int size)
        {
            VTDebug.Context.WriteRawRead(bytes, size);

            // 窗口持续改变大小的时候可能导致Render和SizeChanged事件一起运行，产生多线程修改VTDocument的bug
            // 所以这里把Render放在UI线程处理
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                this.videoTerminal.Renderer.OnInteractionStateChanged(InteractionStateEnum.Receive);

                try
                {
                    this.videoTerminal.Render(bytes, size);
                }
                catch (Exception ex)
                {
                    logger.Error("Render异常", ex);
                }
            });

            switch (this.recordState)
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

        private void SessionTransport_StatusChanged(object client, SessionStatusEnum status)
        {
            logger.InfoFormat("会话状态发生改变, {0}", status);

            try
            {
                switch (status)
                {
                    case SessionStatusEnum.Connected:
                        {
                            break;
                        }

                    case SessionStatusEnum.Connecting:
                        {
                            break;
                        }

                    case SessionStatusEnum.ConnectError:
                        {
                            break;
                        }

                    case SessionStatusEnum.Disconnected:
                        {
                            break;
                        }

                    default:
                        throw new NotImplementedException();
                }
            }
            catch (Exception ex)
            {
                logger.Error("SessionTransport_StatusChanged异常", ex);
            }

            base.NotifyStatusChanged(status);
        }

        private void StartLogger()
        {
            LoggerOptionsWindow window = new LoggerOptionsWindow(this.videoTerminal);
            window.Owner = Window.GetWindow(this.Content);
            if ((bool)window.ShowDialog())
            {
                this.LoggerManager.Start(this.videoTerminal, window.Options);
            }
        }

        private void StopLogger()
        {
            this.LoggerManager.Stop(this.videoTerminal);
        }

        private void PauseLogger()
        {
            this.LoggerManager.Pause(this.videoTerminal);
        }

        private void ResumeLogger()
        {
            this.LoggerManager.Resume(this.videoTerminal);
        }

        /// <summary>
        /// 拷贝当前选中的内容到剪切板
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

        private void Paste()
        {
            string text = System.Windows.Clipboard.GetText();
            this.SendText(text);
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
            clipboardVM.SendToAllTerminalDlg = this.SendToAllCallback;

            ParagraphsWindow paragraphsWindow = new ParagraphsWindow(clipboardVM);
            paragraphsWindow.Title = "剪贴板历史";
            paragraphsWindow.Owner = Window.GetWindow(this.Content);
            paragraphsWindow.Show();
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
        /// 开始录像
        /// </summary>
        private void StartRecord()
        {
            if (this.recordState == RecordStatusEnum.Recording)
            {
                return;
            }

            RecordOptionsVM recordOptionsVM = new RecordOptionsVM();

            RecordOptionsWindow recordOptionsWindow = new RecordOptionsWindow();
            recordOptionsWindow.Owner = Window.GetWindow(this.Content);
            recordOptionsWindow.DataContext = recordOptionsVM;
            if ((bool)recordOptionsWindow.ShowDialog())
            {
                XTermSession playbackSession = JSONHelper.CloneObject<XTermSession, XTermSession>(this.Session);
                playbackSession.Type = (int)SessionTypeEnum.SSHPlayback;

                PlaybackFile playbackFile = new PlaybackFile()
                {
                    ID = Guid.NewGuid().ToString(),
                    Name = recordOptionsVM.FileName,
                    Session = playbackSession
                };

                // 先打开录像文件
                string filePath = TermUtils.GetPlaybackFilePath(playbackFile);
                this.playbackStream = new PlaybackStream();
                int code = this.playbackStream.OpenWrite(playbackFile);
                if (code != ResponseCode.SUCCESS)
                {
                    MTMessageBox.Error("打开录像文件失败, {0}", ResponseCode.GetMessage(code));
                    return;
                }

                // 然后保存录像记录
                code = this.ServiceAgent.AddPlaybackFile(playbackFile);
                if (code != ResponseCode.SUCCESS)
                {
                    MTMessageBox.Error("录制失败, 保存录制记录失败, {0}", ResponseCode.GetMessage(code));
                    this.playbackStream.Close();
                    return;
                }

                this.recordState = RecordStatusEnum.Recording;
            }
        }

        /// <summary>
        /// 停止录像
        /// </summary>
        private void StopRecord()
        {
            if (this.recordState == RecordStatusEnum.Stop)
            {
                return;
            }

            // TODO：此时文件可能正在被写入，PlaybackFile里做了异常处理，所以直接这么写
            // 需要优化
            this.playbackStream.Close();

            this.recordState = RecordStatusEnum.Stop;
        }

        /// <summary>
        /// 暂停录像
        /// </summary>
        private void PauseRecord()
        {
            if (this.recordState == RecordStatusEnum.Pause)
            {
                return;
            }

            this.recordState = RecordStatusEnum.Pause;
        }

        /// <summary>
        /// 继续录像
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        private void ResumeRecord()
        {
            if (this.recordState == RecordStatusEnum.Recording)
            {
                return;
            }

            switch (this.recordState)
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
                        this.recordState = RecordStatusEnum.Recording;
                        break;
                    }

                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// 打开录像
        /// </summary>
        private void OpenRecord()
        {
            OpenRecordWindow openRecordWindow = new OpenRecordWindow();
            openRecordWindow.ServiceAgent = this.ServiceAgent;
            openRecordWindow.Session = this.Session;
            openRecordWindow.DisplayAllPlaybackList = false;
            openRecordWindow.Owner = Window.GetWindow(this.Content);
            openRecordWindow.Show();

            openRecordWindow.InitializeWindow();
        }

        /// <summary>
        /// 查找
        /// </summary>
        public void Find()
        {
            FindWindowMgr.Show(this);
        }

        public void SaveViewport()
        {
            this.SaveToFile(ParagraphTypeEnum.Viewport);
        }

        public void SaveSelection()
        {
            this.SaveToFile(ParagraphTypeEnum.Selected);
        }

        public void SaveAllDocument()
        {
            this.SaveToFile(ParagraphTypeEnum.AllDocument);
        }

        private void SendToAll()
        {
            this.SendAll = !this.SendAll;
        }

        private void About()
        {
            AboutWindow aboutWindow = new AboutWindow();
            aboutWindow.Owner = Application.Current.MainWindow;
            aboutWindow.ShowDialog();
        }

        private void OpenPortForwardWindow()
        {
        }

        #endregion
    }
}

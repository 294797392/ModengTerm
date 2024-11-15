using DotNEToolkit;
using Microsoft.Win32;
using ModengTerm.Base;
using ModengTerm.Base.DataModels;
using ModengTerm.Base.Enumerations;
using ModengTerm.Base.Enumerations.Terminal;
using ModengTerm.Document;
using ModengTerm.Document.Drawing;
using ModengTerm.Document.Enumerations;
using ModengTerm.Terminal.Callbacks;
using ModengTerm.Terminal.DataModels;
using ModengTerm.Terminal.Enumerations;
using ModengTerm.Terminal.Loggering;
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
using System.Transactions;
using System.Windows;
using WPFToolkit.MVVM;
using WPFToolkit.Utility;

namespace ModengTerm.Terminal.ViewModels
{
    public class ShellSessionVM : InputSessionVM
    {
        #region 类变量

        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger("ShellSessionVM");

        private static readonly List<MenuDefinition> ToolPanels1 = new List<MenuDefinition>()
        {
            new MenuDefinition()
            {
                Name = "快捷命令",
                ClassName = "ModengTerm.UserControls.Terminals.ShellCommandUserControl, ModengTerm",
                Parameters = new Dictionary<string, object>()
                {
                    { "type", ToolPanelTypeEnum.QuickCommand }
                }
            },
        };

        private static readonly List<MenuDefinition> ToolPanels2 = new List<MenuDefinition>()
        {
            new MenuDefinition()
            {
                Name = "资源管理器",
                ClassName = "ModengTerm.UserControls.TerminalUserControls.ResourceManagerUserControl, ModengTerm",
                Parameters = new Dictionary<string, object>()
                {
                    { "type", ToolPanelTypeEnum.ResourceManager }
                }
            },
        };

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

        private RecordStatusEnum recordStatus;

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

        private string uri;

        private int totalRows;

        private Visibility contextMenuVisibility;

        private LoggerManager logMgr;

        private AutoCompletionVM autoCompletionVM;

        private bool inputPanelVisible;

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

        public IDocument MainDocument { get; set; }
        public IDocument AlternateDocument { get; set; }

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
        /// 该会话的所有快捷命令
        /// </summary>
        public BindableCollection<QuickCommandVM> ShellCommands { get; private set; }

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
        /// 窗格列表
        /// </summary>
        public BindableCollection<ToolPanelVM> Panels { get; private set; }

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
            this.logMgr = MTermApp.Context.LoggerManager;

            this.RecordStatus = RecordStatusEnum.Stop;
            this.writeEncoding = Encoding.GetEncoding(this.Session.GetOption<string>(OptionKeyEnum.SSH_WRITE_ENCODING));
            this.clipboard = new VTClipboard()
            {
                MaximumHistory = this.Session.GetOption<int>(OptionKeyEnum.TERM_MAX_CLIPBOARD_HISTORY)
            };

            this.ShellCommands = new BindableCollection<QuickCommandVM>();
            this.ShellCommands.AddRange(this.ServiceAgent.GetShellCommands(this.Session.ID).Select(v => new QuickCommandVM(v)));
            this.NotifyPropertyChanged("ShellCommands");
            this.SyncInputSessions = new BindableCollection<SyncInputSessionVM>();

            ToolPanelVM toolPanel1 = this.CreateToolPanelVM(ToolPanels1);
            ToolPanelVM toolPanel2 = this.CreateToolPanelVM(ToolPanels2);
            this.Panels = new BindableCollection<ToolPanelVM>();
            this.Panels.Add(toolPanel1);
            this.Panels.Add(toolPanel2);
            this.NotifyPropertyChanged("Panels");

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
            videoTerminal.Initialize(options);
            this.videoTerminal = videoTerminal;

            #endregion

            #region 加载自动完成列表功能

            this.AutoCompletionVM = new AutoCompletionVM();
            this.AutoCompletionVM.Initialize(this);
            this.AutoCompletionVM.Enabled = this.Session.GetOption<bool>(OptionKeyEnum.TERM_ADVANCE_AUTO_COMPLETION_ENABLED);

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

        protected override List<SessionContextMenu> GetContextMenus()
        {
            return new List<SessionContextMenu>()
            {
                new SessionContextMenu("编辑")
                {
                    Children = new BindableCollection<SessionContextMenu>()
                    {
                        new SessionContextMenu("查找", this.Find),
                        new SessionContextMenu("复制", this.CopySelection),
                        new SessionContextMenu("保存")
                        {
                            Children = new BindableCollection<SessionContextMenu>()
                            {
                                new SessionContextMenu("选中内容", this.SaveSelection),
                                new SessionContextMenu("当前屏幕内容", this.SaveViewport),
                                new SessionContextMenu("所有内容", this.SaveAllDocument)
                            }
                        },
                        new SessionContextMenu("添加到快捷命令列表", this.AddToQuickCommands),
                    },
                },

                new SessionContextMenu("查看")
                {
                    Children = new BindableCollection<SessionContextMenu>()
                    {
                        new SessionContextMenu("资源管理器", new ExecuteShellFunctionCallback(()=>{ this.SwitchPanelVisible(ToolPanelTypeEnum.ResourceManager); })),
                        new SessionContextMenu("快捷命令", new ExecuteShellFunctionCallback(()=>{ this.SwitchPanelVisible(ToolPanelTypeEnum.QuickCommand); })),
                        new SessionContextMenu("输入栏", this.SwitchInputPanelVisible)
                    }
                },

                new SessionContextMenu("配置")
                {
                    Children = new BindableCollection<SessionContextMenu>()
                    {
                        new SessionContextMenu("端口转发", this.OpenPortForwardWindow),
                        new SessionContextMenu("同步输入", this.OpenSyncInputConfigurationWindow),
                        new SessionContextMenu("快捷命令", this.OpenCreateShellCommandWindow)
                    }
                },

                new SessionContextMenu("工具")
                {
                    Children = new BindableCollection<SessionContextMenu>()
                    {
                        new SessionContextMenu("日志")
                        {
                            Children = new BindableCollection<SessionContextMenu>()
                            {
                                new SessionContextMenu("开始", this.StartLogger),
                                new SessionContextMenu("停止", this.StartLogger)
                            }
                        },
                        new SessionContextMenu("录制")
                        {
                            Children = new BindableCollection<SessionContextMenu>()
                            {
                                new SessionContextMenu("开始", this.StartRecord),
                                new SessionContextMenu("停止", this.StopRecord),
                                new SessionContextMenu("打开回放", this.OpenRecord)
                            }
                        }
                    }
                },
            };
        }

        #endregion

        #region 实例方法

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
                case SessionTypeEnum.WindowsConsole:
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
            if (this.sessionTransport.Status != SessionStatusEnum.Connected)
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

            this.videoTerminal.OnInteractionStateChanged(InteractionStateEnum.UserInput);
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
                    shellSession = MTermApp.Context.MainWindowVM.OpenedSessionsVM.ShellSessions.FirstOrDefault(v => v.ID == syncInput.ID);
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

        private ToolPanelVM CreateToolPanelVM(List<MenuDefinition> menus)
        {
            ToolPanelVM toolPanelVM = new ToolPanelVM();
            toolPanelVM.Initialize(menus);
            toolPanelVM.SelectedMenu = toolPanelVM.MenuItems.FirstOrDefault();

            foreach (ToolPanelItemVM panelItem in toolPanelVM.MenuItems)
            {
                panelItem.Type = panelItem.Parameters.GetValue<ToolPanelTypeEnum>("type");
                panelItem.OwnerPanel = toolPanelVM;
            }

            return toolPanelVM;
        }

        private bool FindToolPanelItem(ToolPanelTypeEnum panelType, out ToolPanelVM panel, out ToolPanelItemVM panelItem)
        {
            panel = null;
            panelItem = null;

            foreach (ToolPanelVM toolPanel in this.Panels)
            {
                panelItem = toolPanel.MenuItems.FirstOrDefault(v => v.Type == panelType);
                if (panelItem != null)
                {
                    panel = toolPanel;
                    break;
                }
            }

            if (panel == null || panelItem == null)
            {
                return false;
            }

            return true;
        }

        private void SwitchPanelVisible(ToolPanelTypeEnum panelType)
        {
            ToolPanelVM toolPanel = null;
            ToolPanelItemVM panelItem = null;

            if (!this.FindToolPanelItem(panelType, out toolPanel, out panelItem))
            {
                logger.ErrorFormat("SwitchPanelVisible失败, 未找到对应的Panel, {0}", panelType);
                return;
            }

            // 当前状态
            bool visible = false;

            if (toolPanel.Visible)
            {
                if (toolPanel.SelectedMenu == panelItem)
                {
                    visible = true;
                }
            }

            if (visible)
            {
                // 当前是显示状态，隐藏
                toolPanel.Visible = false;
            }
            else
            {
                // 当前是隐藏状态，显示
                toolPanel.Visible = true;
                toolPanel.SelectedMenu = null;
                toolPanel.InvokeWhenSelectionChanged(panelItem);
            }
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
                VTKeyboard keyboard = this.videoTerminal.Keyboard;

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
        /// 打开快捷命令编辑窗口
        /// </summary>
        public void OpenCreateShellCommandWindow()
        {
            CreateShellCommandWindow window = new CreateShellCommandWindow(this);
            window.Owner = App.Current.MainWindow;
            if ((bool)window.ShowDialog())
            {
                // 重新读取所有快捷命令刷新界面

                List<ShellCommand> shellCommands = MTermApp.Context.ServiceAgent.GetShellCommands(this.ID.ToString());

                this.ShellCommands.Clear();
                this.ShellCommands.AddRange(shellCommands.Select(v => new QuickCommandVM(v)));
            }
        }

        /// <summary>
        /// 打开同步输入配置窗口
        /// </summary>
        public void OpenSyncInputConfigurationWindow()
        {
            SendAllConfigurationWindow sendAllConfigurationWindow = new SendAllConfigurationWindow(this);
            sendAllConfigurationWindow.Owner = App.Current.MainWindow;
            sendAllConfigurationWindow.ShowDialog();
        }

        #endregion

        #region 事件处理器

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

        private void SessionTransport_DataReceived(SessionTransport client, byte[] bytes, int size)
        {
            VTDebug.Context.WriteRawRead(bytes, size);

            // 窗口持续改变大小的时候可能导致Render和SizeChanged事件一起运行，产生多线程修改VTDocument的bug
            // 所以这里把Render放在UI线程处理
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                this.videoTerminal.OnInteractionStateChanged(InteractionStateEnum.Receive);

                try
                {
                    this.videoTerminal.ProcessData(bytes, size);
                }
                catch (Exception ex)
                {
                    logger.Error("Render异常", ex);
                }
            });

            this.HandleRecord(bytes, size);
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

        public void StartLogger()
        {
            LoggerOptionsWindow window = new LoggerOptionsWindow(this);
            window.Owner = Window.GetWindow(this.Content);
            if ((bool)window.ShowDialog())
            {
                this.logMgr.Start(this.videoTerminal, window.Options);
            }
        }

        public void StopLogger()
        {
            this.logMgr.Stop(this.videoTerminal);
        }

        public void PauseLogger()
        {
            this.logMgr.Pause(this.videoTerminal);
        }

        public void ResumeLogger()
        {
            this.logMgr.Resume(this.videoTerminal);
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
        public void StartRecord()
        {
            if (this.recordStatus == RecordStatusEnum.Recording)
            {
                return;
            }

            RecordOptionsVM recordOptionsVM = new RecordOptionsVM();

            RecordOptionsWindow recordOptionsWindow = new RecordOptionsWindow();
            recordOptionsWindow.Owner = Window.GetWindow(this.Content);
            recordOptionsWindow.DataContext = recordOptionsVM;
            if ((bool)recordOptionsWindow.ShowDialog())
            {
                Playback playbackFile = new Playback()
                {
                    ID = Guid.NewGuid().ToString(),
                    Name = recordOptionsVM.FileName,
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
        /// 打开录像
        /// </summary>
        public void OpenRecord()
        {
            OpenRecordWindow openRecordWindow = new OpenRecordWindow(this.Session);
            openRecordWindow.Owner = Window.GetWindow(this.Content);
            openRecordWindow.Show();
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

        /// <summary>
        /// 发送到所有窗口
        /// </summary>
        private void SendToAll()
        {
            foreach (ShellSessionVM shellSession in MTermApp.Context.MainWindowVM.OpenedSessionsVM.ShellSessions)
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

        private void About()
        {
            AboutWindow aboutWindow = new AboutWindow();
            aboutWindow.Owner = Application.Current.MainWindow;
            aboutWindow.ShowDialog();
        }

        private void OpenPortForwardWindow()
        {
            if (this.Session.Type != (int)SessionTypeEnum.SSH)
            {
                MTMessageBox.Info("该会话没有转发信息");
                return;
            }

            PortForwardStatusWindow portForwardStatusWindow = new PortForwardStatusWindow(this);
            portForwardStatusWindow.Owner = App.Current.MainWindow;
            portForwardStatusWindow.Show();
        }

        /// <summary>
        /// 把当前选中的内容添加到快捷输入列表里
        /// </summary>
        private void AddToQuickCommands()
        {
            VTParagraph selectedParagraph = this.videoTerminal.CreateParagraph(ParagraphTypeEnum.Selected, ParagraphFormatEnum.PlainText);
            if (selectedParagraph.IsEmpty)
            {
                return;
            }

            ShellCommand shellCommand = new ShellCommand()
            {
                ID = Guid.NewGuid().ToString(),
                AutoCRLF = false,
                SessionId = this.Session.ID,
                Name = string.Format("未命名_{0}", DateTime.Now.ToString(DateTimeFormat.yyyyMMddhhmmss)),
                Type = (int)CommandTypeEnum.PureText,
                Command = selectedParagraph.Content
            };

            int code = this.ServiceAgent.AddShellCommand(shellCommand);
            if (code != ResponseCode.SUCCESS)
            {
                MTMessageBox.Info("新建快捷命令失败, {0}", code);
                return;
            }

            QuickCommandVM qcvm = new QuickCommandVM(shellCommand);
            this.ShellCommands.Add(qcvm);
        }

        private void SwitchInputPanelVisible()
        {
            this.InputPanelVisible = !this.InputPanelVisible;
        }

        #endregion
    }
}

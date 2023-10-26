using ModengTerm.Base.DataModels;
using ModengTerm.Base.Enumerations;
using ModengTerm.Terminal.Document;
using ModengTerm.Terminal.Enumerations;
using ModengTerm.Terminal.Loggering;
using ModengTerm.Terminal.Rendering;
using ModengTerm.Terminal.Session;
using ModengTerm.ViewModels;
using System.Text;
using XTerminal.Base;
using XTerminal.Base.Definitions;
using XTerminal.Base.Enumerations;
using XTerminal.Parser;

namespace ModengTerm.Terminal.ViewModels
{
    /// <summary>
    /// 处理虚拟终端的所有逻辑
    /// </summary>
    public class VideoTerminal : OpenedSessionVM, IVideoTerminal
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("VideoTerminal");

        private static readonly byte[] OS_OperatingStatusData = new byte[4] { 0x1b, (byte)'[', (byte)'0', (byte)'n' };
        private static readonly byte[] DA_DeviceAttributesData = new byte[7] { 0x1b, (byte)'[', (byte)'?', (byte)'1', (byte)':', (byte)'0', (byte)'c' };

        #endregion

        #region 公开事件

        /// <summary>
        /// 当某一行被完整打印之后触发
        /// </summary>
        public event Action<IVideoTerminal, VTHistoryLine> LinePrinted;

        public event Action<IVideoTerminal, VTDocument, VTDocument> DocumentChanged;

        #endregion

        #region 实例变量

        /// <summary>
        /// 与终端进行通信的信道
        /// </summary>
        private SessionTransport sessionTransport;

        /// <summary>
        /// 终端字符解析器
        /// </summary>
        private VTParser vtParser;

        /// <summary>
        /// 主缓冲区文档模型
        /// </summary>
        private VTDocument mainDocument;

        /// <summary>
        /// 备用缓冲区文档模型
        /// </summary>
        private VTDocument alternateDocument;

        /// <summary>
        /// 当前正在使用的文档模型
        /// </summary>
        private VTDocument activeDocument;

        /// <summary>
        /// UI线程上下文
        /// </summary>
        internal SynchronizationContext uiSyncContext;

        /// <summary>
        /// 当前终端行数
        /// </summary>
        private int viewportRow;

        /// <summary>
        /// 当前终端列数
        /// </summary>
        private int viewportColumn;

        #region Mouse

        /// <summary>
        /// 鼠标滚轮滚动一次，滚动几行
        /// </summary>
        private int scrollDelta;

        #endregion

        /// <summary>
        /// DECAWM是否启用
        /// </summary>
        private bool autoWrapMode;

        private bool xtermBracketedPasteMode;

        private IDrawingDocument mainDrawingDocument;
        private IDrawingDocument alternateDrawingDocument;
        private IDrawingDocument backgroundDrawingDocument;
        private VTWallpaper wallpaper;

        #region Termimal

        /// <summary>
        /// 终端大小显示方式
        /// </summary>
        private TerminalSizeModeEnum sizeMode;

        #endregion

        /// <summary>
        /// 是否正在运行
        /// </summary>
        private bool isRunning;

        /// <summary>
        /// 输入编码方式
        /// </summary>
        private Encoding outputEncoding;

        /// <summary>
        /// 提供终端屏幕的功能
        /// </summary>
        private IDrawingWindow drawingWindow;

        /// <summary>
        /// 根据当前电脑键盘的按键状态，转换成标准的ANSI控制序列
        /// </summary>
        private VTKeyboard keyboard;

        /// <summary>
        /// 终端颜色表
        /// ColorName -> RgbKey
        /// </summary>
        private VTColorTable colorTable;
        internal string foregroundColor;
        internal string backgroundColor;

        private bool sendAll;

        #endregion

        #region 属性

        /// <summary>
        /// 获取当前光标所在行
        /// </summary>
        private VTextLine ActiveLine { get { return this.activeDocument.ActiveLine; } }

        /// <summary>
        /// 获取当前光标所在行
        /// </summary>
        private int CursorRow { get { return this.Cursor.Row; } }

        /// <summary>
        /// 获取当前光标所在列
        /// 下一个字符要显示的位置
        /// </summary>
        private int CursorCol { get { return this.Cursor.Column; } }

        /// <summary>
        /// activeDocument的光标信息
        /// 该坐标是基于ViewableDocument的坐标
        /// Cursor的位置是下一个要打印的字符的位置
        /// </summary>
        public VTCursor Cursor { get { return this.activeDocument.Cursor; } }

        public VTScrollInfo ScrollInfo { get { return this.activeDocument.Scrollbar; } }

        public VTDocument ActiveDocument { get { return this.activeDocument; } }

        /// <summary>
        /// 可视区域内的行数
        /// </summary>
        public int ViewportRow
        {
            get
            {
                return this.viewportRow;
            }
            private set
            {
                if (this.viewportRow != value)
                {
                    this.viewportRow = value;
                    this.NotifyPropertyChanged("ViewportRow");
                }
            }
        }

        /// <summary>
        /// 可视区域内的列数
        /// </summary>
        public int ViewportColumn
        {
            get
            {
                return this.viewportColumn;
            }
            private set
            {
                if (this.viewportColumn != value)
                {
                    this.viewportColumn = value;
                    this.NotifyPropertyChanged("ViewportColumn");
                }
            }
        }

        /// <summary>
        /// 是否发送到所有终端
        /// </summary>
        public bool SendAll
        {
            get { return this.sendAll; }
            set
            {
                if (this.sendAll != value)
                {
                    this.sendAll = value;
                    this.NotifyPropertyChanged("SendAll");
                }
            }
        }

        public VTWallpaper Background { get { return this.wallpaper; } }

        /// <summary>
        /// UI线程上下文对象
        /// </summary>
        public SynchronizationContext UISyncContext { get { return this.uiSyncContext; } }

        public VTLogger Logger { get; set; }

        #endregion

        #region 构造方法

        public VideoTerminal(XTermSession session) :
            base(session)
        {
        }

        #endregion

        #region 公开接口

        /// <summary>
        /// 初始化终端模拟器
        /// </summary>
        /// <param name="sessionInfo"></param>
        protected override int OnOpen()
        {
            XTermSession sessionInfo = this.Session;

            this.uiSyncContext = SynchronizationContext.Current;
            this.drawingWindow = this.Content as IDrawingWindow;

            // DECAWM
            this.autoWrapMode = false;

            // 初始化变量

            this.isRunning = true;

            this.outputEncoding = Encoding.GetEncoding(sessionInfo.GetOption<string>(OptionKeyEnum.WRITE_ENCODING));
            this.scrollDelta = sessionInfo.GetOption<int>(OptionKeyEnum.MOUSE_SCROLL_DELTA);
            this.colorTable = sessionInfo.GetOption<VTColorTable>(OptionKeyEnum.SSH_TEHEM_COLOR_TABLE);
            this.foregroundColor = sessionInfo.GetOption<string>(OptionKeyEnum.SSH_THEME_FORE_COLOR);
            this.backgroundColor = sessionInfo.GetOption<string>(OptionKeyEnum.SSH_THEME_BACK_COLOR);

            #region 初始化键盘

            this.keyboard = new VTKeyboard();
            this.keyboard.Encoding = this.outputEncoding;
            this.keyboard.SetAnsiMode(true);
            this.keyboard.SetKeypadMode(false);

            #endregion

            #region 初始化终端解析器

            this.vtParser = new VTParser();
            this.vtParser.ColorTable = this.colorTable;
            this.vtParser.ActionEvent += VtParser_ActionEvent;
            this.vtParser.Initialize();

            #endregion

            #region 初始化文档模型

            VTEventHandler eventHandler = new VTEventHandler()
            {
                OnScrollChanged = this.OnScrollChanged,
                OnSizeChanged = this.OnSizeChanged,
                OnMouseDown = this.OnMouseDown,
                OnMouseMove = this.OnMouseMove,
                OnMouseUp = this.OnMouseUp,
                OnMouseWheel = this.OnMouseWheel
            };

            this.mainDrawingDocument = this.drawingWindow.CreateCanvas();
            this.mainDrawingDocument.Name = "MainDocument";
            this.mainDrawingDocument.AddEventHandler(eventHandler);
            this.alternateDrawingDocument = this.drawingWindow.CreateCanvas();
            this.alternateDrawingDocument.Name = "AlternateDocument";
            this.alternateDrawingDocument.AddEventHandler(eventHandler);
            this.drawingWindow.InsertCanvas(0, this.mainDrawingDocument);
            this.drawingWindow.InsertCanvas(0, this.alternateDrawingDocument);
            this.drawingWindow.VisibleCanvas(this.mainDrawingDocument, true);
            this.drawingWindow.VisibleCanvas(this.alternateDrawingDocument, false);

            VTDocumentOptions mainOptions = this.CreateDocumentOptions(sessionInfo, false);
            this.mainDocument = new VTDocument(mainOptions, this.mainDrawingDocument, false)
            {
                Name = "MainDocument",
                ScrollbarVisible = true,
                BookmarkVisible = true,
            };
            this.mainDocument.Initialize();

            VTDocumentOptions alternateOptions = this.CreateDocumentOptions(sessionInfo, true);
            this.alternateDocument = new VTDocument(alternateOptions, this.alternateDrawingDocument, true)
            {
                Name = "AlternateDocument",
                ScrollbarVisible = false,
                BookmarkVisible = false,
            };
            this.alternateDocument.Initialize();
            this.activeDocument = this.mainDocument;

            // 初始化完VTDocument之后，真正要使用的Column和Row已经被计算出来了
            // 此时重新设置sessionInfo里的Row和Column，因为SessionTransport要使用
            // 行和列以主缓冲区为准
            sessionInfo.SetOption<int>(OptionKeyEnum.SSH_TERM_ROW, this.mainDocument.ViewportRow);
            sessionInfo.SetOption<int>(OptionKeyEnum.SSH_TERM_COL, this.mainDocument.ViewportColumn);

            this.ViewportRow = this.mainDocument.ViewportRow;
            this.ViewportColumn = this.mainDocument.ViewportColumn;

            #endregion

            #region 初始化光标

            this.activeDocument.Cursor.RequestInvalidate();
            this.alternateDocument.Cursor.RequestInvalidate();

            #endregion

            #region 初始化背景

            // 此时Inser(0)在Z顺序的最下面一层
            this.backgroundDrawingDocument = this.drawingWindow.CreateCanvas();
            this.backgroundDrawingDocument.Name = "BackgroundDocument";
            this.backgroundDrawingDocument.ScrollbarVisible = false;
            this.drawingWindow.InsertCanvas(0, this.backgroundDrawingDocument);

            this.wallpaper = new VTWallpaper(this.backgroundDrawingDocument)
            {
                PaperType = sessionInfo.GetOption<WallpaperTypeEnum>(OptionKeyEnum.SSH_THEME_BACKGROUND_TYPE),
                Uri = sessionInfo.GetOption<string>(OptionKeyEnum.SSH_THEME_BACKGROUND_URI),
                BackgroundColor = sessionInfo.GetOption<string>(OptionKeyEnum.SSH_THEME_BACK_COLOR),
                Effect = sessionInfo.GetOption<EffectTypeEnum>(OptionKeyEnum.SSH_THEME_BACKGROUND_EFFECT),
                Rect = new VTRect(0, 0, this.drawingWindow.GetSize())
            };
            this.wallpaper.Initialize();
            this.wallpaper.RequestInvalidate();

            #endregion

            #region 连接终端通道

            SessionTransport transport = new SessionTransport();
            transport.StatusChanged += this.SessionTransport_StatusChanged;
            transport.DataReceived += this.SessionTransport_DataReceived;
            transport.Initialize(sessionInfo);
            transport.OpenAsync();
            this.sessionTransport = transport;

            #endregion

            return ResponseCode.SUCCESS;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        protected override void OnClose()
        {
            this.isRunning = false;

            this.vtParser.ActionEvent -= VtParser_ActionEvent;
            this.vtParser.Release();

            this.sessionTransport.StatusChanged -= this.SessionTransport_StatusChanged;
            this.sessionTransport.DataReceived -= this.SessionTransport_DataReceived;
            this.sessionTransport.Close();
            this.sessionTransport.Release();

            this.mainDocument.Release();
            this.alternateDocument.Release();

            this.backgroundDrawingDocument.DeleteDrawingObjects();

            this.drawingWindow.RemoveDocument(this.mainDrawingDocument);
            this.drawingWindow.RemoveDocument(this.alternateDrawingDocument);
        }

        /// <summary>
        /// 获取当前使用鼠标选中的段落区域
        /// </summary>
        /// <returns></returns>
        public VTParagraph GetSelectedParagraph()
        {
            return this.activeDocument.CreateParagraph(ParagraphTypeEnum.Selected, LogFileTypeEnum.PlainText);
        }

        /// <summary>
        /// 选中全部的文本
        /// </summary>
        public void SelectAll()
        {
            this.activeDocument.SelectAll();
        }

        /// <summary>
        /// 创建指定的段落内容
        /// </summary>
        /// <param name="paragraphType">段落类型</param>
        /// <param name="fileType">要创建的内容格式</param>
        /// <returns></returns>
        public VTParagraph CreateParagraph(ParagraphTypeEnum paragraphType, LogFileTypeEnum fileType)
        {
            return this.activeDocument.CreateParagraph(paragraphType, fileType);
        }

        /// <summary>
        /// 向SSH主机发送用户输入
        /// 用户每输入一个字符，就调用一次这个函数
        /// </summary>
        /// <param name="input">用户输入信息</param>
        public void SendInput(UserInput input)
        {
            if (this.sessionTransport.Status != SessionStatusEnum.Connected)
            {
                return;
            }

            byte[] bytes = this.keyboard.TranslateInput(input);
            if (bytes == null)
            {
                return;
            }

            VTDebug.Context.WriteInteractive(VTSendTypeEnum.UserInput, bytes);

            // 这里输入的都是键盘按键
            int code = this.sessionTransport.Write(bytes);
            if (code != ResponseCode.SUCCESS)
            {
                logger.ErrorFormat("处理输入异常, {0}", ResponseCode.GetMessage(code));
            }
        }

        public void SendInput(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            byte[] bytes = this.outputEncoding.GetBytes(text);

            int code = this.sessionTransport.Write(bytes);
            if (code != ResponseCode.SUCCESS)
            {
                logger.ErrorFormat("粘贴数据失败, {0}", code);
            }
        }

        /// <summary>
        /// 滚动并重新渲染
        /// </summary>
        /// <param name="physicsRow">要滚动到的物理行数</param>
        public void ScrollTo(int physicsRow, ScrollOptions options = ScrollOptions.ScrollToTop)
        {
            if (this.activeDocument.IsAlternate)
            {
                return;
            }

            this.activeDocument.ScrollTo(physicsRow, options);
            this.activeDocument.RequestInvalidate();
        }

        /// <summary>
        /// 设置某一行的标签状态
        /// 如果该行所在的文档是备用缓冲区，那么什么都不做
        /// </summary>
        /// <param name="textLine">要设置的行</param>
        /// <param name="targetState">要设置的标签状态</param>
        public void AddBookmark(VTextLine textLine, VTBookmarkStates targetState)
        {
            VTDocument document = textLine.OwnerDocument;

            // 备用缓冲区不可以新建书签
            if (document.IsAlternate)
            {
                return;
            }

            if (textLine.BookmarkState == targetState)
            {
                return;
            }

            textLine.BookmarkState = targetState;

            // 重绘
            textLine.RequestInvalidate();

            // 更新历史行里的标签状态
            document.Scrollbar.UpdateHistory(textLine);
        }

        /// <summary>
        /// 设置书签的显示或隐藏状态
        /// </summary>
        /// <param name="visible"></param>
        public void SetBookmarkVisible(bool visible)
        {
            this.mainDocument.BookmarkVisible = visible;
        }

        /// <summary>
        /// 开始录制回放文件
        /// </summary>
        /// <param name="playbackFile">要录制的回放文件的名字</param>
        public void StartRecord(string playbackFile)
        {

        }

        /// <summary>
        /// 停止录制回放文件
        /// </summary>
        public void StopRecord()
        {

        }

        #endregion

        #region 实例方法

        private VTDocumentOptions CreateDocumentOptions(XTermSession sessionInfo, bool isAlternate)
        {
            string fontFamily = sessionInfo.GetOption<string>(OptionKeyEnum.SSH_THEME_FONT_FAMILY);
            double fontSize = sessionInfo.GetOption<double>(OptionKeyEnum.SSH_THEME_FONT_SIZE);

            VTypeface typeface = this.drawingWindow.GetTypeface(fontSize, fontFamily);
            typeface.BackgroundColor = sessionInfo.GetOption<string>(OptionKeyEnum.SSH_THEME_BACK_COLOR);
            typeface.ForegroundColor = sessionInfo.GetOption<string>(OptionKeyEnum.SSH_THEME_FORE_COLOR);

            VTDocumentOptions documentOptions = new VTDocumentOptions()
            {
                DefaultViewportColumn = sessionInfo.GetOption<int>(OptionKeyEnum.SSH_TERM_COL),
                DefaultViewportRow = sessionInfo.GetOption<int>(OptionKeyEnum.SSH_TERM_ROW),
                WindowSize = this.drawingWindow.GetSize(),
                DECPrivateAutoWrapMode = false,
                CursorStyle = sessionInfo.GetOption<VTCursorStyles>(OptionKeyEnum.SSH_THEME_CURSOR_STYLE),
                CursorColor = sessionInfo.GetOption<string>(OptionKeyEnum.SSH_THEME_CURSOR_COLOR),
                CursorSpeed = sessionInfo.GetOption<VTCursorSpeeds>(OptionKeyEnum.SSH_THEME_CURSOR_SPEED),
                ScrollDelta = sessionInfo.GetOption<int>(OptionKeyEnum.MOUSE_SCROLL_DELTA),
                ScrollbackMax = sessionInfo.GetOption<int>(OptionKeyEnum.TERM_MAX_SCROLLBACK),
                Typeface = typeface,
                SizeMode = sessionInfo.GetOption<TerminalSizeModeEnum>(OptionKeyEnum.SSH_TERM_SIZE_MODE),
                Session = sessionInfo,
                ContentMargin = sessionInfo.GetOption<double>(OptionKeyEnum.SSH_THEME_CONTENT_MARGIN),
                BookmarkColor = sessionInfo.GetOption<string>(OptionKeyEnum.SSH_BOOKMARK_COLOR)
            };

            if (isAlternate)
            {
                documentOptions.ScrollbarVisible = false;
                documentOptions.BookmarkVisible = false;
            }
            else 
            {
                documentOptions.ScrollbarVisible = true;
                documentOptions.BookmarkVisible = true;
            }

            return documentOptions;
        }

        private void PerformDeviceStatusReport(StatusType statusType)
        {
            switch (statusType)
            {
                case StatusType.OS_OperatingStatus:
                    {
                        // Result ("OK") is CSI 0 n
                        VTDebug.Context.WriteInteractive(VTActions.DSR_DeviceStatusReport, "{0}", statusType);
                        VTDebug.Context.WriteInteractive(VTSendTypeEnum.DSR_DeviceStatusReport, statusType, OS_OperatingStatusData);
                        this.sessionTransport.Write(OS_OperatingStatusData);
                        break;
                    }

                case StatusType.CPR_CursorPositionReport:
                    {
                        // 打开VIM后会收到这个请求

                        // 1,1 is the top - left corner of the viewport in VT - speak, so add 1
                        // Result is CSI ? r ; c R
                        int cursorRow = this.CursorRow + 1;
                        int cursorCol = this.CursorCol + 1;
                        VTDebug.Context.WriteInteractive(VTActions.DSR_DeviceStatusReport, "{0},{1},{2}", statusType, this.CursorRow, this.CursorCol);
                        string cprData = string.Format("\x1b[{0};{1}R", cursorRow, cursorCol);
                        byte[] cprBytes = Encoding.ASCII.GetBytes(cprData);
                        VTDebug.Context.WriteInteractive(VTSendTypeEnum.DSR_DeviceStatusReport, statusType, cprBytes);
                        this.sessionTransport.Write(cprBytes);
                        break;
                    }

                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// 自动判断ch是多字节字符还是单字节字符，创建一个VTCharacter
        /// </summary>
        /// <param name="ch">多字节或者单字节的字符</param>
        /// <returns></returns>
        private VTCharacter CreateCharacter(object ch, VTextAttributeState attributeState)
        {
            if (ch is char)
            {
                // 说明是unicode字符
                // 如果无法确定unicode字符的类别，那么占1列，否则占2列
                int column = 2;
                char c = Convert.ToChar(ch);
                if ((c >= 0x2500 && c <= 0x259F) ||     //  Box Drawing, Block Elements
                    (c >= 0x2000 && c <= 0x206F))       // Unicode - General Punctuation
                {
                    // https://symbl.cc/en/unicode/blocks/box-drawing/
                    // https://unicodeplus.com/U+2500#:~:text=The%20unicode%20character%20U%2B2500%20%28%E2%94%80%29%20is%20named%20%22Box,Horizontal%22%20and%20belongs%20to%20the%20Box%20Drawing%20block.
                    // gotop命令会用到Box Drawing字符，BoxDrawing字符不能用宋体，不然渲染的时候界面会乱掉。BoxDrawing字符的范围是0x2500 - 0x257F
                    // 经过测试发现WindwosTerminal如果用宋体渲染top的话，界面也是乱的...

                    column = 1;
                }

                return VTCharacter.Create(c, column, attributeState);
            }
            else
            {
                // 说明是ASCII码可见字符
                return VTCharacter.Create(Convert.ToChar(ch), 1, attributeState);
            }
        }

        #endregion

        #region 事件处理器

        private void VtParser_ActionEvent(VTParser parser, VTActions action, object parameter)
        {
            switch (action)
            {
                case VTActions.Print:
                    {
                        // 根据测试得出结论：
                        // 在VIM模式下输入中文字符，VIM会自动把光标往后移动2列，所以判断VIM里一个中文字符占用2列的宽度
                        // 在正常模式下，如果遇到中文字符，也使用2列来显示
                        // 也就是说，如果终端列数一共是80，那么可以显示40个中文字符，显示完40个中文字符后就要换行

                        // 如果在shell里删除一个中文字符，那么会执行两次光标向后移动的动作，然后EraseLine - ToEnd
                        // 由此可得出结论，不论是VIM还是shell，一个中文字符都是按照占用两列的空间来计算的

                        // 用户输入的时候，如果滚动条没滚动到底，那么先把滚动条滚动到底
                        // 不然会出现在VTDocument当前的最后一行打印字符的问题
                        this.activeDocument.ScrollToBottom();

                        // 创建并打印新的字符
                        char ch = Convert.ToChar(parameter);
                        VTDebug.Context.WriteInteractive(action, "{0},{1},{2}", this.CursorRow, this.CursorCol, ch);
                        VTCharacter character = this.CreateCharacter(parameter, this.activeDocument.AttributeState);
                        this.activeDocument.PrintCharacter(this.ActiveLine, character, this.CursorCol);
                        this.activeDocument.SetCursor(this.CursorRow, this.CursorCol + character.ColumnSize);

                        break;
                    }

                case VTActions.CarriageReturn:
                    {
                        // CR
                        // 把光标移动到行开头
                        VTDebug.Context.WriteInteractive(action, "{0},{1}", this.CursorRow, this.CursorCol);
                        this.activeDocument.SetCursor(this.CursorRow, 0);
                        break;
                    }

                case VTActions.FF:
                case VTActions.VT:
                case VTActions.LF:
                    {
                        // LF
                        // 滚动边距会影响到LF（DECSTBM_SetScrollingRegion），在实现的时候要考虑到滚动边距

                        VTDebug.Context.WriteInteractive(action, "{0},{1}", this.CursorRow, this.CursorCol);

                        // 如果滚动条不在最底部，那么先把滚动条滚动到底
                        this.activeDocument.ScrollToBottom();

                        // 想像一下有一个打印机往一张纸上打字，当打印机想移动到下一行打字的时候，它会发出一个LineFeed指令，让纸往上移动一行
                        // LineFeed，字面意思就是把纸上的下一行喂给打印机使用
                        this.activeDocument.LineFeed();

                        // 换行之后记录历史行
                        // 注意用户可以输入Backspace键或者上下左右光标键来修改最新行的内容，所以最新一行的内容是实时变化的，目前的解决方案是在渲染整个文档的时候去更新最后一个历史行的数据
                        // MainScrrenBuffer和AlternateScrrenBuffer里的行分别记录
                        // AlternateScreenBuffer是用来给man，vim等程序使用的
                        // 暂时只记录主缓冲区里的数据，备用缓冲区需要考虑下怎么记录，因为VIM，Man等程序用的是备用缓冲区，用户是可以实时编辑缓冲区里的数据的
                        if (this.activeDocument == this.mainDocument)
                        {
                            // 1. 更新旧的最后一行的历史行数据
                            // 2. 创建新的历史行

                            VTextLine oldLastLine = this.ActiveLine.PreviousLine;
                            VTextLine newLastLine = this.ActiveLine;

                            VTScrollInfo scrollBar = this.mainDocument.Scrollbar;

                            // 更新旧的最后一行和新的最后一行的历史记录
                            VTHistoryLine oldHistoryLine = scrollBar.UpdateHistory(oldLastLine);
                            scrollBar.UpdateHistory(newLastLine);

                            #region 更新滚动条的值

                            // 滚动条滚动到底
                            // 计算滚动条可以滚动的最大值
                            int scrollMax = this.mainDocument.FirstLine.PhysicsRow;
                            if (scrollMax > 0)
                            {
                                scrollBar.ScrollMax = scrollMax;
                                scrollBar.ScrollValue = scrollMax;
                            }

                            #endregion

                            // 触发行被完全打印的事件
                            if (this.LinePrinted != null)
                            {
                                this.LinePrinted(this, oldHistoryLine);
                            }
                        }

                        break;
                    }

                case VTActions.RI_ReverseLineFeed:
                    {
                        VTDebug.Context.WriteInteractive(action, string.Empty);

                        // 和LineFeed相反，也就是把光标往上移一个位置
                        // 在用man命令的时候会触发这个指令
                        // 反向换行 – 执行\n的反向操作，将光标向上移动一行，维护水平位置，如有必要，滚动缓冲区 *
                        this.activeDocument.ReverseLineFeed();
                        break;
                    }

                case VTActions.PlayBell:
                    {
                        // 响铃
                        break;
                    }

                case VTActions.ForwardTab:
                    {
                        // 执行TAB键的动作（在当前光标位置处打印4个空格）
                        // 微软的terminal项目里说，如果光标在该行的最右边，那么再次执行TAB的时候光标会自动移动到下一行，目前先不这么做

                        VTDebug.Context.WriteInteractive(action, string.Empty);

                        int tabSize = 4;
                        this.activeDocument.SetCursor(this.CursorRow, this.CursorCol + tabSize);

                        break;
                    }

                #region Erase

                case VTActions.EL_EraseLine:
                    {
                        List<int> parameters = parameter as List<int>;
                        EraseType eraseType = (EraseType)VTParameter.GetParameter(parameters, 0, 0);
                        VTDebug.Context.WriteInteractive(action, "{0},{1},{2}", this.CursorRow, this.CursorCol, eraseType);
                        this.activeDocument.EraseLine(this.ActiveLine, this.CursorCol, eraseType);
                        break;
                    }

                case VTActions.ED_EraseDisplay:
                    {
                        List<int> parameters = parameter as List<int>;
                        EraseType eraseType = (EraseType)VTParameter.GetParameter(parameters, 0, 0);
                        VTDebug.Context.WriteInteractive(action, "{0},{1},{2}", this.CursorRow, this.CursorCol, eraseType);

                        switch (eraseType)
                        {
                            // In most terminals, this is done by moving the viewport into the scrollback, clearing out the current screen.
                            // top和clear指令会执行EraseType.All，在其他的终端软件里top指令不会清空已经存在的行，而是把已经存在的行往上移动
                            // 所以EraseType.All的动作和Scrollback一样执行
                            case EraseType.All:
                            case EraseType.Scrollback:
                                {
                                    if (this.activeDocument.IsAlternate)
                                    {
                                        // xterm-256color类型的终端VIM程序的PageDown和PageUp会执行EraseType.All
                                        // 所以这里把备用缓冲区和主缓冲区分开处理
                                        VTextLine next = this.activeDocument.FirstLine;
                                        while (next != null)
                                        {
                                            next.EraseAll();

                                            next = next.NextLine;
                                        }
                                    }
                                    else
                                    {
                                        // 相关命令：
                                        // MainDocument：clear
                                        // AlternateDocument：暂无

                                        // Erase Saved Lines
                                        // 模拟xshell的操作，把当前行移动到可视区域的第一行

                                        VTScrollInfo scrollBar = this.mainDocument.Scrollbar;

                                        VTextLine firstLine = this.activeDocument.FirstLine;
                                        VTextLine lastLine = this.activeDocument.LastLine;

                                        // 当前终端里显示的行数
                                        int lines = scrollBar.LastLine.PhysicsRow - firstLine.PhysicsRow;

                                        // 把当前终端里显示的行数全部放到滚动区域上面
                                        // 先做换行动作，换行完光标在往下的lines行
                                        for (int i = 0; i < lines; i++)
                                        {
                                            firstLine.EraseAll();
                                            this.activeDocument.MoveLine(firstLine, VTextLine.MoveOptions.MoveToLast);
                                            firstLine = this.activeDocument.FirstLine;
                                        }

                                        this.activeDocument.DirtyAll();

                                        int scrollMax = this.activeDocument.FirstLine.PhysicsRow;
                                        scrollBar.ScrollMax = scrollMax;
                                        scrollBar.ScrollValue = scrollMax;

                                        // 重新设置光标所在行的数据
                                        VTextLine cursorLine = this.activeDocument.FirstLine.FindNext(this.activeDocument.Cursor.Row);
                                        this.activeDocument.SetCursorPhysicsRow(cursorLine.PhysicsRow);
                                    }
                                    break;
                                }

                            default:
                                {
                                    this.activeDocument.EraseDisplay(this.ActiveLine, this.CursorCol, eraseType);
                                    break;
                                }
                        }

                        break;
                    }

                #endregion

                #region 光标操作

                // 下面的光标移动指令不能进行VTDocument的滚动
                // 光标的移动坐标是相对于可视区域内的坐标
                // 服务器发送过来的光标原点是从(1,1)开始的，我们程序里的是(0,0)开始的，所以要减1

                case VTActions.BS:
                    {
                        VTDebug.Context.WriteInteractive(action, "{0},{1}", this.CursorRow, this.CursorCol);
                        this.activeDocument.SetCursor(this.CursorRow, this.CursorCol - 1);
                        break;
                    }

                case VTActions.CursorBackward:
                    {
                        List<int> parameters = parameter as List<int>;
                        int n = VTParameter.GetParameter(parameters, 0, 1);
                        VTDebug.Context.WriteInteractive(action, "{0},{1},{2}", this.CursorRow, this.CursorCol, n);
                        this.activeDocument.SetCursor(this.CursorRow, this.CursorCol - n);
                        break;
                    }

                case VTActions.CUF_CursorForward:
                    {
                        List<int> parameters = parameter as List<int>;
                        int n = VTParameter.GetParameter(parameters, 0, 1);
                        VTDebug.Context.WriteInteractive(action, "{0},{1},{2}", this.CursorRow, this.CursorCol, n);
                        this.activeDocument.SetCursor(this.CursorRow, this.CursorCol + n);
                        break;
                    }

                case VTActions.CUU_CursorUp:
                    {
                        List<int> parameters = parameter as List<int>;
                        int n = VTParameter.GetParameter(parameters, 0, 1);
                        VTDebug.Context.WriteInteractive(action, "{0},{1},{2}", this.CursorRow, this.CursorCol, n);
                        this.activeDocument.SetCursor(this.CursorRow - n, this.CursorCol);
                        break;
                    }

                case VTActions.CUD_CursorDown:
                    {
                        List<int> parameters = parameter as List<int>;
                        int n = VTParameter.GetParameter(parameters, 0, 1);
                        VTDebug.Context.WriteInteractive(action, "{0},{1},{2}", this.CursorRow, this.CursorCol, n);
                        this.activeDocument.SetCursor(this.CursorRow + n, this.CursorCol);
                        break;
                    }

                case VTActions.VPA_VerticalLinePositionAbsolute:
                    {
                        // 绝对垂直行位置 光标在当前列中垂直移动到第 <n> 个位置
                        // 保持列不变，把光标移动到指定的行处
                        List<int> parameters = parameter as List<int>;
                        int row = VTParameter.GetParameter(parameters, 0, 1);
                        row = Math.Max(0, row - 1);
                        VTDebug.Context.WriteInteractive(action, "{0},{1},{2}", this.CursorRow, this.CursorCol, row);
                        this.activeDocument.SetCursor(row, this.CursorCol);
                        break;
                    }

                case VTActions.HVP_HorizontalVerticalPosition:
                case VTActions.CUP_CursorPosition:
                    {
                        List<int> parameters = parameter as List<int>;

                        int row = 0, col = 0;
                        if (parameters.Count == 2)
                        {
                            // VT的光标原点是(1,1)，我们程序里的是(0,0)，所以要减1
                            int newrow = parameters[0];
                            int newcol = parameters[1];

                            // 测试中发现在ubuntu系统上执行apt install或者apt remove命令，HVP会发送0列过来，这里处理一下，如果遇到参数是0，那么就直接变成0
                            row = newrow == 0 ? 0 : newrow - 1;
                            col = newcol == 0 ? 0 : newcol - 1;

                            // 对行和列做限制
                            if (row >= this.viewportRow)
                            {
                                row = viewportRow - 1;
                            }

                            if (col >= this.viewportColumn)
                            {
                                col = viewportColumn - 1;
                            }
                        }
                        else
                        {
                            // 如果没有参数，那么说明就是定位到原点(0,0)
                        }

                        VTDebug.Context.WriteInteractive(action, "{0},{1},{2},{3}", this.CursorRow, this.CursorCol, row, col);
                        this.activeDocument.SetCursor(row, col);
                        break;
                    }

                case VTActions.CHA_CursorHorizontalAbsolute:
                    {
                        List<int> parameters = parameter as List<int>;

                        // 将光标移动到当前行中的第n列
                        int n = VTParameter.GetParameter(parameters, 0, -1);
                        if (n == -1)
                        {
                            break;
                        }

                        VTDebug.Context.WriteInteractive(action, "{0},{1},{2}", this.CursorRow, this.CursorCol, n);

                        this.ActiveLine.PadColumns(n);
                        this.activeDocument.SetCursor(this.CursorRow, n - 1);
                        break;
                    }

                case VTActions.DECSC_CursorSave:
                    {
                        VTDebug.Context.WriteInteractive(action, "{0},{1}", this.CursorRow, this.CursorCol);

                        // 收到这个指令的时候把光标状态保存一下，等下次收到DECRC_CursorRestore再还原保存了的光标状态
                        this.activeDocument.CursorSave();
                        break;
                    }

                case VTActions.DECRC_CursorRestore:
                    {
                        VTDebug.Context.WriteInteractive(action, "{0},{1},{2},{3}", this.CursorRow, this.CursorCol, this.activeDocument.CursorState.Row, this.activeDocument.CursorState.Column);
                        this.activeDocument.CursorRestore();
                        break;
                    }

                #endregion

                #region 文本特效

                case VTActions.UnsetAll:
                    {
                        VTDebug.Context.WriteInteractive(action, string.Empty);

                        // 重置所有文本装饰
                        this.activeDocument.ClearAttribute();
                        break;
                    }

                case VTActions.Bold:
                case VTActions.BoldUnset:
                case VTActions.Underline:
                case VTActions.UnderlineUnset:
                case VTActions.Italics:
                case VTActions.ItalicsUnset:
                case VTActions.DoublyUnderlined:
                case VTActions.DoublyUnderlinedUnset:
                case VTActions.Foreground:
                case VTActions.ForegroundUnset:
                case VTActions.Background:
                case VTActions.BackgroundUnset:
                case VTActions.ReverseVideo:
                case VTActions.ReverseVideoUnset:
                    {
                        VTDebug.Context.WriteInteractive(action, "{0},{1},{2}", this.CursorRow, this.CursorCol, parameter == null ? string.Empty : parameter.ToString());

                        // 打开VIM的时候，VIM会在打印第一行的~号的时候设置验色，然后把剩余的行全部打印，也就是说设置一次颜色可以对多行都生效
                        // 所以这里要记录下如果当前有文本特效被设置了，那么在行改变的时候也需要设置文本特效
                        // 缓存下来，每次打印字符的时候都要对ActiveLine Apply一下

                        if (action == VTActions.ReverseVideo)
                        {
                            VTColor foreColor = VTColor.CreateFromRgbKey(this.backgroundColor);
                            VTColor backColor = VTColor.CreateFromRgbKey(this.foregroundColor);
                            this.activeDocument.SetAttribute(VTextAttributes.Background, true, backColor);
                            this.activeDocument.SetAttribute(VTextAttributes.Foreground, true, foreColor);
                        }
                        else
                        {
                            bool enabled;
                            VTextAttributes attribute = VTUtils.VTAction2TextAttribute(action, out enabled);
                            this.activeDocument.SetAttribute(attribute, enabled, parameter);
                        }
                        break;
                    }

                case VTActions.Faint:
                case VTActions.FaintUnset:
                case VTActions.CrossedOut:
                case VTActions.CrossedOutUnset:
                    {
                        logger.ErrorFormat(string.Format("未执行的VTAction, {0}", action));
                        break;
                    }

                #endregion

                #region DECPrivateMode

                case VTActions.DECANM_AnsiMode:
                    {
                        bool enable = Convert.ToBoolean(parameter);
                        VTDebug.Context.WriteInteractive(action, "{0}", enable);
                        this.keyboard.SetAnsiMode(enable);
                        break;
                    }

                case VTActions.DECCKM_CursorKeysMode:
                    {
                        bool enable = Convert.ToBoolean(parameter);
                        VTDebug.Context.WriteInteractive(action, "{0}", enable);
                        this.keyboard.SetCursorKeyMode(enable);
                        break;
                    }

                case VTActions.DECKPAM_KeypadApplicationMode:
                    {
                        VTDebug.Context.WriteInteractive(action, string.Empty);
                        this.keyboard.SetKeypadMode(true);
                        break;
                    }

                case VTActions.DECKPNM_KeypadNumericMode:
                    {
                        VTDebug.Context.WriteInteractive(action, string.Empty);
                        this.keyboard.SetKeypadMode(false);
                        break;
                    }

                case VTActions.DECAWM_AutoWrapMode:
                    {
                        bool enable = Convert.ToBoolean(parameter);
                        VTDebug.Context.WriteInteractive(action, "{0}", enable);
                        this.autoWrapMode = enable;
                        this.activeDocument.DECPrivateAutoWrapMode = enable;
                        break;
                    }

                case VTActions.XTERM_BracketedPasteMode:
                    {
                        this.xtermBracketedPasteMode = Convert.ToBoolean(parameter);
                        VTDebug.Context.WriteInteractive(action, "{0}", this.xtermBracketedPasteMode);
                        break;
                    }

                case VTActions.ATT610_StartCursorBlink:
                    {
                        bool enable = Convert.ToBoolean(parameter);
                        VTDebug.Context.WriteInteractive(action, "{0}", enable);
                        this.Cursor.AllowBlink = enable;
                        break;
                    }

                case VTActions.DECTCEM_TextCursorEnableMode:
                    {
                        bool enable = Convert.ToBoolean(parameter);
                        VTDebug.Context.WriteInteractive(action, "{0}", enable);
                        this.Cursor.IsVisible = enable;
                        break;
                    }

                #endregion

                #region 文本操作

                case VTActions.DCH_DeleteCharacter:
                    {
                        // 从指定位置删除n个字符，删除后的字符串要左对齐，默认删除1个字符
                        List<int> parameters = parameter as List<int>;
                        int count = VTParameter.GetParameter(parameters, 0, 1);
                        VTDebug.Context.WriteInteractive(action, "{0},{1},{2}", this.CursorRow, this.CursorCol, count);
                        this.activeDocument.DeleteCharacter(this.ActiveLine, this.CursorCol, count);
                        break;
                    }

                case VTActions.ICH_InsertCharacter:
                    {
                        // 相关命令：
                        // MainDocument：sudo apt install pstat，然后回车，最后按方向键上回到历史命令
                        // AlternateDocument：暂无

                        // Insert Ps (Blank) Character(s) (default = 1) (ICH).
                        // 在当前光标处插入N个空白字符,这会将所有现有文本移到右侧。 向右溢出屏幕的文本会被删除
                        // 目前没发现这个操作对终端显示有什么影响，所以暂时不实现
                        List<int> parameters = parameter as List<int>;
                        int count = VTParameter.GetParameter(parameters, 0, 1);
                        this.activeDocument.InsertCharacters(this.ActiveLine, this.CursorCol, count);
                        break;
                    }

                case VTActions.ECH_EraseCharacters:
                    {
                        // 从当前光标处用空格填充n个字符
                        // Erase Characters from the current cursor position, by replacing them with a space
                        List<int> parameters = parameter as List<int>;
                        int count = VTParameter.GetParameter(parameters, 0, 1);
                        this.ActiveLine.EraseRange(this.CursorCol, count);
                        break;
                    }

                case VTActions.IL_InsertLine:
                    {
                        // 将 <n> 行插入光标位置的缓冲区。 光标所在的行及其下方的行将向下移动。
                        List<int> parameters = parameter as List<int>;
                        int lines = VTParameter.GetParameter(parameters, 0, 1);
                        VTDebug.Context.WriteInteractive(action, "{0}", lines);
                        if (lines > 0)
                        {
                            this.activeDocument.InsertLines(this.ActiveLine, lines);
                        }
                        break;
                    }

                case VTActions.DL_DeleteLine:
                    {
                        // 从缓冲区中删除<n> 行，从光标所在的行开始。
                        List<int> parameters = parameter as List<int>;
                        int lines = VTParameter.GetParameter(parameters, 0, 1);
                        VTDebug.Context.WriteInteractive(action, "{0}", lines);
                        if (lines > 0)
                        {
                            this.activeDocument.DeleteLines(this.ActiveLine, lines);
                        }
                        break;
                    }

                #endregion

                #region 上下滚动

                case VTActions.SD_ScrollDown:
                    {
                        // Scroll down Ps lines (default = 1) (SD), VT420.

                        break;
                    }

                case VTActions.SU_ScrollUp:
                    {
                        break;
                    }

                #endregion

                case VTActions.UseAlternateScreenBuffer:
                    {
                        VTDebug.Context.WriteInteractive(action, string.Empty);

                        IDrawingDocument remove = this.mainDocument.DrawingObject;
                        IDrawingDocument add = this.alternateDocument.DrawingObject;
                        this.drawingWindow.VisibleCanvas(remove, false);
                        this.drawingWindow.VisibleCanvas(add, true);

                        // 这里只重置行数，在用户调整窗口大小的时候需要执行终端的Resize操作
                        this.alternateDocument.SetScrollMargin(0, 0);
                        this.alternateDocument.EraseAll();
                        this.activeDocument = this.alternateDocument;

                        this.alternateDocument.Selection.Reset();
                        this.DocumentChanged?.Invoke(this, this.mainDocument, this.alternateDocument);
                        break;
                    }

                case VTActions.UseMainScreenBuffer:
                    {
                        VTDebug.Context.WriteInteractive(action, string.Empty);

                        IDrawingDocument remove = this.alternateDocument.DrawingObject;
                        IDrawingDocument add = this.mainDocument.DrawingObject;
                        this.drawingWindow.VisibleCanvas(remove, false);
                        this.drawingWindow.VisibleCanvas(add, true);

                        this.mainDocument.DirtyAll();
                        this.activeDocument = this.mainDocument;

                        this.mainDocument.Selection.Reset();
                        this.DocumentChanged?.Invoke(this, this.alternateDocument, this.mainDocument);
                        break;
                    }

                case VTActions.DSR_DeviceStatusReport:
                    {
                        // DSR，参考https://invisible-island.net/xterm/ctlseqs/ctlseqs.html

                        List<int> parameters = parameter as List<int>;
                        StatusType statusType = (StatusType)Convert.ToInt32(parameters[0]);
                        this.PerformDeviceStatusReport(statusType);
                        break;
                    }

                case VTActions.DA_DeviceAttributes:
                    {
                        VTDebug.Context.WriteInteractive(action, string.Empty);
                        VTDebug.Context.WriteInteractive(VTSendTypeEnum.DA_DeviceAttributes, DA_DeviceAttributesData);
                        this.sessionTransport.Write(DA_DeviceAttributesData);
                        break;
                    }

                case VTActions.DECSTBM_SetScrollingRegion:
                    {
                        // 设置可滚动区域
                        // 不可以操作滚动区域以外的行，只能对滚动区域内的行进行操作
                        // 对于滚动区域的作用的解释，举个例子说明
                        // 比方说marginTop是1，marginBottom也是1
                        // 那么在执行LineFeed动作的时候，默认情况下，是把第一行挂到最后一行的后面，有了margin之后，就要把第二行挂到倒数第二行的后面
                        // ScrollMargin会对很多动作产生影响：LF，RI_ReverseLineFeed，DeleteLine，InsertLine

                        // 视频终端的规范里说，如果topMargin等于bottomMargin，或者bottomMargin大于屏幕高度，那么忽略这个指令
                        // 边距还会影响插入行 (IL) 和删除行 (DL)、向上滚动 (SU) 和向下滚动 (SD) 修改的行。

                        // Notes on DECSTBM
                        // * The value of the top margin (Pt) must be less than the bottom margin (Pb).
                        // * The maximum size of the scrolling region is the page size
                        // * DECSTBM moves the cursor to column 1, line 1 of the page
                        // * https://github.com/microsoft/terminal/issues/1849

                        // 当前终端屏幕可显示的行数量
                        int lines = this.viewportRow;

                        List<int> parameters = parameter as List<int>;
                        int topMargin = VTParameter.GetParameter(parameters, 0, 1);
                        int bottomMargin = VTParameter.GetParameter(parameters, 1, lines);

                        if (bottomMargin < 0 || topMargin < 0)
                        {
                            logger.ErrorFormat("DECSTBM_SetScrollingRegion参数不正确，忽略本次设置, topMargin = {0}, bottomMargin = {1}", topMargin, bottomMargin);
                            return;
                        }
                        if (topMargin >= bottomMargin)
                        {
                            logger.ErrorFormat("DECSTBM_SetScrollingRegion参数不正确，topMargin大于等bottomMargin，忽略本次设置, topMargin = {0}, bottomMargin = {1}", topMargin, bottomMargin);
                            return;
                        }
                        if (bottomMargin > lines)
                        {
                            logger.ErrorFormat("DECSTBM_SetScrollingRegion参数不正确，bottomMargin大于当前屏幕总行数, bottomMargin = {0}, lines = {1}", bottomMargin, lines);
                            return;
                        }

                        // 如果topMargin等于1，那么就表示使用默认值，也就是没有marginTop，所以当topMargin == 1的时候，marginTop改为0
                        int marginTop = topMargin == 1 ? 0 : topMargin;
                        // 如果bottomMargin等于控制台高度，那么就表示使用默认值，也就是没有marginBottom，所以当bottomMargin == 控制台高度的时候，marginBottom改为0
                        int marginBottom = lines - bottomMargin;
                        VTDebug.Context.WriteInteractive(action, "topMargin1 = {0}, bottomMargin1 = {1}, topMargin2 = {2}, bottomMargin2 = {3}", topMargin, bottomMargin, marginTop, marginBottom);
                        this.activeDocument.SetScrollMargin(marginTop, marginBottom);
                        break;
                    }

                case VTActions.DECSLRM_SetLeftRightMargins:
                    {
                        List<int> parameters = parameter as List<int>;
                        int leftMargin = VTParameter.GetParameter(parameters, 0, 0);
                        int rightMargin = VTParameter.GetParameter(parameters, 1, 0);

                        VTDebug.Context.WriteInteractive(action, "leftMargin = {0}, rightMargin = {1}", leftMargin, rightMargin);
                        logger.ErrorFormat("未实现DECSLRM_SetLeftRightMargins");
                        break;
                    }

                default:
                    {
                        throw new NotImplementedException(string.Format("未执行的VTAction, {0}", action));
                    }
            }
        }

        private void SessionTransport_DataReceived(SessionTransport client, byte[] bytes, int size)
        {
            VTDebug.Context.WriteRawRead(bytes, size);

            try
            {
                this.vtParser.ProcessCharacters(bytes, size);
            }
            catch (Exception ex)
            {
                logger.Error("ProcessCharacters异常", ex);
            }

            this.uiSyncContext.Send((o) =>
            {
            // 全部数据都处理完了之后，只渲染一次
                this.activeDocument.RequestInvalidate();
            }, null);

            // 更新最新的历史行
            // 解决当一次性打印多个字符的时候，不需要每打印一个字符就更新历史行，而是等所有字符都打印完了再更新
            // 不要在Print事件里保存历史记录，因为可能会连续触发多次Print事件
            // 触发完多次Print事件后，会最后触发一次PerformDrawing，在PerformDrawing完了再保存最后一行历史行
            if (this.activeDocument == this.mainDocument)
            {
                VTScrollInfo scrollBar = this.mainDocument.Scrollbar;
                scrollBar.UpdateHistory(this.ActiveLine);
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

                    case SessionStatusEnum.ConnectionError:
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

        public void OnMouseWheel(bool upper)
        {
            this.activeDocument.OnMouseWheel(upper);
        }

        public void OnMouseDown(VTPoint p, int clickCount)
        {
            this.activeDocument.OnMouseDown(p, clickCount);
        }

        public void OnMouseMove(VTPoint p)
        {
            this.activeDocument.OnMouseMove(p);
        }

        public void OnMouseUp(VTPoint p)
        {
            this.activeDocument.OnMouseUp(p);
        }

        /// <summary>
        /// 当窗口大小改变的时候触发
        /// </summary>
        /// <param name="vt"></param>
        /// <param name="contentSize">新的内存区域大小</param>
        public void OnSizeChanged(VTSize contentSize)
        {
            // 有可能在大小改变的时候还没连接上终端
            if (this.Status != SessionStatusEnum.Connected)
            {
                return;
            }

            // 重新设置文档大小
            this.mainDocument.Resize(contentSize);
            this.alternateDocument.Resize(contentSize);

            // 重绘背景
            VTSize windowSize = this.drawingWindow.GetSize();
            this.wallpaper.Rect = new VTRect(0, 0, windowSize);
            this.wallpaper.RequestInvalidate();

            // 给SSH主机发个Resiz指令
            this.sessionTransport.Resize(this.activeDocument.ViewportRow, this.activeDocument.ViewportColumn);

            // 更新界面
            this.ViewportRow = this.activeDocument.ViewportRow;
            this.ViewportColumn = this.activeDocument.ViewportColumn;
        }

        private void OnScrollChanged(int scrollValue)
        {
            if (this.activeDocument.IsAlternate)
            {
                return;
            }

            this.ScrollTo(scrollValue);
        }

        #endregion
    }
}
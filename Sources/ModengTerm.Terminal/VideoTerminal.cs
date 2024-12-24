using ModengTerm.Base;
using ModengTerm.Base.DataModels;
using ModengTerm.Base.Enumerations;
using ModengTerm.Base.Enumerations.Terminal;
using ModengTerm.Document;
using ModengTerm.Document.Drawing;
using ModengTerm.Document.Enumerations;
using ModengTerm.Document.Utility;
using ModengTerm.Terminal.Loggering;
using ModengTerm.Terminal.Parsing;
using ModengTerm.Terminal.Renderer;
using ModengTerm.Terminal.Session;
using System.Text;
using XTerminal.Base.Definitions;

namespace ModengTerm.Terminal
{
    public class VTOptions
    {
        public GraphicsInterface AlternateDocument { get; set; }

        public GraphicsInterface MainDocument { get; set; }

        /// <summary>
        /// 该终端所对应的Session
        /// </summary>
        public XTermSession Session { get; set; }

        /// <summary>
        /// 用来发送数据给主机（比如终端的窗口大小改变了）
        /// </summary>
        public SessionTransport SessionTransport { get; set; }

        /// <summary>
        /// 终端的宽度
        /// </summary>
        public double Width { get; set; }

        /// <summary>
        /// 终端的高度
        /// </summary>
        public double Height { get; set; }
    }

    /// <summary>
    /// 处理虚拟终端的所有逻辑
    /// 主缓冲区：可以滚动，保存历史记录
    /// 备用缓冲区：行和列是固定的
    /// </summary>
    public class VideoTerminal : IVideoTerminal
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("VideoTerminal");

        private static readonly byte[] OS_OperatingStatusData = new byte[4] { 0x1b, (byte)'[', (byte)'0', (byte)'n' };
        private static readonly string DA_DeviceAttributesData = "\u001b[?1;0c";
        private static readonly string DA2_SecondaryDeviceAttributes = "\u001b[>0;10;1c";
        private static readonly string VT200_MOUSE_MODE_DATA = "\u001b[M{0}{1}{2}";
        private static readonly string FOCUSIN_DATA = "\u001b[I";
        private static readonly string FOCUSOUT_DATA = "\u001b[O";

        #endregion

        #region 公开事件

        /// <summary>
        /// 当某一行被完整打印之后触发
        /// 如果同一行被多次打印（top命令），那么只会在第一次打印的时候触发
        /// </summary>
        public event Action<IVideoTerminal, bool, int, VTHistoryLine> OnLineFeed;

        /// <summary>
        /// 当切换显示文档之后触发
        /// IVideoTerminal：事件触发者
        /// VTDocument：oldDocument，切换之前显示的文档
        /// VTDocument：newDocument，切换之后显示的文档
        /// </summary>
        public event Action<IVideoTerminal, VTDocument, VTDocument> OnDocumentChanged;

        /// <summary>
        /// 当用户通过键盘输入数据的时候触发
        /// </summary>
        public event Action<IVideoTerminal, VTKeyboardInput> OnKeyboardInput;

        /// <summary>
        /// 打印一个字符的时候触发
        /// </summary>
        public event Action<IVideoTerminal> OnPrint;

        /// <summary>
        /// 当可视区域的行或列改变的时候触发
        /// </summary>
        public event Action<IVideoTerminal, int, int> OnViewportChanged;

        public event Action<IVideoTerminal, double, double> RequestChangeWindowSize;

        public event Action<IVideoTerminal, ASCIITable> OnC0ActionExecuted;

        #endregion

        #region 实例变量

        private SessionTransport sessionTransport;

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

        #region Mouse

        /// <summary>
        /// 鼠标滚轮滚动一次，滚动几行
        /// </summary>
        private int scrollDelta;

        #endregion

        /// <summary>
        /// 是否正在运行
        /// </summary>
        private bool isRunning;

        /// <summary>
        /// 输入编码方式
        /// </summary>
        private Encoding writeEncoding;

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

        private VTOptions vtOptions;

        /// <summary>
        /// G0,G1,G2,G3字符集
        /// </summary>
        private VTCharsetMap[] gsetList;
        private int glSetNumber;
        private int glSetNumberSingleShift = -1; // 只映射下一次使用的gsetNumber。如果为-1表示不映射。

        /// <summary>
        /// GL部分要使用的字符映射关系
        /// </summary>
        private VTCharsetMap glTranslationTable;
        private int grSetNumber;

        /// <summary>
        /// GR部分要使用的字符映射关系
        /// </summary>
        private VTCharsetMap grTranslationTable;

        private bool isApplicationMode;
        private bool isAnsiMode;
        /// <summary>
        /// DECAWM是否启用
        /// </summary>
        private bool autoWrapMode;
        private bool xtermBracketedPasteMode;
        private bool isVT200MouseMode;
        private bool isFocusEventMode;

        private bool acceptC1Control;

        private char lastPrintChar;

        /// <summary>
        /// 终端数据解析器
        /// </summary>
        private VTParser vtParser;

        /// <summary>
        /// 数据渲染器
        /// </summary>
        private VTermRenderer renderer;

        private BellPlayer bellPlayer;

        private bool clickToCursor;
        private VTextPointer textPointer;

        #endregion

        #region 属性

        public int ViewportRow { get { return this.activeDocument.ViewportRow; } }

        public int ViewportColumn { get { return this.activeDocument.ViewportColumn; } }

        /// <summary>
        /// 获取当前光标所在行
        /// </summary>
        public int CursorRow { get { return Cursor.Row; } }

        /// <summary>
        /// 获取当前光标所在列
        /// 下一个字符要显示的位置
        /// </summary>
        public int CursorCol { get { return Cursor.Column; } }

        /// <summary>
        /// 会话名字
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 获取当前光标所在行
        /// </summary>
        private VTextLine ActiveLine { get { return this.activeDocument.ActiveLine; } }

        /// <summary>
        /// activeDocument的光标信息
        /// 该坐标是基于ViewableDocument的坐标
        /// Cursor的位置是下一个要打印的字符的位置
        /// </summary>
        public VTCursor Cursor { get { return this.activeDocument.Cursor; } }

        public VTScrollInfo ScrollInfo { get { return this.activeDocument.Scrollbar; } }

        /// <summary>
        /// 当前正在使用的缓冲区
        /// </summary>
        public VTDocument ActiveDocument { get { return this.activeDocument; } }

        /// <summary>
        /// 备用缓冲区
        /// </summary>
        public VTDocument AlternateDocument { get { return this.alternateDocument; } }

        /// <summary>
        /// 主缓冲区
        /// </summary>
        public VTDocument MainDocument { get { return this.mainDocument; } }

        public VTLogger Logger { get; set; }

        /// <summary>
        /// 电脑按键和发送的数据的映射关系
        /// </summary>
        public VTKeyboard Keyboard { get { return keyboard; } }

        /// <summary>
        /// 当前显示的是否是备用缓冲区
        /// </summary>
        public bool IsAlternate { get { return this.activeDocument == this.alternateDocument; } }

        /// <summary>
        /// 终端所关联的Session
        /// </summary>
        public XTermSession Session { get { return this.vtOptions.Session; } }

        /// <summary>
        /// 终端使用的字体信息
        /// </summary>
        public VTypeface Typeface { get; private set; }

        /// <summary>
        /// 获取当前是否有选中区域
        /// </summary>
        public bool HasSelection
        {
            get
            {
                return !this.activeDocument.Selection.IsEmpty;
            }
        }

        /// <summary>
        /// 是否禁用响铃
        /// </summary>
        public bool DisableBell { get; set; }

        #endregion

        #region 构造方法

        public VideoTerminal()
        {
        }

        #endregion

        #region 公开接口

        /// <summary>
        /// 初始化终端模拟器
        /// </summary>
        /// <param name="sessionInfo"></param>
        public void Initialize(VTOptions options)
        {
            XTermSession sessionInfo = options.Session;

            vtOptions = options;

            #region 字符集映射设置

            // 设置默认的字符集映射
            // 参考:
            // terminal项目 - TerminalOutput构造函数
            // https://vt100.net/docs/vt220-rm/chapter4.html#F4-1
            // Character sets remain designated until the terminal receives another SCS sequence. All locking shifts remain active until the terminal receives another locking shift. Single shifts SS2 and SS3 are active for only the next single graphic character.
            this.gsetList = new VTCharsetMap[4]
            {
                VTCharsetMap.Ascii, VTCharsetMap.Ascii,
                VTCharsetMap.Latin1, VTCharsetMap.Latin1,
            };
            this.glTranslationTable = VTCharsetMap.Ascii;
            this.grTranslationTable = VTCharsetMap.Latin1;

            #endregion

            this.bellPlayer = VTermApp.Context.Factory.LookupModule<BellPlayer>();
            this.textPointer = new VTextPointer();

            // 初始化变量
            isRunning = true;

            sessionTransport = options.SessionTransport;

            Name = sessionInfo.Name;

            writeEncoding = Encoding.GetEncoding(sessionInfo.GetOption<string>(OptionKeyEnum.SSH_WRITE_ENCODING));
            scrollDelta = sessionInfo.GetOption<int>(OptionKeyEnum.MOUSE_SCROLL_DELTA);
            colorTable = sessionInfo.GetOption<VTColorTable>(OptionKeyEnum.TEHEM_COLOR_TABLE);
            foregroundColor = sessionInfo.GetOption<string>(OptionKeyEnum.THEME_FONT_COLOR);
            backgroundColor = sessionInfo.GetOption<string>(OptionKeyEnum.THEME_BACKGROUND_COLOR);
            this.autoWrapMode = sessionInfo.GetOption<bool>(OptionKeyEnum.TERM_ADVANCE_AUTO_WRAP_MODE, true); // DECAWM
            this.clickToCursor = sessionInfo.GetOption<bool>(OptionKeyEnum.TERM_ADVANCE_CLICK_TO_CURSOR, false);

            #region 初始化数据解析器

            this.vtParser = new VTParser();
            this.vtParser.OnC0Actions += VtParser_OnC0Actions;
            this.vtParser.OnESCActions += VtParser_OnESCActions;
            this.vtParser.OnCSIActions += VtParser_OnCSIActions;
            this.vtParser.OnPrint += VtParser_OnPrint;
            this.vtParser.Initialize();

            #endregion

            #region 初始化键盘

            keyboard = new VTKeyboard();
            keyboard.Encoding = writeEncoding;
            keyboard.SetAnsiMode(true);
            keyboard.SetKeypadMode(false);

            #endregion

            #region 初始化文档模型

            VTDocumentOptions mainOptions = this.CreateDocumentOptions("MainDocument", sessionInfo, options.MainDocument);
            this.mainDocument = new VTDocument(mainOptions);
            this.mainDocument.MouseDown += MainDocument_MouseDown;
            this.mainDocument.MouseUp += MainDocument_MouseUp;
            this.mainDocument.Initialize();
            this.mainDocument.History.Add(this.mainDocument.FirstLine.History);

            VTDocumentOptions alternateOptions = this.CreateDocumentOptions("AlternateDocument", sessionInfo, options.AlternateDocument);
            alternateOptions.RollbackMax = 0;
            this.alternateDocument = new VTDocument(alternateOptions);
            this.alternateDocument.MouseDown += MainDocument_MouseDown;
            this.alternateDocument.MouseUp += MainDocument_MouseUp;
            this.alternateDocument.Initialize();
            this.alternateDocument.SetVisible(false);

            this.activeDocument = this.mainDocument;
            this.Typeface = this.mainDocument.Typeface;

            // 初始化完VTDocument之后，真正要使用的Column和Row已经被计算出来并保存到了VTDocumentOptions里
            // 此时重新设置sessionInfo里的Row和Column，因为SessionTransport要使用
            sessionInfo.SetOption<int>(OptionKeyEnum.SSH_TERM_ROW, mainOptions.ViewportRow);
            sessionInfo.SetOption<int>(OptionKeyEnum.SSH_TERM_COL, mainOptions.ViewportColumn);

            this.OnViewportChanged?.Invoke(this, this.mainDocument.ViewportRow, this.mainDocument.ViewportColumn);

            #endregion

            #region 初始化背景

            // 此时Inser(0)在Z顺序的最下面一层
            //this.backgroundCanvas = this.drawingTerminal.CreateCanvas(0);
            //this.backgroundCanvas.Name = "BackgroundDocument";
            //this.backgroundCanvas.ScrollbarVisible = false;

            //this.wallpaper = new VTWallpaper(this.backgroundCanvas)
            //{
            //    PaperType = sessionInfo.GetOption<WallpaperTypeEnum>(OptionKeyEnum.SSH_THEME_BACKGROUND_TYPE),
            //    Uri = sessionInfo.GetOption<string>(OptionKeyEnum.SSH_THEME_BACKGROUND_URI),
            //    BackgroundColor = sessionInfo.GetOption<string>(OptionKeyEnum.SSH_THEME_BACK_COLOR),
            //    Effect = sessionInfo.GetOption<EffectTypeEnum>(OptionKeyEnum.SSH_THEME_BACKGROUND_EFFECT),
            //    Rect = new VTRect(0, 0, this.drawingTerminal.GetSize())
            //};
            //this.wallpaper.Initialize();
            //this.wallpaper.RequestInvalidate();

            #endregion

            #region 初始化渲染器

            this.renderer = this.CreateRenderer();
            this.renderer.Initialize();

            #endregion
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Release()
        {
            isRunning = false;

            this.vtParser.OnC0Actions -= VtParser_OnC0Actions;
            this.vtParser.OnESCActions -= VtParser_OnESCActions;
            this.vtParser.OnCSIActions -= VtParser_OnCSIActions;
            this.vtParser.OnPrint -= VtParser_OnPrint;
            this.vtParser.Release();

            this.renderer.Release();

            this.mainDocument.MouseDown -= MainDocument_MouseDown;
            this.mainDocument.MouseUp -= MainDocument_MouseUp;
            this.mainDocument.Release();
            this.alternateDocument.MouseDown -= MainDocument_MouseDown;
            this.alternateDocument.MouseUp -= MainDocument_MouseUp;
            this.alternateDocument.Release();
        }

        /// <summary>
        /// 选中全部的文本
        /// </summary>
        public void SelectAll()
        {
            if (this.IsAlternate)
            {
                activeDocument.SelectViewport();
            }
            else
            {
                activeDocument.SelectAll();
            }
        }

        public void UnSelectAll()
        {
            this.activeDocument.UnSelectAll();
            this.activeDocument.RequestInvalidate();
        }

        /// <summary>
        /// 创建指定的段落内容
        /// </summary>
        /// <param name="paragraphType">段落类型</param>
        /// <param name="fileType">要创建的内容格式</param>
        /// <returns></returns>
        public VTParagraph CreateParagraph(ParagraphTypeEnum paragraphType, ParagraphFormatEnum formatType)
        {
            // 所有要保存的行存储在这里
            int startColumn = -1, endColumn = -1;
            int firstPhysicsRow = -1, lastPhysicsRow = -1;

            switch (paragraphType)
            {
                case ParagraphTypeEnum.AllDocument:
                    {
                        startColumn = 0;
                        endColumn = this.ViewportColumn;

                        if (IsAlternate)
                        {
                            firstPhysicsRow = 0;
                            lastPhysicsRow = this.ViewportRow;
                        }
                        else
                        {
                            firstPhysicsRow = 0;
                            lastPhysicsRow = this.mainDocument.History.Lines;
                        }

                        break;
                    }

                case ParagraphTypeEnum.Viewport:
                    {
                        startColumn = 0;
                        endColumn = this.ViewportColumn;

                        if (this.IsAlternate)
                        {
                            firstPhysicsRow = 0;
                            lastPhysicsRow = this.ViewportRow;
                        }
                        else
                        {
                            firstPhysicsRow = this.activeDocument.FirstPhysicsRow;
                            lastPhysicsRow = this.activeDocument.LastPhysicsRow;
                        }

                        break;
                    }

                case ParagraphTypeEnum.Selected:
                    {
                        VTextSelection selection = this.activeDocument.Selection;
                        if (selection.IsEmpty)
                        {
                            return VTParagraph.Empty;
                        }

                        VTextPointer top = selection.TopPointer;
                        VTextPointer bottom = selection.BottomPointer;

                        if (top.PhysicsRow == bottom.PhysicsRow)
                        {
                            // 处理是同一行的情况
                            firstPhysicsRow = top.PhysicsRow;
                            lastPhysicsRow = top.PhysicsRow;
                            startColumn = top.ColumnIndex > bottom.ColumnIndex ? bottom.ColumnIndex : top.ColumnIndex;
                            endColumn = top.ColumnIndex > bottom.ColumnIndex ? top.ColumnIndex : bottom.ColumnIndex;
                        }
                        else
                        {
                            firstPhysicsRow = top.PhysicsRow;
                            lastPhysicsRow = bottom.PhysicsRow;
                            startColumn = top.ColumnIndex;
                            endColumn = bottom.ColumnIndex;
                        }

                        break;
                    }

                default:
                    throw new NotImplementedException();
            }

            VTParagraphOptions paragraphOptions = new VTParagraphOptions()
            {
                StartColumn = startColumn,
                EndColumn = endColumn,
                FirstPhysicsRow = firstPhysicsRow,
                LastPhysicsRow = lastPhysicsRow,
                FormatType = formatType
            };

            //logger.FatalFormat("start = {0},{1}, end = {2},{3}", firstPhysicsRow, startColumn, lastPhysicsRow, endColumn);

            return this.activeDocument.GetParagraph(paragraphOptions);
        }

        /// <summary>
        /// 滚动到指定位置并重新渲染
        /// </summary>
        /// <param name="physicsRow">要滚动到的物理行数</param>
        public void ScrollTo(int physicsRow, ScrollOptions options = ScrollOptions.ScrollToTop)
        {
            if (this.IsAlternate)
            {
                // 备用缓冲区不可以滚动
                return;
            }

            activeDocument.ScrollTo(physicsRow, options);
            activeDocument.RequestInvalidate();
        }

        /// <summary>
        /// 重置终端大小
        /// </summary>
        /// <param name="newSize">以像素为单位的新的终端大小</param>
        public void Resize(VTSize newSize)
        {
            if (this.sessionTransport.Status != SessionStatusEnum.Connected)
            {
                return;
            }

            int oldRow = this.ViewportRow, oldCol = this.ViewportColumn;
            int newRow = 0, newCol = 0;
            VTUtils.CalculateAutoFitSize(newSize, this.activeDocument.Typeface, out newRow, out newCol);

            if (newRow == oldRow && newCol == oldCol)
            {
                // 变化之后的行和列和现在的行和列一样，什么都不做
                return;
            }

            logger.InfoFormat("Resize, newRow = {0}, newCol = {1}, oldRow = {2}, oldCol = {3}", newRow, newCol, oldRow, oldCol);

            // 对Document执行Resize
            // 目前的实现在ubuntu下没问题，但是在Windows10操作系统上运行Windows命令行里的vim程序会有问题，可能是Windows下的vim程序兼容性导致的？暂时先这样
            // 遇到过一种情况：如果终端名称不正确，比如XTerm，那么当行数增加的时候，光标会移动到该行的最右边，终端名称改成xterm就没问题了

            this.MainDocumentResize(this.mainDocument, newRow, newCol);
            this.AlternateDocumentResize(this.alternateDocument, newRow, newCol);

            // 给HOST发送修改行列的请求，HOST会重绘终端
            // TODO：Resize之后在异步线程里修改VTDocument的时候，还没修改完，可能会再次触发Resize
            // 在数据处理线程和UI线程同时在修改VTDocument，导致多线程访问冲突
            this.sessionTransport.Resize(newRow, newCol);

            this.OnViewportChanged?.Invoke(this, newRow, newCol);
        }

        /// <summary>
        /// 渲染数据
        /// 该方法必须在UI线程调用
        /// </summary>
        /// <param name="bytes">要渲染的数据</param>
        /// <param name="size">要渲染的数据长度</param>
        public void ProcessData(byte[] bytes, int size)
        {
            VTDocument oldDocument = this.activeDocument;
            int oldScroll = oldDocument.Scrollbar.Value;

            // 执行动作之前先把光标滚动到可视区域内
            // 不然没法打印字符
            if (oldDocument == this.mainDocument)
            {
                VTCursor cursor = oldDocument.Cursor;
                if (oldDocument.OutsideViewport(cursor.PhysicsRow))
                {
                    oldDocument.ScrollTo(cursor.PhysicsRow);
                }
            }

            this.renderer.Render(bytes, size);

            // 全部数据都处理完了之后，只渲染一次
            // 处理控制序列的时候可能会动态切换当前活动的缓冲区
            // 所以这里要重新渲染当前活动的缓冲区
            VTDocument newDocument = this.activeDocument;
            newDocument.RequestInvalidate();

            if (oldDocument == newDocument)
            {
                int newScroll = oldDocument.Scrollbar.Value;
                if (newScroll != oldScroll)
                {
                    // 计算ScrollData
                    VTScrollData scrollData = this.GetScrollData(oldDocument, oldScroll, newScroll);
                    oldDocument.InvokeScrollChanged(scrollData);
                }
            }
        }

        /// <summary>
        /// 触发OnKeyboardInput事件
        /// </summary>
        /// <param name="kbdInput">事件参数</param>
        public void RaiseKeyboardInput(VTKeyboardInput kbdInput)
        {
            this.OnKeyboardInput?.Invoke(this, kbdInput);
        }

        /// <summary>
        /// 当焦点状态改变的时候触发
        /// </summary>
        public void FocusChanged(bool focused)
        {
            if (this.isFocusEventMode)
            {
                this.HandleFocusEventMode(focused);
            }
        }

        #endregion

        #region 实例方法

        /// <summary>
        /// 自动判断ch是多字节字符还是单字节字符，创建一个VTCharacter
        /// </summary>
        /// <param name="ch">多字节或者单字节的字符</param>
        /// <returns></returns>
        private VTCharacter CreateCharacter(char ch, VTextAttributeState attributeState)
        {
            if (ch > byte.MaxValue)
            {
                // 说明是unicode字符
                // 如果无法确定unicode字符的类别，那么占1列，否则占2列
                int column = 2;
                char c = Convert.ToChar(ch);
                if (c >= 0x2500 && c <= 0x259F ||     //  Box Drawing, Block Elements
                    c >= 0x2000 && c <= 0x206F)       // Unicode - General Punctuation
                {
                    // https://symbl.cc/en/unicode/blocks/box-drawing/
                    // https://unicodeplus.com/U+2500#:~:text=The%20unicode%20character%20U%2B2500%20%28%E2%94%80%29%20is%20named%20%22Box,Horizontal%22%20and%20belongs%20to%20the%20Box%20Drawing%20block.
                    // gotop命令会用到Box Drawing字符，BoxDrawing字符不能用宋体，不然渲染的时候界面会乱掉。BoxDrawing字符的范围是0x2500 - 0x257F
                    // 经过测试发现WindowsTerminal如果用宋体渲染top的话，界面也是乱的...

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

        private VTDocumentOptions CreateDocumentOptions(string name, XTermSession sessionInfo, GraphicsInterface graphicsInterface)
        {
            string fontFamily = sessionInfo.GetOption<string>(OptionKeyEnum.THEME_FONT_FAMILY);
            double fontSize = sessionInfo.GetOption<double>(OptionKeyEnum.THEME_FONT_SIZE);

            VTypeface typeface = graphicsInterface.GetTypeface(fontSize, fontFamily);
            typeface.BackgroundColor = sessionInfo.GetOption<string>(OptionKeyEnum.THEME_BACKGROUND_COLOR);
            typeface.ForegroundColor = sessionInfo.GetOption<string>(OptionKeyEnum.THEME_FONT_COLOR);

            VTSize displaySize = new VTSize(this.vtOptions.Width, this.vtOptions.Height);
            TerminalSizeModeEnum sizeMode = sessionInfo.GetOption<TerminalSizeModeEnum>(OptionKeyEnum.SSH_TERM_SIZE_MODE);

            int viewportRow = sessionInfo.GetOption<int>(OptionKeyEnum.SSH_TERM_ROW);
            int viewportColumn = sessionInfo.GetOption<int>(OptionKeyEnum.SSH_TERM_COL);
            if (sizeMode == TerminalSizeModeEnum.AutoFit)
            {
                /// 如果SizeMode等于Fixed，那么就使用DefaultViewportRow和DefaultViewportColumn
                /// 如果SizeMode等于AutoFit，那么动态计算行和列
                VTUtils.CalculateAutoFitSize(displaySize, typeface, out viewportRow, out viewportColumn);
            }

            VTDocumentOptions documentOptions = new VTDocumentOptions()
            {
                Name = name,
                ViewportRow = viewportRow,
                ViewportColumn = viewportColumn,
                AutoWrapMode = false,
                CursorStyle = sessionInfo.GetOption<VTCursorStyles>(OptionKeyEnum.THEME_CURSOR_STYLE),
                CursorColor = sessionInfo.GetOption<string>(OptionKeyEnum.THEME_CURSOR_COLOR),
                CursorSpeed = sessionInfo.GetOption<VTCursorSpeeds>(OptionKeyEnum.THEME_CURSOR_SPEED),
                ScrollDelta = sessionInfo.GetOption<int>(OptionKeyEnum.MOUSE_SCROLL_DELTA),
                RollbackMax = sessionInfo.GetOption<int>(OptionKeyEnum.TERM_MAX_ROLLBACK),
                Typeface = typeface,
                GraphicsInterface = graphicsInterface,
                SelectionColor = sessionInfo.GetOption<string>(OptionKeyEnum.THEME_SELECTION_COLOR)
            };

            return documentOptions;
        }

        /// <summary>
        /// 获取终端文档一共显示了多少行数据
        /// Windows控制台刚启动或者窗口放大之后，会把所有行都打印一遍空字符，但是实际上只显示了前几行（缩小然后放大窗口的时候）
        /// 这种情况下使用最后一个非空行作为总行数
        /// </summary>
        /// <param name="document"></param>
        /// <returns>文档总行数</returns>
        private int GetMaxRow(VTDocument document)
        {
            // Windows命令行窗口在放大之后，会打印空行，但是实际上只显示了前几行
            // 所以在这里使用最后一个非空行当做最后一行

            int lastRowWithText = this.GetLastRowWithText(document);

            return Math.Max(lastRowWithText, document.Cursor.PhysicsRow) + 1;
        }

        /// <summary>
        /// 获取最后一个非空行的物理行号
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        private int GetLastRowWithText(VTDocument document)
        {
            VTHistory history = document.History;

            int lastRowIndex = history.Lines - 1;

            while (true)
            {
                VTHistoryLine historyLine;
                if (history.TryGetHistory(lastRowIndex, out historyLine))
                {
                    if (historyLine.Characters.FirstOrDefault(v => v.Character != ' ') != null)
                    {
                        return lastRowIndex;
                    }

                    lastRowIndex--;
                }
                else
                {
                    return -1;
                }
            }
        }

        /// <summary>
        /// 控制台刚启动或者窗口放大之后，会把所有行都打印一遍空字符，但是实际上只显示了前几行（缩小然后放大窗口的时候）
        /// 这种情况下使用最后一个非空行作为总行数
        /// </summary>
        /// <param name="document"></param>
        private void ScrollToBottom(VTDocument document)
        {
            // 先获取总行数
            int maxRow = this.GetMaxRow(document);

            // 如果总行数比可视区域的行数多，那么说明需要滚动
            // 否则不需要滚动
            if (maxRow <= document.ViewportRow)
            {
                return;
            }

            document.ScrollToBottom();
        }

        /// <summary>
        /// 重置终端大小
        /// 模仿Xshell的做法：
        /// 1. 扩大行的时候，如果有滚动内容，那么显示滚动内容。如果没有滚动内容，则直接在后面扩大。
        /// 2. 减少行的时候，如果有滚动内容，那么减少滚动内容。
        /// 3. ActiveLine保持不变
        /// 调用这个函数的时候保证此时文档已经滚动到底
        /// 是否需要考虑scrollMargin?目前没考虑
        /// </summary>
        /// <param name="document"></param>
        /// <param name="newRow"></param>
        /// <param name="newCol"></param>
        private void MainDocumentResize(VTDocument document, int newRow, int newCol)
        {
            // 调整大小前先把光标滚动到可视区域
            // 因为重新调整大小之后，HOST会重新打印所有内容
            this.ScrollToBottom(document);

            int oldRow = document.ViewportRow;
            int oldCol = document.ViewportColumn;

            document.Resize(newRow, newCol);

            switch ((SessionTypeEnum)this.Session.Type)
            {
                case SessionTypeEnum.Localhost:
                    {
                        // 对Windows命令行做特殊处理
                        // Windows的命令行比较特殊，在窗口放大的时候，它不会把上面被隐藏的行显示出来，而是在下面增加了新的行

                        VTScrollInfo scrollInfo = document.Scrollbar;

                        // 更新滚动条信息
                        int maxRow = this.GetMaxRow(document);

                        logger.DebugFormat("Windows Commandline, maxRow = {0}, newRow = {1}", maxRow, newRow);

                        if (newRow > oldRow)
                        {
                            // 扩大

                        }
                        else
                        {
                            // 缩小
                            if (maxRow > newRow)
                            {
                                scrollInfo.Maximum = maxRow - newRow;
                                this.ScrollTo(scrollInfo.Maximum);
                            }
                        }

                        break;
                    }

                default:
                    {
                        VTScrollInfo scrollInfo = document.Scrollbar;

                        // 更新滚动条信息
                        int maxRow = this.GetMaxRow(document);

                        logger.DebugFormat("maxRow = {0}, newRow = {1}", maxRow, newRow);

                        if (maxRow > newRow)
                        {
                            scrollInfo.Maximum = maxRow - newRow;
                            this.ScrollTo(scrollInfo.Maximum);
                        }
                        else
                        {
                            this.ScrollTo(0);
                            scrollInfo.Maximum = 0;
                            scrollInfo.Value = 0;
                        }

                        break;
                    }
            }

            document.DeleteViewoprt();
        }

        private void AlternateDocumentResize(VTDocument document, int newRow, int newCol)
        {
            document.Resize(newRow, newCol);

            // 备用缓冲区，因为SSH主机会重新打印所有字符，所以清空所有文本
            document.DeleteViewoprt();
        }

        private VTCharsetMap Select94CharsetMap(ulong charset)
        {
            switch (charset)
            {
                case 'B':
                case '1': return VTCharsetMap.Ascii;
                case '0':
                case '2': return VTCharsetMap.DecSpecialGraphics;
                case 'A': return VTCharsetMap.BritishNrcs;
                case '4': return VTCharsetMap.DutchNrcs;
                case '5':
                case 'C': return VTCharsetMap.FinnishNrcs;
                case 'R': return VTCharsetMap.FrenchNrcs;
                case 'f': return VTCharsetMap.FrenchNrcsIso;
                case '9':
                case 'Q': return VTCharsetMap.FrenchCanadianNrcs;
                case 'K': return VTCharsetMap.GermanNrcs;
                case 'Y': return VTCharsetMap.ItalianNrcs;
                case '6':
                case 'E': return VTCharsetMap.NorwegianDanishNrcs;
                case '`': return VTCharsetMap.NorwegianDanishNrcsIso;
                case 'Z': return VTCharsetMap.SpanishNrcs;
                case '7':
                case 'H': return VTCharsetMap.SwedishNrcs;
                case '=': return VTCharsetMap.SwissNrcs;

                default:
                    {
                        throw new NotImplementedException(string.Format("未处理的94 charset, {0}", charset));
                    }
            }
        }

        private VTCharsetMap Select96CharsetMap(ulong charset)
        {
            switch (charset)
            {
                case 'A': return VTCharsetMap.Latin1;
                case 'B': return VTCharsetMap.Latin2;
                case 'L': return VTCharsetMap.LatinCyrillic;
                case 'F': return VTCharsetMap.LatinGreek;
                case 'H': return VTCharsetMap.LatinHebrew;
                case 'M': return VTCharsetMap.Latin5;

                default:
                    {
                        throw new NotImplementedException(string.Format("未处理的96 charset, {0}", charset));
                    }
            }
        }

        private char TranslateCharacter(char ch)
        {
            // 最终要打印的字符
            char chPrint = ch;

            // 首先根据GL和GR的映射状态转换要显示的字符
            if (ch >= 0x20 && ch <= 0x7F)
            {
                // 此时显示的是GL区域的字符
                if (this.glSetNumberSingleShift == 2 || this.glSetNumberSingleShift == 3)
                {
                    VTCharsetMap translationTable = this.gsetList[this.glSetNumberSingleShift];

                    chPrint = translationTable.TranslateCharacter(ch);

                    this.glSetNumberSingleShift = -1;
                }
                else
                {
                    chPrint = this.glTranslationTable.TranslateCharacter(ch);
                }
            }
            else if (ch >= 0xA0 && ch <= 0xFF)
            {
                // 此时显示的是GR区域的字符
                chPrint = this.grTranslationTable.TranslateCharacter(ch);
            }

            return chPrint;
        }

        /// <summary>
        /// 根据配置创建一个渲染器
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private VTermRenderer CreateRenderer()
        {
            XTermSession session = this.vtOptions.Session;

            RenderModeEnum renderMode = session.GetOption<RenderModeEnum>(OptionKeyEnum.TERM_ADVANCE_RENDER_MODE);

            switch (renderMode)
            {
                case RenderModeEnum.Default:
                    {
                        switch ((SessionTypeEnum)this.Session.Type)
                        {
                            case SessionTypeEnum.Tcp:
                                {
                                    // RawTcp默认使用文本显示
                                    return new TextRenderer(this);
                                }

                            default:
                                {
                                    return new VideoTerminalRenderer(this) { Parser = this.vtParser };
                                }
                        }
                    }

                case RenderModeEnum.Hexdump:
                    {
                        return new HexdumpRenderer(this);
                    }

                default:
                    {
                        throw new NotImplementedException();
                    }
            }
        }

        /// <summary>
        /// 代码参考自microsoft/terminal项目
        /// AdaptDispatch::SetGraphicsRendition
        /// 和terminal项目不同的地方是，这里会判断parameters里是否包含参数，如果不包含参数，那么它会被视为单个0参数
        /// 参考自：https://learn.microsoft.com/zh-cn/windows/console/console-virtual-terminal-sequences - 文本格式
        /// 
        /// SGR - Modifies the graphical rendering options applied to the next
        ///   characters written into the buffer.
        ///       - Options include colors, invert, underlines, and other "font style"
        ///         type options.
        /// </summary>
        /// <param name="parameters"></param>
        private void ApplyGraphicsOptions(List<int> parameters)
        {
            if (parameters.Count == 0)
            {
                // 如果未指定任何参数，它会被视为单个 0 参数
                // 0就表示设置为默认值
                this.activeDocument.ClearAttribute();
                return;
            }

            int size = parameters.Count;

            for (int i = 0; i < size; i++)
            {
                i += this.ApplyGraphicsOption(parameters, i);
            }
        }

        private int ApplyGraphicsOption(List<int> parameters, int optionIndex)
        {
            int paramIndex = optionIndex;
            GraphicsOptions options = (GraphicsOptions)parameters[optionIndex];

            VTDebug.Context.WriteInteractive(string.Format("SGR - {0}", options), string.Empty);

            int i = 0;

            switch (options)
            {
                case GraphicsOptions.Off:
                    {
                        // 重置所有文本装饰
                        activeDocument.ClearAttribute();
                        break;
                    }

                case GraphicsOptions.ForegroundDefault:
                    {
                        this.activeDocument.SetAttribute(VTextAttributes.Foreground, false, null);
                        break;
                    }

                case GraphicsOptions.BackgroundDefault:
                    {
                        this.activeDocument.SetAttribute(VTextAttributes.Background, false, null);
                        break;
                    }

                case GraphicsOptions.NotBoldOrFaint:
                    {
                        this.activeDocument.SetAttribute(VTextAttributes.Bold, false, null);
                        this.activeDocument.SetAttribute(VTextAttributes.Faint, false, null);
                        break;
                    }

                case GraphicsOptions.RGBColorOrFaint:
                    {
                        logger.ErrorFormat("Faint");
                        this.activeDocument.SetAttribute(VTextAttributes.Faint, true, null);
                        break;
                    }

                case GraphicsOptions.Negative:
                    {
                        // ReverseVideo
                        VTColor foreColor = VTColor.CreateFromRgbKey(backgroundColor);
                        VTColor backColor = VTColor.CreateFromRgbKey(foregroundColor);
                        activeDocument.SetAttribute(VTextAttributes.Background, true, backColor);
                        activeDocument.SetAttribute(VTextAttributes.Foreground, true, foreColor);
                        break;
                    }

                case GraphicsOptions.Positive:
                    {
                        // ReverseVideoUnset
                        activeDocument.SetAttribute(VTextAttributes.Background, false, null);
                        activeDocument.SetAttribute(VTextAttributes.Foreground, false, null);
                        break;
                    }

                case GraphicsOptions.BoldBright:
                    {
                        this.activeDocument.SetAttribute(VTextAttributes.Bold, true, null);
                        break;
                    }

                case GraphicsOptions.Italics:
                    {
                        this.activeDocument.SetAttribute(VTextAttributes.Italics, true, null);
                        break;
                    }

                case GraphicsOptions.NotItalics:
                    {
                        this.activeDocument.SetAttribute(VTextAttributes.Italics, false, null);
                        break;
                    }

                case GraphicsOptions.Underline:
                    {
                        this.activeDocument.SetAttribute(VTextAttributes.Underline, true, null);
                        break;
                    }

                case GraphicsOptions.DoublyUnderlined:
                    {
                        this.activeDocument.SetAttribute(VTextAttributes.DoublyUnderlined, true, null);
                        break;
                    }

                case GraphicsOptions.NoUnderline:
                    {
                        // UnderlineUnset
                        activeDocument.SetAttribute(VTextAttributes.Underline, false, null);

                        // DoublyUnderlineUnset
                        activeDocument.SetAttribute(VTextAttributes.DoublyUnderlined, false, null);
                        break;
                    }

                case GraphicsOptions.ForegroundBlack:
                case GraphicsOptions.ForegroundBlue:
                case GraphicsOptions.ForegroundGreen:
                case GraphicsOptions.ForegroundCyan:
                case GraphicsOptions.ForegroundRed:
                case GraphicsOptions.ForegroundMagenta:
                case GraphicsOptions.ForegroundYellow:
                case GraphicsOptions.ForegroundWhite:
                case GraphicsOptions.BrightForegroundBlack:
                case GraphicsOptions.BrightForegroundBlue:
                case GraphicsOptions.BrightForegroundGreen:
                case GraphicsOptions.BrightForegroundCyan:
                case GraphicsOptions.BrightForegroundRed:
                case GraphicsOptions.BrightForegroundMagenta:
                case GraphicsOptions.BrightForegroundYellow:
                case GraphicsOptions.BrightForegroundWhite:
                    {
                        VTColorIndex colorIndex = VTermUtils.GraphicsOptions2VTColorIndex(options);
                        this.activeDocument.SetAttribute(VTextAttributes.Foreground, true, this.colorTable.GetColor(colorIndex));
                        break;
                    }

                case GraphicsOptions.BackgroundBlack:
                case GraphicsOptions.BackgroundBlue:
                case GraphicsOptions.BackgroundGreen:
                case GraphicsOptions.BackgroundCyan:
                case GraphicsOptions.BackgroundRed:
                case GraphicsOptions.BackgroundMagenta:
                case GraphicsOptions.BackgroundYellow:
                case GraphicsOptions.BackgroundWhite:
                case GraphicsOptions.BrightBackgroundBlack:
                case GraphicsOptions.BrightBackgroundBlue:
                case GraphicsOptions.BrightBackgroundGreen:
                case GraphicsOptions.BrightBackgroundCyan:
                case GraphicsOptions.BrightBackgroundRed:
                case GraphicsOptions.BrightBackgroundMagenta:
                case GraphicsOptions.BrightBackgroundYellow:
                case GraphicsOptions.BrightBackgroundWhite:
                    {
                        VTColorIndex colorIndex = VTermUtils.GraphicsOptions2VTColorIndex(options);
                        this.activeDocument.SetAttribute(VTextAttributes.Background, true, this.colorTable.GetColor(colorIndex));
                        break;
                    }

                case GraphicsOptions.NotCrossedOut:
                case GraphicsOptions.CrossedOut:
                case GraphicsOptions.Steady:
                case GraphicsOptions.BlinkOrXterm256Index:
                case GraphicsOptions.RapidBlink:
                    {
                        logger.FatalFormat("未实现SGR, {0}", options);
                        break;
                    }

                case GraphicsOptions.BackgroundExtended:
                    {
                        VTColor extColor;
                        i += this.SetRgbColorsHelper(parameters, paramIndex + 1, out extColor);
                        this.activeDocument.SetAttribute(VTextAttributes.Background, true, extColor);
                        break;
                    }

                case GraphicsOptions.ForegroundExtended:
                    {
                        VTColor extColor;
                        i += this.SetRgbColorsHelper(parameters, paramIndex + 1, out extColor);
                        this.activeDocument.SetAttribute(VTextAttributes.Foreground, true, extColor);
                        break;
                    }

                default:
                    {
                        // TODO：
                        logger.ErrorFormat("未实现的SGR, {0}", options.ToString());
                        break;
                        //throw new NotImplementedException();
                    }
            }

            return i;
        }

        /// <summary>
        /// - Helper to parse extended graphics options, which start with 38 (FG) or 48 (BG)
        ///     These options are followed by either a 2 (RGB) or 5 (xterm index)
        ///      RGB sequences then take 3 MORE params to designate the R, G, B parts of the color
        ///      Xterm index will use the param that follows to use a color from the preset 256 color xterm color table.
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="paramIndex">从第几个参数开始使用</param>
        /// <param name="rgbColor"></param>
        /// <returns></returns>
        private int SetRgbColorsHelper(List<int> parameters, int paramIndex, out VTColor rgbColor)
        {
            rgbColor = null;

            int optionsConsumed = 1;
            GraphicsOptions options = (GraphicsOptions)parameters[paramIndex];
            if (options == GraphicsOptions.RGBColorOrFaint)
            {
                // 这里使用RGB颜色
                optionsConsumed = 4;
                rgbColor = VTColor.CreateFromRgb((byte)parameters[paramIndex + 1], (byte)parameters[paramIndex + 2], (byte)parameters[paramIndex + 3], 255);
            }
            else if (options == GraphicsOptions.BlinkOrXterm256Index)
            {
                // 这里使用xterm颜色表里的颜色
                optionsConsumed = 2;
                int tableIndex = parameters.Count > paramIndex ? parameters[paramIndex + 1] : 0;
                if (tableIndex <= 255)
                {
                    byte r, g, b;
                    byte index = (byte)tableIndex;
                    Xterm256Color.ConvertRGB(index, out r, out g, out b);
                    rgbColor = VTColor.CreateFromRgb(r, g, b, 255);
                }
            }
            return optionsConsumed;
        }

        /// <summary>
        /// 设置DECPrivateMode模式
        /// </summary>
        /// <param name="privateModes">要设置的模式列表</param>
        /// <param name="enable">启用或者禁用</param>
        private bool PerformDECPrivateMode(List<int> privateModes, bool enable)
        {
            bool success = false;

            foreach (int mode in privateModes)
            {
                switch ((DECPrivateMode)mode)
                {
                    case DECPrivateMode.DECCKM_CursorKeysMode:
                        {
                            // set - Enable Application Mode, reset - Normal mode

                            // true表示ApplicationMode
                            // false表示NormalMode
                            VTDebug.Context.WriteInteractive("DECCKM_CursorKeysMode", "{0}", enable);

                            this.isApplicationMode = enable;
                            this.keyboard.SetCursorKeyMode(enable);
                            break;
                        }

                    case DECPrivateMode.DECANM_AnsiMode:
                        {
                            VTDebug.Context.WriteInteractive("DECANM_AnsiMode", "{0}", enable);

                            this.isAnsiMode = enable;
                            this.keyboard.SetAnsiMode(enable);
                            break;
                        }

                    case DECPrivateMode.DECAWM_AutoWrapMode:
                        {
                            VTDebug.Context.WriteInteractive("DECAWM_AutoWrapMode", "{0}", enable);
                            this.autoWrapMode = enable;
                            this.activeDocument.AutoWrapMode = enable;
                            break;
                        }

                    case DECPrivateMode.ASB_AlternateScreenBuffer:
                        {
                            // 是否使用备用缓冲区
                            // 打开VIM等编辑器的时候会触发
                            if (enable)
                            {
                                // 使用备用缓冲区
                                VTDebug.Context.WriteInteractive("UseAlternateScreenBuffer", string.Empty);

                                this.mainDocument.SetVisible(false);
                                this.alternateDocument.SetVisible(true);

                                this.activeDocument = this.alternateDocument;

                                //this.OnDocumentChanged?.Invoke(this, this.mainDocument, this.alternateDocument);

                                logger.InfoFormat("UseAlternateScreenBuffer");
                            }
                            else
                            {
                                // 使用主缓冲区

                                VTDebug.Context.WriteInteractive("UseMainScreenBuffer", string.Empty);

                                this.mainDocument.SetVisible(true);
                                this.alternateDocument.SetVisible(false);

                                this.alternateDocument.SetScrollMargin(0, 0);
                                this.alternateDocument.EraseAll();
                                this.alternateDocument.SetCursorLogical(0, 0);
                                this.alternateDocument.ClearAttribute();
                                this.alternateDocument.Selection.Clear();

                                this.activeDocument = this.mainDocument;

                                //this.OnDocumentChanged?.Invoke(this, this.alternateDocument, this.mainDocument);

                                logger.InfoFormat("UseMainScreenBuffer, {0}", this.activeDocument.Name);
                            }
                            break;
                        }

                    case DECPrivateMode.XTERM_BracketedPasteMode:
                        {
                            // Sets the XTerm bracketed paste mode. This controls whether pasted content is bracketed with control sequences to differentiate it from typed text.
                            VTDebug.Context.WriteInteractive("XTERM_BracketedPasteMode", "{0}", enable);
                            this.xtermBracketedPasteMode = enable;
                            break;
                        }

                    case DECPrivateMode.ATT610_StartCursorBlink:
                        {
                            // 控制是否要闪烁光标
                            VTDebug.Context.WriteInteractive("ATT610_StartCursorBlink", "{0}", enable);
                            this.Cursor.AllowBlink = enable;
                            break;
                        }

                    case DECPrivateMode.DECTCEM_TextCursorEnableMode:
                        {
                            // 控制是否要显示光标
                            VTDebug.Context.WriteInteractive("DECTCEM_TextCursorEnableMode", "{0}", enable);
                            this.Cursor.IsVisible = enable;
                            break;
                        }

                    case DECPrivateMode.DECARM_AutoRepeatMode:
                        {
                            logger.FatalFormat("未实现DECARM_AutoRepeatMode");
                            break;
                        }

                    case DECPrivateMode.DECSCNM_ScreenMode:
                        {
                            // This control function selects a dark or light background on the screen.
                            // Default: Dark background.

                            // When DECSCNM is set, the screen displays dark characters on a light background.
                            // When DECSCNM is reset, the screen displays light characters on a dark background.

                            logger.FatalFormat("未实现DECSCNM_ScreenMode");

                            break;
                        }

                    case DECPrivateMode.VT200_MOUSE_MODE:
                        {
                            this.isVT200MouseMode = enable;
                            break;
                        }

                    case DECPrivateMode.FOCUS_EVENT_MODE:
                        {
                            this.isFocusEventMode = enable;
                            break;
                        }

                    default:
                        {
                            logger.FatalFormat("未实现DECPrivateMode, {0}, {1}", mode, enable);
                            break;
                        }
                }
            }

            return success;
        }

        /// <summary>
        /// Window Manipulation - Performs a variety of actions relating to the window,
        ///      such as moving the window position, resizing the window, querying
        ///      window state, forcing the window to repaint, etc.
        ///  This is kept separate from the input version, as there may be
        ///      codes that are supported in one direction but not the other.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="parameter1"></param>
        /// <param name="parameter2"></param>
        private void PerformWindowManipulation(List<int> parameters)
        {
            WindowManipulationType type = (WindowManipulationType)parameters[0];

            switch (type)
            {
                case WindowManipulationType.ResizeWindowInCharacters:
                    {
                        int newRow = VTParameter.GetParameter(parameters, 1, -1);
                        int newCol = VTParameter.GetParameter(parameters, 2, -1);

                        // 省略了参数
                        if (newRow == -1 || newCol == -1)
                        {
                            return;
                        }

                        // 零参数表示使用显示器的高度和宽度
                        if (newRow == 0 || newCol == 0)
                        {
                            logger.FatalFormat("未实现ResizeWindowInCharacters, 参数为0的情况");
                            return;
                        }

                        int oldRow = this.ViewportRow;
                        int oldCol = this.ViewportColumn;

                        int columns = newCol - oldCol;
                        int rows = newRow - oldRow;

                        double width = this.activeDocument.Typeface.Width;
                        double height = this.activeDocument.Typeface.Height;
                        double deltaX = columns * width;
                        double deltaY = rows * width;

                        this.RequestChangeWindowSize?.Invoke(this, deltaX, deltaY);

                        break;
                    }

                case WindowManipulationType.RestoreIconAndWindow:
                case WindowManipulationType.SaveIconAndWindow:
                    {
                        // 没啥用，不实现
                        break;
                    }

                default:
                    {
                        logger.FatalFormat("未处理的WindowManipulationType, {0}", type);
                        break;
                    }
            }
        }

        private void PerformModes(List<int> parameters, bool enable)
        {
            foreach (int mode in parameters)
            {
                switch ((Modes)mode)
                {
                    case (Modes)25:
                        {
                            // 不知道啥意思, terminal也没实现，文档里也找不到对应的说明
                            VTDebug.Context.WriteInteractive("SwitchMode", "{0},{1}", mode, enable);
                            break;
                        }

                    default:
                        {
                            logger.ErrorFormat("未实现Modes, {0}, {1}", mode, enable);
                            break;
                        }
                }
            }
        }

        /// <summary>
        /// 指定要使用的字符集
        /// https://learn.microsoft.com/zh-cn/windows/console/console-virtual-terminal-sequences
        /// 
        /// 参考：
        /// terminal: OutputStateMachineEngine.cpp - OutputStateMachineEngine::ActionEscDispatch - default
        /// https://invisible-island.net/xterm/ctlseqs/ctlseqs.html: ESC ( C
        /// </summary>
        /// <param name="vtid">VTID里不包含ESC字符</param>
        private bool DesignateCharset(VTID vtid)
        {
            byte ch = vtid[0];
            ulong finalBytes = vtid.SubSequence(1);

            switch ((char)ch)
            {
                case '%':
                    {
                        //Routine Description:
                        // DOCS - Selects the coding system through which character sets are activated.
                        //     When ISO2022 is selected, the code page is set to ISO-8859-1, C1 control
                        //     codes are accepted, and both GL and GR areas of the code table can be
                        //     remapped. When UTF8 is selected, the code page is set to UTF-8, the C1
                        //     control codes are disabled, and only the GL area can be remapped.
                        //Arguments:
                        // - codingSystem - The coding system that will be selected.

                        CodingSystem codingSystem = (CodingSystem)vtid[1];

                        switch (codingSystem)
                        {
                            case CodingSystem.UTF8:
                                {
                                    this.acceptC1Control = true;
                                    break;
                                }

                            case CodingSystem.ISO2022:
                                {
                                    this.acceptC1Control = false;
                                    break;
                                }

                            default:
                                throw new NotImplementedException();
                        }

                        break;
                    }

                case '(':
                    {
                        this.Designate94Charset(0, finalBytes);
                        break;
                    }

                case ')':
                    {
                        this.Designate94Charset(1, finalBytes);
                        break;
                    }

                case '*':
                    {
                        this.Designate94Charset(2, finalBytes);
                        break;
                    }

                case '+':
                    {
                        this.Designate94Charset(3, finalBytes);
                        break;
                    }

                case '-':
                    {
                        this.Designate96Charset(1, finalBytes);
                        break;
                    }

                case '.':
                    {
                        this.Designate96Charset(2, finalBytes);
                        break;
                    }

                case '/':
                    {
                        this.Designate96Charset(3, finalBytes);
                        break;
                    }

                default:
                    {
                        VTDebug.Context.WriteCode("DesignateCharset失败", new List<byte>() { 0 });
                        logger.ErrorFormat("DesignateCharset失败, 可能是未处理的ESC指令??");
                        return false;
                    }
            }

            return true;
        }

        /// <summary>
        /// 根据滚动之前的值和滚动之后的值生成VTScrollData数据
        /// </summary>
        /// <param name="oldScroll">滚动之前滚动条的值</param>
        /// <param name="newScroll">滚动之后滚动条的值</param>
        /// <returns></returns>
        private VTScrollData GetScrollData(VTDocument document, int oldScroll, int newScroll)
        {
            int scrolledRows = Math.Abs(newScroll - oldScroll);

            int scrollValue = newScroll;
            int viewportRow = document.ViewportRow;
            VTHistory history = document.History;
            VTScrollInfo scrollbar = document.Scrollbar;

            List<VTHistoryLine> removedLines = new List<VTHistoryLine>();
            List<VTHistoryLine> addedLines = new List<VTHistoryLine>();

            if (scrolledRows >= viewportRow)
            {
                // 此时说明把所有行都滚动到屏幕外了

                // 遍历显示
                VTextLine current = document.FirstLine;
                for (int i = 0; i < viewportRow; i++)
                {
                    addedLines.Add(current.History);
                }

                // 我打赌不会报异常
                IEnumerable<VTHistoryLine> historyLines;
                history.TryGetHistories(oldScroll, oldScroll + viewportRow, out historyLines);
                removedLines.AddRange(historyLines);
            }
            else
            {
                // 此时说明有部分行被移动出去了
                if (newScroll > oldScroll)
                {
                    // 往下滚动
                    IEnumerable<VTHistoryLine> historyLines;
                    history.TryGetHistories(oldScroll, oldScroll + scrolledRows, out historyLines);
                    removedLines.AddRange(historyLines);

                    history.TryGetHistories(oldScroll + viewportRow, oldScroll + viewportRow + scrolledRows - 1, out historyLines);
                    addedLines.AddRange(historyLines);
                }
                else
                {
                    // 往上滚动,2
                    IEnumerable<VTHistoryLine> historyLines;
                    history.TryGetHistories(oldScroll + viewportRow - scrolledRows, oldScroll + viewportRow - 1, out historyLines);
                    removedLines.AddRange(historyLines);

                    history.TryGetHistories(newScroll, newScroll + scrolledRows, out historyLines);
                    addedLines.AddRange(historyLines);
                }
            }

            return new VTScrollData()
            {
                NewScroll = newScroll,
                OldScroll = oldScroll,
                AddedLines = addedLines,
                RemovedLines = removedLines
            };
        }

        /// <summary>
        /// 对滚动区域内的数据进行滚动
        /// </summary>
        /// <param name="delta">要滚动的行数。大于0表示往下滚动，小于0表示往上滚动</param>
        private void ScrollViewportVertically(int marginTop, int marginBottom, int delta)
        {
            VTDocument document = this.activeDocument;
            VTCursor cursor = document.Cursor;
            VTHistory history = document.History;
            VTScrollInfo scrollInfo = document.Scrollbar;
            VTextLine scrollRegionTopLine = document.FirstLine.FindNext(marginTop);
            VTextLine scrollRegionBottomLine = document.LastLine.FindPrevious(marginBottom);

            int n = Math.Abs(delta);

            // 更新可视区域的记录
            if (delta > 0)
            {
                VTextLine newTopLine = scrollRegionBottomLine;
                for (int i = 0; i < n; i++)
                {
                    VTextLine previous = newTopLine.PreviousLine;

                    document.SwapLineReverse(newTopLine, scrollRegionTopLine);
                    newTopLine.DeleteAll();

                    newTopLine = previous;
                }
            }
            else
            {
                VTextLine newBottomLine = scrollRegionTopLine;
                for (int i = 0; i < n; i++)
                {
                    VTextLine next = newBottomLine.NextLine;

                    document.SwapLine(newBottomLine, scrollRegionBottomLine);
                    newBottomLine.DeleteAll();

                    newBottomLine = next;
                }
            }

            // 更新历史记录，让历史记录的行的顺序和可视区域一致
            document.UpdateViewportHistory(marginTop, marginBottom);

            // 更新光标所在行
            document.SetCursorLogical(cursor.Row, cursor.Column);
        }

        private void HandleVT200MouseMode(VTDocument document, MouseData mouseData, bool release)
        {
            if (this.sessionTransport.Status != SessionStatusEnum.Connected)
            {
                return;
            }

            if (!document.HitTest(mouseData, this.textPointer))
            {
                return;
            }

            GraphicsInterface gi = document.GraphicsInterface;
            int lowTwoBits = release ? 3 : (int)mouseData.Button;
            int nextThreeBits = (int)gi.PressedModifierKey;
            int cb = ((nextThreeBits << 2) + lowTwoBits) + 32;
            int cx = this.textPointer.ColumnIndex + 1 + 32;
            int cy = this.textPointer.LogicalRow + 1 + 32;

            string seq = string.Format(VT200_MOUSE_MODE_DATA, (char)cb, (char)cx, (char)cy);
            byte[] bytes = Encoding.ASCII.GetBytes(seq);
            int code = this.sessionTransport.Write(bytes);
            if (code != ResponseCode.SUCCESS)
            {
                logger.ErrorFormat("发送VT200_MOUSE_MODE数据失败, {0}", code);
            }
        }

        private void HandleClickToCursor(VTDocument document, MouseData mouseData)
        {
            VTScrollInfo scrollInfo = document.Scrollbar;
            VTCursor cursor = document.Cursor;
            VTHistory history = document.History;

            if (document.HitTest(mouseData, this.textPointer))
            {
                int physicsRow = this.textPointer.PhysicsRow;
                int row = physicsRow - scrollInfo.Value;
                int col = this.textPointer.ColumnIndex;
                document.SetCursorLogical(row, col);
                cursor.Reposition();

                if (physicsRow > history.Lines - 1)
                {
                    // 缺少的历史行数
                    int adds = physicsRow - (history.Lines - 1);
                    VTextLine textLine = document.ActiveLine.FindPrevious(adds - 1);
                    for (int i = 0; i < adds; i++)
                    {
                        history.Add(textLine.History);
                        textLine = textLine.NextLine;
                    }
                }
            }
        }

        private void HandleFocusEventMode(bool focused)
        {
            if (focused)
            {
                byte[] bytes = FOCUSIN_DATA.Select(v => Convert.ToByte(v)).ToArray();
                this.sessionTransport.Write(bytes);
            }
            else
            {
                byte[] bytes = FOCUSOUT_DATA.Select(v => Convert.ToByte(v)).ToArray();
                this.sessionTransport.Write(bytes);
            }
        }

        #endregion

        #region VTParser事件

        private void VtParser_OnPrint(VTParser arg1, char ch)
        {
            char chPrint = this.TranslateCharacter(ch);

            VTDebug.Context.WriteInteractive("Print", "{0},{1},{2}", CursorRow, CursorCol, chPrint);

            this.PrintCharacter(chPrint);
        }

        private void VtParser_OnC0Actions(VTParser arg1, ASCIITable ascii)
        {
            switch (ascii)
            {
                case ASCIITable.BEL:
                    {
                        // 响铃
                        if (!this.DisableBell)
                        {
                            this.bellPlayer.Enqueue();
                        }
                        break;
                    }

                case ASCIITable.BS:
                    {
                        // Backspace，退格，光标向前移动一位
                        VTDebug.Context.WriteInteractive("Backspace", "{0},{1}", CursorRow, CursorCol);
                        activeDocument.SetCursorLogical(CursorRow, CursorCol - 1);
                        break;
                    }

                case ASCIITable.TAB:
                    {
                        /* 
                         * tab键，移动到下一个制表位，一个制表位默认8个字符 
                         * 注意不是每次向前移动8个字符，而是移动到下一个制表位
                         */

                        VTCursor cursor = this.activeDocument.Cursor;
                        int tabSize = 8;
                        int newCol = 0;
                        int oldCol = cursor.Column;

                        int nextTabPos = (int)Math.Floor((double)oldCol / tabSize) + 1;
                        newCol = nextTabPos * tabSize;
                        VTDebug.Context.WriteInteractive("ForwardTab", "{0},{1}", oldCol, newCol);
                        activeDocument.SetCursorLogical(CursorRow, newCol);
                        break;
                    }

                case ASCIITable.CR:
                    {
                        int oldRow = this.activeDocument.Cursor.Row;
                        int oldCol = this.activeDocument.Cursor.Column;

                        this.activeDocument.SetCursorLogical(CursorRow, 0);

                        int newRow = this.activeDocument.Cursor.Row;
                        int newCol = this.activeDocument.Cursor.Column;

                        VTDebug.Context.WriteInteractive("CarriageReturn", "{0},{1},{2},{3}", oldRow, oldCol, newRow, newCol);
                        break;
                    }

                case ASCIITable.LF:
                case ASCIITable.FF:
                case ASCIITable.VT:
                    {
                        // 这三个都是LF
                        this.LineFeed();
                        break;
                    }

                case ASCIITable.SI:
                    {
                        this.glSetNumber = 0;
                        this.glTranslationTable = this.gsetList[this.glSetNumber];
                        break;
                    }

                case ASCIITable.SO:
                    {
                        this.glSetNumber = 1;
                        this.glTranslationTable = this.gsetList[this.glSetNumber];
                        break;
                    }

                default:
                    {
                        throw new NotImplementedException(string.Format("未实现的控制字符:{0}", ascii));
                    }
            }

            this.OnC0ActionExecuted?.Invoke(this, ascii);
        }

        private void VtParser_OnESCActions(VTParser arg1, EscActionCodes escCode, VTID vtid)
        {
            switch (escCode)
            {
                case EscActionCodes.DECSC_CursorSave:
                    {
                        VTDebug.Context.WriteInteractive("CursorSave", "{0},{1}", CursorRow, CursorCol);

                        // 收到这个指令的时候把光标状态保存一下，等下次收到DECRC_CursorRestore再还原保存了的光标状态
                        activeDocument.CursorSave();
                        break;
                    }

                case EscActionCodes.DECRC_CursorRestore:
                    {
                        VTDebug.Context.WriteInteractive("CursorRestore", "{0},{1},{2},{3}", CursorRow, CursorCol, activeDocument.CursorState.Row, activeDocument.CursorState.Column);
                        activeDocument.CursorRestore();
                        break;
                    }

                case EscActionCodes.DECKPAM_KeypadApplicationMode:
                    {
                        VTDebug.Context.WriteInteractive("DECKPAM_KeypadApplicationMode", string.Empty);
                        keyboard.SetKeypadMode(true);
                        break;
                    }

                case EscActionCodes.DECKPNM_KeypadNumericMode:
                    {
                        VTDebug.Context.WriteInteractive("DECKPNM_KeypadNumericMode", string.Empty);
                        keyboard.SetKeypadMode(false);
                        break;
                    }

                case EscActionCodes.RI_ReverseLineFeed:
                    {
                        this.RI_ReverseLineFeed();
                        break;
                    }

                case EscActionCodes.SS2_SingleShift:
                    {
                        this.SS2_SingleShift();
                        break;
                    }

                case EscActionCodes.SS3_SingleShift:
                    {
                        this.SS3_SingleShift();
                        break;
                    }

                case EscActionCodes.LS2_LockingShift:
                    {
                        this.LS2_LockingShift();
                        break;
                    }

                case EscActionCodes.LS3_LockingShift:
                    {
                        this.LS3_LockingShift();
                        break;
                    }

                case EscActionCodes.LS1R_LockingShift:
                    {
                        this.LS1R_LockingShift();
                        break;
                    }

                case EscActionCodes.LS2R_LockingShift:
                    {
                        this.LS2R_LockingShift();
                        break;
                    }

                case EscActionCodes.LS3R_LockingShift:
                    {
                        this.LS3R_LockingShift();
                        break;
                    }

                default:
                    {
                        if (!this.DesignateCharset(vtid))
                        {
                            logger.ErrorFormat("未实现EscAction, {0}", escCode);
                        }
                        break;
                    }
            }
        }

        private void VtParser_OnCSIActions(VTParser arg1, CsiActionCodes csiCode, List<int> parameters)
        {
            switch (csiCode)
            {
                case CsiActionCodes.SGR_SetGraphicsRendition:
                    {
                        this.ApplyGraphicsOptions(parameters);
                        break;
                    }

                case CsiActionCodes.DECRST_PrivateModeReset:
                    {
                        this.PerformDECPrivateMode(parameters, false);
                        break;
                    }

                case CsiActionCodes.DECSET_PrivateModeSet:
                    {
                        this.PerformDECPrivateMode(parameters, true);
                        break;
                    }

                case CsiActionCodes.ED_EraseDisplay:
                    {
                        VTEraseType eraseType = (VTEraseType)VTParameter.GetParameter(parameters, 0, 0);
                        this.EraseDisplay(eraseType);
                        break;
                    }

                case CsiActionCodes.CUB_CursorBackward:
                    {
                        int n = VTParameter.GetParameter(parameters, 0, 1);
                        int newRow = this.CursorRow;
                        int newCol = this.CursorCol - n;

                        this.CursorMovePosition(newRow, newCol);
                        break;
                    }

                case CsiActionCodes.HVP_HorizontalVerticalPosition:
                case CsiActionCodes.CUP_CursorPosition:
                    {
                        this.CUP_CursorPosition(parameters);
                        break;
                    }

                case CsiActionCodes.CUF_CursorForward:
                    {
                        int n = VTParameter.GetParameter(parameters, 0, 1);

                        int oldRow = this.CursorRow;
                        int oldCol = this.CursorCol;

                        this.CursorMovePosition(oldRow, oldCol + n);

                        int newRow = this.CursorRow;
                        int newCol = this.CursorCol;

                        VTDebug.Context.WriteInteractive("CUB_CursorBackward", "{0},{1},{2},{3},{4}", oldRow, oldCol, newRow, newCol, n);
                        break;
                    }

                case CsiActionCodes.CUU_CursorUp:
                    {
                        int n = VTParameter.GetParameter(parameters, 0, 1);
                        int oldRow = this.CursorRow;
                        int oldCol = this.CursorCol;

                        this.CursorMovePosition(oldRow - n, oldCol);

                        int newRow = this.CursorRow;
                        int newCol = this.CursorCol;

                        VTDebug.Context.WriteInteractive("CUU_CursorUp", "{0},{1},{2},{3},{4}", oldRow, oldCol, newRow, newCol, n);
                        break;
                    }

                case CsiActionCodes.CUD_CursorDown:
                    {
                        int n = VTParameter.GetParameter(parameters, 0, 1);
                        int oldRow = this.CursorRow;
                        int oldCol = this.CursorCol;

                        this.CursorMovePosition(oldRow + n, oldCol);

                        int newRow = this.CursorRow;
                        int newCol = this.CursorCol;

                        VTDebug.Context.WriteInteractive("CUD_CursorDown", "{0},{1},{2},{3},{4}", oldRow, oldCol, newRow, newCol, n);
                        break;
                    }

                case CsiActionCodes.DTTERM_WindowManipulation:
                    {
                        this.PerformWindowManipulation(parameters);
                        break;
                    }

                case CsiActionCodes.DECSTBM_SetScrollingRegion:
                    {
                        this.DECSTBM_SetScrollingRegion(parameters);
                        break;
                    }

                case CsiActionCodes.DECSLRM_SetLeftRightMargins:
                    {
                        int leftMargin = VTParameter.GetParameter(parameters, 0, 0);
                        int rightMargin = VTParameter.GetParameter(parameters, 1, 0);

                        VTDebug.Context.WriteInteractive("DECSLRM_SetLeftRightMargins", "leftMargin = {0}, rightMargin = {1}", leftMargin, rightMargin);
                        logger.ErrorFormat("未实现DECSLRM_SetLeftRightMargins");
                        break;
                    }

                case CsiActionCodes.EL_EraseLine:
                    {
                        VTEraseType eraseType = (VTEraseType)VTParameter.GetParameter(parameters, 0, 0);
                        this.EL_EraseLine(eraseType);
                        break;
                    }

                case CsiActionCodes.DCH_DeleteCharacter:
                    {
                        // 从指定位置删除n个字符，删除后的字符串要左对齐，默认删除1个字符
                        int count = VTParameter.GetParameter(parameters, 0, 1);
                        VTDebug.Context.WriteInteractive("DCH_DeleteCharacter", "{0},{1},{2}", CursorRow, CursorCol, count);
                        activeDocument.DeleteCharacter(CursorCol, count);
                        break;
                    }

                case CsiActionCodes.ICH_InsertCharacter:
                    {
                        int count = VTParameter.GetParameter(parameters, 0, 1);
                        VTDebug.Context.WriteInteractive("ICH_InsertCharacter", "{0},{1},{2}", CursorRow, CursorCol, count);
                        this.activeDocument.InsertCharacters(CursorCol, count);
                        break;
                    }

                case CsiActionCodes.DSR_DeviceStatusReport:
                    {
                        StatusType statusType = (StatusType)Convert.ToInt32(parameters[0]);
                        this.DSR_DeviceStatusReport(statusType);
                        break;
                    }

                case CsiActionCodes.DA_DeviceAttributes:
                    {
                        VTDebug.Context.WriteInteractive("DA_DeviceAttributes", string.Empty);
                        byte[] bytes = DA_DeviceAttributesData.Select(v => Convert.ToByte(v)).ToArray();
                        VTDebug.Context.WriteInteractive(VTSendTypeEnum.DA_DeviceAttributes, bytes);
                        sessionTransport.Write(bytes);
                        break;
                    }

                case CsiActionCodes.DA2_SecondaryDeviceAttributes:
                    {
                        VTDebug.Context.WriteInteractive("DA2_SecondaryDeviceAttributes", string.Empty);
                        byte[] bytes = DA2_SecondaryDeviceAttributes.Select(v => Convert.ToByte(v)).ToArray();
                        VTDebug.Context.WriteInteractive(VTSendTypeEnum.DA2_SecondaryDeviceAttributes, bytes);
                        this.sessionTransport.Write(bytes);
                        break;
                    }

                case CsiActionCodes.IL_InsertLine:
                    {
                        int n = VTParameter.GetParameter(parameters, 0, 1);
                        VTDebug.Context.WriteInteractive("IL_InsertLine", "{0}", n);
                        if (n > 0)
                        {
                            VTDocument document = this.activeDocument;
                            VTCursor cursor = document.Cursor;
                            this.ScrollViewportVertically(cursor.Row, document.ScrollMarginBottom, n);
                        }
                        break;
                    }

                case CsiActionCodes.DL_DeleteLine:
                    {
                        int n = VTParameter.GetParameter(parameters, 0, 1);
                        VTDebug.Context.WriteInteractive("DL_DeleteLine", "{0}", n);
                        if (n > 0)
                        {
                            VTDocument document = this.activeDocument;
                            VTCursor cursor = document.Cursor;
                            this.ScrollViewportVertically(cursor.Row, document.ScrollMarginBottom, -n);
                        }
                        break;
                    }

                case CsiActionCodes.CHA_CursorHorizontalAbsolute:
                    {
                        this.CHA_CursorHorizontalAbsolute(parameters);
                        break;
                    }

                case CsiActionCodes.VPA_VerticalLinePositionAbsolute:
                    {
                        this.VPA_VerticalLinePositionAbsolute(parameters);
                        break;
                    }

                case CsiActionCodes.SD_ScrollDown:
                    {
                        int n = VTParameter.GetParameter(parameters, 0, 1);
                        VTDebug.Context.WriteInteractive("SD_ScrollDown", "{0}", n);
                        VTDocument document = this.activeDocument;
                        this.ScrollViewportVertically(document.ScrollMarginTop, document.ScrollMarginBottom, n);
                        break;
                    }

                case CsiActionCodes.SU_ScrollUp:
                    {
                        int n = VTParameter.GetParameter(parameters, 0, 1);
                        VTDebug.Context.WriteInteractive("SU_ScrollUp", "{0}", n);
                        VTDocument document = this.activeDocument;
                        this.ScrollViewportVertically(document.ScrollMarginTop, document.ScrollMarginBottom, -n);
                        break;
                    }

                case CsiActionCodes.ECH_EraseCharacters:
                    {
                        int count = VTParameter.GetParameter(parameters, 0, 1);
                        VTDebug.Context.WriteInteractive("ECH_EraseCharacters", "{0}", count);
                        this.ECH_EraseCharacters(count);
                        break;
                    }

                case CsiActionCodes.REP_RepeatCharacter:
                    {
                        int n = VTParameter.GetParameter(parameters, 0, -1);
                        VTDebug.Context.WriteInteractive("REP_RepeatCharacter", "{0},{1}", this.lastPrintChar, n);
                        for (int i = 0; i < n; i++)
                        {
                            this.PrintCharacter(this.lastPrintChar);
                        }

                        break;
                    }

                case CsiActionCodes.SM_SetMode:
                    {
                        this.PerformModes(parameters, true);
                        break;
                    }

                case CsiActionCodes.RM_ResetMode:
                    {
                        this.PerformModes(parameters, false);
                        break;
                    }

                default:
                    {
                        throw new NotImplementedException();
                    }
            }
        }

        #endregion

        #region VTActions

        /// <summary>
        /// 打印字符并移动光标到下一个打印位置
        /// </summary>
        /// <param name="ch"></param>
        private void PrintCharacter(char ch)
        {
            VTDocument document = this.activeDocument;
            VTCursor cursor = document.Cursor;
            int vpcols = document.ViewportColumn;

            // 创建并打印新的字符
            VTCharacter character = this.CreateCharacter(ch, document.AttributeState);

            // 处理自动换行逻辑
            if (this.autoWrapMode)
            {
                VTextLine activeLine = document.ActiveLine;

                // 剩余多少列可以显示
                int left = vpcols - cursor.Column;

                if (left < character.ColumnSize ||
                    left <= 0)
                {
                    // 剩余列没法容纳新字符，移动光标到下一行
                    document.SetCursorLogical(cursor.Row, 0);
                    this.LineFeed();
                }
            }

            // 根据测试得出结论：
            // 在VIM模式下输入中文字符，VIM会自动把光标往后移动2列，所以判断VIM里一个中文字符占用2列的宽度
            // 在正常模式下，如果遇到中文字符，也使用2列来显示
            // 也就是说，如果终端列数一共是80，那么可以显示40个中文字符，显示完40个中文字符后就要换行

            // 如果在shell里删除一个中文字符，那么会执行两次光标向后移动的动作，然后EraseLine - ToEnd
            // 由此可得出结论，不论是VIM还是shell，一个中文字符都是按照占用两列的空间来计算的
            document.PrintCharacter(character);
            document.SetCursorLogical(cursor.Row, cursor.Column + character.ColumnSize);

            this.OnPrint?.Invoke(this);

            // REP_RepeatCharacter会重复打印最后一个字符
            // 这里记录一下打印的最后一个字符
            if (this.lastPrintChar != ch)
            {
                this.lastPrintChar = ch;
            }
        }

        public void CarriageReturn()
        {
            // CR
            // 把光标移动到行开头

            int oldRow = this.activeDocument.Cursor.Row;
            int oldCol = this.activeDocument.Cursor.Column;

            this.activeDocument.SetCursorLogical(CursorRow, 0);

            int newRow = this.activeDocument.Cursor.Row;
            int newCol = this.activeDocument.Cursor.Column;

            VTDebug.Context.WriteInteractive("CarriageReturn", "{0},{1},{2},{3}", oldRow, oldCol, newRow, newCol);
        }

        // 换行和反向换行
        internal void LineFeed()
        {
            int oldCursorRow = this.CursorRow;
            int oldCursorCol = this.CursorCol;

            // LF
            // 滚动边距会影响到LF（DECSTBM_SetScrollingRegion），在实现的时候要考虑到滚动边距

            // 想像一下有一个打印机往一张纸上打字，当打印机想移动到下一行打字的时候，它会发出一个LineFeed指令，让纸往上移动一行
            // LineFeed，字面意思就是把纸上的下一行喂给打印机使用
            // 换行逻辑是把第一行拿到最后一行，要考虑到scrollMargin

            // 要换行的文档
            VTDocument document = this.activeDocument;
            VTScrollInfo scrollInfo = document.Scrollbar;
            VTCursor cursor = document.Cursor;
            VTHistory history = document.History;

            // 可滚动区域的第一行和最后一行
            int scrollRegionTop = document.ScrollMarginTop;
            int scrollRegionBottom = document.ScrollMarginBottom;
            VTextLine head = document.FirstLine.FindNext(scrollRegionTop);
            VTextLine last = document.LastLine.FindPrevious(scrollRegionBottom);
            bool hasScrollMargin = last != document.LastLine;
            int oldPhysicsRow = cursor.PhysicsRow;

            // 光标所在行是可滚动区域的最后一行
            // 也表示即将滚动
            if (last == ActiveLine)
            {
                // 没换行之前的光标所在行，该行数据被打印完了
                VTextLine oldActiveLine = document.ActiveLine;

                document.SwapLine(head, last);

                // 光标在滚动区域的最后一行，那么把滚动区域的第一行拿到滚动区域最后一行的下面
                logger.DebugFormat("LineFeed，光标在可滚动区域最后一行，向下滚动一行");

                if (this.IsAlternate)
                {
                    // 备用缓冲区重置被移动的行
                    head.DeleteAll();
                    document.SetCursorPhysical(oldPhysicsRow);
                }
                else
                {
                    // 换行之后记录历史行
                    // 注意用户可以输入Backspace键或者上下左右光标键来修改最新行的内容，所以最新一行的内容是实时变化的，目前的解决方案是在渲染整个文档的时候去更新最后一个历史行的数据
                    // MainScrrenBuffer和AlternateScrrenBuffer里的行分别记录
                    // AlternateScreenBuffer是用来给man，vim等程序使用的
                    // 暂时只记录主缓冲区里的数据，备用缓冲区需要考虑下是否需要记录和怎么记录，因为VIM，Man等程序用的是备用缓冲区，用户是可以实时编辑缓冲区里的数据的

                    // 在主缓冲区换行并且主缓冲区有ScorllMargin
                    // 如果有滚动边距，保持总行数不变
                    if (hasScrollMargin)
                    {
                        document.SetCursorPhysical(oldPhysicsRow);
                        document.ActiveLine.DeleteAll();

                        // 更新历史记录
                        document.UpdateViewportHistory(scrollRegionTop, scrollRegionBottom);
                    }
                    else
                    {
                        // 滚动到底了并且没有ScrollMargin

                        // 如果滚动条没滚动到底，说明下面的行可以继续使用
                        if (!scrollInfo.ScrollAtBottom)
                        {
                            scrollInfo.Value += 1;
                            document.SetCursorLogical(oldCursorRow, oldCursorCol);
                            VTHistoryLine historyLine;
                            if (history.TryGetHistory(cursor.PhysicsRow, out historyLine))
                            {
                                document.LastLine.SetHistory(historyLine);
                            }
                            else
                            {
                                // 不知道这是什么情况...
                                logger.FatalFormat("LineFeed FatalError");
                            }
                        }
                        else
                        {
                            if (scrollInfo.Maximum < document.RollbackMax)
                            {
                                // 历史记录没有超出设定的行数

                                int scrollMax = scrollInfo.Maximum + 1;
                                // 更新滚动条的值
                                // 滚动条滚动到底
                                // 计算滚动条可以滚动的最大值
                                scrollInfo.Maximum = scrollMax;
                                scrollInfo.Value = scrollMax;

                                // 设置光标所在物理行号并更新ActiveLine
                                document.SetCursorPhysical(oldPhysicsRow + 1);

                                VTextLine newActiveLine = document.ActiveLine;

                                // 有一些程序会在主缓冲区更新内容(top)，所以要判断该行是否已经被加入到了历史记录里
                                // 如果加入到了历史记录，那么就更新；如果没加入历史记录再加入历史记录
                                VTHistoryLine historyLine;
                                if (!history.TryGetHistory(cursor.PhysicsRow, out historyLine))
                                {
                                    // 为新的ActiveLine创建一个新的VTHistoryLine
                                    historyLine = new VTHistoryLine();
                                    newActiveLine.SetHistory(historyLine);
                                    history.Add(historyLine);
                                }
                                else
                                {
                                    // newActiveLine和HistoryLine可能不匹配
                                    // 所以在这里强制重新更新一下光标所在行的历史记录
                                    newActiveLine.SetHistory(historyLine);
                                }
                            }
                            else
                            {
                                // 历史记录已经超出了设定的行数了
                                // 此时最后一行的物理行号不变，需要更新ActiveLine
                                document.SetCursorPhysical(document.Cursor.PhysicsRow);

                                VTextLine newActiveLine = document.ActiveLine;
                                VTHistoryLine historyLine = new VTHistoryLine();
                                newActiveLine.SetHistory(historyLine);
                                history.Add(historyLine);
                            }
                        }
                    }
                }

                // 触发行被完全打印的事件
                this.OnLineFeed?.Invoke(this, this.IsAlternate, oldPhysicsRow, oldActiveLine.History);
            }
            else
            {
                // 没换行之前的光标所在行，该行数据被打印完了
                VTextLine oldActiveLine = document.ActiveLine;

                // 光标不在可滚动区域的最后一行，说明可以直接移动光标
                logger.DebugFormat("LineFeed，光标在滚动区域内，直接移动光标到下一行");
                document.SetCursorLogical(document.Cursor.Row + 1, document.Cursor.Column);

                int newPhysicsRow = document.Cursor.PhysicsRow;

                // 光标移动到该行的时候再加入历史记录
                if (!this.IsAlternate)
                {
                    VTextLine newActiveLine = document.ActiveLine;

                    // 有一些程序会在主缓冲区更新内容(top)，所以要判断该行是否已经被加入到了历史记录里
                    // 如果加入到了历史记录，那么就更新；如果没加入历史记录再加入历史记录
                    VTHistoryLine historyLine;
                    if (!history.TryGetHistory(newPhysicsRow, out historyLine))
                    {
                        history.Add(newActiveLine.History);
                    }
                    else
                    {
                        // newActiveLine和HistoryLine可能不匹配，因为在缩小窗口的时候VTextLine直接被删除了
                        // 所以在这里强制重新更新一下光标所在行的历史记录
                        newActiveLine.SetHistory(historyLine);
                    }
                }

                // 触发行被完全打印的事件
                this.OnLineFeed?.Invoke(this, this.IsAlternate, oldPhysicsRow, oldActiveLine.History);
            }

            int newRow = this.CursorRow;
            int newCol = this.CursorCol;

            VTDebug.Context.WriteInteractive("LineFeed", "{0},{1},{2},{3}", oldCursorRow, oldCursorCol, newRow, newCol);
        }

        private void RI_ReverseLineFeed()
        {
            // 和LineFeed相反，把光标所在行向上移动一行
            // 在用man命令的时候往上滚动会触发这个指令
            // 反向换行 – 执行\n的反向操作，将光标所在行向上移动一行，维护水平位置，如有必要，滚动缓冲区 *

            // 反向换行不增加新行，也不减少新行，保持总行数和物理行号不变

            VTDocument document = this.activeDocument;
            VTCursor cursor = document.Cursor;
            int scrollRegionTop = document.ScrollMarginTop;
            int scrollRegionBottom = document.ScrollMarginBottom;
            int oldCursorRow = cursor.Row;
            int oldCursorCol = cursor.Column;
            int oldCursorPhysicsRow = cursor.PhysicsRow;

            // 可滚动区域的第一行和最后一行
            VTextLine scrollRegionTopLine = document.FirstLine.FindNext(scrollRegionTop);
            VTextLine scrollRegionBottomLine = document.LastLine.FindPrevious(scrollRegionBottom);

            if (scrollRegionTopLine == ActiveLine)
            {
                // 此时光标位置在滚动区域的第一行
                logger.DebugFormat("RI_ReverseLineFeed，光标在可视区域第一行，向上移动一行并且可视区域往上移动一行");

                // 把滚动区域最后一行拿到滚动区域第一行前面
                document.SwapLineReverse(scrollRegionBottomLine, scrollRegionTopLine);

                // 清除滚动区域第一行的数据
                scrollRegionBottomLine.DeleteAll();

                document.UpdateViewportHistory(scrollRegionTop, scrollRegionBottom);

                document.SetCursorPhysical(oldCursorPhysicsRow);
            }
            else
            {
                // 光标位置在可滚动区域里面
                logger.DebugFormat("RI_ReverseLineFeed，光标在可视区域里，直接移动光标到上一行");
                document.SetCursorLogical(cursor.Row - 1, cursor.Column);
            }

            int newRow = this.CursorRow;
            int newCol = this.CursorCol;

            VTDebug.Context.WriteInteractive("RI_ReverseLineFeed", "{0},{1},{2},{3}", oldCursorRow, oldCursorCol, newRow, newCol);
        }

        public void EraseDisplay(VTEraseType eraseType)
        {
            int oldRow = this.CursorRow;
            int oldCol = this.CursorCol;

            switch (eraseType)
            {
                // terminal项目里是All和Scrollback执行的是一样的操作：In most terminals, this is done by moving the viewport into the scrollback, clearing out the current screen.
                // top和clear指令会执行EraseType.All，在其他的终端软件里top指令不会清空已经存在的行，而是把已经存在的行往上移动
                // 所以EraseType.All的动作和Scrollback一样执行

                case VTEraseType.All:
                    {
                        this.activeDocument.EraseAll();

                        break;
                    }

                case VTEraseType.Scrollback:
                    {
                        if (this.IsAlternate)
                        {
                            // xterm-256color类型的终端VIM程序的PageDown和PageUp会执行EraseType.All
                            // 备用缓冲区是没有滚动数据的，只能清空当前显示的所有内容
                            // 所以这里把备用缓冲区和主缓冲区分开处理
                            VTextLine next = activeDocument.FirstLine;
                            while (next != null)
                            {
                                next.EraseAll();

                                next = next.NextLine;
                            }
                        }
                        else
                        {
                            // 把当前所有行移动到可视区域外，并更新当前光标所在行

                            // 相关命令：
                            // MainDocument：xterm-256color类型的终端clear程序
                            // AlternateDocument：暂无

                            // Erase Saved Lines
                            // 模拟xshell的操作，把当前行（光标所在行，就是最后一行）移动到可视区域的第一行

                            VTDocument document = this.activeDocument;
                            VTScrollInfo scrollInfo = document.Scrollbar;
                            VTHistory history = document.History;

                            // 获取当前屏幕一共显示了多少行
                            int displayRows = document.GetDisplayRows(scrollInfo.Maximum);

                            // 计算滚动条的最大值
                            int newScrollMax = scrollInfo.Value + displayRows;
                            newScrollMax = Math.Min(newScrollMax, document.RollbackMax);

                            if (newScrollMax == scrollInfo.Maximum)
                            {
                                return;
                            }

                            scrollInfo.Maximum = newScrollMax;
                            scrollInfo.Value = newScrollMax;

                            // 从ScrollMax开始显示
                            VTextLine textLine = document.FirstLine;
                            for (int i = 0; i < document.ViewportRow; i++)
                            {
                                VTHistoryLine historyLine;
                                if (document.History.TryGetHistory(scrollInfo.Value + i, out historyLine))
                                {
                                    textLine.SetHistory(historyLine);
                                }
                                else
                                {
                                    historyLine = new VTHistoryLine();
                                    textLine.SetHistory(historyLine);
                                }

                                textLine = textLine.NextLine;
                            }
                        }
                        break;
                    }

                default:
                    {
                        activeDocument.EraseDisplay(CursorCol, (EraseType)eraseType);
                        break;
                    }
            }

            int newRow = this.CursorRow;
            int newCol = this.CursorCol;

            VTDebug.Context.WriteInteractive("EraseDisplay", "{0},{1},{2},{3},{4}", oldRow, oldCol, newRow, newCol, eraseType);
        }

        public void EL_EraseLine(VTEraseType eraseType)
        {
            // TODO：优化填充算法

            VTDebug.Context.WriteInteractive("EL_EraseLine", "{0},{1},{2}", CursorRow, CursorCol, eraseType);

            switch (eraseType)
            {
                case VTEraseType.All:
                    {
                        for (int i = 0; i < this.ViewportColumn; i++)
                        {
                            VTCharacter character = VTCharacter.CreateEmpty();
                            this.activeDocument.PrintCharacter(character, i);
                        }

                        break;
                    }

                case VTEraseType.FromBeginning:
                    {
                        for (int i = 0; i <= this.CursorCol; i++)
                        {
                            VTCharacter character = VTCharacter.CreateEmpty();
                            this.activeDocument.PrintCharacter(character, i);
                        }

                        break;
                    }

                case VTEraseType.ToEnd:
                    {
                        for (int i = this.CursorCol; i < this.ViewportColumn; i++)
                        {
                            VTCharacter character = VTCharacter.CreateEmpty();
                            this.activeDocument.PrintCharacter(character, i);
                        }

                        break;
                    }

                default:
                    throw new NotImplementedException();
            }
        }

        public void ECH_EraseCharacters(int count)
        {
            // TODO：优化算法
            VTextAttributeState attributeState = this.activeDocument.AttributeState;

            for (int i = 0; i < count; i++)
            {
                int column = this.CursorCol + i;

                VTCharacter character = this.CreateCharacter(' ', attributeState);

                this.activeDocument.PrintCharacter(character, column);
            }
        }

        // 设备状态
        public void DSR_DeviceStatusReport(StatusType statusType)
        {
            switch (statusType)
            {
                case StatusType.OS_OperatingStatus:
                    {
                        // Result ("OK") is CSI 0 n
                        VTDebug.Context.WriteInteractive("DSR_DeviceStatusReport", "{0}", statusType);
                        VTDebug.Context.WriteInteractive(VTSendTypeEnum.DSR_DeviceStatusReport, statusType, OS_OperatingStatusData);
                        sessionTransport.Write(OS_OperatingStatusData);
                        break;
                    }

                case StatusType.CPR_CursorPositionReport:
                    {
                        // 打开VIM后会收到这个请求

                        // 1,1 is the top - left corner of the viewport in VT - speak, so add 1
                        // Result is CSI ? r ; c R
                        int cursorRow = CursorRow + 1;
                        int cursorCol = CursorCol + 1;
                        VTDebug.Context.WriteInteractive("DSR_DeviceStatusReport", "{0},{1},{2}", statusType, CursorRow, CursorCol);
                        string cprData = string.Format("\x1b[{0};{1}R", cursorRow, cursorCol);
                        byte[] cprBytes = Encoding.ASCII.GetBytes(cprData);
                        VTDebug.Context.WriteInteractive(VTSendTypeEnum.DSR_DeviceStatusReport, statusType, cprBytes);
                        sessionTransport.Write(cprBytes);
                        break;
                    }

                default:
                    throw new NotImplementedException();
            }
        }

        #region 光标控制

        // 下面的光标移动指令不能进行VTDocument的滚动
        // 光标的移动坐标是相对于可视区域内的坐标
        // 服务器发送过来的光标原点是从(1,1)开始的，我们程序里的是(0,0)开始的，所以要减1

        private void CursorMovePosition(int row, int col)
        {
            row = MTermUtils.Clamp(row, 0, this.ViewportRow - 1);
            col = MTermUtils.Clamp(col, 0, this.ViewportColumn - 1);

            this.activeDocument.SetCursorLogical(row, col);
        }

        public void CUP_CursorPosition(List<int> parameters)
        {
            // 打开vim，输入i，然后按tab，虽然第一行的字符列数小于要移动到的col，但是vim还是会移动，所以这里把不足的列数使用空格补齐

            int oldRow = this.CursorRow;
            int oldCol = this.CursorCol;

            int row = 0, col = 0;
            if (parameters.Count == 2)
            {
                // VT的光标原点是(1,1)，我们程序里的是(0,0)，所以要减1
                int newrow = parameters[0];
                int newcol = parameters[1];

                // 测试中发现在ubuntu系统上执行apt install或者apt remove命令，HVP会发送0列过来，这里处理一下，如果遇到参数是0，那么就直接变成0
                row = newrow == 0 ? 0 : newrow - 1;
                col = newcol == 0 ? 0 : newcol - 1;

                int viewportRow = this.ViewportRow;
                int viewportColumn = this.ViewportColumn;

                // 对行和列做限制
                if (row >= viewportRow)
                {
                    row = viewportRow - 1;
                }

                if (col >= viewportColumn)
                {
                    col = viewportColumn - 1;
                }
            }
            else
            {
                // 如果没有参数，那么说明就是定位到原点(0,0)
            }

            activeDocument.SetCursorLogical(row, col);

            int newRow = this.CursorRow;
            int newCol = this.CursorCol;

            VTDebug.Context.WriteInteractive("CUP_CursorPosition", "{0},{1},{2},{3}", oldRow, oldCol, newRow, newCol);
        }

        public void CHA_CursorHorizontalAbsolute(List<int> parameters)
        {
            int oldRow = this.CursorRow;
            int oldCol = this.CursorCol;
            // 将光标移动到当前行中的第n列
            int n = VTParameter.GetParameter(parameters, 0, -1);

            if (n == -1)
            {
                VTDebug.Context.WriteInteractive("CHA_CursorHorizontalAbsolute", "{0},{1},{2}, n是-1, 不执行操作", oldRow, oldCol, n);
            }
            else
            {
                int col = n - 1;
                activeDocument.SetCursorLogical(CursorRow, col);

                int newRow = this.CursorRow;
                int newCol = this.CursorCol;

                VTDebug.Context.WriteInteractive("CHA_CursorHorizontalAbsolute", "{0},{1},{2},{3},{4}", oldRow, oldCol, newRow, newCol, n);
            }
        }

        public void VPA_VerticalLinePositionAbsolute(List<int> parameters)
        {
            int oldRow = this.CursorRow;
            int oldCol = this.CursorCol;

            // 绝对垂直行位置 光标在当前列中垂直移动到第 <n> 个位置
            // 保持列不变，把光标移动到指定的行处
            int row = VTParameter.GetParameter(parameters, 0, 1);
            row = Math.Max(0, row - 1);

            activeDocument.SetCursorLogical(row, oldCol);

            int newRow = this.CursorRow;
            int newCol = this.CursorCol;

            VTDebug.Context.WriteInteractive("VPA_VerticalLinePositionAbsolute", "{0},{1},{2},{3},{4}", oldRow, oldCol, newRow, newCol, row);
        }

        #endregion

        // Margin
        public void DECSTBM_SetScrollingRegion(List<int> parameters)
        {
            // 当前终端屏幕可显示的行数量
            int lines = this.ViewportRow;

            int topMargin = VTParameter.GetParameter(parameters, 0, 1);
            int bottomMargin = VTParameter.GetParameter(parameters, 1, lines);

            if (bottomMargin < 0 || topMargin < 0)
            {
                logger.ErrorFormat("DECSTBM_SetScrollingRegion参数不正确，忽略本次设置, topMargin = {0}, bottomMargin = {1}", topMargin, bottomMargin);
                return;
            }
            if (topMargin >= bottomMargin)
            {
                logger.ErrorFormat("DECSTBM_SetScrollingRegion参数不正确，topMargin大于等于bottomMargin，忽略本次设置, topMargin = {0}, bottomMargin = {1}", topMargin, bottomMargin);
                return;
            }
            if (bottomMargin > lines)
            {
                logger.ErrorFormat("DECSTBM_SetScrollingRegion参数不正确，bottomMargin大于当前屏幕总行数, bottomMargin = {0}, lines = {1}", bottomMargin, lines);
                return;
            }

            // 如果topMargin等于1，那么就表示使用默认值，也就是没有marginTop，所以当topMargin == 1的时候，marginTop改为0
            int marginTop = topMargin == 1 ? 0 : topMargin - 1;
            // 如果bottomMargin等于控制台高度，那么就表示使用默认值，也就是没有marginBottom，所以当bottomMargin == 控制台高度的时候，marginBottom改为0
            int marginBottom = lines - bottomMargin;

            VTDebug.Context.WriteInteractive("DECSTBM_SetScrollingRegion", "topMargin1 = {0}, bottomMargin1 = {1}, topMargin2 = {2}, bottomMargin2 = {3}", topMargin, bottomMargin, marginTop, marginBottom);

            activeDocument.SetScrollMargin(marginTop, marginBottom);
        }

        /// <summary>
        /// SGR
        /// 打开VIM的时候，VIM会在打印第一行的~号的时候设置验色，然后把剩余的行全部打印，也就是说设置一次颜色可以对多行都生效
        /// 所以这里要记录下如果当前有文本特效被设置了，那么在行改变的时候也需要设置文本特效
        /// 缓存下来，每次打印字符的时候都要对ActiveLine Apply一下
        /// </summary>
        /// <param name="options"></param>
        /// <param name="parameters"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void PerformSGR(GraphicsOptions options, VTColor extColor)
        {
            VTDebug.Context.WriteInteractive(string.Format("SGR - {0}", options), string.Empty);

            switch (options)
            {
                case GraphicsOptions.Off:
                    {
                        // 重置所有文本装饰
                        activeDocument.ClearAttribute();
                        break;
                    }

                case GraphicsOptions.ForegroundDefault:
                    {
                        this.activeDocument.SetAttribute(VTextAttributes.Foreground, false, null);
                        break;
                    }

                case GraphicsOptions.BackgroundDefault:
                    {
                        this.activeDocument.SetAttribute(VTextAttributes.Background, false, null);
                        break;
                    }

                case GraphicsOptions.NotBoldOrFaint:
                    {
                        this.activeDocument.SetAttribute(VTextAttributes.Bold, false, null);
                        this.activeDocument.SetAttribute(VTextAttributes.Faint, false, null);
                        break;
                    }

                case GraphicsOptions.RGBColorOrFaint:
                    {
                        logger.ErrorFormat("Faint");
                        this.activeDocument.SetAttribute(VTextAttributes.Faint, true, null);
                        break;
                    }

                case GraphicsOptions.Negative:
                    {
                        // ReverseVideo
                        VTColor foreColor = VTColor.CreateFromRgbKey(backgroundColor);
                        VTColor backColor = VTColor.CreateFromRgbKey(foregroundColor);
                        activeDocument.SetAttribute(VTextAttributes.Background, true, backColor);
                        activeDocument.SetAttribute(VTextAttributes.Foreground, true, foreColor);
                        break;
                    }

                case GraphicsOptions.Positive:
                    {
                        // ReverseVideoUnset
                        activeDocument.SetAttribute(VTextAttributes.Background, false, null);
                        activeDocument.SetAttribute(VTextAttributes.Foreground, false, null);
                        break;
                    }

                case GraphicsOptions.BoldBright:
                    {
                        this.activeDocument.SetAttribute(VTextAttributes.Bold, true, null);
                        break;
                    }

                case GraphicsOptions.Italics:
                    {
                        this.activeDocument.SetAttribute(VTextAttributes.Italics, true, null);
                        break;
                    }

                case GraphicsOptions.NotItalics:
                    {
                        this.activeDocument.SetAttribute(VTextAttributes.Italics, false, null);
                        break;
                    }

                case GraphicsOptions.Underline:
                    {
                        this.activeDocument.SetAttribute(VTextAttributes.Underline, true, null);
                        break;
                    }

                case GraphicsOptions.DoublyUnderlined:
                    {
                        this.activeDocument.SetAttribute(VTextAttributes.DoublyUnderlined, true, null);
                        break;
                    }

                case GraphicsOptions.NoUnderline:
                    {
                        // UnderlineUnset
                        activeDocument.SetAttribute(VTextAttributes.Underline, false, null);

                        // DoublyUnderlineUnset
                        activeDocument.SetAttribute(VTextAttributes.DoublyUnderlined, false, null);
                        break;
                    }

                case GraphicsOptions.ForegroundBlack:
                case GraphicsOptions.ForegroundBlue:
                case GraphicsOptions.ForegroundGreen:
                case GraphicsOptions.ForegroundCyan:
                case GraphicsOptions.ForegroundRed:
                case GraphicsOptions.ForegroundMagenta:
                case GraphicsOptions.ForegroundYellow:
                case GraphicsOptions.ForegroundWhite:
                case GraphicsOptions.BrightForegroundBlack:
                case GraphicsOptions.BrightForegroundBlue:
                case GraphicsOptions.BrightForegroundGreen:
                case GraphicsOptions.BrightForegroundCyan:
                case GraphicsOptions.BrightForegroundRed:
                case GraphicsOptions.BrightForegroundMagenta:
                case GraphicsOptions.BrightForegroundYellow:
                case GraphicsOptions.BrightForegroundWhite:
                    {
                        VTColorIndex colorIndex = VTermUtils.GraphicsOptions2VTColorIndex(options);
                        this.activeDocument.SetAttribute(VTextAttributes.Foreground, true, this.colorTable.GetColor(colorIndex));
                        break;
                    }

                case GraphicsOptions.BackgroundBlack:
                case GraphicsOptions.BackgroundBlue:
                case GraphicsOptions.BackgroundGreen:
                case GraphicsOptions.BackgroundCyan:
                case GraphicsOptions.BackgroundRed:
                case GraphicsOptions.BackgroundMagenta:
                case GraphicsOptions.BackgroundYellow:
                case GraphicsOptions.BackgroundWhite:
                case GraphicsOptions.BrightBackgroundBlack:
                case GraphicsOptions.BrightBackgroundBlue:
                case GraphicsOptions.BrightBackgroundGreen:
                case GraphicsOptions.BrightBackgroundCyan:
                case GraphicsOptions.BrightBackgroundRed:
                case GraphicsOptions.BrightBackgroundMagenta:
                case GraphicsOptions.BrightBackgroundYellow:
                case GraphicsOptions.BrightBackgroundWhite:
                    {
                        VTColorIndex colorIndex = VTermUtils.GraphicsOptions2VTColorIndex(options);
                        this.activeDocument.SetAttribute(VTextAttributes.Background, true, this.colorTable.GetColor(colorIndex));
                        break;
                    }

                case GraphicsOptions.ForegroundExtended:
                    {
                        this.activeDocument.SetAttribute(VTextAttributes.Foreground, true, extColor);
                        break;
                    }

                case GraphicsOptions.BackgroundExtended:
                    {
                        this.activeDocument.SetAttribute(VTextAttributes.Background, true, extColor);
                        break;
                    }

                case GraphicsOptions.NotCrossedOut:
                case GraphicsOptions.CrossedOut:
                    {
                        logger.ErrorFormat("未实现SGR, {0}", options.ToString());
                        break;
                    }

                case GraphicsOptions.BlinkOrXterm256Index:
                case GraphicsOptions.RapidBlink:
                    {
                        break;
                    }

                case GraphicsOptions.Steady:
                    {
                        break;
                    }

                default:
                    {
                        // TODO：
                        logger.ErrorFormat("未实现的SGR, {0}", options.ToString());
                        throw new NotImplementedException();
                    }
            }
        }

        public void ASB_AlternateScreenBuffer(bool enable)
        {
        }

        #region 切换字符集

        public void SS2_SingleShift()
        {
            this.glSetNumberSingleShift = 2;
        }

        public void SS3_SingleShift()
        {
            this.glSetNumberSingleShift = 3;
        }

        public void LS2_LockingShift()
        {
            this.glSetNumber = 2;
            this.glTranslationTable = this.gsetList[this.glSetNumber];
        }

        public void LS3_LockingShift()
        {
            this.glSetNumber = 3;
            this.glTranslationTable = this.gsetList[this.glSetNumber];
        }

        public void LS1R_LockingShift()
        {
            this.grSetNumber = 1;
            this.grTranslationTable = this.gsetList[this.grSetNumber];
        }

        public void LS2R_LockingShift()
        {
            this.grSetNumber = 2;
            this.grTranslationTable = this.gsetList[this.grSetNumber];
        }

        public void LS3R_LockingShift()
        {
            this.grSetNumber = 3;
            this.grTranslationTable = this.gsetList[this.grSetNumber];
        }


        public void Designate94Charset(int gsetIndex, ulong charset)
        {
            VTCharsetMap charsetMap = this.Select94CharsetMap(charset);

            this.gsetList[gsetIndex] = charsetMap;

            this.glTranslationTable = this.gsetList[this.glSetNumber];
            this.grTranslationTable = this.gsetList[this.grSetNumber];
        }

        public void Designate96Charset(int gsetIndex, ulong charset)
        {
            VTCharsetMap charsetMap = this.Select96CharsetMap(charset);

            this.gsetList[gsetIndex] = charsetMap;

            this.glTranslationTable = this.gsetList[this.glSetNumber];
            this.grTranslationTable = this.gsetList[this.grSetNumber];
        }

        #endregion

        #endregion

        #region 事件处理器

        private void MainDocument_MouseDown(VTDocument document, MouseData mouseData)
        {
            if (mouseData.Button == VTMouseButton.LeftButton)
            {
                if (this.clickToCursor)
                {
                    this.HandleClickToCursor(document, mouseData);
                }
            }

            if (this.isVT200MouseMode)
            {
                this.HandleVT200MouseMode(document, mouseData, false);
            }
        }

        private void MainDocument_MouseUp(VTDocument document, MouseData mouseData)
        {
            if (this.isVT200MouseMode)
            {
                this.HandleVT200MouseMode(document, mouseData, true);
            }
        }

        #endregion
    }
}
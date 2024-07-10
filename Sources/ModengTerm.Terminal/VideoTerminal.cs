using ModengTerm.Base;
using ModengTerm.Base.DataModels;
using ModengTerm.Base.DataModels.Terminal;
using ModengTerm.Base.Enumerations;
using ModengTerm.Document;
using ModengTerm.Document.Drawing;
using ModengTerm.Document.Enumerations;
using ModengTerm.Document.Utility;
using ModengTerm.Terminal.Loggering;
using ModengTerm.Terminal.Parsing;
using ModengTerm.Terminal.Session;
using System.Printing;
using System.Text;
using XTerminal.Base.Definitions;

namespace ModengTerm.Terminal
{
    public class VTOptions
    {
        public IDocumentRenderer AlternateDocument { get; set; }

        public IDocumentRenderer MainDocument { get; set; }

        /// <summary>
        /// 该终端所对应的Session
        /// </summary>
        public XTermSession Session { get; set; }

        /// <summary>
        /// 发送数据给主机的回调
        /// </summary>
        public SessionTransport SessionTransport { get; set; }
    }

    /// <summary>
    /// 处理虚拟终端的所有逻辑
    /// 主缓冲区：文档物理行数是不固定的，可以大于终端行数
    /// 备用缓冲区：文档的物理行数是固定的，等于终端的行数
    /// </summary>
    public class VideoTerminal : IVideoTerminal, VTDispatchHandler
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("VideoTerminal");

        private static readonly byte[] OS_OperatingStatusData = new byte[4] { 0x1b, (byte)'[', (byte)'0', (byte)'n' };
        private static readonly byte[] DA_DeviceAttributesData = new byte[7] { 0x1b, (byte)'[', (byte)'?', (byte)'1', (byte)':', (byte)'0', (byte)'c' };

        #endregion

        #region 公开事件

        /// <summary>
        /// 当某一行被完整打印之后触发
        /// 如果同一行被多次打印（top命令），那么只会在第一次打印的时候触发
        /// 只有在主缓冲区的时候才会触发
        /// </summary>
        public event Action<IVideoTerminal, VTHistoryLine> LinePrinted;

        /// <summary>
        /// 当切换显示文档之后触发
        /// IVideoTerminal：事件触发者
        /// VTDocument：oldDocument，切换之前显示的文档
        /// VTDocument：newDocument，切换之后显示的文档
        /// </summary>
        public event Action<IVideoTerminal, VTDocument, VTDocument> DocumentChanged;

        /// <summary>
        /// 当可视区域的行或列改变的时候触发
        /// </summary>
        public event Action<IVideoTerminal, int, int> ViewportChanged;

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

        /// <summary>
        /// UI线程上下文
        /// </summary>
        internal SynchronizationContext uiSyncContext;

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

        private IDocumentRenderer documentCanvas;

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

        /// <summary>
        /// UI线程上下文对象
        /// </summary>
        public SynchronizationContext UISyncContext { get { return uiSyncContext; } }

        public VTLogger Logger { get; set; }

        /// <summary>
        /// 电脑按键和发送的数据的映射关系
        /// </summary>
        public VTKeyboard Keyboard { get { return keyboard; } }

        /// <summary>
        /// 当前显示的是否是备用缓冲区
        /// </summary>
        public bool IsAlternate { get { return this.activeDocument == this.alternateDocument; } }

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
            uiSyncContext = SynchronizationContext.Current;

            vtOptions = options;
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

            // DECAWM
            autoWrapMode = false;

            // 初始化变量

            isRunning = true;

            sessionTransport = options.SessionTransport;

            XTermSession sessionInfo = options.Session;

            Name = sessionInfo.Name;

            writeEncoding = Encoding.GetEncoding(sessionInfo.GetOption<string>(OptionKeyEnum.WRITE_ENCODING));
            scrollDelta = sessionInfo.GetOption<int>(OptionKeyEnum.MOUSE_SCROLL_DELTA);
            colorTable = sessionInfo.GetOption<VTColorTable>(OptionKeyEnum.TEHEM_COLOR_TABLE);
            foregroundColor = sessionInfo.GetOption<string>(OptionKeyEnum.THEME_FONT_COLOR);
            backgroundColor = sessionInfo.GetOption<string>(OptionKeyEnum.THEME_BACKGROUND_COLOR);

            #region 初始化键盘

            keyboard = new VTKeyboard();
            keyboard.Encoding = writeEncoding;
            keyboard.SetAnsiMode(true);
            keyboard.SetKeypadMode(false);

            #endregion

            #region 初始化文档模型

            VTDocumentOptions mainOptions = this.CreateDocumentOptions("MainDocument", sessionInfo, options.MainDocument);
            this.mainDocument = new VTDocument(mainOptions);
            this.mainDocument.Initialize();
            this.mainDocument.History.AddHistory(this.mainDocument.FirstLine.History);

            VTDocumentOptions alternateOptions = this.CreateDocumentOptions("AlternateDocument", sessionInfo, options.AlternateDocument);
            alternateOptions.ScrollbackMax = 0;
            this.alternateDocument = new VTDocument(alternateOptions);
            this.alternateDocument.Initialize();
            this.alternateDocument.SetVisible(false);

            this.activeDocument = this.mainDocument;

            // 初始化完VTDocument之后，真正要使用的Column和Row已经被计算出来并保存到了VTDocumentOptions里
            // 此时重新设置sessionInfo里的Row和Column，因为SessionTransport要使用
            sessionInfo.SetOption<int>(OptionKeyEnum.SSH_TERM_ROW, mainOptions.ViewportRow);
            sessionInfo.SetOption<int>(OptionKeyEnum.SSH_TERM_COL, mainOptions.ViewportColumn);

            this.ViewportChanged?.Invoke(this, this.mainDocument.ViewportRow, this.mainDocument.ViewportColumn);

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
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Release()
        {
            isRunning = false;

            this.mainDocument.Release();
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

        /// <summary>
        /// 获取当前使用鼠标选中的段落区域
        /// </summary>
        /// <returns></returns>
        public VTParagraph GetSelectedParagraph()
        {
            VTextSelection selection = this.activeDocument.Selection;
            if (selection.IsEmpty)
            {
                return VTParagraph.Empty;
            }

            return this.CreateParagraph(ParagraphTypeEnum.Selected, ParagraphFormatEnum.PlainText);
        }

        /// <summary>
        /// 创建指定的段落内容
        /// </summary>
        /// <param name="paragraphType">段落类型</param>
        /// <param name="fileType">要创建的内容格式</param>
        /// <returns></returns>
        public VTParagraph CreateParagraph(ParagraphTypeEnum paragraphType, ParagraphFormatEnum fileType)
        {
            // 所有要保存的行存储在这里
            List<VTHistoryLine> historyLines = new List<VTHistoryLine>();
            int startCharacterIndex = -1, endCharacterIndex = -1;

            switch (paragraphType)
            {
                case ParagraphTypeEnum.AllDocument:
                    {
                        if (IsAlternate)
                        {
                            // 备用缓冲区直接保存VTextLine
                            VTextLine current = this.activeDocument.FirstLine;
                            while (current != null)
                            {
                                historyLines.Add(current.History);
                                current = current.NextLine;
                            }
                        }
                        else
                        {
                            historyLines.AddRange(this.activeDocument.History.GetAllHistoryLines());
                        }

                        startCharacterIndex = 0;
                        endCharacterIndex = historyLines.LastOrDefault().Characters.Count - 1;

                        break;
                    }

                case ParagraphTypeEnum.Viewport:
                    {
                        VTextLine current = this.activeDocument.FirstLine;
                        while (current != null)
                        {
                            historyLines.Add(current.History);
                            current = current.NextLine;
                        }

                        startCharacterIndex = 0;
                        endCharacterIndex = historyLines.LastOrDefault().Characters.Count - 1;

                        break;
                    }

                case ParagraphTypeEnum.Selected:
                    {
                        VTextSelection selection = this.activeDocument.Selection;

                        if (selection.IsEmpty)
                        {
                            return VTParagraph.Empty;
                        }

                        VTextPointer topPointer = null, bottomPointer = null;

                        if (selection.StartPointer.PhysicsRow < selection.EndPointer.PhysicsRow)
                        {
                            topPointer = selection.StartPointer;
                            bottomPointer = selection.EndPointer;
                        }
                        else
                        {
                            topPointer = selection.EndPointer;
                            bottomPointer = selection.StartPointer;
                        }

                        int topRow = topPointer.PhysicsRow;
                        int bottomRow = bottomPointer.PhysicsRow;

                        if (IsAlternate)
                        {
                            // 备用缓冲区没有滚动内容，只能选中当前显示出来的文档
                            int rows = bottomRow - topRow + 1;
                            VTextLine firstLine = this.activeDocument.FindLine(topRow);
                            while (rows >= 0)
                            {
                                historyLines.Add(firstLine.History);

                                firstLine = firstLine.NextLine;

                                rows--;
                            }
                        }
                        else
                        {
                            IEnumerable<VTHistoryLine> histories;
                            if (!this.activeDocument.History.TryGetHistories(topRow, bottomRow, out histories))
                            {
                                logger.ErrorFormat("SaveSelected失败, 有的历史记录为空");
                                return VTParagraph.Empty;
                            }
                            historyLines.AddRange(histories);

                            if (historyLines.Count == 0)
                            {
                                logger.WarnFormat("选中区域为空");
                                return VTParagraph.Empty;
                            }
                        }

                        VTHistoryLine topLine = historyLines.FirstOrDefault();
                        VTHistoryLine bottomLine = historyLines.LastOrDefault();

                        #region 处理当选中的第一行没有文本的情况

                        if (topPointer.CharacterIndex == -1)
                        {
                            // 第一行没有选中字符
                            int columns = 0;

                            for (int i = 0; i < topLine.Characters.Count; i++)
                            {
                                VTCharacter character = topLine.Characters[i];

                                if (columns >= topPointer.ColumnIndex)
                                {
                                    startCharacterIndex = i;
                                    break;
                                }

                                columns += character.ColumnSize;
                            }

                            // 如果选中的第一行没有选中的字符，那么就用下一行的第一个字符作为选中起始点
                            if (startCharacterIndex == -1)
                            {
                                startCharacterIndex = 0;
                                historyLines.Remove(topLine);
                            }
                        }
                        else
                        {
                            startCharacterIndex = topPointer.CharacterIndex;
                        }

                        #endregion

                        #region 处理当选中的最后一行没有文本的情况

                        if (bottomPointer.CharacterIndex == -1)
                        {
                            // 最后一行没有选中字符

                            int columns = bottomLine.Characters.Sum(v => v.ColumnSize);

                            for (int i = bottomLine.Characters.Count - 1; i >= 0; i--)
                            {
                                VTCharacter character = bottomLine.Characters[i];

                                if (columns <= bottomPointer.ColumnIndex)
                                {
                                    endCharacterIndex = i;
                                    break;
                                }

                                columns -= character.ColumnSize;
                            }

                            // 如果选中的最后一行没有选中的字符，那么就用上一行的最后一个字符作为终点
                            if (endCharacterIndex == -1)
                            {
                                historyLines.Remove(bottomLine);

                                if (historyLines.Count > 0)
                                {
                                    VTHistoryLine historyLine = historyLines.LastOrDefault();
                                    endCharacterIndex = historyLine.Characters.Count - 1;
                                }
                            }
                        }
                        else
                        {
                            endCharacterIndex = bottomPointer.CharacterIndex;
                        }

                        #endregion

                        break;
                    }

                default:
                    throw new NotImplementedException();
            }

            // 当选中的内容为空的时候会出现
            if (historyLines.Count == 0)
            {
                return VTParagraph.Empty;
            }

            CreateContentParameter parameter = new CreateContentParameter()
            {
                SessionName = String.Empty,
                HistoryLines = historyLines,
                StartCharacterIndex = startCharacterIndex,
                EndCharacterIndex = endCharacterIndex,
                ContentType = fileType,
                Typeface = this.activeDocument.Typeface
            };

            string text = VTUtils.CreateContent(parameter);

            return new VTParagraph()
            {
                Content = text,
                CreationTime = DateTime.Now,
                StartCharacterIndex = startCharacterIndex,
                EndCharacterIndex = endCharacterIndex,
                CharacterList = historyLines,
                IsAlternate = IsAlternate
            };
        }

        /// <summary>
        /// 滚动并重新渲染
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

        public void OnSizeChanged(VTSize newSize)
        {
            if (this.sessionTransport.Status != SessionStatusEnum.Connected)
            {
                return;
            }

            VTDocument document = this.activeDocument;

            int oldRow = document.ViewportRow, oldCol = document.ViewportColumn;
            int newRow = 0, newCol = 0;
            VTUtils.CalculateAutoFitSize(newSize, document.Typeface, out newRow, out newCol);

            if (newRow == document.ViewportRow && newCol == document.ViewportColumn)
            {
                // 变化之后的行和列和现在的行和列一样，什么都不做
                return;
            }

            // 对Document执行Resize
            // 目前的实现在ubuntu下没问题，但是在Windows10操作系统上运行Windows命令行里的vim程序会有问题，可能是Windows下的vim程序兼容性导致的？暂时先这样
            // 遇到过一种情况：如果终端名称不正确，比如XTerm，那么当行数增加的时候，光标会移动到该行的最右边，终端名称改成xterm就没问题了
            // 目前的实现思路是：如果是减少行，那么从第一行开始删除；如果是增加行，那么从最后一行开始新建行。不考虑ScrollMargin

            this.MainDocumentResize(this.mainDocument, newRow, newCol);
            this.AlternateDocumentResize(this.alternateDocument, newRow, newCol);

            // 重绘当前显示的文档
            this.activeDocument.RequestInvalidate();

            // 给HOST发送修改行列的请求
            this.sessionTransport.Resize(newRow, newCol);

            if (this.ViewportChanged != null)
            {
                this.ViewportChanged(this, newRow, newCol);
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

        private VTDocumentOptions CreateDocumentOptions(string name, XTermSession sessionInfo, IDocumentRenderer drawingDocument)
        {
            string fontFamily = sessionInfo.GetOption<string>(OptionKeyEnum.THEME_FONT_FAMILY);
            double fontSize = sessionInfo.GetOption<double>(OptionKeyEnum.THEME_FONT_SIZE);

            VTypeface typeface = drawingDocument.GetTypeface(fontSize, fontFamily);
            typeface.BackgroundColor = sessionInfo.GetOption<string>(OptionKeyEnum.THEME_BACKGROUND_COLOR);
            typeface.ForegroundColor = sessionInfo.GetOption<string>(OptionKeyEnum.THEME_FONT_COLOR);

            VTSize displaySize = drawingDocument.ContentSize;
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
                ScrollbackMax = sessionInfo.GetOption<int>(OptionKeyEnum.TERM_MAX_SCROLLBACK),
                Typeface = typeface,
                Controller = drawingDocument,
                SelectionColor = sessionInfo.GetOption<string>(OptionKeyEnum.THEME_SELECTION_COLOR),
            };

            return documentOptions;
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
            // 调整大小前前先滚动到底，不然会有问题
            this.activeDocument.ScrollToBottom();

            document.Resize(newRow, newCol);
        }

        private void AlternateDocumentResize(VTDocument document, int newRow, int newCol)
        {
            document.Resize(newRow, newCol);

            // 备用缓冲区，因为SSH主机会重新打印所有字符，所以清空所有文本
            document.DeleteAll();
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

        #endregion

        #region VTDispatchHandler

        public void PlayBell()
        { }

        public void ForwardTab()
        {
            // 执行TAB键的动作（在当前光标位置处打印4个空格）
            // 微软的terminal项目里说，如果光标在该行的最右边，那么再次执行TAB的时候光标会自动移动到下一行，目前先不这么做

            VTDebug.Context.WriteInteractive("ForwardTab", string.Empty);

            int tabSize = 4;
            activeDocument.SetCursor(CursorRow, CursorCol + tabSize);
        }

        public void CarriageReturn()
        {
            // CR
            // 把光标移动到行开头
            int oldRow = this.CursorRow;
            int oldCol = this.CursorCol;

            activeDocument.SetCursor(CursorRow, 0);

            int newRow = this.CursorRow;
            int newCol = this.CursorCol;

            VTDebug.Context.WriteInteractive("CarriageReturn", "{0},{1},{2},{3}", oldRow, oldCol, newRow, newCol);
        }

        // 换行和反向换行
        public void LineFeed()
        {
            // LF
            // 滚动边距会影响到LF（DECSTBM_SetScrollingRegion），在实现的时候要考虑到滚动边距

            // 想像一下有一个打印机往一张纸上打字，当打印机想移动到下一行打字的时候，它会发出一个LineFeed指令，让纸往上移动一行
            // LineFeed，字面意思就是把纸上的下一行喂给打印机使用
            // 换行逻辑是把第一行拿到最后一行，要考虑到scrollMargin

            // 要换行的文档
            VTDocument document = this.activeDocument;

            // 可滚动区域的第一行和最后一行
            VTextLine head = document.FirstLine.FindNext(document.ScrollMarginTop);
            VTextLine last = document.LastLine.FindPrevious(document.ScrollMarginBottom);

            VTHistory history = document.History;

            // 光标所在行是可滚动区域的最后一行
            // 也表示即将滚动
            if (last == ActiveLine)
            {
                // 光标在滚动区域的最后一行，那么把滚动区域的第一行拿到滚动区域最后一行的下面
                logger.DebugFormat("LineFeed，光标在可滚动区域最后一行，向下滚动一行");

                document.SwapLine(head, last);

                if (this.IsAlternate)
                {
                    // 备用缓冲区重置被移动的行
                    head.DeleteAll();
                }
                else
                {
                    // 换行之后记录历史行
                    // 注意用户可以输入Backspace键或者上下左右光标键来修改最新行的内容，所以最新一行的内容是实时变化的，目前的解决方案是在渲染整个文档的时候去更新最后一个历史行的数据
                    // MainScrrenBuffer和AlternateScrrenBuffer里的行分别记录
                    // AlternateScreenBuffer是用来给man，vim等程序使用的
                    // 暂时只记录主缓冲区里的数据，备用缓冲区需要考虑下是否需要记录和怎么记录，因为VIM，Man等程序用的是备用缓冲区，用户是可以实时编辑缓冲区里的数据的

                    // 主缓冲区要创建一个新的VTHistoryLine
                    #region 更新滚动条的值

                    VTScrollInfo scrollInfo = document.Scrollbar;

                    // 滚动条滚动到底
                    // 计算滚动条可以滚动的最大值

                    int scrollMax = scrollInfo.ScrollMax + 1;
                    scrollMax = Math.Min(scrollMax, document.ScrollMax);
                    scrollInfo.ScrollMax = scrollMax;
                    scrollInfo.ScrollValue = scrollMax;

                    #endregion

                    VTextLine activeLine = this.ActiveLine;

                    VTHistoryLine historyLine = new VTHistoryLine();
                    activeLine.SetHistory(historyLine);
                    history.AddHistory(historyLine);

                    // 触发行被完全打印的事件
                    LinePrinted?.Invoke(this, last.History);
                }
            }
            else
            {
                // 光标不在可滚动区域的最后一行，说明可以直接移动光标
                logger.DebugFormat("LineFeed，光标在滚动区域内，直接移动光标到下一行");
                document.SetCursor(Cursor.Row + 1, Cursor.Column);

                VTextLine activeLine = this.ActiveLine;

                // 光标移动到该行的时候再加入历史记录
                if (!this.IsAlternate)
                {
                    // 有一些程序会在主缓冲区更新内容(top)，所以要判断该行是否已经被加入到了历史记录里
                    // 如果加入到了历史记录，那么就更新；如果没加入历史记录再加入历史记录
                    int physicsRow = activeLine.GetPhysicsRow();
                    if (!history.ContainHistory(physicsRow))
                    {
                        history.AddHistory(activeLine.History);

                        this.LinePrinted?.Invoke(this, activeLine.History);
                    }
                }
            }
        }

        public void RI_ReverseLineFeed()
        {
            // 和LineFeed相反，也就是把光标往上移一个位置
            // 在用man命令的时候往上滚动会触发这个指令
            // 反向换行 – 执行\n的反向操作，将光标向上移动一行，维护水平位置，如有必要，滚动缓冲区 *

            // 反向换行不增加新行，也不减少新行，保持总行数不变

            VTDocument document = this.activeDocument;

            // 可滚动区域的第一行和最后一行
            VTextLine head = document.FirstLine.FindNext(document.ScrollMarginTop);
            VTextLine last = document.LastLine.FindPrevious(document.ScrollMarginBottom);

            if (head == ActiveLine)
            {
                // 此时光标位置在可视区域的第一行
                logger.DebugFormat("RI_ReverseLineFeed，光标在可视区域第一行，向上移动一行并且可视区域往上移动一行");

                // 上移之后，删除整行数据，终端会重新打印该行数据的
                // 如果不删除的话，在man程序下有可能会显示重叠的信息
                // 复现步骤：man cc -> enter10次 -> help -> enter10次 -> q -> 一直按上键
                document.SwapLineReverse(last, head);

                last.DeleteAll();

                if (this.IsAlternate)
                {
                }
                else
                {
                    // 物理行号和scrollMargin无关，保持物理行号不变
                }
            }
            else
            {
                // 这里假设光标在可视区域里面
                // 实际上有可能光标在可视区域上的上面或者下面，但是目前还没找到判断方式

                // 光标位置在可视区域里面
                logger.DebugFormat("RI_ReverseLineFeed，光标在可视区域里，直接移动光标到上一行");
                document.SetCursor(Cursor.Row - 1, Cursor.Column);
            }
        }

        public void PrintCharacter(char ch)
        {
            // 根据测试得出结论：
            // 在VIM模式下输入中文字符，VIM会自动把光标往后移动2列，所以判断VIM里一个中文字符占用2列的宽度
            // 在正常模式下，如果遇到中文字符，也使用2列来显示
            // 也就是说，如果终端列数一共是80，那么可以显示40个中文字符，显示完40个中文字符后就要换行

            // 如果在shell里删除一个中文字符，那么会执行两次光标向后移动的动作，然后EraseLine - ToEnd
            // 由此可得出结论，不论是VIM还是shell，一个中文字符都是按照占用两列的空间来计算的

            // 用户输入的时候，如果滚动条没滚动到底，那么先把滚动条滚动到底
            // 不然会出现在VTDocument当前的最后一行打印字符的问题
            activeDocument.ScrollToBottom();

            char chPrint = this.TranslateCharacter(ch);

            // 创建并打印新的字符
            VTDebug.Context.WriteInteractive("Print", "{0},{1},{2}", CursorRow, CursorCol, chPrint);
            VTCharacter character = this.CreateCharacter(chPrint, activeDocument.AttributeState);
            activeDocument.PrintCharacter(character);
            activeDocument.SetCursor(CursorRow, CursorCol + character.ColumnSize);
        }

        public void EraseDisplay(VTEraseType eraseType)
        {
            switch (eraseType)
            {
                // terminal项目里是All和Scrollback执行的是一样的操作：In most terminals, this is done by moving the viewport into the scrollback, clearing out the current screen.
                // top和clear指令会执行EraseType.All，在其他的终端软件里top指令不会清空已经存在的行，而是把已经存在的行往上移动
                // 所以EraseType.All的动作和Scrollback一样执行

                case VTEraseType.All:
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
                            int displayRows = document.GetDisplayRows(scrollInfo.ScrollMax);

                            // 计算滚动条的最大值
                            int newScrollMax = scrollInfo.ScrollValue + displayRows;
                            newScrollMax = Math.Min(newScrollMax, document.ScrollMax);

                            if (newScrollMax == scrollInfo.ScrollMax)
                            {
                                return;
                            }

                            scrollInfo.ScrollMax = newScrollMax;
                            scrollInfo.ScrollValue = newScrollMax;

                            // 从ScrollMax开始显示
                            VTextLine textLine = document.FirstLine;
                            for (int i = 0; i < document.ViewportRow; i++)
                            {
                                VTHistoryLine historyLine;
                                if (document.History.TryGetHistory(scrollInfo.ScrollValue + i, out historyLine))
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
        }

        public void EL_EraseLine(VTEraseType eraseType)
        {
            // TODO：优化填充算法

            VTDebug.Context.WriteInteractive("EraseLine", "{0},{1},{2}", CursorRow, CursorCol, eraseType);

            switch (eraseType)
            {
                case VTEraseType.All:
                    {
                        for (int i = 0; i < this.ViewportColumn; i++)
                        {
                            VTCharacter character = this.CreateCharacter(' ', activeDocument.AttributeState);
                            this.activeDocument.PrintCharacter(character, i);
                        }

                        break;
                    }

                case VTEraseType.FromBeginning:
                    {
                        for (int i = 0; i <= this.CursorCol; i++)
                        {
                            VTCharacter character = this.CreateCharacter(' ', activeDocument.AttributeState);
                            this.activeDocument.PrintCharacter(character, i);
                        }

                        break;
                    }

                case VTEraseType.ToEnd:
                    {
                        for (int i = this.CursorCol; i < this.ViewportColumn; i++)
                        {
                            VTCharacter character = this.CreateCharacter(' ', activeDocument.AttributeState);
                            this.activeDocument.PrintCharacter(character, i);
                        }

                        break;
                    }

                default:
                    throw new NotImplementedException();
            }
        }

        // 字符操作
        public void DCH_DeleteCharacter(int count)
        {
            VTDebug.Context.WriteInteractive("DCH_DeleteCharacter", "{0},{1},{2}", CursorRow, CursorCol, count);
            activeDocument.DeleteCharacter(CursorCol, count);
        }

        public void ICH_InsertCharacter(int count)
        {
            activeDocument.InsertCharacters(CursorCol, count);
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

        // 行操作
        public void IL_InsertLine(int count)
        {
            VTDebug.Context.WriteInteractive("IL_InsertLine", "{0}", count);
            if (count > 0)
            {
                activeDocument.InsertLines(count);
            }
        }

        public void DL_DeleteLine(int count)
        {
            VTDebug.Context.WriteInteractive("DL_DeleteLine", "{0}", count);
            if (count > 0)
            {
                activeDocument.DeleteLines(count);
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

        public void DA_DeviceAttributes(List<int> parameters)
        {
            VTDebug.Context.WriteInteractive("DA_DeviceAttributes", string.Empty);
            VTDebug.Context.WriteInteractive(VTSendTypeEnum.DA_DeviceAttributes, DA_DeviceAttributesData);
            sessionTransport.Write(DA_DeviceAttributesData);
        }

        #region 光标控制

        // 下面的光标移动指令不能进行VTDocument的滚动
        // 光标的移动坐标是相对于可视区域内的坐标
        // 服务器发送过来的光标原点是从(1,1)开始的，我们程序里的是(0,0)开始的，所以要减1

        private void CursorMovePosition(int row, int col)
        {
            row = MTermUtils.Clamp(row, 0, this.ViewportRow - 1);
            col = MTermUtils.Clamp(col, 0, this.ViewportColumn - 1);

            this.activeDocument.SetCursor(row, col);
        }

        public void CUP_CursorPosition(int row, int col)
        {
            // 打开vim，输入i，然后按tab，虽然第一行的字符列数小于要移动到的col，但是vim还是会移动，所以这里把不足的列数使用空格补齐

            if (this.ActiveLine.Columns < col)
            {
                this.ActiveLine.PadColumns(col);
            }

            activeDocument.SetCursor(row, col);
        }

        public void CUF_CursorForward(int n)
        {
            int newRow = this.CursorRow;
            int newCol = this.CursorCol + n;

            this.CursorMovePosition(newRow, newCol);
        }

        public void CUB_CursorBackward(int n)
        {
            int newRow = this.CursorRow;
            int newCol = this.CursorCol - n;

            this.CursorMovePosition(newRow, newCol);
        }

        public void CUU_CursorUp(int n)
        {
            int newRow = this.CursorRow - n;
            int newCol = this.CursorCol;

            this.CursorMovePosition(newRow, newCol);
        }

        public void CUD_CursorDown(int n)
        {
            int newRow = this.CursorRow + n;
            int newCol = this.CursorCol;

            this.CursorMovePosition(newRow, newCol);
        }

        public void CHA_CursorHorizontalAbsolute(int col)
        {
            ActiveLine.PadColumns(col);
            activeDocument.SetCursor(CursorRow, col);
        }

        public void VPA_VerticalLinePositionAbsolute(int row)
        {
            activeDocument.SetCursor(row, CursorCol);
        }

        public void Backspace()
        {
            VTDebug.Context.WriteInteractive("Backspace", "{0},{1}", CursorRow, CursorCol);
            activeDocument.SetCursor(CursorRow, CursorCol - 1);
        }

        #endregion

        // 滚动控制
        public void SD_ScrollDown(List<int> parameters) { }
        public void SU_ScrollUp(List<int> parameters) { }

        // Margin
        public void DECSTBM_SetScrollingRegion(int topMargin, int bottomMargin)
        {
            activeDocument.SetScrollMargin(topMargin, bottomMargin);
        }

        public void DECSLRM_SetLeftRightMargins(int leftMargin, int rightMargin)
        {
        }


        public void DECSC_CursorSave()
        {
            VTDebug.Context.WriteInteractive("CursorSave", "{0},{1}", CursorRow, CursorCol);

            // 收到这个指令的时候把光标状态保存一下，等下次收到DECRC_CursorRestore再还原保存了的光标状态
            activeDocument.CursorSave();
        }

        public void DECRC_CursorRestore()
        {
            VTDebug.Context.WriteInteractive("CursorRestore", "{0},{1},{2},{3}", CursorRow, CursorCol, activeDocument.CursorState.Row, activeDocument.CursorState.Column);
            activeDocument.CursorRestore();
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
                        VTColorIndex colorIndex = TermUtils.GraphicsOptions2VTColorIndex(options);
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
                        VTColorIndex colorIndex = TermUtils.GraphicsOptions2VTColorIndex(options);
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

                default:
                    {
                        // TODO：
                        logger.ErrorFormat("未实现的SGR, {0}", options.ToString());
                        throw new NotImplementedException();
                    }
            }
        }

        #region DECPrivateMode

        public void DECCKM_CursorKeysMode(bool isApplicationMode)
        {
            VTDebug.Context.WriteInteractive("DECCKM_CursorKeysMode", "{0}", isApplicationMode);
            keyboard.SetCursorKeyMode(isApplicationMode);
        }

        public void DECANM_AnsiMode(bool isAnsiMode)
        {
            VTDebug.Context.WriteInteractive("DECANM_AnsiMode", "{0}", isAnsiMode);
            keyboard.SetAnsiMode(isAnsiMode);
        }

        public void DECKPAM_KeypadApplicationMode()
        {
            VTDebug.Context.WriteInteractive("DECKPAM_KeypadApplicationMode", string.Empty);
            keyboard.SetKeypadMode(true);
        }

        public void DECKPNM_KeypadNumericMode()
        {
            VTDebug.Context.WriteInteractive("DECKPNM_KeypadNumericMode", string.Empty);
            keyboard.SetKeypadMode(false);
        }

        public void DECAWM_AutoWrapMode(bool isAutoWrapMode)
        {
            VTDebug.Context.WriteInteractive("DECAWM_AutoWrapMode", "{0}", isAutoWrapMode);
            autoWrapMode = isAutoWrapMode;
            activeDocument.AutoWrapMode = isAutoWrapMode;
        }

        public void XTERM_BracketedPasteMode(bool enable)
        {
            VTDebug.Context.WriteInteractive("XTERM_BracketedPasteMode", "{0}", enable);
            xtermBracketedPasteMode = enable;
        }


        public void ATT610_StartCursorBlink(bool enable)
        {
            VTDebug.Context.WriteInteractive("ATT610_StartCursorBlink", "{0}", enable);
            Cursor.AllowBlink = enable;
        }

        public void DECTCEM_TextCursorEnableMode(bool enable)
        {
            VTDebug.Context.WriteInteractive("DECTCEM_TextCursorEnableMode", "{0}", enable);
            Cursor.IsVisible = enable;
        }

        #endregion

        public void ASB_AlternateScreenBuffer(bool enable)
        {
            if (enable)
            {
                // 使用备用缓冲区
                VTDebug.Context.WriteInteractive("UseAlternateScreenBuffer", string.Empty);

                uiSyncContext.Send(new SendOrPostCallback((o) =>
                {
                    mainDocument.SetVisible(false);
                    alternateDocument.SetVisible(true);
                }), null);

                activeDocument = alternateDocument;

                DocumentChanged?.Invoke(this, mainDocument, alternateDocument);
            }
            else
            {
                // 使用主缓冲区

                VTDebug.Context.WriteInteractive("UseMainScreenBuffer", string.Empty);

                uiSyncContext.Send(new SendOrPostCallback((o) =>
                {
                    mainDocument.SetVisible(true);
                    alternateDocument.SetVisible(false);
                }), null);

                alternateDocument.SetScrollMargin(0, 0);
                alternateDocument.EraseAll();
                alternateDocument.SetCursor(0, 0);
                alternateDocument.ClearAttribute();
                alternateDocument.Selection.Clear();

                activeDocument = mainDocument;

                DocumentChanged?.Invoke(this, alternateDocument, mainDocument);
            }
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

        private void SshMonitorThreadProc()
        {
        }

        #endregion
    }
}
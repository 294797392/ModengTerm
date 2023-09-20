using ModengTerm.Base.Enumerations;
using ModengTerm.Terminal.Document;
using ModengTerm.Terminal.Enumerations;
using ModengTerm.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using XTerminal;
using XTerminal.Base;
using XTerminal.Base.DataModels;
using XTerminal.Base.Definitions;
using XTerminal.Base.Enumerations;
using XTerminal.Document;
using XTerminal.Document.Rendering;
using XTerminal.Parser;
using XTerminal.Session;

namespace ModengTerm.Terminal.ViewModels
{
    /// <summary>
    /// 处理虚拟终端的所有逻辑
    /// </summary>
    public partial class VideoTerminal : OpenedSessionVM
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("VideoTerminal");

        private static readonly byte[] OS_OperatingStatusData = new byte[4] { 0x1b, (byte)'[', (byte)'0', (byte)'n' };
        private static readonly byte[] DA_DeviceAttributesData = new byte[7] { 0x1b, (byte)'[', (byte)'?', (byte)'1', (byte)':', (byte)'0', (byte)'c' };

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
        private SynchronizationContext uiSyncContext;

        /// <summary>
        /// 当前终端行数
        /// </summary>
        private int rowSize;

        /// <summary>
        /// 当前终端列数
        /// </summary>
        private int colSize;

        #region Mouse

        /// <summary>
        /// 鼠标滚轮滚动一次，滚动几行
        /// </summary>
        private int scrollDelta;

        /// <summary>
        /// 存储滚动条的信息
        /// </summary>
        private VTScrollInfo scrollInfo;

        #endregion

        /// <summary>
        /// DECAWM是否启用
        /// </summary>
        private bool autoWrapMode;

        private bool xtermBracketedPasteMode;

        private VTScrollback scrollback;

        #region SelectionRange

        /// <summary>
        /// 鼠标是否按下
        /// </summary>
        private bool isMouseDown;
        private VTPoint mouseDownPos;

        /// <summary>
        /// 当前鼠标是否处于Selection状态
        /// </summary>
        private bool selectionState;

        private IDrawingDocument selectionCanvas;
        /// <summary>
        /// 存储选中的文本信息
        /// </summary>
        private VTextSelection textSelection;

        #endregion

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
        private IVideoTerminal videoTerminal;

        /// <summary>
        /// 根据当前电脑键盘的按键状态，转换成标准的ANSI控制序列
        /// </summary>
        private VTKeyboard keyboard;

        /// <summary>
        /// 终端渲染区域相对于整个桌面的位置
        /// </summary>
        private VTRect vtRect;

        /// <summary>
        /// 终端颜色表
        /// ColorName -> RgbKey
        /// </summary>
        private Dictionary<string, string> colorTable;
        private string background;
        private string foreground;
        private double fontSize;
        private string fontFamily;

        /// <summary>
        /// 是否正在查找
        /// </summary>
        private bool find;

        #endregion

        #region 属性

        /// <summary>
        /// activeDocument的光标信息
        /// 该坐标是基于ViewableDocument的坐标
        /// Cursor的位置是下一个要打印的字符的位置
        /// </summary>
        public VTCursor Cursor { get { return this.activeDocument.Cursor; } }

        /// <summary>
        /// 获取当前光标所在行
        /// </summary>
        public VTextLine ActiveLine { get { return this.activeDocument.ActiveLine; } }

        /// <summary>
        /// 获取当前光标所在行
        /// </summary>
        public int CursorRow { get { return this.Cursor.Row; } }

        /// <summary>
        /// 获取当前光标所在列
        /// 下一个字符要显示的位置
        /// </summary>
        public int CursorCol { get { return this.Cursor.Column; } }

        /// <summary>
        /// 当前终端显示的画面
        /// </summary>
        public IDrawingDocument ActiveCanvas { get { return this.activeDocument.Canvas; } }

        /// <summary>
        /// 获取当前滚动条是否滚动到底了
        /// </summary>
        public bool ScrollAtBottom
        {
            get
            {
                return this.scrollInfo.ScrollAtBottom;
            }
        }

        /// <summary>
        /// 获取当前滚动条是否滚动到顶了
        /// </summary>
        public bool ScrollAtTop
        {
            get
            {
                return this.scrollInfo.ScrollAtTop;
            }
        }

        /// <summary>
        /// 获取滚动数据
        /// </summary>
        public VTScrollback Scrollback { get { return this.scrollback; } }

        public VTDocument ActiveDocument { get { return this.activeDocument; } }

        public SessionTransport SessionTransport { get { return this.sessionTransport; } }

        /// <summary>
        /// 当前终端的行数
        /// </summary>
        public int RowSize
        {
            get
            {
                return this.rowSize;
            }
            private set
            {
                if (this.rowSize != value)
                {
                    this.rowSize = value;
                    this.NotifyPropertyChanged("RowSize");
                }
            }
        }

        /// <summary>
        /// 当前终端的列数
        /// </summary>
        public int ColumnSize
        {
            get
            {
                return this.colSize;
            }
            private set
            {
                if (this.colSize != value)
                {
                    this.colSize = value;
                    this.NotifyPropertyChanged("ColumnSize");
                }
            }
        }

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
            this.videoTerminal = this.Content as IVideoTerminal;

            // DECAWM
            this.autoWrapMode = false;

            // 初始化变量

            this.isRunning = true;

            this.outputEncoding = Encoding.GetEncoding(sessionInfo.GetOption<string>(OptionKeyEnum.WRITE_ENCODING));
            this.scrollDelta = sessionInfo.GetOption<int>(OptionKeyEnum.MOUSE_SCROLL_DELTA);
            this.fontSize = sessionInfo.GetOption<double>(OptionKeyEnum.SSH_THEME_FONT_SIZE);
            this.fontFamily = sessionInfo.GetOption<string>(OptionKeyEnum.SSH_THEME_FONT_FAMILY);
            this.colorTable = sessionInfo.GetOption<Dictionary<string, string>>(OptionKeyEnum.SSH_TEHEM_COLOR_TABLE);
            this.background = sessionInfo.GetOption<string>(OptionKeyEnum.SSH_THEME_BACK_COLOR);
            this.foreground = sessionInfo.GetOption<string>(OptionKeyEnum.SSH_THEME_FORE_COLOR);

            #region 初始化历史记录管理器

            this.scrollback = new VTMemoryScrollback();
            this.scrollback.Initialize();

            #endregion

            #region 初始化终端大小

            this.vtRect = this.videoTerminal.GetDisplayRect();

            this.sizeMode = sessionInfo.GetOption<TerminalSizeModeEnum>(OptionKeyEnum.SSH_TERM_SIZE_MODE);
            switch (this.sizeMode)
            {
                case TerminalSizeModeEnum.AutoFit:
                    {
                        this.CalculateAutoFitSize(this.vtRect, out this.rowSize, out this.colSize);

                        // 算完真正的终端大小之后重新设置一下参数，因为在SessionDriver里是直接使用SessionInfo里的参数的
                        sessionInfo.SetOption<int>(OptionKeyEnum.SSH_TERM_ROW, this.rowSize);
                        sessionInfo.SetOption<int>(OptionKeyEnum.SSH_TERM_COL, this.colSize);
                        break;
                    }

                case TerminalSizeModeEnum.Fixed:
                    {
                        this.rowSize = sessionInfo.GetOption<int>(OptionKeyEnum.SSH_TERM_ROW);
                        this.colSize = sessionInfo.GetOption<int>(OptionKeyEnum.SSH_TERM_COL);
                        break;
                    }

                default:
                    throw new NotImplementedException();
            }

            #endregion

            #region 初始化键盘

            this.keyboard = new VTKeyboard();
            this.keyboard.Encoding = this.outputEncoding;
            this.keyboard.SetAnsiMode(true);
            this.keyboard.SetKeypadMode(false);

            #endregion

            #region 初始化终端解析器

            this.vtParser = new VTParser();
            this.vtParser.ActionEvent += VtParser_ActionEvent;
            this.vtParser.Initialize();

            #endregion

            #region 初始化TextSelection

            this.selectionCanvas = this.videoTerminal.CreateDocument();
            this.videoTerminal.AddCanvas(this.selectionCanvas);
            this.textSelection = new VTextSelection();
            this.textSelection.DrawingObject = this.selectionCanvas.CreateDrawingObject(this.textSelection);

            #endregion

            #region 初始化文档模型

            int fontSize = sessionInfo.GetOption<int>(OptionKeyEnum.SSH_THEME_FONT_SIZE);
            string fontFamily = sessionInfo.GetOption<string>(OptionKeyEnum.SSH_THEME_FONT_FAMILY);

            // 计算光标大小
            VTextMetrics cursorSize = this.videoTerminal.MeasureText(" ", fontSize, fontFamily);

            VTDocumentOptions documentOptions = new VTDocumentOptions()
            {
                ColumnSize = this.colSize,
                RowSize = this.rowSize,
                DECPrivateAutoWrapMode = false,
                CursorStyle = sessionInfo.GetOption<VTCursorStyles>(OptionKeyEnum.SSH_THEME_CURSOR_STYLE),
                CursorColor = sessionInfo.GetOption<string>(OptionKeyEnum.SSH_THEME_CURSOR_COLOR),
                CursorSpeed = sessionInfo.GetOption<VTCursorSpeeds>(OptionKeyEnum.SSH_THEME_CURSOR_SPEED),
                CursorSize = new VTSize(cursorSize.Width, cursorSize.Height),
                FontFamily = fontFamily,
                FontSize = fontSize,
                ForegroundColor = this.foreground,
                BackgroundColor = this.background,
                ColorTable = this.colorTable
            };
            this.mainDocument = new VTDocument(documentOptions, this.videoTerminal.CreateDocument(), false) { Name = "MainDocument" };
            this.alternateDocument = new VTDocument(documentOptions, this.videoTerminal.CreateDocument(), true) { Name = "AlternateDocument" };
            this.activeDocument = this.mainDocument;
            this.videoTerminal.AddCanvas(this.mainDocument.Canvas);

            #endregion

            #region 初始化光标

            this.activeDocument.Cursor.RequestInvalidate();
            this.alternateDocument.Cursor.RequestInvalidate();

            #endregion

            #region 初始化滚动条

            this.scrollInfo = new VTScrollInfo();

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

            this.mainDocument.Dispose();
            this.alternateDocument.Dispose();

            this.scrollback.Release();
        }

        /// <summary>
        /// 复制当前选中的行
        /// </summary>
        public void CopySelection()
        {
            string text = this.CreateContent(ContentScopeEnum.SaveSelected, LogFileTypeEnum.PlainText);

            // 调用剪贴板API复制到剪贴板
            Clipboard.SetText(text);
        }

        /// <summary>
        /// 将剪贴板的数据发送给会话
        /// </summary>
        /// <returns>发送成功返回SUCCESS，失败返回错误码</returns>
        public int Paste()
        {
            string text = Clipboard.GetText();
            if (string.IsNullOrEmpty(text))
            {
                return ResponseCode.SUCCESS;
            }

            byte[] bytes = this.outputEncoding.GetBytes(text);

            int code = this.sessionTransport.Write(bytes);
            if (code != ResponseCode.SUCCESS)
            {
                logger.ErrorFormat("粘贴数据失败, {0}", code);
            }

            return code;
        }

        /// <summary>
        /// 选中全部的文本
        /// </summary>
        public void SelectAll()
        {
            VTHistoryLine lastHistoryLine = this.scrollback.LastLine;

            this.textSelection.SetRange(this.activeDocument, this.vtRect, 0, 0, lastHistoryLine.PhysicsRow, lastHistoryLine.Characters.Count - 1);
            this.textSelection.RequestInvalidate();
        }

        /// <summary>
        /// 根据scope生成内容
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="fileType"></param>
        /// <returns></returns>
        public string CreateContent(ContentScopeEnum scope, LogFileTypeEnum fileType)
        {
            List<List<VTCharacter>> characters = new List<List<VTCharacter>>();
            int startIndex = 0, endIndex = 0;

            switch (scope)
            {
                case ContentScopeEnum.SaveAll:
                    {
                        if (this.activeDocument.IsAlternate)
                        {
                            // 备用缓冲区直接保存VTextLine
                            VTextLine current = this.activeDocument.FirstLine;
                            while (current != null)
                            {
                                characters.Add(current.Characters);
                                current = current.NextLine;
                            }

                            startIndex = 0;
                            endIndex = Math.Max(0, this.activeDocument.LastLine.Characters.Count - 1);
                        }
                        else
                        {
                            List<VTHistoryLine> historyLines;
                            if (!this.scrollback.TryGetHistories(this.scrollback.FirstLine.PhysicsRow, this.scrollback.LastLine.PhysicsRow, out historyLines))
                            {
                                logger.ErrorFormat("SaveAll失败, 有的历史记录为空");
                                return string.Empty;
                            }

                            characters.AddRange(historyLines.Select(v => v.Characters));
                            startIndex = 0;
                            endIndex = this.scrollback.LastLine.Characters.Count - 1;
                        }
                        break;
                    }

                case ContentScopeEnum.SaveDocument:
                    {
                        VTextLine current = this.activeDocument.FirstLine;
                        while (current != null)
                        {
                            characters.Add(current.Characters);
                            current = current.NextLine;
                        }

                        startIndex = 0;
                        endIndex = Math.Max(0, this.activeDocument.LastLine.Characters.Count - 1);
                        break;
                    }

                case ContentScopeEnum.SaveSelected:
                    {
                        if (this.textSelection.IsEmpty)
                        {
                            return string.Empty;
                        }

                        int topRow, bottomRow;
                        this.textSelection.Normalize(out topRow, out bottomRow, out startIndex, out endIndex);

                        List<VTHistoryLine> historyLines;
                        if (!this.scrollback.TryGetHistories(topRow, bottomRow - topRow + 1, out historyLines))
                        {
                            logger.ErrorFormat("SaveSelected失败, 有的历史记录为空");
                            return string.Empty;
                        }

                        characters.AddRange(historyLines.Select(v => v.Characters));
                        break;
                    }

                default:
                    throw new NotImplementedException();
            }

            CreateContentParameter parameter = new CreateContentParameter()
            {
                SessionName = this.Session.Name,
                CharactersList = characters,
                StartCharacterIndex = startIndex,
                EndCharacterIndex = endIndex,
                ColorTable = this.colorTable,
                ContentType = fileType,
                Background = this.background,
                FontSize = this.fontSize,
                FontFamily = this.fontFamily,
                Foreground = this.foreground
            };

            return VTUtils.CreateContent(parameter);
        }

        /// <summary>
        /// 处理用户输入
        /// 用户每输入一个字符，就调用一次这个函数
        /// </summary>
        /// <param name="input">用户输入信息</param>
        public void HandleInput(UserInput input)
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

        /// <summary>
        /// 滚动并重新渲染
        /// </summary>
        /// <param name="scrollValue">要滚动到的值</param>
        public void ScrollTo(int scrollValue)
        {
            if (this.ScrollToHistory(scrollValue))
            {
                this.PerformDrawing(this.activeDocument);
            }
        }

        #endregion

        #region 实例方法

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
        /// update anything about ui
        /// 如果需要布局则进行布局
        /// 如果不需要布局，那么就看是否需要重绘某些文本行
        /// </summary>
        /// <param name="document">要渲染的文档</param>
        /// <param name="scrollValue">
        /// 是否要移动滚动条，设置为-1表示不移动滚动条
        /// 注意这里只是更新UI上的滚动条位置，并不会实际的去滚动内容
        /// </param>
        private void PerformDrawing(VTDocument document)
        {
            // 当前行的Y方向偏移量
            double offsetY = 0;

            this.uiSyncContext.Send((state) =>
            {
                #region 渲染文档

                VTextLine next = document.FirstLine;

                while (next != null)
                {
                    // 更新Y偏移量信息
                    next.OffsetY = offsetY;

                    next.RequestInvalidate();

                    // 更新下一个文本行的Y偏移量
                    offsetY += next.Height;

                    next = next.NextLine;
                }

                #endregion

                #region 更新光标位置

                VTCursor cursor = document.Cursor;
                int cursorCol = cursor.Column;
                VTextLine activeLine = document.FindLine(document.ActivePhysicsRow);

                // 如果显示的是主缓冲区，那么光标在最后一行的时候才更新
                // 如果显示的是备用缓冲区，光标可以在任意一个位置显示，那么直接渲染光标

                if (activeLine != null)
                {
                    cursor.OffsetY = activeLine.OffsetY;
                    // 有可能有中文字符，一个中文字符占用2列
                    // 先通过光标所在列找到真正的字符所在列
                    int characterIndex = activeLine.FindCharacterIndex(cursorCol - 1);
                    VTRect rect = activeLine.MeasureLine(characterIndex, 1);
                    cursor.OffsetX = rect.Right;
                }
                else
                {
                    // 此时说明有滚动，有滚动的情况下直接隐藏光标
                    cursor.OffsetX = int.MinValue;
                    cursor.OffsetX = int.MinValue;
                }

                cursor.RequestInvalidate();

                #endregion

                #region 移动滚动条

                if (this.scrollInfo.Dirty)
                {
                    this.videoTerminal.SetScrollInfo(this.scrollInfo);
                    this.scrollInfo.SetDirty(false);
                }

                #endregion

                #region 更新选中区域

                if (!this.textSelection.IsEmpty)
                {
                    this.textSelection.UpdateRange(this.activeDocument, this.vtRect);
                }
                this.textSelection.RequestInvalidate();

                #endregion

            }, null);

            document.SetArrangeDirty(false);
        }

        /// <summary>
        /// 当光标在容器外面移动的时候，进行滚动
        /// </summary>
        /// <param name="mousePosition">当前鼠标的坐标</param>
        /// <param name="vtc">相对于电脑显示器的画布的边界框</param>
        /// <returns>是否执行了滚动动作</returns>
        private void ScrollIfCursorOutsideDocument(VTPoint mousePosition, VTRect vtc)
        {
            // 要滚动到的目标行
            int scrollTarget = -1;

            if (mousePosition.Y < 0)
            {
                // 光标在容器上面
                if (!this.ScrollAtTop)
                {
                    // 不在最上面，往上滚动一行
                    scrollTarget = this.scrollInfo.ScrollValue - 1;
                }
            }
            else if (mousePosition.Y > vtc.Height)
            {
                // 光标在容器下面
                if (!this.ScrollAtBottom)
                {
                    // 往下滚动一行
                    scrollTarget = this.scrollInfo.ScrollValue + 1;
                }
            }

            if (scrollTarget != -1)
            {
                this.ScrollToHistory(scrollTarget);
            }
        }

        /// <summary>
        /// 使用像素坐标对VTextLine做命中测试
        /// </summary>
        /// <param name="mousePosition">鼠标坐标</param>
        /// <param name="vtc">相对于电脑显示器的画布的边界框，也是鼠标的限定范围</param>
        /// <param name="pointer">存储命中测试结果的变量</param>
        /// <remarks>如果传递进来的鼠标位置在窗口外，那么会把鼠标限定在距离鼠标最近的Surface边缘处</remarks>
        /// <returns>
        /// 是否获取成功
        /// 当光标不在某一行或者不在某个字符上的时候，就获取失败
        /// </returns>
        private bool GetTextPointer(VTPoint mousePosition, VTRect vtc, VTextPointer pointer)
        {
            double mouseX = mousePosition.X;
            double mouseY = mousePosition.Y;

            VTDocument document = this.activeDocument;

            #region 先计算鼠标位于哪一行上

            VTextLine cursorLine = null;

            if (mouseY < 0)
            {
                // 光标在画布的上面，那么命中的行数就是第一行
                cursorLine = document.FirstLine;
            }
            else if (mouseY > vtc.Height)
            {
                // 光标在画布的下面，那么命中的行数是最后一行
                cursorLine = document.LastLine;
            }
            else
            {
                // 光标在画布中，那么做命中测试
                // 找到鼠标所在行
                cursorLine = HitTestHelper.HitTestVTextLine(document.FirstLine, mouseY);
                if (cursorLine == null)
                {
                    // 这里说明鼠标没有在任何一行上
                    logger.DebugFormat("没有找到鼠标位置对应的行, cursorY = {0}", mouseY);
                    return false;
                }
            }

            #endregion

            #region 再计算鼠标悬浮于哪个字符上

            int characterIndex = 0;

            if (mouseX < 0)
            {
                // 鼠标在画布左边，那么悬浮的就是第一个字符
                characterIndex = 0;
            }
            if (mouseX > vtc.Width)
            {
                // 鼠标在画布右边，那么悬浮的就是最后一个字符
                characterIndex = cursorLine.Characters.Count;
            }
            else
            {
                // 鼠标的水平方向在画布中间，那么做字符命中测试
                VTRect characterBounds;
                if (!HitTestHelper.HitTestVTCharacter(cursorLine, mouseX, out characterIndex, out characterBounds))
                {
                    // 没有命中字符
                    return false;
                }
            }

            #endregion

            // 命中成功再更新TextPointer，保证pointer不为空
            pointer.PhysicsRow = cursorLine.PhysicsRow;
            pointer.CharacterIndex = characterIndex;

            return true;
        }

        /// <summary>
        /// 自动判断ch是多字节字符还是单字节字符，创建一个VTCharacter
        /// </summary>
        /// <param name="ch">多字节或者单字节的字符</param>
        /// <returns></returns>
        private VTCharacter CreateCharacter(object ch, List<VTextAttributeState> attributeStates)
        {
            if (ch is char)
            {
                return VTCharacter.Create(Convert.ToChar(ch), 2, VTCharacterFlags.MulitByteChar, attributeStates);
            }
            else
            {
                return VTCharacter.Create(Convert.ToChar(ch), 1, VTCharacterFlags.SingleByteChar, attributeStates);
            }
        }

        /// <summary>
        /// 滚动到指定的历史记录
        /// 并更新UI上的滚动条位置
        /// 注意该方法不会重新渲染界面，只修改文档模型
        /// </summary>
        /// <param name="scrollValue">要显示的第一行历史记录</param>
        /// <returns>如果进行了滚动，那么返回true，如果因为某种原因没进行滚动，那么返回false</returns>
        private bool ScrollToHistory(int scrollValue)
        {
            // 要滚动的值和当前值是一样的，也不滚动
            if (this.scrollInfo.ScrollValue == scrollValue)
            {
                return false;
            }

            // 只移动主缓冲区
            VTDocument scrollDocument = this.mainDocument;

            // 要滚动到的值
            int newScroll = scrollValue;
            // 滚动之前的值
            int oldScroll = this.scrollInfo.ScrollValue;

            // 需要进行滚动的行数
            int scrolledRows = Math.Abs(newScroll - oldScroll);

            // 终端行大小
            int rows = this.rowSize;

            #region 更新要显示的行

            if (scrolledRows >= rows)
            {
                // 此时说明把所有行都滚动到屏幕外了，需要重新显示所有行

                // 遍历显示
                VTextLine currentTextLine = scrollDocument.FirstLine;
                for (int i = 0; i < rows; i++)
                {
                    VTHistoryLine historyLine;
                    if (!this.scrollback.TryGetHistory(scrollValue + i, out historyLine))
                    {
                        // 百分之百不可能找不到
                        throw new NotImplementedException();
                    }

                    currentTextLine.SetHistory(historyLine);
                    currentTextLine = currentTextLine.NextLine;
                }
            }
            else
            {
                // 此时说明只需要更新移动出去的行就可以了
                if (newScroll > oldScroll)
                {
                    // 往下滚动，把上面的拿到下面，从第一行开始

                    // 从当前文档的最后一行的下一行开始显示
                    int lastRow = scrollDocument.LastLine.PhysicsRow + 1;

                    for (int i = 0; i < scrolledRows; i++)
                    {
                        // 该值永远是第一行，因为下面被Move到最后一行了
                        VTextLine firstLine = scrollDocument.FirstLine;
                        scrollDocument.MoveLine(firstLine, VTextLine.MoveOptions.MoveToLast);

                        VTHistoryLine historyLine;
                        if (this.scrollback.TryGetHistory(lastRow + i, out historyLine))
                        {
                            firstLine.SetHistory(historyLine);
                        }
                        else
                        {
                            // 有可能会找不到，找不到就清空
                            // 打开终端 -> clear -> 滚到最上面 -> 再往下滚，就会复现
                            firstLine.EraseAll();
                        }
                    }
                }
                else
                {
                    // 往上滚动，把下面的拿到上面，从最后一行开始

                    // 从当前文档的第一行的上一行开始显示
                    int firstRow = scrollDocument.FirstLine.PhysicsRow - 1;

                    for (int i = 0; i < scrolledRows; i++)
                    {
                        VTHistoryLine historyLine;
                        if (!this.scrollback.TryGetHistory(firstRow - i, out historyLine))
                        {
                            // 百分之百不可能找不到！！！
                            throw new NotImplementedException();
                        }

                        VTextLine lastLine = scrollDocument.LastLine;
                        lastLine.SetHistory(historyLine);
                        scrollDocument.MoveLine(lastLine, VTextLine.MoveOptions.MoveToFirst);
                    }
                }
            }

            scrollDocument.SetArrangeDirty(true);

            #endregion

            // 更新当前滚动条的值
            this.scrollInfo.ScrollValue = scrollValue;

            return true;
        }

        /// <summary>
        /// 把主缓冲区滚动到底
        /// </summary>
        private void ScrollToBottom()
        {
            if (!this.ScrollAtBottom)
            {
                this.ScrollToHistory(this.scrollInfo.ScrollMax);
            }
        }

        /// <summary>
        /// 根据当前屏幕大小计算终端的自适应大小
        /// </summary>
        /// <param name="vtc">屏幕大小</param>
        /// <param name="rowSize">计算出来的终端行数</param>
        /// <param name="colSize">计算出来的终端列数</param>
        private void CalculateAutoFitSize(VTRect vtc, out int rowSize, out int colSize)
        {
            // 自适应屏幕大小
            // 计算一共有多少行，和每行之间的间距是多少
            // 使用空白字符计算一行的高度，然后用屏幕高度除以一行的高度
            VTextMetrics metrics = this.videoTerminal.MeasureText(" ", this.fontSize, this.fontFamily);

            // 终端控件的初始宽度和高度，在打开Session的时候动态设置
            rowSize = (int)Math.Floor(vtc.Height / metrics.Height);
            colSize = (int)Math.Floor(vtc.Width / metrics.Width);
        }

        #endregion

        #region 事件处理器

        private void VtParser_ActionEvent(VTActions action, object parameter)
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

                        if (!this.activeDocument.IsAlternate)
                        {
                            // 用户输入的时候，如果滚动条没滚动到底，那么先把滚动条滚动到底
                            // 不然会出现在VTDocument当前的最后一行打印字符的问题
                            this.ScrollToBottom();
                        }

                        // 创建并打印新的字符
                        char ch = Convert.ToChar(parameter);
                        VTDebug.Context.WriteInteractive(action, "{0},{1},{2}", this.CursorRow, this.CursorCol, ch);
                        VTCharacter character = this.CreateCharacter(parameter, this.activeDocument.AttributeStates);
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
                        this.ScrollToBottom();

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

                            // 更新旧的最后一行和新的最后一行的历史记录
                            this.scrollback.UpdateHistory(oldLastLine);
                            this.scrollback.UpdateHistory(newLastLine);

                            #region 更新滚动条的值

                            // 滚动条滚动到底
                            // 计算滚动条可以滚动的最大值
                            int scrollMax = this.mainDocument.FirstLine.PhysicsRow;
                            if (scrollMax > 0)
                            {
                                // 更新滚动条的值
                                this.scrollInfo.ScrollMax = scrollMax;
                                this.scrollInfo.ScrollValue = scrollMax;
                            }

                            #endregion
                        }

                        break;
                    }

                case VTActions.RI_ReverseLineFeed:
                    {
                        // 和LineFeed相反，也就是把光标往上移一个位置
                        // 在用man命令的时候会触发这个指令
                        // 反向换行 – 执行\n的反向操作，将光标向上移动一行，维护水平位置，如有必要，滚动缓冲区 *
                        this.activeDocument.ReverseLineFeed();
                        VTDebug.Context.WriteInteractive(action, string.Empty);
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

                        int tabSize = 4;
                        for (int i = 0; i < tabSize; i++)
                        {
                            this.VtParser_ActionEvent(VTActions.Print, ' ');
                        }

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
                            case EraseType.Scrollback:
                                {
                                    // 相关命令：
                                    // MainDocument：clear
                                    // AlternateDocument：暂无

                                    // Erase Saved Lines
                                    // 模拟xshell的操作，把当前行移动到可视区域的第一行

                                    VTextLine firstLine = this.activeDocument.FirstLine;
                                    VTextLine lastLine = this.activeDocument.LastLine;

                                    // 当前终端里显示的行数
                                    int lines = this.ActiveLine.PhysicsRow - firstLine.PhysicsRow;

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
                                    this.scrollInfo.ScrollMax = scrollMax;
                                    this.scrollInfo.ScrollValue = scrollMax;

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

                            // 刚打开VIM就按空格键，此时VIM会响应一个CursorPosition向右移动一个单位的事件
                            // 此时要把光标向右移动一个单位
                            this.ActiveLine.PadColumns(newcol);
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
                        VTDebug.Context.WriteInteractive(action, "{0},{1},{2},{3}", this.CursorRow, this.CursorCol, parameter == null ? string.Empty : parameter.ToString(), this.ActiveLine.PhysicsRow);

                        // 打开VIM的时候，VIM会在打印第一行的~号的时候设置验色，然后把剩余的行全部打印，也就是说设置一次颜色可以对多行都生效
                        // 所以这里要记录下如果当前有文本特效被设置了，那么在行改变的时候也需要设置文本特效
                        // 缓存下来，每次打印字符的时候都要对ActiveLine Apply一下

                        if (action == VTActions.ReverseVideo)
                        {
                            string foreground = this.Session.GetOption<string>(OptionKeyEnum.SSH_THEME_FORE_COLOR);
                            string background = this.Session.GetOption<string>(OptionKeyEnum.SSH_THEME_BACK_COLOR);
                            VTColor backColor = VTColor.CreateFromRgbKey(foreground);
                            VTColor foreColor = VTColor.CreateFromRgbKey(background);
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
                        logger.ErrorFormat("未实现XTERM_BracketedPasteMode");
                        this.xtermBracketedPasteMode = Convert.ToBoolean(parameter);
                        break;
                    }

                case VTActions.ATT610_StartCursorBlink:
                    {
                        break;
                    }

                case VTActions.DECTCEM_TextCursorEnableMode:
                    {
                        break;
                    }

                #endregion

                #region 文本操作

                case VTActions.DCH_DeleteCharacter:
                    {
                        // 从指定位置删除n个字符，删除后的字符串要左对齐，默认删除1个字符
                        List<int> parameters = parameter as List<int>;
                        int count = VTParameter.GetParameter(parameters, 0, 1);
                        VTDebug.Context.WriteInteractive(action, "{0},{1},{2}", count, this.CursorRow, this.CursorCol);
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

                #endregion

                #region 上下滚动

                #endregion

                case VTActions.UseAlternateScreenBuffer:
                    {
                        VTDebug.Context.WriteInteractive(action, string.Empty);

                        IDrawingDocument remove = this.mainDocument.Canvas;
                        IDrawingDocument add = this.alternateDocument.Canvas;
                        this.videoTerminal.RemoveCanvas(remove);
                        this.videoTerminal.AddCanvas(add);

                        // 这里只重置行数，在用户调整窗口大小的时候需要执行终端的Resize操作
                        this.alternateDocument.SetScrollMargin(0, 0);
                        this.alternateDocument.EraseAll();
                        this.activeDocument = this.alternateDocument;

                        this.textSelection.Reset();
                        this.videoTerminal.SetScrollVisible(false);
                        break;
                    }

                case VTActions.UseMainScreenBuffer:
                    {
                        VTDebug.Context.WriteInteractive(action, string.Empty);

                        IDrawingDocument remove = this.alternateDocument.Canvas;
                        IDrawingDocument add = this.mainDocument.Canvas;
                        this.videoTerminal.RemoveCanvas(remove);
                        this.videoTerminal.AddCanvas(add);

                        this.mainDocument.DirtyAll();
                        this.activeDocument = this.mainDocument;

                        this.textSelection.Reset();
                        this.videoTerminal.SetScrollVisible(true);
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
                        int lines = this.rowSize;

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

            // 全部字符都处理完了之后，只渲染一次
            this.PerformDrawing(this.activeDocument);

            // 更新最新的历史行
            // 解决当一次性打印多个字符的时候，不需要每打印一个字符就更新历史行，而是等所有字符都打印完了再更新
            // 不要在Print事件里保存历史记录，因为可能会连续触发多次Print事件
            // 触发完多次Print事件后，会最后触发一次PerformDrawing，在PerformDrawing完了再保存最后一行历史行
            if (this.activeDocument == this.mainDocument)
            {
                this.scrollback.UpdateHistory(this.ActiveLine);
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


        public void OnMouseDown(IVideoTerminal vt, VTPoint location, int clickCount)
        {
            if (clickCount == 1)
            {
                this.isMouseDown = true;
                this.mouseDownPos = location;

                // 点击的时候先清除选中区域
                this.textSelection.Reset();
                this.textSelection.UpdateRange(this.activeDocument, this.vtRect);
                this.textSelection.RequestInvalidate();
            }
            else
            {
                // 双击就是选中单词
                // 三击就是选中整行内容

                int startIndex = 0, endIndex = 0;

                VTextLine lineHit = HitTestHelper.HitTestVTextLine(this.activeDocument.FirstLine, location.Y);
                if (lineHit == null)
                {
                    return;
                }

                switch (clickCount)
                {
                    case 2:
                        {
                            // 选中单词
                            string text = VTUtils.CreatePlainText(lineHit.Characters);
                            int characterIndex;
                            VTRect characterBounds;
                            if (!HitTestHelper.HitTestVTCharacter(lineHit, location.X, out characterIndex, out characterBounds))
                            {
                                return;
                            }
                            VDocumentUtils.GetSegement(text, characterIndex, out startIndex, out endIndex);
                            break;
                        }

                    case 3:
                        {
                            // 选中一整行
                            string text = VTUtils.CreatePlainText(lineHit.Characters);
                            startIndex = 0;
                            endIndex = text.Length - 1;
                            break;
                        }

                    default:
                        {
                            return;
                        }
                }

                this.textSelection.Reset();
                this.textSelection.Start.CharacterIndex = startIndex;
                this.textSelection.Start.PhysicsRow = lineHit.PhysicsRow;

                this.textSelection.End.CharacterIndex = endIndex;
                this.textSelection.End.PhysicsRow = lineHit.PhysicsRow;

                this.textSelection.UpdateRange(this.activeDocument, this.vtRect);
                this.textSelection.RequestInvalidate();
            }
        }

        public void OnMouseMove(IVideoTerminal vt, VTPoint location)
        {
            if (!this.isMouseDown)
            {
                return;
            }

            if (!this.selectionState)
            {
                // 此时说明开始选中操作
                this.selectionState = true;
                this.textSelection.Reset();
            }

            // 整理思路是算出来StartTextPointer和EndTextPointer之间的几何图形
            // 然后渲染几何图形，SelectionRange本质上就是一堆矩形
            VTextPointer startPointer = this.textSelection.Start;
            VTextPointer endPointer = this.textSelection.End;

            // 如果还没有测量起始字符，那么测量起始字符
            if (this.textSelection.Start.CharacterIndex == -1)
            {
                if (!this.GetTextPointer(location, this.vtRect, startPointer))
                {
                    // 没有命中起始字符，那么直接返回啥都不做
                    //logger.DebugFormat("没命中起始字符");
                    return;
                }
            }

            // 首先检测鼠标是否在Surface边界框的外面
            // 如果在Surface的外面并且行数超出了Surface可以显示的最多行数，那么根据鼠标方向进行滚动，每次滚动一行
            this.ScrollIfCursorOutsideDocument(location, this.vtRect);

            // 更新当前鼠标的命中信息，保存在endPointer里
            if (!this.GetTextPointer(location, this.vtRect, endPointer))
            {
                // 命中失败，不更新
                return;
            }

            #region 起始字符和结束字符测量出来的索引位置都是-1，啥都不做

            if (startPointer.CharacterIndex < 0 || endPointer.CharacterIndex < 0)
            {
                logger.WarnFormat("鼠标命中的起始字符和结束字符位置都小于0");
                return;
            }

            #endregion

            #region 起始字符和结束字符是同一个字符，啥都不做

            if (startPointer.CharacterIndex == endPointer.CharacterIndex)
            {
                //logger.WarnFormat("鼠标命中的起始字符和结束字符是相同字符");
                return;
            }

            #endregion

            // 重新渲染
            // PerformDrawing会更新TextSelection的形状
            this.PerformDrawing(this.activeDocument);
        }

        public void OnMouseUp(IVideoTerminal vt, VTPoint location)
        {
            this.isMouseDown = false;
            this.selectionState = false;
        }

        public void OnMouseWheel(IVideoTerminal vt, bool upper)
        {
            // 只有主缓冲区才可以用鼠标滚轮进行滚动
            // 备用缓冲区不可以滚动
            if (this.activeDocument.IsAlternate)
            {
                return;
            }

            int scrollValue = this.scrollInfo.ScrollValue;
            int scrollMax = this.scrollInfo.ScrollMax;

            if (upper)
            {
                // 向上滚动

                // 先判断是不是已经滚动到顶了
                if (this.ScrollAtTop)
                {
                    // 滚动到顶直接返回
                    return;
                }

                if (scrollValue < this.scrollDelta)
                {
                    // 一次可以全部滚完并且还有剩余
                    this.ScrollToHistory(0);
                }
                else
                {
                    this.ScrollToHistory(scrollValue - this.scrollDelta);
                }
            }
            else
            {
                // 向下滚动

                if (this.ScrollAtBottom)
                {
                    // 滚动到底直接返回
                    return;
                }

                // 剩余可以往下滚动的行数
                int remainScroll = scrollMax - scrollValue;

                if (remainScroll >= this.scrollDelta)
                {
                    this.ScrollToHistory(scrollValue + this.scrollDelta);
                }
                else
                {
                    // 直接滚动到底
                    this.ScrollToHistory(scrollMax);
                }
            }

            // 重新渲染
            this.PerformDrawing(this.activeDocument);
        }

        public void OnSizeChanged(IVideoTerminal vt, VTRect vtc)
        {
            // 不管当前是什么状态，第一步先更新终端屏幕大小
            this.vtRect = vtc;

            // 有可能在大小改变的时候还没连接上终端
            if (this.Status != SessionStatusEnum.Connected)
            {
                return;
            }

            // 如果是固定大小的终端，那么什么都不做
            if (this.sizeMode == TerminalSizeModeEnum.Fixed)
            {
                return;
            }

            int newRows, newCols;
            this.CalculateAutoFitSize(vtc, out newRows, out newCols);

            // 如果行和列都没变化，那么就什么都不做
            if (this.rowSize == newRows && this.colSize == newCols)
            {
                return;
            }

            VTDebug.Context.WriteInteractive("ResizeTerminal", "{0},{1},{2},{3}", this.rowSize, this.colSize, newRows, newCols);

            // 缩放前先滚动到底，不然会有问题
            this.ScrollToBottom();

            // 对Document执行Resize
            // 目前的实现在ubuntu下没问题，但是在Windows10操作系统上运行Windows命令行里的vim程序会有问题，可能是Windows下的vim程序兼容性导致的，暂时先这样
            // 遇到过一种情况：如果终端名称不正确，比如XTerm，那么当行数增加的时候，光标会移动到该行的最右边，终端名称改成xterm就没问题了
            // 目前的实现思路是：如果是减少行，那么从第一行开始删除；如果是增加行，那么从最后一行开始新建行。不考虑ScrollMargin
            this.mainDocument.Resize(newRows, newCols);
            this.alternateDocument.Resize(newRows, newCols);

            #region 处理主缓冲区

            // 第一行的值就是滚动条的最大值
            int scrollMax = this.mainDocument.FirstLine.PhysicsRow;
            if (this.scrollInfo.ScrollMax != scrollMax)
            {
                this.scrollInfo.ScrollMax = scrollMax;
                this.scrollInfo.ScrollValue = scrollMax;

                // 从第一行开始重新渲染显示终端内容
                VTextLine currentLine = this.mainDocument.FirstLine;
                while (currentLine != null)
                {
                    VTHistoryLine historyLine;
                    if (this.scrollback.TryGetHistory(currentLine.PhysicsRow, out historyLine))
                    {
                        currentLine.SetHistory(historyLine);
                    }
                    else
                    {
                        // 没有找到要显示的滚动区域外的内容，说明已经全部显示了
                        // 但是还是要继续循环下去，因为相当于是把底部的文本行拿到了最上面，此时底部的文本行需要清空
                        currentLine.EraseAll();
                    }

                    currentLine = currentLine.NextLine;
                }
            }

            #endregion

            #region 处理备用缓冲区

            // 备用缓冲区，因为SSH主机会重新打印所有字符，所以清空所有文本
            this.alternateDocument.EraseAll();

            #endregion

            // 如果是修改行大小，那么会自动触发重绘
            // 如果是修改列，那么不会自动触发重绘，要手动重绘
            // 这里偷个懒，不管修改的是列还是行都重绘一次
            this.PerformDrawing(this.activeDocument);

            // 给SSH主机发个Resiz指令
            this.sessionTransport.Resize(newRows, newCols);

            // 更新界面上的行和列
            this.ColumnSize = newCols;
            this.RowSize = newRows;
        }

        #endregion
    }
}
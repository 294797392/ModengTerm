using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using XTerminal.Base;
using XTerminal.Document;
using XTerminal.Document.Rendering;
using XTerminal.Parser;
using XTerminal.Rendering;
using XTerminal.Session;

namespace XTerminal
{
    /// <summary>
    /// 处理虚拟终端的所有逻辑
    /// </summary>
    public class VideoTerminal
    {
        private enum OutsideScrollResult
        {
            /// <summary>
            /// 没滚动
            /// </summary>
            None,

            /// <summary>
            /// 鼠标往上滚动
            /// </summary>
            ScrollTop,

            /// <summary>
            /// 鼠标往下滚动
            /// </summary>
            ScrollDown
        }

        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("VideoTerminal");

        private static readonly byte[] OS_OperationStatusResponse = new byte[4] { (byte)'\x1b', (byte)'[', (byte)'0', (byte)'n' };
        private static readonly byte[] CPR_CursorPositionReportResponse = new byte[6] { (byte)'\x1b', (byte)'[', (byte)'0', (byte)';', (byte)'0', (byte)'R' };
        private static readonly byte[] DA_DeviceAttributesResponse = new byte[7] { 0x1b, (byte)'[', (byte)'?', (byte)'1', (byte)':', (byte)'0', (byte)'c' };

        #endregion

        #region 公开事件

        public event Action<VideoTerminal, SessionStatusEnum> SessionStatusChanged;

        #endregion

        #region 实例变量

        /// <summary>
        /// 与终端进行通信的信道
        /// </summary>
        private SessionBase session;

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

        private VTInitialOptions initialOptions;

        /// <summary>
        /// DECAWM是否启用
        /// </summary>
        private bool autoWrapMode;

        private bool xtermBracketedPasteMode;

        /// <summary>
        /// 闪烁光标的线程
        /// </summary>
        private Thread cursorBlinkingThread;

        #region History & Scroll

        /// <summary>
        /// 所有行
        /// Row(scrollValue) -> VTextLine
        /// </summary>
        private Dictionary<int, VTHistoryLine> historyLines;
        private VTHistoryLine activeHistoryLine;

        /// <summary>
        /// 记录滚动条滚动到底的时候，滚动条的值
        /// </summary>
        private int scrollMax;

        /// <summary>
        /// 记录当前滚动条滚动的值
        /// 也就是文档里的第一条历史记录的Row的值
        /// </summary>
        private int currentScroll;

        /// <summary>
        /// 当鼠标按下的时候，记录Canvas相对于屏幕的坐标
        /// </summary>
        private VTRect canvasRect;

        #endregion

        #region SelectionRange

        /// <summary>
        /// 鼠标是否按下
        /// </summary>
        private bool isMouseDown;
        private VTPoint mouseDownPos;

        /// <summary>
        /// 存储选中的文本信息
        /// </summary>
        private VTextSelection textSelection;

        #endregion

        private int renderCounter;
        private int dataReceivedCounter;

        /// <summary>
        /// 是否正在运行
        /// </summary>
        private bool isRunning;

        #endregion

        #region 属性

        /// <summary>
        /// activeDocument的光标信息
        /// 该坐标是基于ViewableDocument的坐标
        /// </summary>
        private VTCursor Cursor { get { return this.activeDocument.Cursor; } }

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
        /// </summary>
        public int CursorCol { get { return this.Cursor.Column; } }

        /// <summary>
        /// 文档渲染器
        /// </summary>
        public ITerminalSurface Surface { get { return this.activeDocument.Surface; } }

        /// <summary>
        /// 文档画布容器
        /// </summary>
        public ITerminalScreen SurfacePanel { get; set; }

        /// <summary>
        /// 根据当前电脑键盘的按键状态，转换成标准的ANSI控制序列
        /// </summary>
        public VTKeyboard Keyboard { get; private set; }

        public VTextOptions TextOptions { get; private set; }

        /// <summary>
        /// 获取当前滚动条是否滚动到底了
        /// </summary>
        public bool ScrollAtBottom
        {
            get
            {
                return this.currentScroll == this.scrollMax;
            }
        }

        /// <summary>
        /// 获取当前滚动条是否滚动到顶了
        /// </summary>
        public bool ScrollAtTop
        {
            get
            {
                return this.currentScroll == 0;
            }
        }

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
        /// <param name="options"></param>
        public void Initialize(VTInitialOptions options)
        {
            this.initialOptions = options;
            this.uiSyncContext = SynchronizationContext.Current;

            // DECAWM
            this.autoWrapMode = this.initialOptions.TerminalProperties.DECPrivateAutoWrapMode;

            // 初始化变量
            this.historyLines = new Dictionary<int, VTHistoryLine>();
            this.TextOptions = new VTextOptions();
            this.textSelection = new VTextSelection();

            this.isRunning = true;

            #region 初始化键盘

            this.Keyboard = new VTKeyboard();
            this.Keyboard.SetAnsiMode(true);
            this.Keyboard.SetKeypadMode(false);

            #endregion

            #region 初始化终端解析器

            this.vtParser = new VTParser();
            this.vtParser.ActionEvent += VtParser_ActionEvent;
            this.vtParser.Initialize();

            #endregion

            #region 初始化鼠标事件

            this.SurfacePanel.InputEvent += this.VideoTerminal_InputEvent;
            this.SurfacePanel.ScrollChanged += this.CanvasPanel_ScrollChanged;
            this.SurfacePanel.VTMouseDown += this.CanvasPanel_VTMouseDown;
            this.SurfacePanel.VTMouseMove += this.CanvasPanel_VTMouseMove;
            this.SurfacePanel.VTMouseUp += this.CanvasPanel_VTMouseUp;

            #endregion

            #region 初始化文档模型

            VTDocumentOptions documentOptions = new VTDocumentOptions()
            {
                ColumnSize = initialOptions.TerminalProperties.Columns,
                RowSize = initialOptions.TerminalProperties.Rows,
                DECPrivateAutoWrapMode = initialOptions.TerminalProperties.DECPrivateAutoWrapMode,
                CursorStyle = initialOptions.CursorOption.Style,
                Interval = initialOptions.CursorOption.Interval,
                CanvasCreator = this.SurfacePanel
            };
            this.mainDocument = new VTDocument(documentOptions) { Name = "MainDocument" };
            this.alternateDocument = new VTDocument(documentOptions) { Name = "AlternateDocument" };
            this.activeDocument = this.mainDocument;
            this.activeHistoryLine = VTHistoryLine.Create(0, null, this.ActiveLine);
            this.historyLines[0] = this.activeHistoryLine;
            this.SurfacePanel.AddSurface(this.activeDocument.Surface);

            #endregion

            #region 初始化光标

            this.Surface.Draw(this.Cursor);
            this.cursorBlinkingThread = new Thread(this.CursorBlinkingThreadProc);
            this.cursorBlinkingThread.IsBackground = true;
            this.cursorBlinkingThread.Start();
            // 先初始化备用缓冲区的光标渲染上下文
            this.alternateDocument.Surface.Draw(this.alternateDocument.Cursor);

            #endregion

            #region 连接终端通道

            SessionBase session = SessionFactory.Create(options);
            session.StatusChanged += this.VTSession_StatusChanged;
            session.DataReceived += this.VTSession_DataReceived;
            session.Connect();
            this.session = session;

            #endregion
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Release()
        {
            this.isRunning = false;

            this.vtParser.ActionEvent -= VtParser_ActionEvent;
            this.vtParser.Release();

            this.SurfacePanel.InputEvent -= this.VideoTerminal_InputEvent;
            this.SurfacePanel.ScrollChanged -= this.CanvasPanel_ScrollChanged;
            this.SurfacePanel.VTMouseDown -= this.CanvasPanel_VTMouseDown;
            this.SurfacePanel.VTMouseMove -= this.CanvasPanel_VTMouseMove;
            this.SurfacePanel.VTMouseUp -= this.CanvasPanel_VTMouseUp;

            this.cursorBlinkingThread.Join();

            this.session.StatusChanged -= this.VTSession_StatusChanged;
            this.session.DataReceived -= this.VTSession_DataReceived;
            this.session.Disconnect();

            this.historyLines.Clear();
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
                        this.session.Write(OS_OperationStatusResponse);
                        break;
                    }

                case StatusType.CPR_CursorPositionReport:
                    {
                        // Result is CSI r ; c R
                        int cursorRow = this.CursorRow;
                        int cursorCol = this.CursorCol;
                        CPR_CursorPositionReportResponse[2] = (byte)cursorRow;
                        CPR_CursorPositionReportResponse[4] = (byte)cursorCol;
                        this.session.Write(CPR_CursorPositionReportResponse);
                        break;
                    }

                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// 如果需要布局则进行布局
        /// 如果不需要布局，那么就看是否需要重绘某些文本行
        /// </summary>
        /// <param name="document">要渲染的文档</param>
        /// <param name="scrollValue">
        /// 是否要移动滚动条，设置为-1表示不移动滚动条
        /// 注意这里只是更新UI上的滚动条位置，并不会实际的去滚动
        /// </param>
        private void DrawDocument(VTDocument document, int scrollValue = -1)
        {
            // 当前行的Y方向偏移量
            double offsetY = 0;

            bool arrangeDirty = document.IsArrangeDirty;
            bool activeLineDirty = this.ActiveLine.IsRenderDirty;

            this.uiSyncContext.Send((state) =>
            {
                #region 渲染文档

                VTextLine next = document.FirstLine;

                while (next != null)
                {
                    // 更新Y偏移量信息
                    next.OffsetY = offsetY;

                    if (next.IsRenderDirty)
                    {
                        // 此时说明该行有字符变化，需要重绘
                        this.Surface.Draw(next);
                        //logger.ErrorFormat("renderCounter = {0}", this.renderCounter++);
                    }
                    else if (next.IsMeasureDirety)
                    {
                        // 字符没有变化，那么只重新测量然后更新一下文本的偏移量就好了
                        this.Surface.MeasureLine(next);
                    }

                    if (arrangeDirty)
                    {
                        this.Surface.Arrange(next, next.OffsetX, next.OffsetY);
                    }

                    // 更新下一个文本行的Y偏移量
                    offsetY += next.Height;

                    // 如果最后一行渲染完毕了，那么就退出
                    if (next == document.LastLine)
                    {
                        break;
                    }

                    next = next.NextLine;
                }

                #endregion

                #region 渲染光标

                this.Cursor.OffsetY = this.ActiveLine.OffsetY;
                this.Cursor.OffsetX = this.Surface.MeasureBlock(this.ActiveLine, this.ActiveLine.FindCharacterIndex(this.CursorCol)).Width;
                this.Surface.Arrange(this.Cursor, this.Cursor.OffsetX, this.Cursor.OffsetY);

                #endregion

                #region 移动滚动条

                if (scrollValue != -1)
                {
                    this.SurfacePanel.ScrollTo(scrollValue);
                }

                #endregion

            }, null);

            if (this.activeDocument == this.mainDocument)
            {
                if (this.ScrollAtBottom)
                {
                    // 滚动到底了，说明是ActiveLine就是当前正在输入的行
                    // 更新下历史行的大小和文字，不然在执行光标选中的时候文本为空，会影响到测量
                    this.activeHistoryLine.Freeze(this.ActiveLine);
                }
            }

            document.SetArrangeDirty(false);
        }

        /// <summary>
        /// 滚动到指定的历史记录
        /// 并更新UI上的滚动条位置
        /// </summary>
        /// <param name="scrollValue">要显示的第一行历史记录</param>
        private void ScrollToHistory(int scrollValue)
        {
            this.currentScroll = scrollValue;

            // 终端可以显示的总行数
            int terminalRows = this.initialOptions.TerminalProperties.Rows;

            VTHistoryLine historyLine;
            if (!this.historyLines.TryGetValue(scrollValue, out historyLine))
            {
                logger.ErrorFormat("ScrollTo失败, 找不到对应的VTHistoryLine, scrollValue = {0}", scrollValue);
                return;
            }

            // 找到后面的行数显示
            VTHistoryLine currentHistory = historyLine;
            VTextLine currentTextLine = this.activeDocument.FirstLine;
            for (int i = 0; i < terminalRows; i++)
            {
                // 直接使用VTHistoryLine的List<VTCharacter>的引用
                // 冻结状态下的VTextLine不会再有修改了
                // 非冻结状态(ActiveLine)需要重新创建一个集合
                currentTextLine.SetHistory(currentHistory);
                currentHistory = currentHistory.NextLine;
                currentTextLine = currentTextLine.NextLine;
            }

            this.activeDocument.SetArrangeDirty(true);
            this.DrawDocument(this.activeDocument, scrollValue);
        }

        /// <summary>
        /// 当光标在容器外面的时候，进行滚动
        /// </summary>
        /// <param name="mousePosition">当前鼠标的坐标</param>
        /// <param name="canvasBoundary">相对于电脑显示器的画布的边界框</param>
        /// <returns>是否执行了滚动动作</returns>
        private OutsideScrollResult ScrollIfCursorOutsidePanel(VTPoint mousePosition, VTRect canvasBoundary)
        {
            OutsideScrollResult scrollResult = OutsideScrollResult.None;

            int scrollTarget = -1;

            if (mousePosition.Y < 0)
            {
                // 光标在容器上面
                if (!this.ScrollAtTop)
                {
                    // 不在最上面，往上滚动一行
                    scrollTarget = this.currentScroll - 1;
                    scrollResult = OutsideScrollResult.ScrollTop;
                }
            }
            else if (mousePosition.Y > canvasBoundary.Height)
            {
                // 光标在容器下面
                if (!this.ScrollAtBottom)
                {
                    scrollTarget = this.currentScroll + 1;
                    scrollResult = OutsideScrollResult.ScrollDown;
                }
            }

            if (scrollTarget != -1)
            {
                this.ScrollToHistory(scrollTarget);
            }

            return scrollResult;
        }

        /// <summary>
        /// 使用像素坐标对VTextLine做命中测试
        /// </summary>
        /// <param name="mousePosition">鼠标坐标</param>
        /// <param name="canvasBoundary">相对于电脑显示器的画布的边界框，也是鼠标的限定范围</param>
        /// <param name="pointer">存储命中测试结果的变量</param>
        /// <remarks>如果传递进来的鼠标位置在窗口外，那么会把鼠标限定在距离鼠标最近的Canvas边缘处</remarks>
        /// <returns>
        /// 是否获取成功
        /// 当光标不在某一行或者不在某个字符上的时候，就获取失败
        /// </returns>
        private bool GetTextPointer(VTPoint mousePosition, VTRect canvasBoundary, VTextPointer pointer)
        {
            double mouseX = mousePosition.X;
            double mouseY = mousePosition.Y;

            if (mouseX < 0)
            {
                mouseX = 0;
            }
            if (mouseX > canvasBoundary.Width)
            {
                mouseX = canvasBoundary.Width;
            }

            if (mouseY < 0)
            {
                mouseY = 0;
            }
            if (mouseY > canvasBoundary.Height)
            {
                mouseY = canvasBoundary.Height;
            }

            pointer.IsCharacterHit = false;
            pointer.CharacterIndex = -1;

            #region 先找到鼠标悬浮的历史行

            // 有可能当前有滚动，所以要从历史行里开始找
            // 先获取到当前屏幕上显示的历史行的首行

            VTHistoryLine topHistoryLine;
            if (!this.historyLines.TryGetValue(this.currentScroll, out topHistoryLine))
            {
                logger.ErrorFormat("GetTextPointer失败, 不存在历史行记录, currentScroll = {0}", this.currentScroll);
                return false;
            }

            // 当前行的Y偏移量
            double offsetY = 0;
            int termLines = this.initialOptions.TerminalProperties.Rows;
            VTHistoryLine lineHit = topHistoryLine;
            for (int i = 0; i < termLines; i++)
            {
                VTRect bounds = new VTRect(0, offsetY, lineHit.Width, lineHit.Height);

                if (bounds.Top <= mouseY && bounds.Bottom >= mouseY)
                {
                    break;
                }

                offsetY += bounds.Height;

                lineHit = lineHit.NextLine;

                if (lineHit == null)
                {
                    // 当行数少于可显示的总行数的时候，会发生这种情况
                    // 比如在终端刚打开的时候
                    break;
                }
            }

            // 这里说明鼠标没有在任何一个行上
            if (lineHit == null)
            {
                logger.ErrorFormat("没有找到鼠标位置对应的行, cursorY = {0}", mouseY);
                return false;
            }

            pointer.Line = lineHit;

            #endregion

            #region 再计算鼠标悬浮于哪个字符上

            string text = lineHit.Text;
            for (int i = 0; i < text.Length; i++)
            {
                VTRect characterBounds = this.Surface.MeasureCharacter(lineHit, i);

                if (characterBounds.Left <= mouseX && characterBounds.Right >= mouseX)
                {
                    // 测量出来的边界框没有Y边距，要手动设置
                    characterBounds.Y = offsetY;
                    // 鼠标命中了字符，使用命中的字符的边界框
                    pointer.CharacterBounds = characterBounds;
                    pointer.CharacterIndex = i;
                    pointer.IsCharacterHit = true;
                    break;
                }
            }

            if (!pointer.IsCharacterHit)
            {
                // 如果没命中字符，那么以鼠标当前位置为中心生成一个空白字符的CharacterBounds
                pointer.CharacterBounds = new VTRect(mouseX - 5, offsetY, 10, lineHit.Height);
            }

            #endregion

            return true;
        }

        /// <summary>
        /// 当文档滚动后，TextPointer的位置就会变化
        /// 这个函数计算TextPointer当前在文档里的正确位置
        /// </summary>
        /// <returns></returns>
        private bool UpdateTextPointerBounds(VTextPointer pointer)
        {
            VTHistoryLine topHistoryLine;
            if (!this.historyLines.TryGetValue(this.currentScroll, out topHistoryLine))
            {
                logger.ErrorFormat("GetTextPointerRect失败, 不存在历史行记录, currentScroll = {0}", this.currentScroll);
                return false;
            }

            // 当前行的Y偏移量
            double offsetY = 0;
            int termLines = this.initialOptions.TerminalProperties.Rows;
            VTHistoryLine currentLine = topHistoryLine;
            for (int i = 0; i < termLines; i++)
            {
                if (currentLine.Row == pointer.Row)
                {
                    // 找到了滚动后的Pointer指向的历史行, 更新
                    // 值更新Y坐标就可以了
                    VTRect characterBounds = pointer.CharacterBounds;
                    characterBounds.Y = offsetY;
                    pointer.CharacterBounds = characterBounds;
                    return true;
                }

                offsetY += currentLine.Height;

                currentLine = currentLine.NextLine;

                if (currentLine == null)
                {
                    // 当行数少于可显示的总行数的时候，会发生这种情况
                    // 比如在终端刚打开的时候
                    return false;
                }
            }

            // 此时说明TextPointer指向的行已经不在当前显示区域里了，被滚走了
            return false;
        }

        /// <summary>
        /// 获取pointer2相对于pointer1的方向
        /// </summary>
        /// <param name="pointer1">第一个pointer</param>
        /// <param name="pointer2">第二个pointer</param>
        /// <returns></returns>
        private TextPointerPositions GetTextPointerPosition(VTextPointer pointer1, VTextPointer pointer2)
        {
            VTRect rect1 = pointer1.CharacterBounds;
            VTRect rect2 = pointer2.CharacterBounds;
            int row1 = pointer1.Row;
            int row2 = pointer2.Row;

            if (rect2.X == rect1.X && row2 < row1)
            {
                return TextPointerPositions.Top;
            }
            else if (rect2.X > rect1.X && row2 < row1)
            {
                return TextPointerPositions.RightTop;
            }
            else if (rect2.X > rect1.X && row2 == row1)
            {
                return TextPointerPositions.Right;
            }
            else if (rect2.X > rect1.X && row2 > row1)
            {
                return TextPointerPositions.RightBottom;
            }
            else if (rect2.X == rect1.X && row2 > row1)
            {
                return TextPointerPositions.Bottom;
            }
            else if (rect2.X < rect1.X && row2 > row1)
            {
                return TextPointerPositions.LeftBottom;
            }
            else if (rect2.X < rect1.X && row2 == row1)
            {
                return TextPointerPositions.Left;
            }
            else if (rect2.X < rect1.X && row2 < row1)
            {
                return TextPointerPositions.LeftTop;
            }
            else
            {
                return TextPointerPositions.Original;
            }
        }

        /// <summary>
        /// 自动判断ch是多字节字符还是单字节字符，创建一个VTCharacter
        /// </summary>
        /// <param name="ch">多字节或者单字节的字符</param>
        /// <returns></returns>
        private VTCharacter CreateCharacter(object ch)
        {
            if (ch is char)
            {
                return VTCharacter.Create(Convert.ToChar(ch), 2, VTCharacterFlags.MulitByteChar);
            }
            else
            {
                return VTCharacter.Create(Convert.ToChar(ch), 1, VTCharacterFlags.SingleByteChar);
            }
        }

        #endregion

        #region 事件处理器

        /// <summary>
        /// 当用户按下按键的时候触发
        /// </summary>
        /// <param name="terminal"></param>
        private void VideoTerminal_InputEvent(ITerminalScreen canvasPanel, VTInputEvent evt)
        {
            byte[] bytes = this.Keyboard.TranslateInput(evt);

            // 这里输入的都是键盘按键
            int code = this.session.Write(bytes);
            if (code != ResponseCode.SUCCESS)
            {
                logger.ErrorFormat("处理输入异常, {0}", ResponseCode.GetMessage(code));
            }
        }

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

                        char ch = Convert.ToChar(parameter);
                        logger.DebugFormat("Print:{0}, cursorRow = {1}, cursorCol = {2}", ch, this.CursorRow, this.CursorCol);
                        VTCharacter character = this.CreateCharacter(parameter);
                        this.activeDocument.PrintCharacter(this.ActiveLine, character, this.CursorCol);
                        this.activeDocument.SetCursor(this.CursorRow, this.CursorCol + character.ColumnSize);
                        break;
                    }

                case VTActions.CarriageReturn:
                    {
                        // CR
                        // 把光标移动到行开头
                        this.activeDocument.SetCursor(this.CursorRow, 0);
                        logger.DebugFormat("CarriageReturn, cursorRow = {0}, cursorCol = {1}", this.CursorRow, this.CursorCol);
                        break;
                    }

                case VTActions.FF:
                case VTActions.VT:
                case VTActions.LF:
                    {
                        // LF
                        // 滚动边距会影响到LF（DECSTBM_SetScrollingRegion），在实现的时候要考虑到滚动边距

                        if (this.activeDocument == this.mainDocument)
                        {
                            // 如果滚动条不在最底部，那么先把滚动条滚动到底
                            if (!this.ScrollAtBottom)
                            {
                                this.ScrollToHistory(this.scrollMax);
                            }
                        }

                        // 想像一下有一个打印机往一张纸上打字，当打印机想移动到下一行打字的时候，它会发出一个LineFeed指令，让纸往上移动一行
                        // LineFeed，字面意思就是把纸上的下一行喂给打印机使用
                        this.activeDocument.LineFeed();
                        logger.DebugFormat("LineFeed, cursorRow = {0}, cursorCol = {1}, {2}", this.CursorRow, this.CursorCol, action);

                        // 换行之后记录历史行
                        // 注意用户可以输入Backspace键或者上下左右光标键来修改最新行的内容，所以最新一行的内容是实时变化的，目前的解决方案是在渲染整个文档的时候去更新最后一个历史行的数据
                        // 只记录MainScrrenBuffer里的行，AlternateScrrenBuffer里的行不记录。AlternateScreenBuffer是用来给man，vim等程序使用的
                        if (this.activeDocument == this.mainDocument)
                        {
                            // 可以确保换行之前的行已经被用户输入完了，不会被更改了，所以这里冻结一下换行之前的历史行的数据，冻结之后，该历史行的数据就不会再更改了
                            // 有几种特殊情况：
                            // 1. 如果主机一次性返回了多行数据，那么有可能前面的几行都没有测量，所以这里要先判断上一行是否有测量过
                            if (this.ActiveLine.PreviousLine.IsMeasureDirety)
                            {
                                this.Surface.MeasureLine(this.ActiveLine.PreviousLine);
                            }
                            this.activeHistoryLine.Freeze(this.ActiveLine.PreviousLine);

                            // 再创建最新行的历史行
                            // 先测量下最新的行，确保有高度
                            this.Surface.MeasureLine(this.ActiveLine);
                            int historyIndex = this.activeHistoryLine.Row + 1;
                            VTHistoryLine historyLine = VTHistoryLine.Create(historyIndex, this.activeHistoryLine, this.ActiveLine);
                            this.historyLines[historyIndex] = historyLine;
                            this.activeHistoryLine = historyLine;

                            // 滚动条滚动到底
                            int terminalRows = this.initialOptions.TerminalProperties.Rows;
                            int scrollMax = historyIndex - terminalRows + 1;
                            if (scrollMax > 0)
                            {
                                this.scrollMax = scrollMax;
                                this.currentScroll = scrollMax;
                                logger.DebugFormat("scrollMax = {0}", scrollMax);
                                this.uiSyncContext.Send((state) =>
                                {
                                    this.SurfacePanel.UpdateScrollInfo(scrollMax);
                                    this.SurfacePanel.ScrollToEnd(ScrollOrientation.Down);
                                }, null);
                            }
                        }

                        break;
                    }

                case VTActions.RI_ReverseLineFeed:
                    {
                        // 和LineFeed相反，也就是把光标往上移一个位置
                        // 在用man命令的时候会触发这个指令
                        // 反向换行 – 执行\n的反向操作，将光标向上移动一行，维护水平位置，如有必要，滚动缓冲区 *
                        this.activeDocument.ReverseLineFeed();
                        logger.DebugFormat("ReverseLineFeed");
                        break;
                    }

                #region Erase

                case VTActions.EL_EraseLine:
                    {
                        List<int> parameters = parameter as List<int>;
                        EraseType eraseType = (EraseType)VTParameter.GetParameter(parameters, 0, 0);
                        logger.DebugFormat("EL_EraseLine, eraseType = {0}, cursorRow = {1}, cursorCol = {2}", eraseType, this.CursorRow, this.CursorCol);
                        this.activeDocument.EraseLine(this.ActiveLine, this.CursorCol, eraseType);
                        break;
                    }

                case VTActions.ED_EraseDisplay:
                    {
                        List<int> parameters = parameter as List<int>;
                        EraseType eraseType = (EraseType)VTParameter.GetParameter(parameters, 0, 0);
                        logger.DebugFormat("ED_EraseDisplay, eraseType = {0}, cursorRow = {1}, cursorCol = {2}", eraseType, this.CursorRow, this.CursorCol);
                        this.activeDocument.EraseDisplay(this.ActiveLine, this.CursorCol, eraseType);
                        break;
                    }

                #endregion

                #region 光标移动

                // 下面的光标移动指令不能进行VTDocument的滚动
                // 光标的移动坐标是相对于可视区域内的坐标

                case VTActions.BS:
                    {
                        this.activeDocument.SetCursor(this.CursorRow, this.CursorCol - 1);
                        logger.DebugFormat("CursorBackward, cursorRow = {0}, cursorCol = {1}", this.CursorRow, this.CursorCol);
                        break;
                    }

                case VTActions.CursorBackward:
                    {
                        List<int> parameters = parameter as List<int>;
                        int n = VTParameter.GetParameter(parameters, 0, 1);
                        this.activeDocument.SetCursor(this.CursorRow, this.CursorCol - n);
                        logger.DebugFormat("CursorBackward, cursorRow = {0}, cursorCol = {1}", this.CursorRow, this.CursorCol);
                        break;
                    }

                case VTActions.CUF_CursorForward:
                    {
                        List<int> parameters = parameter as List<int>;
                        int n = VTParameter.GetParameter(parameters, 0, 1);
                        this.activeDocument.SetCursor(this.CursorRow, this.CursorCol + n);
                        break;
                    }

                case VTActions.CUU_CursorUp:
                    {
                        List<int> parameters = parameter as List<int>;
                        int n = VTParameter.GetParameter(parameters, 0, 1);
                        this.activeDocument.SetCursor(this.CursorRow - n, this.CursorCol);
                        break;
                    }

                case VTActions.CUD_CursorDown:
                    {
                        List<int> parameters = parameter as List<int>;
                        int n = VTParameter.GetParameter(parameters, 0, 1);
                        this.activeDocument.SetCursor(this.CursorRow + n, this.CursorCol);
                        break;
                    }

                case VTActions.CUP_CursorPosition:
                    {
                        List<int> parameters = parameter as List<int>;

                        int row = 0, col = 0;
                        if (parameters.Count == 2)
                        {
                            // VT的光标原点是(1,1)，我们程序里的是(0,0)，所以要减1
                            row = parameters[0] - 1;
                            col = parameters[1] - 1;
                        }
                        else
                        {
                            // 如果没有参数，那么说明就是定位到原点(0,0)
                        }

                        logger.DebugFormat("CUP_CursorPosition, row = {0}, col = {1}", row, col);
                        this.activeDocument.SetCursor(row, col);
                        break;
                    }

                #endregion

                #region 文本特效

                case VTActions.PlayBell:
                case VTActions.Bold:
                case VTActions.Foreground:
                case VTActions.Background:
                case VTActions.DefaultAttributes:
                case VTActions.DefaultBackground:
                case VTActions.DefaultForeground:
                case VTActions.Underline:
                case VTActions.UnderlineUnset:
                case VTActions.Faint:
                case VTActions.ItalicsUnset:
                case VTActions.CrossedOutUnset:
                case VTActions.DoublyUnderlined:
                case VTActions.DoublyUnderlinedUnset:
                case VTActions.ReverseVideo:
                case VTActions.ReverseVideoUnset:
                    break;

                #endregion

                #region DECPrivateMode

                case VTActions.DECANM_AnsiMode:
                    {
                        bool enable = Convert.ToBoolean(parameter);
                        logger.DebugFormat("DECANM_AnsiMode, enable = {0}", enable);
                        this.Keyboard.SetAnsiMode(enable);
                        break;
                    }

                case VTActions.DECCKM_CursorKeysMode:
                    {
                        bool enable = Convert.ToBoolean(parameter);
                        logger.DebugFormat("DECCKM_CursorKeysMode, enable = {0}", enable);
                        this.Keyboard.SetCursorKeyMode(enable);
                        break;
                    }

                case VTActions.DECKPAM_KeypadApplicationMode:
                    {
                        logger.DebugFormat("DECKPAM_KeypadApplicationMode");
                        this.Keyboard.SetKeypadMode(true);
                        break;
                    }

                case VTActions.DECKPNM_KeypadNumericMode:
                    {
                        logger.DebugFormat("DECKPNM_KeypadNumericMode");
                        this.Keyboard.SetKeypadMode(false);
                        break;
                    }

                case VTActions.DECAWM_AutoWrapMode:
                    {
                        bool enable = Convert.ToBoolean(parameter);
                        this.autoWrapMode = enable;
                        logger.DebugFormat("DECAWM_AutoWrapMode, enable = {0}", enable);
                        this.activeDocument.DECPrivateAutoWrapMode = enable;
                        break;
                    }

                case VTActions.XTERM_BracketedPasteMode:
                    {
                        this.xtermBracketedPasteMode = Convert.ToBoolean(parameter);
                        logger.ErrorFormat("未实现XTERM_BracketedPasteMode");
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
                        logger.ErrorFormat("DCH_DeleteCharacter, {0}, cursorPos = {1}", count, this.CursorCol);
                        this.activeDocument.DeleteCharacter(this.ActiveLine, this.CursorCol, count);
                        break;
                    }

                case VTActions.ICH_InsertCharacter:
                    {
                        // 在当前光标处插入N个空白字符,这会将所有现有文本移到右侧。 向右溢出屏幕的文本会被删除
                        // 目前没发现这个操作对终端显示有什么影响，所以暂时不实现
                        List<int> parameters = parameter as List<int>;
                        int count = VTParameter.GetParameter(parameters, 0, 1);
                        logger.ErrorFormat("未实现InsertCharacters, {0}, cursorPos = {1}", count, this.CursorCol);
                        break;
                    }

                #endregion

                #region 上下滚动

                #endregion

                case VTActions.UseAlternateScreenBuffer:
                    {
                        logger.DebugFormat("UseAlternateScreenBuffer");

                        ITerminalSurface remove = this.mainDocument.Surface;
                        ITerminalSurface add = this.alternateDocument.Surface;
                        this.SurfacePanel.SwitchSurface(remove, add);

                        // 这里只重置行数，在用户调整窗口大小的时候需要执行终端的Resize操作
                        this.alternateDocument.SetScrollMargin(0, 0);
                        this.alternateDocument.DeleteAll();
                        this.activeDocument = this.alternateDocument;
                        break;
                    }

                case VTActions.UseMainScreenBuffer:
                    {
                        logger.DebugFormat("UseMainScreenBuffer");

                        ITerminalSurface remove = this.alternateDocument.Surface;
                        ITerminalSurface add = this.mainDocument.Surface;
                        this.SurfacePanel.SwitchSurface(remove, add);

                        this.mainDocument.DirtyAll();
                        this.activeDocument = this.mainDocument;
                        break;
                    }

                case VTActions.DSR_DeviceStatusReport:
                    {
                        List<int> parameters = parameter as List<int>;
                        StatusType statusType = (StatusType)Convert.ToInt32(parameters[0]);
                        logger.DebugFormat("DSR_DeviceStatusReport, statusType = {0}", statusType);
                        this.PerformDeviceStatusReport(statusType);
                        break;
                    }

                case VTActions.DA_DeviceAttributes:
                    {
                        logger.DebugFormat("DA_DeviceAttributes");
                        this.session.Write(DA_DeviceAttributesResponse);
                        break;
                    }

                case VTActions.DECSTBM_SetScrollingRegion:
                    {
                        // 视频终端的规范里说，如果topMargin等于bottomMargin，或者bottomMargin大于屏幕高度，那么忽略这个指令
                        // 边距还会影响插入行 (IL) 和删除行 (DL)、向上滚动 (SU) 和向下滚动 (SD) 修改的行。

                        // Notes on DECSTBM
                        // * The value of the top margin (Pt) must be less than the bottom margin (Pb).
                        // * The maximum size of the scrolling region is the page size
                        // * DECSTBM moves the cursor to column 1, line 1 of the page
                        // * https://github.com/microsoft/terminal/issues/1849

                        // 当前终端屏幕可显示的行数量
                        int lines = this.initialOptions.TerminalProperties.Rows;

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
                            logger.DebugFormat("DECSTBM_SetScrollingRegion参数不正确，bottomMargin大于当前屏幕总行数, bottomMargin = {0}, lines = {1}", bottomMargin, lines);
                            return;
                        }

                        // topMargin == 1表示默认值，也就是没有marginTop，所以当topMargin == 1的时候，marginTop改为0
                        int marginTop = topMargin == 1 ? 0 : topMargin;
                        // bottomMargin == 控制台高度表示默认值，也就是没有marginBottom，所以当bottomMargin == 控制台高度的时候，marginBottom改为0
                        int marginBottom = lines - bottomMargin;
                        logger.DebugFormat("SetScrollingRegion, topMargin = {0}, bottomMargin = {1}", marginTop, marginBottom);
                        this.activeDocument.SetScrollMargin(marginTop, marginBottom);
                        break;
                    }

                case VTActions.IL_InsertLine:
                    {
                        // 将 <n> 行插入光标位置的缓冲区。 光标所在的行及其下方的行将向下移动。
                        List<int> parameters = parameter as List<int>;
                        int lines = VTParameter.GetParameter(parameters, 0, 1);
                        logger.DebugFormat("IL_InsertLine, lines = {0}", lines);
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
                        logger.DebugFormat("DL_DeleteLine, lines = {0}", lines);
                        if (lines > 0)
                        {
                            this.activeDocument.DeleteLines(this.ActiveLine, lines);
                        }
                        break;
                    }

                default:
                    {
                        logger.WarnFormat("未执行的VTAction, {0}", action);
                        break;
                    }
            }
        }

        private void VTSession_DataReceived(SessionBase client, byte[] bytes)
        {
            //string str = string.Join(",", bytes.Select(v => v.ToString()).ToList());
            //logger.InfoFormat("Received, {0}", str);
            this.vtParser.ProcessCharacters(bytes);

            // 全部字符都处理完了之后，只渲染一次

            this.DrawDocument(this.activeDocument);
            //logger.ErrorFormat("receivedCounter = {0}", this.dataReceivedCounter++);
            //this.activeDocument.Print();
            //logger.ErrorFormat("TotalRows = {0}", this.activeDocument.TotalRows);
        }

        private void VTSession_StatusChanged(object client, SessionStatusEnum status)
        {
            logger.InfoFormat("会话状态发生改变, {0}", status);
            if (this.SessionStatusChanged != null)
            {
                this.SessionStatusChanged(this, status);
            }
        }

        private void CursorBlinkingThreadProc()
        {
            while (this.isRunning)
            {
                VTCursor cursor = this.Cursor;

                cursor.IsVisible = !cursor.IsVisible;

                try
                {
                    double opacity = cursor.IsVisible ? 1 : 0;

                    this.uiSyncContext.Send((state) =>
                    {
                        this.Surface.SetOpacity(cursor, opacity);
                    }, null);
                }
                catch (Exception e)
                {
                    logger.ErrorFormat(string.Format("渲染光标异常, {0}", e));
                }
                finally
                {
                    Thread.Sleep(cursor.Interval);
                }
            }
        }


        /// <summary>
        /// 当滚动条滚动的时候触发
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="scrollValue">滚动到的行数</param>
        private void CanvasPanel_ScrollChanged(ITerminalScreen arg1, int scrollValue)
        {
            this.ScrollToHistory(scrollValue);
        }

        private void CanvasPanel_VTMouseUp(ITerminalScreen arg1, VTPoint cursorPos)
        {
            this.isMouseDown = false;

            this.textSelection.Reset();
        }

        private void CanvasPanel_VTMouseMove(ITerminalScreen arg1, VTPoint mousePosition)
        {
            if (!this.isMouseDown)
            {
                return;
            }

            // 首先检测鼠标是否在容器边界框的外面
            OutsideScrollResult scrollResult = this.ScrollIfCursorOutsidePanel(mousePosition, this.canvasRect);

            // 整理思路是算出来StartTextPointer和EndTextPointer之间的几何图形
            // 然后渲染几何图形，SelectionRange本质上就是一堆矩形
            VTextPointer startPointer = this.textSelection.Start;
            VTextPointer endPointer = this.textSelection.End;

            // 鼠标按下的时候不在行上，然后鼠标移动到了行上
            // 需要刷新起始的鼠标命中信息
            if (startPointer.Line == null)
            {
                if (!this.GetTextPointer(mousePosition, this.canvasRect, startPointer))
                {
                    return;
                }
            }

            // 得到当前鼠标的命中信息
            if (!this.GetTextPointer(mousePosition, this.canvasRect, endPointer))
            {
                // 只有在没有Outside滚动的时候，才返回
                // Outside滚动会导致GetTextPointer失败，虽然失败，还是要更新SelectionRange
                if (scrollResult == OutsideScrollResult.None)
                {
                    return;
                }
            }

            #region 鼠标移动后悬浮在相同的字符上没变化，不用操作

            if (startPointer.IsCharacterHit && endPointer.IsCharacterHit)
            {
                if (startPointer.CharacterIndex == endPointer.CharacterIndex)
                {
                    return;
                }
            }

            #endregion

            // 先算鼠标的移动方向
            TextPointerPositions pointerPosition = this.GetTextPointerPosition(this.textSelection.Start, this.textSelection.End);

            switch (pointerPosition)
            {
                case TextPointerPositions.Original:
                    {
                        break;
                    }

                // 这两个是鼠标在同一行上移动
                case TextPointerPositions.Right:
                case TextPointerPositions.Left:
                    {
                        VTRect rect1 = startPointer.CharacterBounds;
                        VTRect rect2 = endPointer.CharacterBounds;

                        double xmin = Math.Min(rect1.X, rect2.X);
                        double xmax = Math.Max(rect1.X, rect2.X);
                        double x = xmin;
                        double y = rect1.Y;
                        double width = xmax - xmin;
                        double height = rect1.Height;

                        VTRect bounds = new VTRect(x, y, width, height);
                        this.textSelection.Ranges.Clear();
                        this.textSelection.Ranges.Add(bounds);
                        break;
                    }

                // 其他的是鼠标上下移动
                default:
                    {
                        this.textSelection.Ranges.Clear();

                        // 构建上边和下边的矩形
                        VTextPointer topPointer = startPointer.Row < endPointer.Row ? startPointer : endPointer;
                        VTextPointer bottomPointer = startPointer.Row < endPointer.Row ? endPointer : startPointer;

                        //logger.FatalFormat("top = {0}, bottom = {1}", topPointer.Row, bottomPointer.Row);

                        // 相对于Panel的起始选中边界框和结束选中的边界框
                        VTRect topBounds = topPointer.CharacterBounds;
                        VTRect bottomBounds = bottomPointer.CharacterBounds;

                        // 当光标在Panel外的时候（也就是一边在滚动，一边在选中），需要做特殊处理
                        if (scrollResult == OutsideScrollResult.ScrollTop)
                        {
                            // 鼠标往上滚动

                            // 鼠标已经移动到窗口上面了，要修改topBounds位置为第一行
                            topBounds.Y = 0;

                            //logger.FatalFormat("bottom:{0}, top:{1}", bottomPointer.Row, topPointer.Row);

                            if (!this.UpdateTextPointerBounds(bottomPointer))
                            {
                                // Pointer更新失败，说明Pointer所指向的行已经滚动到文档外了，那么把bottom的字符位置改成右下角
                                bottomBounds.X = this.canvasRect.Width;
                                bottomBounds.Y = this.canvasRect.Height;
                                //logger.FatalFormat("更新失败");
                            }
                            else
                            {
                                // 更新成功，刷新bottomBounds
                                bottomBounds = bottomPointer.CharacterBounds;
                            }
                        }
                        else if (scrollResult == OutsideScrollResult.ScrollDown)
                        {
                            // 鼠标往下滚动

                            // 鼠标已经移动到窗口下面了，要修改bottomBounds位置为最后一行
                            bottomBounds.Y = this.canvasRect.Height;

                            if (!this.UpdateTextPointerBounds(topPointer))
                            {
                                topBounds.X = 0;
                                topBounds.Y = 0;
                            }
                            else
                            {
                                topBounds = topPointer.CharacterBounds;
                            }
                        }

                        this.textSelection.Ranges.Add(new VTRect(topBounds.X, topBounds.Y, 9999, topBounds.Height));
                        this.textSelection.Ranges.Add(new VTRect(0, bottomBounds.Y, bottomBounds.X + bottomBounds.Width, bottomBounds.Height));

                        // 构建中间的几何图形
                        VTRect middleBounds = new VTRect(0, topBounds.Y + topBounds.Height, 9999, bottomBounds.Y - topBounds.Bottom);
                        this.textSelection.Ranges.Add(middleBounds);
                        break;
                    }
            }

            this.uiSyncContext.Send((state) =>
            {
                this.Surface.Draw(this.textSelection);
            }, null);
        }

        private void CanvasPanel_VTMouseDown(ITerminalScreen canvasPanel, VTPoint mousePosition)
        {
            this.isMouseDown = true;
            this.mouseDownPos = mousePosition;
            this.canvasRect = this.Surface.GetRectRelativeToDesktop();

            // 得到startPos对应的VTextLine
            if (this.GetTextPointer(mousePosition, this.canvasRect, this.textSelection.Start))
            {
                logger.DebugFormat("命中:{0}", this.textSelection.Start.Row);
            }
        }

        #endregion
    }
}

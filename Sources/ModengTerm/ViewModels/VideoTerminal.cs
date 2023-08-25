using ModengTerm.VideoTerminal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using XTerminal;
using XTerminal.Base;
using XTerminal.Base.DataModels;
using XTerminal.Base.Definitions;
using XTerminal.Base.Enumerations;
using XTerminal.Document;
using XTerminal.Document.Rendering;
using XTerminal.Enumerations;
using XTerminal.Parser;
using XTerminal.Session;
using XTerminal.ViewModels;

namespace ModengTerm.ViewModels
{
    /// <summary>
    /// 处理虚拟终端的所有逻辑
    /// </summary>
    public partial class VideoTerminal : OpenedSessionVM
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
        private static readonly byte[] CPR_CursorPositionReportResponse = new byte[7] { 0x1b, (byte)'[', (byte)'?', (byte)'0', (byte)';', (byte)'0', (byte)'R' };
        //private static readonly byte[] CPR_CursorPositionReportResponse = new byte[5] { (byte)'\x1b', (byte)'[', (byte)'0', (byte)';', (byte)'0' };
        private static readonly byte[] DA_DeviceAttributesResponse = new byte[7] { 0x1b, (byte)'[', (byte)'?', (byte)'1', (byte)':', (byte)'0', (byte)'c' };

        #endregion

        #region 公开事件

        public event Action<VideoTerminal, SessionStatusEnum> SessionStatusChanged;

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

        private XTermSession sessionInfo;

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
        /// 记录滚动条滚动到底的时候，滚动条的值
        /// </summary>
        private int scrollMax;

        /// <summary>
        /// 记录当前滚动条滚动的值
        /// 也就是当前Surface上渲染的第一行的PhysicsRow
        /// 默认值是0
        /// </summary>
        private int scrollValue;

        #endregion

        /// <summary>
        /// DECAWM是否启用
        /// </summary>
        private bool autoWrapMode;

        private bool xtermBracketedPasteMode;

        #region History & Scroll

        /// <summary>
        /// 存储所有的历史行
        /// Row(scrollValue) -> VTextLine
        /// 注意该字典里保存的是mainDocument的历史行，alternateDocument没有保存，需要单独考虑alternateDocument
        /// </summary>
        private Dictionary<int, VTHistoryLine> historyLines;

        /// <summary>
        /// 历史行的第一行
        /// </summary>
        private VTHistoryLine firstHistoryLine;

        /// <summary>
        /// 历史行的最后一行
        /// </summary>
        private VTHistoryLine lastHistoryLine;

        #endregion

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

        private IDrawingCanvas selectionCanvas;
        /// <summary>
        /// 存储选中的文本信息
        /// </summary>
        private VTextSelection textSelection;

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

        #endregion

        #region 属性

        /// <summary>
        /// activeDocument的光标信息
        /// 该坐标是基于ViewableDocument的坐标
        /// Cursor的位置是下一个要打印的字符的位置
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
        /// 下一个字符要显示的位置
        /// </summary>
        public int CursorCol { get { return this.Cursor.Column; } }

        /// <summary>
        /// 当前终端显示的画面
        /// </summary>
        public IDrawingCanvas ActiveCanvas { get { return this.activeDocument.Canvas; } }

        /// <summary>
        /// 当前终端显示的文档
        /// </summary>
        public VTDocument ActiveDocument { get { return this.activeDocument; } }

        /// <summary>
        /// 获取当前滚动条是否滚动到底了
        /// </summary>
        public bool ScrollAtBottom
        {
            get
            {
                return this.scrollValue == this.scrollMax;
            }
        }

        /// <summary>
        /// 获取当前滚动条是否滚动到顶了
        /// </summary>
        public bool ScrollAtTop
        {
            get
            {
                return this.scrollValue == 0;
            }
        }

        public SessionTransport SessionTransport { get { return this.sessionTransport; } }

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

        public VideoTerminal()
        {
        }

        #endregion

        #region 公开接口

        /// <summary>
        /// 初始化终端模拟器
        /// </summary>
        /// <param name="sessionInfo"></param>
        public override int Open(XTermSession sessionInfo)
        {
            this.sessionInfo = sessionInfo;
            this.uiSyncContext = SynchronizationContext.Current;
            this.videoTerminal = this.Content as IVideoTerminal;

            // DECAWM
            this.autoWrapMode = false;

            // 初始化变量
            this.historyLines = new Dictionary<int, VTHistoryLine>();

            this.isRunning = true;

            this.outputEncoding = Encoding.GetEncoding(sessionInfo.GetOption<string>(OptionKeyEnum.WRITE_ENCODING));
            this.scrollDelta = sessionInfo.GetOption<int>(OptionKeyEnum.MOUSE_SCROLL_DELTA);
            this.rowSize = sessionInfo.GetOption<int>(OptionKeyEnum.SSH_TERM_ROW);
            this.colSize = sessionInfo.GetOption<int>(OptionKeyEnum.SSH_TERM_COL);

            // 初始化绘图
            this.InitializeDrawing();

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

            this.selectionCanvas = this.videoTerminal.CreateCanvas();
            this.videoTerminal.AddCanvas(this.selectionCanvas);
            this.textSelection = new VTextSelection();
            this.textSelection.DrawingContext = this.selectionCanvas.CreateDrawingObject(this.textSelection);

            #endregion

            #region 初始化文档模型

            VTDocumentOptions documentOptions = new VTDocumentOptions()
            {
                ColumnSize = this.colSize,
                RowSize = this.rowSize,
                DECPrivateAutoWrapMode = false,
                CursorStyle = sessionInfo.GetOption<VTCursorStyles>(OptionKeyEnum.SSH_THEME_CURSOR_STYLE),
                BlinkSpeed = sessionInfo.GetOption<VTCursorSpeeds>(OptionKeyEnum.SSH_THEME_CURSOR_SPEED),
                FontFamily = sessionInfo.GetOption<string>(OptionKeyEnum.SSH_THEME_FONT_FAMILY),
                FontSize = sessionInfo.GetOption<int>(OptionKeyEnum.SSH_THEME_FONT_SIZE),
                Foreground = sessionInfo.GetOption<string>(OptionKeyEnum.SSH_THEME_FONT_COLOR)
            };
            this.mainDocument = new VTDocument(documentOptions, this.videoTerminal.CreateCanvas()) { Name = "MainDocument" };
            this.alternateDocument = new VTDocument(documentOptions, this.videoTerminal.CreateCanvas()) { Name = "AlternateDocument" };
            this.activeDocument = this.mainDocument;
            this.firstHistoryLine = VTHistoryLine.Create(0, null, this.ActiveLine);
            this.historyLines[0] = this.firstHistoryLine;
            this.lastHistoryLine = this.firstHistoryLine;
            this.videoTerminal.AddCanvas(this.mainDocument.Canvas);

            #endregion

            #region 初始化光标

            this.activeDocument.Cursor.RequestInvalidate();
            // 先初始化备用缓冲区的光标渲染上下文
            this.alternateDocument.Cursor.RequestInvalidate();

            #endregion

            #region 连接终端通道

            SessionTransport transport = new SessionTransport();
            transport.StatusChanged += this.VTSession_StatusChanged;
            transport.DataReceived += this.VTSession_DataReceived;
            transport.Initialize(sessionInfo);
            transport.Open();
            this.sessionTransport = transport;

            #endregion

            return ResponseCode.SUCCESS;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public override void Close()
        {
            this.isRunning = false;

            this.vtParser.ActionEvent -= VtParser_ActionEvent;
            this.vtParser.Release();

            this.sessionTransport.StatusChanged -= this.VTSession_StatusChanged;
            this.sessionTransport.DataReceived -= this.VTSession_DataReceived;
            this.sessionTransport.Close();
            this.sessionTransport.Release();

            this.mainDocument.Dispose();
            this.alternateDocument.Dispose();

            this.historyLines.Clear();
            this.firstHistoryLine = null;
            this.lastHistoryLine = null;
        }

        /// <summary>
        /// 复制当前选中的行
        /// </summary>
        public void CopySelection()
        {
            VTHistoryLine startLine, endLine;
            int startIndex, endIndex;
            this.AdjustSelection(out startLine, out endLine, out startIndex, out endIndex);

            string text = XTermUtils.BuildDocument(startLine, endLine, startIndex, endIndex, SaveFormatEnum.TextFormat);

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
            this.textSelection.SetRange(this.activeDocument, this.vtRect, 0, 0, this.lastHistoryLine.PhysicsRow, this.lastHistoryLine.Characters.Count - 1);
            this.textSelection.RequestInvalidate();
        }

        /// <summary>
        /// 把终端的内容保存到文件
        /// </summary>
        /// <param name="saveMode"></param>
        /// <param name="format"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public bool SaveToFile(SaveModeEnum saveMode, SaveFormatEnum format, string filePath)
        {
            VTHistoryLine startLine = null, endLine = null;
            int startIndex = 0, endIndex = 0;

            switch (saveMode)
            {
                case SaveModeEnum.SaveAll:
                    {
                        startLine = this.firstHistoryLine;
                        endLine = this.lastHistoryLine;
                        startIndex = 0;
                        endIndex = endLine.Characters.Count - 1;
                        break;
                    }

                case SaveModeEnum.SaveScreen:
                    {
                        this.historyLines.TryGetValue(this.activeDocument.FirstLine.PhysicsRow, out startLine);
                        this.historyLines.TryGetValue(this.activeDocument.LastLine.PhysicsRow, out endLine);
                        startIndex = 0;
                        if (endLine != null)
                        {
                            endIndex = endLine.Characters.Count - 1;
                        }
                        break;
                    }

                case SaveModeEnum.SaveSelected:
                    {
                        this.AdjustSelection(out startLine, out endLine, out startIndex, out endIndex);
                        break;
                    }

                default:
                    throw new NotImplementedException();
            }

            if (startLine == null || endLine == null)
            {
                logger.ErrorFormat("保存文件失败, satrtLine或endLine为空");
                return false;
            }

            string content = XTermUtils.BuildDocument(startLine, endLine, startIndex, endIndex, format);
            try
            {
                File.WriteAllText(filePath, content);
            }
            catch (Exception ex)
            {
                logger.Error("写入文件异常", ex);
                return false;
            }

            return true;
        }

        /// <summary>
        /// 把终端内容滚动到某个位置
        /// </summary>
        /// <param name="scrollValue"></param>
        public void ScrollTo(int scrollValue)
        {
            if (this.scrollValue == scrollValue)
            {
                return;
            }

            this.ScrollToHistory(scrollValue);
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

            // 这里输入的都是键盘按键
            int code = this.sessionTransport.Write(bytes);
            if (code != ResponseCode.SUCCESS)
            {
                logger.ErrorFormat("处理输入异常, {0}", ResponseCode.GetMessage(code));
            }
        }

        #endregion

        #region 实例方法

        private void AdjustSelection(out VTHistoryLine topLine, out VTHistoryLine bottomLine, out int startIndex, out int endIndex)
        {
            topLine = null;
            bottomLine = null;
            startIndex = -1;
            endIndex = -1;

            if (this.textSelection.IsEmpty)
            {
                logger.WarnFormat("CopySelection失败, 选中内容为空");
                return;
            }

            VTextPointer startPointer = this.textSelection.Start;
            VTextPointer endPointer = this.textSelection.End;

            // 找到起始行和结束行
            if (!this.historyLines.TryGetValue(startPointer.PhysicsRow, out topLine) ||
                !this.historyLines.TryGetValue(endPointer.PhysicsRow, out bottomLine))
            {
                logger.WarnFormat("CopySelection失败, 未找到选中的起始历史行或结束历史行");
                return;
            }

            startIndex = startPointer.CharacterIndex;
            endIndex = endPointer.CharacterIndex;

            // 要考虑鼠标从下往上选中的情况
            // 如果鼠标从下往上选中，那么此时下面的VTextPointer是起始，上面的VTextPointer是结束
            if (topLine.PhysicsRow > bottomLine.PhysicsRow)
            {
                VTHistoryLine tempLine = topLine;
                topLine = bottomLine;
                bottomLine = tempLine;

                startIndex = endPointer.CharacterIndex;
                endIndex = startPointer.CharacterIndex;
            }
            else if (topLine.PhysicsRow == bottomLine.PhysicsRow)
            {
                // 注意要处理鼠标从右向左选中的情况
                // 如果鼠标是从右向左进行选中，那么Start就是Selection的右边，End就是Selection的左边
                startIndex = Math.Min(startPointer.CharacterIndex, endPointer.CharacterIndex);
                endIndex = Math.Max(startPointer.CharacterIndex, endPointer.CharacterIndex);
            }
        }

        private void PerformDeviceStatusReport(StatusType statusType)
        {
            switch (statusType)
            {
                case StatusType.OS_OperatingStatus:
                    {
                        // Result ("OK") is CSI 0 n
                        this.sessionTransport.Write(OS_OperationStatusResponse);
                        break;
                    }

                case StatusType.CPR_CursorPositionReport:
                    {
                        // 打开VIM后会收到这个请求

                        // Result is CSI ? r ; c R
                        int cursorRow = this.CursorRow;
                        int cursorCol = this.CursorCol;
                        CPR_CursorPositionReportResponse[3] = (byte)cursorRow;
                        CPR_CursorPositionReportResponse[5] = (byte)cursorCol;
                        this.sessionTransport.Write(CPR_CursorPositionReportResponse);
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

                // 如果显示的是主缓冲区，那么光标在最后一行的时候才更新
                // 如果显示的是备用缓冲区，光标可以在任意一个位置显示，那么直接渲染光标
                if (this.activeDocument == this.alternateDocument || this.ScrollAtBottom)
                {
                    this.Cursor.OffsetY = this.ActiveLine.OffsetY;
                    // 有可能有中文字符，一个中文字符占用2列
                    // 先通过光标所在列找到真正的字符所在列
                    int characterIndex = this.ActiveLine.FindCharacterIndex(this.CursorCol - 1);
                    VTRect rect = this.ActiveLine.MeasureLine(characterIndex, 1);
                    this.Cursor.OffsetX = rect.Right;
                }
                else
                {
                    // 此时说明有滚动，有滚动的情况下直接隐藏光标
                    this.Cursor.OffsetX = int.MinValue;
                    this.Cursor.OffsetX = int.MinValue;
                }

                this.Cursor.RequestInvalidate();

                #endregion

                #region 移动滚动条

                int scrollMax, scrollValue;
                this.videoTerminal.GetScrollInfo(out scrollMax, out scrollValue);
                if (this.scrollMax != scrollMax || this.scrollValue != scrollValue)
                {
                    this.videoTerminal.SetScrollInfo(this.scrollMax, this.scrollValue);
                }

                #endregion

                #region 更新选中高亮几何图形

                if (!this.textSelection.IsEmpty)
                {
                    this.textSelection.UpdateRange(this.activeDocument, this.vtRect);
                    this.textSelection.RequestInvalidate();
                }

                #endregion

            }, null);

            document.SetArrangeDirty(false);
        }

        /// <summary>
        /// 滚动到指定的历史记录
        /// 并更新UI上的滚动条位置
        /// </summary>
        /// <param name="scrollValue">要显示的第一行历史记录</param>
        private void ScrollToHistory(int scrollValue)
        {
            // 只有主缓冲区可以滚动
            if (this.activeDocument != this.mainDocument)
            {
                return;
            }

            // 要滚动到的值
            int newScroll = scrollValue;
            // 滚动之前的值
            int oldScroll = this.scrollValue;

            // 更新当前滚动条的值，一定要先更新，因为DrawDocument函数会用到该值
            this.scrollValue = scrollValue;

            // 需要进行滚动的行数
            int scrolledRows = Math.Abs(newScroll - oldScroll);

            // 终端行大小
            int rows = this.rowSize;

            #region 更新要显示的行

            if (scrolledRows >= rows)
            {
                // 先找到Surface上要显示的第一行数据
                VTHistoryLine historyLine;
                if (!this.historyLines.TryGetValue(scrollValue, out historyLine))
                {
                    logger.ErrorFormat("ScrollTo失败, 找不到对应的VTHistoryLine, scrollValue = {0}", scrollValue);
                    return;
                }

                // 此时说明把所有行都滚动到Surface外了，需要重新显示所有行
                // 找到后面的行数显示
                VTHistoryLine currentHistory = historyLine;
                VTextLine currentTextLine = this.activeDocument.FirstLine;
                for (int i = 0; i < rows; i++)
                {
                    // 直接使用VTHistoryLine的List<VTCharacter>的引用
                    // 冻结状态下的VTextLine不会再有修改了
                    // 非冻结状态(ActiveLine)需要重新创建一个集合
                    currentTextLine.SetHistory(currentHistory);
                    currentHistory = currentHistory.NextLine;
                    currentTextLine = currentTextLine.NextLine;
                }
            }
            else
            {
                // 此时说明只需要更新移动出去的行就可以了
                if (newScroll > oldScroll)
                {
                    // 往下滚动，把上面的拿到下面，从第一行开始
                    for (int i = 0; i < scrolledRows; i++)
                    {
                        // 该值永远是第一行，因为下面被Move到最后一行了
                        VTextLine firstLine = this.activeDocument.FirstLine;

                        VTHistoryLine historyLine;
                        if (this.historyLines.TryGetValue(oldScroll + rows + i, out historyLine))
                        {
                            firstLine.SetHistory(historyLine);
                        }
                        else
                        {
                            // 当扩大终端行数之后，然后把滚动条拖上去，然后输入字符的时候，会出现没有历史行的情况
                            // 因为扩大行数的时候会新增加行，但是这些行并是空行，并没有对应的历史记录
                            // 所以这里就直接清空行
                            firstLine.SetEmpty();
                        }

                        this.activeDocument.MoveLine(firstLine, VTextLine.MoveOptions.MoveToLast);
                    }
                }
                else
                {
                    // 往上滚动，把下面的拿到上面，从最后一行开始
                    for (int i = 1; i <= scrolledRows; i++)
                    {
                        VTHistoryLine historyLine = this.historyLines[oldScroll - i];

                        VTextLine lastLine = this.activeDocument.LastLine;
                        lastLine.SetHistory(historyLine);
                        this.activeDocument.MoveLine(lastLine, VTextLine.MoveOptions.MoveToFirst);
                    }
                }
            }

            this.activeDocument.SetArrangeDirty(true);

            #endregion

            #region 如果有TextSelection，那么更新TextSelection

            if (!this.textSelection.IsEmpty)
            {
                // 把文本选中标记为脏数据，在下次渲染的时候会重新渲染文本选中
                this.textSelection.RequestInvalidate();
            }

            #endregion

            #region 重新渲染

            this.PerformDrawing(this.activeDocument);

            #endregion
        }

        /// <summary>
        /// 当光标在容器外面移动的时候，进行滚动
        /// </summary>
        /// <param name="mousePosition">当前鼠标的坐标</param>
        /// <param name="surfaceBoundary">相对于电脑显示器的画布的边界框</param>
        /// <returns>是否执行了滚动动作</returns>
        private OutsideScrollResult ScrollIfCursorOutsideSurface(VTPoint mousePosition, VTRect surfaceBoundary)
        {
            OutsideScrollResult scrollResult = OutsideScrollResult.None;

            // 要滚动到的目标行
            int scrollTarget = -1;

            if (mousePosition.Y < 0)
            {
                // 光标在容器上面
                if (!this.ScrollAtTop)
                {
                    // 不在最上面，往上滚动一行
                    scrollTarget = this.scrollValue - 1;
                    scrollResult = OutsideScrollResult.ScrollTop;
                }
            }
            else if (mousePosition.Y > surfaceBoundary.Height)
            {
                // 光标在容器下面
                if (!this.ScrollAtBottom)
                {
                    scrollTarget = this.scrollValue + 1;
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
        /// <param name="surfaceBoundary">相对于电脑显示器的画布的边界框，也是鼠标的限定范围</param>
        /// <param name="pointer">存储命中测试结果的变量</param>
        /// <remarks>如果传递进来的鼠标位置在窗口外，那么会把鼠标限定在距离鼠标最近的Surface边缘处</remarks>
        /// <returns>
        /// 是否获取成功
        /// 当光标不在某一行或者不在某个字符上的时候，就获取失败
        /// </returns>
        private bool GetTextPointer(VTPoint mousePosition, VTRect surfaceBoundary, VTextPointer pointer)
        {
            double mouseX = mousePosition.X;
            double mouseY = mousePosition.Y;

            if (mouseX < 0)
            {
                mouseX = 0;
            }
            if (mouseX > surfaceBoundary.Width)
            {
                mouseX = surfaceBoundary.Width;
            }

            if (mouseY < 0)
            {
                mouseY = 0;
            }
            if (mouseY > surfaceBoundary.Height)
            {
                mouseY = surfaceBoundary.Height;
            }

            #region 先找到鼠标所在行

            VTextLine cursorLine = HitTestHelper.HitTestVTextLine(this.activeDocument.FirstLine, mouseY);
            if (cursorLine == null)
            {
                // 这里说明鼠标没有在任何一行上
                logger.DebugFormat("没有找到鼠标位置对应的行, cursorY = {0}", mouseY);
                return false;
            }

            #endregion

            #region 再计算鼠标悬浮于哪个字符上

            int characterIndex;
            VTRect characterBounds;
            if (!HitTestHelper.HitTestVTCharacter(cursorLine, mouseX, out characterIndex, out characterBounds))
            {
                return false;
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

                        if (this.activeDocument == this.mainDocument)
                        {
                            // 用户输入的时候，如果滚动条没滚动到底，那么先把滚动条滚动到底
                            // 不然会出现在VTDocument当前的最后一行打印字符的问题
                            if (!this.ScrollAtBottom)
                            {
                                this.ScrollToHistory(this.scrollMax);
                            }
                        }

                        char ch = Convert.ToChar(parameter);
                        VTDebug.WriteAction("Print:{0}({1}), Cursor={2},{3}", ch, Convert.ToInt32(parameter), this.CursorRow, this.CursorCol);
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
                        VTDebug.WriteAction("CarriageReturn, Cursor={0},{1}", this.CursorRow, this.CursorCol);
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

                        // 换行之后记录历史行
                        // 注意用户可以输入Backspace键或者上下左右光标键来修改最新行的内容，所以最新一行的内容是实时变化的，目前的解决方案是在渲染整个文档的时候去更新最后一个历史行的数据
                        // MainScrrenBuffer和AlternateScrrenBuffer里的行分别记录
                        // AlternateScreenBuffer是用来给man，vim等程序使用的
                        // 暂时只记录主缓冲区里的数据，备用缓冲区需要考虑下怎么记录，因为VIM，Man等程序用的是备用缓冲区，用户是可以实时编辑缓冲区里的数据的
                        if (this.activeDocument == this.mainDocument)
                        {
                            VTextLine oldLastLine = this.ActiveLine.PreviousLine;
                            VTextLine newLastLine = this.ActiveLine;
                            newLastLine.PhysicsRow = oldLastLine.PhysicsRow + 1;

                            // 可以确保换行之前的行已经被用户输入完了，不会被更改了，所以这里冻结一下换行之前的历史行的数据，冻结之后，该历史行的数据就不会再更改了
                            // 有几种特殊情况：
                            // 1. 如果主机一次性返回了多行数据，那么有可能前面的几行都没有测量，所以这里要先判断上一行是否有测量过

                            #region 更新历史行

                            this.lastHistoryLine.SetVTextLine(oldLastLine);

                            #endregion

                            #region 创建新行对应的历史行

                            // 再创建最新行的历史行
                            int newHistoryRow = newLastLine.PhysicsRow;
                            VTHistoryLine newHistory = VTHistoryLine.Create(newLastLine.PhysicsRow, this.lastHistoryLine, this.ActiveLine);
                            this.historyLines[newHistoryRow] = newHistory;
                            this.lastHistoryLine = newHistory;

                            #endregion

                            #region 更新滚动条的值

                            // 滚动条滚动到底
                            // 计算滚动条可以滚动的最大值
                            int scrollMax = newHistoryRow - this.rowSize + 1;
                            if (scrollMax > 0)
                            {
                                // 更新滚动条的值
                                this.scrollMax = scrollMax;
                                this.scrollValue = scrollMax;
                            }

                            #endregion
                        }

                        VTDebug.WriteAction("LF/FF/VT, Cursor={0},{1}, {2}", this.CursorRow, this.CursorCol, action);
                        break;
                    }

                case VTActions.RI_ReverseLineFeed:
                    {
                        // 和LineFeed相反，也就是把光标往上移一个位置
                        // 在用man命令的时候会触发这个指令
                        // 反向换行 – 执行\n的反向操作，将光标向上移动一行，维护水平位置，如有必要，滚动缓冲区 *
                        this.activeDocument.ReverseLineFeed();
                        VTDebug.WriteAction("ReverseLineFeed");
                        break;
                    }

                case VTActions.PlayBell:
                    {
                        // 响铃
                        break;
                    }

                #region Erase

                case VTActions.EL_EraseLine:
                    {
                        List<int> parameters = parameter as List<int>;
                        EraseType eraseType = (EraseType)VTParameter.GetParameter(parameters, 0, 0);
                        VTDebug.WriteAction("EL_EraseLine, eraseType = {0}, 从光标位置{1},{2}开始erase", eraseType, this.CursorRow, this.CursorCol);
                        this.activeDocument.EraseLine(this.ActiveLine, this.CursorCol, eraseType);
                        break;
                    }

                case VTActions.ED_EraseDisplay:
                    {
                        List<int> parameters = parameter as List<int>;
                        EraseType eraseType = (EraseType)VTParameter.GetParameter(parameters, 0, 0);
                        VTDebug.WriteAction("ED_EraseDisplay, eraseType = {0}, Cursor={1},{2}", eraseType, this.CursorRow, this.CursorCol);
                        this.activeDocument.EraseDisplay(this.ActiveLine, this.CursorCol, eraseType);
                        break;
                    }

                #endregion

                #region 光标移动

                // 下面的光标移动指令不能进行VTDocument的滚动
                // 光标的移动坐标是相对于可视区域内的坐标
                // 服务器发送过来的光标原点是从(1,1)开始的，我们程序里的是(0,0)开始的，所以要减1

                case VTActions.BS:
                    {
                        this.activeDocument.SetCursor(this.CursorRow, this.CursorCol - 1);
                        VTDebug.WriteAction("CursorBackward, Cursor = {0}, {1}", this.CursorRow, this.CursorCol);
                        break;
                    }

                case VTActions.CursorBackward:
                    {
                        List<int> parameters = parameter as List<int>;
                        int n = VTParameter.GetParameter(parameters, 0, 1);
                        this.activeDocument.SetCursor(this.CursorRow, this.CursorCol - n);
                        VTDebug.WriteAction("CursorBackward, Cursor = {0}, {1}, n = {2}", this.CursorRow, this.CursorCol, n);
                        break;
                    }

                case VTActions.CUF_CursorForward:
                    {
                        List<int> parameters = parameter as List<int>;
                        int n = VTParameter.GetParameter(parameters, 0, 1);
                        this.activeDocument.SetCursor(this.CursorRow, this.CursorCol + n);
                        VTDebug.WriteAction("CUF_CursorForward, Cursor = {0}, {1}, n = {2}", this.CursorRow, this.CursorCol, n);
                        break;
                    }

                case VTActions.CUU_CursorUp:
                    {
                        List<int> parameters = parameter as List<int>;
                        int n = VTParameter.GetParameter(parameters, 0, 1);
                        this.activeDocument.SetCursor(this.CursorRow - n, this.CursorCol);
                        VTDebug.WriteAction("CUU_CursorUp, Cursor = {0}, {1}, n = {2}", this.CursorRow, this.CursorCol, n);
                        break;
                    }

                case VTActions.CUD_CursorDown:
                    {
                        List<int> parameters = parameter as List<int>;
                        int n = VTParameter.GetParameter(parameters, 0, 1);
                        this.activeDocument.SetCursor(this.CursorRow + n, this.CursorCol);
                        VTDebug.WriteAction("CUD_CursorDown, Cursor = {0}, {1}, n = {2}", this.CursorRow, this.CursorCol, n);
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

                            // 刚打开VIM就按空格键，此时VIM会响应一个CursorPosition向右移动一个单位的事件
                            // 此时要把光标向右移动一个单位
                            this.ActiveLine.PadColumns(parameters[1]);
                        }
                        else
                        {
                            // 如果没有参数，那么说明就是定位到原点(0,0)
                        }

                        VTDebug.WriteAction("CUP_CursorPosition, row = {0}, col = {1}", row, col);
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
                            VTDebug.WriteAction("CHA_CursorHorizontalAbsolute失败");
                            return;
                        }

                        this.ActiveLine.PadColumns(n);
                        this.activeDocument.SetCursor(this.CursorRow, n - 1);
                        VTDebug.WriteAction("CHA_CursorHorizontalAbsolute, targetColumn = {0}", n);
                        break;
                    }

                #endregion

                #region 文本特效

                case VTActions.Bold:
                case VTActions.BoldUnset:
                case VTActions.Underline:
                case VTActions.UnderlineUnset:
                case VTActions.Italics:
                case VTActions.ItalicsUnset:
                case VTActions.DoublyUnderlined:
                case VTActions.DoublyUnderlinedUnset:
                case VTActions.Foreground:
                case VTActions.DefaultForeground:
                case VTActions.Background:
                case VTActions.DefaultBackground:
                    {
                        // Foreground和DefaultForeground的CharacterIndex可能都是0，并且颜色也是空的
                        // 注意在渲染的时候要处理这种情况

                        bool unset;
                        VTextDecorations decoration = VDocumentUtils.VTAction2TextDecoration(action, out unset);
                        VTextAttribute textAttribute = this.ActiveLine.Attributes.FirstOrDefault(v => v.Decoration == decoration && !v.Unset);
                        if (textAttribute == null)
                        {
                            textAttribute = new VTextAttribute()
                            {
                                StartColumn = this.CursorCol,
                                Decoration = decoration,
                                Parameter = parameter
                            };
                            this.ActiveLine.Attributes.Add(textAttribute);
                        }

                        if (unset)
                        {
                            // 因为光标所在列是下一个要打印的字符的列，所以这里要减1
                            // 有可能在第0列就设置了Unset，这种情况要处理一下
                            textAttribute.EndColumn = this.CursorCol == 0 ? 0 : this.CursorCol - 1;
                            textAttribute.Unset = true;

                            VTDebug.WriteAction("{0}, start = {1}, end = {2}, parameter = {3}", action, textAttribute.StartColumn, textAttribute.EndColumn, textAttribute.Parameter);
                        }
                        break;
                    }

                case VTActions.DefaultAttributes:
                case VTActions.Faint:
                case VTActions.CrossedOutUnset:
                case VTActions.ReverseVideo:
                case VTActions.ReverseVideoUnset:
                    break;

                #endregion

                #region DECPrivateMode

                case VTActions.DECANM_AnsiMode:
                    {
                        bool enable = Convert.ToBoolean(parameter);
                        VTDebug.WriteAction("DECANM_AnsiMode, enable = {0}", enable);
                        this.keyboard.SetAnsiMode(enable);
                        break;
                    }

                case VTActions.DECCKM_CursorKeysMode:
                    {
                        bool enable = Convert.ToBoolean(parameter);
                        VTDebug.WriteAction("DECCKM_CursorKeysMode, enable = {0}", enable);
                        this.keyboard.SetCursorKeyMode(enable);
                        break;
                    }

                case VTActions.DECKPAM_KeypadApplicationMode:
                    {
                        VTDebug.WriteAction("DECKPAM_KeypadApplicationMode");
                        this.keyboard.SetKeypadMode(true);
                        break;
                    }

                case VTActions.DECKPNM_KeypadNumericMode:
                    {
                        VTDebug.WriteAction("DECKPNM_KeypadNumericMode");
                        this.keyboard.SetKeypadMode(false);
                        break;
                    }

                case VTActions.DECAWM_AutoWrapMode:
                    {
                        bool enable = Convert.ToBoolean(parameter);
                        this.autoWrapMode = enable;
                        VTDebug.WriteAction("DECAWM_AutoWrapMode, enable = {0}", enable);
                        this.activeDocument.DECPrivateAutoWrapMode = enable;
                        break;
                    }

                case VTActions.XTERM_BracketedPasteMode:
                    {
                        this.xtermBracketedPasteMode = Convert.ToBoolean(parameter);
                        VTDebug.WriteAction("未实现XTERM_BracketedPasteMode");
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
                        VTDebug.WriteAction("DCH_DeleteCharacter, {0}, cursorPos = {1}", count, this.CursorCol);
                        this.activeDocument.DeleteCharacter(this.ActiveLine, this.CursorCol, count);
                        break;
                    }

                case VTActions.ICH_InsertCharacter:
                    {
                        // 在当前光标处插入N个空白字符,这会将所有现有文本移到右侧。 向右溢出屏幕的文本会被删除
                        // 目前没发现这个操作对终端显示有什么影响，所以暂时不实现
                        List<int> parameters = parameter as List<int>;
                        int count = VTParameter.GetParameter(parameters, 0, 1);
                        VTDebug.WriteAction("未实现InsertCharacters, {0}, cursorPos = {1}", count, this.CursorCol);
                        break;
                    }

                #endregion

                #region 上下滚动

                #endregion

                case VTActions.UseAlternateScreenBuffer:
                    {
                        VTDebug.WriteAction("UseAlternateScreenBuffer");

                        IDrawingCanvas remove = this.mainDocument.Canvas;
                        IDrawingCanvas add = this.alternateDocument.Canvas;
                        this.videoTerminal.RemoveCanvas(remove);
                        this.videoTerminal.AddCanvas(add);

                        // 这里只重置行数，在用户调整窗口大小的时候需要执行终端的Resize操作
                        this.alternateDocument.SetScrollMargin(0, 0);
                        this.alternateDocument.DeleteAll();
                        this.activeDocument = this.alternateDocument;
                        break;
                    }

                case VTActions.UseMainScreenBuffer:
                    {
                        VTDebug.WriteAction("UseMainScreenBuffer");

                        IDrawingCanvas remove = this.alternateDocument.Canvas;
                        IDrawingCanvas add = this.mainDocument.Canvas;
                        this.videoTerminal.RemoveCanvas(remove);
                        this.videoTerminal.AddCanvas(add);

                        this.mainDocument.DirtyAll();
                        this.activeDocument = this.mainDocument;
                        break;
                    }

                case VTActions.DSR_DeviceStatusReport:
                    {
                        // DSR，参考https://invisible-island.net/xterm/ctlseqs/ctlseqs.html

                        List<int> parameters = parameter as List<int>;
                        StatusType statusType = (StatusType)Convert.ToInt32(parameters[0]);
                        VTDebug.WriteAction("DSR_DeviceStatusReport, statusType = {0}", statusType);
                        this.PerformDeviceStatusReport(statusType);
                        break;
                    }

                case VTActions.DA_DeviceAttributes:
                    {
                        VTDebug.WriteAction("DA_DeviceAttributes");
                        this.sessionTransport.Write(DA_DeviceAttributesResponse);
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
                            VTDebug.WriteAction("DECSTBM_SetScrollingRegion参数不正确，忽略本次设置, topMargin = {0}, bottomMargin = {1}", topMargin, bottomMargin);
                            return;
                        }
                        if (topMargin >= bottomMargin)
                        {
                            VTDebug.WriteAction("DECSTBM_SetScrollingRegion参数不正确，topMargin大于等bottomMargin，忽略本次设置, topMargin = {0}, bottomMargin = {1}", topMargin, bottomMargin);
                            return;
                        }
                        if (bottomMargin > lines)
                        {
                            VTDebug.WriteAction("DECSTBM_SetScrollingRegion参数不正确，bottomMargin大于当前屏幕总行数, bottomMargin = {0}, lines = {1}", bottomMargin, lines);
                            return;
                        }

                        // 如果topMargin等于1，那么就表示使用默认值，也就是没有marginTop，所以当topMargin == 1的时候，marginTop改为0
                        int marginTop = topMargin == 1 ? 0 : topMargin;
                        // 如果bottomMargin等于控制台高度，那么就表示使用默认值，也就是没有marginBottom，所以当bottomMargin == 控制台高度的时候，marginBottom改为0
                        int marginBottom = lines - bottomMargin;
                        VTDebug.WriteAction("SetScrollingRegion, topMargin = {0}, bottomMargin = {1}", marginTop, marginBottom);
                        this.activeDocument.SetScrollMargin(marginTop, marginBottom);
                        break;
                    }

                case VTActions.IL_InsertLine:
                    {
                        // 将 <n> 行插入光标位置的缓冲区。 光标所在的行及其下方的行将向下移动。
                        List<int> parameters = parameter as List<int>;
                        int lines = VTParameter.GetParameter(parameters, 0, 1);
                        VTDebug.WriteAction("IL_InsertLine, lines = {0}", lines);
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
                        VTDebug.WriteAction("DL_DeleteLine, lines = {0}", lines);
                        if (lines > 0)
                        {
                            this.activeDocument.DeleteLines(this.ActiveLine, lines);
                        }
                        break;
                    }

                default:
                    {
                        VTDebug.WriteAction("未执行的VTAction, {0}", action);
                        break;
                    }
            }
        }

        private void VTSession_DataReceived(SessionTransport client, byte[] bytes, int size)
        {
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
                VTextLine lastTextLine = this.mainDocument.FindLine(this.lastHistoryLine.PhysicsRow);
                if (lastTextLine != null)
                {
                    this.lastHistoryLine.SetVTextLine(lastTextLine);
                }
            }
        }

        private void VTSession_StatusChanged(object client, SessionStatusEnum status)
        {
            logger.InfoFormat("会话状态发生改变, {0}", status);
            if (this.SessionStatusChanged != null)
            {
                this.SessionStatusChanged(this, status);
            }
        }


        public void OnMouseDown(IVideoTerminal vt, VTPoint location, int clickCount)
        {
            if (clickCount == 1)
            {
                this.isMouseDown = true;
                this.mouseDownPos = location;
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
                            int characterIndex;
                            VTRect characterBounds;
                            if (!HitTestHelper.HitTestVTCharacter(lineHit, location.X, out characterIndex, out characterBounds))
                            {
                                return;
                            }
                            VDocumentUtils.GetSegement(lineHit.Text, characterIndex, out startIndex, out endIndex);
                            break;
                        }

                    case 3:
                        {
                            // 选中一整行
                            startIndex = 0;
                            endIndex = lineHit.Text.Length - 1;
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

            // 如果还没有测量起始字符，那么测量起始字符
            if (this.textSelection.Start.CharacterIndex == -1)
            {
                if (!this.GetTextPointer(location, this.vtRect, this.textSelection.Start))
                {
                    // 没有命中起始字符，那么直接返回啥都不做
                    //logger.DebugFormat("没命中起始字符");
                    return;
                }
            }

            // 首先检测鼠标是否在Surface边界框的外面
            // 如果在Surface的外面并且行数超出了Surface可以显示的最多行数，那么根据鼠标方向进行滚动，每次滚动一行
            OutsideScrollResult scrollResult = this.ScrollIfCursorOutsideSurface(location, this.vtRect);

            // 整理思路是算出来StartTextPointer和EndTextPointer之间的几何图形
            // 然后渲染几何图形，SelectionRange本质上就是一堆矩形
            VTextPointer startPointer = this.textSelection.Start;
            VTextPointer endPointer = this.textSelection.End;

            // 得到当前鼠标的命中信息
            if (!this.GetTextPointer(location, this.vtRect, endPointer))
            {
                // 只有在没有Outside滚动的时候，才返回
                // Outside滚动会导致GetTextPointer失败，虽然失败，还是要更新SelectionRange
                if (scrollResult == OutsideScrollResult.None)
                {
                    return;
                }
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

            #region 计算并重新渲染选中内容的几何图形，要考虑到滚动条滚动的情况

            // 此时的VTextLine测量数据都是最新的
            // 主缓冲区和备用缓冲区都支持选中
            this.textSelection.UpdateRange(this.activeDocument, this.vtRect);
            this.textSelection.RequestInvalidate();

            #endregion
        }

        public void OnMouseUp(IVideoTerminal vt, VTPoint location)
        {
            this.isMouseDown = false;
            this.selectionState = false;
        }

        public void OnMouseWheel(IVideoTerminal vt, bool upper)
        {
            // 只有主缓冲区才可以用鼠标滚轮进行滚动
            if (this.activeDocument != this.mainDocument)
            {
                return;
            }

            if (upper)
            {
                // 向上滚动

                // 先判断是不是已经滚动到顶了
                if (this.ScrollAtTop)
                {
                    // 滚动到顶直接返回
                    return;
                }

                if (this.scrollValue < this.scrollDelta)
                {
                    // 一次可以全部滚完并且还有剩余
                    this.ScrollToHistory(0);
                }
                else
                {
                    this.ScrollToHistory(this.scrollValue - this.scrollDelta);
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
                int remainScroll = this.scrollMax - this.scrollValue;

                if (remainScroll >= this.scrollDelta)
                {
                    this.ScrollToHistory(this.scrollValue + this.scrollDelta);
                }
                else
                {
                    // 直接滚动到底
                    this.ScrollToHistory(this.scrollMax);
                }
            }
        }

        public void OnSizeChanged(IVideoTerminal vt, VTRect vtRect)
        {
            this.vtRect = vtRect;

            // 如果是固定大小的终端，那么什么都不做
            TerminalSizeModeEnum sizeMode = sessionInfo.GetOption<TerminalSizeModeEnum>(OptionKeyEnum.SSH_TERM_SIZE_MODE);
            if (sizeMode == TerminalSizeModeEnum.Fixed)
            {
                return;
            }

            // 自适应屏幕大小
            // 计算一共有多少行，和每行之间的间距是多少
            int fontSize = sessionInfo.GetOption<int>(OptionKeyEnum.SSH_THEME_FONT_SIZE);
            string fontFamily = sessionInfo.GetOption<string>(OptionKeyEnum.SSH_THEME_FONT_FAMILY);
            VTextMetrics metrics = this.videoTerminal.MeasureText(" ", fontSize, fontFamily);

            // 终端控件的初始宽度和高度，在打开Session的时候动态设置
            int newRows = (int)Math.Floor(vtRect.Height / metrics.Height);
            int newCols = (int)Math.Floor(vtRect.Width / metrics.Width);

            // 如果行和列都没变化，那么就什么都不做
            bool rowChanged = this.rowSize != newRows;
            bool colChanged = this.colSize != newCols;
            if (!rowChanged && !colChanged)
            {
                return;
            }

            VTDebug.WriteAction("resize, oldRow = {0}, oldCol = {1}, newRow = {2}, newCol = {3}", this.rowSize, this.colSize, newRows, newCols);

            // 对Document执行Resize
            // 目前的实现在ubuntu下没问题，但是在Windows10操作系统上运行Windows命令行里的vim程序会有问题，可能是Windows下的vim程序兼容性导致的，暂时先这样
            // 遇到过一种情况：如果终端名称不正确，比如XTerm，那么当行数增加的时候，光标会移动到该行的最右边，终端名称改成xterm就没问题了
            // 目前的实现思路是：如果是减少行，那么从第一行开始删除；如果是增加行，那么从最后一行开始新建行。不考虑ScrollMargin
            this.mainDocument.Resize(newRows, newCols);
            this.alternateDocument.Resize(newRows, newCols);

            // 如果是修改列大小，那么会自动触发重绘
            // 如果是修改行，那么不会自动触发重绘，要手动重绘
            // 这里偷个懒，不管修改的是列还是行都重绘一次
            this.PerformDrawing(this.activeDocument);

            this.sessionTransport.Resize(newRows, newCols);

            this.ColumnSize = newCols;
            this.RowSize = newRows;
        }

        #endregion
    }
}
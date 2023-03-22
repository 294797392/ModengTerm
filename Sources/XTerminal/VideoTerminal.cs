﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using VideoTerminal.Options;
using XTerminal.Base;
using XTerminal.Channels;
using XTerminal.Document;
using XTerminal.Document.Rendering;
using XTerminal.Parser;
using XTerminal.Rendering;

namespace XTerminal
{
    /// <summary>
    /// 处理虚拟终端的所有逻辑
    /// </summary>
    public class VideoTerminal
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("VideoTerminal");

        private static readonly byte[] OS_OperationStatusResponse = new byte[4] { (byte)'\x1b', (byte)'[', (byte)'0', (byte)'n' };
        private static readonly byte[] CPR_CursorPositionReportResponse = new byte[6] { (byte)'\x1b', (byte)'[', (byte)'0', (byte)';', (byte)'0', (byte)'R' };
        private static readonly byte[] DA_DeviceAttributesResponse = new byte[7] { 0x1b, (byte)'[', (byte)'?', (byte)'1', (byte)':', (byte)'0', (byte)'c' };

        #endregion

        #region 实例变量

        /// <summary>
        /// 与终端进行通信的信道
        /// </summary>
        private VTChannel vtChannel;

        /// <summary>
        /// 终端字符解析器
        /// </summary>
        private VTParser vtParser;

        /// <summary>
        /// 所有行
        /// Row -> VTextLine
        /// </summary>
        private Dictionary<int, VTHistoryLine> historyLines;
        private int historyLineIndex;
        private VTHistoryLine lastHistoryLine;

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
        public IDocumentCanvas Canvas { get; private set; }

        /// <summary>
        /// 文档画布容器
        /// </summary>
        public IDocumentCanvasPanel CanvasPanel { get; set; }

        /// <summary>
        /// 根据当前电脑键盘的按键状态，转换成对应的终端数据流
        /// </summary>
        public VTKeyboard Keyboard { get; private set; }

        public VTextOptions TextOptions { get; private set; }

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
            this.autoWrapMode = this.initialOptions.TerminalOption.DECPrivateAutoWrapMode;

            // 初始化变量
            this.historyLines = new Dictionary<int, VTHistoryLine>();
            this.TextOptions = new VTextOptions();

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

            #region 初始化渲染器

            this.CanvasPanel.InputEvent += this.VideoTerminal_InputEvent;
            this.CanvasPanel.ScrollChanged += this.CanvasPanel_ScrollChanged;
            DocumentCanvasOptions canvasOptions = new DocumentCanvasOptions()
            {
                Rows = initialOptions.TerminalOption.Rows
            };
            IDocumentCanvas characterCanvas = this.CanvasPanel.CreateCanvas();
            characterCanvas.Initialize(canvasOptions);
            this.CanvasPanel.AddCanvas(characterCanvas);
            this.Canvas = characterCanvas;

            #endregion

            #region 初始化文档模型

            VTDocumentOptions documentOptions = new VTDocumentOptions()
            {
                ColumnSize = initialOptions.TerminalOption.Columns,
                RowSize = initialOptions.TerminalOption.Rows,
                DECPrivateAutoWrapMode = initialOptions.TerminalOption.DECPrivateAutoWrapMode,
                CursorStyle = initialOptions.CursorOption.Style,
                Interval = initialOptions.CursorOption.Interval
            };
            this.mainDocument = new VTDocument(documentOptions) { Name = "MainDocument" };
            this.alternateDocument = new VTDocument(documentOptions) { Name = "AlternateDocument" };
            this.activeDocument = this.mainDocument;

            // 初始化文档行数据模型和渲染模型的关联关系
            List<IDocumentDrawable> drawableLines = this.Canvas.GetDrawableLines();
            this.activeDocument.AttachAll(drawableLines);

            #endregion

            #region 初始化光标

            IDocumentDrawable drawableCursor = this.Canvas.GetDrawableCursor();
            this.Cursor.AttachDrawable(drawableCursor);
            this.Canvas.DrawDrawable(drawableCursor);
            this.cursorBlinkingThread = new Thread(this.CursorBlinkingThreadProc);
            this.cursorBlinkingThread.IsBackground = true;
            this.cursorBlinkingThread.Start();

            #endregion

            #region 连接中断通道

            VTChannel vtChannel = VTChannelFactory.Create(options);
            vtChannel.StatusChanged += this.VTChannel_StatusChanged;
            vtChannel.DataReceived += this.VTChannel_DataReceived;
            vtChannel.Connect();
            this.vtChannel = vtChannel;

            #endregion
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Release()
        {

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
                        this.vtChannel.Write(OS_OperationStatusResponse);
                        break;
                    }

                case StatusType.CPR_CursorPositionReport:
                    {
                        // Result is CSI r ; c R
                        int cursorRow = this.CursorRow;
                        int cursorCol = this.CursorCol;
                        CPR_CursorPositionReportResponse[2] = (byte)cursorRow;
                        CPR_CursorPositionReportResponse[4] = (byte)cursorCol;
                        this.vtChannel.Write(CPR_CursorPositionReportResponse);
                        break;
                    }

                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// 渲染一个文档
        /// </summary>
        /// <param name="document">要渲染的文档</param>
        /// <param name="startOffsetY">该文档的起始Y偏移量</param>
        /// <returns>该文档渲染后的底部Y偏移量</returns>
        private double DrawDocument(VTDocumentBase document, double startOffsetY)
        {
            double offsetY = startOffsetY;

            VTextLine next = document.FirstLine;

            while (next != null)
            {
                // 首先获取当前行的DrawingObject
                IDocumentDrawable drawableLine = next.Drawable;
                if (drawableLine == null)
                {
                    // 不应该发生
                    logger.FatalFormat("没有空闲的drawableLine了");
                    return -1;
                }

                // 更新Y偏移量信息
                next.OffsetY = offsetY;

                if (next.IsDirty)
                {
                    // 此时说明该行有字符变化，需要重绘
                    // 重绘的时候会也会UpdatePosition
                    this.Canvas.DrawDrawable(drawableLine);
                    next.SetDirty(false);
                }
                else
                {
                    // 字符没有变化，那么只重新测量然后更新一下文本的偏移量就好了
                    next.Metrics = this.Canvas.MeasureLine(next, 0);
                    this.Canvas.UpdatePosition(drawableLine, next.OffsetX, next.OffsetY);
                }

                // 更新下一个文本行的Y偏移量
                offsetY += next.Metrics.Height;

                // 如果最后一行渲染完毕了，那么就退出
                if (next == document.LastLine)
                {
                    break;
                }

                next = next.NextLine;
            }

            return offsetY;
        }

        /// <summary>
        /// 如果需要布局则进行布局
        /// 如果不需要布局，那么就看是否需要重绘某些文本行
        /// </summary>
        /// <param name="document">要渲染的文档</param>
        private void DrawDocument(VTDocument document)
        {
            // 当前行的Y方向偏移量
            double offsetY = 0;

            this.uiSyncContext.Send((state) =>
            {
                this.DrawDocument(document, offsetY);

                #region 更新光标

                this.Cursor.OffsetY = this.ActiveLine.OffsetY;
                this.Cursor.OffsetX = this.Canvas.MeasureLine(this.ActiveLine, this.CursorCol).WidthIncludingWhitespace;
                this.Canvas.UpdatePosition(this.Cursor.Drawable, this.Cursor.OffsetX, this.Cursor.OffsetY);

                #endregion

            }, null);
        }

        #endregion

        #region 事件处理器

        /// <summary>
        /// 当用户按下按键的时候触发
        /// </summary>
        /// <param name="terminal"></param>
        private void VideoTerminal_InputEvent(IDocumentCanvasPanel canvasPanel, VTInputEvent evt)
        {
            // 这里输入的都是键盘按键
            byte[] bytes = this.Keyboard.TranslateInput(evt);
            if (bytes == null)
            {
                return;
            }

            this.vtChannel.Write(bytes);
        }

        /// <summary>
        /// 当滚动条滚动的时候触发
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="scrollLine">滚动到的行数</param>
        private void CanvasPanel_ScrollChanged(IDocumentCanvasPanel arg1, int scrollLine)
        {
            // 终端可以显示的总行数
            logger.ErrorFormat("ScrollChanged = {0}", scrollLine);
            int terminalRows = this.initialOptions.TerminalOption.Rows;

            // 当前滚动的行是最后一行，不动
            if (scrollLine == this.lastHistoryLine.Row)
            {
                return;
            }

            VTHistoryLine historyLine;
            if (!this.historyLines.TryGetValue(scrollLine, out historyLine))
            {
                logger.ErrorFormat("CanvasPanel_ScrollChanged失败, 找不到对应的VTHistoryLine, scrollLine = {0}", scrollLine);
                return;
            }

            // 找到后面的行数显示
            VTHistoryLine currentHistory = historyLine;
            VTextLine currentTextLine = this.activeDocument.FirstLine;
            for (int i = 0; i < terminalRows; i++)
            {
                currentTextLine.SetHistory(currentHistory);
                currentHistory = currentHistory.NextLine;
                currentTextLine = currentTextLine.NextLine;
            }

            this.activeDocument.IsArrangeDirty = true;
            this.DrawDocument(this.activeDocument);
        }

        private int index = 0;

        private void VtParser_ActionEvent(VTActions action, object parameter)
        {
            switch (action)
            {
                case VTActions.Print:
                    {
                        // 根据测试得出结论：不论字符是单字节字符（英文字母）还是多字节字符（中文..），都只占用一列来显示

                        char ch = Convert.ToChar(parameter);
                        logger.DebugFormat("Print:{0}, cursorRow = {1}, cursorCol = {2}", ch, this.CursorRow, this.CursorCol);
                        this.activeDocument.PrintCharacter(this.ActiveLine, ch, this.CursorCol);
                        this.activeDocument.SetCursor(this.CursorRow, this.CursorCol + 1);
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

                        // 想像一下有一个打印机往一张纸上打字，当打印机想移动到下一行打字的时候，它会发出一个LineFeed指令，让纸往上移动一行
                        // LineFeed，字面意思就是把纸上的下一行喂给打印机使用
                        this.activeDocument.LineFeed();
                        logger.DebugFormat("LineFeed, cursorRow = {0}, cursorCol = {1}, {2}", this.CursorRow, this.CursorCol, action);

                        // 换行之后记录历史行
                        // 因为用户可以输入Backspace键或者上下左右光标键来修改该行的内容
                        // 所以只记录换行之前的行，因为可以确保换行之前的行已经被用户输入完了，不会被更改了
                        // 只记录MainScrrenBuffer里的行，AlternateScrrenBuffer里的行不记录。AlternateScreenBuffer是用来给man，vim等程序使用的
                        if (this.activeDocument == this.mainDocument)
                        {
                            VTextLine previousLine = this.ActiveLine.PreviousLine;
                            previousLine.Metrics = this.Canvas.MeasureLine(previousLine, 0);

                            VTHistoryLine historyLine = VTHistoryLine.Create(this.historyLineIndex, this.lastHistoryLine, previousLine);
                            this.historyLines[this.historyLineIndex] = historyLine;
                            this.lastHistoryLine = historyLine;

                            int terminalRows = this.initialOptions.TerminalOption.Rows;
                            int scrollMax = this.historyLineIndex - terminalRows + 2;
                            //logger.InfoFormat("scrollMax = {0}", scrollMax);
                            if (scrollMax > 0)
                            {
                                logger.ErrorFormat("scrollMax = {0}", scrollMax);
                                this.uiSyncContext.Send((state) =>
                                {
                                    this.CanvasPanel.UpdateScrollInfo(scrollMax);
                                    this.CanvasPanel.ScrollToEnd(ScrollOrientation.Down);
                                }, null);
                            }

                            this.historyLineIndex++;
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

                        logger.DebugFormat("CUP_CursorPosition, row = {0}, col = {1}, {2}", row, col, this.index++);
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

                        this.mainDocument.DetachAll();

                        // 切换ActiveDocument
                        // 这里只重置行数，在用户调整窗口大小的时候需要执行终端的Resize操作
                        this.alternateDocument.SetScrollMargin(0, 0);
                        this.alternateDocument.DeleteAll();
                        this.alternateDocument.AttachAll(this.Canvas.GetDrawableLines());
                        this.alternateDocument.Cursor.AttachDrawable(this.Canvas.GetDrawableCursor());
                        this.activeDocument = this.alternateDocument;
                        break;
                    }

                case VTActions.UseMainScreenBuffer:
                    {
                        logger.DebugFormat("UseMainScreenBuffer");

                        this.alternateDocument.DetachAll();

                        this.mainDocument.AttachAll(this.Canvas.GetDrawableLines());
                        this.mainDocument.DirtyAll();
                        this.mainDocument.Cursor.AttachDrawable(this.Canvas.GetDrawableCursor());
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
                        this.vtChannel.Write(DA_DeviceAttributesResponse);
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
                        int lines = this.initialOptions.TerminalOption.Rows;

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

        private void VTChannel_DataReceived(VTChannel client, byte[] bytes)
        {
            //string str = string.Join(",", bytes.Select(v => v.ToString()).ToList());
            //logger.InfoFormat("Received, {0}", str);
            this.vtParser.ProcessCharacters(bytes);

            // 全部字符都处理完了之后，只渲染一次

            this.DrawDocument(this.activeDocument);
            //this.activeDocument.Print();
            //logger.ErrorFormat("TotalRows = {0}", this.activeDocument.TotalRows);
        }

        private void VTChannel_StatusChanged(object client, VTChannelState state)
        {
            logger.InfoFormat("客户端状态发生改变, {0}", state);
        }

        private void CursorBlinkingThreadProc()
        {
            while (true)
            {
                VTCursor cursor = this.Cursor;

                cursor.IsVisible = !cursor.IsVisible;

                IDocumentDrawable drawableCursor = cursor.Drawable;
                if (drawableCursor == null)
                {
                    // 此时可能正在切换AlternateScreenBuffer
                    Thread.Sleep(cursor.Interval);
                    continue;
                }

                try
                {
                    double opacity = cursor.IsVisible ? 1 : 0;

                    this.uiSyncContext.Send((state) =>
                    {
                        this.Canvas.SetOpacity(drawableCursor, opacity);
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

        #endregion
    }
}

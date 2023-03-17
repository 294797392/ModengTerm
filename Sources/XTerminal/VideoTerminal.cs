using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using VideoTerminal.Options;
using XTerminal.Channels;
using XTerminal.Document;
using XTerminal.Document.Rendering;
using XTerminal.Parser;
using XTerminal.Rendering;
using XTerminal.VTDefinitions;

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
        private Dictionary<int, VTextLine> textLines;

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
        /// Terminal区域的总长宽
        /// </summary>
        private double fullWidth;
        private double fullHeight;

        /// <summary>
        /// SIMD - SELECT IMPLICIT MOVEMENT DIRECTION
        /// 0：数据的移动方向和字符方向一致（从左到右）
        /// 1：数据的移动方向和字符方向相反（从右到左）
        /// </summary>
        private int implicitMovementDirection;

        #region Modes - ECMA048 - Modes

        /// <summary>
        /// DCSM - DEVICE COMPONENT SELECT MODE
        /// PRESENTATION:
        /// 某些控制功能在Presentaion模式下执行，
        /// DATA;
        /// 某些控制功能在Data模式下执行，
        /// 
        /// 默认PRESENTATION状态
        /// 
        /// 受DCSM状态影响的控制功能有：CPR, CR, DCH, DL, EA, ECH, ED, EF, EL, ICH, IL, LF, NEL, RI, SLH, SLL, SPH, SPL
        /// </summary>
        private int deviceComponentSelectMode;

        #endregion

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
        /// 输入设备
        /// </summary>
        public IInputDevice InputDevice { get; set; }

        /// <summary>
        /// 文档渲染器
        /// </summary>
        public IDocumentRenderer Renderer { get; set; }

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

        public void Initialize(VTInitialOptions options)
        {
            this.initialOptions = options;
            this.uiSyncContext = SynchronizationContext.Current;

            this.autoWrapMode = this.initialOptions.TerminalOption.DECPrivateAutoWrapMode;

            // 0:和字符方向相同（向右）
            // 1:和字符方向相反（向左）
            this.implicitMovementDirection = 0;

            // 初始化变量
            this.textLines = new Dictionary<int, VTextLine>();
            this.TextOptions = new VTextOptions();

            // 初始化键盘
            this.Keyboard = new VTKeyboard();
            this.Keyboard.SetAnsiMode(true);
            this.Keyboard.SetKeypadMode(false);

            // 初始化视频终端
            this.InputDevice.InputEvent += this.VideoTerminal_InputEvent;

            // 初始化终端解析器
            this.vtParser = new VTParser();
            this.vtParser.ActionEvent += VtParser_ActionEvent;
            this.vtParser.Initialize();

            #region 初始化渲染器

            DocumentRendererOptions rendererOptions = new DocumentRendererOptions()
            {
                Rows = initialOptions.TerminalOption.Rows
            };
            this.Renderer.Initialize(rendererOptions);

            #endregion

            #region 初始化文档模型

            VTDocumentOptions documentOptions = new VTDocumentOptions()
            {
                Columns = initialOptions.TerminalOption.Columns,
                Rows = initialOptions.TerminalOption.Rows,
                DECPrivateAutoWrapMode = initialOptions.TerminalOption.DECPrivateAutoWrapMode,
                CursorStyle = initialOptions.CursorOption.Style,
                Interval = initialOptions.CursorOption.Interval
            };
            this.mainDocument = new VTDocument(documentOptions) { Name = "MainDocument" };
            this.alternateDocument = new VTDocument(documentOptions) { Name = "AlternateDocument" };
            this.activeDocument = this.mainDocument;

            // 初始化文档行数据模型和渲染模型的关联关系
            ViewableDocument document = this.activeDocument.ViewableArea;
            List<IDocumentDrawable> drawableLines = this.Renderer.GetDrawableLines();
            this.activeDocument.AttachAll(drawableLines);

            #endregion

            #region 初始化光标

            IDocumentDrawable drawableCursor = this.Renderer.GetDrawableCursor();
            this.Cursor.AttachDrawable(drawableCursor);
            this.Renderer.DrawDrawable(drawableCursor);
            this.cursorBlinkingThread = new Thread(this.CursorBlinkingThreadProc);
            this.cursorBlinkingThread.IsBackground = true;
            this.cursorBlinkingThread.Start();

            #endregion

            // 连接终端通道
            VTChannel vtChannel = VTChannelFactory.Create(options);
            vtChannel.StatusChanged += this.VTChannel_StatusChanged;
            vtChannel.DataReceived += this.VTChannel_DataReceived;
            vtChannel.Connect();
            this.vtChannel = vtChannel;
        }

        #endregion

        #region 实例方法

        /// <summary>
        /// 重新测量Terminal所需要的大小
        /// 如果大小改变了，那么调整布局大小
        /// </summary>
        //private void InvalidateMeasure()
        //{
        //    if (this.ActiveLine == null)
        //    {
        //        logger.ErrorFormat("InvalidateMeasure失败, activeLine不存在");
        //        return;
        //    }

        //    double width = Math.Max(this.ActiveLine.Bounds.RightBottom.X, this.fullWidth);
        //    double height = Math.Max(this.ActiveLine.Bounds.RightBottom.Y, this.fullHeight);

        //    // 布局大小是否改变了
        //    bool sizeChanged = false;

        //    // 长宽和原来的完整长宽不一样就算改变了
        //    if (width != this.fullWidth || height != this.fullHeight)
        //    {
        //        sizeChanged = true;
        //    }

        //    if (sizeChanged)
        //    {
        //        this.fullWidth = width;
        //        this.fullHeight = height;

        //        this.Renderer.Resize(width, height);
        //        this.Renderer.ScrollToEnd(ScrollOrientation.Bottom);
        //    }
        //}

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
                    // 重绘的时候会也会Arrange
                    this.Renderer.DrawDrawable(drawableLine);
                    next.SetDirty(false);
                }
                else
                {
                    // 字符没有变化，那么只重新测量然后更新一下文本的偏移量就好了
                    next.Metrics = this.Renderer.MeasureLine(next, 0);
                    this.Renderer.UpdatePosition(drawableLine, next.OffsetX, next.OffsetY);
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
                // 先渲染顶部Margin区域
                if (!document.TopMarginArea.IsEmpty)
                {
                    offsetY = this.DrawDocument(document.TopMarginArea, offsetY);
                }

                // 再渲染中间可视区域
                offsetY = this.DrawDocument(document.ViewableArea, offsetY);

                // 最后渲染底部Margin区域
                if (!document.BottomMarginArea.IsEmpty)
                {
                    offsetY = this.DrawDocument(document.BottomMarginArea, offsetY);
                }

                #region 更新光标

                this.Cursor.OffsetY = this.ActiveLine.OffsetY;
                this.Cursor.OffsetX = this.Renderer.MeasureLine(this.ActiveLine, this.CursorCol).WidthIncludingWhitespace;
                this.Renderer.UpdatePosition(this.Cursor.Drawable, this.Cursor.OffsetX, this.Cursor.OffsetY);

                #endregion

            }, null);
        }

        #endregion

        #region 事件处理器

        /// <summary>
        /// 当用户按下按键的时候触发
        /// </summary>
        /// <param name="terminal"></param>
        private void VideoTerminal_InputEvent(IInputDevice terminal, VTInputEvent evt)
        {
            // todo:translate and send to remote host
            if (string.IsNullOrEmpty(evt.Text))
            {
                {
                    // 这里输入的都是键盘按键
                    byte[] bytes = this.Keyboard.TranslateInput(evt);
                    if (bytes == null)
                    {
                        return;
                    }

                    this.vtChannel.Write(bytes);
                }
            }
            else
            {
                // 这里输入的都是中文
            }
        }

        private int index = 0;

        private void VtParser_ActionEvent(VTActions action, params object[] param)
        {
            switch (action)
            {
                case VTActions.Print:
                    {
                        char ch = (char)param[0];
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

                case VTActions.EL_EraseLine:
                    {
                        EraseType eraseType = (EraseType)param[0];
                        logger.DebugFormat("EL_EraseLine, eraseType = {0}, cursorRow = {1}, cursorCol = {2}", eraseType, this.CursorRow, this.CursorCol);
                        this.activeDocument.EraseLine(this.ActiveLine, this.CursorCol, eraseType);
                        break;
                    }

                #region 光标移动

                // 下面的光标移动指令不能进行VTDocument的滚动
                // 光标的移动坐标是相对于可视区域内的坐标

                case VTActions.CursorBackward:
                    {
                        int n = Convert.ToInt32(param[0]);
                        this.activeDocument.SetCursor(this.CursorRow, this.CursorCol - n);
                        logger.DebugFormat("CursorBackward, cursorRow = {0}, cursorCol = {1}", this.CursorRow, this.CursorCol);
                        break;
                    }

                case VTActions.CUF_CursorForward:
                    {
                        int n = Convert.ToInt32(param[0]);
                        this.activeDocument.SetCursor(this.CursorRow, this.CursorCol + n);
                        break;
                    }

                case VTActions.CUU_CursorUp:
                    {
                        int n = Convert.ToInt32(param[0]);
                        this.activeDocument.SetCursor(this.CursorRow - n, this.CursorCol);
                        break;
                    }

                case VTActions.CUD_CursorDown:
                    {
                        int n = Convert.ToInt32(param[0]);
                        this.activeDocument.SetCursor(this.CursorRow + n, this.CursorCol);
                        break;
                    }

                case VTActions.CUP_CursorPosition:
                    {
                        int row = Convert.ToInt32(param[0]);
                        int col = Convert.ToInt32(param[1]);

                        logger.DebugFormat("CUP_CursorPosition, row = {0}, col = {1}, {2}", row, col, this.index++);

                        // 把相对于ViewableDocument的光标坐标转换成相对于整个VTDocument的光标坐标
                        ViewableDocument document = this.activeDocument.ViewableArea;

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
                        VTMode vtMode = (VTMode)param[0];
                        logger.DebugFormat("DECANM_AnsiMode, mode = {0}", vtMode);
                        this.Keyboard.SetAnsiMode(vtMode == VTMode.AnsiMode);
                        break;
                    }

                case VTActions.SetCursorKeyMode:
                    {
                        VTCursorKeyMode cursorKeyMode = (VTCursorKeyMode)param[0];
                        this.Keyboard.SetCursorKeyMode(cursorKeyMode == VTCursorKeyMode.ApplicationMode);
                        break;
                    }

                case VTActions.SetKeypadMode:
                    {
                        VTKeypadMode keypadMode = (VTKeypadMode)param[0];
                        this.Keyboard.SetKeypadMode(keypadMode == VTKeypadMode.ApplicationMode);
                        break;
                    }

                case VTActions.AutoWrapMode:
                    {
                        this.autoWrapMode = (bool)param[0];
                        this.activeDocument.DECPrivateAutoWrapMode = this.autoWrapMode;
                        break;
                    }

                case VTActions.XTERM_BracketedPasteMode:
                    {
                        this.xtermBracketedPasteMode = (bool)param[0];
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
                        // 从指定位置删除n个字符，删除后的字符串要左对齐
                        int count = Convert.ToInt32(param[0]);
                        this.activeDocument.DeleteCharacter(this.ActiveLine, this.CursorCol, count);
                        break;
                    }

                case VTActions.ICH_InsertCharacter:
                    {
                        // 在当前光标处插入N个空白字符,这会将所有现有文本移到右侧。 向右溢出屏幕的文本会被删除
                        // 目前没发现这个操作对终端显示有什么影响，所以暂时不实现
                        int count = Convert.ToInt32(param[0]);
                        logger.ErrorFormat("未实现InsertCharacters, {0}, cursorPos = {1}", count, this.CursorCol);
                        break;
                    }

                #endregion

                case VTActions.UseAlternateScreenBuffer:
                    {
                        logger.DebugFormat("UseAlternateScreenBuffer");

                        // 先记录当前的光标
                        this.mainDocument.Cursor.OwnerLine = this.ActiveLine;
                        this.mainDocument.DetachAll();

                        // 切换ActiveDocument
                        // 这里只重置行数，在用户调整窗口大小的时候需要执行终端的Resize操作
                        this.alternateDocument.ResetRows();
                        this.alternateDocument.ViewableArea.DeleteAll();
                        this.alternateDocument.AttachAll(this.Renderer.GetDrawableLines());
                        this.alternateDocument.Cursor.AttachDrawable(this.Renderer.GetDrawableCursor());
                        // 更新activeLine为AlternateDocument的第一行
                        this.activeDocument = this.alternateDocument;
                        break;
                    }

                case VTActions.UseMainScreenBuffer:
                    {
                        this.alternateDocument.DetachAll();

                        logger.DebugFormat("UseMainScreenBuffer");

                        // 恢复之前保存的光标
                        this.mainDocument.AttachAll(this.Renderer.GetDrawableLines());
                        this.mainDocument.ViewableArea.DirtyAll();
                        this.mainDocument.Cursor.AttachDrawable(this.Renderer.GetDrawableCursor());
                        this.activeDocument = this.mainDocument;
                        break;
                    }

                case VTActions.ED_EraseDisplay:
                    {
                        int parameter = Convert.ToInt32(param[0]);
                        this.activeDocument.EraseDisplay(this.ActiveLine, this.CursorCol, (EraseType)parameter);
                        break;
                    }

                case VTActions.DSR_DeviceStatusReport:
                    {
                        StatusType statusType = (StatusType)Convert.ToInt32(param[0]);
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
                        // 但是在测试的时候，打开VIM，topMargin和bottomMargin都被设置为了1，如果不设置的话貌似显示不正常，所以当topMargin和bottomMargin一致的时候也执行
                        // 边距还会影响插入行 (IL) 和删除行 (DL)、向上滚动 (SU) 和向下滚动 (SD) 修改的行。

                        int topMargin = Convert.ToInt32(param[0]);
                        int bottomMargin = Convert.ToInt32(param[1]);
                        logger.DebugFormat("SetScrollingRegion, topMargin = {0}, bottomMargin = {1}", topMargin, bottomMargin);

                        if (bottomMargin < 0 || topMargin < 0)
                        {
                            // 参考：https://github.com/microsoft/terminal/issues/1849
                            logger.ErrorFormat("DECSTBM_SetScrollingRegion参数不正确，忽略本次设置");
                            return;
                        }
                        if (topMargin > this.initialOptions.TerminalOption.Rows ||
                            bottomMargin > this.initialOptions.TerminalOption.Rows ||
                            bottomMargin + topMargin > this.initialOptions.TerminalOption.Rows)
                        {
                            // 上边距加下边距超出了整个可视区域的范围了
                            return;
                        }
                        if (topMargin >= bottomMargin)
                        {
                            // 上边距必须小于下边距
                            // 打开VIM的时候，上边距和下边距分别是1和1
                            topMargin = bottomMargin - 1;
                        }

                        // Margin目前的实现方式：
                        // 相当于是把ViewableDocument缩小bottomMargin/topMargin行，然后创建MarginLine用来填充剩余的区域
                        this.activeDocument.DetachAll();
                        this.activeDocument.SetScrollMargin(topMargin, bottomMargin);
                        // 设置完滚动区域后重新附加渲染模型
                        List<IDocumentDrawable> drawableLines = this.Renderer.GetDrawableLines();
                        this.activeDocument.AttachAll(drawableLines);
                        break;
                    }

                case VTActions.IL_InsertLine:
                    {
                        // 将 <n> 行插入光标位置的缓冲区。 光标所在的行及其下方的行将向下移动。
                        int lines = Convert.ToInt32(param[0]);
                        logger.DebugFormat("IL_InsertLine, lines = {0}", lines);
                        this.activeDocument.InsertLines(this.ActiveLine, lines);
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
                        this.Renderer.SetOpacity(drawableCursor, opacity);
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

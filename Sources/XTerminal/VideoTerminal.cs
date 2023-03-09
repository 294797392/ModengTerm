using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using VideoTerminal.Options;
using XTerminal.Channels;
using XTerminal.Document;
using XTerminal.Parser;
using XTerminal.Terminal;

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
        private static readonly byte[] DA_DeviceAttributesResponse = new byte[7] { (byte)'\x1b', (byte)'[', (byte)'?', (byte)'1', (byte)':', (byte)'0', (byte)'c' };

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
        /// 空白字符的测量信息
        /// </summary>
        private VTextMetrics blankCharacterMetrics;

        /// <summary>
        /// 光标所在行
        /// </summary>
        private int cursorRow;

        /// <summary>
        /// 光标所在列
        /// </summary>
        private int cursorCol;

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

        #endregion

        #region 属性

        /// <summary>
        /// 输入设备
        /// </summary>
        public IInputDevice InputDevice { get; set; }

        /// <summary>
        /// 终端显示器接口
        /// </summary>
        public IVTMonitor Monitor { get; set; }

        /// <summary>
        /// 根据当前电脑键盘的按键状态，转换成对应的终端数据流
        /// </summary>
        public VTKeyboard Keyboard { get; private set; }

        public VTextOptions TextOptions { get; private set; }

        /// <summary>
        /// 光标所在行
        /// </summary>
        public VTextLine ActiveLine { get { return this.activeDocument.ActiveLine; } }

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

            this.blankCharacterMetrics = this.Monitor.MeasureText(" ", VTextStyle.Default);

            #region 初始化文档模型

            VTDocumentOptions documentOptions = new VTDocumentOptions()
            {
                Columns = initialOptions.TerminalOption.Columns,
                DECPrivateAutoWrapMode = initialOptions.TerminalOption.DECPrivateAutoWrapMode
            };
            this.mainDocument = new VTDocument(documentOptions) { Name = "MainDocument" };
            this.alternateDocument = new VTDocument(documentOptions) { Name = "AlternateDocument" };
            this.activeDocument = this.mainDocument;

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
        private void InvalidateMeasure()
        {
            if (this.ActiveLine == null)
            {
                logger.ErrorFormat("InvalidateMeasure失败, activeLine不存在");
                return;
            }

            double width = Math.Max(this.ActiveLine.Bounds.RightBottom.X, this.fullWidth);
            double height = Math.Max(this.ActiveLine.Bounds.RightBottom.Y, this.fullHeight);

            // 布局大小是否改变了
            bool sizeChanged = false;

            // 长宽和原来的完整长宽不一样就算改变了
            if (width != this.fullWidth || height != this.fullHeight)
            {
                sizeChanged = true;
            }

            if (sizeChanged)
            {
                this.fullWidth = width;
                this.fullHeight = height;

                this.Monitor.Resize(width, height);
                this.Monitor.ScrollToEnd(ScrollOrientation.Bottom);
            }
        }

        private void DrawLine(VTextLine textLine)
        {
            if (textLine == null)
            {
                logger.WarnFormat("DrawLine失败, Line不存在");
                return;
            }

            this.uiSyncContext.Send((v) =>
            {
                this.Monitor.DrawLine(textLine);
                this.InvalidateMeasure();
            }, null);
        }

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
                        int cursorRow = this.activeDocument.Cursor.Row;
                        int cursorCol = this.activeDocument.Cursor.Column;
                        CPR_CursorPositionReportResponse[2] = (byte)cursorRow;
                        CPR_CursorPositionReportResponse[4] = (byte)cursorCol;
                        this.vtChannel.Write(CPR_CursorPositionReportResponse);
                        break;
                    }

                default:
                    throw new NotImplementedException();
            }
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

        private void VtParser_ActionEvent(VTActions action, params object[] param)
        {
            switch (action)
            {
                case VTActions.Print:
                    {
                        char ch = (char)param[0];
                        logger.DebugFormat("Print:{0}, cursorRow = {1}, cursorCol = {2}", ch, this.cursorRow, this.cursorCol);
                        this.activeDocument.PrintCharacter(ch, this.cursorCol);
                        this.cursorCol++;
                        this.activeDocument.SetCursor(this.cursorRow, this.cursorCol);
                        this.DrawLine(this.ActiveLine);
                        break;
                    }

                case VTActions.CarriageReturn:
                    {
                        // CR
                        // 把光标移动到行开头
                        this.cursorCol = 0;
                        this.activeDocument.SetCursor(this.cursorRow, this.cursorCol);
                        logger.DebugFormat("CarriageReturn, cursorRow = {0}, cursorCol = {1}", this.cursorRow, this.cursorCol);
                        break;
                    }

                case VTActions.FF:
                case VTActions.VT:
                case VTActions.LF:
                    {
                        // LF
                        this.cursorRow++;
                        this.activeDocument.CreateNextLine();
                        this.activeDocument.SetCursor(this.cursorRow, this.cursorCol);
                        // 空行要画一下，不然该行就没有位置信息，在创建下一行的时候下一行的位置信息就会出问题
                        this.DrawLine(this.ActiveLine);
                        logger.DebugFormat("LineFeed, cursorRow = {0}, cursorCol = {1}, {2}", this.cursorRow, this.cursorCol, action);
                        break;
                    }

                case VTActions.EL_EraseLine:
                    {
                        EraseType eraseType = (EraseType)param[0];
                        logger.DebugFormat("EL_EraseLine, eraseType = {0}, cursorRow = {1}, cursorCol = {2}", eraseType, this.cursorRow, this.cursorCol);
                        this.activeDocument.EraseLine(eraseType);
                        this.DrawLine(this.activeDocument.ActiveLine);
                        break;
                    }

                #region 光标移动

                case VTActions.CursorBackward:
                    {
                        this.cursorCol--;
                        logger.DebugFormat("CursorBackward, cursorRow = {0}, cursorCol = {1}", this.cursorRow, this.cursorCol);
                        this.activeDocument.SetCursor(this.cursorRow, this.cursorCol);
                        break;
                    }

                case VTActions.CUF_CursorForward:
                    {
                        int n = Convert.ToInt32(param[0]);
                        this.cursorCol += n;
                        this.activeDocument.SetCursor(this.cursorRow, this.cursorCol);
                        break;
                    }

                case VTActions.CUU_CursorUp:
                    {
                        int n = Convert.ToInt32(param[0]);
                        this.cursorRow -= n;
                        this.activeDocument.SetCursor(this.cursorRow, this.cursorCol);
                        break;
                    }

                case VTActions.CUD_CursorDown:
                    {
                        int n = Convert.ToInt32(param[0]);
                        this.cursorRow += n;
                        this.activeDocument.SetCursor(this.cursorRow, this.cursorCol);
                        break;
                    }

                case VTActions.CUP_CursorPosition:
                    {
                        int row = Convert.ToInt32(param[0]);
                        int col = Convert.ToInt32(param[1]);
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

                case VTActions.SetVTMode:
                    {
                        VTMode vtMode = (VTMode)param[0];
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

                case VTActions.DCH_DeleteCharacter:
                    {
                        int count = Convert.ToInt32(param[0]);
                        this.activeDocument.DeleteCharacter(count);
                        this.DrawLine(this.ActiveLine);
                        break;
                    }

                case VTActions.ICH_InsertCharacter:
                    {
                        // 目前没发现这个操作对终端显示有什么影响，所以暂时不实现
                        int count = Convert.ToInt32(param[0]);
                        logger.ErrorFormat("未实现InsertCharacters, {0}, cursorPos = {1}", count, this.cursorCol);
                        break;
                    }

                case VTActions.UseAlternateScreenBuffer:
                    {
                        this.cursorCol = 0;
                        this.cursorRow = 0;
                        this.alternateDocument.Reset();
                        this.alternateDocument.SetCursor(0, 0);
                        this.activeDocument = this.alternateDocument;
                        this.uiSyncContext.Send((state) =>
                        {
                            this.Monitor.DrawDocument(this.alternateDocument);
                        }, null);
                        break;
                    }

                case VTActions.UseMainScreenBuffer:
                    {
                        this.cursorCol = this.mainDocument.Cursor.Column;
                        this.cursorRow = this.mainDocument.Cursor.Row;
                        this.activeDocument = this.mainDocument;
                        this.uiSyncContext.Send((state) =>
                        {
                            this.Monitor.DrawDocument(this.mainDocument);
                        }, null);
                        break;
                    }

                case VTActions.ED_EraseDisplay:
                    {
                        int parameter = Convert.ToInt32(param[0]);
                        this.activeDocument.EraseDisplay((EraseType)parameter);
                        break;
                    }

                case VTActions.DSR_DeviceStatusReport:
                    {
                        StatusType statusType = (StatusType)Convert.ToInt32(param[0]);
                        this.PerformDeviceStatusReport(statusType);
                        break;
                    }

                case VTActions.DA_DeviceAttributes:
                    {
                        this.vtChannel.Write(DA_DeviceAttributesResponse);
                        break;
                    }

                case VTActions.RI_ReverseLineFeed:
                    {
                        this.cursorRow--;
                        this.activeDocument.SetCursor(this.cursorRow, this.cursorCol);
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
        }

        private void VTChannel_StatusChanged(object client, VTChannelState state)
        {
            logger.InfoFormat("客户端状态发生改变, {0}", state);
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using VideoTerminal.Options;
using XTerminal.Channels;
using XTerminal.Document;
using XTerminal.Document.Rendering;
using XTerminal.Parser;
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
        /// 鼠标所在行
        /// </summary>
        private VTextLine activeLine;

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

            this.blankCharacterMetrics = this.Renderer.MeasureText(" ", VTextStyle.Default);

            #region 初始化文档模型

            VTDocumentOptions documentOptions = new VTDocumentOptions()
            {
                Columns = initialOptions.TerminalOption.Columns,
                Rows = initialOptions.TerminalOption.Rows,
                DECPrivateAutoWrapMode = initialOptions.TerminalOption.DECPrivateAutoWrapMode
            };
            this.mainDocument = new VTDocument(documentOptions) { Name = "MainDocument" };
            this.alternateDocument = new VTDocument(documentOptions) { Name = "AlternateDocument" };
            this.activeDocument = this.mainDocument;
            this.activeDocument.TryGetLine(0, out this.activeLine);

            #endregion

            #region 初始化渲染器

            DocumentRendererOptions rendererOptions = new DocumentRendererOptions()
            {
                Rows = initialOptions.TerminalOption.Rows
            };
            this.Renderer.Initialize(rendererOptions);

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
                        int cursorRow = this.cursorRow;
                        int cursorCol = this.cursorCol;
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
        /// 如果需要布局则进行布局
        /// 如果不需要布局，那么就看是否需要重绘某些文本行
        /// </summary>
        /// <param name="vtDocument"></param>
        private void RenderDocument(VTDocument vtDocument)
        {
            if (vtDocument.IsArrangeDirty)
            {
                this.uiSyncContext.Send((state) =>
                {
                    this.Renderer.RenderDocument(vtDocument);
                }, null);

                vtDocument.IsArrangeDirty = false;
            }
            else
            {
                ViewableDocument document = vtDocument.ViewableArea;

                VTextLine next = document.FirstLine;
                while (next != null)
                {
                    if (next.IsCharacterDirty)
                    {
                        // 需要重绘
                        this.Renderer.RenderElement(next.DrawingElement);
                        next.IsCharacterDirty = false;
                    }

                    if (next == document.LastLine)
                    {
                        // 最后一行渲染完了，退出
                        break;
                    }

                    next = next.NextLine;
                }
            }
        }

        private bool UpdateActiveLine(int row)
        {
            if (this.activeLine.Row != row)
            {
                VTextLine newActiveLine;
                if (!this.activeDocument.TryGetLine(row, out newActiveLine))
                {
                    logger.ErrorFormat("UpdateActiveLine失败, 没找到对应的行, row = {0}, lastRow = {1}, activeDocument = {2}", row, this.activeDocument.LastLine.Row, this.activeDocument.Name);
                    return false;
                }

                this.activeLine = newActiveLine;
            }

            return true;
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
                        this.activeDocument.PrintCharacter(this.activeLine, ch, this.cursorCol);
                        this.cursorCol++;
                        break;
                    }

                case VTActions.CarriageReturn:
                    {
                        // CR
                        // 把光标移动到行开头
                        this.cursorCol = 0;
                        logger.DebugFormat("CarriageReturn, cursorRow = {0}, cursorCol = {1}", this.cursorRow, this.cursorCol);
                        break;
                    }

                case VTActions.FF:
                case VTActions.VT:
                case VTActions.LF:
                    {
                        // LF

                        // 想像一下有一个打印机往一张纸上打字，当打印机想移动到下一行打字的时候，它会发出一个LineFeed指令，让纸往上移动一行
                        // LineFeed，字面意思就是把纸上的下一行喂给打印机使用

                        if (!this.activeDocument.ContainsLine(this.cursorRow + 1))
                        {
                            this.activeDocument.CreateNextLine();
                        }

                        // 更新可视区域
                        ViewableDocument document = this.activeDocument.ViewableArea;
                        int firstVisibleRow = document.FirstLine.Row;
                        int lastVisibleRow = document.LastLine.Row;

                        if (this.cursorRow == lastVisibleRow)
                        {
                            // 光标在可视区域的最后一行，那么要把可视区域向下移动
                            logger.DebugFormat("LineFeed，光标在可视区域最后一行，向下移动一行并且可视区域往下移动一行");
                            this.activeDocument.ScrollViewableDocument(ScrollOrientation.Down, 1);
                            this.cursorRow++;
                        }
                        else if (this.cursorRow < firstVisibleRow)
                        {
                            // 光标位置在可视区域上面？？
                            logger.ErrorFormat("LineFeed状态不正确，光标在可视区域上面");
                        }
                        else if (this.cursorRow > lastVisibleRow)
                        {
                            // 光标位置在可视区域的下面？？
                            logger.ErrorFormat("LineFeed状态不正确，光标在可视区域下面");
                        }
                        else
                        {
                            // 光标在可视区域里
                            logger.DebugFormat("LineFeed，光标在可视区域里，直接移动光标到下一行");
                            this.cursorRow++;
                        }
                        this.UpdateActiveLine(this.cursorRow);
                        logger.DebugFormat("LineFeed, cursorRow = {0}, cursorCol = {1}, {2}", this.cursorRow, this.cursorCol, action);
                        break;
                    }

                case VTActions.EL_EraseLine:
                    {
                        EraseType eraseType = (EraseType)param[0];
                        logger.DebugFormat("EL_EraseLine, eraseType = {0}, cursorRow = {1}, cursorCol = {2}", eraseType, this.cursorRow, this.cursorCol);
                        this.activeDocument.EraseLine(this.activeLine, this.cursorCol, eraseType);
                        break;
                    }

                #region 光标移动

                // 下面的光标移动指令不能进行VTDocument的滚动
                // 光标的移动坐标是相对于可视区域内的坐标

                case VTActions.CursorBackward:
                    {
                        int n = Convert.ToInt32(param[0]);
                        this.cursorCol -= n;
                        logger.DebugFormat("CursorBackward, cursorRow = {0}, cursorCol = {1}", this.cursorRow, this.cursorCol);
                        break;
                    }

                case VTActions.CUF_CursorForward:
                    {
                        int n = Convert.ToInt32(param[0]);
                        this.cursorCol += n;
                        break;
                    }

                case VTActions.CUU_CursorUp:
                    {
                        int n = Convert.ToInt32(param[0]);
                        this.cursorRow -= n;
                        this.UpdateActiveLine(this.cursorRow);
                        break;
                    }

                case VTActions.CUD_CursorDown:
                    {
                        int n = Convert.ToInt32(param[0]);
                        this.cursorRow += n;
                        this.UpdateActiveLine(this.cursorRow);
                        break;
                    }

                case VTActions.CUP_CursorPosition:
                    {
                        int row = Convert.ToInt32(param[0]);
                        int col = Convert.ToInt32(param[1]);

                        logger.DebugFormat("CUP_CursorPosition, row = {0}, col = {1}", row, col);

                        // 把相对于ViewableDocument的光标坐标转换成相对于整个VTDocument的光标坐标
                        ViewableDocument document = this.activeDocument.ViewableArea;
                        int firstVisibleRow = document.FirstLine.Row;
                        this.cursorRow = firstVisibleRow + row;
                        this.cursorCol = col;
                        this.UpdateActiveLine(this.cursorRow);
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
                        this.activeDocument.DeleteCharacter(this.activeLine, this.cursorCol, count);
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
                        logger.DebugFormat("UseAlternateScreenBuffer");

                        // 先记录当前的光标
                        this.activeDocument.Cursor.Row = this.cursorRow;
                        this.activeDocument.Cursor.Column = this.cursorCol;

                        this.cursorCol = 0;
                        this.cursorRow = 0;
                        this.activeDocument = this.alternateDocument;
                        this.activeDocument.ViewableArea.DirtyAll();
                        this.alternateDocument.Reset();
                        this.alternateDocument.Clear();
                        this.UpdateActiveLine(this.cursorRow);
                        this.uiSyncContext.Send((state) =>
                        {
                            this.Renderer.Reset();
                            this.RenderDocument(this.alternateDocument);
                        }, null);
                        break;
                    }

                case VTActions.UseMainScreenBuffer:
                    {
                        logger.DebugFormat("UseMainScreenBuffer");

                        // 恢复之前保存的光标
                        this.cursorCol = this.mainDocument.Cursor.Column;
                        this.cursorRow = this.mainDocument.Cursor.Row;
                        this.activeDocument = this.mainDocument;
                        this.activeDocument.ViewableArea.DirtyAll();
                        this.UpdateActiveLine(this.cursorRow);
                        this.uiSyncContext.Send((state) =>
                        {
                            this.Renderer.Reset();
                            this.RenderDocument(this.mainDocument);
                        }, null);
                        break;
                    }

                case VTActions.ED_EraseDisplay:
                    {
                        int parameter = Convert.ToInt32(param[0]);
                        this.activeDocument.EraseDisplay(this.activeLine, this.cursorCol, (EraseType)parameter);
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
                        // 和LineFeed相反，也就是把光标往上移一个位置
                        // 在用man命令的时候会触发这个指令
                        // 反向索引 – 执行\n的反向操作，将光标向上移动一行，维护水平位置，如有必要，滚动缓冲区 *

                        ViewableDocument document = this.activeDocument.ViewableArea;
                        int firstVisibleRow = document.FirstLine.Row;
                        int lastVisibleRow = document.LastLine.Row;

                        if (this.cursorRow == firstVisibleRow)
                        {
                            // 此时光标位置在可视区域的第一行
                            logger.DebugFormat("RI_ReverseLineFeed，光标在可视区域第一行，向上移动一行并且可视区域往上移动一行");
                            this.activeDocument.ScrollViewableDocument(ScrollOrientation.Up, 1);
                            this.cursorRow--;
                            this.UpdateActiveLine(this.cursorRow);
                        }
                        else if (this.cursorRow < firstVisibleRow)
                        {
                            // 光标位置在可视区域上面？？
                            logger.ErrorFormat("RI_ReverseLineFeed状态不正确，光标在可视区域上面");
                        }
                        else if (this.cursorRow > lastVisibleRow)
                        {
                            // 光标位置在可视区域的下面？？
                            logger.ErrorFormat("RI_ReverseLineFeed状态不正确，光标在可视区域下面");
                        }
                        else
                        {
                            // 光标位置在可视区域里面
                            logger.DebugFormat("RI_ReverseLineFeed，光标在可视区域里面，直接向上移动一行光标");
                            this.cursorRow--;
                            this.UpdateActiveLine(this.cursorRow);
                        }
                        break;
                    }

                case VTActions.DECSTBM_SetScrollingRegion:
                    {
                        logger.ErrorFormat("未实现SetScrollingRegion");
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

            this.RenderDocument(this.activeDocument);
            //this.activeDocument.Print();
        }

        private void VTChannel_StatusChanged(object client, VTChannelState state)
        {
            logger.InfoFormat("客户端状态发生改变, {0}", state);
        }

        #endregion
    }
}

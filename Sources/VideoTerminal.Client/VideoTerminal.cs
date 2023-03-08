using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using VideoTerminal.Options;
using XTerminal.Channels;
using XTerminal.Drawing;
using XTerminalParser;

namespace XTerminal
{
    /// <summary>
    /// 处理虚拟终端的所有逻辑
    /// </summary>
    public class VideoTerminal
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("VTApplication");

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
        /// 活动的文本行
        /// 也就是光标所在文本行
        /// </summary>
        private VTextLine activeLine;

        /// <summary>
        /// 第一行
        /// </summary>
        private VTextLine firstLine;

        /// <summary>
        /// 最后一行
        /// </summary>
        private VTextLine lastLine;

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

        private IDrawingCanvas alternateCanvas;
        private IDrawingCanvas mainCanvas;

        #endregion

        #region 属性

        /// <summary>
        /// 终端设备的一些接口
        /// </summary>
        public IVTController Controller { get; set; }

        /// <summary>
        /// 输入设备
        /// </summary>
        public IInputDevice InputDevice { get; private set; }

        /// <summary>
        /// 终端显示器接口
        /// </summary>
        public IDrawingCanvas DrawingCanvas { get; private set; }

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
            this.DrawingCanvas = this.Controller.CreatePresentationDevice();
            this.Controller.SwitchPresentaionDevice(null, this.DrawingCanvas);
            this.mainCanvas = this.DrawingCanvas;
            this.InputDevice = this.Controller.GetInputDevice();
            this.InputDevice.InputEvent += this.VideoTerminal_InputEvent;

            // 初始化终端解析器
            this.vtParser = new VTParser();
            this.vtParser.ActionEvent += VtParser_ActionEvent;
            this.vtParser.Initialize();

            this.blankCharacterMetrics = this.DrawingCanvas.MeasureText(" ", VTextStyle.Default);

            // 创建第一个TextBlock
            this.activeLine = this.CreateTextLine(0, 0);

            // 连接终端通道
            VTChannel vtChannel = VTChannelFactory.Create(options);
            vtChannel.StatusChanged += this.VTChannel_StatusChanged;
            vtChannel.DataReceived += this.VTChannel_DataReceived;
            vtChannel.Connect();
            this.vtChannel = vtChannel;
        }

        #endregion

        #region 实例方法

        private VTextLine CreateFirstLine()
        {
            VTextLine firstLine = new VTextLine()
            {
                Row = 0,
                OffsetX = 0,
                OffsetY = 0,
                CursorAtRightMargin = false,
                TerminalColumns = this.initialOptions.TerminalOption.Columns,
                DECPrivateAutoWrapMode = this.autoWrapMode,
                OwnerCanvas = this.DrawingCanvas
            };

            VTextLine previousLine;
            if (!this.textLines.TryGetValue(row - 1, out previousLine))
            {
                logger.ErrorFormat("CreateTextLine失败, 找不到上一行, previousRow = {0}", row - 1);
                return null;
            }

            return this.CreateTextLine(row, previousLine.Boundary.LeftBottom.Y);
        }

        private VTextLine CreateTextLine(int row, double offsetY)
        {
            VTextLine textLine = new VTextLine()
            {
                Row = row,
                OffsetX = 0,
                OffsetY = offsetY,
                CursorAtRightMargin = false,
                TerminalColumns = this.initialOptions.TerminalOption.Columns,
                DECPrivateAutoWrapMode = this.autoWrapMode,
                OwnerCanvas = this.DrawingCanvas,
            };

            this.textLines[row] = textLine;

            return textLine;
        }

        /// <summary>
        /// 重新测量Terminal所需要的大小
        /// 如果大小改变了，那么调整布局大小
        /// </summary>
        private void InvalidateMeasure()
        {
            if (this.activeLine == null)
            {
                logger.ErrorFormat("InvalidateMeasure失败, activeLine不存在");
            }

            double width = Math.Max(this.activeLine.Boundary.RightBottom.X, this.fullWidth);
            double height = Math.Max(this.activeLine.Boundary.RightBottom.Y, this.fullHeight);

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

                this.DrawingCanvas.Resize(width, height);
                this.DrawingCanvas.ScrollToEnd(ScrollOrientation.Bottom);
            }
        }

        /// <summary>
        /// 执行删除行操作
        /// </summary>
        /// <param name="parameter"></param>
        private void PerformEraseLine(int parameter)
        {
            switch (parameter)
            {
                case 0:
                    {
                        // 删除从当前光标处到该行结尾的所有字符

                        // 获取光标所在行
                        VTextLine cursorLine;
                        if (!this.textLines.TryGetValue(this.cursorRow, out cursorLine))
                        {
                            logger.ErrorFormat("获取光标所在VTextLine失败, cursorRow = {0}", this.cursorRow);
                            return;
                        }

                        // 删除
                        cursorLine.DeleteText(this.cursorCol);

                        // 刷新UI
                        this.DrawLine(cursorLine);
                        break;
                    }

                case 1:
                    {
                        // 删除从行首到当前光标处的内容
                        VTextLine cursorLine;
                        if (!this.textLines.TryGetValue(this.cursorRow, out cursorLine))
                        {
                            logger.ErrorFormat("获取光标所在VTextLine失败, cursorRow = {0}", this.cursorRow);
                            return;
                        }

                        cursorLine.DeleteText(0, this.cursorCol);

                        this.DrawLine(cursorLine);
                        break;
                    }

                case 2:
                    {
                        // 删除光标所在整行
                        VTextLine cursorLine;
                        if (!this.textLines.TryGetValue(this.cursorRow, out cursorLine))
                        {
                            logger.ErrorFormat("获取光标所在VTextLine失败, cursorRow = {0}", this.cursorRow);
                            return;
                        }

                        cursorLine.DeleteAll();

                        this.DrawLine(cursorLine);
                        break;
                    }

                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// 执行删除显示操作
        /// </summary>
        /// <param name="parameter"></param>
        private void PerformEraseDisplay(int parameter)
        {
            switch (parameter)
            {
                case 0:
                    {
                        // 从当前光标处直到屏幕最后一行全部都删除（包括当前光标处）

                        // 获取光标所在行
                        VTextLine cursorLine;
                        if (!this.textLines.TryGetValue(this.cursorRow, out cursorLine))
                        {
                            logger.ErrorFormat("PerformEraseDisplay失败, 获取光标所在行失败, cursorRow = {0}", this.cursorRow);
                            return;
                        }

                        // 先删第一行，从当前光标位置开始删除
                        cursorLine.DeleteText(this.cursorCol);

                        // 刷新UI
                        this.DrawLine(cursorLine);

                        break;
                    }

                case 1:
                    {
                        // 从屏幕的开始处删除到当前光标处

                        break;
                    }

                case 2:
                    {
                        // 删除显示的全部字符，所有行都被删除，changed to single-width，光标不移动

                        break;
                    }

                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// 从当前光标处开始删除字符
        /// </summary>
        /// <param name="n">要删除的字符数</param>
        private void PerformDeleteCharacters(int n)
        {
            VTextLine textLine;
            if (!this.textLines.TryGetValue(this.cursorRow, out textLine))
            {
                logger.ErrorFormat("PerformDeleteCharacters失败，没找到当前光标对应的VTextLine, cursorRow = {0}", cursorRow);
                return;
            }

            textLine.DeleteText(this.cursorCol, n);
        }

        /// <summary>
        /// 在当前光标处插入n个字符，要插入的字符由ch指定
        /// </summary>
        /// <param name="n"></param>
        /// <param name="ch"></param>
        private void PerformInsertCharacters(int n, char ch)
        {
            VTextLine textLine;
            if (!this.textLines.TryGetValue(this.cursorRow, out textLine))
            {
                logger.ErrorFormat("PerformInsertCharacters失败，没找到当前光标对应的VTextLine, cursorRow = {0}", cursorRow);
                return;
            }

            //textLine.InsertCharacter(ch, this.cursorCol, n);
        }

        private void DrawLine(VTextLine textLine)
        {
            this.uiSyncContext.Send((v) =>
            {
                this.DrawingCanvas.DrawLine(textLine);
                this.InvalidateMeasure();
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

        private void VtParser_ActionEvent(VTActions action, params object[] param)
        {
            switch (action)
            {
                case VTActions.Print:
                    {
                        char ch = (char)param[0];

                        switch (ch)
                        {
                            // 遇到下面的字符，渲染完后就重新创建TextBlock
                            //case ':':
                            //case '/':
                            //case '\\':
                            case ' ':
                                {
                                    //Console.WriteLine("渲染断字符, {0}", ch);
                                    this.activeLine.PrintCharacter(ch, this.cursorCol);
                                    this.DrawLine(this.activeLine);
                                    this.cursorCol++;
                                    break;
                                }

                            default:
                                {
                                    this.activeLine.PrintCharacter(ch, this.cursorCol);
                                    this.DrawLine(this.activeLine);
                                    this.cursorCol++;
                                    break;
                                }
                        }

                        break;
                    }

                case VTActions.CarriageReturn:
                    {
                        // CR
                        // 把光标移动到行开头
                        logger.DebugFormat("CR");
                        this.cursorCol = 0;
                        break;
                    }

                case VTActions.LineFeed:
                    {
                        // LF
                        logger.DebugFormat("LF");
                        this.cursorRow++;
                        this.activeLine = this.CreateFirstLine(this.cursorRow);
                        break;
                    }

                case VTActions.EraseLine:
                    {
                        logger.WarnFormat("EraseLine");
                        int parameter = Convert.ToInt32(param[0]);
                        this.PerformEraseLine(parameter);
                        break;
                    }

                case VTActions.CursorForword:
                    {
                        int n = Convert.ToInt32(param[0]);
                        logger.WarnFormat("CursorForword, {0}", n);
                        this.cursorCol += n;
                        break;
                    }

                case VTActions.CursorBackward:
                    {
                        logger.WarnFormat("CursorBackward");
                        this.cursorCol--;
                        break;
                    }

                case VTActions.CursorUp:
                    {
                        int n = Convert.ToInt32(param[0]);
                        logger.WarnFormat("CursorUp, {0}", n);
                        this.cursorRow -= n;
                        break;
                    }

                case VTActions.CursorDown:
                    {
                        int n = Convert.ToInt32(param[0]);
                        logger.WarnFormat("CursorDown, {0}", n);
                        this.cursorRow += n;
                        break;
                    }

                case VTActions.PlayBell:
                case VTActions.Bold:
                case VTActions.Foreground:
                case VTActions.Background:
                case VTActions.DefaultAttributes:
                case VTActions.DefaultBackground:
                case VTActions.DefaultForeground:
                    break;

                case VTActions.SetVTMode:
                    {
                        VTMode vtMode = (VTMode)param[0];
                        logger.WarnFormat("SetMode, {0}", vtMode);
                        this.Keyboard.SetAnsiMode(vtMode == VTMode.AnsiMode);
                        break;
                    }

                case VTActions.SetCursorKeyMode:
                    {
                        VTCursorKeyMode cursorKeyMode = (VTCursorKeyMode)param[0];
                        logger.WarnFormat("SetCursorKeyMode, {0}", cursorKeyMode);
                        this.Keyboard.SetCursorKeyMode(cursorKeyMode == VTCursorKeyMode.ApplicationMode);
                        break;
                    }

                case VTActions.SetKeypadMode:
                    {
                        VTKeypadMode keypadMode = (VTKeypadMode)param[0];
                        logger.WarnFormat("SetKeypadMode, {0}", keypadMode);
                        this.Keyboard.SetKeypadMode(keypadMode == VTKeypadMode.ApplicationMode);
                        break;
                    }

                case VTActions.DeleteCharacters:
                    {
                        int count = Convert.ToInt32(param[0]);
                        logger.WarnFormat("DeleteCharacters, {0}", count);
                        this.PerformDeleteCharacters(count);
                        break;
                    }

                case VTActions.InsertCharacters:
                    {
                        // 目前没发现这个操作对终端显示有什么影响，所以暂时不实现
                        int count = Convert.ToInt32(param[0]);
                        logger.ErrorFormat("未实现InsertCharacters, {0}, cursorPos = {1}", count, this.cursorCol);
                        //this.PerformInsertCharacters(count, ' ');
                        break;
                    }

                case VTActions.SetDECAWM:
                    {
                        this.autoWrapMode = (bool)param[0];
                        break;
                    }

                case VTActions.UseAlternateScreenBuffer:
                    {
                        this.uiSyncContext.Send((state) =>
                        {
                            IDrawingCanvas alternateCanvas = this.Controller.CreatePresentationDevice();
                            this.Controller.SwitchPresentaionDevice(this.DrawingCanvas, alternateCanvas);
                            this.alternateCanvas = alternateCanvas;
                            this.DrawingCanvas = alternateCanvas;
                        }, null);
                        break;
                    }

                case VTActions.UseMainScreenBuffer:
                    {
                        if (this.alternateCanvas == null)
                        {
                            logger.ErrorFormat("UseMainScreenBuffer, alternateCanvas不存在");
                            return;
                        }

                        this.uiSyncContext.Send((state) =>
                        {
                            this.Controller.SwitchPresentaionDevice(this.alternateCanvas, this.mainCanvas);
                            this.Controller.ReleasePresentationDevice(this.alternateCanvas);
                            this.DrawingCanvas = this.mainCanvas;
                            this.alternateCanvas = null;
                        }, null);
                        break;
                    }

                case VTActions.ED_EraseDisplay:
                    {
                        int parameter = Convert.ToInt32(param[0]);
                        this.PerformEraseDisplay(parameter);
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

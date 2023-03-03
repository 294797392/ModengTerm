using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

        private ChannelAuthorition authorition;

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
        /// 当前正在渲染的文本块
        /// 当前正在活动的文本块（Active Data）
        /// </summary>
        private VTextBlock activeTextBlock;

        // 当前渲染的文本的左上角X位置
        private double textOffsetX;

        // 空白字符的宽度
        private double whitespaceWidth;

        /// <summary>
        /// 一个字符的高度
        /// </summary>
        private double characterHeight;

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

        public void Initialize()
        {
            this.uiSyncContext = SynchronizationContext.Current;

            // 0:和字符方向相同（向右）
            // 1:和字符方向相反（向左）
            this.implicitMovementDirection = 0;

            this.textLines = new Dictionary<int, VTextLine>();
            this.TextOptions = new VTextOptions();

            this.Keyboard = new VTKeyboard();
            this.Keyboard.SetAnsiMode(true);
            this.Keyboard.SetKeypadMode(false);

            // 初始化视频终端
            this.DrawingCanvas = this.Controller.CreatePresentationDevice();
            this.Controller.SwitchPresentaionDevice(null, this.DrawingCanvas);
            this.InputDevice = this.Controller.GetInputDevice();
            this.InputDevice.InputEvent += this.VideoTerminal_InputEvent;

            // 初始化终端解析器
            this.vtParser = new VTParser();
            this.vtParser.ActionEvent += VtParser_ActionEvent;
            this.vtParser.Initialize();

            VTextMetrics spaceTextMetrics = this.DrawingCanvas.MeasureText(" ", VTextStyle.Default);
            this.whitespaceWidth = spaceTextMetrics.WidthIncludingWhitespace;
            this.characterHeight = spaceTextMetrics.Height;

            // 创建第一个TextBlock
            this.activeTextBlock = this.CreateTextBlock(0, 0, 0);
        }

        public void RunSSHClient(SSHChannelAuthorition authorition)
        {
            this.authorition = authorition;
            this.vtChannel = VTChannelFactory.CreateSSHClient(authorition.ServerAddress, authorition.ServerPort, authorition.UserName, authorition.Password);
            this.vtChannel.StatusChanged += this.VTChannel_StatusChanged;
            this.vtChannel.DataReceived += this.VTChannel_DataReceived;
            this.vtChannel.Connect();
        }

        /// <summary>
        /// 关闭连接并释放资源
        /// </summary>
        public void Exit()
        {
            //this.terminal.InputEvent -= this.VideoTerminal_InputEvent;

            //this.vtParser.ActionEvent -= this.VtParser_ActionEvent;

            //this.vtChannel.StatusChanged -= this.VTChannel_StatusChanged;
            //this.vtChannel.DataReceived -= this.VTChannel_DataReceived;
            //this.vtChannel.Disconnect();
        }

        #endregion

        #region 实例方法

        private VTextBlock CreateTextBlock(int row, int column, double offsetX)
        {
            VTextBlock textBlock = new VTextBlock()
            {
                ID = Guid.NewGuid().ToString(),
                Column = column,
                Row = row,
                OffsetX = offsetX,
                OffsetY = row * this.characterHeight,
                Style = VTextStyle.Default
            };

            // 找到文本块所属的行，并把文本块加到行里
            VTextLine ownerLine;
            if (!this.textLines.TryGetValue(row, out ownerLine))
            {
                ownerLine = new VTextLine()
                {
                    Row = row,
                    OffsetY = this.characterHeight * row,
                    OwnerCanvas = this.DrawingCanvas
                };
                this.textLines[row] = ownerLine;
            }

            ownerLine.AddTextBlock(textBlock);

            return textBlock;
        }

        /// <summary>
        /// 重新测量Terminal所需要的大小
        /// 如果大小改变了，那么调整布局大小
        /// </summary>
        private void InvalidateMeasure()
        {
            if (this.activeTextBlock == null)
            {
                return;
            }

            double width = Math.Max(this.activeTextBlock.Boundary.RightBottom.X, this.fullWidth);
            double height = Math.Max(this.activeTextBlock.Boundary.RightBottom.Y, this.fullHeight);

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

                this.uiSyncContext.Send((v) =>
                {
                    this.DrawingCanvas.Resize(width, height);
                    this.DrawingCanvas.ScrollToEnd(ScrollOrientation.Bottom);
                }, null);
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

                        // 获取光标所在文本
                        VTextLine cursorLine;
                        if (!this.textLines.TryGetValue(this.cursorRow, out cursorLine))
                        {
                            logger.ErrorFormat("获取光标所在VTextLine失败, cursorRow = {0}", this.cursorRow);
                            return;
                        }

                        // 删除
                        cursorLine.DeleteText(this.cursorCol);

                        // 刷新UI
                        this.uiSyncContext.Send((v) =>
                        {
                            this.DrawingCanvas.DrawLine(cursorLine);
                        }, null);
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

                        this.uiSyncContext.Send((v) => 
                        {
                            this.DrawingCanvas.DrawLine(cursorLine);
                        }, null);

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

                        this.uiSyncContext.Send((v) =>
                        {
                            this.DrawingCanvas.DrawLine(cursorLine);
                        }, null);
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
                        char ch = param[0].ToString()[0];

                        switch (ch)
                        {
                            // 遇到下面的字符，渲染完后就重新创建TextBlock
                            //case ':':
                            //case '/':
                            //case '\\':
                            case ' ':
                                {
                                    //Console.WriteLine("渲染断字符, {0}", ch);
                                    this.activeTextBlock = this.CreateTextBlock(this.cursorRow, this.cursorCol, this.textOffsetX);
                                    this.activeTextBlock.OwnerLine.AddText(ch.ToString());
                                    this.uiSyncContext.Send((v) =>
                                    {
                                        this.DrawingCanvas.DrawLine(this.activeTextBlock.OwnerLine);
                                    }, null);
                                    this.InvalidateMeasure();
                                    // 下次新创建的TextBlock的X偏移量
                                    this.textOffsetX = this.activeTextBlock.Boundary.RightTop.X;
                                    this.cursorCol++;
                                    this.activeTextBlock = null;
                                    break;
                                }

                            default:
                                {
                                    if (this.activeTextBlock == null)
                                    {
                                        this.activeTextBlock = this.CreateTextBlock(this.cursorRow, this.cursorCol, this.textOffsetX);
                                    }
                                    this.activeTextBlock.OwnerLine.AddText(ch.ToString());
                                    this.uiSyncContext.Send((v) =>
                                    {
                                        this.DrawingCanvas.DrawLine(this.activeTextBlock.OwnerLine);
                                    }, null);
                                    this.InvalidateMeasure();
                                    // 下次新创建的TextBlock的X偏移量
                                    this.textOffsetX = this.activeTextBlock.Boundary.RightTop.X;
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
                        this.textOffsetX = 0;
                        break;
                    }

                case VTActions.LineFeed:
                    {
                        // LF
                        logger.DebugFormat("LF");
                        //this.textOffsetX = 0;
                        //this.cursorCol = 0;
                        this.cursorRow++;
                        this.activeTextBlock = this.CreateTextBlock(this.cursorRow, 0, 0);
                        break;
                    }

                case VTActions.EraseLine:
                    {
                        logger.DebugFormat("EraseLine");
                        this.PerformEraseLine(Convert.ToInt32(param[0]));
                        break;
                    }

                case VTActions.CursorBackward:
                    {
                        logger.DebugFormat("CursorBackward");
                        this.cursorCol--;
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
                        logger.DebugFormat("SetMode");
                        VTMode vtMode = (VTMode)param[0];
                        this.Keyboard.SetAnsiMode(vtMode == VTMode.AnsiMode);
                        break;
                    }

                case VTActions.SetCursorKeyMode:
                    {
                        logger.DebugFormat("SetCursorKeyMode");
                        VTCursorKeyMode cursorKeyMode = (VTCursorKeyMode)param[0];
                        this.Keyboard.SetCursorKeyMode(cursorKeyMode == VTCursorKeyMode.ApplicationMode);
                        break;
                    }

                case VTActions.SetKeypadMode:
                    {
                        logger.DebugFormat("SetKeypadMode");
                        VTKeypadMode keypadMode = (VTKeypadMode)param[0];
                        this.Keyboard.SetKeypadMode(keypadMode == VTKeypadMode.ApplicationMode);
                        break;
                    }

                case VTActions.DeleteCharacters:
                    {
                        logger.DebugFormat("DeleteCharacters");
                        this.PerformDeleteCharacters(Convert.ToInt32(param[0]));
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

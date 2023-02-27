using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XTerminalBase.Channels;
using XTerminalDevice.Interface;
using XTerminalParser;

namespace XTerminalDevice
{
    /// <summary>
    /// 控制虚拟终端设备
    /// </summary>
    public class VTApplication
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

        private VTScreen vtScreen;

        // 所有的文本列表
        private List<VTextBlock> textBlocks;

        /// <summary>
        /// 当前正在渲染的文本块
        /// 当前正在活动的文本块（Active Data）
        /// </summary>
        private VTextBlock activeTextBlock;

        // 当前渲染的文本的左上角X位置
        private double textOffsetX;

        // 当前渲染的文本的左上角Y位置
        private double textOffsetY;

        // 当前行的高度
        private double textLineHeight;

        // 空白字符的宽度
        private double whitespaceWidth;

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

        #endregion

        #region 属性

        /// <summary>
        /// 终端设备的一些接口
        /// </summary>
        public IVTDevice VTDevice { get; set; }

        /// <summary>
        /// 输入设备
        /// </summary>
        public IInputDevice InputDevice { get; private set; }

        /// <summary>
        /// 显示字符的设备
        /// </summary>
        public IPresentationDevice PresentationDevice { get; private set; }

        /// <summary>
        /// 根据当前电脑键盘的按键状态，转换成对应的终端数据流
        /// </summary>
        public VTKeyboard Keyboard { get; private set; }

        public VTextOptions TextOptions { get; private set; }

        #endregion

        #region 构造方法

        public VTApplication()
        {
        }

        #endregion

        #region 公开接口

        public void Initialize()
        {
            // 0:和字符方向相同（向右）
            // 1:和字符方向相反（向左）
            this.implicitMovementDirection = 0;

            this.textBlocks = new List<VTextBlock>();
            this.TextOptions = new VTextOptions();

            this.Keyboard = new VTKeyboard();
            this.Keyboard.SetAnsiMode(true);
            this.Keyboard.SetKeypadMode(false);

            // 初始化视频终端
            this.PresentationDevice = this.VTDevice.CreatePresentationDevice();
            this.VTDevice.SwitchPresentaionDevice(null, this.PresentationDevice);
            this.InputDevice = this.VTDevice.GetInputDevice();
            this.InputDevice.InputEvent += this.VideoTerminal_InputEvent;

            // 初始化终端解析器
            this.vtParser = new VTParser();
            this.vtParser.ActionEvent += VtParser_ActionEvent;
            this.vtParser.Initialize();

            VTextBlock spaceText = new VTextBlock()
            {
                Size = this.TextOptions.FontSize,
                Foreground = VTForeground.DarkBlack,
                Text = " ",
            };
            this.whitespaceWidth = this.PresentationDevice.MeasureText(spaceText).WidthIncludingWhitespace;
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

        /// <summary>
        /// 给定一个位置，获取对应位置的文本块
        /// 如果没有，则为null
        /// </summary>
        /// <returns></returns>
        private VTextBlock GetTextBlockAt(int row, int col)
        {
            VTextBlock textBlock = this.textBlocks.FirstOrDefault(v => v.Column <= col && v.Column + v.Column >= col);
            return textBlock;
        }

        /// <summary>
        /// 打印TextBlock文本
        /// </summary>
        /// <param name="textBlock"></param>
        private void DrawTextBlock(VTextBlock textBlock)
        {
            if (textBlock == null)
            {
                return;
            }

            // 遇到空格就渲染当前的文本
            this.PresentationDevice.DrawText(textBlock);
            this.textLineHeight = Math.Max(this.textLineHeight, textBlock.Height);
        }

        /// <summary>
        /// 如果activeTextBlock为空，那么新建activeTextBlock
        /// </summary>
        private void EnsureActiveTextBlock()
        {
            if (this.activeTextBlock == null)
            {
                this.activeTextBlock = new VTextBlock()
                {
                    Index = this.textBlocks.Count,
                    Foreground = this.TextOptions.Foreground,
                    Size = this.TextOptions.FontSize,
                    X = this.textOffsetX,
                    Y = this.textOffsetY,
                    Row = this.cursorRow,
                    Column = this.cursorCol,
                    Text = string.Empty
                };
                this.textBlocks.Add(activeTextBlock);
            }
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
                this.PresentationDevice.Resize(width, height);
                this.PresentationDevice.ScrollToEnd(ScrollOrientation.Bottom);
            }
        }

        /// <summary>
        /// 执行Backspace操作
        /// </summary>
        private void PerformBackspace(VTextBlock textBlock)
        {
            if (this.implicitMovementDirection == 0)
            {
                // implicit movement的方向是从左到右（和字符方向一致）
                textBlock.DeleteCharacter(DeleteCharacterFrom.BackToFront, 1);
            }
            else if (this.implicitMovementDirection == 1)
            {
                // implicit movement的方向是从右到左（和字符方向相反）
                throw new NotImplementedException();
            }
        }

        private void PerformEraseLine(int parameter)
        {
            switch (parameter)
            {
                case 0:
                    {
                        // 当前光标处到结尾
                        // 获取光标所在文本
                        VTextBlock textBlock = this.GetTextBlockAt(this.cursorRow, this.cursorCol);
                        int startIndex = this.cursorCol - textBlock.Column;
                        break;
                    }

                case 1:
                    {
                        // 删除从行首到当前光标处的内容
                        break;
                    }

                case 2:
                    {
                        // 删除整行
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
                        char ch = param[0].ToString()[0];

                        switch (ch)
                        {
                            case ' ':
                                {
                                    //Console.WriteLine("SSH -> PC, 空格");
                                    this.DrawTextBlock(this.activeTextBlock);
                                    this.InvalidateMeasure();
                                    this.textOffsetX += this.whitespaceWidth;
                                    this.cursorCol++;
                                    this.activeTextBlock = null;
                                    break;
                                }

                            default:
                                {
                                    this.EnsureActiveTextBlock();
                                    this.activeTextBlock.InsertCharacter(ch);
                                    this.DrawTextBlock(this.activeTextBlock);
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
                        break;
                    }

                case VTActions.LineFeed:
                    {
                        // LF
                        this.DrawTextBlock(this.activeTextBlock);
                        this.InvalidateMeasure();
                        this.textOffsetY += this.textLineHeight;
                        this.textOffsetX = 0;
                        this.cursorCol = 0;
                        this.cursorRow++;
                        this.activeTextBlock = null;
                        break;
                    }


                case VTActions.EraseLine:
                    {
                        this.PerformEraseLine(Convert.ToInt32(param[0]));
                        break;
                    }

                case VTActions.CursorBackward:
                    {
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

                default:
                    {
                        logger.WarnFormat("未执行的VTAction, {0}", action);
                        break;
                    }
            }
        }

        private void VTChannel_DataReceived(VTChannel client, byte[] bytes)
        {
            //logger.InfoFormat("Received");
            this.vtParser.ProcessCharacters(bytes);
        }

        private void VTChannel_StatusChanged(object client, VTChannelState state)
        {
            logger.InfoFormat("客户端状态发生改变, {0}", state);
        }

        #endregion
    }
}

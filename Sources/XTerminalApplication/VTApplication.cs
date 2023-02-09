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

        /// <summary>
        /// 当前正在渲染的文本块
        /// </summary>
        private VTextBlock textBlock;

        // 当前渲染的文本的左上角X位置
        private double textOffsetX;

        // 当前渲染的文本的左上角Y位置
        private double textOffsetY;

        // 所有的文本列表
        private List<VTextBlock> textBlocks;

        // 当前行的高度
        private double textLineHeight;

        // 空白字符的宽度
        private double whitespaceWidth;

        /// <summary>
        /// Terminal区域的总长宽
        /// </summary>
        private double fullWidth;
        private double fullHeight;

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
        /// 打印TextBlock文本
        /// </summary>
        /// <param name="textBlock"></param>
        private void FlushText(VTextBlock textBlock)
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
        /// 重新测量Terminal所需要的大小
        /// 如果大小改变了，那么调整布局大小
        /// </summary>
        private void InvalidateMeasure()
        {
            if (this.textBlock == null) 
            {
                return;
            }

            double width = Math.Max(this.textBlock.Boundary.RightBottom.X, this.fullWidth);
            double height = Math.Max(this.textBlock.Boundary.RightBottom.Y, this.fullHeight);

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
                // 这里输入的都是键盘按键
                byte[] bytes = this.Keyboard.TranslateInput(evt);
                if (bytes == null)
                {
                    return;
                }

                this.vtChannel.Write(bytes);
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
                                    this.FlushText(this.textBlock);
                                    this.InvalidateMeasure();
                                    this.textOffsetX += this.whitespaceWidth;
                                    this.textBlock = null;
                                    break;
                                }

                            default:
                                {
                                    if (this.textBlock == null)
                                    {
                                        this.textBlock = new VTextBlock()
                                        {
                                            Index = this.textBlocks.Count,
                                            Foreground = this.TextOptions.Foreground,
                                            Size = this.TextOptions.FontSize,
                                            X = this.textOffsetX,
                                            Y = this.textOffsetY
                                        };
                                        this.textBlocks.Add(textBlock);
                                    }
                                    this.textBlock.AppendText(ch);

                                    this.FlushText(this.textBlock);
                                    this.InvalidateMeasure();
                                    // 下次新创建的TextBlock的X偏移量
                                    this.textOffsetX = this.textBlock.Boundary.RightTop.X;
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
                        this.FlushText(this.textBlock);
                        this.InvalidateMeasure();
                        this.textOffsetY += this.textLineHeight;
                        this.textOffsetX = 0;
                        this.textBlock = null;
                        break;
                    }

                default:
                    {
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

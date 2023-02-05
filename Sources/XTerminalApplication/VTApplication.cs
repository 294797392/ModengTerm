using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XTerminalBase.Channels;
using XTerminalParser;

namespace XTerminalController
{
    /// <summary>
    /// 第三方用户通过这个类来启动终端
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

        // 终端控制器
        private IVideoTerminal terminal;

        // 当前正在渲染的文本块
        private VTextBlock textBlock;

        // 当前渲染的文本的X位置
        private double textOffsetX;

        // 当前渲染的文本的Y位置
        private double textOffsetY;

        // 所有的文本列表
        private List<VTextBlock> textBlocks;

        // 当前行的高度
        private double textLineHeight;

        // 空白字符的宽度
        private double whitespaceWidth;

        #endregion

        #region 属性

        /// <summary>
        /// 根据当前电脑键盘的按键状态，转换成对应的终端数据流
        /// </summary>
        public VTKeyboard Keyboard { get; private set; }

        public VTextOptions TextOptions { get; private set; }

        #endregion

        #region 构造方法

        private VTApplication(IVideoTerminal terminal)
        {
            this.textBlocks = new List<VTextBlock>();
            this.TextOptions = new VTextOptions();

            this.Keyboard = new VTKeyboard();
            this.Keyboard.SetAnsiMode(true);
            this.Keyboard.SetKeypadMode(false);

            // 初始化视频终端
            this.terminal = terminal;
            this.terminal.InputEvent += this.VideoTerminal_InputEvent;

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
            this.whitespaceWidth = this.terminal.MeasureText(spaceText).WidthIncludingWhitespace;
        }

        #endregion

        #region 实例方法

        private void RunSSHClient(SSHChannelAuthorition authorition)
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
            this.terminal.InputEvent -= this.VideoTerminal_InputEvent;

            this.vtParser.ActionEvent -= this.VtParser_ActionEvent;

            this.vtChannel.StatusChanged -= this.VTChannel_StatusChanged;
            this.vtChannel.DataReceived -= this.VTChannel_DataReceived;
            this.vtChannel.Disconnect();
        }

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
            this.terminal.DrawText(textBlock);
            this.textLineHeight = Math.Max(this.textLineHeight, textBlock.Height);
        }

        #endregion

        #region 事件处理器

        /// <summary>
        /// 当用户按下按键的时候触发
        /// </summary>
        /// <param name="terminal"></param>
        private void VideoTerminal_InputEvent(IVideoTerminal terminal, VTInputEvent evt)
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
                                    this.textBlock = null;
                                    this.textOffsetX += this.whitespaceWidth;
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

                                    // 渲染之前字符的宽度
                                    double width1 = this.textBlock.Metrics.Width;
                                    this.FlushText(this.textBlock);
                                    // 渲染之后的字符宽度
                                    double width2 = this.textBlock.Metrics.Width;
                                    // 下次新创建的TextBlock的X偏移量
                                    this.textOffsetX += (width2 - width1);
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
                        this.textBlock = null;
                        this.textOffsetY += this.textLineHeight;
                        this.textOffsetX = 0;
                        break;
                    }

                default:
                    {
                        break;
                    }
            }

            //this.videoTerminal.PerformAction(action, param);
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

        #region 公开接口

        /// <summary>
        /// 创建一个VTApplication的实例并开始运行
        /// </summary>
        /// <param name="authorition"></param>
        /// <param name="terminal"></param>
        /// <returns></returns>
        public static VTApplication Run(SSHChannelAuthorition authorition, IVideoTerminal terminal)
        {
            VTApplication vtApp = new VTApplication(terminal);
            vtApp.RunSSHClient(authorition);
            return vtApp;
        }

        /// <summary>
        /// 退出一个VTApplication并释放资源
        /// </summary>
        /// <param name="vtApp"></param>
        public static void Exit(VTApplication vtApp)
        {
            vtApp.Exit();
        }

        #endregion
    }
}

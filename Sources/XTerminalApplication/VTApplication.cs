using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XTerminalClient;
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

        private ClientAuthorition authorition;
        private ClientBase client;
        private IVideoTerminal terminal;
        private VTParser vtParser;

        private VTextBlock textBlock;
        private double textOffsetX;
        private double textOffsetY;
        private List<VTextBlock> textBlocks;

        #endregion

        #region 构造方法

        private VTApplication()
        {
            this.textBlocks = new List<VTextBlock>();
        }

        #endregion

        #region 实例方法

        private void RunSSHClient(SSHClientAuthorition authorition, IVideoTerminal terminal)
        {
            this.authorition = authorition;
            this.terminal = terminal;
            this.terminal.InputEvent += this.Terminal_InputEvent;

            this.vtParser = new VTParser();
            this.vtParser.ActionEvent += VtParser_ActionEvent;
            this.vtParser.Initialize();

            this.client = ClientFactory.CreateSSHClient(authorition.ServerAddress, authorition.ServerPort, authorition.UserName, authorition.Password);
            this.client.StatusChanged += this.Client_StatusChanged;
            this.client.DataReceived += this.Client_DataReceived;
            this.client.Connect();
        }

        /// <summary>
        /// 关闭连接并释放资源
        /// </summary>
        public void Exit()
        {
            this.terminal.InputEvent -= this.Terminal_InputEvent;

            this.vtParser.ActionEvent -= this.VtParser_ActionEvent;

            this.client.StatusChanged -= this.Client_StatusChanged;
            this.client.DataReceived -= this.Client_DataReceived;
            this.client.Disconnect();
        }

        #endregion

        #region 事件处理器

        /// <summary>
        /// 当用户按下按键的时候触发
        /// </summary>
        /// <param name="terminal"></param>
        /// <param name="input">输入数据</param>
        private void Terminal_InputEvent(IVideoTerminal terminal, VTInputEventArgs input)
        {
            // todo:translate and send to remote host
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
                            case ' ':
                                {
                                    // 遇到空格就创建一个新的文本块
                                    if (this.textBlock != null)
                                    {
                                        this.terminal.DrawText(this.textBlock);
                                        this.textBlocks.Add(this.textBlock);

                                        this.textOffsetX = this.textBlock.X;
                                        this.textOffsetY = this.textBlock.Y;

                                        this.textBlock = null;
                                    }
                                    else
                                    {

                                    }
                                    break;
                                }

                            default:
                                {
                                    if (this.textBlock == null)
                                    {
                                        this.textBlock = new VTextBlock();
                                    }
                                    this.textBlock.AppendText(ch);
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
                        break;
                    }

                default:
                    {
                        break;
                    }
            }

            //this.videoTerminal.PerformAction(action, param);
        }

        private void Client_DataReceived(ClientBase client, byte[] bytes)
        {
            logger.InfoFormat("Received");
            this.vtParser.ProcessCharacters(bytes);
        }

        private void Client_StatusChanged(object client, ClientState state)
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
        public static VTApplication Run(SSHClientAuthorition authorition, IVideoTerminal terminal)
        {
            VTApplication vtApp = new VTApplication();
            vtApp.RunSSHClient(authorition, terminal);
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

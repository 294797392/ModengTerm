using ICare.Utility.Misc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Threading;
using System.Linq;
using GardeniaTerminalCore;

namespace GardeniaTerminalCore
{
    internal class VideoTerminalImpl : VideoTerminal
    {
        internal VideoTerminalImpl()
        { }
    }

    /// <summary>
    /// 参考Control Functions for Coded Character Sets Ecma-048.pdf标准
    /// 这是一个解释ASCII转义字符所表示的意义的标准
    /// 这是一个ECMA定义的标准，所有的终端都会实现这个标准
    /// 位置：
    ///     Dependencies/Control Functions for Coded Character Sets Ecma-048.pdf
    ///     
    /// 控制序列：
    ///     对于7位ascii编码系统：
    ///         由两个字符开头。第一个字符是01/11，第二个是Fe。Fe可以看作成命令类型，Fe的范围在04/00（64） - 05/15（95）之间
    ///     对于8位ascii编码系统：
    ///         由一个字符开头。范围在08/00 - 09/15之间
    /// 
    /// 完整的控制序列格式：
    ///     对于7位编码的ascii码：ControlFunction Fe [ [Fe参数] [Fe结束符] ]
    ///     对于8位编码的ascii码：ControlFunction [ [参数] [结束符] ]，8位ascii编码系统中，Fe就是ControlFunction
    /// 
    /// 一般情况下，使用7位ascii编码的主机返回的ControlFunctions和Fe百分之九十都是ESC和CSI。
    /// 
    /// 一些Fe的功能说明：
    ///     CSI：
    ///         名称：控制序列
    ///         格式：CSI P..P I..I F
    ///         参数说明：
    ///             CSI：控制序列字符，在7位编码系统中，由01/11 05/11两个字符组成；在8位编码系统中，由09/11单个字符组成
    ///             P..P：参数字符串（ParameterBytes），由03/00（48） - 03/15（63）之间的字符组成
    ///             I..I：中间字符串（IntermediateBytes），由02/00（32） - 02/15（47）之间的字符组成，后面会跟一个字符串终结字符（F）
    ///             F：结束字符（FinalByte），由04/00（64） - 07/14（126）之间的某个字符表示。07/00 - 07/14之间的字符也是结束符，但是这是留给厂商做实验使用的。注意，带有中间字符串和不带有中间字符串的结束符的含义不一样
    ///             
    /// 
    /// UTF8：用UTF-8就有复杂点.因为此时程序是把一个字节一个字节的来读取,然后再根据字节中开头的bit标志来识别是该把1个还是两个或三个字节做为一个单元来处理.
    /// UTF16：UTF-16就把两个字节当成一个单元来解析.这个很简单.
    /// UTF32：UTF-32就把四个字节当成一个单元来解析.这个很简单.
    /// </summary>
    public abstract class VideoTerminal
    {
        #region 事件

        //public event Action<object, VTAction, ParseState> Action;

        #endregion

        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("VideoTerminal");

        #endregion

        #region 实例变量

        private ParseState psrState;
        private VTPresentation presentation;
        private SynchronizationContext uiThreadContext;

        private SendOrPostCallback printTextAction;
        private SendOrPostCallback moveCursorAction;
        private SendOrPostCallback operationSystemCommandsAction;
        private SendOrPostCallback eraseLineAction;

        #endregion

        #region 属性

        public int DCMS { get; set; }

        /// <summary>
        /// 是否支持8位ascii字符
        /// </summary>
        public virtual bool Support8BitCharacters { get { return false; } }

        /// <summary>
        /// 是否解析C1字符集
        /// </summary>
        public virtual bool SupportC1Characters { get { return false; } }

        /// <summary>
        /// 设置终端字符编码方式
        /// </summary>
        public virtual Encoding CharacherEncoding { get; protected set; }

        public virtual SocketBase Socket { get; protected set; }

        public virtual VTKeyboard Keyboard { get; protected set; }

        public virtual VTScreen Screen { get; set; }

        public virtual VTWindow Window { get; set; }

        #endregion

        #region 公开接口

        public void Open()
        {
            this.DCMS = TerminalModes.DCSM_PRESENTATION;

            this.presentation = new VTPresentation();
            this.CharacherEncoding = DefaultValues.DefaultEncoding;
            this.Keyboard = VTKeyboard.Create();

            this.uiThreadContext = SynchronizationContext.Current;
            this.printTextAction = new SendOrPostCallback(this.InvokePrintText);
            this.moveCursorAction = new SendOrPostCallback(this.InvokeMoveCursor);
            this.operationSystemCommandsAction = new SendOrPostCallback(this.InvokeOperationSystemCommands);
            this.eraseLineAction = new SendOrPostCallback(this.InvokeEraseLine);

            this.psrState = new ParseState();
            this.psrState.StateTable = VTPrsTbl.ANSITable;
            this.psrState.NextState = VTPrsTbl.ANSITable[0];

            this.Socket = SocketBase.Create(SocketProtocols.SSH);
            this.Socket.Authorition = new SSHSocketAuthorition()
            {
                UserName = "zyf",
                Password = "18612538605",
                ServerAddress = "192.168.42.243",
                ServerPort = 22
            };
            this.Socket.StatusChanged += this.Socket_StatusChanged;
            this.Socket.Connect();
        }

        public void Close()
        {

        }

        public bool HandleInputWideChar(string wideChar, out byte[] data)
        {
            data = this.CharacherEncoding.GetBytes(wideChar);
            return false;
        }

        public bool HandleInputChar(KeyEventArgs key, out byte[] data)
        {
            data = this.Keyboard.ConvertKey(key);
            return data != null;
        }

        #endregion

        #region 实例方法

        private void InvokeOperationSystemCommands(object state)
        {
            ParseState psrState = this.psrState;
            string text = this.CharacherEncoding.GetString(psrState.ParameterBytes.ToArray());
            logger.DebugFormat("OSC:{0}", text);

            int action;
            if (!int.TryParse(text[0].ToString(), out action))
            {
                logger.WarnFormat("OSC参数不合法");
                return;
            }

            string[] pair = text.Split(';');
            if (pair.Length != 2)
            {
                logger.ErrorFormat("OSC参数不合法, action={0}", action);
                return;
            }

            string name = pair[1];
            switch (action)
            {
                /* Change Icon Name and Window Title to Pt */
                case 0:
                    {
                        this.Window.SetTitle(name);
                        this.Window.SetIconName(name);
                    }
                    break;

                /* Change Icon Name to Pt */
                case 1:
                    {
                        this.Window.SetIconName(name);
                    }
                    break;

                /* Change Window Title to Pt */
                case 2:
                    {
                        this.Window.SetTitle(name);
                    }
                    break;

                /* Set top-level Window Property */
                case 3:
                    {
                        logger.WarnFormat("未处理的OSC参数3");
                    }
                    break;

                /* TODO:... */
                case 4:
                    {
                        logger.WarnFormat("未处理的OSC参数4");
                    }
                    break;

                default:
                    {
                        logger.WarnFormat("未知的OSC参数,{0}", action);
                    }
                    break;
            }
        }

        private void InvokeEraseLine(object state)
        {
            ParseState psrState = this.psrState;
            int parameter = 0;
            if (psrState.ParameterBytes.Count > 0)
            {
                string text = this.CharacherEncoding.GetString(psrState.ParameterBytes.ToArray());
                if (!int.TryParse(psrState.PrevChar.ToString(), out parameter))
                {
                    logger.ErrorFormat("EraseLine参数不合法,{0}", psrState.PrevChar);
                    return;
                }
            }

            switch (parameter)
            {
                // Erase to Right (default)
                case 0:
                    {
                        //this.Screen.EraseCharAtCaretPosition(-1, this.Screen.CurrentCaretPosition);
                    }
                    break;

                // Erase to Left
                case 1:
                    {
                        //this.Screen.EraseCharAtCaretPosition(1, this.Screen.CurrentCaretPosition);
                    }
                    break;

                // Erase All
                case 2:
                    {
                        //this.Screen.EraseCharAtCaretPosition(0, this.Screen.CurrentCaretPosition);
                    }
                    break;

                default:
                    logger.WarnFormat("未知的EL参数,{0}", parameter);
                    break;
            }
        }

        private void InvokePrintText(object state)
        {
            VTPresentation presentation = (VTPresentation)state;
            this.Screen.PrintText(presentation.Text);
        }

        private void InvokeMoveCursor(object state)
        {
            VTPresentation presentation = (VTPresentation)state;

            if (this.DCMS == TerminalModes.DCSM_PRESENTATION)
            {
                this.Screen.MoveCursor(presentation.CursorColumn, 0);
            }
            else
            {
                throw new NotImplementedException("InvokeMoveCursor TerminalModes.DCSM_DATA");
            }
        }

        private void InvokeAction(SendOrPostCallback action, object userData)
        {
            this.uiThreadContext.Send(action, userData);
        }


        /// <summary>
        /// 解析终端数据流
        /// </summary>
        /// <returns></returns>
        private void StartParsing()
        {
            Task.Factory.StartNew(this.Parse, this.Socket);
        }

        private void ReceiveUTF8String(SocketBase socket, byte unicode_start, ParseState psrState)
        {
            // https://www.cnblogs.com/fnlingnzb-learner/p/6163205.html

            bool bit6 = MiscUtility.GetBit(unicode_start, 5);
            bool bit7 = MiscUtility.GetBit(unicode_start, 6);
            byte nextByte = unicode_start;

            do
            {
                if (!this.psrState.ParsingUnicode)
                {
                    this.psrState.ParsingUnicode = true;
                    // 如果第6位是1，那么7，8位肯定都是1，3字节编码一个字，否则2字节编码一个字
                    this.psrState.UnicodeSize = bit6 ? 3 : 2;
                    this.psrState.UnicodeRemainSize = this.psrState.UnicodeSize;
                    this.psrState.UnicodeBuff = new byte[this.psrState.UnicodeSize];
                }

                this.psrState.UnicodeBuff[this.psrState.UnicodeSize - this.psrState.UnicodeRemainSize] = nextByte;
                this.psrState.UnicodeRemainSize -= 1;

                if (this.psrState.UnicodeRemainSize == 0)
                {
                    // Unicode字符接收完毕
                    this.psrState.ParsingUnicode = false;
                    this.presentation.Text = Encoding.UTF8.GetString(this.psrState.UnicodeBuff);
                    break;
                }
                else
                {
                    // 继续解析
                    nextByte = socket.Read();
                }

            } while (true);

            this.presentation.Text = Encoding.UTF8.GetString(this.psrState.UnicodeBuff);
        }

        private void Parse(object state)
        {
            SocketBase socket = state as SocketBase;

            while (!socket.EOF)
            {
                byte c = socket.Read();

                this.psrState.PrevChar = this.psrState.Char;
                this.psrState.Char = c;
                int prevState = this.psrState.NextState;
                this.psrState.NextState = this.psrState.StateTable[c];
                int nextState = this.psrState.NextState;

                // 如果当前模式是OSC或者CSI控制指令模式，则收集参数序列
                if (this.psrState.State == States.ANSI_OSC)
                {
                    this.psrState.ParameterBytes.Add(c);
                }
                else if (this.psrState.State == States.ANSI_CSI)
                {
                    this.psrState.ParameterBytes.Add(c);
                }

                switch (nextState)
                {
                    // 在每个状态下，接收到的不用处理的字符。
                    case VTPsrDef.CASE_IGNORE:
                        {
                        }
                        continue;

                    #region SingleCharactor ControlFunction（单字节指令）
                    case VTPsrDef.CASE_CR:
                        {
                            this.presentation.Text = "\r";
                            this.InvokeAction(this.printTextAction, this.presentation);
                        }
                        break;
                    case VTPsrDef.CASE_LF:
                        {
                            this.presentation.Text = "\n";
                            this.InvokeAction(this.printTextAction, this.presentation);
                        }
                        break;
                    case VTPsrDef.CASE_VT:
                        { }
                        break;
                    case VTPsrDef.CASE_FF:
                        { }
                        break;
                    case VTPsrDef.CASE_BELL:
                        {
                            /* OSC状态有两种结束模式，一种是以CASE_BELL结束，一种是以CASE_ST结束 */
                            logger.Debug("CASE_BELL");
                            if (this.psrState.State == States.ANSI_OSC)
                            {
                                this.psrState.ParameterBytes.RemoveLast();
                                this.InvokeAction(this.operationSystemCommandsAction, this.presentation);
                                this.psrState.ResetState();
                            }
                            else
                            {
                                /* 响铃 */
                                System.Media.SystemSounds.Beep.Play();
                            }
                        }
                        break;
                    case VTPsrDef.CASE_BS:
                        {
                            logger.Debug("CASE_BS");
                            this.presentation.CursorColumn = -1;
                            this.InvokeAction(this.moveCursorAction, this.presentation);
                        }
                        break;
                    #endregion

                    #region Print
                    // 向屏幕输出终端数据流
                    case VTPsrDef.CASE_PRINT:
                        {
                            bool bit8 = MiscUtility.GetBit(c, 7);
                            if (!bit8)
                            {
                                /* 
                                 * 第八位没用到，7位编码方式，1字节代表一个字，直接输出ascii码
                                 */
                                this.presentation.Text = ((char)c).ToString();
                                this.InvokeAction(this.printTextAction, this.presentation);
                                continue;
                            }

                            if (this.CharacherEncoding == Encoding.UTF8)
                            {
                                this.ReceiveUTF8String(socket, c, this.psrState);
                                this.InvokeAction(this.printTextAction, this.presentation);
                            }
                            else
                            {
                                logger.ErrorFormat("不支持的编码方式");
                                throw new NotImplementedException();
                            }
                        }
                        break;
                    #endregion

                    #region C1 (8-Bit) Control Characters

                    #region Operation System Commands(OSC ESC ])
                    case VTPsrDef.CASE_OSC:
                        {
                            /* 
                             * OSC状态有两种结束模式：
                             * Xterm支持以CASE_BELL结束
                             * 另外一种是以CASE_ST（ 09/13 or ESC 05/13）结束 
                             */
                            this.psrState.StateTable = VTPrsTbl.SOSTable;
                            this.psrState.State = States.ANSI_OSC;
                            logger.Debug("CASE_OSC");
                        }
                        break;
                    #endregion

                    // ESC状态下收到CSI控制字符。
                    case VTPsrDef.CASE_CSI_STATE:
                        {
                            this.psrState.StateTable = VTPrsTbl.CSITable;
                            this.psrState.State = States.ANSI_CSI;
                            logger.Debug("CASE_CSI_STATE");
                        }
                        break;

                    /* 字符串结束符 */
                    case VTPsrDef.CASE_ST:
                        {
                            /* 收到String Terminaotr状态，所有的参数都接收完了，开始处理 */
                            logger.DebugFormat("CASE_ST, ControlString:{0}", this.psrState.State);

                            switch (this.psrState.State)
                            {
                                case States.ANSI_OSC:
                                    {
                                        /* 之前是CASE_OSC状态 */
                                        this.psrState.ParameterBytes.RemoveLast();
                                        this.InvokeAction(this.operationSystemCommandsAction, this.presentation);
                                        this.psrState.ResetState();
                                    }
                                    break;
                            }
                            this.psrState.ResetState();
                        }
                        break;

                    #endregion

                    #region ESC Functions

                    // ANSI状态下收到ESC控制字符。
                    case VTPsrDef.CASE_ESC:
                        {
                            this.psrState.StateTable = VTPrsTbl.EscTable;
                            logger.Debug("CASE_ESC");
                        }
                        break;

                    /* 
                     * 在ESC模式下收到了数字。通常，这是ESC指令的参数。每个数字都代表了一个不同的含义
                     */
                    case VTPsrDef.CASE_ESC_DIGIT:
                        {
                            /*
                             * 存储ESCDigits，收到FinalByte的时候会用到，不同的FinalByte，ParameterBytes有不同的含义
                             * FinalByte代表了CSI命令的功能类型
                             */
                            //this.psrState.ParameterBytes.Add(c);
                            if (this.psrState.StateTable == VTPrsTbl.CSITable)
                            {
                                this.psrState.StateTable = VTPrsTbl.Csi2Table; // xterm-331 charproc.c 2497, TODO:Csi2Table和CsiTable的区别
                            }
                        }
                        break;

                    /*
                     * 在ESC模式下收到了分号（;）。通常，这用来分割ESC控制指令的参数，暂时不做处理
                     */
                    case VTPsrDef.CASE_ESC_SEMI:
                        {
                            if (this.psrState.StateTable == VTPrsTbl.CSITable)
                            {
                                this.psrState.StateTable = VTPrsTbl.Csi2Table; // xterm-331 charproc.c 2509
                            }
                        }
                        break;

                    #region FinalBytes

                    /*
                     * 在ESC模式下收到了FinalByte（结束符），FinalByte表示ESC指令的类型，一个ESC指令由FinalByte结束
                     */

                    case VTPsrDef.CASE_CUU:
                        {
                            // 光标上移
                            logger.Debug("CASE_CUU - cursor up");
                            this.psrState.ResetState();
                        }
                        break;
                    case VTPsrDef.CASE_CUD:
                        {
                            // 光标下移
                            logger.Debug("CASE_CUD - cursor down");
                            this.psrState.ResetState();
                        }
                        break;
                    case VTPsrDef.CASE_CUF:
                        {
                            // 光标左移
                            logger.Debug("CASE_CUF - cursor forward");
                            this.psrState.ResetState();
                        }
                        break;
                    case VTPsrDef.CASE_CUB:
                        {
                            // 光标右移
                            logger.Debug("CASE_CUB - cursor backward");
                            this.psrState.ResetState();
                        }
                        break;
                    case VTPsrDef.CASE_CUP:
                        {
                            // 设置光标位置
                            logger.Debug("CASE_CUP - cursor position");
                            this.psrState.ResetState();
                        }
                        break;
                    /*
                     * CSI状态下收到了FinalByte是SGR的字符。
                     * SGR：SELECT GRAPHIC RENDITION  
                     */
                    case VTPsrDef.CASE_SGR:
                        {
                            logger.Debug("CASE_SGR");
                            //this.HandleSGR(this.psrState.EscDigits);
                            this.psrState.ResetState();
                        }
                        break;
                    #endregion

                    #endregion

                    #region CSI Functions

                    // CSI状态下收到了EL，该字符属于FinalByte，但是该字符前不存在IntermediateBytes
                    case VTPsrDef.CASE_EL:
                        {
                            logger.Debug("CASE_EL");
                            this.psrState.ParameterBytes.RemoveLast();
                            this.InvokeAction(this.eraseLineAction, this.presentation);
                            this.psrState.ResetState();
                        }
                        break;

                    // CSI状态下收到了IntermediateBytes（位组合是02/00的字符），该字符后面紧跟一个FinalByte
                    case VTPsrDef.CASE_CSI_SPACE_STATE:
                        {
                            // 进入FinalByte状态表
                            this.psrState.StateTable = VTPrsTbl.CsiSpTable;
                            logger.Debug("CASE_CSI_SPACE_STATE");
                        }
                        break;

                    case VTPsrDef.CASE_CSI_IGNORE:
                        {
                            // from xterm-331 charproc.c 4409行
                            this.psrState.StateTable = VTPrsTbl.CIgTable;
                        }
                        break;

                    #endregion

                    default:
                        {
                            logger.ErrorFormat("未处理的字符:{0}, State:{1}", c, this.psrState.State);
                        }
                        break;
                }
            }
        }

        #endregion

        #region 事件处理器

        private void Socket_StatusChanged(object stream, SocketState status)
        {
            logger.InfoFormat("VTStream Status Changed：{0}", status);

            switch (status)
            {
                case SocketState.Init:
                    break;

                case SocketState.Ready:
                    this.StartParsing();
                    logger.InfoFormat("开始读取并解析终端数据流...");
                    break;
            }
        }

        #endregion

        public static VideoTerminal Create()
        {
            return new VideoTerminalImpl();
        }
    }
}
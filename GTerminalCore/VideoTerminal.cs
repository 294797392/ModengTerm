using ICare.Utility.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GTerminalCore
{
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
    public abstract class VideoTerminal : IVideoTerminal
    {
        #region 事件

        public event Action<object, VTAction, ParseState> Action;

        #endregion

        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("VideoTerminal");

        #endregion

        #region 实例变量

        private ParseState psrState;

        #endregion

        #region 属性

        /// <summary>
        /// 是否支持8位ascii字符
        /// </summary>
        public virtual bool Support8BitCharacters { get { return false; } }

        /// <summary>
        /// 是否解析C1字符集
        /// </summary>
        public virtual bool SupportC1Characters { get { return false; } }

        public abstract VTTypeEnum Type { get; }

        public virtual IVTStream Stream { get; protected set; }

        public virtual IVTKeyboard Keyboard { get; protected set; }

        /// <summary>
        /// 设置终端字符编码方式
        /// </summary>
        public virtual Encoding CharacherEncoding { get; protected set; }

        #endregion

        #region 构造方法

        public VideoTerminal(IVTStream stream)
        {
            this.psrState = new ParseState();
            this.psrState.StateTable = VTPrsTbl.AnsiTable;
            this.psrState.NextState = VTPrsTbl.AnsiTable[0];

            this.Stream = stream;
            this.Stream.StatusChanged += this.VTStream_StatusChanged;
            this.Keyboard = new GVTKeyboard();
            this.CharacherEncoding = DefaultValues.DefaultEncoding;
        }

        #endregion

        #region 公开接口

        public bool HandleInputWideChar(string wideChar, out byte[] data)
        {
            data = this.CharacherEncoding.GetBytes(wideChar);
            return false;
        }

        public bool HandleInputChar(KeyEventArgs key, out byte[] data)
        {
            data = this.Keyboard.GetCurrentInputData(key);
            return data != null;
        }

        #endregion

        #region 实例方法

        /// <summary>
        /// 解析终端数据流
        /// </summary>
        /// <returns></returns>
        private void StartParsing()
        {
            Task.Factory.StartNew(this.Parse, this.Stream);
        }

        private void HandleUTF8Charactor(IVTStream stream, byte unicode_start, ParseState psrState)
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
                    this.psrState.Text = this.CharacherEncoding.GetString(this.psrState.UnicodeBuff);
                    break;
                }
                else
                {
                    // 继续解析
                    nextByte = stream.Read();
                }

            } while (true);

            psrState.Text = Encoding.UTF8.GetString(this.psrState.UnicodeBuff);
        }

        private void Parse(object state)
        {
            IVTStream stream = state as IVTStream;

            while (!stream.EOF)
            {
                byte c = stream.Read();

                this.psrState.Char = c;
                int lastState = this.psrState.NextState;
                this.psrState.NextState = this.psrState.StateTable[(int)c];
                int nextState = this.psrState.NextState;

                if (this.psrState.StateTable == VTPrsTbl.SosTable)
                {
                    this.psrState.ParameterBytes.Add(c);
                }
                else if (this.psrState.StateTable != VTPrsTbl.EscTable)
                {
                    // 退出了ESC状态，重置ControlFunction
                    this.psrState.ControlFunction = 0;
                }

                switch (nextState)
                {
                    // 在每个状态下，接收到的不用处理的字符。
                    case VTPsrDef.CASE_IGNORE:
                        {
                        }
                        continue;

                    #region 单字节指令（SingleCharactor ControlFunction）
                    case VTPsrDef.CASE_CR:
                        {
                            this.NotifyAction(VTAction.MoveCursor, this.psrState);
                        }
                        break;
                    case VTPsrDef.CASE_LF:
                        {
                            this.NotifyAction(VTAction.NewLine, this.psrState);
                        }
                        break;
                    case VTPsrDef.CASE_VT:
                        { }
                        break;
                    case VTPsrDef.CASE_FF:
                        { }
                        break;
                    #endregion

                    #region 打印终端数据流
                    // 向屏幕输出终端数据流
                    case VTPsrDef.CASE_PRINT:
                        {
                            bool bit8 = MiscUtility.GetBit(c, 7);
                            if (!bit8)
                            {
                                /* 
                                 * 第八位没用到，7位编码方式，1字节代表一个字，直接输出ascii码
                                 */
                                this.psrState.Text = ((char)c).ToString();
                                this.NotifyAction(VTAction.Print, this.psrState);
                                continue;
                            }

                            if (this.CharacherEncoding == Encoding.UTF8)
                            {
                                this.HandleUTF8Charactor(stream, c, this.psrState);
                                this.NotifyAction(VTAction.Print, this.psrState);
                            }
                            else
                            {
                                logger.ErrorFormat("不支持的编码方式");
                                throw new NotImplementedException();
                            }
                        }
                        break;
                    #endregion

                    // ANSI状态下收到ESC控制字符。
                    case VTPsrDef.CASE_ESC:
                        {
                            this.psrState.StateTable = VTPrsTbl.EscTable;
                            logger.Debug("CASE_ESC");
                        }
                        break;

                    #region ESC子状态
                    case VTPsrDef.CASE_OSC:
                        {
                            this.psrState.StateTable = VTPrsTbl.SosTable;
                            this.psrState.ControlFunction = ANSI.ANSI_OSC;
                            logger.Debug("CASE_OSC");
                        }
                        break;
                    #endregion


                    case VTPsrDef.CASE_ST:
                        {
                            /* 收到String Terminaotr状态，所有的参数都接收完了，开始处理 */
                            logger.DebugFormat("CASE_ST, parameters:{0}", this.CharacherEncoding.GetString(this.psrState.ParameterBytes.ToArray()));

                            switch (this.psrState.ControlFunction)
                            {
                                case ANSI.ANSI_OSC:
                                    {
                                        /* 之前是CASE_OSC状态 */
                                    }
                                    break;
                            }

                            this.psrState.ParameterBytes.Clear();
                        }
                        break;


                    case VTPsrDef.CASE_BELL:
                        {
                            logger.Debug("CASE_BELL");
                        }
                        break;


                    // ESC状态下收到CSI控制字符。
                    case VTPsrDef.CASE_CSI_STATE:
                        {
                            this.psrState.StateTable = VTPrsTbl.CsiTable;
                        }
                        break;

                    /* ESC状态下收到数字字符。在CSI子状态下，属于参数字段；在DEC子状态下，？ */
                    case VTPsrDef.CASE_ESC_DIGIT:
                        {
                            /*
                             * CASE_CSI_STATE模式下:
                             *      ParameterBytes
                             * DEC模式下：
                             * 
                             * 存储ParameterBytes，收到FinalByte的时候会用到，不同的FinalByte，ParameterBytes有不同的含义
                             * FinalByte代表了CSI命令的功能类型
                             */
                            this.psrState.ParameterBytes.Add(c);
                            if (this.psrState.StateTable == VTPrsTbl.CsiTable)
                            {
                                this.psrState.StateTable = VTPrsTbl.Csi2Table; // TODO:Csi2Table和CsiTable的区别
                            }
                        }
                        break;

                    // ESC状态下接收到了分号
                    case VTPsrDef.CASE_ESC_SEMI:
                        {
                            this.psrState.ParameterBytes.Add(c);
                            if (this.psrState.StateTable == VTPrsTbl.CsiTable)
                            {
                                this.psrState.StateTable = VTPrsTbl.Csi2Table;
                            }
                        }
                        break;

                    // CSI状态下收到了IntermediateBytes（位组合是02/00的字符），该字符后面紧跟一个FinalByte
                    case VTPsrDef.CASE_CSI_SPACE_STATE:
                        {
                            // 进入FinalByte状态表
                            this.psrState.StateTable = VTPrsTbl.CsiSpTable;
                        }
                        break;

                    case VTPsrDef.CASE_CSI_IGNORE:
                        {
                            // from xterm-331 charproc.c 4409行
                            this.psrState.StateTable = VTPrsTbl.CIgTable;
                        }
                        break;

                    // CSI状态下收到了FinalByte是SGR的字符。
                    case VTPsrDef.CASE_SGR:
                        {
                            // 重置为ANSI状态
                            this.psrState.StateTable = VTPrsTbl.AnsiTable;
                        }
                        break;

                    default:
                        {
                            logger.ErrorFormat("未解析的字符:{0}, StateTable:{1}", c, this.psrState.StateTable);
                        }
                        break;
                }
            }
        }

        protected virtual void NotifyAction(VTAction action, ParseState state)
        {
            if (this.Action != null)
            {
                this.Action(this, action, state);
            }
        }

        #endregion

        #region 事件处理器

        private void VTStream_StatusChanged(object stream, VTStreamState status)
        {
            logger.InfoFormat("VTStream Status Changed：{0}", status);

            switch (status)
            {
                case VTStreamState.Init:
                    break;

                case VTStreamState.Ready:
                    this.StartParsing();
                    logger.InfoFormat("开始读取并解析终端数据流...");
                    break;
            }
        }

        #endregion
    }
}
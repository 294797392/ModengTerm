using DotNEToolkit.Extentions;
using ModengTerm.Terminal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using XTerminal.Base;

namespace ModengTerm.Terminal.Parsing
{
    /// <summary>
    /// 模块名称：终端数据流解析器
    /// 负责解析数据流
    /// 模块描述：对从终端获取到的数据流进行解析操作，并抛对应的事件给外部模块
    /// 
    /// 参考资料：
    /// https://vt100.net/emu/dec_ansi_parser
    /// https://invisible-island.net/xterm/
    /// https://github.com/microsoft/terminal.git
    /// https://invisible-island.net/xterm/ctlseqs/ctlseqs.html
    /// Dependencies/Control Functions for Coded Character Sets Ecma-048.pdf
    /// 
    /// 控制字符集分两种：
    /// C0控制字符：C0控制字符就是ASCII码，一个ASCII码表示一个控制字符，范围是0x00-0x1F（十进制是0-31）
    /// C1控制字符：范围是0x80-0x9F（十进制是128-159）。C1控制字符集有两个区间，如果系统只支持7位ASCII码字符集，那么C1控制字符的范围是0x4F-0x5F(十进制是79-95)；如果系统支持8位ASCII码字符集，那么C1控制字符的范围是0x8F-0x9F（十进制是143-159）
    /// 作为一个终端模拟器，那么必须要同时支持7位字符集的C1控制字符和8位字符集的C1控制字符
    /// ASCII码为0x80-0xFF（十进制是143-255）的字符又称为扩充ASCII码字符集
    /// 
    /// 控制序列：
    ///     控制序列可能由7位ASCII码或者8位ASCII码表示
    ///     对于7位ascii编码：
    ///         由两个字符开头。第一个字符是01/11，第二个是Fe。Fe可以看作成命令类型，Fe的范围在04/00（64） - 05/15（95）之间
    ///     对于8位ascii编码：
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
    ///     OSC：
    ///         名称：系统命令
    ///         格式：OSC 数字;字符串 BEL
    ///         参数说明：
    ///             BEL表示OSC的结束符，数字和字符串之间使用分号分割，可以通过判断是否读取到了分号的方式去收集数字和字符串
    ///         参考：
    ///             xterm-331 - VTPrsTbl.c 6578行
    ///             terminal - stateMachine.cpp _EventOscParam函数
    /// 
    /// UTF8：用UTF-8就有复杂点.因为此时程序是把一个字节一个字节的来读取,然后再根据字节中开头的bit标志来识别是该把1个还是两个或三个字节做为一个单元来处理.
    /// UTF16：UTF-16就把两个字节当成一个单元来解析.这个很简单.
    /// UTF32：UTF-32就把四个字节当成一个单元来解析.这个很简单.
    /// </summary>
    public partial class VTParser
    {
        // The DEC STD 070 reference recommends supporting up to at least 16384 for
        // parameter values, so 32767 should be more than enough. At most we might
        // want to increase this to 65535, since that is what XTerm and VTE support,
        // but for now 32767 is the safest limit for our existing code base.
        private const int MAX_PARAMETER_VALUE = 32767;

        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("VTParser");

        #endregion

        #region 公开事件

        #endregion

        #region 实例变量

        private VTStates state;

        private int oscParam;
        private StringBuilder oscString;

        /// <summary>
        /// 存储CSI非Param的参数
        /// </summary>
        private VTID vtid;
        private List<int> parameters;

        /// <summary>
        /// Designate VT52 mode (DECANM)
        /// </summary>
        private bool isAnsiMode;

        /// <summary>
        /// 是否处理C1区域的转义字符序列
        /// </summary>
        private bool acceptC1Control;

        /// <summary>
        /// 当前Keypad的模式是否是Application模式
        /// </summary>
        private bool isApplicationMode;

        private DCSStringHandlerDlg dcsStringHandler;

        private List<byte> sequenceBytes;

        /// <summary>
        /// 待显示的还未显示的字符字节数组
        /// </summary>
        private List<byte> pendingPrint;

        #endregion

        #region 属性

        /// <summary>
        /// 解析数据使用的编码格式
        /// </summary>
        public Encoding Encoding { get; set; }

        #endregion

        #region 公开接口

        /// <summary>
        /// 初始化终端解析器
        /// </summary>
        /// <param name="eventDlg">终端事件回调</param>
        public void Initialize()
        {
            this.isAnsiMode = true;
            this.isApplicationMode = false;

            this.oscString = new StringBuilder();
            this.oscParam = 0;

            this.vtid = new VTID();
            this.parameters = new List<int>();

            this.state = VTStates.Ground;   // 状态机默认设置为基态

            this.sequenceBytes = new List<byte>();
            this.pendingPrint = new List<byte>();
        }

        public void Release()
        {

        }

        /// <summary>
        /// 解析终端字节流
        /// </summary>
        /// <param name="bytes">要解析的字节流</param>
        /// <param name="size">字节流长度</param>
        public void ProcessCharacters(byte[] bytes, int size)
        {
            int length = size;

            for (int i = 0; i < length; i++)
            {
                byte ch = bytes[i];

                this.sequenceBytes.Add(ch);

                // 在OSCString的状态下，ESC转义字符可以用作OSC状态的结束符，所以在这里不进入ESC状态
                //if (VTParserUtils.IsC1ControlCharacter(ch))
                //{
                //    if (this.acceptC1Control)
                //    {
                //        byte[] c1To7Bit = new byte[2] { 0x1B, (byte)(ch - 0x40) };
                //        this.ProcessCharacters(c1To7Bit, 2);
                //    }
                //}
                if (VTParserUtils.IsEscape(ch) && this.state != VTStates.OSCString && this.state != VTStates.OSCParam)
                {
                    this.EnterEscape();
                }
                else
                {
                    switch (this.state)
                    {
                        case VTStates.Ground:
                            {
                                this.EventGround(ch);
                                break;
                            }

                        case VTStates.Escape:
                            {
                                this.EventEscape(ch);
                                break;
                            }

                        case VTStates.EscapeIntermediate:
                            {
                                this.EventEscapeIntermediate(ch);
                                break;
                            }

                        case VTStates.OSCParam:
                            {
                                this.EventOSCParam(ch);
                                break;
                            }

                        case VTStates.OSCString:
                            {
                                this.EventOSCString(ch);
                                break;
                            }

                        case VTStates.OSCTermination:
                            {
                                this.EventOSCTermination(ch);
                                break;
                            }

                        case VTStates.CSIEntry:
                            {
                                this.EventCSIEntry(ch);
                                break;
                            }

                        case VTStates.CSIIntermediate:
                            {
                                this.EventCSIIntermediate(ch);
                                break;
                            }

                        case VTStates.CSIIgnore:
                            {
                                this.EventCSIIgnore(ch);
                                break;
                            }

                        case VTStates.CSIParam:
                            {
                                this.EventCSIParam(ch);
                                break;
                            }

                        case VTStates.DCSEntry:
                            {
                                this.EventDCSEntry(ch);
                                break;
                            }

                        case VTStates.DCSIgnore:
                            {
                                this.EventDCSIgnore(ch);
                                break;
                            }

                        case VTStates.DCSIntermediate:
                            {
                                this.EventDCSIntermediate(ch);
                                break;
                            }

                        case VTStates.DCSParam:
                            {
                                this.EventDCSParam(ch);
                                break;
                            }

                        case VTStates.DCSPassthrough:
                            {
                                this.EventDCSPassThrough(ch);
                                break;
                            }

                        case VTStates.Vt52Param:
                            {
                                this.EventVt52Param(ch);
                                break;
                            }

                        default:
                            {
                                throw new NotImplementedException();
                            }
                    }
                }
            }

            this.PrintPending();
        }

        #endregion

        #region 实例方法

        /// <summary>
        /// 函数功能：
        /// 如果value是437，ch是2，那么返回的结果就是4372
        /// </summary>
        /// <param name="ch"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private int AccumulateTo(byte ch, int value)
        {
            int digit = ch - '0';

            value = value * 10 + digit;

            if (value > MAX_PARAMETER_VALUE)
            {
                value = MAX_PARAMETER_VALUE;
            }

            return value;
        }

        /// <summary>
        /// 做Cancel操作
        /// </summary>
        private void ActionClear()
        {
            this.oscParam = 0;
            this.oscString.Clear();

            this.vtid.Clear();
            this.parameters.Clear();

            this.dcsStringHandler = null;
        }

        /// <summary>
        /// This state is entered whenever the C0 control ESC is received. This will immediately cancel any escape sequence, control sequence or control string in progress
        /// </summary>
        private void EnterEscape()
        {
            this.state = VTStates.Escape;
            // 根据https://vt100.net/emu/dec_ansi_parser#STDCSIGN这个网页上的表格，第一次进入Escape状态的时候需要做clear动作
            this.ActionClear();
        }

        private void EnterEscapeIntermediate()
        {
            this.state = VTStates.EscapeIntermediate;
        }

        private void EnterGround()
        {
            this.state = VTStates.Ground;
        }

        private void EnterCSIEntry()
        {
            this.state = VTStates.CSIEntry;
            // 根据https://vt100.net/emu/dec_ansi_parser#STDCSIGN这个网页上的表格，第一次进入csi entry状态的时候需要做clear动作
            this.ActionClear();
        }

        private void EnterCSIIntermediate()
        {
            this.state = VTStates.CSIIntermediate;
        }

        private void EnterCSIIgnore()
        {
            this.state = VTStates.CSIIgnore;
        }

        private void EnterCSIParam()
        {
            this.state = VTStates.CSIParam;
        }

        private void EnterOSCParam()
        {
            this.state = VTStates.OSCParam;
        }

        private void EnterOSCString()
        {
            this.state = VTStates.OSCString;
        }

        private void EnterOSCTermination()
        {
            this.state = VTStates.OSCTermination;
        }

        private void EnterDCSEntry()
        {
            this.state = VTStates.DCSEntry;
            this.ActionClear();
        }

        private void EnterDCSIgnore()
        {
            this.state = VTStates.DCSIgnore;
        }

        private void EnterDCSParam()
        {
            this.state = VTStates.DCSParam;
        }

        private void EnterDCSIntermediate()
        {
            this.state = VTStates.DCSIntermediate;
        }

        private void EnterDCSPassThrough()
        {
            this.state = VTStates.DCSPassthrough;
        }

        private void EnterVt52Param()
        {
            this.state = VTStates.Vt52Param;
        }

        #region Action - An event may cause one of these actions to occur with or without a change of state

        private void ActionIgnore(byte ch)
        {

        }

        private void ActionCSIDispatch(byte ch)
        {
            this.vtid.Add(ch);
            this.ActionCSIDispatch(this.vtid, this.parameters);
        }

        /// <summary>
        /// 收集OSC参数的动作
        /// </summary>
        /// <param name="ch"></param>
        private void ActionOSCParam(byte ch)
        {
            this.oscParam = this.AccumulateTo(ch, this.oscParam);
        }

        /// <summary>
        /// 分发OSC事件
        /// </summary>
        private void ActionOSCDispatch(byte ch)
        {
            //Console.WriteLine("分发的OSC字符串 = {0}", this.oscString);
        }

        /// <summary>
        /// 收集CSI或者Escape状态下的Intermediate字符
        /// </summary>
        /// <param name="ch"></param>
        private void ActionCollect(byte ch)
        {
            this.vtid.Add(ch);
        }

        /// <summary>
        /// 收集CSI状态下的Parameter字符
        ///  - Triggers the Param action to indicate that the state machine should store this character as a part of a parameter
        ///   to a control sequence.
        /// </summary>
        /// <param name="ch"></param>
        private void ActionParam(byte ch)
        {
            if (this.parameters.Count == 0)
            {
                this.parameters.Add(0);
            }

            if (VTParserUtils.IsParameterDelimiter(ch))
            {
                this.parameters.Add(0);
            }
            else
            {
                int last = this.parameters.Last();
                this.parameters[this.parameters.Count - 1] = this.AccumulateTo(ch, last);
            }
        }

        /// <summary>
        /// When a final character has been recognised in a device control string, this state will establish a channel to a handler for the appropriate control function, and then pass all subsequent characters through to this alternate handler, until the data string is terminated
        /// 当在设备控制字符串中识别出最后一个字符时，此状态将建立通向适当控制功能的处理程序的通道，然后将所有后续字符传递给此备用处理程序，直到数据字符串终止
        /// </summary>
        /// <param name="ch">final byte</param>
        private void ActionDCSDispatch(byte ch)
        {
            // 根据最后一个字符判断要使用的DCS的事件处理器
            this.dcsStringHandler = this.ActionDCSDispatch(ch, this.parameters);

            if (this.dcsStringHandler != null)
            {
                // 获取到了该DCS事件处理器，那么要进入收集DCS后续字符的状态
                this.EnterDCSPassThrough();
            }
            else
            {
                // 没获取到该DCS事件处理器（通常情况是没实现该处理器，不支持处理该事件），那么直接进入忽略DCS事件状态
                this.EnterDCSIgnore();
            }
        }

        /// <summary>
        /// - Triggers the Vt52EscDispatch action to indicate that the listener should handle
        ///      a VT52 escape sequence. These sequences start with ESC and a single letter,
        ///      sometimes followed by parameters.
        /// </summary>
        /// <param name="ch"></param>
        private void ActionVt52EscDispatch(byte ch)
        {
            this.ActionVt52EscDispatch(ch, this.parameters);
        }

        #endregion

        #region Event - 处理状态机的逻辑

        /// <summary>
        /// 当状态改变为Ground的时候触发
        /// </summary>
        /// <param name="ch"></param>
        private void EventGround(byte ch)
        {
            if (VTParserUtils.IsC0Code(ch) || VTParserUtils.IsDelete(ch))
            {
                // 如果是C0控制字符和Delete字符，说明要执行动作
                this.ActionExecute(ch);
            }
            else if (VTParserUtils.IsPrintable(ch))
            {
                // 其他字符直接打印
                this.ActionPrint(ch);
            }
            else
            {
                // 不是可见字符，当多字节字符处理，继续打印
                this.ActionPrint(ch);

                //// 不是可见字符，当多字节字符处理，用UTF8编码
                //// UTF8参考：https://www.cnblogs.com/fnlingnzb-learner/p/6163205.html
                //if (this.unicodeText.Count == 0)
                //{
                //    bool bit6 = DotNEToolkit.Utility.ByteUtils.GetBit(ch, 5);
                //    this.unicodeText.Capacity = bit6 ? 3 : 2;
                //}

                //this.unicodeText.Add(ch);
                //if (this.unicodeText.Count == this.unicodeText.Capacity)
                //{
                //    string text = this.Encoding.GetString(this.unicodeText.ToArray());
                //    this.ActionPrint(text[0]);
                //    this.unicodeText.Clear();
                //}
            }
        }

        /// <summary>
        /// 当状态变成Escape的时候触发
        /// </summary>
        /// <param name="ch"></param>
        private void EventEscape(byte ch)
        {
            if (VTParserUtils.IsC0Code(ch))
            {
                this.ActionExecute(ch);
            }
            else if (VTParserUtils.IsDelete(ch))
            {
                this.ActionIgnore(ch);
            }
            else if (VTParserUtils.IsIntermediate(ch))
            {
                this.ActionCollect(ch);
                this.EnterEscapeIntermediate();
            }
            else if (this.isAnsiMode)
            {
                if (VTParserUtils.IsCSIIndicator(ch))
                {
                    // 0x5B，进入到了csi entry状态
                    this.EnterCSIEntry();
                }
                else if (VTParserUtils.IsOSCIndicator(ch))
                {
                    // 0x5D，进入到了osc状态
                    this.EnterOSCParam();
                }
                else if (VTParserUtils.IsDCSIndicator(ch))
                {
                    // 0x50，进入到了dcs状态
                    this.EnterDCSEntry();
                }
                else
                {
                    this.ActionEscDispatch(ch);
                    this.EnterGround();
                }
            }
            else if (VTParserUtils.IsVt52CursorAddress(ch))
            {
                // 判断是否是VT52模式下的移动光标指令, 当进入了VT52模式下才会触发
                // 在VT52模式下只有移动光标的指令有参数，所以这里把移动光标的指令单独做处理
                this.EnterVt52Param();
            }
            else
            {
                // 这里是其他的不带参数的VT52控制字符
                this.ActionVt52EscDispatch(ch);
                this.EnterGround();
            }
        }

        /// <summary>
        /// - Processes a character event into an Action that occurs while in the EscapeIntermediate state.
        ///   Events in this state will:
        ///   1. Execute C0 control characters
        ///   2. Ignore Delete characters
        ///   3. Collect Intermediate characters
        ///   4. Dispatch an Escape action.
        /// </summary>
        /// <param name="ch"></param>
        private void EventEscapeIntermediate(byte ch)
        {
            if (VTParserUtils.IsC0Code(ch))
            {
                this.ActionExecute(ch);
            }
            else if (VTParserUtils.IsIntermediate(ch))
            {
                this.ActionCollect(ch);
            }
            else if (VTParserUtils.IsDelete(ch))
            {
                this.ActionIgnore(ch);
            }
            else if (this.isAnsiMode)
            {
                this.ActionEscDispatch(ch);
                this.EnterGround();
            }
            else if (VTParserUtils.IsVt52CursorAddress(ch))
            {
                this.EnterVt52Param();
            }
            else
            {
                this.ActionVt52EscDispatch(ch);
                this.EnterGround();
            }
        }

        /// <summary>
        /// 进入到了OSC状态，开始解析OSC命令
        /// </summary>
        /// <param name="ch"></param>
        private void EventOSCParam(byte ch)
        {
            if (VTParserUtils.IsOSCTerminator(ch))
            {
                // OSC状态下出现了BEL结束符
                // 参考terminal的做法，进入Ground状态
                this.EnterGround();
            }
            else if (VTParserUtils.IsNumericParamValue(ch))
            {
                // OSC状态下的数字，收集起来
                this.ActionOSCParam(ch);
            }
            else if (VTParserUtils.IsOSCDelimiter(ch))
            {
                // OSC状态下出现了分隔符，说明要开始收集字符串了
                this.EnterOSCString();
            }
            else
            {
                // 其他所有的字符都忽略
                this.ActionIgnore(ch);
            }
        }

        /// <summary>
        /// - Processes a character event into a Action that occurs while in the OscParam state.
        ///   Events in this state will:
        ///   1. Trigger the OSC action associated with the param on an OscTerminator
        ///   2. If we see a ESC, enter the OscTermination state. We'll wait for one
        ///      more character before we dispatch the string.
        ///   3. Ignore OscInvalid characters.
        ///   4. Collect everything else into the OscString
        /// </summary>
        /// <param name="ch"></param>
        private void EventOSCString(byte ch)
        {
            if (VTParserUtils.IsOSCTerminator(ch))
            {
                // 出现了OSC结束符，那么进入Ground状态
                this.ActionOSCDispatch(ch);
                this.EnterGround();
            }
            else if (VTParserUtils.IsEscape(ch))
            {
                // OSC状态下出现了ESC字符，那么有两种情况会出现：
                // 1. ESC后面有ST字符，说明是OSC状态结束了
                // 2. ESC后面没有ST字符，说明是ESC状态
                // 所以这里定义一个OSCTermination状态来处理这两种状态
                this.EnterOSCTermination();
            }
            else if (VTParserUtils.IsOSCIndicator(ch))
            {
                // OSC非法字符，忽略
                this.ActionIgnore(ch);
            }
            else
            {
                // 剩下的就是OSC的有效字符，收集
                this.oscString.Append((char)ch);
            }
        }

        /// <summary>
        /// - Handle the two-character termination of a OSC sequence.
        ///   Events in this state will:
        ///   1. Trigger the OSC action associated with the param on an OscTerminator
        ///   2. Otherwise treat this as a normal escape character event.
        /// </summary>
        /// <param name="ch"></param>
        private void EventOSCTermination(byte ch)
        {
            if (VTParserUtils.IsStringTermination(ch))
            {
                // OSC状态下出现了ESC后，后面紧跟着ST字符，说明是OSC状态结束了
                this.ActionOSCDispatch(ch);
                this.EnterGround();
            }
            else
            {
                // OSC状态下出现了ESC后，后面没有ST字符，说明要Cancel OSC状态并直接进入ESC模式
                this.EnterEscape();
                this.EventEscape(ch);
            }
        }

        /// <summary>
        /// - Processes a character event into an Action that occurs while in the CsiEntry state.
        ///   Events in this state will:
        ///   1. Execute C0 control characters
        ///   2. Ignore Delete characters
        ///   3. Collect Intermediate characters
        ///   4. Begin to ignore all remaining parameters when an invalid character is detected (CsiIgnore)
        ///   5. Store parameter data
        ///   6. Collect Control Sequence Private markers
        ///   7. Dispatch a control sequence with parameters for action
        /// </summary>
        /// <param name="ch"></param>
        private void EventCSIEntry(byte ch)
        {
            if (VTParserUtils.IsC0Code(ch))
            {
                this.ActionExecute(ch);
            }
            else if (VTParserUtils.IsDelete(ch))
            {
                this.ActionIgnore(ch);
            }
            else if (VTParserUtils.IsIntermediate(ch))
            {
                this.ActionCollect(ch);
                this.EnterCSIIntermediate();
            }
            else if (VTParserUtils.IsCSIInvalid(ch))
            {
                this.EnterCSIIgnore();
            }
            else if (VTParserUtils.IsNumericParamValue(ch) || VTParserUtils.IsParameterDelimiter(ch))
            {
                this.ActionParam(ch);
                this.EnterCSIParam();
            }
            else if (VTParserUtils.IsCSIPrivateMarker(ch))
            {
                this.ActionCollect(ch);
                this.EnterCSIParam();
            }
            else
            {
                this.ActionCSIDispatch(ch);
                this.EnterGround();
            }
        }

        /// <summary>
        /// - Processes a character event into an Action that occurs while in the CsiIntermediate state.
        ///   Events in this state will:
        ///   1. Execute C0 control characters
        ///   2. Ignore Delete characters
        ///   3. Collect Intermediate characters
        ///   4. Begin to ignore all remaining parameters when an invalid character is detected (CsiIgnore)
        ///   5. Dispatch a control sequence with parameters for action
        /// </summary>
        /// <param name="ch"></param>
        private void EventCSIIntermediate(byte ch)
        {
            if (VTParserUtils.IsC0Code(ch))
            {
                this.ActionExecute(ch);
            }
            else if (VTParserUtils.IsIntermediate(ch))
            {
                this.ActionCollect(ch);
            }
            else if (VTParserUtils.IsDelete(ch))
            {
                this.ActionIgnore(ch);
            }
            else if (VTParserUtils.IsIntermediateInvalid(ch))
            {
                this.EnterCSIIgnore();
            }
            else
            {
                this.ActionCSIDispatch(ch);
                this.EnterGround();
            }
        }

        /// <summary>
        ///  Processes a character event into an Action that occurs while in the CsiIgnore state.
        ///   Events in this state will:
        ///   1. Execute C0 control characters
        ///   2. Ignore Delete characters
        ///   3. Collect Intermediate characters
        ///   4. Begin to ignore all remaining parameters when an invalid character is detected (CsiIgnore)
        ///   5. Return to Ground
        /// </summary>
        /// <param name="ch"></param>
        private void EventCSIIgnore(byte ch)
        {
            if (VTParserUtils.IsC0Code(ch))
            {
                this.ActionExecute(ch);
            }
            else if (VTParserUtils.IsDelete(ch))
            {
                this.ActionIgnore(ch);
            }
            else if (VTParserUtils.IsIntermediate(ch))
            {
                this.ActionIgnore(ch);
            }
            else if (VTParserUtils.IsIntermediateInvalid(ch))
            {
                this.ActionIgnore(ch);
            }
            else
            {
                this.EnterGround();
            }
        }

        /// <summary>
        ///  - Processes a character event into an Action that occurs while in the CsiParam state.
        ///   Events in this state will:
        ///   1. Execute C0 control characters
        ///   2. Ignore Delete characters
        ///   3. Collect Intermediate characters
        ///   4. Begin to ignore all remaining parameters when an invalid character is detected (CsiIgnore)
        ///   5. Store parameter data
        ///   6. Dispatch a control sequence with parameters for action
        /// </summary>
        /// <param name="ch"></param>
        private void EventCSIParam(byte ch)
        {
            if (VTParserUtils.IsC0Code(ch))
            {
                this.ActionExecute(ch);
            }
            else if (VTParserUtils.IsDelete(ch))
            {
                this.ActionIgnore(ch);
            }
            else if (VTParserUtils.IsNumericParamValue(ch) || VTParserUtils.IsParameterDelimiter(ch))
            {
                this.ActionParam(ch);
            }
            else if (VTParserUtils.IsIntermediate(ch))
            {
                this.ActionCollect(ch);
                this.EnterCSIIntermediate();
            }
            else if (VTParserUtils.IsParameterInvalid(ch))
            {
                this.EnterCSIIgnore();
            }
            else
            {
                this.ActionCSIDispatch(ch);
                this.EnterGround();
            }
        }

        /// <summary>
        /// - Processes a character event into an Action that occurs while in the DcsEntry state.
        ///   Events in this state will:
        ///   1. Ignore C0 control characters
        ///   2. Ignore Delete characters
        ///   3. Begin to ignore all remaining characters when an invalid character is detected (DcsIgnore)
        ///   4. Store parameter data
        ///   5. Collect Intermediate characters
        ///   6. Dispatch the Final character in preparation for parsing the data string
        ///  DCS sequences are structurally almost the same as CSI sequences, just with an
        ///      extra data string. It's safe to reuse CSI functions for
        ///      determining if a character is a parameter, delimiter, or invalid.
        /// </summary>
        /// <param name="ch"></param>
        private void EventDCSEntry(byte ch)
        {
            if (VTParserUtils.IsC0Code(ch))
            {
                this.ActionIgnore(ch);
            }
            else if (VTParserUtils.IsDelete(ch))
            {
                this.ActionIgnore(ch);
            }
            else if (VTParserUtils.IsCSIInvalid(ch))
            {
                this.EnterDCSIgnore();
            }
            else if (VTParserUtils.IsNumericParamValue(ch) || VTParserUtils.IsParameterDelimiter(ch))
            {
                this.ActionParam(ch);
                this.EnterDCSParam();
            }
            else if (VTParserUtils.IsIntermediate(ch))
            {
                this.ActionCollect(ch);
                this.EnterDCSIntermediate();
            }
            else
            {
                this.ActionDCSDispatch(ch);
            }
        }

        /// <summary>
        /// - Processes a character event into an Action that occurs while in the DcsIgnore state.
        ///   In this state the entire DCS string is considered invalid and we will ignore everything.
        ///   The termination state is handled outside when an ESC is seen.
        /// </summary>
        /// <param name="ch"></param>
        private void EventDCSIgnore(byte ch)
        {
            this.ActionIgnore(ch);
        }

        /// <summary>
        /// - Processes a character event into an Action that occurs while in the DcsIntermediate state.
        ///   Events in this state will:
        ///   1. Ignore C0 control characters
        ///   2. Ignore Delete characters
        ///   3. Collect intermediate data.
        ///   4. Begin to ignore all remaining intermediates when an invalid character is detected (DcsIgnore)
        ///   5. Dispatch the Final character in preparation for parsing the data string
        /// </summary>
        /// <param name="ch"></param>
        private void EventDCSIntermediate(byte ch)
        {
            if (VTParserUtils.IsC0Code(ch))
            {
                this.ActionIgnore(ch);
            }
            else if (VTParserUtils.IsDelete(ch))
            {
                this.ActionIgnore(ch);
            }
            else if (VTParserUtils.IsIntermediate(ch))
            {
                this.ActionCollect(ch);
            }
            else if (VTParserUtils.IsIntermediateInvalid(ch))
            {
                this.EnterDCSIgnore();
            }
            else
            {
                this.ActionDCSDispatch(ch);
            }
        }

        /// <summary>
        /// - Processes a character event into an Action that occurs while in the DcsParam state.
        ///   Events in this state will:
        ///   1. Ignore C0 control characters
        ///   2. Ignore Delete characters
        ///   3. Collect DCS parameter data
        ///   4. Enter DcsIntermediate if we see an intermediate
        ///   5. Begin to ignore all remaining parameters when an invalid character is detected (DcsIgnore)
        ///   6. Dispatch the Final character in preparation for parsing the data string
        /// </summary>
        /// <param name="ch"></param>
        private void EventDCSParam(byte ch)
        {
            if (VTParserUtils.IsC0Code(ch))
            {
                this.ActionIgnore(ch);
            }
            else if (VTParserUtils.IsDelete(ch))
            {
                this.ActionIgnore(ch);
            }
            else if (VTParserUtils.IsNumericParamValue(ch) || VTParserUtils.IsParameterDelimiter(ch))
            {
                this.ActionParam(ch);
            }
            else if (VTParserUtils.IsIntermediate(ch))
            {
                this.ActionCollect(ch);
                this.EnterDCSIntermediate();
            }
            else if (VTParserUtils.IsParameterInvalid(ch))
            {
                this.EnterDCSIgnore();
            }
            else
            {
                this.ActionDCSDispatch(ch);
            }
        }

        /// <summary>
        /// - Processes a character event into an Action that occurs while in the DcsPassThrough state.
        ///   Events in this state will:
        ///   1. Pass through if character is valid.
        ///   2. Ignore everything else.
        ///   The termination state is handled outside when an ESC is seen.
        /// </summary>
        /// <param name="ch"></param>
        private void EventDCSPassThrough(byte ch)
        {
            if (VTParserUtils.IsC0Code(ch) || VTParserUtils.IsDCSPassThroughValid(ch))
            {
                if (!this.dcsStringHandler(ch))
                {
                    this.EnterDCSIgnore();
                }
            }
            else
            {
                this.ActionIgnore(ch);
            }
        }

        /// <summary>
        /// - Processes a character event into an Action that occurs while in the Vt52Param state.
        ///   Events in this state will:
        ///   1. Execute C0 control characters
        ///   2. Ignore Delete characters
        ///   3. Store exactly two parameter characters
        ///   4. Dispatch a control sequence with parameters for action (always Direct Cursor Address)
        /// </summary>
        /// <param name="ch"></param>
        private void EventVt52Param(byte ch)
        {
            if (VTParserUtils.IsC0Code(ch))
            {
                this.ActionExecute(ch);
            }
            else if (VTParserUtils.IsDelete(ch))
            {
                this.ActionIgnore(ch);
            }
            else
            {
                this.parameters.Add(ch);
                if (this.parameters.Count == 2)
                {
                    // The command character is processed before the parameter values,
                    // but it will always be 'Y', the Direct Cursor Address command.

                    // 到了这里说明Y指令的参数收集完了，可以执行了，因为Y指令是移动光标指令，有且只有两个参数
                    this.ActionVt52EscDispatch((byte)'Y');
                    this.EnterGround();
                }
            }
        }

        #endregion

        #endregion

        #region 事件处理器

        #endregion
    }
}

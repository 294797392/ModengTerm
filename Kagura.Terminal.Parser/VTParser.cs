using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using VideoTerminal.Sockets;

namespace VideoTerminal.Parser
{
    /// <summary>
    /// 模块名称：终端数据流解析器
    /// 
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
    public class VTParser
    {
        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("VTParserMain");

        #endregion

        #region 实例变量

        private SocketBase socket;
        private Dictionary<StateMachineID, StateMachine> stateMachineMap;

        #endregion

        #region 属性

        /// <summary>
        /// 存储字符解析器上下文信息
        /// </summary>
        public VTParserContext ParserContext { get; private set; }

        #endregion

        #region 公开接口

        /// <summary>
        /// 运行终端解析器
        /// </summary>
        public void Run(SocketBase socket)
        {
            this.stateMachineMap = new Dictionary<StateMachineID, StateMachine>();
            this.socket = socket;
            this.socket.DataReceived += this.Socket_DataReceived;
            this.ParserContext = new VTParserContext()
            {
                State = VTStates.Ground,        // 状态机默认设置为基态
            };
        }

        #endregion

        #region 实例方法

        #endregion

        #region 事件处理器

        private void Socket_DataReceived(SocketBase sender, byte[] bytes)
        {
            int length = bytes.Length;

            for (int i = 0; i < length; i++)
            {
                byte ch = bytes[i];

                // 更新ParserContext数据
                this.ParserContext.PreviousChar = this.ParserContext.CurrentChar;
                this.ParserContext.CurrentChar = ch;

                StateTable table = StateTable.GetTable(this.ParserContext.State);
                StateMachineID smid = table.StateMachineList[ch];

                try
                {
                    // 开始运行状态机, 一直运行到下一个基态为止
                    StateMachine stateMachine;
                    if (!this.stateMachineMap.TryGetValue(smid, out stateMachine))
                    {
                        stateMachine = StateMachineFactory.Create(table, smid);
                        stateMachine.Action += this.StateMachine_Action;
                        stateMachine.Context = this.ParserContext;
                        this.stateMachineMap[smid] = stateMachine;
                    }

                    logger.DebugFormat("进入{0}:{1}模式", this.ParserContext.State, smid);

                    int code = stateMachine.Run();
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                }
            }
        }



        /// <summary>
        /// 当状态机触发事件的时候执行
        /// </summary>
        /// <param name="stateMachine">触发该事件的状态机实例</param>
        /// <param name="action">该状态机触发的事件</param>
        private void StateMachine_Action(StateMachine stateMachine, ParserActions action)
        {
            Console.WriteLine("采集到的OSC字符串 = {0}", stateMachine.Context.OSCString);
        }

        #endregion
    }
}

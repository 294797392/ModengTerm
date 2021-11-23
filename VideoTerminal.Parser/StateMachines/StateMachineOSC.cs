using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VideoTerminal.Parser.StateMachines
{
    /// <summary>
    /// 处理OSC状态的状态机
    /// 
    /// 代码大量参考了terminal项目stateMachine.cpp里的代码
    /// 
    ///     OSC：
    ///         名称：系统命令
    ///         格式：OSC 数字;字符串 BEL
    ///         参数说明：
    ///             BEL和ST都表示OSC的结束符，数字和字符串之间使用分号分割，可以通过判断是否读取到了分号的方式去收集数字和字符串
    ///         参考：
    ///             xterm-331 - VTPrsTbl.c 6578行
    ///             terminal - stateMachine.cpp _EventOscParam函数
    /// </summary>
    public class StateMachineOSC : StateMachine
    {
        // The DEC STD 070 reference recommends supporting up to at least 16384 for
        // parameter values, so 32767 should be more than enough. At most we might
        // want to increase this to 65535, since that is what XTerm and VTE support,
        // but for now 32767 is the safest limit for our existing code base.
        private const int MAX_PARAMETER_VALUE = 32767;

        #region 类变量

        private static log4net.ILog logger = log4net.LogManager.GetLogger("StateMachineOSC");

        #endregion

        #region 实例变量

        /// <summary>
        /// 当前是否是oscParam状态
        /// 如果没出现分隔符，那么参数类型就是数字
        /// </summary>
        private bool oscParamState = true;

        /// <summary>
        /// 当前OSC状态的参数类型是否是字符串
        /// 如果出现了分隔符，那么参数类型就是字符串
        /// </summary>
        private bool oscStringState = false;

        /// <summary>
        /// 当前是否是OSC结束状态
        /// </summary>
        private bool oscTerminationState = false;

        #endregion

        public override VTStates Run()
        {
            byte ch = this.Context.CurrentChar;

            if (this.oscParamState)
            {
                return this.ProcessParamState(ch);
            }
            else if (this.oscStringState)
            {
                return this.ProcessStringState(ch);
            }
            else if (this.oscTerminationState)
            {
                return this.ProcessTerminationState(ch);
            }

            return VTStates.OSC;
        }

        /// <summary>
        /// 当状态是ParamState的时候进行处理
        /// 该状态是进入OSC之后的第一个状态
        /// </summary>
        /// <param name="ch"></param>
        private VTStates ProcessParamState(byte ch)
        {
            // 还没出现分隔符，那么收集的参数类型是数字
            if (this.IsNumericValue(ch))
            {
                int oscParam = this.Context.OSCParameter;
                this.Context.OSCParameter = this.AccumulateTo(ch, oscParam);
            }
            // 出现了分隔符，那么说明下次就要收集字符串，把状态变成oscStringState
            else if (this.IsDelimiter(ch))
            {
                this.oscParamState = false;
                this.oscStringState = true;
            }
            else if (this.IsTerminator(ch))
            {
                // 还没出现分隔符OSC状态就结束了，什么时候会出现这个状态?
                // 这里照搬terminal的做法，直接进入Ground状态并重置上下文信息
                //this.NotifyAction(ParserActions.OSCCompleted);  // 是否需要通知事件？terminal没有分发该事件
                this.Reset();
                logger.WarnFormat("还没出现分隔符，OSC状态就结束了");
                return VTStates.Ground;
            }

            return VTStates.OSC;
        }

        /// <summary>
        /// 当出现了分隔符后进行处理
        /// 该状态是OSC的第二个状态
        /// </summary>
        /// <param name="ch"></param>
        private VTStates ProcessStringState(byte ch)
        {
            // 已经遇到了分隔符，那么收集的参数类型是字符串
            if (this.IsTerminator(ch))
            {
                // 此处OSC是以BEL字符作为结束符
                this.NotifyEvent(ParserEvents.OSCCompleted);
                this.Reset();
                return VTStates.Ground;
            }
            else if (ch == ASCIIChars.ESC)
            {
                // OSC状态下出现了ESC字符
                // 此时分两种情况：
                // 1. ESC后面紧跟着ST，那么说明OSC结束，需要执行对应的OSC操作
                // 2. ESC后面没有ST，说明是普通的ESC状态，需要进入ESC状态
                this.oscStringState = false;
                this.oscTerminationState = true;
            }
            else if (this.IsValid(ch))
            {
                // OSC状态下的无效字符，忽略
            }
            else
            {
                // 收集OSC字符串参数
                this.Context.OSCString.Append((char)ch);
            }

            return VTStates.OSC;
        }

        /// <summary>
        /// 当出现了以ST为结束符的时候的处理
        /// 该状态是OSC最后一个状态
        /// </summary>
        /// <param name="ch"></param>
        private VTStates ProcessTerminationState(byte ch)
        {
            // 这里照搬terminal的代码
            // stateMachine.cpp _EventOscTermination函数

            if (this.IsStringTerminator(ch))
            {
                // ESC后面紧跟着ST，说明要执行OSC操作，然后进入Ground状态
                this.NotifyEvent(ParserEvents.OSCCompleted);
                this.Reset();
                return VTStates.Ground;
            }
            else
            {
                // ESC后面没有ST，当做普通的ESC状态做处理
                this.Reset();

                // 此时已经是ESC状态了，需要执行一次ESC状态机的逻辑
                throw new NotImplementedException();

                return VTStates.Escape;
            }
        }

        /// <summary>
        /// 判断当前状态是否是BEL结束状态
        /// OSC有两种结束模式，一种是以BEL结束，一种是以ST结束
        /// 这里判断OSC是否是以BEL结束
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        private bool IsTerminator(byte ch)
        {
            // OSC状态下，出现BEL字符就表示状态结束
            return ch == ASCIIChars.BEL;
        }

        /// <summary>
        /// 判断当前的状态是否是OSC的String Terminator
        /// OSC有两种结束模式，一种是以BEL结束，一种是以ST结束
        /// 这里判断OSC是否是以ST结束
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        private bool IsStringTerminator(byte ch)
        {
            return ch == '\\';
        }

        /// <summary>
        /// 判断该字符是否是OSC的数字参数
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        private bool IsNumericValue(byte ch)
        {
            return ch >= 0x30 && ch <= 0x39;
        }

        /// <summary>
        /// 判断该字符是否是OSC分隔符
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        private bool IsDelimiter(byte ch)
        {
            return ch == ';';
        }

        /// <summary>
        /// 判断该字符是否是有效的OSC参数
        /// </summary>
        /// <param name="ch"></param>
        /// <returns>
        /// true表示无效字符，直接忽略
        /// false表示有效字符，要处理
        /// </returns>
        private bool IsValid(byte ch)
        {
            return ch <= '\x17' || ch == '\x19' || (ch >= '\x1c' && ch <= '\x1f');
        }

        /// <summary>
        /// 重置该状态机的状态
        /// </summary>
        /// <param name="vtState">状态机的下一个状态</param>
        private void Reset()
        {
            this.Context.Reset();

            this.oscParamState = true;
            this.oscStringState = false;
            this.oscTerminationState = false;
        }

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
    }
}

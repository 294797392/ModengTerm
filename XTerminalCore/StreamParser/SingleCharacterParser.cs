using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using XTerminalCore.Invocations;

namespace XTerminalCore
{
    /// <summary>
    /// 单字符的ControlFunction：
    /// Bell (Ctrl-G).
    /// BS - Backspace(Ctrl-H).
    /// CR - Carriage Return(Ctrl-M).
    /// ENQ - Return Terminal Status(Ctrl-E).  Default response is an empty string, but may be overridden by a resource answerbackString.
    /// FF - Form Feed or New Page (NP).  (FF  is Ctrl-L).  FF  is treated the same as LF.
    /// LF - Line Feed or New Line(NL).  (LF  is Ctrl-J).
    /// SI - Shift In(Ctrl-O) -> Switch to Standard Character Set.This invokes the G0 character set(the default).
    /// SO - Shift Out(Ctrl-N) -> Switch to Alternate Character Set.This invokes the G1 character set.
    /// SP - Space.
    /// TAB - Horizontal Tab (HT) (Ctrl-I).
    /// VT - Vertical Tab(Ctrl-K).  This is treated the same as LF.
    /// </summary>
    public class SingleCharacterParser : IControlFunctionParser
    {
        public static SingleCharacterParser Instance = new SingleCharacterParser();

        private SingleCharacterParser()
        {
        }

        public override bool Parse(StreamReader reader, out IFormattedCf controlFunc)
        {
            controlFunc = null;
            return true;
        }

        /// <summary>
        /// ControlFunction -> Invocation
        /// </summary>
        //private static Dictionary<byte, IInvocation> FunctionContentMap = new Dictionary<byte, IInvocation>();

        //public override bool Parse(byte[] chars, int cfIndex, IFormattedCf controlFunc)
        //{
        //    if (!FunctionContentMap.TryGetValue(chars[cfIndex], out invocation))
        //    {
        //        SingleCharacterInvocation scInvocation = new SingleCharacterInvocation();
        //        scInvocation.Action = chars[cfIndex];
        //        FunctionContentMap[chars[cfIndex]] = scInvocation;
        //        invocation = scInvocation;
        //    }

        //    dataSize = 1;

        //    return true;
        //}
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using XTerminalCore.Invocations;

namespace XTerminalCore
{
    public abstract class IControlFunctionParser
    {
        //public abstract byte ControlFunction { get; }

        /// <summary>
        /// 从数据流中解析出ControlFunction的所有内容
        /// </summary>
        /// <param name="reader">要解析的数据流</param>
        /// <param name="controlFunc">结构化的ControlFunction</param>
        /// <returns></returns>
        public abstract bool Parse(StreamReader reader, out IFormattedCf controlFunc);
    }
}
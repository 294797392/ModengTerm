using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XTerminalCore.Invocations;

namespace XTerminalCore
{
    public abstract class IControlFunctionParser
    {
        /// <summary>
        /// 从一串字符中解析出ControlFunction的所有内容
        /// </summary>
        /// <param name="chars"></param>
        /// <param name="cfIndex">ControlFunction Code的索引位置</param>
        /// <param name="controlFunc">结构化的ControlFunction</param>
        /// <returns></returns>
        public abstract bool Parse(byte[] chars, int cfIndex, out IFormattedCf controlFunc);
    }
}
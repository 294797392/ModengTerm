using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AsciiControlFunctions
{
    public struct ControlFunctionParserResult
    {
        /// <summary>
        /// ControlFunction Code
        /// </summary>
        public byte ControlFunction;

        /// <summary>
        /// 一段完整的ControlFunction的内容
        /// </summary>
        public byte[] FunctionContent;

        /// <summary>
        /// Fe类型
        /// </summary>
        public byte FeChar;
    }
}
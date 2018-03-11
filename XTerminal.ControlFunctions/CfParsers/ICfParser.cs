using AsciiControlFunctions.CfInvocations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AsciiControlFunctions
{
    public abstract class ICfParser
    {
        /// <summary>
        /// 从一串字符中解析出ControlFunction的所有内容
        /// </summary>
        /// <param name="chars"></param>
        /// <param name="cfIndex">ControlFunction的索引位置</param>
        /// <param name="result"></param>
        /// <param name="dataSize">IFormattedCf数据结构的大小</param>
        /// <returns></returns>
        public abstract bool Parse(byte[] chars, int cfIndex, out ICfInvocation invocation, out int dataSize);
    }
}
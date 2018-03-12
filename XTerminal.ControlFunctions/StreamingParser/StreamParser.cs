using ControlFunctions.CfInvocations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlFunctions.StreamingParser
{
    /// <summary>
    /// 终端数据流解析器
    /// </summary>
    public class StreamParser
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("StreamParser");

        /// <summary>
        /// 从一串字符中解析出所有ControlFunction从开头到结尾的字符
        /// 7位编码和8位编码通用
        /// </summary>
        /// <param name="chars">要解析的字符</param>
        /// <param name="functionBytes">解析出来的所有字符</param>
        /// <returns></returns>
        public static bool Parse(byte[] chars, out List<ICfInvocation> invocations)
        {
            invocations = new List<ICfInvocation>();

            int length = chars.Length;

            for (int idx = 0; idx < length; idx++)
            {
                byte c = chars[idx];
                ICfParser parser;
                if (ControlFunctions.IsControlFunction(c, out parser))
                {
                    if (parser == null)
                    {
                        throw new NotImplementedException(string.Format("未实现ControlFunction'{0}'的解析器", c));
                    }

                    int dataSize;
                    ICfInvocation invocation;
                    if (!parser.Parse(chars, idx, out invocation, out dataSize))
                    {
                        logger.ErrorFormat("解析ControlFunction'{0}'失败", c);
                        return false;
                    }
                    logger.InfoFormat("解析成功:{0}", invocation);
                    idx += dataSize - 1;
                    invocations.Add(invocation);
                }
            }

            return true;
        }
    }
}
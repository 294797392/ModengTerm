using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XTerminalCore.InvocationConverting;
using XTerminalCore.Invocations;

namespace XTerminalCore
{
    /// <summary>
    /// 终端数据流解析器
    /// </summary>
    public class OutputStreamParser
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("OutputStreamParser");

        public event Action<char> CharParsed;
        public event Action<IInvocation> InvocationParsed;

        private IInvocationConverter InvocationConverter = new XtermInvocationConverter();

        /// <summary>
        /// 从一串字符中解析出所有ControlFunction从开头到结尾的字符
        /// 7位编码和8位编码通用
        /// </summary>
        /// <param name="chars">要解析的字符</param>
        /// <returns></returns>
        public bool Parse(byte[] chars)
        {
            int length = chars.Length;

            for (int idx = 0; idx < length; idx++)
            {
                byte c = chars[idx];
                IControlFunctionParser parser;
                if (ControlFunctions.IsControlFunction(c, out parser))
                {
                    if (parser == null)
                    {
                        throw new NotImplementedException(string.Format("未实现ControlFunction'{0}'的解析器", c));
                    }

                    IInvocation currentInvocation;
                    if (parser == SingleCharacterParser.Instance)
                    {
                        // 单字节ControlFunction
                        SingleCharacterInvocation scInvocation;
                        scInvocation.Action = chars[idx];
                        currentInvocation = scInvocation;
                    }
                    else
                    {
                        IFormattedCf controlFunc;
                        if (!parser.Parse(chars, idx, out controlFunc))
                        {
                            logger.ErrorFormat("解析ControlFunction'{0}'失败", c);
                            return false;
                        }
                        logger.InfoFormat("解析成功:{0}", controlFunc);

                        if (!this.InvocationConverter.Convert(controlFunc, out currentInvocation))
                        {
                            logger.ErrorFormat("ControlFunc转Invocation失败");
                            return false;
                        }
                        idx += controlFunc.GetSize() - 1;
                    }
                    if (this.InvocationParsed != null)
                    {
                        this.InvocationParsed(currentInvocation);
                    }
                }
                else
                {
                    if (this.CharParsed != null)
                    {
                        this.CharParsed((char)c);
                    }
                }
            }

            return true;
        }
    }
}
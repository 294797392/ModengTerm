using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XTerminalCore.InvocationConverting;
using XTerminalCore.Invocations;

namespace XTerminalCore
{
    /// <summary>
    /// 解析Esc ControlFunction数据
    /// </summary>
    public class EscapeParser : IControlFunctionParser
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("EscapeParser");

        /// <summary>
        /// Fe Code -> FeParser
        /// </summary>
        private static Dictionary<byte, FeParser> FeParserMap = new Dictionary<byte, FeParser>()
        {
            { Fe.CSI_7BIT, new CSIParser() },
            { Fe.OSC_7BIT, new OSCParser() },
        };

        public override bool Parse(byte[] chars, int cfIndex, out IFormattedCf controlFunc)
        {
            FeParser feParser;
            if (!FeParserMap.TryGetValue(chars[cfIndex + 1], out feParser))
            {
                throw new NotImplementedException(string.Format("未实现Fe:{0}的Parser", chars[cfIndex + 1]));
            }

            byte[] buffer = new byte[chars.Length - cfIndex - 1];
            Buffer.BlockCopy(chars, cfIndex + 1, buffer, 0, buffer.Length);

            if (!feParser.Parse(buffer, out controlFunc))
            {
                logger.ErrorFormat("转换IFormattedCf失败, Fe:{0}", chars[cfIndex + 1]);
                return false;
            }

            return true;
        }
    }
}
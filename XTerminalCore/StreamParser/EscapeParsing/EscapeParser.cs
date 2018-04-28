using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using XTerminalCore.InvocationConverting;
using XTerminalCore.Invocations;

namespace XTerminalCore
{
    /// <summary>
    /// 解析Esc ControlFunction
    /// </summary>
    public class EscapeParser : IControlFunctionParser
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("EscapeParser");

        /// <summary>
        /// Fe -> FeParser
        /// </summary>
        private static Dictionary<byte, FeParser> FeParserMap = new Dictionary<byte, FeParser>()
        {
            { Fe.CSI_7BIT, new CSIParser() },
            { Fe.OSC_7BIT, new OSCParser() },
        };

        public override bool Parse(StreamReader reader, out IFormattedCf controlFunc)
        {
            byte c = (byte)reader.Read();
            FeParser feParser;
            if (!FeParserMap.TryGetValue(c, out feParser))
            {
                throw new NotImplementedException(string.Format("未实现Fe:{0}的Parser", c));
            }

            if (!feParser.Parse(reader, out controlFunc))
            {
                logger.ErrorFormat("解析IFormattedCf失败, Fe:{0}", c);
                return false;
            }

            return true;
        }
    }
}
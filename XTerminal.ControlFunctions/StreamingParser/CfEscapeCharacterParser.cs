using ControlFunctions.FeParsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ControlFunctions.CfInvocations;
using ControlFunctions.CfInvocationConverters;

namespace ControlFunctions
{
    /// <summary>
    /// 解析Esc ControlFunction数据
    /// </summary>
    public class CfEscapeCharacterParser : ICfParser
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("EscControlFunctionParser");

        private IInvocationConverter InvocationConverter = new XtermInvocationConverter();

        /// <summary>
        /// Fe Code -> FeParser
        /// </summary>
        private static Dictionary<byte, FeParser> FeParserMap = new Dictionary<byte, FeParser>()
        {
            { Fe.CSI_7BIT, new CSIParser() },
            { Fe.OSC_7BIT, new OSCParser() },
        };

        public override bool Parse(byte[] chars, int cfIndex, out ICfInvocation invocation, out int dataSize)
        {
            invocation = null;
            dataSize = 0;

            FeParser feParser;
            if (!FeParserMap.TryGetValue(chars[cfIndex + 1], out feParser))
            {
                throw new NotImplementedException(string.Format("未实现Fe:{0}的Parser", chars[cfIndex + 1]));
            }

            byte[] buffer = new byte[chars.Length - cfIndex - 1];
            Buffer.BlockCopy(chars, cfIndex + 1, buffer, 0, buffer.Length);

            IFormattedCf formattedCf;
            if (!feParser.Parse(buffer, out formattedCf))
            {
                logger.ErrorFormat("转换IFormattedCf失败, Fe:{0}", chars[cfIndex + 1]);
                return false;
            }

            if (!this.InvocationConverter.Convert(formattedCf, out invocation))
            {
                logger.ErrorFormat("转换CfInvocation失败, Fe:{0}", chars[cfIndex + 1]);
                return false;
            }

            dataSize = formattedCf.GetSize();

            return true;
        }
    }
}
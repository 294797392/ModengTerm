using AsciiControlFunctions.FeParsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AsciiControlFunctions.CfInvocations;

namespace AsciiControlFunctions
{
    /// <summary>
    /// 解析Esc ControlFunction数据
    /// </summary>
    public class CfEscapeCharacterParser : ICfParser
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("EscControlFunctionParser");

        //public override bool Parse(byte[] chars, int offset, out ControlFunctionParserResult result, out int funcEndIdx)
        //{
        //    result.FeChar = 0;
        //    result.FunctionContent = null;
        //    result.ControlFunction = 0;
        //    funcEndIdx = 0;
        //    int length = chars.Length;
        //    byte fe = chars[offset + 1];

        //    if (!Fe.Is7bitFe(fe))
        //    {
        //        return false;
        //    }

        //    if (fe == Fe.CSI_7BIT)
        //    {
        //        #region 解析CSI命令

        //        if (fe == Fe.CSI_7BIT)
        //        {
        //            int charsSize = 0;
        //            bool privateUse = false;
        //            bool withIntermediateByte0200 = false;
        //            for (int idx = offset + 1; idx < length; idx++)
        //            {
        //                charsSize++;
        //                if (FinalByte.IsFinalByte(chars[idx], out privateUse, out withIntermediateByte0200))
        //                {
        //                    result.FeChar = fe;
        //                    result.FunctionContent = new byte[charsSize];
        //                    Array.Copy(chars, offset + 1, result.FunctionContent, 0, result.FunctionContent.Length);
        //                    funcEndIdx = idx;
        //                    return true;
        //                }
        //            }
        //        }

        //        #endregion
        //    }
        //    else if (fe == Fe.OSC_7BIT)
        //    {
        //        #region 解析OSC命令

        //        int charsSize = 0;
        //        for (int idx = offset + 1; idx < length; idx++)
        //        {
        //            charsSize++;
        //            if (OSCStructures.IsTerminatedChar(chars[idx]))
        //            {
        //                result.FeChar = fe;
        //                result.FunctionContent = new byte[charsSize];
        //                Array.Copy(chars, offset + 1, result.FunctionContent, 0, result.FunctionContent.Length);
        //                funcEndIdx = idx;
        //                return true;
        //            }
        //        }

        //        #endregion
        //    }

        //    logger.ErrorFormat("解析7位编码Fe'{0}'失败", fe);

        //    return false;
        //}

        public override bool Parse(byte[] chars, int offset, out ICfInvocation result, out int funcEndIdx)
        {
            throw new NotImplementedException();
        }
    }
}
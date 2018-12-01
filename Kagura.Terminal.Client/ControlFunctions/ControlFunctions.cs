using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XTerminal.ControlFunctions.CSI;

namespace XTerminal.ControlFunctions
{
    #region 弃用注释，请不要相信这里所说的，没删掉是因为要做一个记录

    ///     由01/11开头，后面跟一个Fe，Fe可以看作成命令类型，7位编码的ASCII码和8位编码的ASCII码使用的Fe码的范围不同，但是每个Fe所表示的意义都是相同的
    ///     对于7位编码的ASCII码：
    ///         1.Fe的范围在04/00（64） - 05/15（95）之间
    ///     对于8位编码的ASCII码：
    ///         1.Fe的范围在08/00（128） - 09/15（159）之间
    /// 

    #endregion

    /// <summary>
    /// 根据Control Functions for Coded Character Sets Ecma-048.pdf标准，
    /// 这是一个解释ASCII转义字符所表示的意义的标准
    /// 这是一个ECMA定义的标准，所有的终端都会实现这个标准
    /// 参考：
    ///     Dependencies/Control Functions for Coded Character Sets Ecma-048.pdf
    ///     
    /// 控制序列：
    ///     对于7位编码的ascii码：
    ///         由两个字符开头。第一个字符是01/11，第二个是Fe。Fe可以看作成命令类型，Fe的范围在04/00（64） - 05/15（95）之间
    ///     对于8位编码的ascii码：
    ///         由一个字符开头。范围在08/00 - 09/15之间
    /// 
    /// 完整的控制序列格式是：
    ///     对于7位编码的ascii码：ControlFunction Fe [ [Fe参数] [Fe结束符] ]
    ///     对于8位编码的ascii码：ControlFunction [ [参数] [结束符] ]，注意，8位编码的ascii码中，Fe就是ControlFunction
    /// 
    /// 一般情况下，使用7位ascii编码的主机返回的ControlFunctions和Fe百分之九十都是ESC和CSI，这里解释一下CSI的含义，这个解释同样可以用作于8位ascii编码的主机
    /// 
    /// 一些Fe的功能说明：
    ///     CSI：
    ///         名称：控制序列
    ///         格式：CSI P..P I..I F
    ///         参数说明：
    ///             CSI：控制序列字符，在7位编码中，由01/11 05/11两个字符组成；在8位编码中，由09/11单个字符组成
    ///             P..P：参数字符串，由03/00（48） - 03/15（63）之间的字符组成
    ///             I..I：中间字符串，由02/00（32） - 02/15（47）之间的字符组成，后面会跟一个字符串终结字符（F）
    ///             F：结束字符，由04/00（64） - 07/14（126）之间的某个字符表示。07/00 - 07/14之间的字符也是结束符，但是这是留给厂商做实验使用的。注意，带有中间字符串和不带有中间字符串的结束符的含义不一样
    /// </summary>
    public static class ControlFunctions
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("ControlFunctions");

        #region C0 set

        public static byte NUL = CharacterUtils.BitCombinations(00, 00);
        public static byte SOH = CharacterUtils.BitCombinations(00, 01);
        public static byte STX = CharacterUtils.BitCombinations(00, 02);
        public static byte ETX = CharacterUtils.BitCombinations(00, 03);
        public static byte EOT = CharacterUtils.BitCombinations(00, 04);
        public static byte ENQ = CharacterUtils.BitCombinations(00, 05);
        public static byte ACK = CharacterUtils.BitCombinations(00, 06);
        /// <summary>
        /// BEL is used when there is a need to call for attention; it may control alarm or attention devices
        /// </summary>
        public static byte BEL = CharacterUtils.BitCombinations(00, 07);
        public static byte BS = CharacterUtils.BitCombinations(00, 08);
        public static byte HT = CharacterUtils.BitCombinations(00, 09);
        /// <summary>
        /// 换行，使用新行
        /// </summary>
        public static byte LF = CharacterUtils.BitCombinations(00, 10);
        public static byte VT = CharacterUtils.BitCombinations(00, 11);
        public static byte FF = CharacterUtils.BitCombinations(00, 12);
        /// <summary>
        /// 回车，把光标移动到一行的最前面
        /// </summary>
        public static byte CR = CharacterUtils.BitCombinations(00, 13);
        public static byte SOorLS1 = CharacterUtils.BitCombinations(00, 14);
        public static byte S1orLS0 = CharacterUtils.BitCombinations(00, 15);

        public static byte DLE = CharacterUtils.BitCombinations(01, 00);
        public static byte DC1 = CharacterUtils.BitCombinations(01, 01);
        public static byte DC2 = CharacterUtils.BitCombinations(01, 02);
        public static byte DC3 = CharacterUtils.BitCombinations(01, 03);
        public static byte DC4 = CharacterUtils.BitCombinations(01, 04);
        public static byte NAK = CharacterUtils.BitCombinations(01, 05);
        public static byte SYN = CharacterUtils.BitCombinations(01, 06);
        public static byte ETB = CharacterUtils.BitCombinations(01, 07);
        public static byte CAN = CharacterUtils.BitCombinations(01, 08);
        public static byte EM = CharacterUtils.BitCombinations(01, 09);
        public static byte SUB = CharacterUtils.BitCombinations(01, 10);
        public static byte ESC = CharacterUtils.BitCombinations(01, 11);
        public static byte IS4 = CharacterUtils.BitCombinations(01, 12);
        public static byte IS3 = CharacterUtils.BitCombinations(01, 13);
        public static byte IS2 = CharacterUtils.BitCombinations(01, 14);
        public static byte IS1 = CharacterUtils.BitCombinations(01, 15);

        #endregion

        #region 针对于8位编码的ascii码

        #endregion

        /// <summary>
        /// ControlFunctionCode -> ControlFunctionParser
        /// </summary>
        private static Dictionary<byte, ControlFunctionParser> _7bitControlFunctionMap = new Dictionary<byte, ControlFunctionParser>()
        {
            { ControlFunctions.NUL, null },
            { ControlFunctions.SOH, null },
            { ControlFunctions.STX, null },
            { ControlFunctions.ETX, null },
            { ControlFunctions.EOT, null },
            { ControlFunctions.ENQ, null },
            { ControlFunctions.ACK, null },
            { ControlFunctions.BEL, new SingleCharacterControlFunctionParser() },
            { ControlFunctions.BS, null },
            { ControlFunctions.HT, null },
            { ControlFunctions.LF, new SingleCharacterControlFunctionParser() },
            { ControlFunctions.VT, null },
            { ControlFunctions.FF, null },
            { ControlFunctions.CR, new SingleCharacterControlFunctionParser() },
            { ControlFunctions.SOorLS1, null },
            { ControlFunctions.S1orLS0, null },
            { ControlFunctions.DLE, null },
            { ControlFunctions.DC1, null },
            { ControlFunctions.DC2, null },
            { ControlFunctions.DC3, null },
            { ControlFunctions.DC4, null },
            { ControlFunctions.NAK, null },
            { ControlFunctions.SYN, null },
            { ControlFunctions.ETB, null },
            { ControlFunctions.CAN, null },
            { ControlFunctions.EM, null },
            { ControlFunctions.SUB, null },
            { ControlFunctions.ESC, new EscControlFunctionParser() },
            { ControlFunctions.IS4, null },
            { ControlFunctions.IS3, null },
            { ControlFunctions.IS2, null },
            { ControlFunctions.IS1, null },
        };

        public static bool IsControlFunction(byte c)
        {
            return _7bitControlFunctionMap.ContainsKey(c);
        }

        /// <summary>
        /// 从一串字符中解析出所有ControlFunction从开头到结尾的字符
        /// 7位编码和8位编码通用
        /// </summary>
        /// <param name="chars">要解析的字符</param>
        /// <param name="functionBytes">解析出来的所有字符</param>
        /// <returns></returns>
        public static bool Parse(byte[] chars, out List<ControlFunctionParserResult> results)
        {
            results = new List<ControlFunctionParserResult>();
            int length = chars.Length;

            for (int idx = 0; idx < length; idx++)
            {
                byte c = chars[idx];
                ControlFunctionParser parser;
                if (_7bitControlFunctionMap.TryGetValue(c, out parser))
                {
                    if (parser == null)
                    {
                        throw new NotImplementedException(string.Format("未实现ControlFunction'{0}'的解析器", c));
                    }

                    int endIdx;
                    ControlFunctionParserResult result;
                    if (!parser.Parse(chars, idx, out result, out endIdx))
                    {
                        logger.ErrorFormat("解析ControlFunction'{0}'失败", c);
                        return false;
                    }
                    idx = endIdx;
                    results.Add(result);
                }
            }

            return true;
        }
    }

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

    /// <summary>
    /// 解析从ControlFunction开头到结尾的所有字符
    /// </summary>
    public abstract class ControlFunctionParser
    {
        /// <summary>
        /// 从一串字符中解析出ControlFunction的所有内容
        /// </summary>
        /// <param name="chars"></param>
        /// <param name="offset">要解析的数据的偏移位置，从offset处开始解析，第offset位是ControlFunction</param>
        /// <param name="result"></param>
        /// <param name="funcEndIdx">ControlFunction在chars里的最后一个字符的索引</param>
        /// <returns></returns>
        public abstract bool Parse(byte[] chars, int offset, out ControlFunctionParserResult result, out int funcEndIdx);
    }

    /// <summary>
    /// 解析Esc ControlFunction数据
    /// </summary>
    public class EscControlFunctionParser : ControlFunctionParser
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("EscControlFunctionParser");

        public override bool Parse(byte[] chars, int offset, out ControlFunctionParserResult result, out int funcEndIdx)
        {
            result.FeChar = 0;
            result.FunctionContent = null;
            result.ControlFunction = 0;
            funcEndIdx = 0;
            int length = chars.Length;
            byte fe = chars[offset + 1];

            if (!Fe.Is7bitFe(fe))
            {
                return false;
            }

            if (fe == Fe.CSI_7BIT)
            {
                #region 解析CSI命令

                if (fe == Fe.CSI_7BIT)
                {
                    int charsSize = 0;
                    bool privateUse = false;
                    bool withIntermediateByte0200 = false;
                    for (int idx = offset + 1; idx < length; idx++)
                    {
                        charsSize++;
                        if (FinalByte.IsFinalByte(chars[idx], out privateUse, out withIntermediateByte0200))
                        {
                            result.FeChar = fe;
                            result.FunctionContent = new byte[charsSize];
                            Array.Copy(chars, offset + 1, result.FunctionContent, 0, result.FunctionContent.Length);
                            funcEndIdx = idx;
                            return true;
                        }
                    }
                }

                #endregion
            }
            else if (fe == Fe.OSC_7BIT)
            {
                #region 解析OSC命令

                int charsSize = 0;
                for (int idx = offset + 1; idx < length; idx++)
                {
                    charsSize++;
                    if (OSC.IsTerminatedChar(chars[idx]))
                    {
                        result.FeChar = fe;
                        result.FunctionContent = new byte[charsSize];
                        Array.Copy(chars, offset + 1, result.FunctionContent, 0, result.FunctionContent.Length);
                        funcEndIdx = idx;
                        return true;
                    }
                }

                #endregion
            }

            logger.ErrorFormat("解析7位编码Fe'{0}'失败", fe);

            return false;
        }
    }

    /// <summary>
    /// 单字符的ControlFunction：
    /// Bell (Ctrl-G).
    /// BS - Backspace(Ctrl-H).
    /// CR - Carriage Return(Ctrl-M).
    /// ENQ - Return Terminal Status(Ctrl-E).  Default response is an empty string, but may be overridden by a resource answerbackString.
    /// FF - Form Feed or New Page (NP).  (FF  is Ctrl-L).  FF  is treated the same as LF.
    /// LF - Line Feed or New Line(NL).  (LF  is Ctrl-J).
    /// SI - Shift In(Ctrl-O) -> Switch to Standard Character Set.This invokes the G0 character set(the default).
    /// SO - Shift Out(Ctrl-N) -> Switch to Alternate Character Set.This invokes the G1 character set.
    /// SP - Space.
    /// TAB - Horizontal Tab (HT) (Ctrl-I).
    /// VT - Vertical Tab(Ctrl-K).  This is treated the same as LF.
    /// </summary>
    public class SingleCharacterControlFunctionParser : ControlFunctionParser
    {
        /// <summary>
        /// ControlFunction -> FunctionContent Map
        /// </summary>
        private static Dictionary<byte, byte[]> FunctionContentMap = new Dictionary<byte, byte[]>();

        public override bool Parse(byte[] chars, int offset, out ControlFunctionParserResult result, out int funcEndIdx)
        {
            result.ControlFunction = chars[offset];
            result.FeChar = 0;
            if (!FunctionContentMap.TryGetValue(chars[offset], out result.FunctionContent))
            {
                result.FunctionContent = new byte[] { chars[offset] };
                FunctionContentMap[chars[offset]] = new byte[] { chars[offset] };
            }
            funcEndIdx = offset;
            return true;
        }
    }
}
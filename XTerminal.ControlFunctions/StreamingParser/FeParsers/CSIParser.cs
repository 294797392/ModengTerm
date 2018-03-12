using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ControlFunctions.CfInvocations;

namespace ControlFunctions.FeParsers
{
    // CSI是Fe的一种

    /// <summary>
    /// 参数字节
    /// which, if present, consist of bit combinations from 03/00 to 03/15;
    /// </summary>
    public struct ParameterBytes
    {
        public static readonly byte MinimumValue = CharacterUtils.BitCombinations(03, 00);
        public static readonly byte MaximumValue = CharacterUtils.BitCombinations(03, 15);

        public byte[] Content;

        public static bool IsParameterByte(byte c)
        {
            return c >= MinimumValue && c <= MaximumValue;
        }
    }

    /// <summary>
    /// 中间字节
    /// which, if present, consist of bit combinations from 02/00 to 02/15. 
    /// Together with the Final Byte F, they identify the control function
    /// </summary>
    public struct IntermediateBytes
    {
        public static readonly byte MinimumValue = CharacterUtils.BitCombinations(02, 00);
        public static readonly byte MaximumValue = CharacterUtils.BitCombinations(02, 15);

        public byte[] Content;

        public static bool IsIntermediateByte(byte c)
        {
            return c >= MinimumValue && c <= MaximumValue;
        }
    }

    /// <summary>
    /// 结束字节
    /// 定义了控制序列（CSI）的结束符
    /// 
    /// it consists of a bit combination from 04/00 to 07/14; it terminates the control 
    /// combinations 07/00 to 07/14 are available as Final Bytes of control sequences for private (or
    /// experimental) use.
    /// </summary>
    public struct FinalByte
    {
        #region Without Intermediate Bytes 

        public static byte ICH = CharacterUtils.BitCombinations(04, 00);
        /// <summary>
        /// 鼠标向上移动
        /// Cursor UP
        /// </summary>
        public static byte CUU = CharacterUtils.BitCombinations(04, 01);
        /// <summary>
        /// 鼠标向下移动
        /// Cursor Down
        /// </summary>
        public static byte CUD = CharacterUtils.BitCombinations(04, 02);
        /// <summary>
        /// 鼠标向右
        /// Cursor Forward
        /// </summary>
        public static byte CUF = CharacterUtils.BitCombinations(04, 03);
        /// <summary>
        /// 鼠标向左
        /// Cursor Back
        /// </summary>
        public static byte CUB = CharacterUtils.BitCombinations(04, 04);
        public static byte CNL = CharacterUtils.BitCombinations(04, 05);
        public static byte CPL = CharacterUtils.BitCombinations(04, 06);
        public static byte CHA = CharacterUtils.BitCombinations(04, 07);
        public static byte CUP = CharacterUtils.BitCombinations(04, 08);
        public static byte CHT = CharacterUtils.BitCombinations(04, 09);
        public static byte ED = CharacterUtils.BitCombinations(04, 10);
        public static byte EL = CharacterUtils.BitCombinations(04, 11);
        public static byte IL = CharacterUtils.BitCombinations(04, 12);
        public static byte DL = CharacterUtils.BitCombinations(04, 13);
        public static byte EF = CharacterUtils.BitCombinations(04, 14);
        public static byte EA = CharacterUtils.BitCombinations(04, 15);

        public static byte DCH = CharacterUtils.BitCombinations(05, 00);
        public static byte SSE = CharacterUtils.BitCombinations(05, 01);
        public static byte CPR = CharacterUtils.BitCombinations(05, 02);
        public static byte SU = CharacterUtils.BitCombinations(05, 03);
        public static byte SD = CharacterUtils.BitCombinations(05, 04);
        public static byte NP = CharacterUtils.BitCombinations(05, 05);
        public static byte PP = CharacterUtils.BitCombinations(05, 06);
        public static byte CTC = CharacterUtils.BitCombinations(05, 07);
        public static byte ECH = CharacterUtils.BitCombinations(05, 08);
        public static byte CVT = CharacterUtils.BitCombinations(05, 09);
        public static byte CBT = CharacterUtils.BitCombinations(05, 10);
        /// <summary>
        /// START REVERSED STRING
        /// </summary>
        public static byte SRS = CharacterUtils.BitCombinations(05, 11);
        public static byte PTX = CharacterUtils.BitCombinations(05, 12);
        public static byte SDS = CharacterUtils.BitCombinations(05, 13);
        public static byte SIMD = CharacterUtils.BitCombinations(05, 14);

        public static byte HPA = CharacterUtils.BitCombinations(06, 00);
        public static byte HPR = CharacterUtils.BitCombinations(06, 01);
        public static byte REP = CharacterUtils.BitCombinations(06, 02);
        public static byte DA = CharacterUtils.BitCombinations(06, 03);
        public static byte VPA = CharacterUtils.BitCombinations(06, 04);
        /// <summary>
        /// LINE POSITION FORWARD 
        /// </summary>
        public static byte VPR = CharacterUtils.BitCombinations(06, 05);
        public static byte HVP = CharacterUtils.BitCombinations(06, 06);
        /// <summary>
        /// TABULATION CLEAR 
        /// </summary>
        public static byte TBC = CharacterUtils.BitCombinations(06, 07);
        /// <summary>
        /// SET MODE
        /// </summary>
        public static byte SM = CharacterUtils.BitCombinations(06, 08);
        public static byte MC = CharacterUtils.BitCombinations(06, 09);
        public static byte HPB = CharacterUtils.BitCombinations(06, 10);
        public static byte VPB = CharacterUtils.BitCombinations(06, 11);
        public static byte RM = CharacterUtils.BitCombinations(06, 12);
        /// <summary>
        /// SELECT GRAPHIC RENDITION
        /// </summary>
        public static byte SGR = CharacterUtils.BitCombinations(06, 13);
        public static byte DSR = CharacterUtils.BitCombinations(06, 14);
        public static byte DAQ = CharacterUtils.BitCombinations(06, 15);

        #endregion

        #region With single Intermediate Bytes 

        public static byte SL = CharacterUtils.BitCombinations(04, 00);
        public static byte SR = CharacterUtils.BitCombinations(04, 01);
        public static byte GSM = CharacterUtils.BitCombinations(04, 02);
        public static byte GSS = CharacterUtils.BitCombinations(04, 03);
        public static byte FNT = CharacterUtils.BitCombinations(04, 04);
        public static byte TSS = CharacterUtils.BitCombinations(04, 05);
        public static byte JFY = CharacterUtils.BitCombinations(04, 06);
        public static byte SPI = CharacterUtils.BitCombinations(04, 07);
        public static byte QUAD = CharacterUtils.BitCombinations(04, 08);
        public static byte SSU = CharacterUtils.BitCombinations(04, 09);
        public static byte PFS = CharacterUtils.BitCombinations(04, 10);
        public static byte SHS = CharacterUtils.BitCombinations(04, 11);
        public static byte SVS = CharacterUtils.BitCombinations(04, 12);
        public static byte IGS = CharacterUtils.BitCombinations(04, 13);
        public static byte IDCS = CharacterUtils.BitCombinations(04, 15);

        public static byte PPA = CharacterUtils.BitCombinations(05, 00);
        public static byte PPR = CharacterUtils.BitCombinations(05, 01);
        public static byte PPB = CharacterUtils.BitCombinations(05, 02);
        public static byte SPD = CharacterUtils.BitCombinations(05, 03);
        public static byte DTA = CharacterUtils.BitCombinations(05, 04);
        public static byte SHL = CharacterUtils.BitCombinations(05, 05);
        public static byte SLL = CharacterUtils.BitCombinations(05, 06);
        public static byte FNK = CharacterUtils.BitCombinations(05, 07);
        public static byte SPQR = CharacterUtils.BitCombinations(05, 08);
        public static byte SEF = CharacterUtils.BitCombinations(05, 09);
        public static byte PEC = CharacterUtils.BitCombinations(05, 10);
        public static byte SSW = CharacterUtils.BitCombinations(05, 11);
        public static byte SACS = CharacterUtils.BitCombinations(05, 12);
        public static byte SAPV = CharacterUtils.BitCombinations(05, 13);
        public static byte STAB = CharacterUtils.BitCombinations(05, 14);
        public static byte GCC = CharacterUtils.BitCombinations(05, 15);


        public static byte TATE = CharacterUtils.BitCombinations(06, 00);
        public static byte TALE = CharacterUtils.BitCombinations(06, 01);
        public static byte TAC = CharacterUtils.BitCombinations(06, 02);
        public static byte TCC = CharacterUtils.BitCombinations(06, 03);
        public static byte TSR = CharacterUtils.BitCombinations(06, 04);
        public static byte SCO = CharacterUtils.BitCombinations(06, 05);
        public static byte SRCS = CharacterUtils.BitCombinations(06, 06);
        public static byte SCS = CharacterUtils.BitCombinations(06, 07);
        public static byte SLS = CharacterUtils.BitCombinations(06, 08);
        public static byte SCP = CharacterUtils.BitCombinations(06, 11);

        #endregion

        public static byte MinimumValue = CharacterUtils.BitCombinations(04, 00);
        public static byte MaximumValue = CharacterUtils.BitCombinations(06, 15);
        public static byte PrivateUse_MinimumValue = CharacterUtils.BitCombinations(07, 00);
        public static byte PrivateUse_MaximumValue = CharacterUtils.BitCombinations(07, 14);

        private static Dictionary<byte, byte> FinalByteMap = new Dictionary<byte, byte>()
            {
                { FinalByte.ICH, FinalByte.ICH },
                { FinalByte.CUU, FinalByte.ICH },
                { FinalByte.CUD, FinalByte.ICH },
                { FinalByte.CUF, FinalByte.ICH },
                { FinalByte.CUB, FinalByte.ICH },
                { FinalByte.CNL, FinalByte.ICH },
                { FinalByte.CPL, FinalByte.ICH },
                { FinalByte.CHA, FinalByte.ICH },
                { FinalByte.CUP, FinalByte.ICH },
                { FinalByte.CHT, FinalByte.ICH },
                { FinalByte.ED, FinalByte.ICH },
                { FinalByte.EL, FinalByte.ICH },
                { FinalByte.IL, FinalByte.ICH },
                { FinalByte.DL, FinalByte.ICH },
                { FinalByte.EF, FinalByte.ICH },
                { FinalByte.EA, FinalByte.ICH },

                { FinalByte.DCH, FinalByte.ICH },
                { FinalByte.SSE, FinalByte.ICH },
                { FinalByte.CPR, FinalByte.ICH },
                { FinalByte.SU, FinalByte.ICH },
                { FinalByte.SD, FinalByte.ICH },
                { FinalByte.NP, FinalByte.ICH },
                { FinalByte.PP, FinalByte.ICH },
                { FinalByte.CTC, FinalByte.ICH },
                { FinalByte.ECH, FinalByte.ICH },
                { FinalByte.CVT, FinalByte.ICH },
                { FinalByte.CBT, FinalByte.ICH },
                { FinalByte.SRS, FinalByte.ICH },
                { FinalByte.PTX, FinalByte.ICH },
                { FinalByte.SDS, FinalByte.ICH },
                { FinalByte.SIMD, FinalByte.ICH },

                { FinalByte.HPA, FinalByte.ICH },
                { FinalByte.HPR, FinalByte.ICH },
                { FinalByte.REP, FinalByte.ICH },
                { FinalByte.DA, FinalByte.ICH },
                { FinalByte.VPA, FinalByte.ICH },
                { FinalByte.VPR, FinalByte.ICH },
                { FinalByte.HVP, FinalByte.ICH },
                { FinalByte.TBC, FinalByte.ICH },
                { FinalByte.SM, FinalByte.ICH },
                { FinalByte.MC, FinalByte.ICH },
                { FinalByte.HPB, FinalByte.ICH },
                { FinalByte.VPB, FinalByte.ICH },
                { FinalByte.RM, FinalByte.ICH },
                { FinalByte.SGR, FinalByte.ICH },
                { FinalByte.DSR, FinalByte.ICH },
                { FinalByte.DAQ, FinalByte.ICH },

            };
        private static Dictionary<byte, byte> FinalByteWithIntermediateBytes0200Map = new Dictionary<byte, byte>()
            {
                { FinalByte.SL, FinalByte.ICH },
                { FinalByte.SR, FinalByte.ICH },
                { FinalByte.GSM, FinalByte.ICH },
                { FinalByte.GSS, FinalByte.ICH },
                { FinalByte.FNT, FinalByte.ICH },
                { FinalByte.TSS, FinalByte.ICH },
                { FinalByte.JFY, FinalByte.ICH },
                { FinalByte.SPI, FinalByte.ICH },
                { FinalByte.QUAD, FinalByte.ICH },
                { FinalByte.SSU, FinalByte.ICH },
                { FinalByte.PFS, FinalByte.ICH },
                { FinalByte.SHS, FinalByte.ICH },
                { FinalByte.SVS, FinalByte.ICH },
                { FinalByte.IGS, FinalByte.ICH },
                { FinalByte.IDCS, FinalByte.ICH },

                { FinalByte.PPA, FinalByte.ICH },
                { FinalByte.PPR, FinalByte.ICH },
                { FinalByte.PPB, FinalByte.ICH },
                { FinalByte.SPD, FinalByte.ICH },
                { FinalByte.DTA, FinalByte.ICH },
                { FinalByte.SHL, FinalByte.ICH },
                { FinalByte.SLL, FinalByte.ICH },
                { FinalByte.FNK, FinalByte.ICH },
                { FinalByte.SPQR, FinalByte.ICH },
                { FinalByte.SEF, FinalByte.ICH },
                { FinalByte.PEC, FinalByte.ICH },
                { FinalByte.SSW, FinalByte.ICH },
                { FinalByte.SACS, FinalByte.ICH },
                { FinalByte.SAPV, FinalByte.ICH },
                { FinalByte.STAB, FinalByte.ICH },
                { FinalByte.GCC, FinalByte.ICH },

                { FinalByte.TATE, FinalByte.ICH },
                { FinalByte.TALE, FinalByte.ICH },
                { FinalByte.TAC, FinalByte.ICH },
                { FinalByte.TCC, FinalByte.ICH },
                { FinalByte.TSR, FinalByte.ICH },
                { FinalByte.SCO, FinalByte.ICH },
                { FinalByte.SRCS, FinalByte.ICH },
                { FinalByte.SCS, FinalByte.ICH },
                { FinalByte.SLS, FinalByte.ICH },
                { FinalByte.SCP, FinalByte.ICH },
            };

        public byte Content;
        public bool PrivateUse;
        public bool WithIntermediateByte0200;

        /// <summary>
        /// 判断是否是CSI命令里的结束符
        /// </summary>
        /// <param name="c"></param>
        /// <param name="privateUse"></param>
        /// <param name="withIntermediateBytes"></param>
        /// <returns></returns>
        public static bool IsFinalByte(byte c, out bool privateUse, out bool withIntermediateBytes)
        {
            privateUse = false;
            withIntermediateBytes = false;
            if (FinalByteMap.ContainsKey(c))
            {
                privateUse = c > PrivateUse_MinimumValue && c < PrivateUse_MaximumValue;
                withIntermediateBytes = false;
                return true;
            }

            if (FinalByteWithIntermediateBytes0200Map.ContainsKey(c))
            {
                privateUse = c > PrivateUse_MinimumValue && c < PrivateUse_MaximumValue;
                withIntermediateBytes = true;
                return true;
            }

            return false;
        }
    }

    public struct FormattedCSI : IFormattedCf
    {
        public byte[] ParameterBytes;
        public byte[] IntermediateBytes;
        public FinalByte FinalByte;

        public int GetSize()
        {
            return this.ParameterBytes.Length + this.IntermediateBytes.Length + 1;
        }
    }

    public class CSIParser : FeParser
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("CSIParser");

        public static readonly byte InvalidFinalByte = byte.MaxValue;

        public override bool Parse(byte[] chars, out IFormattedCf cf)
        {
            cf = null;
            List<byte> parameterBytes = new List<byte>(); // parameterBytes
            List<byte> intermediateBytes = new List<byte>(); // intermediateBytes
            byte finalByte = InvalidFinalByte;    // 结束符
            bool privateUse = false;              // 是否是内部使用的命令
            bool withIntermediateByte0200 = false;// 命令里是否带有0200中间字节

            #region 解析CSI命令各组成部分的数据

            for (int index = 1; index < chars.Length; index++)
            {
                byte c = chars[index];
                if (ParameterBytes.IsParameterByte(c))
                {
                    parameterBytes.Add(c);
                }

                if (IntermediateBytes.IsIntermediateByte(c))
                {
                    intermediateBytes.Add(c);
                }

                if (FinalByte.IsFinalByte(c, out privateUse, out withIntermediateByte0200))
                {
                    finalByte = c;
                    break;
                }
            }

            #endregion

            if (finalByte == InvalidFinalByte)
            {
                logger.ErrorFormat("解析CSI失败, 未找到FinalByte");
                return false;
            }

            FormattedCSI csi;
            csi.ParameterBytes = parameterBytes.ToArray();
            csi.IntermediateBytes = intermediateBytes.ToArray();
            csi.FinalByte.Content = finalByte;
            csi.FinalByte.PrivateUse = privateUse;
            csi.FinalByte.WithIntermediateByte0200 = withIntermediateByte0200;
            cf = csi;

            return true;
        }
    }
}
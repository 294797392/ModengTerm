using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XTerminal.ControlFunctions.CSI;

namespace XTerminal.ControlFunctions
{
    /// <summary>
    /// 对于7位ascii编码：
    ///     Fe表示跟在ControlFunction后面的一个字符
    ///     可以认为是控制函数的命令类型
    /// 对于8位ascii编码：
    ///     Fe表示ControlFunction
    /// 
    /// 参考：
    ///     xterminal\Dependencies\Control Functions for Coded Character Sets Ecma-048.pdf 5.4章节
    /// </summary>
    public class Fe
    {
        #region 7 bit code Fe

        public static byte BPH_7BIT = CharacterUtils.BitCombinations(04, 02);
        public static byte NBH_7BIT = CharacterUtils.BitCombinations(04, 03);
        public static byte NEL_7BIT = CharacterUtils.BitCombinations(04, 05);
        public static byte SSA_7BIT = CharacterUtils.BitCombinations(04, 06);
        public static byte ESA_7BIT = CharacterUtils.BitCombinations(04, 07);
        public static byte HTS_7BIT = CharacterUtils.BitCombinations(04, 08);
        public static byte HTJ_7BIT = CharacterUtils.BitCombinations(04, 09);
        public static byte VTS_7BIT = CharacterUtils.BitCombinations(04, 10);
        public static byte PLD_7BIT = CharacterUtils.BitCombinations(04, 11);
        public static byte PLU_7BIT = CharacterUtils.BitCombinations(04, 12);
        public static byte R1_7BIT = CharacterUtils.BitCombinations(04, 13);
        public static byte SS2_7BIT = CharacterUtils.BitCombinations(04, 14);
        public static byte SS3_7BIT = CharacterUtils.BitCombinations(04, 15);

        public static byte DCS_7BIT = CharacterUtils.BitCombinations(05, 00);
        public static byte PU1_7BIT = CharacterUtils.BitCombinations(05, 01);
        public static byte PU2_7BIT = CharacterUtils.BitCombinations(05, 02);
        public static byte STS_7BIT = CharacterUtils.BitCombinations(05, 03);
        public static byte CCH_7BIT = CharacterUtils.BitCombinations(05, 04);
        public static byte MW_7BIT = CharacterUtils.BitCombinations(05, 05);
        public static byte SPA_7BIT = CharacterUtils.BitCombinations(05, 06);
        public static byte EPA_7BIT = CharacterUtils.BitCombinations(05, 07);
        public static byte SOS_7BIT = CharacterUtils.BitCombinations(05, 08);
        public static byte SCI_7BIT = CharacterUtils.BitCombinations(05, 10);
        public static byte CSI_7BIT = CharacterUtils.BitCombinations(05, 11);
        public static byte ST_7BIT = CharacterUtils.BitCombinations(05, 12);
        public static byte OSC_7BIT = CharacterUtils.BitCombinations(05, 13);
        public static byte PM_7BIT = CharacterUtils.BitCombinations(05, 14);
        public static byte APC_7BIT = CharacterUtils.BitCombinations(05, 15);

        #endregion

        #region 8 bit code Fe

        public static byte BPH_8BIT = CharacterUtils.BitCombinations(08, 02);
        public static byte NBH_8BIT = CharacterUtils.BitCombinations(08, 03);
        public static byte NEL_8BIT = CharacterUtils.BitCombinations(08, 05);
        public static byte SSA_8BIT = CharacterUtils.BitCombinations(08, 06);
        public static byte ESA_8BIT = CharacterUtils.BitCombinations(08, 07);
        public static byte HTS_8BIT = CharacterUtils.BitCombinations(08, 08);
        public static byte HTJ_8BIT = CharacterUtils.BitCombinations(08, 09);
        public static byte VTS_8BIT = CharacterUtils.BitCombinations(08, 10);
        public static byte PLD_8BIT = CharacterUtils.BitCombinations(08, 11);
        public static byte PLU_8BIT = CharacterUtils.BitCombinations(08, 12);
        public static byte R1_8BIT = CharacterUtils.BitCombinations(08, 13);
        public static byte SS2_8BIT = CharacterUtils.BitCombinations(08, 14);
        public static byte SS3_8BIT = CharacterUtils.BitCombinations(08, 15);

        public static byte DCS_8BIT = CharacterUtils.BitCombinations(09, 00);
        public static byte PU1_8BIT = CharacterUtils.BitCombinations(09, 01);
        public static byte PU2_8BIT = CharacterUtils.BitCombinations(09, 02);
        public static byte STS_8BIT = CharacterUtils.BitCombinations(09, 03);
        public static byte CCH_8BIT = CharacterUtils.BitCombinations(09, 04);
        public static byte MW_8BIT = CharacterUtils.BitCombinations(09, 05);
        public static byte SPA_8BIT = CharacterUtils.BitCombinations(09, 06);
        public static byte EPA_8BIT = CharacterUtils.BitCombinations(09, 07);
        public static byte SOS_8BIT = CharacterUtils.BitCombinations(09, 08);
        public static byte SCI_8BIT = CharacterUtils.BitCombinations(09, 10);
        public static byte CSI_8BIT = CharacterUtils.BitCombinations(09, 11);
        public static byte ST_8BIT = CharacterUtils.BitCombinations(09, 12);
        public static byte OSC_8BIT = CharacterUtils.BitCombinations(09, 13);
        public static byte PM_8BIT = CharacterUtils.BitCombinations(09, 14);
        public static byte APC_8BIT = CharacterUtils.BitCombinations(09, 15);

        #endregion

        private static Dictionary<byte, byte> _7BitFeMap = new Dictionary<byte, byte>()
        {
            { BPH_7BIT, BPH_7BIT },
            { NBH_7BIT, NBH_7BIT },
            { NEL_7BIT, NEL_7BIT },
            { SSA_7BIT, SSA_7BIT },
            { ESA_7BIT, ESA_7BIT },
            { HTS_7BIT, HTS_7BIT },
            { HTJ_7BIT, HTJ_7BIT },
            { VTS_7BIT, VTS_7BIT },
            { PLD_7BIT, PLD_7BIT },
            { PLU_7BIT, PLU_7BIT },
            { R1_7BIT, R1_7BIT },
            { SS2_7BIT, SS2_7BIT },
            { SS3_7BIT, SS3_7BIT },

            { DCS_7BIT, DCS_7BIT },
            { PU1_7BIT, PU1_7BIT },
            { PU2_7BIT, PU2_7BIT },
            { STS_7BIT, STS_7BIT },
            { CCH_7BIT, CCH_7BIT },
            { MW_7BIT, MW_7BIT },
            { SPA_7BIT, SPA_7BIT },
            { EPA_7BIT, EPA_7BIT },
            { SOS_7BIT, SOS_7BIT },
            { SCI_7BIT, SCI_7BIT },
            { CSI_7BIT, CSI_7BIT },
            { ST_7BIT, ST_7BIT },
            { OSC_7BIT, OSC_7BIT },
            { PM_7BIT, PM_7BIT },
            { APC_7BIT, APC_7BIT },
        };

        public static bool Is7bitFe(byte feByte)
        {
            return _7BitFeMap.ContainsKey(feByte);
        }
    }

    #region CSI数据结构定义

    // CSI是Fe的一种
    namespace CSI
    {
        /// <summary>
        /// 参数字节
        /// which, if present, consist of bit combinations from 03/00 to 03/15;
        /// </summary>
        public struct ParameterBytes
        {
            public static readonly byte MinimumValue = CharacterUtils.BitCombinations(03, 00);
            public static readonly byte MaximumValue = CharacterUtils.BitCombinations(03, 15);

            public static bool IsParameterByte(byte c)
            {
                return c > MinimumValue && c < MaximumValue;
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

            public static bool IsIntermediateByte(byte c)
            {
                return c > MinimumValue && c < MaximumValue;
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
            public static byte CUU = CharacterUtils.BitCombinations(04, 01);
            public static byte CUD = CharacterUtils.BitCombinations(04, 02);
            public static byte CUF = CharacterUtils.BitCombinations(04, 03);
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
            public static byte SRS = CharacterUtils.BitCombinations(05, 11);
            public static byte PTX = CharacterUtils.BitCombinations(05, 12);
            public static byte SDS = CharacterUtils.BitCombinations(05, 13);
            public static byte SIMD = CharacterUtils.BitCombinations(05, 14);

            public static byte HPA = CharacterUtils.BitCombinations(06, 00);
            public static byte HPR = CharacterUtils.BitCombinations(06, 01);
            public static byte REP = CharacterUtils.BitCombinations(06, 02);
            public static byte DA = CharacterUtils.BitCombinations(06, 03);
            public static byte VPA = CharacterUtils.BitCombinations(06, 04);
            public static byte VPR = CharacterUtils.BitCombinations(06, 05);
            public static byte HVP = CharacterUtils.BitCombinations(06, 06);
            public static byte TBC = CharacterUtils.BitCombinations(06, 07);
            public static byte SM = CharacterUtils.BitCombinations(06, 08);
            public static byte MC = CharacterUtils.BitCombinations(06, 09);
            public static byte HPB = CharacterUtils.BitCombinations(06, 10);
            public static byte VPB = CharacterUtils.BitCombinations(06, 11);
            public static byte RM = CharacterUtils.BitCombinations(06, 12);
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

            public byte Char;
            public bool PrivateUse;
            public bool WithIntermediateByte0200;

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
    }

    #endregion

    #region OSC数据结构定义

    /*
     * OSC is used as the opening delimiter of a control string for operating system use. The command string 
     * following may consist of a sequence of bit combinations in the range 00/08 to 00/13 and 02/00 to 07/14. 
     * The control string is closed by the terminating delimiter STRING TERMINATOR (ST). The 
     * interpretation of the command string depends on the relevant operating system. 
     * 参考：
     *      xterminal\Dependencies\Control Functions for Coded Character Sets Ecma-048.pdf 8.3.89章节
     */

    public class OSC
    {
        /// <summary>
        /// 每种终端可能使用不同的STRING_TERMINATOR分隔符
        /// 需要在界面做相应配置
        /// 经过测试，在ubuntu上，使用ascii码的32，是一个空字符
        /// </summary>
        public static readonly byte STRING_TERMINATOR = CharacterUtils.BitCombinations(02, 00);

        /// <summary>
        /// 检测是否是OSC结束符
        /// </summary>
        /// <returns></returns>
        public static bool IsTerminatedChar(byte c)
        {
            return c == STRING_TERMINATOR;
        }
    }

    #endregion

    /// <summary>
    /// 解析控制函数的类型
    /// </summary>
    public static class FeParser
    {
        /// <summary>
        /// 从一串字符中解析出CSI的parameter，intermediate，final数据
        /// </summary>
        /// <param name="bytes">要解析的bytes</param>
        /// <param name="parameter"></param>
        /// <param name="intermediate"></param>
        /// <param name="final"></param>
        /// <returns></returns>
        public static bool ParseCSI(byte[] chars, out ParameterBytes parameter, out IntermediateBytes intermediate, out FinalByte final)
        {
            final.PrivateUse = false;
            final.Char = 0;
            final.WithIntermediateByte0200 = false;

            bool isCsiStart = false;
            bool is7BitAscii = true;
            int length = chars.Length;
            List<byte> pBytes = new List<byte>(); // parameterBytes
            List<byte> iBytes = new List<byte>(); // intermediateBytes

            for (int idx = 0; idx < length; idx++)
            {
                byte c = chars[idx];
                if (isCsiStart)
                {
                    if (ParameterBytes.IsParameterByte(c))
                    {
                        pBytes.Add(c);
                    }

                    if (IntermediateBytes.IsIntermediateByte(c))
                    {
                        iBytes.Add(c);
                    }

                    if (FinalByte.IsFinalByte(c, out final.PrivateUse, out final.WithIntermediateByte0200))
                    {
                        return true;
                    }
                }
                else
                {
                    if (c == Fe.CSI_8BIT)
                    {
                        is7BitAscii = false;
                        isCsiStart = true;
                        continue;
                    }

                    if (c != ControlFunctions.ESC)
                    {
                        continue;
                    }

                    if (chars[idx + 1] == Fe.CSI_7BIT)
                    {
                        isCsiStart = true;
                    }
                }
            }

            return false;
        }
    }
}
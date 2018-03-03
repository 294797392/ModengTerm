using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XTerminal.CharacterParsers;

namespace XTerminal.AnsiCharacrers
{
    /// <summary>
    /// 参数字节
    /// which, if present, consist of bit combinations from 03/00 to 03/15;
    /// </summary>
    public struct ParameterBytes
    {

    }

    /// <summary>
    /// 中间字节
    /// which, if present, consist of bit combinations from 02/00 to 02/15. 
    /// Together with the Final Byte F, they identify the control function
    /// </summary>
    public struct IntermediateBytes
    {

    }

    /// <summary>
    /// 结束字节
    /// 定义了控制序列（CSI）的结束符
    /// 
    /// it consists of a bit combination from 04/00 to 07/14; it terminates the control 
    /// combinations 07/00 to 07/14 are available as Final Bytes of control sequences for private (or
    /// experimental) use.
    /// </summary>
    public class FinalBytes
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
    }

    /// <summary>
    /// 存储控制序列的信息
    /// CSI属于Fe
    /// </summary>
    public class CSI
    {
    }
}
using AsciiControlFunctions.FeParsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AsciiControlFunctions
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
        //public static bool ParseCSI(byte[] chars, out ParameterBytes parameter, out IntermediateBytes intermediate, out FinalByte final)
        //{
        //    final.PrivateUse = false;
        //    final.Char = 0;
        //    final.WithIntermediateByte0200 = false;

        //    bool isCsiStart = false;
        //    bool is7BitAscii = true;
        //    int length = chars.Length;
        //    List<byte> pBytes = new List<byte>(); // parameterBytes
        //    List<byte> iBytes = new List<byte>(); // intermediateBytes

        //    for (int idx = 0; idx < length; idx++)
        //    {
        //        byte c = chars[idx];
        //        if (isCsiStart)
        //        {
        //            if (ParameterBytes.IsParameterByte(c))
        //            {
        //                pBytes.Add(c);
        //            }

        //            if (IntermediateBytes.IsIntermediateByte(c))
        //            {
        //                iBytes.Add(c);
        //            }

        //            if (FinalByte.IsFinalByte(c, out final.PrivateUse, out final.WithIntermediateByte0200))
        //            {
        //                return true;
        //            }
        //        }
        //        else
        //        {
        //            if (c == Fe.CSI_8BIT)
        //            {
        //                is7BitAscii = false;
        //                isCsiStart = true;
        //                continue;
        //            }

        //            if (c != ControlFunctions.ESC)
        //            {
        //                continue;
        //            }

        //            if (chars[idx + 1] == Fe.CSI_7BIT)
        //            {
        //                isCsiStart = true;
        //            }
        //        }
        //    }

        //    return false;
        //}
    }
}
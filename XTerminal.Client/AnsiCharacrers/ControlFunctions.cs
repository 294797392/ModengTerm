using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XTerminal.CharacterParsers
{
    /// <summary>
    /// 根据Control Functions for Coded Character Sets Ecma-048.pdf标准，
    /// 这是一个ECMA定义的标准，所有的终端模拟器都会实现这个标准
    /// 参考：
    ///     Dependencies/Control Functions for Coded Character Sets Ecma-048.pdf
    ///     
    /// 控制序列：
    ///     由01/11开头，后面跟一个Fe，Fe可以看作成命令类型，7位编码的ASCII码和8位编码的ASCII码使用的Fe码的范围不同，但是每个Fe所表示的意义都是相同的
    ///     对于7位编码的ASCII码：
    ///         1.Fe的范围在04/00（64） - 05/15（95）之间
    ///     对于8位编码的ASCII码：
    ///         1.Fe的范围在08/00（128） - 09/15（159）之间
    /// 
    /// 完整的转义序列格式是：
    ///     ControlFunction Fe [ [Fe参数] [Fe结束符] ]
    /// 
    /// 一般情况下，主机返回的ControlFunctions和Fe百分之九十都是ESC和CSI，这里解释一下CSI的含义
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
        private static Dictionary<byte, byte> cfMap = new Dictionary<byte, byte>();

        #region C0 set

        public static byte NUL = CharacterUtils.BitCombinations(00, 00);
        public static byte SOH = CharacterUtils.BitCombinations(00, 01);
        public static byte STX = CharacterUtils.BitCombinations(00, 02);
        public static byte ETX = CharacterUtils.BitCombinations(00, 03);
        public static byte EOT = CharacterUtils.BitCombinations(00, 04);
        public static byte ENQ = CharacterUtils.BitCombinations(00, 05);
        public static byte ACK = CharacterUtils.BitCombinations(00, 06);
        public static byte BEL = CharacterUtils.BitCombinations(00, 07);
        public static byte BS = CharacterUtils.BitCombinations(00, 08);
        public static byte HT = CharacterUtils.BitCombinations(00, 09);
        public static byte LF = CharacterUtils.BitCombinations(00, 10);
        public static byte VT = CharacterUtils.BitCombinations(00, 11);
        public static byte FF = CharacterUtils.BitCombinations(00, 12);
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


        static ControlFunctions()
        {
            cfMap[ControlFunctions.NUL] = ControlFunctions.NUL;
            cfMap[ControlFunctions.SOH] = ControlFunctions.NUL;
            cfMap[ControlFunctions.STX] = ControlFunctions.NUL;
            cfMap[ControlFunctions.ETX] = ControlFunctions.NUL;
            cfMap[ControlFunctions.EOT] = ControlFunctions.NUL;
            cfMap[ControlFunctions.ENQ] = ControlFunctions.NUL;
            cfMap[ControlFunctions.ACK] = ControlFunctions.NUL;
            cfMap[ControlFunctions.BEL] = ControlFunctions.NUL;
            cfMap[ControlFunctions.BS] = ControlFunctions.NUL;
            cfMap[ControlFunctions.HT] = ControlFunctions.NUL;
            cfMap[ControlFunctions.LF] = ControlFunctions.NUL;
            cfMap[ControlFunctions.VT] = ControlFunctions.NUL;
            cfMap[ControlFunctions.FF] = ControlFunctions.NUL;
            cfMap[ControlFunctions.CR] = ControlFunctions.NUL;
            cfMap[ControlFunctions.SOorLS1] = ControlFunctions.NUL;
            cfMap[ControlFunctions.S1orLS0] = ControlFunctions.NUL;
            cfMap[ControlFunctions.DLE] = ControlFunctions.NUL;
            cfMap[ControlFunctions.DC1] = ControlFunctions.NUL;
            cfMap[ControlFunctions.DC2] = ControlFunctions.NUL;
            cfMap[ControlFunctions.DC3] = ControlFunctions.NUL;
            cfMap[ControlFunctions.DC4] = ControlFunctions.NUL;
            cfMap[ControlFunctions.NAK] = ControlFunctions.NUL;
            cfMap[ControlFunctions.SYN] = ControlFunctions.NUL;
            cfMap[ControlFunctions.ETB] = ControlFunctions.NUL;
            cfMap[ControlFunctions.CAN] = ControlFunctions.NUL;
            cfMap[ControlFunctions.EM] = ControlFunctions.NUL;
            cfMap[ControlFunctions.SUB] = ControlFunctions.NUL;
            cfMap[ControlFunctions.ESC] = ControlFunctions.NUL;
            cfMap[ControlFunctions.IS4] = ControlFunctions.NUL;
            cfMap[ControlFunctions.IS3] = ControlFunctions.NUL;
            cfMap[ControlFunctions.IS2] = ControlFunctions.NUL;
            cfMap[ControlFunctions.IS1] = ControlFunctions.NUL;


            //cfMap[ControlFunctions.BPH_7BIT] = ControlFunctions.NUL;
            //cfMap[ControlFunctions.NBH_7BIT] = ControlFunctions.NUL;
            //cfMap[ControlFunctions.NEL_7BIT] = ControlFunctions.NUL;
            //cfMap[ControlFunctions.SSA_7BIT] = ControlFunctions.NUL;
            //cfMap[ControlFunctions.ESA_7BIT] = ControlFunctions.NUL;
            //cfMap[ControlFunctions.HTS_7BIT] = ControlFunctions.NUL;
            //cfMap[ControlFunctions.HTJ_7BIT] = ControlFunctions.NUL;
            //cfMap[ControlFunctions.VTS_7BIT] = ControlFunctions.NUL;
            //cfMap[ControlFunctions.PLD_7BIT] = ControlFunctions.NUL;
            //cfMap[ControlFunctions.PLU_7BIT] = ControlFunctions.NUL;
            //cfMap[ControlFunctions.R1_7BIT] = ControlFunctions.NUL;
            //cfMap[ControlFunctions.SS2_7BIT] = ControlFunctions.NUL;
            //cfMap[ControlFunctions.SS3_7BIT] = ControlFunctions.NUL;
            //cfMap[ControlFunctions.DCS_7BIT] = ControlFunctions.NUL;
            //cfMap[ControlFunctions.PU1_7BIT] = ControlFunctions.NUL;
            //cfMap[ControlFunctions.PU2_7BIT] = ControlFunctions.NUL;
            //cfMap[ControlFunctions.STS_7BIT] = ControlFunctions.NUL;
            //cfMap[ControlFunctions.CCH_7BIT] = ControlFunctions.NUL;
            //cfMap[ControlFunctions.MW_7BIT] = ControlFunctions.NUL;
            //cfMap[ControlFunctions.SPA_7BIT] = ControlFunctions.NUL;
            //cfMap[ControlFunctions.EPA_7BIT] = ControlFunctions.NUL;
            //cfMap[ControlFunctions.SOS_7BIT] = ControlFunctions.NUL;
            //cfMap[ControlFunctions.SCI_7BIT] = ControlFunctions.NUL;
            //cfMap[ControlFunctions.CSI_7BIT] = ControlFunctions.NUL;
            //cfMap[ControlFunctions.ST_7BIT] = ControlFunctions.NUL;
            //cfMap[ControlFunctions.OSC_7BIT] = ControlFunctions.NUL;
            //cfMap[ControlFunctions.PM_7BIT] = ControlFunctions.NUL;
            //cfMap[ControlFunctions.APC_7BIT] = ControlFunctions.NUL;


            //cfMap[ControlFunctions.BPH_8BIT] = ControlFunctions.NUL;
            //cfMap[ControlFunctions.NBH_8BIT] = ControlFunctions.NUL;
            //cfMap[ControlFunctions.NEL_8BIT] = ControlFunctions.NUL;
            //cfMap[ControlFunctions.SSA_8BIT] = ControlFunctions.NUL;
            //cfMap[ControlFunctions.ESA_8BIT] = ControlFunctions.NUL;
            //cfMap[ControlFunctions.HTS_8BIT] = ControlFunctions.NUL;
            //cfMap[ControlFunctions.HTJ_8BIT] = ControlFunctions.NUL;
            //cfMap[ControlFunctions.VTS_8BIT] = ControlFunctions.NUL;
            //cfMap[ControlFunctions.PLD_8BIT] = ControlFunctions.NUL;
            //cfMap[ControlFunctions.PLU_8BIT] = ControlFunctions.NUL;
            //cfMap[ControlFunctions.R1_8BIT] = ControlFunctions.NUL;
            //cfMap[ControlFunctions.SS2_8BIT] = ControlFunctions.NUL;
            //cfMap[ControlFunctions.SS3_8BIT] = ControlFunctions.NUL;
            //cfMap[ControlFunctions.DCS_8BIT] = ControlFunctions.NUL;
            //cfMap[ControlFunctions.PU1_8BIT] = ControlFunctions.NUL;
            //cfMap[ControlFunctions.PU2_8BIT] = ControlFunctions.NUL;
            //cfMap[ControlFunctions.STS_8BIT] = ControlFunctions.NUL;
            //cfMap[ControlFunctions.CCH_8BIT] = ControlFunctions.NUL;
            //cfMap[ControlFunctions.MW_8BIT] = ControlFunctions.NUL;
            //cfMap[ControlFunctions.SPA_8BIT] = ControlFunctions.NUL;
            //cfMap[ControlFunctions.EPA_8BIT] = ControlFunctions.NUL;
            //cfMap[ControlFunctions.SOS_8BIT] = ControlFunctions.NUL;
            //cfMap[ControlFunctions.SCI_8BIT] = ControlFunctions.NUL;
            //cfMap[ControlFunctions.CSI_8BIT] = ControlFunctions.NUL;
            //cfMap[ControlFunctions.ST_8BIT] = ControlFunctions.NUL;
            //cfMap[ControlFunctions.OSC_8BIT] = ControlFunctions.NUL;
            //cfMap[ControlFunctions.PM_8BIT] = ControlFunctions.NUL;
            //cfMap[ControlFunctions.APC_8BIT] = ControlFunctions.NUL;

        }

        public static bool IsControlFunction(byte c)
        {
            return cfMap.ContainsKey(c);
        }
    }
}
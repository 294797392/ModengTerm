using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XTerminalCore
{
    /*
     * OSC is used as the opening delimiter of a control string for operating system use. The command string 
     * following may consist of a sequence of bit combinations in the range 00/08 to 00/13 and 02/00 to 07/14. 
     * The control string is closed by the terminating delimiter STRING TERMINATOR (ST). The 
     * interpretation of the command string depends on the relevant operating system. 
     * 参考：
     *      xterminal\Dependencies\Control Functions for Coded Character Sets Ecma-048.pdf 8.3.89章节
     */

    public struct FormattedOSC : IFormattedCf
    {
        private static readonly byte PsMinimumValue = CharacterUtils.BitCombinations(00, 08);
        private static readonly byte PsMaximumValue = CharacterUtils.BitCombinations(00, 13);

        private static readonly byte PsMinimumValue2 = CharacterUtils.BitCombinations(02, 00);
        private static readonly byte PsMaximumValue2 = CharacterUtils.BitCombinations(07, 14);

        public static bool IsValidOSC(byte Ps)
        {
            return (Ps > PsMinimumValue && Ps < PsMaximumValue) && (Ps > PsMinimumValue2 && Ps < PsMaximumValue2);
        }

        /// <summary>
        /// OSC ControlFunction里定义的CommandString
        /// 不包括OSC字符
        /// </summary>
        public byte[] CommandString;

        public int GetSize()
        {
            return this.CommandString.Length;
        }
    }

    public class OSCParser : FeParser
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("OSCParser");

        public override bool Parse(byte[] chars, out IFormattedCf cf)
        {
            cf = null;

            // 从第二个字符开始读取，一直读取到ST为止
            FormattedOSC osc;
            osc.CommandString = chars.Copy(1, base.ST);
            cf = osc;

            return true;
        }
    }
}
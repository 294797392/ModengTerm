using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AsciiControlFunctions.FeParsers
{
    /*
     * OSC is used as the opening delimiter of a control string for operating system use. The command string 
     * following may consist of a sequence of bit combinations in the range 00/08 to 00/13 and 02/00 to 07/14. 
     * The control string is closed by the terminating delimiter STRING TERMINATOR (ST). The 
     * interpretation of the command string depends on the relevant operating system. 
     * 参考：
     *      xterminal\Dependencies\Control Functions for Coded Character Sets Ecma-048.pdf 8.3.89章节
     */


    public class OSCStructures
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
}

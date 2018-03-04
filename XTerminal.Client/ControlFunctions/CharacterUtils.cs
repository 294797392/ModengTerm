using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XTerminal.ControlFunctions
{
    public class CharacterUtils
    {
        /// <summary>
        /// 把一个高四位和一个低四位的二进制数组合成一个ascii码
        /// </summary>
        /// <param name="high_bit">高四位</param>
        /// <param name="low_bit">低四位</param>
        /// <returns></returns>
        public static byte BitCombinations(int high_bit, int low_bit)
        {
            return Convert.ToByte(string.Format("{0}{1}", Convert.ToString(high_bit, 2), Convert.ToString(low_bit, 2)), 2);
        }
    }
}

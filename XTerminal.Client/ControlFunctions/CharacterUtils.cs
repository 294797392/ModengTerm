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
        /// <param name="high_bit">十进制表示的高四位</param>
        /// <param name="low_bit">十进制表示的低四位</param>
        /// <returns></returns>
        public static byte BitCombinations(int high_bit, int low_bit)
        {
            string high = Convert.ToString(high_bit, 2).PadLeft(4, '0');
            string low = Convert.ToString(low_bit, 2).PadLeft(4, '0');
            return Convert.ToByte(string.Format("{0}{1}", high, low), 2);
        }
    }
}
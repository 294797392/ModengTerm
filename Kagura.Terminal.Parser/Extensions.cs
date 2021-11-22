using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VideoTerminal.Parser
{
    public static class Extensions
    {
        public static byte Byte(this char c)
        {
            return (byte)c;
        }

        /// <summary>
        /// 把一个8进制数转换成byte
        /// </summary>
        /// <param name="octal"></param>
        /// <returns></returns>
        public static byte Octal2Byte(this string octal)
        {
            return (byte)Convert.ToInt32(octal, 8);
        }

        /// <summary>
        /// 删除集合中最后一个元素
        /// </summary>
        /// <param name="list"></param>
        public static void RemoveLast(this List<byte> list)
        {
            list.RemoveAt(list.Count - 1);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XTerminalCore
{
    public static class Utils
    {
        /// <summary>
        /// 从offset处拷贝字符串，一直拷贝到endChar处为止，返回的字符串里包含endChar
        /// </summary>
        /// <param name="source"></param>
        /// <param name="sourceIndex"></param>
        /// <param name="endChar"></param>
        /// <returns></returns>
        public static byte[] Copy(this byte[] source, int sourceIndex, byte endChar)
        {
            int charLen = source.Length;
            int endIndex = sourceIndex;
            for (endIndex = sourceIndex; endIndex < charLen; endIndex++)
            {
                byte c = source[endIndex];
                if (c == endChar)
                {
                    break;
                }
            }

            byte[] result = new byte[endIndex - sourceIndex];
            Array.Copy(source, sourceIndex, result, 0, result.Length);
            return result;
        }


        public static string Stringify(this byte[] c, int index, int count)
        {
            return Encoding.ASCII.GetString(c, index, count);
        }

        public static string Stringify(this byte[] cs)
        {
            return Encoding.ASCII.GetString(cs);
        }

        /// <summary>
        /// 把ascii码尝试转换成一个数字
        /// </summary>
        /// <param name="chars">要转换的ascii码数组</param>
        /// <param name="number">转换后得到的数字</param>
        /// <returns></returns>
        public static bool Numberic(this byte[] chars, out int number)
        {
            string text = Encoding.ASCII.GetString(chars);
            return int.TryParse(text, out number);
        }

        /// <summary>
        /// 查找chars中第一次出现c的索引位置
        /// </summary>
        /// <param name="chars"></param>
        /// <param name="c"></param>
        /// <param name="idx"></param>
        /// <returns></returns>
        public static bool IndexOf(this byte[] chars, byte c, out int idx)
        {
            idx = -1;
            int length = chars.Length;
            for (int i = 0; i < length; i++)
            {
                if (chars[i] == c)
                {
                    idx = i;
                    return true;
                }
            }

            return false;
        }
    }
}
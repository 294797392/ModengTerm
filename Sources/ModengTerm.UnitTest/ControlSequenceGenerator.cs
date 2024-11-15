using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.UnitTest
{
    /// <summary>
    /// 负责生成Ssh返回给客户端的原始控制序列
    /// </summary>
    public static class ControlSequenceGenerator
    {
        private static readonly byte ESC = 0x1b;

        /// <summary>
        /// 光标移动到一个指定的位置
        /// 左上角是1,1
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public static byte[] CUP_CursorPosition(int row, int col)
        {
            List<byte> result = new List<byte>();
            result.Add(ESC);
            result.Add((byte)'[');
            result.AddRange(Encoding.ASCII.GetBytes(row.ToString()));
            result.Add((byte)';');
            result.AddRange(Encoding.ASCII.GetBytes(col.ToString()));
            result.Add((byte)'H');

            return result.ToArray();
        }

        /// <summary>
        /// 构造一个无参数的CUP指令
        /// </summary>
        /// <returns></returns>
        public static byte[] CUP_CursorPosition()
        {
            return new byte[] { ESC, (byte)'[', (byte)'H' };
        }

        /// <summary>
        /// 光标右移
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static byte[] CUF_CursorForward()
        {
            return new byte[] { ESC, (byte)'[', (byte)'C' };
        }

        /// <summary>
        /// 光标上移
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static byte[] CUU_CursorUp()
        {
            return new byte[] { ESC, (byte)'[', (byte)'A' };
        }

        /// <summary>
        /// 光标下移
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static byte[] CUD_CursorDown()
        {
            return new byte[] { ESC, (byte)'[', (byte)'B' };
        }

        /// <summary>
        /// 光标左移
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static byte[] CUB_CursorBackward()
        {
            return new byte[] { ESC, (byte)'[', (byte)'D' };
        }
    }

}

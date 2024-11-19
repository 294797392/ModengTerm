using ModengTerm.Terminal.Parsing;
using System.Text;

namespace ModengTerm.UnitTest
{
    /// <summary>
    /// 负责原始控制序列
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

        public static byte[] ED_EraseDisplay(VTEraseType type)
        {
            int v = (int)type;
            byte[] bytes = Encoding.ASCII.GetBytes(v.ToString());

            return new byte[] { ESC, (byte)'[', bytes[0], (byte)'J' };
        }

        public static byte[] EL_EraseLine(VTEraseType eraseType)
        {
            int v = (int)eraseType;
            byte[] bytes = Encoding.ASCII.GetBytes(v.ToString());

            return new byte[] { ESC, (byte)'[', bytes[0], (byte)'K' };
        }

        public static byte[] DCH_DeleteCharacter(int n) 
        {
            List<byte> bytes = new List<byte>();
            bytes.Add(ESC);
            bytes.Add((byte)'[');
            bytes.AddRange(Encoding.ASCII.GetBytes(n.ToString()));
            bytes.Add((byte)'P');

            return bytes.ToArray();
        }

        public static byte[] ICH_InsertCharacter(int n) 
        {
            List<byte> bytes = new List<byte>();
            bytes.Add(ESC);
            bytes.Add((byte)'[');
            bytes.AddRange(Encoding.ASCII.GetBytes(n.ToString()));
            bytes.Add((byte)'@');

            return bytes.ToArray();
        }

        public static byte[] CRLF()
        {
            return new byte[] { (byte)'\r', (byte)'\n' };
        }

        public static byte[] RI_ReverseLineFeed() 
        {
            return new byte[] { ESC, (byte)'M' };
        }

        public static byte[] DECSTBM_SetScrollingRegion(int topMargin, int bottomMargin)
        {
            List<byte> bytes = new List<byte>();
            bytes.Add(ESC);
            bytes.Add((byte)'[');
            bytes.AddRange(Encoding.ASCII.GetBytes(topMargin.ToString()));
            bytes.Add((byte)';');
            bytes.AddRange(Encoding.ASCII.GetBytes(bottomMargin.ToString()));
            bytes.Add((byte)'r');

            return bytes.ToArray();
        }

        public static byte[] DL_DeleteLine(int n) 
        {
            List<byte> bytes = new List<byte>();
            bytes.Add(ESC);
            bytes.Add((byte)'[');
            bytes.AddRange(Encoding.ASCII.GetBytes(n.ToString()));
            bytes.Add((byte)'M');

            return bytes.ToArray();
        }

        public static byte[] IL_InsertLine(int n)
        {
            List<byte> bytes = new List<byte>();
            bytes.Add(ESC);
            bytes.Add((byte)'[');
            bytes.AddRange(Encoding.ASCII.GetBytes(n.ToString()));
            bytes.Add((byte)'L');

            return bytes.ToArray();
        }

        public static byte[] ECH_EraseCharacters(int n) 
        {
            List<byte> bytes = new List<byte>();
            bytes.Add(ESC);
            bytes.Add((byte)'[');
            bytes.AddRange(Encoding.ASCII.GetBytes(n.ToString()));
            bytes.Add((byte)'X');

            return bytes.ToArray();
        }
    }
}

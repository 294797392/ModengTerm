using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace GTerminalCore
{
    public static class VTInputTbl
    {
        private static byte Byte(this char c)
        {
            return (byte)c;
        }

        public static Dictionary<Key, byte[]> ShiftTable = new Dictionary<Key, byte[]>() { };

        public static Dictionary<Key, byte[]> ControlTable = new Dictionary<Key, byte[]>
        {
            /* 英文字母键 */
            { Key.Q, new byte[] { 'Q'.Byte() } },
            { Key.W, new byte[] { 'W'.Byte() } },
            { Key.E, new byte[] { 'E'.Byte() } },
            { Key.R, new byte[] { 'R'.Byte() } },
            { Key.T, new byte[] { 'T'.Byte() } },
            { Key.Y, new byte[] { 'Y'.Byte() } },
            { Key.U, new byte[] { 'U'.Byte() } },
            { Key.I, new byte[] { 'I'.Byte() } },
            { Key.O, new byte[] { 'O'.Byte() } },
            { Key.P, new byte[] { 'P'.Byte() } },
            { Key.A, new byte[] { 'A'.Byte() } },
            { Key.S, new byte[] { 'S'.Byte() } },
            { Key.D, new byte[] { 'D'.Byte() } },
            { Key.F, new byte[] { 'F'.Byte() } },
            { Key.G, new byte[] { 'G'.Byte() } },
            { Key.H, new byte[] { 'H'.Byte() } },
            { Key.J, new byte[] { 'J'.Byte() } },
            { Key.K, new byte[] { 'K'.Byte() } },
            { Key.L, new byte[] { 'L'.Byte() } },
            { Key.Z, new byte[] { 'Z'.Byte() } },
            { Key.X, new byte[] { 'X'.Byte() } },
            { Key.C, new byte[] { 'C'.Byte() } },
            { Key.V, new byte[] { 'V'.Byte() } },
            { Key.B, new byte[] { 'B'.Byte() } },
            { Key.N, new byte[] { 'N'.Byte() } },
            { Key.M, new byte[] { 'M'.Byte() } },

            /* 数字键 */
            { Key.NumPad0, new byte[] { '0'.Byte() } },
            { Key.NumPad1, new byte[] { '1'.Byte() } },
            { Key.NumPad2, new byte[] { '2'.Byte() } },
            { Key.NumPad3, new byte[] { '3'.Byte() } },
            { Key.NumPad4, new byte[] { '4'.Byte() } },
            { Key.NumPad5, new byte[] { '5'.Byte() } },
            { Key.NumPad6, new byte[] { '6'.Byte() } },
            { Key.NumPad7, new byte[] { '7'.Byte() } },
            { Key.NumPad8, new byte[] { '8'.Byte() } },
            { Key.NumPad9, new byte[] { '9'.Byte() } },

            /* 英文字母上方的数字键 */
            { Key.D0, new byte[] { '0'.Byte() } },
            { Key.D1, new byte[] { '1'.Byte() } },
            { Key.D2, new byte[] { '2'.Byte() } },
            { Key.D3, new byte[] { '3'.Byte() } },
            { Key.D4, new byte[] { '4'.Byte() } },
            { Key.D5, new byte[] { '5'.Byte() } },
            { Key.D6, new byte[] { '6'.Byte() } },
            { Key.D7, new byte[] { '7'.Byte() } },
            { Key.D8, new byte[] { '8'.Byte() } },
            { Key.D9, new byte[] { '9'.Byte() } },

            /* 方向键 */
            { Key.Up, new byte[] { 27, 91, 65 } }, /* esc,csi,a */
            { Key.Down, new byte[] { 27, 91, 66 } }, /* esc,csi,b */
            { Key.Left, new byte[] { 27, 91, 68 } }, /* esc,csi,d */
            { Key.Right, new byte[] { 27, 91, 67 } }, /* esc,csi,c */

            { Key.Enter, new byte[] { ANSI.ANSI_CR } },
            { Key.Space, new byte[] { ANSI.ANSI_SPACE } },
            { Key.Back, new byte[] { ANSI.ANSI_BS } },
            { Key.Tab, new byte[] { ANSI.ANSI_TAB } },
        };


        public static Dictionary<Key, byte[]> AnsiLowerTable = new Dictionary<Key, byte[]>
        {
            /* 英文字母键 */
            { Key.Q, new byte[] { 'Q'.Byte() } },
            { Key.W, new byte[] { 'W'.Byte() } },
            { Key.E, new byte[] { 'E'.Byte() } },
            { Key.R, new byte[] { 'R'.Byte() } },
            { Key.T, new byte[] { 'T'.Byte() } },
            { Key.Y, new byte[] { 'Y'.Byte() } },
            { Key.U, new byte[] { 'U'.Byte() } },
            { Key.I, new byte[] { 'I'.Byte() } },
            { Key.O, new byte[] { 'O'.Byte() } },
            { Key.P, new byte[] { 'P'.Byte() } },
            { Key.A, new byte[] { 'A'.Byte() } },
            { Key.S, new byte[] { 'S'.Byte() } },
            { Key.D, new byte[] { 'D'.Byte() } },
            { Key.F, new byte[] { 'F'.Byte() } },
            { Key.G, new byte[] { 'G'.Byte() } },
            { Key.H, new byte[] { 'H'.Byte() } },
            { Key.J, new byte[] { 'J'.Byte() } },
            { Key.K, new byte[] { 'K'.Byte() } },
            { Key.L, new byte[] { 'L'.Byte() } },
            { Key.Z, new byte[] { 'Z'.Byte() } },
            { Key.X, new byte[] { 'X'.Byte() } },
            { Key.C, new byte[] { 'C'.Byte() } },
            { Key.V, new byte[] { 'V'.Byte() } },
            { Key.B, new byte[] { 'B'.Byte() } },
            { Key.N, new byte[] { 'N'.Byte() } },
            { Key.M, new byte[] { 'M'.Byte() } },

            /* 数字键 */
            { Key.NumPad0, new byte[] { '0'.Byte() } },
            { Key.NumPad1, new byte[] { '1'.Byte() } },
            { Key.NumPad2, new byte[] { '2'.Byte() } },
            { Key.NumPad3, new byte[] { '3'.Byte() } },
            { Key.NumPad4, new byte[] { '4'.Byte() } },
            { Key.NumPad5, new byte[] { '5'.Byte() } },
            { Key.NumPad6, new byte[] { '6'.Byte() } },
            { Key.NumPad7, new byte[] { '7'.Byte() } },
            { Key.NumPad8, new byte[] { '8'.Byte() } },
            { Key.NumPad9, new byte[] { '9'.Byte() } },

            /* 英文字母上方的数字键 */
            { Key.D0, new byte[] { '0'.Byte() } },
            { Key.D1, new byte[] { '1'.Byte() } },
            { Key.D2, new byte[] { '2'.Byte() } },
            { Key.D3, new byte[] { '3'.Byte() } },
            { Key.D4, new byte[] { '4'.Byte() } },
            { Key.D5, new byte[] { '5'.Byte() } },
            { Key.D6, new byte[] { '6'.Byte() } },
            { Key.D7, new byte[] { '7'.Byte() } },
            { Key.D8, new byte[] { '8'.Byte() } },
            { Key.D9, new byte[] { '9'.Byte() } },

            /* 方向键 */
            { Key.Up, new byte[] { 27, 91, 65 } }, /* esc,csi,a */
            { Key.Down, new byte[] { 27, 91, 66 } }, /* esc,csi,b */
            { Key.Left, new byte[] { 27, 91, 68 } }, /* esc,csi,d */
            { Key.Right, new byte[] { 27, 91, 67 } }, /* esc,csi,c */

            { Key.Enter, new byte[] { ANSI.ANSI_CR } },
            { Key.Space, new byte[] { ANSI.ANSI_SPACE } },
            { Key.Back, new byte[] { ANSI.ANSI_BS } },
            { Key.Tab, new byte[] { ANSI.ANSI_TAB } },

        };

        public static Dictionary<Key, byte[]> AnsiUpperTable = new Dictionary<Key, byte[]>
        {
            /* 英文字母键 */
            { Key.Q, new byte[] { 'Q'.Byte() } },
            { Key.W, new byte[] { 'W'.Byte() } },
            { Key.E, new byte[] { 'E'.Byte() } },
            { Key.R, new byte[] { 'R'.Byte() } },
            { Key.T, new byte[] { 'T'.Byte() } },
            { Key.Y, new byte[] { 'Y'.Byte() } },
            { Key.U, new byte[] { 'U'.Byte() } },
            { Key.I, new byte[] { 'I'.Byte() } },
            { Key.O, new byte[] { 'O'.Byte() } },
            { Key.P, new byte[] { 'P'.Byte() } },
            { Key.A, new byte[] { 'A'.Byte() } },
            { Key.S, new byte[] { 'S'.Byte() } },
            { Key.D, new byte[] { 'D'.Byte() } },
            { Key.F, new byte[] { 'F'.Byte() } },
            { Key.G, new byte[] { 'G'.Byte() } },
            { Key.H, new byte[] { 'H'.Byte() } },
            { Key.J, new byte[] { 'J'.Byte() } },
            { Key.K, new byte[] { 'K'.Byte() } },
            { Key.L, new byte[] { 'L'.Byte() } },
            { Key.Z, new byte[] { 'Z'.Byte() } },
            { Key.X, new byte[] { 'X'.Byte() } },
            { Key.C, new byte[] { 'C'.Byte() } },
            { Key.V, new byte[] { 'V'.Byte() } },
            { Key.B, new byte[] { 'B'.Byte() } },
            { Key.N, new byte[] { 'N'.Byte() } },
            { Key.M, new byte[] { 'M'.Byte() } },

            /* 数字键 */
            { Key.NumPad0, new byte[] { '0'.Byte() } },
            { Key.NumPad1, new byte[] { '1'.Byte() } },
            { Key.NumPad2, new byte[] { '2'.Byte() } },
            { Key.NumPad3, new byte[] { '3'.Byte() } },
            { Key.NumPad4, new byte[] { '4'.Byte() } },
            { Key.NumPad5, new byte[] { '5'.Byte() } },
            { Key.NumPad6, new byte[] { '6'.Byte() } },
            { Key.NumPad7, new byte[] { '7'.Byte() } },
            { Key.NumPad8, new byte[] { '8'.Byte() } },
            { Key.NumPad9, new byte[] { '9'.Byte() } },

            /* 英文字母上方的数字键 */
            { Key.D0, new byte[] { '0'.Byte() } },
            { Key.D1, new byte[] { '1'.Byte() } },
            { Key.D2, new byte[] { '2'.Byte() } },
            { Key.D3, new byte[] { '3'.Byte() } },
            { Key.D4, new byte[] { '4'.Byte() } },
            { Key.D5, new byte[] { '5'.Byte() } },
            { Key.D6, new byte[] { '6'.Byte() } },
            { Key.D7, new byte[] { '7'.Byte() } },
            { Key.D8, new byte[] { '8'.Byte() } },
            { Key.D9, new byte[] { '9'.Byte() } },

            /* 方向键 */
            { Key.Up, new byte[] { 27, 91, 65 } }, /* esc,csi,a */
            { Key.Down, new byte[] { 27, 91, 66 } }, /* esc,csi,b */
            { Key.Left, new byte[] { 27, 91, 68 } }, /* esc,csi,d */
            { Key.Right, new byte[] { 27, 91, 67 } }, /* esc,csi,c */

            { Key.Enter, new byte[] { ANSI.ANSI_CR } },
            { Key.Space, new byte[] { ANSI.ANSI_SPACE } },
            { Key.Back, new byte[] { ANSI.ANSI_BS } },
            { Key.Tab, new byte[] { ANSI.ANSI_TAB } },
        };
    }
}
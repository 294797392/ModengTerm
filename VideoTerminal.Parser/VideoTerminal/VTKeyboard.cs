using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace VideoTerminal.Parser
{
    internal class VTKeyboardInstance : VTKeyboard
    {
        private static Dictionary<Key, byte[]> shiftPressedTable = new Dictionary<Key, byte[]>()
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
            { Key.D0, new byte[] { ')'.Byte() } },
            { Key.D1, new byte[] { '!'.Byte() } },
            { Key.D2, new byte[] { '@'.Byte() } },
            { Key.D3, new byte[] { '#'.Byte() } },
            { Key.D4, new byte[] { '$'.Byte() } },
            { Key.D5, new byte[] { '%'.Byte() } },
            { Key.D6, new byte[] { '^'.Byte() } },
            { Key.D7, new byte[] { '&'.Byte() } },
            { Key.D8, new byte[] { '*'.Byte() } },
            { Key.D9, new byte[] { '('.Byte() } },

            /* 其他可打印字符键 */
            { Key.OemMinus, new byte[] { '_'.Byte() } },
            { Key.OemPlus, new byte[] { '+'.Byte() } },
            { Key.OemComma, new byte[] { '<'.Byte() } },
            { Key.OemPeriod, new byte[] { '>'.Byte() } },
            { Key.OemQuestion, new byte[] { '?'.Byte() } },
            { Key.Oem3, new byte[] { '~'.Byte() } },
            { Key.Oem5, new byte[] { '|'.Byte() } },
            { Key.Oem1, new byte[] { ':'.Byte() } },
            { Key.OemQuotes, new byte[] { '"'.Byte() } },

            { Key.Enter, new byte[] { States.ANSI_CR } },
            { Key.Space, new byte[] { States.ANSI_SPACE } },
            { Key.Back, new byte[] { States.ANSI_BS } },
            { Key.Tab, new byte[] { States.ANSI_TAB } },
        };

        private static Dictionary<Key, byte[]> ansiUpperTable = new Dictionary<Key, byte[]>
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

            /* 其他可打印字符键 */
            { Key.OemMinus, new byte[] { '_'.Byte() } },
            { Key.OemPlus, new byte[] { '+'.Byte() } },
            { Key.OemPeriod, new byte[] { '.'.Byte() } },
            { Key.OemComma, new byte[] { ','.Byte() } },
            { Key.OemQuestion, new byte[] { '/'.Byte() } },
            { Key.Oem3, new byte[] { '`'.Byte() } },
            { Key.Oem5, new byte[] { '\\'.Byte() } },
            { Key.Oem1, new byte[] { ';'.Byte() } },
            { Key.OemQuotes, new byte[] { '\''.Byte() } },


            /* 方向键 */
            { Key.Up, new byte[] { 27, 91, 65 } }, /* esc,csi,a */
            { Key.Down, new byte[] { 27, 91, 66 } }, /* esc,csi,b */
            { Key.Left, new byte[] { 27, 91, 68 } }, /* esc,csi,d */
            { Key.Right, new byte[] { 27, 91, 67 } }, /* esc,csi,c */

            { Key.Enter, new byte[] { States.ANSI_CR } },
            { Key.Space, new byte[] { States.ANSI_SPACE } },
            { Key.Back, new byte[] { States.ANSI_BS } },
            { Key.Tab, new byte[] { States.ANSI_TAB } },
        };





        private static Dictionary<Key, byte[]> ctrlPressedTable = new Dictionary<Key, byte[]>
        {
            /* 英文字母键 */
            { Key.Q, new byte[] { "021".Octal2Byte() } },
            { Key.W, new byte[] { "027".Octal2Byte() } },
            { Key.E, new byte[] { "005".Octal2Byte() } },
            { Key.R, new byte[] { "022".Octal2Byte() } },
            { Key.T, new byte[] { "024".Octal2Byte() } },
            { Key.Y, new byte[] { "031".Octal2Byte() } },
            { Key.U, new byte[] { "025".Octal2Byte() } },
            { Key.I, new byte[] { "011".Octal2Byte() } },
            { Key.O, new byte[] { "017".Octal2Byte() } },
            { Key.P, new byte[] { "020".Octal2Byte() } },
            { Key.A, new byte[] { "001".Octal2Byte() } },
            { Key.S, new byte[] { "023".Octal2Byte() } },
            { Key.D, new byte[] { "004".Octal2Byte() } },
            { Key.F, new byte[] { "006".Octal2Byte() } },
            { Key.G, new byte[] { "007".Octal2Byte() } },
            { Key.H, new byte[] { "010".Octal2Byte() } },
            { Key.J, new byte[] { "012".Octal2Byte() } },
            { Key.K, new byte[] { "013".Octal2Byte() } },
            { Key.L, new byte[] { "014".Octal2Byte() } },
            { Key.Z, new byte[] { "032".Octal2Byte() } },
            { Key.X, new byte[] { "030".Octal2Byte() } },
            { Key.C, new byte[] { "003".Octal2Byte() } },
            { Key.V, new byte[] { "026".Octal2Byte() } },
            { Key.B, new byte[] { "002".Octal2Byte() } },
            { Key.N, new byte[] { "016".Octal2Byte() } },
            { Key.M, new byte[] { "015".Octal2Byte() } },

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

            /* 其他可打印字符键 */
            { Key.Oem1, new byte[] { ';'.Byte() } },
            { Key.OemQuotes, new byte[] { '\''.Byte() } },

            /* 方向键 */
            //{ Key.Up, new byte[] { 27, 91, 65 } }, /* esc,csi,a */
            //{ Key.Down, new byte[] { 27, 91, 66 } }, /* esc,csi,b */
            //{ Key.Left, new byte[] { 27, 91, 68 } }, /* esc,csi,d */
            //{ Key.Right, new byte[] { 27, 91, 67 } }, /* esc,csi,c */

            { Key.Enter, new byte[] { States.ANSI_CR } },
            { Key.Space, new byte[] { States.ANSI_SPACE } },
            { Key.Back, new byte[] { States.ANSI_BS } },
            { Key.Tab, new byte[] { States.ANSI_TAB } },
        };


        private static Dictionary<Key, byte[]> defaultTable = new Dictionary<Key, byte[]>
        {
            /* 英文字母键 */
            { Key.Q, new byte[] { 'q'.Byte() } },
            { Key.W, new byte[] { 'w'.Byte() } },
            { Key.E, new byte[] { 'e'.Byte() } },
            { Key.R, new byte[] { 'r'.Byte() } },
            { Key.T, new byte[] { 't'.Byte() } },
            { Key.Y, new byte[] { 'y'.Byte() } },
            { Key.U, new byte[] { 'u'.Byte() } },
            { Key.I, new byte[] { 'i'.Byte() } },
            { Key.O, new byte[] { 'o'.Byte() } },
            { Key.P, new byte[] { 'p'.Byte() } },
            { Key.A, new byte[] { 'a'.Byte() } },
            { Key.S, new byte[] { 's'.Byte() } },
            { Key.D, new byte[] { 'd'.Byte() } },
            { Key.F, new byte[] { 'f'.Byte() } },
            { Key.G, new byte[] { 'g'.Byte() } },
            { Key.H, new byte[] { 'h'.Byte() } },
            { Key.J, new byte[] { 'j'.Byte() } },
            { Key.K, new byte[] { 'k'.Byte() } },
            { Key.L, new byte[] { 'l'.Byte() } },
            { Key.Z, new byte[] { 'z'.Byte() } },
            { Key.X, new byte[] { 'x'.Byte() } },
            { Key.C, new byte[] { 'c'.Byte() } },
            { Key.V, new byte[] { 'v'.Byte() } },
            { Key.B, new byte[] { 'b'.Byte() } },
            { Key.N, new byte[] { 'n'.Byte() } },
            { Key.M, new byte[] { 'm'.Byte() } },

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

            /* 其他可打印字符键 */
            { Key.OemMinus, new byte[] { '-'.Byte() } }, /* 减号键 */
            { Key.OemPlus, new byte[] { '='.Byte() } }, /* 等于号键 */
            { Key.OemComma, new byte[] { ','.Byte() } },
            { Key.OemPeriod, new byte[] { '.'.Byte() } },
            { Key.OemQuestion, new byte[] { '/'.Byte() } },
            { Key.Oem3, new byte[] { '`'.Byte() } },
            { Key.Oem5, new byte[] { '\\'.Byte() } },
            { Key.Oem1, new byte[] { ';'.Byte() } },
            { Key.OemQuotes, new byte[] { '\''.Byte() } },


            /* 方向键 */
            { Key.Up, new byte[] { 27, 91, 65 } }, /* esc,csi,a */
            { Key.Down, new byte[] { 27, 91, 66 } }, /* esc,csi,b */
            { Key.Left, new byte[] { 27, 91, 68 } }, /* esc,csi,d */
            { Key.Right, new byte[] { 27, 91, 67 } }, /* esc,csi,c */

            { Key.Enter, new byte[] { States.ANSI_CR } },
            { Key.Space, new byte[] { States.ANSI_SPACE } },
            { Key.Back, new byte[] { States.ANSI_BS } },
            { Key.Tab, new byte[] { States.ANSI_TAB } },
        };

        public override Dictionary<Key, byte[]> ShiftPressedTable { get { return shiftPressedTable; } }

        public override Dictionary<Key, byte[]> ANSIUpperTable { get { return ansiUpperTable; } }

        public override Dictionary<Key, byte[]> CtrlPressedTable { get { return ctrlPressedTable; } }

        public override Dictionary<Key, byte[]> DefaultTable { get { return defaultTable; } }

        internal VTKeyboardInstance()
        {

        }
    }

    public abstract class VTKeyboard
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger("GVTKeyboard");

        public abstract Dictionary<Key, byte[]> ShiftPressedTable { get; }

        public abstract Dictionary<Key, byte[]> ANSIUpperTable { get; }

        public abstract Dictionary<Key, byte[]> CtrlPressedTable { get; }

        public abstract Dictionary<Key, byte[]> DefaultTable { get; }

        /// <summary>
        /// 获取大写是否锁定
        /// </summary>
        public bool CapsLocked
        {
            get
            {
                byte[] bs = new byte[256];
                WindowsAPI.GetKeyboardState(bs);
                return (bs[0x14] == 1);
            }
        }

        public bool HasDirectionsKeypad
        {
            get
            {
                return true;
            }
        }

        public bool HasMumericKeypad
        {
            get
            {
                return true;
            }
        }

        public byte[] ConvertKey(KeyEventArgs key)
        {
            Dictionary<Key, byte[]> inputTable = null;

            // 按下了Control键
            if ((Keyboard.Modifiers.HasFlag(ModifierKeys.Control)))
            {
                inputTable = this.CtrlPressedTable;
                goto Translate;
            }

            // 按下了Shift
            if ((Keyboard.Modifiers.HasFlag(ModifierKeys.Shift)))
            {
                inputTable = this.ShiftPressedTable;
                goto Translate;
            }

            // 打开了大写锁定
            if ((Keyboard.Modifiers.HasFlag(ModifierKeys.Shift)) || this.CapsLocked)
            {
                inputTable = this.ANSIUpperTable;
                goto Translate;
            }

            // 小写
            inputTable = this.DefaultTable;

            Translate:
            byte[] data;
            if (!inputTable.TryGetValue(key.Key, out data))
            {
                logger.ErrorFormat("未定义按键{0}", key.Key);
            }

            return data;
        }

        public static VTKeyboard Create()
        {
            return new VTKeyboardInstance();
        }
    }
}
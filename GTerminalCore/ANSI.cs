using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GTerminalCore
{
    public class ANSI
    {
        public const byte ANSI_CR = 13; // carriage return，回车，0x0D
        public const byte ANSI_TAB = 9;  // horizontal tab，制表符，0x09
        public const byte ANSI_ESC = 27; // escape，0x1b

        public const byte ANSI_EOT = 0x04;
        public const byte ANSI_BEL = 0x07;
        public const byte ANSI_BS = 0x08;   // backspace 退格，0x08
        public const byte ANSI_HT = 0x09;
        public const byte ANSI_LF = 0x0A;
        public const byte ANSI_VT = 0x0B;
        public const byte ANSI_FF = 0x0C;       /* C0, C1 control names		*/
        public const byte ANSI_SO = 0x0E;
        public const byte ANSI_SI = 0x0F;
        public const byte ANSI_XON = 0x11;      /* DC1 */
        public const byte ANSI_XOFF = 0x13;     /* DC3 */
        public const byte ANSI_NAK = 0x15;
        public const byte ANSI_CAN = 0x18;
        public const byte ANSI_SPA = 0x20;
        public const byte XTERM_POUND = 0x1E;       /* internal mapping for '#'	*/
        public const byte ANSI_DEL = 0x7F;
        public const byte ANSI_SS2 = 0x8E;
        public const byte ANSI_SS3 = 0x8F;
        public const byte ANSI_DCS = 0x90;
        public const byte ANSI_SOS = 0x98;
        public const byte ANSI_CSI = 0x9B;
        public const byte ANSI_ST = 0x9C;
        public const byte ANSI_OSC = 0x9D;
        public const byte ANSI_PM = 0x9E;
        public const byte ANSI_APC = 0x9F;

        public const byte ANSI_SPACE = 32;  // 空格

        public const byte ANSI_A = 65;
        public const byte ANSI_B = 66;
        public const byte ANSI_C = 67;
        public const byte ANSI_D = 68;
    }
}
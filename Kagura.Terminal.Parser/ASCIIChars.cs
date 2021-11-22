using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VideoTerminal.Parser
{
    /// <summary>
    /// 和ASCII码相关的帮助函数
    /// </summary>
    public static class ASCIIChars
    {
        // 从terminal项目拷贝的代码
        public const byte NUL = 0x0; // Null
        public const byte SOH = 0x1; // Start of Heading
        public const byte STX = 0x2; // Start of Text
        public const byte ETX = 0x3; // End of Text
        public const byte EOT = 0x4; // End of Transmission
        public const byte ENQ = 0x5; // Enquiry
        public const byte ACK = 0x6; // Acknowledge
        public const byte BEL = 0x7; // Bell
        public const byte BS = 0x8; // Backspace
        public const byte TAB = 0x9; // Horizontal Tab
        public const byte LF = 0xA; // Line Feed (new line)
        public const byte VT = 0xB; // Vertical Tab
        public const byte FF = 0xC; // Form Feed (new page)
        public const byte CR = 0xD; // Carriage Return
        public const byte SO = 0xE; // Shift Out
        public const byte SI = 0xF; // Shift In
        public const byte DLE = 0x10; // Data Link Escape
        public const byte DC1 = 0x11; // Device Control 1
        public const byte DC2 = 0x12; // Device Control 2
        public const byte DC3 = 0x13; // Device Control 3
        public const byte DC4 = 0x14; // Device Control 4
        public const byte NAK = 0x15; // Negative Acknowledge
        public const byte SYN = 0x16; // Synchronous Idle
        public const byte ETB = 0x17; // End of Transmission Block
        public const byte CAN = 0x18; // Cancel
        public const byte EM = 0x19; // End of Medium
        public const byte SUB = 0x1A; // Substitute
        public const byte ESC = 0x1B; // Escape
        public const byte FS = 0x1C; // File Separator
        public const byte GS = 0x1D; // Group Separator
        public const byte RS = 0x1E; // Record Separator
        public const byte US = 0x1F; // Unit Separator
        public const byte SPC = 0x20; // Space; first printable character
        public const byte DEL = 0x7F; // Delete

        /// <summary>
        /// 判断一个字节的字符是否是C1字符集
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        public static bool IsC1ControlCharacter(byte ch)
        {
            return ch >= 0x80 && ch <= 0x9F;
        }

        /// <summary>
        /// 判断在Ground状态下，该字符是否是一个控制字符
        /// </summary>
        /// <param name="ch">要判断的字符</param>
        /// <returns></returns>
        public static bool IsControlCharacter(byte ch)
        {
            if (ch <= ASCIIChars.US)
            {
                // 该字符是C0系列控制字符
                return true;
            }
            else if (IsC1ControlCharacter(ch))
            {
                // 该字符是C1系列控制字符
                return true;
            }
            else if (ch == ASCIIChars.DEL)
            {
                // 该字符是删除号，也是一个控制字符
                return true;
            }
            else
            {
                // 非控制字符
                return false;
            }
        }
    }
}

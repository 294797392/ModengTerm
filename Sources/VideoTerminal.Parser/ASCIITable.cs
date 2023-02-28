using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XTerminalParser
{
    /// <summary>
    /// 和ASCII码相关的帮助函数
    /// </summary>
    public static class ASCIITable
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
            if (ch <= ASCIITable.US)
            {
                // 该字符是C0系列控制字符
                return true;
            }
            else if (IsC1ControlCharacter(ch))
            {
                // 该字符是C1系列控制字符
                return true;
            }
            else if (ch == ASCIITable.DEL)
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

        /// <summary>
        /// 判断该字符是否是C0控制字符集
        /// </summary>
        /// <returns></returns>
        public static bool IsC0Code(byte ch)
        {
            return (ch >= ASCIITable.NUL && ch <= ASCIITable.ETB) ||
                ch == ASCIITable.EM ||
                (ch >= ASCIITable.FS && ch <= ASCIITable.US);
        }

        /// <summary>
        /// 判断该字符是否是Delete转义字符
        /// </summary>
        /// <returns></returns>
        public static bool IsDelete(byte ch)
        {
            return ch == ASCIITable.DEL;
        }

        /// <summary>
        /// 判断该字符是否是转义字符
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        public static bool IsEscape(byte ch)
        {
            return ch == ASCIITable.ESC;
        }

        /// <summary>
        /// 判断一个字符是否是可见字符
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        public static bool IsPrintable(byte ch)
        {
            return ch >= 0x20 && ch <= 0x7F;
        }

        /// <summary>
        /// 判断该字符是否从ESC状态进入到CSI状态
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        public static bool IsCSIIndicator(byte ch)
        {
            return ch == 0x5B;
        }

        /// <summary>
        /// 判断该字符是否从ESC状态进入到OSC状态
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        public static bool IsOSCIndicator(byte ch)
        {
            return ch == 0x5D;
        }

        /// <summary>
        /// 判断是否是OSC命令的结束符
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        public static bool IsOSCTerminator(byte ch)
        {
            // OSC使用BEL结束
            return ch == 0x07;
        }

        /// <summary>
        /// 判断该字符是否是OSC参数的分隔符
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        public static bool IsOSCDelimiter(byte ch)
        {
            return ch == ';';
        }

        public static bool IsOSCInvalid(byte ch)
        {
            return ch <= '\x17' ||
                ch == '\x19' ||
                (ch >= '\x1c' && ch <= '\x1f');
        }

        /// <summary>
        /// 判断字符是否是数字
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        public static bool IsNumericParamValue(byte ch)
        {
            return ch >= '0' && ch <= '9';
        }

        /// <summary>
        /// 判断是否是ST字符
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        public static bool IsStringTermination(byte ch)
        {
            return ch == 0x5C;
        }

        /// <summary>
        /// 判断某个字符是否是CSI的Intermediate字符
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        public static bool IsIntermediate(byte ch)
        {
            return ch >= ' ' && ch <= '/';
        }

        public static bool IsCSIInvalid(byte ch)
        {
            return ch == ':';   // 0x3A
        }

        public static bool IsParameterDelimiter(byte ch)
        {
            return ch == ';';
        }

        /// <summary>
        /// Determines if a character is an invalid parameter.
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        public static bool IsParameterInvalid(byte ch)
        {
            return IsCSIInvalid(ch) || IsCSIPrivateMarker(ch);
        }

        /// <summary>
        /// - Determines if a character is a private range marker for a control sequence.
        ///   Private range markers indicate vendor-specific behavior.
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        public static bool IsCSIPrivateMarker(byte ch)
        {
            return ch == '<' || ch == '=' || ch == '>' || ch == '?';
        }

        /// <summary>
        /// 判断是否是CSI无效字符
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        public static bool IsIntermediateInvalid(byte ch)
        {
            return IsNumericParamValue(ch) || IsCSIInvalid(ch) || IsParameterDelimiter(ch) || IsCSIPrivateMarker(ch);
        }

        /// <summary>
        /// Determines if a character is "device control string" beginning indicator.
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        public static bool IsDCSIndicator(byte ch)
        {
            return ch == 'P';
        }

        /// <summary>
        /// Determines if a character is valid for a DCS pass through sequence
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        public static bool IsDCSPassThroughValid(byte ch)
        {
            return ch >= ASCIITable.SPC && ch < ASCIITable.DEL;
        }

        /// <summary>
        /// 判断字符是否是VT52模式下移动光标的字符
        /// 
        /// Parameters for cursor movement are at the end of the ESC Y  escape sequence
        ///   Each ordinate is encoded in a single character as value+32.For example, !  is 1.  The screen coordinate system is 0-based
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        public static bool IsVt52CursorAddress(byte ch)
        {
            return ch == 'Y'; // 0x59
        }
    }
}

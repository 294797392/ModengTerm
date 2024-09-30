﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModengTerm.Terminal.Parsing
{
    /// <summary>
    /// https://www.w3school.com.cn/charsets/ref_utf_basic_latin.asp
    /// ASCII C0字符集：十进制 0-127，十六进制 0000-007F
    /// ASCII 控制字符：范围为 0-31，外加 127，旨在控制硬件设备。
    /// </summary>
    public enum ASCIITable
    {
        // 从terminal项目拷贝的代码
        NUL = 0x0, // Null
        SOH = 0x1, // Start of Heading
        STX = 0x2, // Start of Text
        ETX = 0x3, // End of Text
        EOT = 0x4, // End of Transmission
        ENQ = 0x5, // Enquiry
        ACK = 0x6, // Acknowledge
        BEL = 0x7, // Bell
        BS = 0x8, // Backspace
        TAB = 0x9, // Horizontal Tab
        LF = 0xA, // Line Feed (new line)
        VT = 0xB, // Vertical Tab
        FF = 0xC, // Form Feed (new page)
        CR = 0xD, // Carriage Return
        SO = 0xE, // Shift Out
        SI = 0xF, // Shift In
        DLE = 0x10, // Data Link Escape
        DC1 = 0x11, // Device Control 1
        DC2 = 0x12, // Device Control 2
        DC3 = 0x13, // Device Control 3
        DC4 = 0x14, // Device Control 4
        NAK = 0x15, // Negative Acknowledge
        SYN = 0x16, // Synchronous Idle
        ETB = 0x17, // End of Transmission Block
        CAN = 0x18, // Cancel
        EM = 0x19, // End of Medium
        SUB = 0x1A, // Substitute
        ESC = 0x1B, // Escape
        FS = 0x1C, // File Separator
        GS = 0x1D, // Group Separator
        RS = 0x1E, // Record Separator
        US = 0x1F, // Unit Separator
        SPC = 0x20, // Space, first printable character
        DEL = 0x7F, // Delete
    }
}

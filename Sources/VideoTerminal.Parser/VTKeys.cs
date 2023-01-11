using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XTerminalParser
{
    public enum VTKeys
    {
        //
        // 摘要:
        //     BACKSPACE 键。
        Backspace = 8,
        //
        // 摘要:
        //     TAB 键。
        Tab = 9,
        //
        // 摘要:
        //     CLEAR 键。
        Clear = 12,
        //
        // 摘要:
        //     ENTER 键。
        Enter = 13,
        //
        // 摘要:
        //     PAUSE 键。
        Pause = 19,
        //
        // 摘要:
        //     ESC (ESCAPE) 键。
        Escape = 27,
        //
        // 摘要:
        //     SPACEBAR 键。
        Spacebar = 32,
        //
        // 摘要:
        //     PAGE UP 键。
        PageUp = 33,
        //
        // 摘要:
        //     PAGE DOWN 键。
        PageDown = 34,
        //
        // 摘要:
        //     END 键。
        End = 35,
        //
        // 摘要:
        //     HOME 键。
        Home = 36,
        //
        // 摘要:
        //     LEFT ARROW 键。
        LeftArrow = 37,
        //
        // 摘要:
        //     UP ARROW 键。
        UpArrow = 38,
        //
        // 摘要:
        //     RIGHT ARROW 键。
        RightArrow = 39,
        //
        // 摘要:
        //     DOWN ARROW 键。
        DownArrow = 40,
        //
        // 摘要:
        //     SELECT 键。
        Select = 41,
        //
        // 摘要:
        //     PRINT 键。
        Print = 42,
        //
        // 摘要:
        //     EXECUTE 键。
        Execute = 43,
        //
        // 摘要:
        //     PRINT SCREEN 键。
        PrintScreen = 44,
        //
        // 摘要:
        //     INS (INSERT) 键。
        Insert = 45,
        //
        // 摘要:
        //     DEL (DELETE) 键。
        Delete = 46,
        //
        // 摘要:
        //     HELP 键。
        Help = 47,
        //
        // 摘要:
        //     0 键。
        D0 = 48,
        //
        // 摘要:
        //     1 键。
        D1 = 49,
        //
        // 摘要:
        //     2 键。
        D2 = 50,
        //
        // 摘要:
        //     3 键。
        D3 = 51,
        //
        // 摘要:
        //     4 键。
        D4 = 52,
        //
        // 摘要:
        //     5 键。
        D5 = 53,
        //
        // 摘要:
        //     6 键。
        D6 = 54,
        //
        // 摘要:
        //     7 键。
        D7 = 55,
        //
        // 摘要:
        //     8 键。
        D8 = 56,
        //
        // 摘要:
        //     9 键。
        D9 = 57,
        //
        // 摘要:
        //     A 键。
        A = 65,
        //
        // 摘要:
        //     B 键。
        B = 66,
        //
        // 摘要:
        //     C 键。
        C = 67,
        //
        // 摘要:
        //     D 键。
        D = 68,
        //
        // 摘要:
        //     E 键。
        E = 69,
        //
        // 摘要:
        //     F 键。
        F = 70,
        //
        // 摘要:
        //     G 键。
        G = 71,
        //
        // 摘要:
        //     H 键。
        H = 72,
        //
        // 摘要:
        //     I 键。
        I = 73,
        //
        // 摘要:
        //     J 键。
        J = 74,
        //
        // 摘要:
        //     K 键。
        K = 75,
        //
        // 摘要:
        //     L 键。
        L = 76,
        //
        // 摘要:
        //     M 键。
        M = 77,
        //
        // 摘要:
        //     N 键。
        N = 78,
        //
        // 摘要:
        //     O 键。
        O = 79,
        //
        // 摘要:
        //     P 键。
        P = 80,
        //
        // 摘要:
        //     Q 键。
        Q = 81,
        //
        // 摘要:
        //     R 键。
        R = 82,
        //
        // 摘要:
        //     S 键。
        S = 83,
        //
        // 摘要:
        //     T 键。
        T = 84,
        //
        // 摘要:
        //     U 键。
        U = 85,
        //
        // 摘要:
        //     V 键。
        V = 86,
        //
        // 摘要:
        //     W 键。
        W = 87,
        //
        // 摘要:
        //     X 键。
        X = 88,
        //
        // 摘要:
        //     Y 键。
        Y = 89,
        //
        // 摘要:
        //     Z 键。
        Z = 90,
        //
        // 摘要:
        //     左 Windows 徽标键 (Microsoft Natural Keyboard)。
        LeftWindows = 91,
        //
        // 摘要:
        //     右 Windows 徽标键 (Microsoft Natural Keyboard)。
        RightWindows = 92,
        //
        // 摘要:
        //     应用程序键 (Microsoft Natural Keyboard)。
        Applications = 93,
        //
        // 摘要:
        //     计算机休眠键。
        Sleep = 95,
        //
        // 摘要:
        //     数字键盘上的 0 键。
        NumPad0 = 96,
        //
        // 摘要:
        //     数字键盘上的 1 键。
        NumPad1 = 97,
        //
        // 摘要:
        //     数字键盘上的 2 键。
        NumPad2 = 98,
        //
        // 摘要:
        //     数字键盘上的 3 键。
        NumPad3 = 99,
        //
        // 摘要:
        //     数字键盘上的 4 键。
        NumPad4 = 100,
        //
        // 摘要:
        //     数字键盘上的 5 键。
        NumPad5 = 101,
        //
        // 摘要:
        //     数字键盘上的 6 键。
        NumPad6 = 102,
        //
        // 摘要:
        //     数字键盘上的 7 键。
        NumPad7 = 103,
        //
        // 摘要:
        //     数字键盘上的 8 键。
        NumPad8 = 104,
        //
        // 摘要:
        //     数字键盘上的 9 键。
        NumPad9 = 105,
        //
        // 摘要:
        //     乘号键（数字键盘上的乘号键）。
        Multiply = 106,
        //
        // 摘要:
        //     加号键（数字键盘上的加号键）。
        Add = 107,
        //
        // 摘要:
        //     分隔符键。
        Separator = 108,
        //
        // 摘要:
        //     减号键（数字键盘上的减号键）。
        Subtract = 109,
        //
        // 摘要:
        //     小数点键（数字键盘上的小数点键）。
        Decimal = 110,
        //
        // 摘要:
        //     除号键（数字键盘上的除号键）。
        Divide = 111,
        //
        // 摘要:
        //     F1 键。
        F1 = 112,
        //
        // 摘要:
        //     F2 键。
        F2 = 113,
        //
        // 摘要:
        //     F3 键。
        F3 = 114,
        //
        // 摘要:
        //     F4 键。
        F4 = 115,
        //
        // 摘要:
        //     F5 键。
        F5 = 116,
        //
        // 摘要:
        //     F6 键。
        F6 = 117,
        //
        // 摘要:
        //     F7 键。
        F7 = 118,
        //
        // 摘要:
        //     F8 键。
        F8 = 119,
        //
        // 摘要:
        //     F9 键。
        F9 = 120,
        //
        // 摘要:
        //     F10 键。
        F10 = 121,
        //
        // 摘要:
        //     F11 键。
        F11 = 122,
        //
        // 摘要:
        //     F12 键。
        F12 = 123,
        //
        // 摘要:
        //     F13 键。
        F13 = 124,
        //
        // 摘要:
        //     F14 键。
        F14 = 125,
        //
        // 摘要:
        //     F15 键。
        F15 = 126,
        //
        // 摘要:
        //     F16 键。
        F16 = 127,
        //
        // 摘要:
        //     F17 键。
        F17 = 128,
        //
        // 摘要:
        //     F18 键。
        F18 = 129,
        //
        // 摘要:
        //     F19 键。
        F19 = 130,
        //
        // 摘要:
        //     F20 键。
        F20 = 131,
        //
        // 摘要:
        //     F21 键。
        F21 = 132,
        //
        // 摘要:
        //     F22 键。
        F22 = 133,
        //
        // 摘要:
        //     F23 键。
        F23 = 134,
        //
        // 摘要:
        //     F24 键。
        F24 = 135,
        //
        // 摘要:
        //     浏览器后退键（Windows 2000 或更高版本）。
        BrowserBack = 166,
        //
        // 摘要:
        //     浏览器前进键（Windows 2000 或更高版本）。
        BrowserForward = 167,
        //
        // 摘要:
        //     浏览器刷新键（Windows 2000 或更高版本）。
        BrowserRefresh = 168,
        //
        // 摘要:
        //     浏览器停止键（Windows 2000 或更高版本）。
        BrowserStop = 169,
        //
        // 摘要:
        //     浏览器搜索键（Windows 2000 或更高版本）。
        BrowserSearch = 170,
        //
        // 摘要:
        //     浏览器收藏键（Windows 2000 或更高版本）。
        BrowserFavorites = 171,
        //
        // 摘要:
        //     浏览器主页键（Windows 2000 或更高版本）。
        BrowserHome = 172,
        //
        // 摘要:
        //     静音键（Microsoft Natural Keyboard，Windows 2000 或更高版本）。
        VolumeMute = 173,
        //
        // 摘要:
        //     调低音量键（Microsoft Natural Keyboard，Windows 2000 或更高版本）。
        VolumeDown = 174,
        //
        // 摘要:
        //     调高音量键（Microsoft Natural Keyboard，Windows 2000 或更高版本）。
        VolumeUp = 175,
        //
        // 摘要:
        //     媒体下一曲目键（Windows 2000 或更高版本）。
        MediaNext = 176,
        //
        // 摘要:
        //     媒体上一曲目键（Windows 2000 或更高版本）。
        MediaPrevious = 177,
        //
        // 摘要:
        //     媒体停止键（Windows 2000 或更高版本）。
        MediaStop = 178,
        //
        // 摘要:
        //     媒体播放/暂停键（Windows 2000 或更高版本）。
        MediaPlay = 179,
        //
        // 摘要:
        //     启动邮件键（Microsoft Natural Keyboard，Windows 2000 或更高版本）。
        LaunchMail = 180,
        //
        // 摘要:
        //     选择媒体键（Microsoft Natural Keyboard，Windows 2000 或更高版本）。
        LaunchMediaSelect = 181,
        //
        // 摘要:
        //     启动应用程序 1 键（Microsoft Natural Keyboard，Windows 2000 或更高版本）。
        LaunchApp1 = 182,
        //
        // 摘要:
        //     启动应用程序 2 键（Microsoft Natural Keyboard，Windows 2000 或更高版本）。
        LaunchApp2 = 183,
        //
        // 摘要:
        //     OEM 1 键（特定于 OEM）。
        Oem1 = 186,
        //
        // 摘要:
        //     任何国家/地区键盘上的 OEM 加号键（Windows 2000 或更高版本）。
        OemPlus = 187,
        //
        // 摘要:
        //     任何国家/地区键盘上的 OEM 逗号键（Windows 2000 或更高版本）。
        OemComma = 188,
        //
        // 摘要:
        //     任何国家/地区键盘上的 OEM 减号键（Windows 2000 或更高版本）。
        OemMinus = 189,
        //
        // 摘要:
        //     任何国家/地区键盘上的 OEM 句号键（Windows 2000 或更高版本）。
        OemPeriod = 190,
        //
        // 摘要:
        //     OEM 2 键（特定于 OEM）。
        Oem2 = 191,
        //
        // 摘要:
        //     OEM 3 键（特定于 OEM）。
        Oem3 = 192,
        //
        // 摘要:
        //     OEM 4 键（特定于 OEM）。
        Oem4 = 219,
        //
        // 摘要:
        //     OEM 5 键（特定于 OEM）。
        Oem5 = 220,
        //
        // 摘要:
        //     OEM 6 键（特定于 OEM）。
        Oem6 = 221,
        //
        // 摘要:
        //     OEM 7 键（特定于 OEM）。
        Oem7 = 222,
        //
        // 摘要:
        //     OEM 8 键（特定于 OEM）。
        Oem8 = 223,
        //
        // 摘要:
        //     OEM 102 键（特定于 OEM）。
        Oem102 = 226,
        //
        // 摘要:
        //     IME PROCESS 键。
        Process = 229,
        //
        // 摘要:
        //     PACKET 键（通过击键次数用于传递 Unicode 字符）。
        Packet = 231,
        //
        // 摘要:
        //     ATTN 键。
        Attention = 246,
        //
        // 摘要:
        //     CRSEL（光标选择）键。
        CrSel = 247,
        //
        // 摘要:
        //     EXSEL（扩展选择）键。
        ExSel = 248,
        //
        // 摘要:
        //     ERASE EOF 键。
        EraseEndOfFile = 249,
        //
        // 摘要:
        //     播放键。
        Play = 250,
        //
        // 摘要:
        //     缩放键。
        Zoom = 251,
        //
        // 摘要:
        //     留待将来使用的常数。
        NoName = 252,
        //
        // 摘要:
        //     PA1 键。
        Pa1 = 253,
        //
        // 摘要:
        //     CLEAR 键（特定于 OEM）。
        OemClear = 254
    }

    public static class VTKeyConverter
    {
        public static VTKeys ConvertConsoleKey(ConsoleKey key)
        {
            return (VTKeys)key;
        }
    }
}


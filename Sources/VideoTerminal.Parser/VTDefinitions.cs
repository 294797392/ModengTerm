using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XTerminalParser
{
    public enum GraphicsOptions
    {
        Off = 0,
        BoldBright = 1,
        // The 2 and 5 entries here are for BOTH the extended graphics options,
        // as well as the Faint/Blink options.
        RGBColorOrFaint = 2, // 2 is also Faint, decreased intensity (ISO 6429).
        Italics = 3,
        Underline = 4,

        /// <summary>
        /// slowly blinking（less then 150 per minute）
        /// </summary>
        BlinkOrXterm256Index = 5, // 5 is also Blink (appears as Bold).

        /// <summary>
        /// rapidly blinking（150 per minute or more）
        /// </summary>
        RapidBlink = 6,
        Negative = 7,
        Invisible = 8,
        CrossedOut = 9,
        DoublyUnderlined = 21,
        NotBoldOrFaint = 22,
        NotItalics = 23,
        NoUnderline = 24,
        Steady = 25, // _not_ blink
        Positive = 27, // _not_ inverse
        Visible = 28, // _not_ invisible
        NotCrossedOut = 29,
        ForegroundBlack = 30,
        ForegroundRed = 31,
        ForegroundGreen = 32,
        ForegroundYellow = 33,
        ForegroundBlue = 34,
        ForegroundMagenta = 35,
        ForegroundCyan = 36,
        ForegroundWhite = 37,
        ForegroundExtended = 38,
        ForegroundDefault = 39,
        BackgroundBlack = 40,
        BackgroundRed = 41,
        BackgroundGreen = 42,
        BackgroundYellow = 43,
        BackgroundBlue = 44,
        BackgroundMagenta = 45,
        BackgroundCyan = 46,
        BackgroundWhite = 47,
        BackgroundExtended = 48,
        BackgroundDefault = 49,
        Overline = 53,
        NoOverline = 55,
        BrightForegroundBlack = 90,
        BrightForegroundRed = 91,
        BrightForegroundGreen = 92,
        BrightForegroundYellow = 93,
        BrightForegroundBlue = 94,
        BrightForegroundMagenta = 95,
        BrightForegroundCyan = 96,
        BrightForegroundWhite = 97,
        BrightBackgroundBlack = 100,
        BrightBackgroundRed = 101,
        BrightBackgroundGreen = 102,
        BrightBackgroundYellow = 103,
        BrightBackgroundBlue = 104,
        BrightBackgroundMagenta = 105,
        BrightBackgroundCyan = 106,
        BrightBackgroundWhite = 107,
    }

    /// <summary>
    /// DCS命令处理程序
    /// </summary>
    /// <param name="ch"></param>
    /// <returns></returns>
    internal delegate bool DCSStringHandlerDlg(byte ch);

    internal enum EraseType
    {
        ToEnd = 0,
        FromBeginning = 1,
        All = 2,
        Scrollback = 3
    }

    internal enum WindowManipulationType
    {
        Invalid = 0,
        RefreshWindow = 7,
        ResizeWindowInCharacters = 8,
    }

    /// <summary>
    /// 定义CSI命令
    /// </summary>
    internal enum CSIActionCodes
    {
        ICH_InsertCharacter = '@',
        CUU_CursorUp = 'A',
        CUD_CursorDown = 'B',
        CUF_CursorForward = 'C',
        CUB_CursorBackward = 'D',
        CNL_CursorNextLine = 'E',
        CPL_CursorPrevLine = 'F',
        CHA_CursorHorizontalAbsolute = 'G',

        /// <summary>
        /// 移动光标位置
        /// CursorPosition
        /// </summary>
        CUP_CursorPosition = 'H',
        CHT_CursorForwardTab = 'I',
        ED_EraseDisplay = 'J',
        EL_EraseLine = 'K',
        IL_InsertLine = 'L',
        DL_DeleteLine = 'M',
        DCH_DeleteCharacter = 'P',
        SU_ScrollUp = 'S',
        SD_ScrollDown = 'T',
        ECH_EraseCharacters = 'X',
        CBT_CursorBackTab = 'Z',
        HPA_HorizontalPositionAbsolute = '`',
        HPR_HorizontalPositionRelative = 'a',
        REP_RepeatCharacter = 'b',
        DA_DeviceAttributes = 'c',
        //DA2_SecondaryDeviceAttributes = '>c',
        //DA3_TertiaryDeviceAttributes = '=c',
        VPA_VerticalLinePositionAbsolute = 'd',
        VPR_VerticalPositionRelative = 'e',

        /// <summary>
        /// 等同于CUP_CursorPosition
        /// Character and line Position
        /// </summary>
        HVP_HorizontalVerticalPosition = 'f',
        TBC_TabClear = 'g',

        /// <summary>
        /// 设置DECPrivateMode
        /// </summary>
        DECSET_PrivateModeSet = 'h',

        /// <summary>
        /// 重置DECPrivateModde
        /// DEC Reset
        /// </summary>
        DECRST_PrivateModeReset = 'l',

        /// <summary>
        /// 执行SGR操作
        /// 一般都是设置一些文本的颜色和格式，比方说下划线，文本的粗细等等...
        /// </summary>
        SGR_SetGraphicsRendition = 'm',

        DSR_DeviceStatusReport = 'n',

        /// <summary>
        /// Set Scrolling Region
        /// 
        /// - DECSTBM - Set Scrolling Region
        /// This control function sets the top and bottom margins for the current page.
        ///  You cannot perform scrolling outside the margins.
        ///  Default: Margins are at the page limits.
        ///  
        /// 设置当前页面的上边距和下边距
        /// 滚动条不能超出边距范围
        /// 默认的边距和页面大小一致
        /// </summary>
        DECSTBM_SetScrollingRegion = 'r',
        ANSISYSSC_CursorSave = 's', // NOTE: Overlaps with DECLRMM/DECSLRM. Fix when/if implemented.

        /// <summary>
        /// 执行与窗口相关的操作
        /// </summary>
        DTTERM_WindowManipulation = 't', // NOTE: Overlaps with DECSLPP. Fix when/if implemented.
        ANSISYSRC_CursorRestore = 'u',
        DECREQTPARM_RequestTerminalParameters = 'x',
        //DECSCUSR_SetCursorStyle = ' q',
        //DECSTR_SoftReset = '!p',
        //XT_PushSgrAlias = '#p',
        //XT_PopSgrAlias = '#q',
        //XT_PushSgr = '#{',
        //XT_PopSgr = '#}',
        //DECSCPP_SetColumnsPerPage = '$|',
    }

    /// <summary>
    /// CSI参数下DECSET命令的DECPrivateMode类型
    /// </summary>
    internal enum DECPrivateMode
    {
        /// <summary>
        /// 设置光标键操作模式为ApplicationMode或者NormalMode
        /// ApplicationMode和NormalMode下，小键盘的每个按键的功能不一样，也就是要发送的字符序列不一样
        /// DECCKM:Cursor keys
        /// 光标键就是上下左右键
        /// </summary>
        DECCKM_CursorKeysMode = 1,

        /// <summary>
        /// 设置终端的数据流解析模式为ANSI或者VT52模式
        /// </summary>
        DECANM_AnsiMode = 2,

        DECCOLM_SetNumberOfColumns = 3,
        DECSCNM_ScreenMode = 5,
        DECOM_OriginMode = 6,
        DECAWM_AutoWrapMode = 7,
        ATT610_StartCursorBlink = 12,

        /// <summary>
        /// 参考：
        /// terminal - adaptDispatch.cpp UseAlternateScreenBuffer
        /// https://invisible-island.net/xterm/ctlseqs/ctlseqs.html#h3-Operating-System-Commands
        /// 显示或者隐藏光标
        /// </summary>
        DECTCEM_TextCursorEnableMode = 25,
        XTERM_EnableDECCOLMSupport = 40,
        VT200_MOUSE_MODE = 1000,
        BUTTON_EVENT_MOUSE_MODE = 1002,
        ANY_EVENT_MOUSE_MODE = 1003,
        UTF8_EXTENDED_MODE = 1005,
        SGR_EXTENDED_MODE = 1006,
        ALTERNATE_SCROLL = 1007,

        /// <summary>
        /// 参考：
        /// terminal - adaptDispatch.cpp UseAlternateScreenBuffer
        /// https://invisible-island.net/xterm/ctlseqs/ctlseqs.html#h3-Operating-System-Commands
        /// 
        /// - ASBSET - Creates and swaps to the alternate screen buffer. In virtual terminals, there exists both a "main"
        ///     screen buffer and an alternate. ASBSET creates a new alternate, and switches to it. If there is an already
        ///     existing alternate, it is discarded.
        /// 
        /// 虚拟终端有两个屏幕缓冲区，一个是主缓冲区，另外一个是备用缓冲区
        /// 该事件需要做两件事：
        /// 1. 保存主缓冲区里光标的状态
        /// 2. 创建一个备用缓冲区，并切换到备用缓冲区，如果已经存在了一个备用缓冲区，那么丢弃备用缓冲区
        /// </summary>
        ASB_AlternateScreenBuffer = 1049,

        /// <summary>
        /// Set bracketed paste mode
        /// </summary>
        XTERM_BracketedPasteMode = 2004,
        W32IM_Win32InputMode = 9001
    }

    /// <summary>
    /// 定义8位转义字符命令
    /// </summary>
    internal enum EscActionCodes
    {
        DECSC_CursorSave = '7',
        DECRC_CursorRestore = '8',

        /// <summary>
        /// ApplicationMode可以把数字键盘当成上下左右键使用
        /// </summary>
        DECKPAM_KeypadApplicationMode = '=',

        /// <summary>
        /// NumericMode模式下，数字键盘的功能就是输入数字
        /// </summary>
        DECKPNM_KeypadNumericMode = '>',
        IND_Index = 'D',
        NEL_NextLine = 'E',
        HTS_HorizontalTabSet = 'H',
        RI_ReverseLineFeed = 'M',
        SS2_SingleShift = 'N',
        SS3_SingleShift = 'O',
        DECID_IdentifyDevice = 'Z',
        ST_StringTerminator = '\\',
        RIS_ResetToInitialState = 'c',
        LS2_LockingShift = 'n',
        LS3_LockingShift = 'o',
        LS1R_LockingShift = '~',
        LS2R_LockingShift = '}',
        LS3R_LockingShift = '|',
        //DECAC1_AcceptC1Controls = ' 7',
        //DECDHL_DoubleHeightLineTop = '#3',
        //DECDHL_DoubleHeightLineBottom = '#4',
        //DECSWL_SingleWidthLine = '#5',
        //DECDWL_DoubleWidthLine = '#6',
        //DECALN_ScreenAlignmentPattern = '#8'
    }

    /// <summary>
    /// 定义在VT52模式下的终端指令
    /// VT52模式通过CSI指令开启
    /// </summary>
    internal enum VT52ActionCodes
    {
        CursorUp = 'A',
        CursorDown = 'B',
        CursorRight = 'C',
        CursorLeft = 'D',
        EnterGraphicsMode = 'F',
        ExitGraphicsMode = 'G',
        CursorToHome = 'H',
        ReverseLineFeed = 'I',
        EraseToEndOfScreen = 'J',
        EraseToEndOfLine = 'K',
        DirectCursorAddress = 'Y',
        Identify = 'Z',
        EnterAlternateKeypadMode = '=',
        ExitAlternateKeypadMode = '>',
        ExitVt52Mode = '<'
    }
}

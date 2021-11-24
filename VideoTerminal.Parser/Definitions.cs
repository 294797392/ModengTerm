using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VideoTerminal.Parser
{
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
    internal enum DECSETPrivateModeSet
    {
        DECCKM_CursorKeysMode = 1,
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
}

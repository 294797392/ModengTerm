using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XTerminal.Parser
{
    public enum VTActions
    {
        /// <summary>
        /// 播放提示声
        /// </summary>
        PlayBell,

        /// <summary>
        /// 打印字符
        /// </summary>
        Print,

        /// <summary>
        /// 还原字体属性
        /// </summary>
        DefaultAttributes,
        DefaultBackground,
        DefaultForeground,

        /// <summary>
        /// 字体设置为粗体
        /// </summary>
        Bold,
        BoldUnset,

        /// <summary>
        /// 字体设置为斜体
        /// </summary>
        Italics,
        ItalicsUnset,

        /// <summary>
        /// 设置字体下划线
        /// </summary>
        Underline,
        UnderlineUnset,

        /// <summary>
        /// 设置双下划线
        /// </summary>
        DoublyUnderlined,
        DoublyUnderlinedUnset,

        /// <summary>
        /// 设置上划线
        /// </summary>
        Overlined,
        OverlinedUnset,

        /// <summary>
        /// 设置前景色
        /// 用Color表示要设置的前景色
        /// </summary>
        Foreground,

        /// <summary>
        /// 设置背景色
        /// 用Color表示要设置的背景色
        /// </summary>
        Background,

        /// <summary>
        /// 降低颜色强度
        /// </summary>
        Faint,
        FaintUnset,

        /// <summary>
        /// 闪烁光标
        /// </summary>
        Blink,
        BlinkUnset,

        /// <summary>
        /// 隐藏字符
        /// </summary>
        Invisible,
        InvisibleUnset,

        /// <summary>
        /// characters still legible but marked as to be deleted
        /// 仍可读但标记为可删除的字符
        /// </summary>
        CrossedOut,
        CrossedOutUnset,

        /// <summary>
        /// 图像反色？
        /// </summary>
        ReverseVideo,
        ReverseVideoUnset,

        /// <summary>
        /// 用RGB表示前景色
        /// </summary>
        ForegroundRGB,

        /// <summary>
        /// 用RGB表示背景色
        /// </summary>
        BackgroundRGB,

        /// <summary>
        /// CR - Performs a carriage return.
        /// Moves the cursor to the leftmost column
        /// </summary>
        CarriageReturn,
        LF,
        FF,
        VT,

        /// <summary>
        /// 删除行
        /// </summary>
        EL_EraseLine,

        /// <summary>
        /// This sequence erases some or all of the characters in the display according to the parameter. Any complete line erased by this sequence will return that line to single width mode. Editor Function
        /// 0	Erase from the active position to the end of the screen, inclusive (default)
        /// 1	Erase from start of the screen to the active position, inclusive
        /// 2	Erase all of the display -- all lines are erased, changed to single-width, and the cursor does not move.
        /// </summary>
        ED_EraseDisplay,

        /// <summary>
        /// 光标向左移动N个字符的位置
        /// </summary>
        CursorBackward,
        /// <summary>
        /// 光标向右移动N个字符的位置
        /// </summary>
        CUF_CursorForward,
        /// <summary>
        /// 光标向上移动N个字符的位置
        /// </summary>
        CUU_CursorUp,
        /// <summary>
        /// 光标向下移动N个字符的位置
        /// </summary>
        CUD_CursorDown,

        /// <summary>
        /// Tab键
        /// </summary>
        ForwardTab,

        /// <summary>
        /// 显示光标
        /// </summary>
        CursorVisible,

        /// <summary>
        /// 隐藏光标
        /// </summary>
        CursorHiden,

        /// <summary>
        /// 移动光标
        /// </summary>
        CUP_CursorPosition,

        #region DECPrivateMode

        /// <summary>
        /// 设置光标键输入模式
        /// </summary>
        SetCursorKeyMode,

        /// <summary>
        /// 设置辅助键盘（右边的数字小键盘）输入模式
        /// </summary>
        SetKeypadMode,

        /// <summary>
        /// 设置终端模式
        /// VT52和ANSI两种模式
        /// </summary>
        SetVTMode,
        AutoWrapMode,
        XTERM_BracketedPasteMode,
        ATT610_StartCursorBlink,
        DECTCEM_TextCursorEnableMode,

        #endregion

        /// <summary>
        /// 从当前光标处删除字符
        /// </summary>
        DCH_DeleteCharacter,

        /// <summary>
        /// 在当前光标处插入N个空白字符
        /// </summary>
        ICH_InsertCharacter,

        UseAlternateScreenBuffer,
        UseMainScreenBuffer,

        DSR_DeviceStatusReport,
        DA_DeviceAttributes,

        RI_ReverseLineFeed
    }
}

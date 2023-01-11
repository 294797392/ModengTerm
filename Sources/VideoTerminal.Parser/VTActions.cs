using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XTerminalParser
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
        LineFeed,


        /// <summary>
        /// 光标向左移动N个字符的距离
        /// </summary>
        CursorBackward,
        /// <summary>
        /// 光标向右移动N个字符的距离
        /// </summary>
        CursorForword,

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
        CursorPosition
    }
}

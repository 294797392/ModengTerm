using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VideoTerminal.Base;

namespace VTInterface
{
    public class VTAction
    {
        public static readonly VTAction DefaultAttributeAction = new VTAction(VTActions.DefaultAttributes);
        public static readonly VTAction DefaultBackgroundAction = new VTAction(VTActions.DefaultBackground);
        public static readonly VTAction DefaultForegroundAction = new VTAction(VTActions.DefaultForeground);

        public static readonly VTAction ReverseVideoAction = new VTAction(VTActions.ReverseVideo);
        public static readonly VTAction ReverseVideoUnsetAction = new VTAction(VTActions.ReverseVideoUnset);

        public static readonly VTAction CrossedOutAction = new VTAction(VTActions.CrossedOut);
        public static readonly VTAction CrossedOutUnsetAction = new VTAction(VTActions.CrossedOutUnset);

        public static readonly VTAction FaintAction = new VTAction(VTActions.Faint);
        public static readonly VTAction FaintUnsetAction = new VTAction(VTActions.FaintUnset);

        public static readonly VTAction InvisibleAction = new VTAction(VTActions.Invisible);
        public static readonly VTAction InvisibleUnsetAction = new VTAction(VTActions.InvisibleUnset);

        public static readonly VTAction BlinkAction = new VTAction(VTActions.Blink);
        public static readonly VTAction BlinkUnsetAction = new VTAction(VTActions.BlinkUnset);

        public static readonly VTAction BoldAction = new VTAction(VTActions.Bold);
        public static readonly VTAction BoldUnsetAction = new VTAction(VTActions.BoldUnset);

        public static readonly VTAction ItalicsAction = new VTAction(VTActions.Italics);
        public static readonly VTAction ItalicsUnsetAction = new VTAction(VTActions.ItalicsUnset);

        public static readonly VTAction UnderlineAction = new VTAction(VTActions.Underline);
        public static readonly VTAction UnderlineUnsetAction = new VTAction(VTActions.UnderlineUnset);

        public static readonly VTAction DoublyUnderlinedAction = new VTAction(VTActions.DoublyUnderlined);
        public static readonly VTAction DoublyUnderlinedUnsetAction = new VTAction(VTActions.DoublyUnderlinedUnset);

        public static readonly VTAction OverlinedAction = new VTAction(VTActions.Overlined);
        public static readonly VTAction OverlinedUnsetAction = new VTAction(VTActions.OverlinedUnset);

        public static readonly VTAction ForegroundDarkBlackAction = new VTAction(VTActions.Foreground, TextColor.DARK_BLACK);
        public static readonly VTAction ForegroundDarkBlueAction = new VTAction(VTActions.Foreground, TextColor.DARK_BLUE);
        public static readonly VTAction ForegroundDarkCyanAction = new VTAction(VTActions.Foreground, TextColor.DARK_CYAN);
        public static readonly VTAction ForegroundDarkGreenAction = new VTAction(VTActions.Foreground, TextColor.DARK_GREEN);
        public static readonly VTAction ForegroundDarkMagentaAction = new VTAction(VTActions.Foreground, TextColor.DARK_MAGENTA);
        public static readonly VTAction ForegroundDarkRedAction = new VTAction(VTActions.Foreground, TextColor.DARK_RED);
        public static readonly VTAction ForegroundDarkWhiteAction = new VTAction(VTActions.Foreground, TextColor.DARK_WHITE);
        public static readonly VTAction ForegroundDarkYellowAction = new VTAction(VTActions.Foreground, TextColor.DARK_YELLOW);
        public static readonly VTAction ForegroundLightBlackAction = new VTAction(VTActions.Foreground, TextColor.BRIGHT_BLACK);
        public static readonly VTAction ForegroundLightBlueAction = new VTAction(VTActions.Foreground, TextColor.BRIGHT_BLUE);
        public static readonly VTAction ForegroundLightCyanAction = new VTAction(VTActions.Foreground, TextColor.BRIGHT_CYAN);
        public static readonly VTAction ForegroundLightGreenAction = new VTAction(VTActions.Foreground, TextColor.BRIGHT_GREEN);
        public static readonly VTAction ForegroundLightMagentaAction = new VTAction(VTActions.Foreground, TextColor.BRIGHT_MAGENTA);
        public static readonly VTAction ForegroundLightRedAction = new VTAction(VTActions.Foreground, TextColor.BRIGHT_RED);
        public static readonly VTAction ForegroundLightWhiteAction = new VTAction(VTActions.Foreground, TextColor.BRIGHT_WHITE);
        public static readonly VTAction ForegroundLightYellowAction = new VTAction(VTActions.Foreground, TextColor.BRIGHT_YELLOW);

        public static readonly VTAction BackgroundDarkBlackAction = new VTAction(VTActions.Background, TextColor.DARK_BLACK);
        public static readonly VTAction BackgroundDarkBlueAction = new VTAction(VTActions.Background, TextColor.DARK_BLUE);
        public static readonly VTAction BackgroundDarkCyanAction = new VTAction(VTActions.Background, TextColor.DARK_CYAN);
        public static readonly VTAction BackgroundDarkGreenAction = new VTAction(VTActions.Background, TextColor.DARK_GREEN);
        public static readonly VTAction BackgroundDarkMagentaAction = new VTAction(VTActions.Background, TextColor.DARK_MAGENTA);
        public static readonly VTAction BackgroundDarkRedAction = new VTAction(VTActions.Background, TextColor.DARK_RED);
        public static readonly VTAction BackgroundDarkWhiteAction = new VTAction(VTActions.Background, TextColor.DARK_WHITE);
        public static readonly VTAction BackgroundDarkYellowAction = new VTAction(VTActions.Background, TextColor.DARK_YELLOW);
        public static readonly VTAction BackgroundLightBlackAction = new VTAction(VTActions.Background, TextColor.BRIGHT_BLACK);
        public static readonly VTAction BackgroundLightBlueAction = new VTAction(VTActions.Background, TextColor.BRIGHT_BLUE);
        public static readonly VTAction BackgroundLightCyanAction = new VTAction(VTActions.Background, TextColor.BRIGHT_CYAN);
        public static readonly VTAction BackgroundLightGreenAction = new VTAction(VTActions.Background, TextColor.BRIGHT_GREEN);
        public static readonly VTAction BackgroundLightMagentaAction = new VTAction(VTActions.Background, TextColor.BRIGHT_MAGENTA);
        public static readonly VTAction BackgroundLightRedAction = new VTAction(VTActions.Background, TextColor.BRIGHT_RED);
        public static readonly VTAction BackgroundLightWhiteAction = new VTAction(VTActions.Background, TextColor.BRIGHT_WHITE);
        public static readonly VTAction BackgroundLightYellowAction = new VTAction(VTActions.Background, TextColor.BRIGHT_YELLOW);

        public static readonly VTAction ForegroundRGBAction = new VTAction(VTActions.ForegroundRGB);
        public static readonly VTAction BackgroundRGBAction = new VTAction(VTActions.BackgroundRGB);

        public static readonly VTAction PlayBellAction = new VTAction(VTActions.PlayBell);
        public static readonly VTAction PrintAction = new VTAction(VTActions.Print);

        public VTActions Type { get; set; }

        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }

        /// <summary>
        /// 动作参数
        /// </summary>
        public object Data { get; set; }

        public VTAction(VTActions type)
        {
            this.Type = type;
        }

        public VTAction(VTActions type, object data)
        {
            this.Type = type;
            this.Data = data;
        }
    }

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
        BackgroundRGB
    }
}

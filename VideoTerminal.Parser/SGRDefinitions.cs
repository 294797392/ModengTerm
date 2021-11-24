using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VideoTerminal.Parser
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
}
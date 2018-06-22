using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GTerminalCore
{
    public static class SGRDefines
    {
        /// <summary>
        /// cancel the effect
        /// </summary>
        public const byte Default = 0;

        /// <summary>
        /// bold or increased intensity
        /// </summary>
        public const byte Bold = 1;

        /// <summary>
        /// faint, decreased intensity or second colour 
        /// </summary>
        public const byte Faint = 2;

        public const byte Italicized = 3;

        public const byte SinglyUnderlined = 4;

        /// <summary>
        /// slowly blinking (less then 150 per minute) 
        /// </summary>
        public const byte SlowlyBlinking = 5;

        /// <summary>
        /// rapidly blinking (150 per minute or more)
        /// </summary>
        public const byte RapidlyBlinking = 6;

        public const byte NegativeImage = 7;

        public const byte ConcealedCharacters = 8;

        /// <summary>
        /// crossed-out (characters still legible but marked as to be deleted)
        /// </summary>
        public const byte CrossOut = 9;

        public const byte PrimaryFont = 10;

        public const byte FirstAlternativeFont = 11;

        public const byte SecondAlternativeFont = 12;

        public const byte ThirdAlternativeFont = 13;

        public const byte FourthAlternativeFont = 14;

        public const byte FifthAlternativeFont = 15;

        public const byte SixthAlternativeFont = 16;

        public const byte SeventhAlternativeFont = 17;
        System.Diagnostics.Process
        public const byte EighthAlternativeFont = 18;

        public const byte NinthAlternativeFont = 19;

        public const byte Fraktur = 20;

        public const byte DoublyUnderlined = 21;

        public const byte NormalColourOrNormalIntensity = 22;

        public const byte NotItalicizedNotFraktur = 23;

        public const byte NotUnderlined = 24;

        /// <summary>
        /// steady(not blinking)
        /// </summary>
        public const byte Steady = 25;

        public const byte PositiveImage = 27;

        public const byte RevealedCharacters = 28;

        public const byte NotCrossedOut = 29;

        public const byte BlackDisplay = 30;

        public const byte RedDisplay = 31;

        public const byte GreenDisplay = 32;

        public const byte YellowDisplay = 33;

        public const byte BlueDisplay = 34;

        public const byte MagentaDisplay = 35;

        public const byte CyanDisplay = 36;

        public const byte WhiteDisplay = 37;

        public const byte DefaultDisplayColor = 39;

        public const byte BlackBackground = 40;

        public const byte RedBackground = 41;

        public const byte GreenBackground = 42;

        public const byte YellowBackground = 43;

        public const byte BlueBackground = 44;

        public const byte MagentaBackground = 45;

        public const byte CyanBackground = 46;

        public const byte WhiteBackground = 47;

        public const byte DefaultBackgroundColor = 49;

        public const byte Framed = 51;

        public const byte Encircled = 52;

        public const byte Overlined = 53;

        public const byte NotFramedNotEncircled = 54;

        public const byte NotOverlined = 55;
    }
}
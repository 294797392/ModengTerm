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
    }
}

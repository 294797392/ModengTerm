using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Terminal
{
    /// <summary>
    /// 存储字符集信息
    /// 字符集有四组，分别是G1,G2,G3,G4字符集
    /// </summary>
    public class VTCharsetMap
    {
        public static readonly VTCharsetMap Ascii = new VTCharsetMap()
        {
            TranslationMap = new Dictionary<char, char>()
            {
            }
        };

        public static readonly VTCharsetMap DecSpecialGraphics = new VTCharsetMap()
        {
            TranslationMap = new Dictionary<char, char>()
            {
                { '\x5f', '\u0020' }, // Blank
                { '\x60', '\u2666' }, // Diamond (more commonly U+25C6, but U+2666 renders better for us)
                { '\x61', '\u2592' }, // Checkerboard
                { '\x62', '\u2409' }, // HT, SYMBOL FOR HORIZONTAL TABULATION
                { '\x63', '\u240c' }, // FF, SYMBOL FOR FORM FEED
                { '\x64', '\u240d' }, // CR, SYMBOL FOR CARRIAGE RETURN
                { '\x65', '\u240a' }, // LF, SYMBOL FOR LINE FEED
                { '\x66', '\u00b0' }, // Degree symbol
                { '\x67', '\u00b1' }, // Plus/minus
                { '\x68', '\u2424' }, // NL, SYMBOL FOR NEWLINE
                { '\x69', '\u240b' }, // VT, SYMBOL FOR VERTICAL TABULATION
                { '\x6a', '\u2518' }, // Lower-right corner
                { '\x6b', '\u2510' }, // Upper-right corner
                { '\x6c', '\u250c' }, // Upper-left corner
                { '\x6d', '\u2514' }, // Lower-left corner
                { '\x6e', '\u253c' }, // Crossing lines
                { '\x6f', '\u23ba' }, // Horizontal line - Scan 1
                { '\x70', '\u23bb' }, // Horizontal line - Scan 3
                { '\x71', '\u2500' }, // Horizontal line - Scan 5
                { '\x72', '\u23bc' }, // Horizontal line - Scan 7
                { '\x73', '\u23bd' }, // Horizontal line - Scan 9
                { '\x74', '\u251c' }, // Left "T"
                { '\x75', '\u2524' }, // Right "T"
                { '\x76', '\u2534' }, // Bottom "T"
                { '\x77', '\u252c' }, // Top "T"
                { '\x78', '\u2502' }, // | Vertical bar
                { '\x79', '\u2264' }, // Less than or equal to
                { '\x7a', '\u2265' }, // Greater than or equal to
                { '\x7b', '\u03c0' }, // Pi
                { '\x7c', '\u2260' }, // Not equal to
                { '\x7d', '\u00a3' }, // UK pound sign
                { '\x7e', '\u00b7' }, // Centered dot
            }
        };


        /// <summary>
        /// 
        /// </summary>
        public Dictionary<char, char> TranslationMap { get; private set; }
    }
}

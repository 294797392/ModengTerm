using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AsciiControlFunctions
{
    class DefaultValues
    {
        /// <summary>
        /// String Terminator
        /// </summary>
        public static readonly byte ST = CharacterUtils.BitCombinations(02, 00);
    }
}
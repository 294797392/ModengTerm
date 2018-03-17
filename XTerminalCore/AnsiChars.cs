using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XTerminalCore
{
    public class AnsiChars
    {
        #region 非打印控制字符

        public const byte BS = 8;
        public const byte Tab = 9;
        public const byte LF = 10;
        public const byte CR = 13;

        #endregion

        #region 打印字符

        public const byte Space = 32;
        public const byte A = 65;
        public const byte B = 66;
        public const byte C = 67;
        public const byte D = 68;

        #endregion
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XTerminal.Base
{
    public static class XTermConsts
    {
        public const int MIN_PORT = 1;
        public const int MAX_PORT = 65535;

        /// <summary>
        /// 光标闪烁间隔时间
        /// </summary>
        public const int CURSOR_BLINK_INTERVAL = 500;
    }
}

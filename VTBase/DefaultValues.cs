using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VideoTerminal.Base
{
    public static class DefaultValues
    {
        public static Encoding DefaultEncoding = Encoding.UTF8;
        public const int TerminalColumns = 0;
        public const int TerminalRows = 0;
        public const string TerminalName = "xterm-256color";

        /// <summary>
        /// 每次读取的数据缓冲区大小
        /// </summary>
        public const int ReadBufferSize = 256;
    }
}
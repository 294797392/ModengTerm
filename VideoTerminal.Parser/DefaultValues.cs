using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VideoTerminal.Parser
{
    public static class DefaultValues
    {
        public static Encoding DefaultEncoding = Encoding.UTF8;
        public const uint TerminalColumns = 80;
        public const uint TerminalRows = 24;
        public const string TerminalName = "xterm-256color";

        /// <summary>
        /// 每次读取的数据缓冲区大小
        /// </summary>
        public const int ReadBufferSize = 256;
    }
}
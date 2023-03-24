using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using XTerminal.Parser;

namespace XTerminal.Document
{
    internal static class DefaultValues
    {
        public static Encoding DefaultEncoding = Encoding.UTF8;
        public const int TerminalColumns = 80;
        public const int TerminalRows = 24;
        public const string TerminalName = "xterm-256color";

        /// <summary>
        /// 每次读取的数据缓冲区大小
        /// </summary>
        public const int ReadBufferSize = 256;

        public static readonly string KeymapFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "keymap.json");

        public const double FontSize = 12;

        public static readonly VTColors Foreground = VTColors.DarkBlack;
    }
}
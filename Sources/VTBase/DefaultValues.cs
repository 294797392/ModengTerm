using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace XTerminalBase
{
    public static class DefaultValues
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

        /// <summary>
        /// 默认的每一行的间距
        /// </summary>
        public static Thickness LineMargin = new Thickness(0, 1, 0, 1);

        public static double CaretWidth = SystemParameters.CaretWidth;
        public static double CaretHeight = 12;
        public static Brush CaretBrush = Brushes.Red;

        public static FontFamily FontFamily = new FontFamily("宋体");
        public static FontWeight FontWeight = FontWeights.Normal;
        public static FontStyle FontStyle = FontStyles.Normal;
        public static FontStretch FontStretch = FontStretches.Normal;
        public static double FontSize = 12;
    }
}
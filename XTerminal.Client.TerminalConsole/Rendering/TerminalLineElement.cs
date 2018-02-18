using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.TextFormatting;

namespace XTerminal.Client.TerminalConsole.Rendering
{
    /// <summary>
    /// 表示一行里的一个元素
    /// 元素可以是一个普通文本，也可以是一个可以打开的连接
    /// </summary>
    public abstract class TerminalLineElement
    {
        internal TextRunProperties properties;

        /// <summary>
        /// 元素的起始列索引
        /// </summary>
        public int ColumnIndex { get; set; }

        /// <summary>
        /// 元素所在的行索引
        /// </summary>
        public int RowIndex { get; set; }

        /// <summary>
        /// TextRun表示一个字符串
        /// </summary>
        /// <returns></returns>
        public abstract TextRun CreateTextRun();

        public abstract void InputChar(char c);
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoTerminal.Options
{
    public enum TerminalTypes
    {
        VT100,
        VT220,
        XTerm,
        XTerm256Color
    }

    /// <summary>
    /// 保存终端的配置信息
    /// </summary>
    public class TerminalOptions
    {
        /// <summary>
        /// 终端类型
        /// </summary>
        public TerminalTypes Type { get; set; }

        /// <summary>
        /// 终端的列数
        /// </summary>
        public int Columns { get; set; }

        /// <summary>
        /// 终端的行数
        /// </summary>
        public int Rows { get; set; }

        /// <summary>
        /// DECAWM模式
        /// </summary>
        public bool DECPrivateAutoWrapMode { get; set; }
    }
}

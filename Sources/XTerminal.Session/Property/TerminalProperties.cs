using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XTerminal.Session.Property
{
    public enum TerminalTypeEnum
    {
        VT100,
        VT220,
        XTerm,
        XTerm256Color
    }

    /// <summary>
    /// 保存终端的配置信息
    /// </summary>
    public class TerminalProperties
    {
        /// <summary>
        /// 终端类型
        /// </summary>
        public TerminalTypeEnum Type { get; set; }

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

        public string GetTerminalName()
        {
            switch (this.Type)
            {
                case TerminalTypeEnum.VT100: return "vt100";
                case TerminalTypeEnum.VT220: return "vt220";
                case TerminalTypeEnum.XTerm: return "xterm";
                case TerminalTypeEnum.XTerm256Color: return "xterm-256color";
                default:
                    throw new NotImplementedException();
            }
        }
    }
}

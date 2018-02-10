using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XTerminal.Terminal
{
    /// <summary>
    /// 解析终端返回的控制转义指令
    /// </summary>
    public interface IEscapeSequencesParser
    {
        List<IEscapeSequencesCommand> Parse(byte[] data);
    }

    public enum ParserStatusEnum
    {
        /// <summary>
        /// 普通模式
        /// </summary>
        Normal,

        /// <summary>
        /// 当前处于控制字符中（\x1b, esc的ascii码）
        /// </summary>
        Csi,
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XTerminal.Terminals
{
    public enum ControlCharacters
    {
        /// <summary>
        /// 000
        /// Ignored on input (not stored in input buffer; see full duplex protocol).
        /// 忽略, 不做处理
        /// </summary>
        NUL,

        /// <summary>
        /// 005
        /// Transmit answerback message.
        /// 传输应答信号
        /// </summary>
        ENQ,

        /// <summary>
        /// 007
        /// Sound bell tone from keyboard.
        /// 发声
        /// </summary>
        BEL,

        /// <summary>
        /// 010
        /// Move the cursor to the left one character position, unless it is at the left margin, in which case no action occurs.
        /// 把光标向左移动一个字符，如果光标在最左边，那就原地不动
        /// </summary>
        BS,

        /// <summary>
        /// 011
        /// Move the cursor to the next tab stop, or to the right margin if no further tab stops are present on the line.
        /// 把光标向右移动到下一个制表符的停止的地方，如果没有制表符的位置了，那就移动到最右边
        /// </summary>
        HT,

        /// <summary>
        /// 012
        /// This code causes a line feed or a new line operation. (See new line mode).
        /// </summary>
        LF,

        /// <summary>
        /// 013
        /// Interpreted as LF.
        /// 同LF
        /// </summary>
        VT,

        /// <summary>
        /// 014
        /// Interpreted as LF.
        /// 同LF
        /// </summary>
        FF,

        /// <summary>
        /// 015
        /// Move cursor to the left margin on the current line
        /// 把光标移动到当前行的最左边
        /// </summary>
        CR,

        /// <summary>
        /// 016
        /// Invoke G1 character set, as designated by SCS control sequence
        /// 调用G1字符集，由SCS控制序列指定。
        /// </summary>
        SO,

        /// <summary>
        /// 017
        /// Select G0 character set, as selected by ESC ( sequence.
        /// </summary>
        SI,

        /// <summary>
        /// 021
        /// Causes terminal to resume transmission
        /// </summary>
        XON,

        /// <summary>
        /// 023
        /// Causes terminal to stop transmitted all codes except XOFF and XON.
        /// </summary>
        XOFF,

        /// <summary>
        /// 030
        /// If sent during a control sequence, the sequence is immediately terminated and not executed. It also causes the error character to be displayed.
        /// </summary>
        CAN,

        /// <summary>
        /// 032
        /// Interpreted as CAN.
        /// </summary>
        SUB,

        /// <summary>
        /// 033
        /// Invokes a control sequence
        /// </summary>
        ESC,

        /// <summary>
        /// 177
        /// Ignored on input (not stored in input buffer).
        /// </summary>
        DEL
    }
}
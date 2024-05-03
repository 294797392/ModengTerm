using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModengTerm.Terminal.Parsing
{
    /// <summary>
    /// 定义解析器状态
    /// 每种状态都对应一个StateTable
    /// https://www.vt100.net/emu/dec_ansi_parser
    /// </summary>
    public enum VTStates
    {
        /// <summary>
        /// This is the initial state of the parser, and the state used to consume all characters other than components of escape and control sequences.
        /// GL characters (20 to 7F) are printed. I have included 20 (SP) and 7F (DEL) in this area, although both codes have special behaviour. If a 94-character set is mapped into GL, 20 will cause a space to be displayed, and 7F will be ignored. When a 96-character set is mapped into GL, both 20 and 7F may cause a character to be displayed. Later models of the VT220 included the DEC Multinational Character Set (MCS), which has 94 characters in its supplemental set (i.e. the characters supplied in addition to ASCII), so terminals only claiming VT220 compatibility can always ignore 7F. The VT320 introduced ISO Latin-1, which has 96 characters in its supplemental set, so emulators with a VT320 compatibility mode need to treat 7F as a printable character.
        /// 这是解析器的初始状态。除了转移字符和控制字符外，其他字符都是这个状态。
        /// </summary>
        Ground,

        /// <summary>
        /// This state is entered whenever the C0 control ESC is received. This will immediately cancel any escape sequence, control sequence or control string in progress. If an escape sequence or control sequence was in progress, “cancel” means that the sequence will have no effect, because the final character that determines the control function (in conjunction with any intermediates) will not have been received. However, the ESC that cancels a control string may occur after the control function has been determined and the following string has had some effect on terminal state. For example, some soft characters may already have been defined. Cancelling a control string does not undo these effects.
        /// A control string that started with DCS, OSC, PM or APC is usually terminated by the C1 control ST (String Terminator). In a 7-bit environment, ST will be represented by ESC \ (1B 5C). However, receiving the ESC character will “cancel” the control string, so the ST control function that is invoked by the arrival of the following “\” is essentially a “no-op” function. Does this point seem like pure trivia? Maybe, but I worried for ages about whether the control string recogniser needed a one character lookahead in order to know whether ESC \ was going to terminate it. The actual solution became clear when I was using ReGIS on a VT330: sending ESC immediately caused the graphics output cursor to disappear from the screen, so I knew that the control string had already finished before the “\” arrived. Many of the clues that enabled me to derive this state diagram have been as subtle as that.
        /// 收到ESC字符时进入这个状态。这个状态会立即取消所有的控制序列
        /// </summary>
        Escape,

        /// <summary>
        /// This state is entered when an intermediate character arrives in an escape sequence. Escape sequences have no parameters, so the control function to be invoked is determined by the intermediate and final characters. In this parser there is just one escape intermediate, and the parser uses the collect action to remember intermediate characters as they arrive, for processing by the esc_dispatch action when the final character arrives. An alternate approach (and the one adopted by xterm) is to have multiple copies of this state and choose the next appropriate one as each intermediate character arrives. I think that this alternate approach is merely an optimisation; the approach presented here doesn’t require any more states if the repertoire of supported control functions increases.
        /// This state is only split from the escape state because certain escape sequences are the 7-bit representations of C1 controls that change the state of the parser. Without these “compatibility sequences”, there could just be one escape state to collect intermediates and dispatch the sequence when a final character was received.
        /// 收到转义序列时进入这个状态。此时会触发collect动作，通知VTParser收集Intermediate Character（中间字符，包含参数，结束字符等），当收到finla byte的时候，触发esc dispatch执行控制命令
        /// </summary>
        EscapeIntermediate,

        /// <summary>
        /// This state is entered when the control function CSI is recognised, in 7-bit or 8-bit form. This state will only deal with the first character of a control sequence, because the characters 3C-3F can only appear as the first character of a control sequence, if they appear at all. Strictly speaking, X3.64 says that the entire string is “subject to private or experimental interpretation” if the first character is one of 3C-3F, which allows sequences like CSI ?::<? F, but Digital’s terminals only ever used one private-marker character at a time. As far as I am aware, only characters 3D (=), 3E (>) and 3F (?) were used by Digital.
        /// C0 controls are executed immediately during the recognition of a control sequence. C1 controls will cancel the sequence and then be executed. I imagine this treatment of C1 controls is prompted by the consideration that the 7-bit (ESC Fe) and 8-bit representations of C1 controls should act in the same way. When the first character of the 7-bit representation, ESC, is received, it will cancel the control sequence, so the 8-bit representation should do so as well.
        /// 收到CSI控制指令时进入这个状态
        /// </summary>
        CSIEntry,

        /// <summary>
        /// This state is entered when a parameter character is recognised in a control sequence. It then recognises other parameter characters until an intermediate or final character appears. Further occurrences of the private-marker characters 3C-3F or the character 3A, which has no standardised meaning, will cause transition to the csi ignore state.
        /// </summary>
        CSIParam,

        /// <summary>
        /// This state is entered when an intermediate character is recognised in a control sequence. It then recognises other intermediate characters until a final character appears. If any more parameter characters appear, this is an error condition which will cause a transition to the csi ignore state.
        /// Neither X3.64 nor Digital defined any control sequences with more than one intermediate character, although X3.64 doesn’t place any limit on the possible number.
        /// </summary>
        CSIIntermediate,

        /// <summary>
        /// This state is used to consume remaining characters of a control sequence that is still being recognised, but has already been disregarded as malformed. This state will only exit when a final character is recognised, at which point it transitions to ground state without dispatching the control function. This state may be entered because:
        /// a private-marker character 3C-3F is recognised in any place other than the first character of the control sequence,
        /// the character 3A appears anywhere, or
        /// a parameter character 30-3F occurs after an intermediate character has been recognised.
        /// C0 controls will still be executed while a control sequence is being ignored.
        /// </summary>
        CSIIgnore,

        /// <summary>
        /// 收集OSC参数
        /// </summary>
        OSCParam,

        OSCString,

        OSCTermination,

        /// <summary>
        /// This state is entered when the control function DCS is recognised, in 7-bit or 8-bit form. X3.64 doesn’t define any structure for device control strings, but Digital made them appear like control sequences followed by a data string, with a form and length dependent on the control function. This state is only used to recognise the first character of the control string, mirroring the csi entry state.
        /// C0 controls other than CAN, SUB and ESC are not executed while recognising the first part of a device control string.
        /// </summary>
        DCSEntry,

        /// <summary>
        /// This state is entered when a parameter character is recognised in a device control string. It then recognises other parameter characters until an intermediate or final character appears. Occurrences of the private-marker characters 3C-3F or the undefined character 3A will cause a transition to the dcs ignore state.
        /// </summary>
        DCSParam,

        /// <summary>
        /// This state is entered when an intermediate character is recognised in a device control string. It then recognises other intermediate characters until a final character appears. If any more parameter characters appear, this is an error condition which will cause a transition to the dcs ignore state.
        /// </summary>
        DCSIntermediate,

        /// <summary>
        /// This state is a shortcut for writing state machines for all possible device control strings into the main parser. When a final character has been recognised in a device control string, this state will establish a channel to a handler for the appropriate control function, and then pass all subsequent characters through to this alternate handler, until the data string is terminated (usually by recognising the ST control function).
        /// This state has an exit action so that the control function handler can be informed when the data string has come to an end. This is so that the last soft character in a DECDLD string can be completed when there is no other means of knowing that its definition has ended, for example.
        /// </summary>
        DCSPassthrough,

        /// <summary>
        /// This state is used to consume remaining characters of a device control string that is still being recognised, but has already been disregarded as malformed. This state will only exit when the control function ST is recognised, at which point it transitions to ground state. This state may be entered because:
        /// a private-marker character 3C-3F is recognised in any place other than the first character of the control string,
        /// the character 3A appears anywhere, or
        /// a parameter character 30-3F occurs after an intermediate character has been recognised.
        /// These conditions are only errors in the first part of the control string, until a final character has been recognised. The data string that follows is not checked by this parser.
        /// </summary>
        DCSIgnore,

        /// <summary>
        /// The VT500 doesn’t define any function for these control strings, so this state ignores all received characters until the control function ST is recognised.
        /// </summary>
        SOS_PM_APC_String,

        /// <summary>
        /// This isn’t a real state. It is used on the state diagram to show transitions that can occur from any state to some other state. These invariant transitions are:
        /// On the VT220, VT420 and VT500, the C0 controls CAN and SUB cancel any escape sequence, control sequence or control string in progress and return to ground state. SUB will also display the error character, a reversed question mark, “[mirrored '?']”. The programmer’s information for the VT320 says that CAN and SUB “no longer” cancel these sequences, so there must have been a rethink when the VT420 was being designed.
        /// All C1 controls cancel any escape sequence, control sequence or control string in progress and are executed. Control functions special to this parser, i.e. DCS, SOS, CSI, OSC, PM and APC, cause a transition to their appropriate states. All other C1 control functions (even those with no defined meaning), cause a transition to ground state.
        /// On terminals earlier than the VT500, there would have been one other invariant action: the C0 control NUL was ignored on input to the terminal and would not take part in any processing. Its only purpose was as a time-fill character. However, the VT500 defines a control function DECNULM (Null Mode), which allows NUL to be passed to an attached printer. So in this parser, NUL is treated the same as other C0 controls.
        /// </summary>
        Anywhere,

        Vt52Param
    }
}
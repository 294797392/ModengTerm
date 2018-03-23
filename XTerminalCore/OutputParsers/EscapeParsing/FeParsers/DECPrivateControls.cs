using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XTerminalCore
{
    /// <summary>
    /// DEC公司定义的私有命令
    /// 这些命令在标准里是找不到的
    /// 这些私有命令是跟在ESC字符后面的
    /// 参考：
    ///     http://invisible-island.net/xterm/ctlseqs/ctlseqs.html
    /// </summary>
    public class DECPrivateControls
    {
        /// <summary>
        /// Back Index, VT420 and up.
        /// </summary>
        public static readonly byte DECBI = (byte)'6';

        /// <summary>
        /// SaveCursor
        /// </summary>
        public static readonly byte DECSC = (byte)'7';

        /// <summary>
        /// Restore Cursor
        /// </summary>
        public static readonly byte DECRC = (byte)'8';

        /// <summary>
        /// Forward Index,VT420 and up.
        /// </summary>
        public static readonly byte DECFI = (byte)'9';

        /// <summary>
        /// Application Keypad
        /// </summary>
        public static readonly byte DECKPAM = (byte)'=';

        /// <summary>
        /// Normal Keypad
        /// </summary>
        public static readonly byte DECKPNM = (byte)'>';

        /// <summary>
        /// Cursor to lower left corner of screen.  This is enabled by the hpLowerleftBugCompat resource.
        /// </summary>
        public static readonly byte CTLF = (byte)'F';

        /// <summary>
        /// Full Reset
        /// </summary>
        public static readonly byte RIS = (byte)'c';

        /// <summary>
        /// Memory Lock (per HP terminals).  Locks memory above the cursor.
        /// </summary>
        public static readonly byte ML = (byte)'l';

        /// <summary>
        /// Memory Unlock (per HP terminals).
        /// </summary>
        public static readonly byte MU = (byte)'m';

        /// <summary>
        /// Invoke the G2 Character Set as GL
        /// </summary>
        public static readonly byte LS2 = (byte)'n';

        /// <summary>
        /// Invoke the G3 Character Set as GL
        /// </summary>
        public static readonly byte LS3 = (byte)'o';

        /// <summary>
        /// Invoke the G3 Character Set as GR
        /// </summary>
        public static readonly byte LS3R = (byte)'|';

        /// <summary>
        /// Invoke the G2 Character Set as GR
        /// </summary>
        public static readonly byte LS2R = (byte)'}';

        /// <summary>
        /// Invoke the G1 Character Set as GR
        /// </summary>
        public static readonly byte LS1R = (byte)'~';
    }
}
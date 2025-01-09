using ModengTerm.Base.Enumerations.Terminal;
using ModengTerm.Base.Enumerations;
using ModengTerm.Document.Enumerations;
using ModengTerm.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using XTerminal.Base.Enumerations;

namespace ModengTerm.Base
{
    /// <summary>
    /// 存储选项默认值
    /// </summary>
    public static class OptionDefaultValues
    {
        /// <summary>
        /// 默认Adb登录超时时间，单位毫秒
        /// </summary>
        public const int ADBSH_LOGIN_TIMEOUT = 5000;
        public const string ADBSH_ADB_PATH = "adb.exe";
        public const AdbLoginTypeEnum ADBSH_LOGIN_TYPE = AdbLoginTypeEnum.UserNamePassword;
        public const int SSH_TERM_ROW = 24;
        public const int SSH_TERM_COL = 80;
        public static readonly int SSH_READ_BUFFER_SIZE = 16384;
        public static readonly TerminalTypeEnum SSH_TERM_TYPE = TerminalTypeEnum.XTerm256Color;
        public static readonly SSHAuthTypeEnum SSH_AUTH_TYPE = SSHAuthTypeEnum.Password;
        public static readonly string SSH_USER_NAME = string.Empty;
        public static readonly string SSH_PASSWORD = string.Empty;
        public static readonly string SSH_PRIVATE_KEY_FILE = string.Empty;
        public static readonly string SSH_Passphrase = string.Empty;
        public static readonly int SSH_PORT = 22;
        public static readonly string SSH_ADDR = string.Empty;
        //public static readonly List<PortForward> SSH_PORT_FORWARDS = new List<PortForward>();

        //public static readonly string CMD_STARTUP_PATH = Path.Combine(Environment.SystemDirectory, "cmd.exe");
        //public static readonly string CMD_STARTUP_DIR = AppDomain.CurrentDomain.BaseDirectory;
        //public static readonly string CMD_STARTUP_ARGUMENT = string.Empty;
        //public static CmdDriverEnum CMD_DRIVER()
        //{
        //    // 如果是Win10或更高版本那么默认选择PseudoConsoleApi
        //    if (VTBaseUtils.IsWin10())
        //    {
        //        return CmdDriverEnum.Win10PseudoConsoleApi;
        //    }
        //    else
        //    {
        //        return CmdDriverEnum.winpty;
        //    }
        //}

        //public static readonly RawTcpTypeEnum RAW_TCP_TYPE = RawTcpTypeEnum.Client;
        //public static readonly string RAW_TCP_ADDRESS = string.Empty;
        //public static readonly int RAW_TCP_PORT = 0;

        //public static readonly string SERIAL_PORT_NAME = "COM1";
        //public static readonly int SERIAL_PORT_BAUD_RATE = 115200;
        //public static readonly int SERIAL_PORT_DATA_BITS = 8;
        //public static readonly StopBits SERIAL_PORT_STOP_BITS = StopBits.None;
        //public static readonly Parity SERIAL_PORT_PARITY = Parity.None;
        //public static readonly Handshake SERIAL_PORT_HANDSHAKE = Handshake.None;


        public static readonly string TERM_ADVANCE_SEND_COLOR = "0,255,0,255";
        public static readonly string TERM_ADVANCE_RECV_COLOR = "255,0,0,255";
        public static readonly bool TERM_ADVANCE_RENDER_WRITE = false;

        public static readonly bool TERM_ADVANCE_AUTO_COMPLETION_ENABLED = false;

        //public static readonly BehaviorRightClicks BEHAVIOR_RIGHT_CLICK = BehaviorRightClicks.ContextMenu;

        //public static readonly string THEME_BACKGROUND_COLOR = "";
        //public static readonly int SSH_THEME_DOCUMENT_PADDING = 5;
        public static readonly string TERM_WRITE_ENCODING = "UTF-8";
        public static readonly string TERM_READ_ENCODING = "UTF-8";

        //public static readonly int MOUSE_SCROLL_DELTA = 1;
        //public static readonly int TERM_MAX_CLIPBOARD_HISTORY = 1000;
        //public static readonly VTColorTable TEHEM_COLOR_TABLE = new VTColorTable() { };
        //public static readonly string THEME_FONT_COLOR = "";
        //public static readonly string THEME_FONT_FAMILY = "";
        //public static readonly double THEME_FONT_SIZE = 12;
        //public static readonly TerminalSizeModeEnum SSH_TERM_SIZE_MODE = TerminalSizeModeEnum.AutoFit;
        //public static readonly VTCursorStyles THEME_CURSOR_STYLE = VTCursorStyles.Line;
        //public static readonly string THEME_CURSOR_COLOR = "";
        //public static readonly VTCursorSpeeds THEME_CURSOR_SPEED = VTCursorSpeeds.NormalSpeed;
        //public static readonly int TERM_MAX_ROLLBACK = 99999;
        //public static readonly string THEME_SELECTION_COLOR = "";


        public static readonly string THEME_BACKGROUND_IMAGE_DATA = string.Empty;
        public static readonly double THEME_BACKGROUND_IMAGE_OPACITY = 1;
    }
}

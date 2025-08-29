using ModengTerm.Base.DataModels;
using ModengTerm.Base.DataModels.Terminal;
using ModengTerm.Base.Definitions;
using ModengTerm.Base.Enumerations;
using ModengTerm.Base.Enumerations.Terminal;
using ModengTerm.Document;
using ModengTerm.Document.Enumerations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ModengTerm.Base
{
    /// <summary>
    /// XTerminal使用的默认值
    /// </summary>
    public static class VTBaseConsts
    {
        /// <summary>
        /// 显示为“根节点”
        /// 给用户选择
        /// </summary>
        public static readonly SessionGroup RootGroup = new SessionGroup()
        {
            ID = string.Empty,
            Name = "根节点"
        };

        /// <summary>
        /// 默认要打开的会话
        /// </summary>
        public static readonly XTermSession DefaultSession = new XTermSession()
        {
            ID = "0",
            Name = "控制台会话",
            Type = (int)SessionTypeEnum.LocalConsole,
            Options = new Dictionary<string, Dictionary<string, object>>()
            {
                {
                    "3738D66F-9B8A-BB45-C823-22DAF39AAAF6",
                    new Dictionary<string, object>()
                    {
                        { PredefinedOptions.CONSOLE_STARTUP_DIR, AppDomain.CurrentDomain.BaseDirectory },
                        { PredefinedOptions.CONSOLE_STARTUP_ARGUMENT, string.Empty },
                        { PredefinedOptions.CONSOLE_ENGINE, "auto" },
                        { PredefinedOptions.CONSOLE_STARTUP_PATH, Path.Combine(Environment.SystemDirectory, "cmd.exe") },
                        { PredefinedOptions.TERM_ADVANCE_RENDER_WRITE, false },
                        { PredefinedOptions.TERM_ADVANCE_AUTO_WRAP_MODE, false },
                        { PredefinedOptions.THEME_BACKGROUND_IMAGE_DATA, string.Empty },
                        { PredefinedOptions.TERM_ADVANCE_CLICK_TO_CURSOR, false },
                        { PredefinedOptions.THEME_ID, "7A2A6563-8C16-4E6A-9C9F-AA610E4C6827" },
                        { PredefinedOptions.THEME_FONT_FAMILY, "新宋体" },
                        { PredefinedOptions.THEME_FONT_SIZE, 16 },
                        { PredefinedOptions.THEME_BACK_COLOR, "36,36,36,255" },
                        { PredefinedOptions.THEME_FONT_COLOR, "242,242,242,255" },
                        { PredefinedOptions.THEME_CURSOR_STYLE, "line" },
                        { PredefinedOptions.THEME_CURSOR_SPEED, "normal" },
                        { PredefinedOptions.THEME_CURSOR_COLOR, "255,255,255,255" },
                        { PredefinedOptions.TEHEM_COLOR_TABLE, "{ \"rgbKeys\":[\"54,52,46,255\",\"165,100,52,255\",\"0,128,0,255\",\"153,150,6,255\",\"70,70,255,255\",\"123,81,117,255\",\"0,162,196,255\",\"207,216,211,255\",\"83,87,85,255\",\"207,158,114,255\",\"28,196,112,255\",\"226,226,52,255\",\"111,111,244,255\",\"169,126,173,255\",\"80,235,252,255\",\"236,238,238,255\"] }" },
                        { PredefinedOptions.THEME_FIND_HIGHLIGHT_BACKCOLOR, "236,238,238,100" },
                        { PredefinedOptions.THEME_FIND_HIGHLIGHT_FORECOLOR, "54,52,46,255" },
                        { PredefinedOptions.THEME_SELECTION_COLOR, "255,255,255,100" },
                        { PredefinedOptions.SSH_TERM_ROW, 24 },
                        { PredefinedOptions.SSH_TERM_COL, 80 },
                        { PredefinedOptions.SSH_TERM_TYPE, "xterm-256color" },
                        { PredefinedOptions.SSH_TERM_SIZE_MODE, "autoFit" },
                        { PredefinedOptions.TERM_READ_ENCODING, "UTF-8" },
                        { PredefinedOptions.TERM_WRITE_ENCODING, "UTF-8" },
                        { PredefinedOptions.SSH_READ_BUFFER_SIZE, 8192 },
                        { PredefinedOptions.TERM_MAX_ROLLBACK, 99999 },
                        { PredefinedOptions.TERM_MAX_CLIPBOARD_HISTORY, 50 },
                        { PredefinedOptions.SSH_THEME_DOCUMENT_PADDING, 5 },
                        { PredefinedOptions.MOUSE_SCROLL_DELTA, 1 },
                        { PredefinedOptions.TERM_DISABLE_BELL, false },
                        { PredefinedOptions.BEHAVIOR_RIGHT_CLICK, "contextMenu" },
                        { PredefinedOptions.TERM_ADVANCE_RENDER_MODE, "default" },
                        { PredefinedOptions.TERM_ADVANCE_AUTO_COMPLETION_ENABLED, false },
                    }
                }
            }
        };

        /// <summary>
        /// 会话类型元数据信息
        /// </summary>
        public static readonly List<SessionMetadata> SessionMetadatas = new List<SessionMetadata>()
        {
            new SessionMetadata()
            {
                ID = "0",
                Name = "本地命令行",
                Type = SessionTypeEnum.LocalConsole
            },

            new SessionMetadata()
            {
                ID = "1",
                Name = "Ssh",
                Type = SessionTypeEnum.Ssh
            },

            new SessionMetadata()
            {
                ID = "2",
                Name = "串口",
                Type = SessionTypeEnum.SerialPort
            },

            new SessionMetadata()
            {
                ID = "3",
                Name = "Tcp",
                Type = SessionTypeEnum.Tcp
            }
        };

        /// <summary>
        /// 变量名字 -> 变量值
        /// </summary>
        public static readonly Dictionary<string, string> VariableName2Value = new Dictionary<string, string>() 
        {
            { "%SYS_DIR%", Environment.GetFolderPath(Environment.SpecialFolder.System) },
            { "%CLI_DIR%", AppDomain.CurrentDomain.BaseDirectory }
        };

        /// <summary>
        /// 最多保存10个最近打开的会话
        /// </summary>
        public const int MaxRecentSessions = 10;

        public const int MIN_PORT = 1;
        public const int MAX_PORT = 65535;

        public const int TerminalColumns = 80;
        public const int TerminalRows = 24;
        public const TerminalTypeEnum DefaultTerminalType = TerminalTypeEnum.XTerm256Color;

        public const int DefaultTerminalScrollback = 99999;
        public const int DefaultMaxClipboardHistory = 50;

        /// <summary>
        /// 默认的串口波特率列表
        /// </summary>
        public static readonly List<string> SerialPortBaudRates = new List<string>()
        {
            "4800",
            "9600",
            "14410",
            "19200",
            "38400",
            "57600",
            "115200",
            "921600"
        };

        public static readonly List<int> SerialPortDataBits = new List<int>()
        {
            5, 6, 7, 8
        };

        /// <summary>
        /// 默认的SSH服务端口号
        /// </summary>
        public const int DefaultSSHPort = 22;

        /// <summary>
        /// 滚轮滚动一下翻两行
        /// </summary>
        public const int DefaultScrollDelta = 1;

        #region SFTP

        public const string SFTPServerInitialDirectory = "/";
        public static readonly string SFTPClientInitialDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory);

        /// <summary>
        /// 最多记录100个历史目录
        /// </summary>
        public const int MaxHistoryDirectory = 100;

        #endregion

        /// <summary>
        /// TreeViewItem的缩进
        /// </summary>
        public const int TreeViewItemIndent = 15;
    }
}

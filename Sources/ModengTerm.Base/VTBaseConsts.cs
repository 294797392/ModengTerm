using ModengTerm.Base.DataModels;
using ModengTerm.Base.DataModels.Terminal;
using ModengTerm.Base.Definitions;
using ModengTerm.Base.Enumerations;
using ModengTerm.Base.Enumerations.Terminal;
using ModengTerm.Base.Metadatas;
using Newtonsoft.Json;
using System.IO;

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

        private static readonly VTColorTable DefaultSessionColorTable = new VTColorTable()
        {
            RgbKeys = new List<string>()
            {
                "54,52,46,255", "165,100,52,255", "0,128,0,255", "153,150,6,255", "70,70,255,255", "123,81,117,255", "0,162,196,255", "207,216,211,255",
                "83,87,85,255", "207,158,114,255", "28,196,112,255", "226,226,52,255", "111,111,244,255", "169,126,173,255", "80,235,252,255", "236,238,238,255"
            }
        };

        /// <summary>
        /// 默认要打开的会话
        /// </summary>
        public static readonly XTermSession DefaultSession = new XTermSession()
        {
            ID = "0",
            Name = "控制台会话",
            Type = (int)SessionTypeEnum.Console,
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
                        { PredefinedOptions.TERM_ADV_RENDER_SEND, false },
                        { PredefinedOptions.TERM_ADV_AUTO_WRAP_MODE, false },
                        { PredefinedOptions.TERM_ADV_CLICK_TO_CURSOR, false },

                        { PredefinedOptions.THEME_ID, "7A2A6563-8C16-4E6A-9C9F-AA610E4C6827" },
                        { PredefinedOptions.THEME_FONT_FAMILY, VTBaseUtils.GetDefaultFontFamilyName() },
                        { PredefinedOptions.THEME_FONT_SIZE, 16 },
                        { PredefinedOptions.THEME_BACK_COLOR, "36,36,36,255" },
                        { PredefinedOptions.THEME_FONT_COLOR, "242,242,242,255" },
                        { PredefinedOptions.THEME_CURSOR_COLOR, "255,255,255,255" },
                        { PredefinedOptions.TEHEM_COLOR_TABLE, JsonConvert.SerializeObject(DefaultSessionColorTable) },
                        { PredefinedOptions.THEME_BACK_IMAGE_DATA, string.Empty },
                        { PredefinedOptions.THEME_BACK_IMAGE_OPACITY, 1 },
                        { PredefinedOptions.THEME_SELECTION_BACK_COLOR, "236,238,238,100" },

                        { PredefinedOptions.CURSOR_STYLE, "line" },
                        { PredefinedOptions.CURSOR_SPEED, "normal" },
                        { PredefinedOptions.TERM_ROW, 24 },
                        { PredefinedOptions.TERM_COL, 80 },
                        { PredefinedOptions.TERM_TYPE, "xterm-256color" },
                        { PredefinedOptions.TERM_SIZE_MODE, "autoFit" },
                        { PredefinedOptions.TERM_ENCODING, "UTF-8" },
                        { PredefinedOptions.TERM_READ_BUFFER_SIZE, 8192 },
                        { PredefinedOptions.TERM_MAX_ROLLBACK, 99999 },
                        { PredefinedOptions.THEME_PADDING, 5 },
                        { PredefinedOptions.CURSOR_SCROLL_DELTA, 1 },
                        { PredefinedOptions.TERM_DISABLE_BELL, false },
                        { PredefinedOptions.TERM_RIGHT_CLICK_ACTION, "contextMenu" },
                        { PredefinedOptions.TERM_ADV_RENDER_MODE, "default" },
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
                Type = SessionTypeEnum.Console
            },

            new SessionMetadata()
            {
                ID = "1",
                Name = "SSH",
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
            },

            new SessionMetadata()
            {
                ID = "4",
                Name = "Sftp",
                Type = SessionTypeEnum.Sftp
            }
        };

        /// <summary>
        /// 变量名字 -> 变量值
        /// </summary>
        public static readonly Dictionary<string, string> VariableName2Value = new Dictionary<string, string>()
        {
            { "%SYS_DIR%", Environment.GetFolderPath(Environment.SpecialFolder.System) },
            { "%CLI_DIR%", AppDomain.CurrentDomain.BaseDirectory },
            { "%SYS_FONT%", VTBaseUtils.GetDefaultFontFamilyName() }
        };

        public static readonly List<ThemePackage> TerminalThemes = new List<ThemePackage>()
        {
            new ThemePackage()
            {
                ID = "7A2A6563-8C16-4E6A-9C9F-AA610E4C6827",
                Name = "Xshell - NewBlack",
                ColorTable = new VTColorTable()
                {
                    RgbKeys = new List<string>()
                    {
                        "54,52,46,255", "165,100,52,255", "0,128,0,255", "153,150,6,255", "70,70,255,255", "123,81,117,255", "0,162,196,255", "207,216,211,255",
                        "83,87,85,255", "207,158,114,255", "28,196,112,255", "226,226,52,255", "111,111,244,255", "169,126,173,255", "80,235,252,255", "236,238,238,255"
                    }
                },
                BackColor = "36,36,36,255",
                BackImageOpacity = 0.7,
                BackImageUri = string.Empty,
                CursorColor = "255,255,255,255",
                FontColor = "242,242,242,255",
                FontSize = 14,
                SelectionBackColor = "255,255,255,100"
            },

            new ThemePackage()
            {
                ID = "AC439E26-0768-4131-8F86-1D79778846DB",
                Name = "Xshell - XTerm",
                ColorTable = new VTColorTable()
                {
                    RgbKeys = new List<string>()
                    {
                        "0,0,0,255", "187,0,0,255", "0,100,0,255", "200,175,0,255", "30,144,245,255", "187,0,187,255", "0,205,205,255", "235,235,235,255",
                        "85,85,85,255", "255,85,85,255", "85,255,85,255", "255,245,85,255", "80,190,255,255", "255,85,255,255", "85,255,255,255", "255,255,255,255"
                    }
                },
                BackColor = "0,0,0,255",
                BackImageOpacity = 0.7,
                BackImageUri = string.Empty,
                CursorColor = "255,255,255,255",
                FontColor = "255,255,255,255",
                FontSize = 14,
                SelectionBackColor = "255,255,255,100"
            }
        };

        public static readonly List<int> FontSizes = new List<int>()
        {
            12, 14, 16, 18, 20
        };

        /// <summary>
        /// 顶部根菜单
        /// </summary>
        public static readonly List<MenuMetadata> TitleMenus = new List<MenuMetadata>()
        {
            new MenuMetadata() { ID = "70DCA138-F1E2-4F98-A545-CCC2008F1E0A", Name = "会话" },
            new MenuMetadata() { ID = "05B8D545-77CD-4F4E-908B-65A5C27E842D", Name = "视图" },
            new MenuMetadata() { ID = "50ED6E4E-4252-4D92-99DD-E72AC646FACB", Name = "编辑" },
            new MenuMetadata() { ID = "BF0E0737-CE95-403E-ABD2-2768FFBE11B9", Name = "配置" },
            new MenuMetadata() { ID = "000C1900-DC7E-4710-B8B1-46CE00E35E33", Name = "工具" },
            new MenuMetadata() { ID = "A592C670-3B36-495D-89CE-364691D9DFDA", Name = "帮助" }
        };


        /// <summary>
        /// 最多保存10个最近打开的会话
        /// </summary>
        public const int MaxRecentSessions = 10;

        /// <summary>
        /// 默认的串口波特率列表
        /// </summary>
        public static readonly List<int> SerialPortBaudRates = new List<int>()
        {
            4800,
            9600,
            14410,
            19200,
            38400,
            57600,
            115200,
            921600
        };

        public static readonly List<int> SerialPortDataBits = new List<int>()
        {
            5, 6, 7, 8
        };

        /// <summary>
        /// TreeViewItem的缩进
        /// </summary>
        public const int TreeViewItemIndent = 15;
    }
}

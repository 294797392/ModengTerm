using ModengTerm.Base.DataModels;
using ModengTerm.Base.DataModels.Terminal;
using ModengTerm.Base.Definitions;
using ModengTerm.Base.Enumerations;
using ModengTerm.Base.Metadatas;
using Newtonsoft.Json;
using System.IO;

namespace ModengTerm.Base
{
    public static class FtpCommandKeys
    {
        public const string CLIENT_OPEN_ITEM = "FTP.CLIENT_OPEN_ITEM";
        public const string CLIENT_UPLOAD_ITEM = "FTP.CLIENT_UPLOAD_ITEM";
        public const string CLIENT_DELETE_ITEM = "FTP.CLIENT_DELETE_ITEM";
        public const string CLIENT_RENAME_ITEM = "FTP.CLIENT_RENAME_ITEM";
        public const string CLIENT_SHOW_ITEM_PROPERTY = "FTP.CLIENT_SHOW_ITEM_PROPERTY";
        public const string CLIENT_REFRESH_ITEMS = "FTP.CLIENT_REFRESH_ITEMS";
        public const string CLIENT_NEW_FILE = "FTP.CLIENT_NEW_FILE";
        public const string CLIENT_NEW_DIRECTORY = "FTP.CLIENT_NEW_DIRECTORY";

        public const string SERVER_OPEN_ITEM = "FTP.SERVER_OPEN_ITEM";
        public const string SERVER_DOWNLOAD_ITEM = "FTP.SERVER_DOWNLOAD_ITEM";
        public const string SERVER_REFRESH_ITEMS = "FTP.SERVER_REFRESH_ITEMS";
        public const string SERVER_DELETE_ITEM = "FTP.SERVER_DELETE_ITEM";
        public const string SERVER_RENAME_ITEM = "FTP.SERVER_RENAME_ITEM";
    }

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
            // Sftp遇到的问题：
            // InitialDirectory是软件运行目录，是通过%CLI_DIR%宏获取的
            // %CLI_DIR%宏的实现方式是AppDomain.CurrentDomain.BaseDirectory，这个目录后面会带一个反斜杠
            // 如果目录后面有反斜杠，Directory.GetParent会返回删除反斜杠后的目录，相当于返回的是同一个目录
            // 会导致显示初始目录的时候，双击“返回上级目录项”没反应
            // 解决方式就是删除最后一个反斜杠
            { "%CLI_DIR%", AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\') },
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
        /// Ftp本地文件右键菜单列表
        /// </summary>
        public static readonly List<MenuMetadata> FtpClientFileItemMenus = new List<MenuMetadata>()
        {
            new MenuMetadata("打开", FtpCommandKeys.CLIENT_OPEN_ITEM),
            new MenuMetadata("上传", FtpCommandKeys.CLIENT_UPLOAD_ITEM),
            new MenuMetadata("删除", FtpCommandKeys.CLIENT_DELETE_ITEM),
            new MenuMetadata("重命名", FtpCommandKeys.CLIENT_RENAME_ITEM),
            new MenuMetadata("属性", FtpCommandKeys.CLIENT_SHOW_ITEM_PROPERTY),
            new MenuMetadata("刷新", FtpCommandKeys.CLIENT_REFRESH_ITEMS)
        };

        public static readonly List<MenuMetadata> FtpCLientFileListMenus = new List<MenuMetadata>()
        {
            new MenuMetadata("刷新", FtpCommandKeys.CLIENT_REFRESH_ITEMS),
            //new MenuMetadata("新建文件", FtpCommandKeys.CLIENT_NEW_FILE),
            //new MenuMetadata("新建目录", FtpCommandKeys.CLIENT_NEW_DIRECTORY)
        };


        /// <summary>
        /// Ftp服务器文件右键菜单列表
        /// </summary>
        public static readonly List<MenuMetadata> FtpServerFileItemMenus = new List<MenuMetadata>()
        {
            new MenuMetadata("下载", FtpCommandKeys.SERVER_DOWNLOAD_ITEM),
            new MenuMetadata("删除", FtpCommandKeys.SERVER_DELETE_ITEM),
            new MenuMetadata("重命名", FtpCommandKeys.SERVER_RENAME_ITEM),
            new MenuMetadata("刷新", FtpCommandKeys.SERVER_REFRESH_ITEMS)
        };

        public static readonly List<MenuMetadata> FtpServerFileListMenus = new List<MenuMetadata>() 
        {
            new MenuMetadata("刷新", FtpCommandKeys.SERVER_REFRESH_ITEMS)
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

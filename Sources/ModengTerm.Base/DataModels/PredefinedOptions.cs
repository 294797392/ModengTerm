using ModengTerm.Base.Enumerations.Terminal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;

namespace ModengTerm.Base.DataModels
{
    public enum RightClickActions
    {
        /// <summary>
        /// 右键点击的时候什么都不做
        /// </summary>
        None,

        /// <summary>
        /// 右键点击的时候复制选中内容到剪切板
        /// </summary>
        FastCopyPaste,

        /// <summary>
        /// 右键点击的时候弹出上下文菜单
        /// </summary>
        ContextMenu
    }

    /// <summary>
    /// 配置类型
    /// </summary>
    public static class PredefinedOptions
    {
        #region SSH 0 - 1000

        // Terminal 0 - 99
        public const string SSH_TERM_ROW = " 0";
        public const string SSH_TERM_COL = " 1";
        public const string SSH_TERM_TYPE = " 2";
        public const string SSH_TERM_SIZE_MODE = " 3";

        /// <summary>
        /// 输出编码格式
        /// </summary>
        public const string TERM_WRITE_ENCODING = " 5";
        //SSH_WRITE_BUFFER_SIZE =" 6";
        public const string SSH_READ_BUFFER_SIZE = " 7";

        // Theme 100 - 200
        public const string THEME_FONT_FAMILY = " 100";
        public const string THEME_FONT_SIZE = " 101";
        public const string THEME_FONT_COLOR = " 102";
        /// <summary>
        /// 光标闪烁速度
        /// </summary>
        public const string THEME_CURSOR_SPEED = " 103";
        /// <summary>
        /// 光标样式
        /// </summary>
        public const string THEME_CURSOR_STYLE = " 104";
        public const string THEME_CURSOR_COLOR = " 106";
        /// <summary>
        /// 选择的主题ID
        /// </summary>
        public const string THEME_ID = " 107";
        /// <summary>
        /// SGR颜色表
        /// </summary>
        public const string TEHEM_COLOR_TABLE = " 108";

        /// <summary>
        /// 终端背景颜色
        /// </summary>
        public const string THEME_BACK_COLOR = " 112";
        //THEME_BACKGROUND_EFFECT =" 113";

        /// <summary>
        /// 查找高亮前景色
        /// </summary>
        public const string THEME_FIND_HIGHLIGHT_FORECOLOR = " 114";
        /// <summary>
        /// 查找高亮背景色
        /// </summary>
        public const string THEME_FIND_HIGHLIGHT_BACKCOLOR = " 115";

        public const string THEME_SELECTION_COLOR = " 116";

        /// <summary>
        /// 背景图片透明度
        /// </summary>
        public const string THEME_BACKGROUND_IMAGE_OPACITY = " 117";
        /// <summary>
        /// base64格式保存的图片数据
        /// </summary>
        public const string THEME_BACKGROUND_IMAGE_DATA = " 118";


        /// <summary>
        /// 文档内边距
        /// </summary>
        public const string SSH_THEME_DOCUMENT_PADDING = " 501";


        // Server 201 - 300
        public const string SSH_SERVER_ADDR = " 504";
        public const string SSH_SERVER_PORT = "SSH_SERVER_PORT";
        public const string SSH_USER_NAME = "SSH_USER_NAME";
        public const string SSH_PASSWORD = "SSH_PASSWORD";
        public const string SSH_PRIVATE_KEY_FILE = "SSH_PRIVATE_KEY_FILE";
        public const string SSH_Passphrase = "SSH_Passphrase";
        public const string SSH_AUTH_TYPE = "SSH_AUTH_TYPE";
        public const string SSH_PORT_FORWARDS = "SSH_PORT_FORWARDS";

        #endregion

        #region 终端 1001 - 1500

        /// <summary>
        /// 设置滚动条可以滚动到的最大值
        /// </summary>
        public const string TERM_MAX_ROLLBACK = " 1001";
        /// <summary>
        /// 最多可以保存的剪贴板历史记录数据
        /// </summary>
        public const string TERM_MAX_CLIPBOARD_HISTORY = " 1002";

        public const string MOUSE_SCROLL_DELTA = " 1004";

        /// <summary>
        /// 是否禁用响铃
        /// </summary>
        public const string TERM_DISABLE_BELL = " 1005";

        /// <summary>
        /// 发送数据的编码方式
        /// </summary>
        public const string TERM_READ_ENCODING = " 1006";

        #endregion

        #region 终端行为 1501 - 2000

        /// <summary>
        /// 当鼠标右键点击的时候的行为
        /// </summary>
        [EnumDataType(typeof(RightClickActions))]
        public const string BEHAVIOR_RIGHT_CLICK = " 1501";

        #endregion

        #region 命令行 2001 - 2500

        /// <summary>
        /// 命令行可执行程序路径
        /// </summary>
        public const string CONSOLE_STARTUP_PATH = "console_startup_path";

        /// <summary>
        /// 命令行参数
        /// </summary>
        public const string CONSOLE_STARTUP_ARGUMENT = "console_startup_argument";

        /// <summary>
        /// 初始目录
        /// </summary>
        public const string CONSOLE_STARTUP_DIR = "console_startup_directory";

        /// <summary>
        /// 命令行驱动程序
        /// winpty/PesudoConsoleAPI
        /// </summary>
        [EnumDataType(typeof(Win32ConsoleEngineEnum))]
        public const string CONSOLE_ENGINE = "console_engine";

        #endregion

        #region 串口 2501 - 2800

        public const string SERIAL_PORT_NAME = " 2501";
        public const string SERIAL_PORT_BAUD_RATE = " 2502";
        public const string SERIAL_PORT_DATA_BITS = " 2503";
        public const string SERIAL_PORT_STOP_BITS = " 2504";
        public const string SERIAL_PORT_PARITY = " 2505";
        public const string SERIAL_PORT_HANDSHAKE = " 2506";

        #endregion

        #region 预留 2801 - 3000

        #endregion

        #region 终端高级选项 3001 - 4000

        /// <summary>
        /// 终端渲染模式
        /// </summary>
        [EnumDataType(typeof(RenderModeEnum))]
        public const string TERM_ADVANCE_RENDER_MODE = " 3001";
        /// <summary>
        /// 点击即可将光标移动到该位置
        /// </summary>
        public const string TERM_ADVANCE_CLICK_TO_CURSOR = " 3002";

        /// <summary>
        /// 是否启用自动完成
        /// </summary>
        public const string TERM_ADVANCE_AUTO_COMPLETION_ENABLED = " 3003";

        /// <summary>
        /// 自动换行
        /// </summary>
        public const string TERM_ADVANCE_AUTO_WRAP_MODE = " 3004";

        public const string TERM_ADVANCE_SEND_COLOR = " 3005";
        public const string TERM_ADVANCE_RECV_COLOR = " 3006";

        /// <summary>
        /// 渲染发送的数据
        /// </summary>
        public const string TERM_ADVANCE_RENDER_WRITE = " 3007";

        #endregion

        #region RawTcp 4001 - 4500

        public const string RAW_TCP_TYPE = " 4001";
        public const string RAW_TCP_ADDRESS = " 4002";
        public const string RAW_TCP_PORT = " 4003";

        #endregion

        #region 会话 - 系统监控 4501 - 5000

        //WATCH_FREQUENCY =" 4501";
        //WATCH_ADB_ENABLED =" 4502";
        //WATCH_ADB_PASSWORDS =" 4503";
        //WATCH_ADB_PROMPT =" 4504";
        //WATCH_ADB_LOGIN_TIMEOUT =" 4505";
        //WATCH_ADB_PATH =" 4506";
        //WATCH_ADB_TEMP_DIR =" 4507";

        #endregion

        #region 终端 - XYZModem 5001 - 5500

        /// <summary>
        /// 数据包重传次数
        /// </summary>
        public const string MODEM_RETRY_TIMES = " 10";

        /// <summary>
        /// 是否使用xmodel1k传输（每次传输的数据大小是1024字节）
        /// </summary>
        public const string XMODEM_XMODEM1K = " 5001";

        /// <summary>
        /// 指定XModem使用CRC校验，而不是检验和
        /// </summary>
        public const string XMODEM_RECV_CRC = " 5002";

        /// <summary>
        /// 接收的时候使用的填充字符
        /// </summary>
        public const string XMODEM_RECV_PADCHAR = " 5003";

        /// <summary>
        /// 接收的时候是否忽略填充字符
        /// </summary>
        public const string XMODEM_RECV_IGNORE_PADCHAR = " 5004";

        #endregion

        #region 终端 - 登录脚本 5501 - 6000

        //LOGIN_SCRIPT_ITEMS =" 5501";

        #endregion

        #region SFTP 10000 - 11000

        public const string SFTP_SERVER_ADDRESS = " 10000";
        public const string SFTP_SERVER_PORT = " 10001";
        public const string SFTP_USER_NAME = " 10002";
        public const string SFTP_USER_PASSWORD = " 10003";
        //SFTP_AUTH_TYPE =" 10004";
        public const string SFTP_SERVER_INITIAL_DIRECTORY = " 10005";
        public const string SFTP_CLIENT_INITIAL_DIRECTORY = " 10006";

        #endregion
    }
}

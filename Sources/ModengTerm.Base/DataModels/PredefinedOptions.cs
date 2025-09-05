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
        /// 右键点击的时候弹出上下文菜单
        /// </summary>
        ContextMenu,

        /// <summary>
        /// 右键点击的时候复制选中内容到剪切板
        /// </summary>
        FastCopyPaste,

        /// <summary>
        /// 右键点击的时候什么都不做
        /// </summary>
        None,
    }

    /// <summary>
    /// 配置类型
    /// </summary>
    public static class PredefinedOptions
    {
        #region 终端

        // Terminal 0 - 99
        public const string TERM_ROW = "term_row";
        public const string TERM_COL = "term_col";
        /// <summary>
        /// 设置滚动条可以滚动到的最大值
        /// </summary>
        public const string TERM_MAX_ROLLBACK = "term_max_scrollback";
        /// <summary>
        /// 是否禁用响铃
        /// </summary>
        public const string TERM_DISABLE_BELL = "term_disable_bell";
        public const string TERM_SIZE_MODE = "term_size_mode";
        public const string TERM_ENCODING = "term_encoding";
        /// <summary>
        /// 当鼠标右键点击的时候的行为
        /// </summary>
        [EnumDataType(typeof(RightClickActions))]
        public const string TERM_RIGHT_CLICK_ACTION = "term_right_click_action";
        public const string TERM_READ_BUFFER_SIZE = "term_read_buffer_size";
        public const string TERM_TYPE = "term_type";



        // Theme 100 - 200
        public const string THEME_FONT_FAMILY = "theme_font_family";
        public const string THEME_FONT_SIZE = "theme_font_size";
        public const string THEME_FONT_COLOR = "theme_font_color";


        public const string THEME_CURSOR_COLOR = "theme_cursor_color";
        /// <summary>
        /// 选择的主题ID
        /// </summary>
        public const string THEME_ID = "theme_id";
        /// <summary>
        /// SGR颜色表
        /// </summary>
        public const string TEHEM_COLOR_TABLE = "theme_color_table";

        /// <summary>
        /// 终端背景颜色
        /// </summary>
        public const string THEME_BACK_COLOR = "theme_back_color";

        public const string THEME_SELECTION_BACK_COLOR = "theme_selection_back_color";

        /// <summary>
        /// 背景图片透明度
        /// </summary>
        public const string THEME_BACK_IMAGE_OPACITY = "theme_back_image_opacity";
        /// <summary>
        /// base64格式保存的图片数据
        /// </summary>
        public const string THEME_BACK_IMAGE_DATA = "theme_back_image_data";
        public const string THEME_BACK_IMAGE_NAME = "theme_back_image_name";
        /// <summary>
        /// 文档内边距
        /// </summary>
        public const string THEME_PADDING = "theme_padding";


        // Server 201 - 300
        public const string SSH_SERVER_ADDR = "ssh_server_addr";
        public const string SSH_SERVER_PORT = "ssh_server_port";
        public const string SSH_USER_NAME = "ssh_user_name";
        public const string SSH_PASSWORD = "ssh_password";
        public const string SSH_PRIVATE_KEY_ID = "ssh_private_key_id";
        public const string SSH_Passphrase = "ssh_passphrase";
        public const string SSH_AUTH_TYPE = "ssh_auth_type";
        public const string SSH_PORT_FORWARDS = "ssh_port_forwards";

        #endregion

        #region 鼠标

        /// <summary>
        /// 光标闪烁速度
        /// </summary>
        public const string CURSOR_SPEED = "cursor_speed";
        public const string CURSOR_SCROLL_DELTA = "cursor_scroll_delta";
        /// <summary>
        /// 光标样式
        /// </summary>
        public const string CURSOR_STYLE = "cursor_style";

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

        public const string SERIAL_PORT_NAME = "serial_name";
        public const string SERIAL_PORT_BAUD_RATE = "serial_baudrate";
        public const string SERIAL_PORT_DATA_BITS = "serial_databits";
        public const string SERIAL_PORT_STOP_BITS = "serial_stopbits";
        public const string SERIAL_PORT_PARITY = "serial_parity";
        public const string SERIAL_PORT_HANDSHAKE = "serial_handshake";

        #endregion

        #region 终端高级选项 3001 - 4000

        /// <summary>
        /// 终端渲染模式
        /// </summary>
        [EnumDataType(typeof(RenderModeEnum))]
        public const string TERM_ADV_RENDER_MODE = "term_adv_render_mode";
        /// <summary>
        /// 点击即可将光标移动到该位置
        /// </summary>
        public const string TERM_ADV_CLICK_TO_CURSOR = "term_adv_click_to_cursor";
        /// <summary>
        /// 自动换行
        /// </summary>
        public const string TERM_ADV_AUTO_WRAP_MODE = "term_adv_auto_wrap_mode";
        public const string TERM_ADV_SEND_COLOR = "term_adv_send_color";
        public const string TERM_ADV_RECV_COLOR = "term_adv_recv_color";
        /// <summary>
        /// 渲染发送的数据
        /// </summary>
        public const string TERM_ADV_RENDER_SEND = "term_adv_render_send";

        #endregion

        #region RawTcp 4001 - 4500

        public const string RAW_TCP_TYPE = "tcp_type";
        public const string RAW_TCP_ADDRESS = "tcp_addr";
        public const string RAW_TCP_PORT = "tcp_port";

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

        #region SFTP - 常规设置

        public const string FS_GENERAL_SERVER_INITIAL_DIR = "fs_general_server_initial_dir";
        public const string FS_GENERAL_CLIENT_INITIAL_DIR = "fs_general_client_initial_dir";

        #endregion
    }
}

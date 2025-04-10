﻿using ModengTerm.Base.Enumerations.Terminal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;

namespace ModengTerm.Base.Enumerations
{
    public enum BehaviorRightClicks
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
    public enum OptionKeyEnum
    {
        #region SSH 0 - 1000

        // Terminal 0 - 99
        SSH_TERM_ROW = 0,
        SSH_TERM_COL = 1,
        SSH_TERM_TYPE = 2,
        SSH_TERM_SIZE_MODE = 3,

        /// <summary>
        /// 回放文件路径
        /// </summary>
        SSH_PLAYBACK_FILE_PATH = 4,

        /// <summary>
        /// 输出编码格式
        /// </summary>
        TERM_WRITE_ENCODING = 5,
        SSH_WRITE_BUFFER_SIZE = 6,
        SSH_READ_BUFFER_SIZE = 7,

        // Theme 100 - 200
        THEME_FONT_FAMILY = 100,
        THEME_FONT_SIZE = 101,
        THEME_FONT_COLOR = 102,
        /// <summary>
        /// 光标闪烁速度
        /// </summary>
        THEME_CURSOR_SPEED = 103,
        /// <summary>
        /// 光标样式
        /// </summary>
        THEME_CURSOR_STYLE = 104,
        THEME_CURSOR_COLOR = 106,
        /// <summary>
        /// 选择的主题ID
        /// </summary>
        THEME_ID = 107,
        TEHEM_COLOR_TABLE = 108,

        THEME_BACKGROUND_COLOR = 112,
        //THEME_BACKGROUND_EFFECT = 113,

        /// <summary>
        /// 查找高亮前景色
        /// </summary>
        THEME_FIND_HIGHLIGHT_FONTCOLOR = 114,
        /// <summary>
        /// 查找高亮背景色
        /// </summary>
        THEME_FIND_HIGHLIGHT_BACKCOLOR = 115,

        THEME_SELECTION_COLOR = 116,

        /// <summary>
        /// 背景图片透明度
        /// </summary>
        THEME_BACKGROUND_IMAGE_OPACITY = 117,
        /// <summary>
        /// base64格式保存的图片数据
        /// </summary>
        THEME_BACKGROUND_IMAGE_DATA = 118,


        /// <summary>
        /// 文档内边距
        /// </summary>
        SSH_THEME_DOCUMENT_PADDING = 501,

        /// <summary>
        /// 是否显示书签
        /// </summary>
        SSH_BOOKMARK_VISIBLE = 502,
        THEME_BOOKMARK_COLOR = 503,

        // Server 201 - 300
        SSH_ADDR = 504,
        SSH_PORT,
        SSH_USER_NAME,
        SSH_PASSWORD,
        SSH_PRIVATE_KEY_FILE,
        SSH_Passphrase,
        SSH_AUTH_TYPE,
        SSH_PORT_FORWARDS,

        #endregion

        #region 终端 1001 - 1500

        /// <summary>
        /// 设置滚动条可以滚动到的最大值
        /// </summary>
        TERM_MAX_ROLLBACK = 1001,
        /// <summary>
        /// 最多可以保存的剪贴板历史记录数据
        /// </summary>
        TERM_MAX_CLIPBOARD_HISTORY = 1002,

        MOUSE_SCROLL_DELTA = 1004,

        /// <summary>
        /// 是否禁用响铃
        /// </summary>
        TERM_DISABLE_BELL = 1005,

        /// <summary>
        /// 发送数据的编码方式
        /// </summary>
        TERM_READ_ENCODING = 1006,

        #endregion

        #region 终端行为 1501 - 2000

        /// <summary>
        /// 当鼠标右键点击的时候的行为
        /// </summary>
        [EnumDataType(typeof(BehaviorRightClicks))]
        BEHAVIOR_RIGHT_CLICK = 1501,

        #endregion

        #region 命令行 2001 - 2500

        /// <summary>
        /// 命令行可执行程序路径
        /// </summary>
        CMD_STARTUP_PATH = 2001,

        /// <summary>
        /// 命令行参数
        /// </summary>
        CMD_STARTUP_ARGUMENT = 2002,

        /// <summary>
        /// 初始目录
        /// </summary>
        CMD_STARTUP_DIR = 2003,

        /// <summary>
        /// 命令行驱动程序
        /// winpty/PesudoConsoleAPI
        /// </summary>
        CMD_DRIVER = 2004,

        #endregion

        #region 串口 2501 - 2800

        SERIAL_PORT_NAME = 2501,
        SERIAL_PORT_BAUD_RATE = 2502,
        SERIAL_PORT_DATA_BITS = 2503,
        SERIAL_PORT_STOP_BITS = 2504,
        SERIAL_PORT_PARITY = 2505,
        SERIAL_PORT_HANDSHAKE = 2506,

        #endregion

        #region 预留 2801 - 3000

        #endregion

        #region 终端高级选项 3001 - 4000

        /// <summary>
        /// 终端渲染模式
        /// </summary>
        [EnumDataType(typeof(RenderModeEnum))]
        TERM_ADVANCE_RENDER_MODE = 3001,
        /// <summary>
        /// 点击即可将光标移动到该位置
        /// </summary>
        TERM_ADVANCE_CLICK_TO_CURSOR = 3002,

        /// <summary>
        /// 是否启用自动完成
        /// </summary>
        TERM_ADVANCE_AUTO_COMPLETION_ENABLED = 3003,

        /// <summary>
        /// 自动换行
        /// </summary>
        TERM_ADVANCE_AUTO_WRAP_MODE = 3004,

        TERM_ADVANCE_SEND_COLOR = 3005,
        TERM_ADVANCE_RECV_COLOR = 3006,

        /// <summary>
        /// 渲染发送的数据
        /// </summary>
        TERM_ADVANCE_RENDER_WRITE = 3007,

        #endregion

        #region RawTcp 4001 - 4500

        RAW_TCP_TYPE = 4001,
        RAW_TCP_ADDRESS = 4002,
        RAW_TCP_PORT = 4003,

        #endregion

        #region 会话 - 系统监控 4501 - 5000

        WATCH_FREQUENCY = 4501,
        WATCH_ADB_ENABLED = 4502,
        WATCH_ADB_PASSWORDS = 4503,
        WATCH_ADB_PROMPT = 4504,
        WATCH_ADB_LOGIN_TIMEOUT = 4505,
        WATCH_ADB_PATH = 4506,
        WATCH_ADB_TEMP_DIR = 4507,

        #endregion

        #region 终端 - XYZModem 5001 - 5500

        /// <summary>
        /// 数据包重传次数
        /// </summary>
        MODEM_RETRY_TIMES = 10,

        /// <summary>
        /// 是否使用xmodel1k传输（每次传输的数据大小是1024字节）
        /// </summary>
        XMODEM_XMODEM1K = 5001,

        /// <summary>
        /// 指定XModem使用CRC校验，而不是检验和
        /// </summary>
        XMODEM_RECV_CRC = 5002,

        /// <summary>
        /// 接收的时候使用的填充字符
        /// </summary>
        XMODEM_RECV_PADCHAR = 5003,

        /// <summary>
        /// 接收的时候是否忽略填充字符
        /// </summary>
        XMODEM_RECV_IGNORE_PADCHAR = 5004,

        #endregion

        #region 终端 - 登录脚本 5501 - 6000

        LOGIN_SCRIPT_ITEMS = 5501,

        #endregion

        #region SFTP 10000 - 11000

        SFTP_SERVER_ADDRESS = 10000,
        SFTP_SERVER_PORT = 10001,
        SFTP_USER_NAME = 10002,
        SFTP_USER_PASSWORD = 10003,
        SFTP_AUTH_TYPE = 10004,
        SFTP_SERVER_INITIAL_DIRECTORY = 10005,
        SFTP_CLIENT_INITIAL_DIRECTORY = 10006,

        #endregion
    }
}

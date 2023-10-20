using ModengTerm.Base.DataModels;
using ModengTerm.Base.Definitions;
using ModengTerm.Base.Enumerations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using XTerminal.Base.DataModels;
using XTerminal.Base.Definitions;
using XTerminal.Base.Enumerations;

namespace ModengTerm.Base
{
    /// <summary>
    /// XTerminal使用的默认值
    /// </summary>
    public static class MTermConsts
    {
        #region 光标闪烁时间

        public const int LowSpeedBlinkInterval = 300;
        public const int NormalSpeedBlinkInterval = 200;
        public const int HighSpeedBlinkInterval = 100;

        public const VTCursorSpeeds DefaultCursorBlinkSpeed = VTCursorSpeeds.NormalSpeed;
        public const VTCursorStyles DefaultCursorStyle = VTCursorStyles.Line;

        #endregion

        public const int DrawFrameInterval = 40;

        public const int MIN_PORT = 1;
        public const int MAX_PORT = 65535;

        public const int TerminalColumns = 80;
        public const int TerminalRows = 24;
        public const TerminalTypeEnum DefaultTerminalType = TerminalTypeEnum.XTerm256Color;

        /// <summary>
        /// 每次读取的数据缓冲区大小
        /// </summary>
        public const int DefaultReadBufferSize = 8192;

        /// <summary>
        /// 默认打开的会话
        /// </summary>
        public static readonly XTermSession DefaultSession = new XTermSession()
        {
            ID = Guid.Empty.ToString(),
            Name = "命令行",
            Type = (int)SessionTypeEnum.Win32CommandLine,
        };

        /// <summary>
        /// 默认的输入编码方式
        /// </summary>
        public const string DefaultWriteEncoding = "UTF-8";

        public const int DefaultTerminalColumns = 80;
        public const int DefaultTerminalRows = 24;
        public const int DefaultTerminalScrollback = 99999;
        public const int DefaultMaxClipboardHistory = 50;

        /// <summary>
        /// 文档外边距
        /// </summary>
        public const int DefaultContentMargin = 5;

        /// <summary>
        /// 滚动条宽度
        /// </summary>
        public const int DefaultScrollbarWidth = 10;

        /// <summary>
        /// 默认的串口波特率列表
        /// </summary>
        public static readonly List<string> DefaultSerialPortBaudRates = new List<string>()
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

        public static readonly List<int> DefaultSerialPortDataBits = new List<int>()
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

        #region 终端字体

        public static readonly Brush Foreground = Brushes.Black;
        public static readonly double FontSize = 14;

        #endregion

        #region SFTP

        public const string SFTPServerInitialDirectory = "/";
        public static readonly string SFTPClientInitialDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory);

        /// <summary>
        /// 最多记录100个历史目录
        /// </summary>
        public const int MaxHistoryDirectory = 100;

        #endregion
    }
}

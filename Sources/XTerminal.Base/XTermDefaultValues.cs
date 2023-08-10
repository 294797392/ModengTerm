using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using XTerminal.Base.DataModels;
using XTerminal.Base.Enumerations;

namespace XTerminal.Base
{
    /// <summary>
    /// XTerminal使用的默认值
    /// </summary>
    public static class XTermDefaultValues
    {
        public const int MIN_PORT = 1;
        public const int MAX_PORT = 65535;

        /// <summary>
        /// 光标闪烁间隔时间
        /// </summary>
        public const int CURSOR_BLINK_INTERVAL = 500;
        public const int TerminalColumns = 80;
        public const int TerminalRows = 24;
        public const TerminalTypeEnum DefaultTerminalType = TerminalTypeEnum.XTerm;

        /// <summary>
        /// 每次读取的数据缓冲区大小
        /// </summary>
        public const int DefaultReadBufferSize = 256;

        /// <summary>
        /// 默认打开的会话
        /// </summary>
        public static readonly XTermSession DefaultSession = new XTermSession()
        {
            ID = Guid.Empty.ToString(),
            Name = "命令行",
            SessionType = (int)SessionTypeEnum.Win32CommandLine,
        };

        /// <summary>
        /// 默认的输入编码方式
        /// </summary>
        public const string DefaultOutputEncoding = "UTF-8";

        public const int DefaultTerminalColumns = 80;
        public const int DefaultTerminalRows = 24;

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

        /// <summary>
        /// 默认的SSH服务端口号
        /// </summary>
        public const int DefaultSSHPort = 22;

        /// <summary>
        /// 默认的光标闪烁间隔时间是500毫秒
        /// </summary>
        public const int DefaultCursorBlinkInterval = 500;

        /// <summary>
        /// 滚轮滚动一下翻两行
        /// </summary>
        public const int DefaultScrollDelta = 2;

        #region 终端字体

        public static readonly Brush Foreground = Brushes.Black;
        public static readonly double FontSize = 14;

        #endregion
    }
}

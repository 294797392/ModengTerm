using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTerminal.Base.DataModels;

namespace XTerminal.Base
{
    public static class XTermConsts
    {
        public const int MIN_PORT = 1;
        public const int MAX_PORT = 65535;

        /// <summary>
        /// 光标闪烁间隔时间
        /// </summary>
        public const int CURSOR_BLINK_INTERVAL = 500;

        public const int TerminalColumns = 80;
        public const int TerminalRows = 24;

        /// <summary>
        /// 每次读取的数据缓冲区大小
        /// </summary>
        public const int ReadBufferSize = 256;

        /// <summary>
        /// 默认打开的会话
        /// </summary>
        public static readonly XTermSession DefaultSession = new XTermSession()
        {
            ID = Guid.Empty.ToString(),
            Name = "命令行",
            Type = 0, // 0表示Windows命令行
            Column = TerminalColumns,
            Row = TerminalRows,
        };

        /// <summary>
        /// 默认的输入编码方式
        /// </summary>
        public static readonly Encoding DefaultInputEncoding = Encoding.UTF8;

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
    }
}

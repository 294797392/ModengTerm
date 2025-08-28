using ModengTerm.Base.DataModels;
using ModengTerm.Base.Definitions;
using ModengTerm.Base.Enumerations;
using ModengTerm.Base.Enumerations.Terminal;
using ModengTerm.Document;
using ModengTerm.Document.Enumerations;
using System;
using System.Collections.Generic;
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

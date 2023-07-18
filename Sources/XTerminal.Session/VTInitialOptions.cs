using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTerminal.Base;
using XTerminal.Session.Property;
using XTerminal.Sessions;

namespace XTerminal.Session
{
    /// <summary>
    /// 保存一个通道的配置信息
    /// </summary>
    public class VTInitialOptions
    {
        public static readonly VTInitialOptions Home = new VTInitialOptions()
        {
            ReadBufferSize = 16384,

            SessionType = SessionTypeEnum.SSH,
            SessionProperties = new SessionProperties()
            {
                //ServerAddress = "linux-desktop",
                //ServerPort = 22,
                //UserName = "zyf",
                //Password = "18612538605"

                ServerAddress = "ubuntu-dev",
                ServerPort = 22,
                UserName = "oheiheiheiheihei",
                Password = "18612538605",

                //ServerAddress = "tty.sdf.org",
                //ServerPort = 22,
                //UserName = "oheiheiheiheihei",
                //Password = "dycbfuS4TLvPw"
            },

            TerminalProperties = new TerminalProperties()
            {
                Type = TerminalTypeEnum.XTerm,
                Columns = 80,
                Rows = 24,
                DECPrivateAutoWrapMode = false,
            },

            CursorOption = new CursorOptions() 
            {
                Style = VTCursorStyles.Line,
                Interval = 500
            }
        };

        /// <summary>
        /// 从通道里读取数据的缓冲区大小
        /// </summary>
        public int ReadBufferSize { get; set; }

        /// <summary>
        /// 终端设置
        /// </summary>
        public TerminalProperties TerminalProperties { get; set; }

        /// <summary>
        /// 要连接的会话类型
        /// </summary>
        public SessionTypeEnum SessionType { get; set; }

        /// <summary>
        /// 会话详细信息
        /// </summary>
        public SessionProperties SessionProperties { get; set; }

        /// <summary>
        /// 光标信息
        /// </summary>
        public CursorOptions CursorOption { get; set; }

        public VTInitialOptions()
        {
        }
    }
}

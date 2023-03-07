using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTerminal.Channels;

namespace VideoTerminal.Options
{
    /// <summary>
    /// 保存一个通道的配置信息
    /// </summary>
    public class VTInitialOptions
    {
        public static readonly VTInitialOptions Home = new VTInitialOptions()
        {
            ReadBufferSize = 4096,

            ChannelType = VTChannelTypes.SSH,
            Authorition = new SSHChannelAuthorition()
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

            TerminalOption = new TerminalOptions()
            {
                Type = TerminalTypes.XTerm,
                Columns = 80,
                Rows = 24,
                DECPrivateAutoWrapMode = true
            }
        };

        /// <summary>
        /// 从通道里读取数据的缓冲区大小
        /// </summary>
        public int ReadBufferSize { get; set; }

        /// <summary>
        /// 终端设置
        /// </summary>
        public TerminalOptions TerminalOption { get; set; }

        /// <summary>
        /// 要连接的通道类型
        /// </summary>
        public VTChannelTypes ChannelType { get; set; }

        /// <summary>
        /// 连接Channel的验证信息
        /// </summary>
        public ChannelAuthorition Authorition { get; set; }

        public VTInitialOptions()
        {
        }
    }
}

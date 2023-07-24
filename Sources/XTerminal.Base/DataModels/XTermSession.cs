using DotNEToolkit.DataModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTerminal.Base;
using XTerminal.Base.DataModels.Session;
using XTerminal.Base.Enumerations;

namespace XTerminal.Base.DataModels
{
    /// <summary>
    /// 保存一个通道的配置信息
    /// </summary>
    public class XTermSession : ModelBase
    {
        /// <summary>
        /// 从通道里读取数据的缓冲区大小
        /// </summary>
        [JsonProperty("outputBufferSize")]
        public int OutputBufferSize { get; set; }

        /// <summary>
        /// 输入字符的编码方式
        /// </summary>
        [JsonProperty("inputEncoding")]
        public string InputEncoding { get; set; }

        /// <summary>
        /// 终端设置
        /// </summary>
        [JsonProperty("terminalOptions")]
        public TerminalOptions TerminalOptions { get; set; }

        /// <summary>
        /// 要连接的会话类型
        /// </summary>
        [EnumDataType(typeof(SessionTypeEnum))]
        [JsonProperty("type")]
        public int SessionType { get; set; }

        /// <summary>
        /// 会话详细信息
        /// </summary>
        [JsonProperty("authOptions")]
        public ConnectionOptions ConnectionOptions { get; set; }

        /// <summary>
        /// 光标信息
        /// </summary>
        [JsonProperty("cursorOptions")]
        public MouseOptions MouseOptions { get; set; }

        public XTermSession()
        {
            this.ConnectionOptions = new ConnectionOptions();
            this.MouseOptions = new MouseOptions();
            this.TerminalOptions = new TerminalOptions();
        }
    }
}

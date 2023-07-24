using DotNEToolkit.DataModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTerminal.Base.Enumerations;

namespace XTerminal.Base.DataModels.Session
{
    /// <summary>
    /// 连接会话的信息
    /// </summary>
    public class ConnectionOptions
    {
        [JsonProperty("keyFilePath")]
        public string KeyFilePath { get; set; }

        [JsonProperty("keyFilePassphrase")]
        public string KeyFilePassphrase { get; set; }

        [JsonProperty("userName")]
        public string UserName { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        /// <summary>
        /// SSH：服务器地址
        /// 串口：端口号
        /// </summary>
        [JsonProperty("serverAddress")]
        public string ServerAddress { get; set; }

        [JsonProperty("serverPort")]
        public int ServerPort { get; set; }

        /// <summary>
        /// 串口波特率
        /// </summary>
        [JsonProperty("baudRate")]
        public int BaudRate { get; set; }

        /// <summary>
        /// 身份验证方式
        /// </summary>
        [EnumDataType(typeof(SSHAuthTypeEnum))]
        public int SSHAuthType { get; set; }
    }
}

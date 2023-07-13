using DotNEToolkit.DataModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XTerminal.Base.DataModels
{
    /// <summary>
    /// 存储一个会话的详细信息
    /// </summary>
    public class XTermSession : ModelBase
    {
        /// <summary>
        /// 会话所属分组
        /// </summary>
        [JsonProperty("groupId")]
        public string GroupID { get; set; }

        /// <summary>
        /// 会话类型
        /// </summary>
        [JsonProperty("type")]
        public int Type { get; set; }

        /// <summary>
        /// 终端行数
        /// </summary>
        [JsonProperty("row")]
        public int Row { get; set; }

        /// <summary>
        /// 终端列数
        /// </summary>
        [JsonProperty("column")]
        public int Column { get; set; }

        /// <summary>
        /// 要连接的主机名
        /// </summary>
        [JsonProperty("host")]
        public string Host { get; set; }

        /// <summary>
        /// 要连接的主机端口号
        /// </summary>
        [JsonProperty("port")]
        public int Port { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        [JsonProperty("userName")]
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [JsonProperty("password")]
        public string Password { get; set; }

        /// <summary>
        /// 身份验证方式
        /// </summary>
        [JsonProperty("authType")]
        public int AuthType { get; set; }
    }
}

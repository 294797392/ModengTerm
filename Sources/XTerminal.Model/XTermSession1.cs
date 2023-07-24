using DotNEToolkit.DataModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XTerminal.Model
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
        /// 输入缓冲区大小
        /// </summary>
        [JsonProperty("inputBufferSize")]
        public int InputBufferSize { get; set; }

        /// <summary>
        /// 输出缓冲区大小
        /// </summary>
        [JsonProperty("outputBufferSize")]
        public int OutputBufferSize { get; set; }

        /// <summary>
        /// 输入数据的编码方式
        /// </summary>
        [JsonProperty("inputEncoding")]
        public string InputEncoding { get; set; }

        #region 终端设置

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

        #endregion

        #region 身份验证参数

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

        /// <summary>
        /// 串口的波特率
        /// </summary>
        [JsonProperty("baudRate")]
        public int BaudRate { get; set; }

        #endregion

        #region 鼠标参数

        /// <summary>
        /// 滚动一下，移动多少行
        /// </summary>
        [JsonProperty("scrollSensitivity")]
        public int ScrollSensitivity { get; set; }

        /// <summary>
        /// 光标样式
        /// </summary>
        public VTCursorStyles Style { get; set; }

        /// <summary>
        /// 闪烁间隔时间
        /// </summary>
        public int Interval { get; set; }

        #endregion
    }
}

using ModengTerm.Base.Enumerations.Ssh;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModengTerm.Base.DataModels.Ssh
{
    /// <summary>
    /// 存储端口转发状态
    /// </summary>
    public class PortForwardState
    {
        /// <summary>
        /// 源主机
        /// </summary>
        [JsonProperty("srcAddress")]
        public string SourceAddress { get; set; }

        /// <summary>
        /// 源端口号
        /// </summary>
        [JsonProperty("srcPort")]
        public int SourcePort { get; set; }

        /// <summary>
        /// 目标主机
        /// </summary>
        [JsonProperty("destAddress")]
        public string DestinationAddress { get; set; }

        /// <summary>
        /// 目标端口号
        /// </summary>
        [JsonProperty("destPort")]
        public int DestinationPort { get; set; }

        /// <summary>
        /// 是否在会话启动的时候打开
        /// </summary>
        [JsonProperty("autoOpen")]
        public bool AutoOpen { get; set; }

        public PortForwardStatusEnum Status { get; set; }

        /// <summary>
        /// 驱动里的端口转发状态
        /// </summary>
        public object DriverObject { get; set; }
    }
}
